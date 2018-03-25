namespace CodeArt.Web.WebPages
{
    using System;
    using System.Text;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    internal class Utility
    {
        internal static readonly string CONTROL_CLIENT_SCRIPT_BLOCK_IDENTIFIER = "radUploadScripts";
        internal static readonly string ENABLE_MEMORY_OPTIMIZATION_IDENTIFIER = "RadUmo";
        internal static readonly string LANGUAGE_SCRIPT_BLOCK_IDENTIFIER = "radUploadLocalization_{0}";
        internal static readonly string UNIQUE_REQUEST_QUERY_IDENTIFIER = "RadUrid";
        internal static readonly string UPLOAD_IDS_QUERY_IDENTIFIER = "rU_Ids";
        internal static readonly char UPLOAD_IDS_QUERY_SEPARATOR = ',';

        internal static void CopyBaseAttributesToInnerControl(WebControl control, WebControl child)
        {
            short tabIndex = control.TabIndex;
            string accessKey = control.AccessKey;
            Unit width = control.Width;
            Unit height = control.Height;
            try
            {
                control.AccessKey = string.Empty;
                control.TabIndex = 0;
                control.Width = Unit.Empty;
                control.Height = Unit.Empty;
                child.CopyBaseAttributes(control);
            }
            finally
            {
                control.TabIndex = tabIndex;
                control.AccessKey = accessKey;
                control.Width = width;
                control.Height = height;
            }
        }

        internal static string GetEnableMemoryOptimizationIdentifier(HttpContext context)
        {
            return context.Request.QueryString[ENABLE_MEMORY_OPTIMIZATION_IDENTIFIER];
        }

        internal static string GetJsArray(string[] arrayToConvert)
        {
            StringBuilder builder = new StringBuilder("[");
            foreach (string str in arrayToConvert)
            {
                builder.AppendFormat("'{0}',", str.Replace("'", @"\'"));
            }
            if (arrayToConvert.Length > 0)
            {
                builder.Remove(builder.Length - 1, 1);
            }
            builder.Append("]");
            return builder.ToString();
        }

        internal static string GetLocalizationScriptBlockIdentifier(string Language)
        {
            return string.Format(LANGUAGE_SCRIPT_BLOCK_IDENTIFIER, Language);
        }

        internal static string GetUniquePageIdentifier()
        {
            return Guid.NewGuid().ToString();
        }

        internal static string GetUploadUniqueIdentifier(HttpContext context)
        {
            return context.Request.QueryString[UNIQUE_REQUEST_QUERY_IDENTIFIER];
        }

        public static string[] GetValueFromViewState(StateBag viewState, string key, string[] defaultValue)
        {
            return (string[]) GetValueFromViewStateBase(viewState, key, defaultValue);
        }

        public static bool GetValueFromViewState(StateBag viewState, string key, bool defaultValue)
        {
            return (bool) GetValueFromViewStateBase(viewState, key, defaultValue);
        }

        public static int GetValueFromViewState(StateBag viewState, string key, int defaultValue)
        {
            return (int) GetValueFromViewStateBase(viewState, key, defaultValue);
        }

        public static string GetValueFromViewState(StateBag viewState, string key, string defaultValue)
        {
            return (string) GetValueFromViewStateBase(viewState, key, defaultValue);
        }

        public static Unit GetValueFromViewState(StateBag viewState, string key, Unit defaultValue)
        {
            return (Unit) GetValueFromViewStateBase(viewState, key, defaultValue);
        }

        private static object GetValueFromViewStateBase(StateBag viewState, string key, object defaultValue)
        {
            if (viewState[key] == null)
            {
                return defaultValue;
            }
            return viewState[key];
        }

        internal static bool IsInteger(string s)
        {
            if ((s.Length == 0) || (s.Length > 10))
            {
                return false;
            }
            for (int i = 0; i < s.Length; i++)
            {
                if (!char.IsNumber(s, i))
                {
                    return false;
                }
            }
            return true;
        }
    }
}

