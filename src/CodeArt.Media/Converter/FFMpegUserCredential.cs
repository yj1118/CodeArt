namespace CodeArt.Media.Converter
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Security;

    public sealed class FFMpegUserCredential
    {
        public FFMpegUserCredential(string userName, SecureString password)
        {
            this.UserName = userName;
            this.Password = password;
        }

        public FFMpegUserCredential(string userName, SecureString password, string domain) : this(userName, password)
        {
            this.Domain = domain;
        }

        public string Domain { get; private set; }

        public SecureString Password { get; private set; }

        public string UserName { get; private set; }
    }
}

