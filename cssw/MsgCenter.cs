using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace cssw
{

    public interface IMessage
    {
        void SendMessage(Object sender, Object target, int msg, Object wParam, Object lParam);
        void PostMessage(Object sender, Object target, int msg, Object wParam, Object lParam);
        void MsgProc(int msg, Object wParam, Object lParam);
    }

    public class CMsgInfo
    {
        public Object sender { get; set; }
        public Object target { get; set; }
        public int msg { get; set; }
        public Object wParam { get; set; }
        public Object lParam { get; set; }

        public MsgCenter center { get; set; }
    }

    public class MsgCenter : IMessage
    {
        public const int MSG_BASE = 0;
        public const int MSG_CONNECT_WEBSOCKET = MSG_BASE + 1;    //连接websocket服务端
        public const int MSG_STOP_WEBSOCKET = MSG_BASE + 2;       //中断连接
        public const int MSG_CONTINUE_WEBSOCKET = MSG_BASE + 3;     //自动重连
        public const int MSG_PROC_CMD = MSG_BASE + 4;           //主页处理返回数据

        public const int MSG_APP = 100;
        public const int MSG_CMD_BOARDCASE = MSG_APP + 1;          // 打板，监控涨幅大于5的个股
        public const int MSG_CMD_DATAPREPARE = MSG_APP + 2;          // 打板，监控涨幅大于5的个股


        public const int MSG_MAINWINDOW_OPENED = MSG_BASE + 1000;

        public const int MSG_SYSTEM_TEST_BASE = MSG_BASE + 2000;
        public const int MSG_SYSTEM_TEST_TIMEHEART = MSG_SYSTEM_TEST_BASE + 1;

        static Dictionary<IMessage, MsgCenter> _objControl = new Dictionary<IMessage, MsgCenter>();

        //消息订阅
        Dictionary<int, List<Object>> _dicSub = new Dictionary<int, List<Object>>();

        private MsgCenter()
        {

        }

        //public static MsgCenter GetMsgCenter()
        //{
        //    if (null == _msgcenter)
        //        _msgcenter = new MsgCenter();
        //    return _msgcenter;
        //}

        public static MsgCenter GetMsgCenter(Object obj)
        {
            if (null == obj)
            {
                if (_objControl.Count == 0)
                    return null;
                foreach (MsgCenter ob in _objControl.Values)
                {
                    return ob;
                }
            }

            if (_objControl.Keys.Contains(obj))
                return _objControl[(IMessage)obj];
            else
            {
                MsgCenter msg = new MsgCenter();
                _objControl.Add((IMessage)obj, msg);
                return msg;
            }
        }

        public static void postmsg(Object obj)
        {
            CMsgInfo info = obj as CMsgInfo;
            if (null == obj || null == info.center)
                return;
            info.center.SendMessage(info.sender, info.target, info.msg, info.wParam, info.lParam);
            //MsgCenter.GetMsgCenter().SendMessage(info.sender,info.target,info.msg, info.wParam, info.lParam);
        }

        public void UnSubscribeMsg(Object obj)
        {
            if (_dicSub.Keys.Count > 0)
            {
                if (null == obj)
                {
                    _dicSub.Clear();
                }
                foreach (List<Object> items in _dicSub.Values)
                {
                    items.Remove(obj);
                }
            }

        }

        public static void ReleaseMsgCenter(Object obj)
        {
            if (_objControl.Keys.Contains(obj))
            {
                _objControl.Remove((IMessage)obj);
            }
        }

        public void SubscribeMsg(Object o, int msg)
        {
            if (!_dicSub.Keys.Contains(msg))
            {
                _dicSub.Add(msg, new List<object>());
            }

            _dicSub[msg].Add(o);
        }


        public void RegistObject(Object o, int msgmin, int msgmax)
        {

        }
        public void MsgProc(int msg, object wParam, object lParam)
        {
            return;
        }

        public void PostMessage(object sender, object target, int msg, object wParam, object lParam)
        {
            CMsgInfo info = new CMsgInfo();
            info.sender = sender;
            info.target = target;
            info.msg = msg;
            info.wParam = wParam;
            info.lParam = lParam;
            info.center = this;

            Thread thd = new Thread(new ParameterizedThreadStart(postmsg));
            thd.IsBackground = true;
            thd.Start(info);
        }

        public void SendMessage(object sender, object target, int msg, object wParam, object lParam)
        {
            if (null != target)
            {
                (target as IMessage).MsgProc(msg, wParam, lParam);
                return;
            }

            //转发订阅消息
            sendSubMsg(msg, wParam, lParam);

        }
        private void sendSubMsg(int msg, Object wParam, Object lParam)
        {
            try
            {
                if (_dicSub.Keys.Contains(msg) && null != _dicSub[msg])
                {
                    foreach (Object obj in _dicSub[msg])
                    {
                        if (null == obj)
                        {
                            Clog.GetLog().Log("有空对象 + msg:" + msg.ToString());
                        }
                        IMessage imsg = (IMessage)obj;
                        if (null == imsg)
                        {
                            Clog.GetLog().Log("IMessage 为空!!");
                        }
                        ((IMessage)obj).MsgProc(msg, wParam, lParam);
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Write(e.ToString());
                //PluginLog.GetLog().Log(this, e.Message);
                //PluginLog.GetLog().Log(this, e.StackTrace);
                //PluginLog.GetLog().Log(this, "Msg is :" + msg.ToString());

                return;
            }

        }
    }
}
