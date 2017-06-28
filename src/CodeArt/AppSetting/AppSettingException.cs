using System;


namespace CodeArt.AppSetting
{
    public class AppSettingException : Exception
    {
        public AppSettingException(string message)
            : base(message)
        {
        }
    }
}
