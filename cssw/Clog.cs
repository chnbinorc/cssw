using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cssw
{
    public class Clog
    {
        public delegate void LogEvtHandle(String sLog);
        public event LogEvtHandle eLog;

        static Clog _log = new Clog();

        private static readonly object l_ockobj = new object();

        public Clog()
        {
            UseLog = true;
        }
        public static Clog GetLog()
        {
            return _log;
        }

        public bool UseLog { get; set; }

        public void Log(String sLog)
        {

            //lock (l_ockobj)
            {
                if (!UseLog)
                    return;
                String _str = sLog;
                

                DateTime dt = DateTime.Now;

                //if (_str.Length > 200)
                //    _str = _str.Substring(0, 200);
                String slog;
                if (_str.Length > 20000)
                    slog = String.Format("[{0}][size:{1}]", dt.ToString("yyyy-MM-dd HH:mm:ss"), _str.Length);
                else
                    slog = String.Format("[{0}][{1}]", dt.ToString("yyyy-MM-dd HH:mm:ss"), _str);
                if (null != eLog)
                    eLog(slog);
                //MsgCenter.GetMsgCenter(null).PostMessage(null, null, MsgCenter.MSG_CHECK_ROUTER, _str, null);
            }
        }
    }
}
