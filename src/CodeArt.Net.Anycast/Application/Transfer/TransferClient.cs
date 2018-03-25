using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

using CodeArt.IO;
using CodeArt.DTO;
using CodeArt.Concurrent;

using CodeArt.Net.Anycast;

namespace CodeArt.Net.Anycast
{
    /// <summary>
    /// 一个client每次仅执行一个传输任务
    /// </summary>
    public class TransferClient : IDisposable
    {
        private RtpCommunicator _communicator;

        public string CommunicatorName
        {
            get;
            private set;
        }

        public bool IsConnected
        {
            get
            {
                return _communicator != null && _communicator.IsConnected;
            }
        }

        internal TransferClient(string communicatorName, string serverIP, int serverPort)
        {
            this.CommunicatorName = communicatorName;
            InitializeCommunicator(serverIP, serverPort);
            ClientEvents.MessageReceived += OnMessageReceived;
        }

        private AutoResetEvent _single;

        private void InitializeCommunicator(string serverIP, int serverPort)
        {
            var config = new ClientConfig()
            {
                Host = serverIP,
                Port = serverPort,
                ReconnectDelayTime = 5, //间隔5秒重连一次
                ReconnectTimes = 0 //无限重连
            };
            _communicator = new RtpCommunicator(this.CommunicatorName, config);
            _communicator.Connected += OnConnected;
            _communicator.Error += OnError;
            _communicator.LastConnectError += OnLastConnectError;
            _communicator.Dropped += OnDropped;
            _communicator.Start();

            _single = new AutoResetEvent(false);
            _single.WaitOne(); //使用信号量来控制连接是同步的，当成功连接或者失败连接后，池才会返回对象给外界使用
        }

        private void DisposeCommunicator()
        {
            _communicator.Dispose();
            _communicator = null;
        }

        public void Dispose()
        {
            DisposeCommunicator();
            _single.Dispose();
            ClientEvents.MessageReceived -= OnMessageReceived;
        }

        private void OnConnected(object sender, EventArgs e)
        {
            _single.Set();
        }

        private void OnLastConnectError(object sender, AnycastEventsBase.ErrorEventArgs ea)
        {
            _single.Set();
        }

        private void OnDropped(object sender, EventArgs e)
        {

        }

        private void OnError(object sender, AnycastEventsBase.ErrorEventArgs ea)
        {

        }

        public sealed class CancelToken
        {
            public bool IsCanceled
            {
                get;
                private set;
            }

            public void Cancel()
            {
                this.IsCanceled = true;
            }

            public CancelToken()
            {
                this.IsCanceled = false;
            }

        }

        private DTObject _info;
        private CancelToken _token;
        private Future<bool> _future;
        private string _error;

        #region 保存数据

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="stream"></param>
        /// <param name="dataLength">需要上传的总数据长度，不是每种流都支持length属性，所以需要额外的指定</param>
        public void Save(DTObject info, Stream stream, long dataLength, Action<DTObject, Progress> callback, CancelToken token)
        {
            try
            {
                _info = info;
                _info["requestId"] = Guid.NewGuid();
                _token = token;

                _callback = callback;
                long completedLength = 0;
                using (var temp = SegmentReaderSlim.Borrow(SegmentSize.GetAdviceSize(dataLength)))
                {
                    var reader = temp.Item;
                    reader.Read(stream, (seg) =>
                    {
                        var content = seg.GetContent();
                        bool sent = false;
                        bool completed = UseClient((client) =>
                        {
                            var msg = CreateSaveMessage(_info, content);
                            _future = new Future<bool>(); //使用该对象监视进度
                            _future.Start();
                            client.Send(msg);
                            sent = _future.Result;
                            _future = null;
                        });

                        if (_token.IsCanceled)
                        {
                            //取消
                            UseClient((client) =>
                            {
                                var msg = CreateCancelSaveMessage(_info);
                                client.Send(msg);
                            });
                            return false;
                        }

                        if (_error != null)
                        {
                            return false;
                        }

                        if (completed && sent)
                        {
                            completedLength += content.Length;
                            callback(_info, new Progress(dataLength, completedLength, content.Length));
                            return true;
                        }
                        return false;
                    });
                }

                if (_error != null)
                {
                    throw new ApplicationException(_error);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ClearUp(); //全部执行完毕后清理
            }
        }

        private Action<DTObject, Progress> _callback;

        private Message CreateSaveMessage(DTObject info, byte[] content)
        {
            var header = DTObject.Create();
            header.SetObject("info", info);
            header.SetValue(MessageField.MessageType, MessageType.Custom);
            header.SetValue(TransferCommand.Save, true);
            return new Message(header, content);
        }

        private Message CreateCancelSaveMessage(DTObject info)
        {
            var header = DTObject.Create();
            header.SetObject("info", info);
            header.SetValue(MessageField.MessageType, MessageType.Custom);
            header.SetValue(TransferCommand.CancelSave, true);
            return new Message(header, Array.Empty<byte>());
        }

        #endregion

        #region 删除数据

        public void Delete(DTObject info)
        {
            try
            {
                _info = info;
                _info["requestId"] = Guid.NewGuid();

                _future = new Future<bool>(); //使用该对象监视进度
                _future.Start();
                UseClient((client) =>
                {
                    var msg = CreateDeleteMessage(info);
                    client.Send(msg);
                });
                _future.Wait();

                if (_error != null)
                {
                    throw new ApplicationException(_error);
                }

            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                ClearUp(); //全部执行完毕后清理
            }
           
        }

        private Message CreateDeleteMessage(DTObject info)
        {
            var header = DTObject.Create();
            header.SetObject("info", info);
            header.SetValue(MessageField.MessageType, MessageType.Custom);
            header.SetValue(TransferCommand.Delete, true);
            return new Message(header, Array.Empty<byte>());
        }

        #endregion

        #region 获取数据

        private byte[] _content = null;

        /// <summary>
        /// 加载数据到<paramref name="stream"/>
        /// </summary>
        /// <param name="info"></param>
        /// <param name="stream"></param>
        /// <param name="callback"></param>
        /// <param name="token"></param>
        public bool Load(DTObject info, Stream stream, Action<DTObject, Progress> callback, CancelToken token)
        {
            bool result = false;
            try
            {
                _info = info;
                _info["requestId"] = Guid.NewGuid();
                _token = token;

                _callback = callback;
                long completedLength = 0;
                while (true)
                {
                    bool recevied = false;
                    bool completed = UseClient((client) =>
                    {
                        var msg = CreateLoadMessage(_info);
                        _future = new Future<bool>(); //使用该对象监视进度
                        _future.Start();
                        client.Send(msg);
                        recevied = _future.Result;
                        _future = null;
                    });

                    if (_token != null && _token.IsCanceled)
                    {
                        break;
                    }

                    if (_error != null)
                    {
                        break;
                    }

                    if (completed && recevied)
                    {
                        stream.Write(_content, 0, _content.Length);

                        var dataLength = info.GetValue<long>("dataLength");
                        completedLength += _content.Length;
                        if(_callback != null)
                            _callback(_info, new Progress(dataLength, completedLength, _content.Length));

                        if (dataLength == completedLength)
                        {
                            //获取完成
                            result = true;
                            break;
                        }
                    }
                }

                if (_error != null)
                {
                    throw new ApplicationException(_error);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ClearUp(); //全部执行完毕后清理
            }

            return result;
        }

        private Message CreateLoadMessage(DTObject info)
        {
            var header = DTObject.Create();
            header.SetObject("info", info);
            header.SetValue(MessageField.MessageType, MessageType.Custom);
            header.SetValue(TransferCommand.Load, true);
            return new Message(header, Array.Empty<byte>());
        }


        #endregion

        #region 获取文件大小

        public long Size(DTObject info)
        {
            try
            {
                _info = info;
                _info["requestId"] = Guid.NewGuid();

                _future = new Future<bool>(); //使用该对象监视进度
                _future.Start();
                UseClient((client) =>
                {
                    var msg = CreateSizeMessage(info);
                    client.Send(msg);
                });
                _future.Wait();

                if (_error != null)
                {
                    throw new ApplicationException(_error);
                }

                return _info.GetValue<long>("size");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ClearUp(); //全部执行完毕后清理
            }

        }

        private Message CreateSizeMessage(DTObject info)
        {
            var header = DTObject.Create();
            header.SetObject("info", info);
            header.SetValue(MessageField.MessageType, MessageType.Custom);
            header.SetValue(TransferCommand.Size, true);
            return new Message(header, Array.Empty<byte>());
        }

        #endregion


        private void OnMessageReceived(object sender, ClientEvents.MessageReceivedEventArgs ea)
        {
            if (sender != _communicator.Context.Client) return;

            var message = ea.Message;
            if (message.Type != MessageType.Custom) return;

            var info = message.Header.GetObject("info");
            if (info.GetValue<Guid>("requestId") != _info.GetValue<Guid>("requestId")) return;

            if (message.Header.GetValue<bool>(TransferCommand.SaveResult, false))
            {
                //更新_info
                _info.Replace(info);
                _future.SetResult(true); //收到保存结果的信息
                return;
            }


            if (message.Header.GetValue<bool>(TransferCommand.LoadResult, false))
            {
                //更新_info
                _info.Replace(info);
                _content = message.Body;
                _future.SetResult(true); //收到加载的结果的信息
                return;
            }

            if (message.Header.GetValue<bool>(TransferCommand.DeleteResult, false))
            {
                //更新_info
                _info.Replace(info);
                _future.SetResult(true); //收到删除的结果
                return;
            }


            if (message.Header.GetValue<bool>(TransferCommand.SizeResult, false))
            {
                //更新_info
                _info.Replace(info);
                _future.SetResult(true); //收到获取size的结果
                return;
            }

            if (message.Header.GetValue<bool>(TransferCommand.Error, false))
            {
                _info.Replace(info);
                if (_future != null)
                {
                    _future.SetResult(false);
                }
                _error = message.Header.GetValue<string>("error");
                return;
            }

        }


        private bool UseClient(Action<AnycastClient> action)
        {
            var client = _communicator?.Context?.Client;
            if (client == null) return false;
            try
            {
                action(client);
            }
            catch (Exception ex)
            {
                //由于网络异步原因，使用client可能会出错，这里我们不抛出这类异常
                return false;
            }
            return true;
        }

        private void ClearUp()
        {
            _future = null;
            _error = null;
            _token = null;
            _info = null;
            _content = null;
        }


        /// <summary>
        /// 进度
        /// </summary>
        public struct Progress
        {
            /// <summary>
            /// 数据总长度
            /// </summary>
            public long DataLength
            {
                get;
                private set;
            }

            public long CompletedLength
            {
                get;
                private set;
            }

            /// <summary>
            /// 本次传输完成的长度
            /// </summary>
            public long CurrentLength
            {
                get;
                private set;
            }

            /// <summary>
            /// 获取进度的百分比
            /// </summary>
            /// <returns></returns>
            public float GetPercentage()
            {
                return (float)Math.Round(((double)CompletedLength / (double)DataLength), 4);
            }


            public Progress(long dataLength, long completedLength, long currentLength)
            {
                this.DataLength = dataLength;
                this.CompletedLength = completedLength;
                this.CurrentLength = currentLength;
            }
        }



    }
}
