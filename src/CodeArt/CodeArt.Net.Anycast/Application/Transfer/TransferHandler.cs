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
    public abstract class TransferHandler : IMessageHandler
    {
        public void Process(IServerSession origin, Message message)
        {
            if (message.Type != MessageType.Custom) return;

            try
            {
                if (message.Header.GetValue<bool>(TransferCommand.Save, false))
                {
                    Save(origin, message);
                    return;
                }

                if (message.Header.GetValue<bool>(TransferCommand.CancelSave, false))
                {
                    CancelSave(origin, message);
                    return;
                }

                if (message.Header.GetValue<bool>(TransferCommand.Load, false))
                {
                    Load(origin, message);
                    return;
                }

                if (message.Header.GetValue<bool>(TransferCommand.Size, false))
                {
                    Size(origin, message);
                    return;
                }


                if (message.Header.GetValue<bool>(TransferCommand.Delete, false))
                {
                    Delete(origin, message);
                    return;
                }
            }
            catch (Exception ex)
            {
                SendError(origin, message, ex);
            }
        }

        private void SendError(IServerSession origin, Message message, Exception exception)
        {
            var info = message.Header.GetObject("info");
            var msg = CreateErrorMessage(info, exception);
            var o = origin as RemoteSession;
            o.Send(msg);
        }

        private Message CreateErrorMessage(DTObject info, Exception exception)
        {
            var header = DTObject.Create();
            header.SetObject("info", info);
            header.SetValue(MessageField.MessageType, MessageType.Custom);
            header.SetValue(TransferCommand.Error, true);
            header.SetValue("error", exception.Message);
            return new Message(header, Array.Empty<byte>());
        }

        #region 保存

        private void Save(IServerSession origin, Message message)
        {
            //保存内容
            var info = message.Header.GetObject("info");
            var content = message.Body;
            Save(info, content);

            //返回结果
            var msg = CreateSaveMessage(info);
            var o = origin as RemoteSession;
            o.Send(msg);
        }


        private Message CreateSaveMessage(DTObject info)
        {
            var header = DTObject.Create();
            header.SetObject("info", info);
            header.SetValue(MessageField.MessageType, MessageType.Custom);
            header.SetValue(TransferCommand.SaveResult, true);
            return new Message(header, Array.Empty<byte>());
        }

        /// <summary>
        /// 保存上传的内容
        /// </summary>
        /// <param name="info"></param>
        /// <param name="content"></param>
        protected abstract void Save(DTObject info, byte[] content);

        #endregion

        #region 取消保存

        private void CancelSave(IServerSession origin, Message message)
        {
            //保存内容
            var info = message.Header.GetObject("info");
            CancelSave(info);
        }

        protected abstract void CancelSave(DTObject info);


        #endregion

        #region 加载

        private void Load(IServerSession origin, Message message)
        {
            //保存内容
            var info = message.Header.GetObject("info");
            var data = Load(info);

            //返回结果
            info["dataLength"] = data.length;
            var msg = CreateLoadMessage(info, data.content);
            var o = origin as RemoteSession;
            o.Send(msg);
        }

        /// <summary>
        /// 加载内容
        /// </summary>
        /// <param name="info"></param>
        /// <param name="content"></param>
        protected abstract (byte[] content, long length) Load(DTObject info);

        private Message CreateLoadMessage(DTObject info, byte[] content)
        {
            var header = DTObject.Create();
            header.SetObject("info", info);
            header.SetValue(MessageField.MessageType, MessageType.Custom);
            header.SetValue(TransferCommand.LoadResult, true);
            return new Message(header, content);
        }

        #endregion

        #region 删除

        private void Delete(IServerSession origin, Message message)
        {
            //保存内容
            var info = message.Header.GetObject("info");
            Delete(info);

            //返回结果
            var msg = CreateDeleteMessage(info);
            var o = origin as RemoteSession;
            o.Send(msg);
        }

        private Message CreateDeleteMessage(DTObject info)
        {
            var header = DTObject.Create();
            header.SetObject("info", info);
            header.SetValue(MessageField.MessageType, MessageType.Custom);
            header.SetValue(TransferCommand.DeleteResult, true);
            return new Message(header, Array.Empty<byte>());
        }

        protected abstract void Delete(DTObject info);

        #endregion


        #region 获取大小

        private void Size(IServerSession origin, Message message)
        {
            //保存内容
            var info = message.Header.GetObject("info");
            var size = Size(info);
            info["size"] = size;

            //返回结果
            var msg = CreateSizeMessage(info);
            var o = origin as RemoteSession;
            o.Send(msg);
        }

        private Message CreateSizeMessage(DTObject info)
        {
            var header = DTObject.Create();
            header.SetObject("info", info);
            header.SetValue(MessageField.MessageType, MessageType.Custom);
            header.SetValue(TransferCommand.SizeResult, true);
            return new Message(header, Array.Empty<byte>());
        }

        protected abstract long Size(DTObject info);

        #endregion

    }
}
