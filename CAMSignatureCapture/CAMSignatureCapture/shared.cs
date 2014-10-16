using System;
using System.Text;
using System.Runtime.InteropServices;

using Intermec.DeviceManagement.SmartSystem;

namespace CAMSignatureCapture
{
    public class SSAPI
    {
        // Methods
        public static string Get(string GetXML)
        {
            return Get(GetXML, 500);
        }

        public static string Get(string GetXML, int timeout)
        {
            int capacity = GetXML.Length * 2;
            StringBuilder responseXML = new StringBuilder(capacity);
            Get(GetXML, responseXML, ref capacity, timeout);
            if (capacity > (GetXML.Length * 2))
            {
                responseXML = new StringBuilder(capacity);
                Get(GetXML, responseXML, ref capacity, timeout);
            }
            return responseXML.ToString();
        }

        [DllImport("ITCSSAPI.dll", EntryPoint = "ITCSSGet")]
        public static extern uint Get(string GetXML, StringBuilder ResponseXML, ref int ResponseSize, int Timeout);
        public static string Set(string SetXML)
        {
            return Set(SetXML, 500);
        }

        public static string Set(string SetXML, int timeout)
        {
            int capacity = SetXML.Length * 2;
            StringBuilder responseXML = new StringBuilder(capacity);
            Set(SetXML, responseXML, ref capacity, timeout);
            if (capacity > (SetXML.Length * 2))
            {
                responseXML = new StringBuilder(capacity);
                Set(SetXML, responseXML, ref capacity, timeout);
            }
            return responseXML.ToString();
        }

        [DllImport("ITCSSAPI.dll", EntryPoint = "ITCSSSet")]
        public static extern uint Set(string SetXML, StringBuilder ResponseXML, ref int ResponseSize, int Timeout);

        // Nested Types
        public enum ErrorCode : uint
        {
            CONN_NOT_OPENED = 0xc16e0027,
            CREATE_DOM_FAILED = 0xc16e0029,
            FUNCTION_UNAVAILABLE = 0xc16e0033,
            INVALID_EVENT = 0xc16e002f,
            INVALID_PARM = 0xc16e0032,
            MALFORMED_XML = 0xc16e0031,
            MISSING_REQUIRED_PARM = 0xc16e002e,
            MSG_ID_IN_USE = 0xc16e0034,
            NO_MORE_CONNECTION_ALLOWED = 0xc16e002c,
            NOT_GROUP_MEMBER = 0xc16e0035,
            OPEN_CONN_FAILED = 0xc16e0022,
            OPEN_FILE_FAILED = 0xc16e002a,
            OPERATION_FAILED = 0xc16e0021,
            RCV_BUFFER_TOO_SMALL = 0xc16e0026,
            RCV_RESP_FAILED = 0xc16e0024,
            RCV_TIMEOUT = 0xc16e0025,
            READ_FILE_FAILED = 0xc16e0028,
            SEND_REQ_FAILED = 0xc16e0023,
            SYS_RSC_ALLOC_FAILED = 0xc16e002d,
            TIMEOUT = 0xc16e0030,
            XML_MATCH_NOT_FOUND = 0xc16e002b
        }
    }

    public class SmartSystemsAPIWrapper
    {
        // Fields
        private const string XML_Camera_FocusMode = "<Subsystem Name=\"Data Collection\"><Group Name=\"Scanners\" Instance=\"18\"><Group Name=\"Imager Settings\"><Group Name=\"Viewfinder Properties\"><Field Name=\"Focus mode\">{0}</Field></Group></Group></Group></Subsystem>";
        private const string XML_EnableLocationServices = "<Subsystem Name=\"Location Services\"><Group Name=\"Server\"><Field Name=\"Enable Server\">{0}</Field></Group></Subsystem>";
        private const string XML_Imager_AimerMode = "<Subsystem Name=\"Data Collection\"><Group Name=\"Scanners\" Instance=\"0\"><Group Name=\"Imager Settings\"><Group Name=\"Custom\"><Field Name=\"Aimer Mode\">{0}</Field></Group></Group></Group></Subsystem>";
        private const string XML_Imager_IllumLevel = "<Subsystem Name=\"Data Collection\"><Group Name=\"Scanners\" Instance=\"0\"><Group Name=\"Imager Settings\"><Group Name=\"Custom\"><Field Name=\"Illumination Level\">{0}</Field></Group></Group></Group></Subsystem>";

        // Methods
        private static bool Exists(string xml)
        {
            string getXML = string.Format(xml, string.Empty, string.Empty, string.Empty);
            int capacity = getXML.Length * 2;
            StringBuilder responseXML = new StringBuilder(capacity);
            return (SSAPI.Get(getXML, responseXML, ref capacity, 0x3e8) == 0);
        }

        private static string Get(string xml, params object[] args)
        {
            string str = null;
            string getXML = string.Format(xml, args);
            int capacity = getXML.Length * 2;
            StringBuilder responseXML = new StringBuilder(capacity);
            if (SSAPI.Get(getXML, responseXML, ref capacity, 0x3e8) == 0)
            {
                int index = xml.IndexOf('{');
                str = responseXML.ToString();
                if ((index >= 0) && (xml.IndexOf('}') >= 0))
                {
                    str = str.Substring(index);
                    str = str.Substring(0, str.IndexOf('<'));
                }
            }
            return str;
        }

        private static void Set(string xml, params object[] args)
        {
            string setXML = string.Format(xml, args);
            int capacity = setXML.Length * 2;
            StringBuilder responseXML = new StringBuilder(capacity);
            SSAPI.Set(setXML, responseXML, ref capacity, 0x3e8);
        }

        // Properties
        public static FocusMode Focus_Mode
        {
            get
            {
                try
                {
                    return (FocusMode)int.Parse(Get("<Subsystem Name=\"Data Collection\"><Group Name=\"Scanners\" Instance=\"18\"><Group Name=\"Imager Settings\"><Group Name=\"Viewfinder Properties\"><Field Name=\"Focus mode\">{0}</Field></Group></Group></Group></Subsystem>", new object[] { string.Empty }));
                }
                catch
                {
                    return FocusMode.Manual;
                }
            }
            set
            {
                object[] args = new object[] { ((int)value).ToString() };
                Set("<Subsystem Name=\"Data Collection\"><Group Name=\"Scanners\" Instance=\"18\"><Group Name=\"Imager Settings\"><Group Name=\"Viewfinder Properties\"><Field Name=\"Focus mode\">{0}</Field></Group></Group></Group></Subsystem>", args);
            }
        }

        public static int IlluminationLevel
        {
            get
            {
                return int.Parse(Get("<Subsystem Name=\"Data Collection\"><Group Name=\"Scanners\" Instance=\"0\"><Group Name=\"Imager Settings\"><Group Name=\"Custom\"><Field Name=\"Illumination Level\">{0}</Field></Group></Group></Group></Subsystem>", new object[] { string.Empty }));
            }
            set
            {
                Set("<Subsystem Name=\"Data Collection\"><Group Name=\"Scanners\" Instance=\"0\"><Group Name=\"Imager Settings\"><Group Name=\"Custom\"><Field Name=\"Illumination Level\">{0}</Field></Group></Group></Group></Subsystem>", new object[] { value.ToString() });
            }
        }

        public static bool LocationServicesEnabled
        {
            get
            {
                try
                {
                    return (int.Parse(Get("<Subsystem Name=\"Location Services\"><Group Name=\"Server\"><Field Name=\"Enable Server\">{0}</Field></Group></Subsystem>", new object[] { string.Empty })) == 1);
                }
                catch
                {
                    return false;
                }
            }
            set
            {
                Set("<Subsystem Name=\"Location Services\"><Group Name=\"Server\"><Field Name=\"Enable Server\">{0}</Field></Group></Subsystem>", new object[] { value ? 1 : 0 });
            }
        }

        public static bool LocationServicesInstalled
        {
            get
            {
                return !string.IsNullOrEmpty(Get("<Subsystem Name=\"Location Services\"><Group Name=\"Server\"><Field Name=\"Enable Server\">{0}</Field></Group></Subsystem>", new object[] { string.Empty }));
            }
        }

        // Nested Types
        public enum FocusMode
        {
            Manual,
            Continuous_Autofocus,
            Single_Shot_Autofocus,
            Macro,
            Extended
        }
    }
}