using CodeArt.Concurrent.Pattern;
using CodeArt.Util;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Net.Anycast
{
    internal sealed class SessionManager
    {
        private AnycastServer _server;

        public SessionManager(AnycastServer server)
        {
            _server = server;
        }

        #region 组播

        /// <summary>
        /// 组播地址 -> 多个会话对象
        /// </summary>
        private MultiDictionary<string, IServerSession> _multicastSessions = new MultiDictionary<string, IServerSession>(true);

        /// <summary>
        /// 会话对象 -> 组播地址
        /// </summary>
        private MultiDictionary<IServerSession, string> _reverseMulticastSessions = new MultiDictionary<IServerSession, string>(true);

        /// <summary>
        /// 加入组播
        /// </summary>
        /// <param name="multicastAddress"></param>
        /// <param name="session"></param>
        public bool Join(string multicastAddress, IServerSession session)
        {
            lock (_multicastSessions)
            {
                if (_reverseMulticastSessions.Contains(session, multicastAddress)) return false;

                _multicastSessions.Add(multicastAddress, session);
                _reverseMulticastSessions.Add(session, multicastAddress);
            }
            return true;
        }

        /// <summary>
        /// 离开组播
        /// </summary>
        /// <param name="multicastAddress"></param>
        /// <param name="session"></param>
        public bool Leave(string multicastAddress, IServerSession session)
        {
            lock (_multicastSessions)
            {
                if (!_reverseMulticastSessions.Contains(session, multicastAddress)) return false;

                _multicastSessions.RemoveValue(multicastAddress, session);
                _reverseMulticastSessions.RemoveValue(session, multicastAddress);
            }
            return true;
        }

        public IServerSession[] GetMulticastSessions(string multicastAddress)
        {
            IServerSession[] sessions = null;
            lock (_multicastSessions)
            {
                var temp = _multicastSessions.GetValues(multicastAddress);
                sessions = temp != null ? temp.ToArray() : Array.Empty<IServerSession>();
            }
            return sessions;
        }

        public string[] GetMulticastAddresses(IServerSession session)
        {
            string[] multicastAddresses = null;
            lock (_reverseMulticastSessions)
            {
                var temp = _reverseMulticastSessions.GetValues(session);
                multicastAddresses = temp != null ? temp.ToArray() : Array.Empty<string>();
            }
            return multicastAddresses;
        }

        #endregion

        #region 回话对象的集合

        /// <summary>
        /// 单播地址 -> 多个会话对象
        /// </summary>
        private MultiDictionary<string, IServerSession> _sessions = new MultiDictionary<string, IServerSession>(false);

        public void Add(IServerSession session)
        {
            lock (_sessions)
            {
                var unicastAddress = session.UnicastAddress;
                _sessions.Add(unicastAddress, session);
            }

            ServerEvents.AsyncRaiseClientConnected(_server, session);
        }

        public bool Remove(IServerSession session)
        {
            if (session == null) return false;

            bool exist = false;
            lock (_sessions)
            {
                var unicastAddress = session.UnicastAddress;
                exist = _sessions.RemoveValue(unicastAddress, session);
                if (exist)
                {
                    LeaveAll(session);
                }
            }

            if(exist)
            {
                session.Close();
                ServerEvents.AsyncRaiseClientDisconnectedEvent(_server, session);
            }

            return exist;
        }



        private void LeaveAll(IServerSession session)
        {
            var multicastAddresses = GetMulticastAddresses(session);
            foreach (var multicastAddress in multicastAddresses)
            {
                this.Leave(multicastAddress, session);
            }
        }

        /// <summary>
        /// 找到单播地址对应的客户端，由于当客户端刚刚掉线时，session有可能来不及释放，这时候客户端再次登录，就有可能产生多个满足条件的客户端
        /// </summary>
        /// <param name="unicastAddress"></param>
        /// <returns></returns>
        public IServerSession[] GetSessions(string unicastAddress)
        {
            IServerSession[] sessions = null;
            lock (_sessions)
            {
                var temp = _sessions.GetValues(unicastAddress);
                sessions = temp != null ? temp.ToArray() : Array.Empty<IServerSession>();
            }
            return sessions;
        }

        public RemoteSession GetSession(IChannel channel)
        {
            var sessions = this.GetSessions(RemoteSession.GetUnicastAddress(channel)).OfType<RemoteSession>();
            foreach(var session in sessions)
            {
                if (session.Channel == channel) return session;
            }
            return null;
        }

        /// <summary>
        /// 是否存在单播地址对应的session
        /// </summary>
        /// <param name="unicastAddress"></param>
        /// <returns></returns>
        public bool ExistSession(string unicastAddress)
        {
            lock (_sessions)
            {
                return _sessions.ContainsKey(unicastAddress);
            }
        }

        #endregion

        /// <summary>
        /// 获得地址涉及到的session，有可能是组播的也有可能是单播的
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public IServerSession[] GetAnySessions(string address)
        {
            var sessions = this.GetMulticastSessions(address);
            if (sessions.Length > 0) return sessions;
            return GetSessions(address).ToArray();
        }

        public IServerSession[] GetAllSessions()
        {
            lock (_sessions)
            {
                return _sessions.GetValues().ToArray();
            }
        }

    }

}
