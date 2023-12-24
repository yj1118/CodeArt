using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Threading;


namespace CodeArt.OPC
{
    public static class OPCClientEvents
    {
        #region �Ự�Ѵ򿪵��¼�

        public class SessionConnectedArgs : EventArgs
        {
            public string SessionName
            {
                get;
                private set;
            }

            public SessionConnectedArgs(string sessionName)
            {
                this.SessionName = sessionName;
            }
        }

        public delegate void SessionConnectedHandler(object sender, SessionConnectedArgs arg);

        public static event SessionConnectedHandler SessionConnected;

        internal static void RaiseSessionConnected(object sender, SessionConnectedArgs arg)
        {
            if (SessionConnected != null)
            {
                SessionConnected(sender, arg);
            }
        }

        #endregion

        #region �Ự�ѹرյ��¼�

        public class SessionDisconnectedArgs : EventArgs
        {
            public string SessionName
            {
                get;
                private set;
            }

            public SessionDisconnectedArgs(string sessionName)
            {
                this.SessionName = sessionName;
            }
        }

        public delegate void SessionDisconnectedHandler(object sender, SessionDisconnectedArgs arg);

        public static event SessionDisconnectedHandler SessionDisconnected;

        internal static void RaiseSessionDisconnected(object sender, SessionDisconnectedArgs arg)
        {
            if (SessionDisconnected != null)
            {
                SessionDisconnected(sender, arg);
            }
        }

        #endregion
    }
}
