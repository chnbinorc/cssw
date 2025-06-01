using System;
using System.Collections.Generic;
using System.Linq;
using WebSocketSharp;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Diagnostics;

namespace cssw
{
    public class WSControls : IMessage
    {
        Clog _log = Clog.GetLog();
        WebSocket _websocket = new WebSocket("ws://127.0.0.1:16002/");
        MsgCenter _center = null;
        bool _noconnect = false;
        public WSControls()
        {

            _center = MsgCenter.GetMsgCenter(this);
            _center.SubscribeMsg(this, MsgCenter.MSG_CONNECT_WEBSOCKET);
            _center.SubscribeMsg(this, MsgCenter.MSG_STOP_WEBSOCKET);
            _center.SubscribeMsg(this, MsgCenter.MSG_CONTINUE_WEBSOCKET);
            _websocket.OnClose += _websocket_OnClose;
            _websocket.OnError += _websocket_OnError;
            _websocket.OnMessage += _websocket_OnMessage;
            _websocket.OnOpen += _websocket_OnOpen;

        }

        public bool isAlive()
        {
            return _websocket.IsAlive;
        }

        public void Send(string cmd)
        {
            if (_websocket.IsAlive)
            {
                _websocket.Send(cmd);
            }
        }

        private void _websocket_OnOpen(object sender, EventArgs e)
        {
            _log.Log("服务器已连接");
        }

        private void _websocket_OnMessage(object sender, MessageEventArgs e)
        {
            JObject jobj = (JObject)JsonConvert.DeserializeObject(e.Data);
            string name = (string)jobj.GetValue("name");
            string cmd = (string)jobj.GetValue("cmd");
            string data = (string)jobj.GetValue("data");
            //JObject obj = (JObject)JsonConvert.DeserializeObject((string)jobj.GetValue("data"));
            PostMessage(null, null, MsgCenter.MSG_PROC_CMD, name, data);
            
            //switch (name)
            //{
            //    case "BoardCase":
            //        PostMessage(null, null, MsgCenter.MSG_CMD_BOARDCASE, name, data);
            //        break;
            //}
            
        }

        private void _websocket_OnError(object sender, ErrorEventArgs e)
        {
            _log.Log("发生错误: " + e.Message);
        }

        private void _websocket_OnClose(object sender, CloseEventArgs e)
        {
            _log.Log("服务器已断开");
            if (!_noconnect)
                PostMessage(null, null, MsgCenter.MSG_CONTINUE_WEBSOCKET, null, null);
        }

        public void MsgProc(int msg, object wParam, object lParam)
        {
            switch (msg)
            {
                case MsgCenter.MSG_CONNECT_WEBSOCKET:
                    if (!_websocket.IsAlive)
                    {
                        _noconnect = false;
                        _websocket.Connect();
                    }
                    break;
                case MsgCenter.MSG_STOP_WEBSOCKET:
                    _noconnect = true;
                    _websocket.Close();
                    break;
                case MsgCenter.MSG_CONTINUE_WEBSOCKET:
                    while (true)
                    {
                        Thread.Sleep(2000);
                        _log.Log("尝试重新连接服务器");
                        if (!_websocket.IsAlive)
                        {
                            _noconnect = false;
                            _websocket.Connect();
                        }
                        else
                        {
                            break;
                        }
                        
                    }
                    break;


            }
        }

        public void PostMessage(object sender, object target, int msg, object wParam, object lParam)
        {
            MsgCenter.GetMsgCenter(null).PostMessage(sender, target, msg, wParam, lParam);
        }

        public void SendMessage(object sender, object target, int msg, object wParam, object lParam)
        {
            MsgCenter.GetMsgCenter(null).SendMessage(sender, target, msg, wParam, lParam);
        }
    }
}
