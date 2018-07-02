namespace CodeArt.Web.WebPages
{
    using System;
    using System.Configuration;

    internal class Configuration
    {
        private static int _chunkSize = -1;
        private static string _tempFolder = null;
        private static readonly int DEFAULT_CHUNK_SIZE = 0x2000;
        private static readonly string DEFAULT_TEMP_FOLDER = string.Empty;
        public static readonly int MAX_CHUNK_SIZE = 0xf4240;

        public static int ChunkSize
        {
            get
            {
                if (_chunkSize < 0)
                {
                    string s = ConfigurationSettings.AppSettings["Telerik.RadUpload.ChunkSize"];
                    _chunkSize = ((s != null) && Utility.IsInteger(s)) ? int.Parse(s) : DEFAULT_CHUNK_SIZE;
                    _chunkSize = Math.Min(MAX_CHUNK_SIZE, _chunkSize);
                }
                return _chunkSize;
            }
        }

        public static bool IsDefaultChunkSize
        {
            get
            {
                return (ChunkSize == DEFAULT_CHUNK_SIZE);
            }
        }

        public static bool IsDefaultTempFolder
        {
            get
            {
                return (TempFolder == DEFAULT_TEMP_FOLDER);
            }
        }

        public static string TempFolder
        {
            get
            {
                if (_tempFolder == null)
                {
                    string str = ConfigurationSettings.AppSettings["Telerik.RadUpload.TempFolder"];
                    _tempFolder = (str != null) ? str : DEFAULT_TEMP_FOLDER;
                }
                return _tempFolder;
            }
        }
    }
}

