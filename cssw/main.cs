using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebSocketSharp;

namespace cssw
{
    public partial class main : Form,IMessage
    {
        DataTable _dt = new DataTable();
        WSControls _controle = new WSControls();
        Clog _log = Clog.GetLog();
        Dictionary<string, CStockPriceTimes> _db = new Dictionary<string, CStockPriceTimes>();  //所有传回的数据
        bool _flag_board = true;
        bool _flag_send = false;


        public main()
        {
            InitializeComponent();
            init();
        }

        public void init()
        {
            
            btnConnect.Click += btnConnect_Click;
            btnSend.Click += BtnSend_Click;
            btnStopBoard.Click += BtnStopBoard_Click;
            btnPrepare.Click += BtnPrepare_Click;
            _log.eLog += _log_eLog;
            initAllDataTable();

            MsgCenter.GetMsgCenter(_controle).SubscribeMsg(this, MsgCenter.MSG_CMD_BOARDCASE);
            MsgCenter.GetMsgCenter(_controle).SubscribeMsg(this, MsgCenter.MSG_PROC_CMD);
            MsgCenter.GetMsgCenter(_controle).SubscribeMsg(this, MsgCenter.MSG_CMD_DATAPREPARE);

            this.dataGridView1.CellClick += DataGridView1_CellClick;
            this.dataGridView2.CellClick += DataGridView2_CellClick;
            this.SizeChanged += Main_SizeChanged;
            this.Activated += Main_Activated;
            

        }

        private void DataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }
            DataGridViewCell cell = (DataGridViewCell)this.dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex];
            Clipboard.SetDataObject(cell.Value, true);
            Debug.WriteLine(cell.Value);
        }

        private void BtnPrepare_Click(object sender, EventArgs e)
        {
            PostMessage(null, null, MsgCenter.MSG_CMD_DATAPREPARE, null, null);
        }

        private void Main_Activated(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }

        private void Main_SizeChanged(object sender, EventArgs e)
        {
            panel3.Left = panel5.Left = 0;
            panel3.Top = panel5.Top = 0;
            panel3.Width = panel5.Width = tabPage1.Width / 2;
            panel4.Left = panel3.Width;
            panel6.Left = panel5.Width;
            panel4.Top = 0;
            panel4.Width = tabPage1.Width / 2;
            panel6.Top = 0;
            panel6.Width = tabPage1.Width / 2;

            panel3.Height = tabPage1.Height;
            panel4.Height = tabPage1.Height;
            panel5.Height = tabPage1.Height;
            panel6.Height = tabPage1.Height;
            //panel3.Width = panel4.Width = tabPage1.Width / 2;

            tabMainDetail.Left = tabConcept.Left = 0;
            tabMainDetail.Top = 0;
            tabMainDetail.Width = tabConcept.Width = panel6.Width;
            tabMainDetail.Height = tabConcept.Top = tabConcept.Height = panel6.Height / 2;
        }

        private void BtnStopBoard_Click(object sender, EventArgs e)
        {
            _flag_board = false;
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }
            DataGridViewCell cell = (DataGridViewCell)this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
            Clipboard.SetDataObject(cell.Value,true);
            Debug.WriteLine(cell.Value); 
        }

        private void _log_eLog(string sLog)
        {
            Debug.WriteLine(sLog);
        }

        void initDataTable(DataTable dt)
        {
            DataColumn clm = dt.Columns.Add("code", typeof(string));
            dt.Columns.Add("名称", typeof(string));
            dt.Columns.Add("开盘价", typeof(float));
            dt.Columns.Add("收盘价", typeof(float));
            dt.Columns.Add("价格", typeof(float));
            dt.Columns.Add("最高价", typeof(float));
            dt.Columns.Add("最低价", typeof(float));
            dt.Columns.Add("成交量", typeof(float));
            dt.Columns.Add("成交金额", typeof(float));
            dt.Columns.Add("实体涨幅", typeof(float));
            dt.Columns.Add("涨幅", typeof(float));
            dt.Columns.Add("时间", typeof(string));
            dt.Columns.Add("总股本", typeof(float));
            dt.Columns.Add("流通股本", typeof(float));
            dt.Columns.Add("行业代码", typeof(string));
            dt.Columns.Add("行业名称", typeof(string));
            dt.Columns.Add("行业名称拼音", typeof(string));
            dt.Columns.Add("异动涨幅", typeof(int));
            dt.Columns.Add("换手率", typeof(float));
            dt.Columns.Add("平均换手率", typeof(float));
            dt.Columns.Add("价格区间", typeof(int));

            dt.PrimaryKey = new DataColumn[] { clm };
        }

        void initAllDataTable()
        {
            initDataTable(_dt);
        }

        private void BtnSend_Click(object sender, EventArgs e)
        {
            //_controle.Send(getExcuteCmd("CBoardCase", "run"));
            //if (!_flag_send){
            //    Debug.WriteLine("数据未准备好");
            //    return;
            //}
            _flag_board = true;
            PostMessage(null, null, MsgCenter.MSG_CMD_BOARDCASE, null, null);
            
        }

        private string getExcuteCmd(string name,string cmd,string data)
        {
            JObject jb = new JObject();
            jb["name"] = name;
            jb["command"] = cmd;
            jb["data"] = data;
            return jb.ToString();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            PostMessage(null, null, MsgCenter.MSG_CONNECT_WEBSOCKET, null, null);
        }

        public void SendMessage(object sender, object target, int msg, object wParam, object lParam)
        {
            MsgCenter.GetMsgCenter(null).SendMessage(sender, target, msg, wParam, lParam);
        }

        public void PostMessage(object sender, object target, int msg, object wParam, object lParam)
        {
            MsgCenter.GetMsgCenter(null).PostMessage(sender, target, msg, wParam, lParam);
        }

        void insertRow(DataTable des,DataTable src)
        {
            List<DataRow> tmp = new List<DataRow>();
            foreach (DataRow row in src.Rows)
            {
                string sql = string.Format(row["code"].ToString());
                DataRow find = des.Rows.Find(sql);
                if (null != find)
                {
                    continue;
                }
                else
                {
                    tmp.Add(row);
                }
            }

            foreach (DataRow item in tmp)
            {
                string ssql = string.Format("行业代码 = '{0}'", item["行业代码"]);
                DataRow[] finds = des.Select(ssql);

                DataRow clone = des.NewRow();
                clone["code"] = item["code"];
                clone["名称"] = item["名称"];
                clone["开盘价"] = item["开盘价"];
                clone["收盘价"] = item["收盘价"];
                clone["价格"] = item["价格"];
                clone["最高价"] = item["最高价"];
                clone["最低价"] = item["最低价"];
                clone["成交量"] = item["成交量"];
                clone["成交金额"] = item["成交金额"];
                clone["实体涨幅"] = item["实体涨幅"];
                clone["涨幅"] = item["涨幅"];
                clone["时间"] = item["时间"];
                clone["总股本"] = item["总股本"];
                clone["流通股本"] = item["流通股本"];
                clone["行业代码"] = item["行业代码"];
                clone["行业名称"] = item["行业名称"];
                clone["行业名称拼音"] = item["行业名称拼音"];
                clone["异动涨幅"] = item["异动涨幅"];
                clone["换手率"] = item["换手率"];
                clone["平均换手率"] = item["平均换手率"];
                clone["价格区间"] = item["价格区间"];

                if (finds.Length == 0)
                {
                    des.Rows.Add(clone);
                }
                else
                {
                    int pos = des.Rows.IndexOf(finds[finds.Length - 1]);
                    
                    des.Rows.InsertAt(clone, pos);
                }
                
            }
        }

        void updateData(DataTable src,DataTable dt)
        {
            if (null == src || null == dt || src.Rows.Count == 0 || dt.Rows.Count == 0)
            {
                return;
            }

            foreach (DataRow row in src.Rows)
            {
                DataRow find = dt.Rows.Find(row["code"]);
                if (null != find)
                {
                    //row.BeginEdit();
                    row["价格"] = find["价格"];
                    row["最高价"] = find["最高价"];
                    row["最低价"] = find["最低价"];
                    row["成交量"] = find["成交量"];
                    row["成交金额"] = find["成交金额"];
                    row["实体涨幅"] = find["实体涨幅"];
                    row["涨幅"] = find["涨幅"];
                    row["时间"] = find["时间"];
                    row["换手率"] = find["换手率"];
                    //row.EndEdit();
                }
            }
        }

        void updateAllData()
        {
            updateData(this.dataGridView1.DataSource as DataTable, _dt);
            updateData(this.dataGridView2.DataSource as DataTable, _dt);

        }

        void filterAllData()
        {
            if (_dt.Rows.Count > 0)
            {
                label1.Text = string.Format("总数:{0}", _dt.Rows.Count);
                DataRow[] rows = _dt.Select("涨幅 > 0");
                label2.Text = string.Format("上涨:{0}", rows.Count());
                label3.Text = string.Format("上涨占比:{0}%", Math.Round((double)rows.Count() / _dt.Rows.Count, 2) * 100);
            }
            else
            {
                label1.Text = string.Format("总数:{0}", _dt.Rows.Count);
                label2.Text = string.Format("上涨:{0}", _dt.Rows.Count);
                label3.Text = string.Format("上涨占比:{0}", _dt.Rows.Count);
            }
            
            //更新已有的数据
            updateAllData();
            //tab所有
            filterData("涨幅 > 4 and 异动涨幅 >= 2", _dt, this.dataGridView1);
            //tab异动计数小于2的个股
            filterData("涨幅 > 4 and 异动涨幅 < 2", _dt, this.dataGridView2);
        }

        void filterData(string sql,DataTable dt,DataGridView view)
        {
            DataTable src = view.DataSource as DataTable;
            DataRow[] rows = dt.Select(sql);
            DataTable newdt = dt.Clone();
            foreach (DataRow item in rows)
            {
                newdt.Rows.Add(item.ItemArray);
            }
            newdt.DefaultView.Sort= "行业代码";
            if (src == null || src.Rows.Count == 0)
            {
                view.DataSource = newdt;
                view.Columns["开盘价"].Visible = false;
                view.Columns["收盘价"].Visible = false;
                view.Columns["最高价"].Visible = false;
                view.Columns["最低价"].Visible = false;
                view.Columns["总股本"].Visible = false;
                view.Columns["行业代码"].Visible = false;
                view.Columns["价格区间"].Visible = false;
                view.RowHeadersVisible = false;

                foreach (DataGridViewColumn clm in view.Columns)
                {
                    //clm.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    switch (clm.Index){
                        case 0:
                            clm.Width = 50;
                            break;
                        case 1:
                            clm.Width = 60;
                            break;
                        case 4:
                            clm.Width = 60;
                            break;
                        case 7:
                            clm.Width = 55;
                            break;
                        case 8:
                            clm.Width = 60;
                            break;
                        case 9:
                            clm.Width = 60;
                            break;
                        case 10:
                            clm.Width = 60;
                            break;
                        case 11:
                            clm.Width = 60;
                            break;
                        case 13:
                            clm.Width = 60;
                            break;
                        case 16:
                            clm.Width = 70;
                            break;
                        case 17:
                            clm.Width = 60;
                            break;
                        case 18:
                            clm.Width = 60;
                            break;
                        case 19:
                            clm.Width = 70;
                            break;
                            //default:
                            //    clm.Width = 50;
                            //    break;
                    }
                }
                DataGridViewButtonColumn btnColumn = new DataGridViewButtonColumn();
                btnColumn.HeaderText = "操作";
                btnColumn.Text = "关注";
                btnColumn.Width = 40;
                btnColumn.UseColumnTextForButtonValue = true;
                view.Columns.Add(btnColumn);
                view.CellContentClick += DataGridView1_CellContentClick;
                view.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                view.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;

            }
            else
            {
                insertRow(src, newdt);
            }

            if (null == view.DataSource)
            {
                return;
            }

            DataTable dbsrc = view.DataSource as DataTable;
            int idx = 0;
            foreach (DataRow row in dbsrc.Rows)
            {
                DataGridViewCell cell9 = view.Rows[idx].Cells[9];
                float val9 = float.Parse(cell9.Value.ToString());
                if (val9 < 0)
                {
                    cell9.Style.ForeColor = Color.Green;
                }else if (val9 > 4.5)
                {
                    cell9.Style.ForeColor = Color.Crimson;
                    cell9.Style.Font = new Font("宋体", 10, FontStyle.Bold);
                }
                else
                {
                    cell9.Style.ForeColor = Color.Red;
                    cell9.Style.Font = new Font("宋体", 8);
                }

                DataGridViewCell cell10 = view.Rows[idx].Cells[10];
                float val10 = float.Parse(cell10.Value.ToString());
                if (val10 < 0)
                {
                    cell10.Style.ForeColor = Color.Green;
                }
                else if (val10 > 4.5)
                {
                    cell10.Style.ForeColor = Color.Crimson;
                    cell10.Style.Font = new Font("宋体",10,FontStyle.Bold);
                }
                else
                {
                    cell10.Style.ForeColor = Color.Red;
                    cell10.Style.Font = new Font("宋体", 8);
                }

                DataGridViewCell cell4 = view.Rows[idx].Cells[4];
                float val4 = float.Parse(cell4.Value.ToString());
                if (val4 > 50)
                {
                    cell4.Style.ForeColor = Color.Blue;
                    cell4.Style.Font = new Font("宋体", 10, FontStyle.Bold);
                }

                DataGridViewCell cell18 = view.Rows[idx].Cells[18];
                DataGridViewCell cell19 = view.Rows[idx].Cells[19];
                float val18 = float.Parse(cell18.Value.ToString());
                float val19 = float.Parse(cell19.Value.ToString());
                if (val18 > (val19 * 2) )
                {
                    cell18.Style.ForeColor = Color.Crimson;
                    cell18.Style.Font = new Font("宋体", 10, FontStyle.Bold);
                }
                else if (val18 > val19)
                {
                    cell18.Style.ForeColor = Color.Crimson;
                }

                DataGridViewCell cell20 = view.Rows[idx].Cells[20];
                DataGridViewCell cell1 = view.Rows[idx].Cells[1];
                float val20 = float.Parse(cell20.Value.ToString());
                if (val20 == 0)
                {
                    cell1.Style.ForeColor = Color.Crimson;
                }
                else if (val20 == 1)
                {
                    cell1.Style.ForeColor = Color.Blue;
                }
                else if (val20 == 3)
                {
                    cell1.Style.ForeColor = Color.FromArgb(8, 156, 143);
                }

                idx++;
            }
           
        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dataGridView1.Columns[e.ColumnIndex] is DataGridViewButtonColumn)
            {
                DataGridViewCell cell = dataGridView1.Rows[e.RowIndex].Cells[0];
                MessageBox.Show(cell.Value.ToString());
            }
        }

        void runBoardCase(object wParam, object lParam)
        {
            if (string.IsNullOrEmpty(lParam.ToString()))
            {
                return;
            }
            string str = JToken.Parse((string)lParam).ToString();
            JObject jobj = (JObject)JsonConvert.DeserializeObject(str);
            JArray arr = (JArray)jobj.GetValue("data");
            foreach (JObject obj in arr)
            {
                CStockPriceTimes stock = new CStockPriceTimes();
                stock.code = obj["code"].ToString();
                stock.open = float.Parse(obj["open"].ToString());
                stock.close = float.Parse(obj["close"].ToString());
                stock.price = float.Parse(obj["price"].ToString());
                stock.high = float.Parse(obj["high"].ToString());
                stock.low = float.Parse(obj["low"].ToString());
                stock.done_num = float.Parse(obj["done_num"].ToString());
                stock.done_money = float.Parse(obj["done_money"].ToString());
                stock.levelW = float.Parse(obj["levelW"].ToString());
                stock.level = float.Parse(obj["level"].ToString());
                stock.time = obj["time"].ToString();
                stock.name = obj["name"].ToString();
                stock.total_mv = float.Parse(obj["total_mv"].ToString());
                stock.float_mv = float.Parse(obj["float_mv"].ToString());
                stock.ind_code = obj["ind_code"].ToString();
                stock.ind_name = obj["ind_name"].ToString();
                stock.pinyin = obj["pinyin"].ToString();
                stock.risecount = int.Parse(obj["risecount"].ToString());
                stock.turn_over = float.Parse(obj["turn_over"].ToString());
                stock.turn_over_avg = float.Parse(obj["turn_over_avg"].ToString());
                stock.pricelevel = int.Parse(obj["pricelevel"].ToString());

                if (_db.Keys.Contains(stock.code))
                {
                    _db[stock.code] = stock;
                }
                else
                {
                    _db.Add(stock.code, stock);
                }

                DataRow row = _dt.Rows.Find(stock.code);
                if (null != row)
                {
                    row.BeginEdit();
                    row["价格"] = stock.price;
                    row["最高价"] = stock.high;
                    row["最低价"] = stock.low;
                    row["成交量"] = stock.done_num;
                    row["成交金额"] = stock.done_money;
                    row["实体涨幅"] = stock.levelW;
                    row["涨幅"] = stock.level;
                    row["时间"] = stock.time;
                    row["换手率"] = stock.turn_over;
                    row.EndEdit();
                }
                else
                {
                    _dt.Rows.Add(stock.code,
                            stock.name,
                            stock.open,
                            stock.close,
                            stock.price,
                            stock.high,
                            stock.low,
                            stock.done_num,
                            stock.done_money,
                            stock.levelW,
                            stock.level,
                            stock.time,
                            stock.total_mv,
                            stock.float_mv,
                            stock.ind_code,
                            stock.ind_name,
                            stock.pinyin,
                            stock.risecount,
                            stock.turn_over,
                            stock.turn_over_avg,
                            stock.pricelevel
                            );
                }
                
            }
            this.Invoke((EventHandler)delegate
            {
                filterAllData();
                if (null != this.dataGridView1.DataSource)
                {
                    DataTable all = this.dataGridView1.DataSource as DataTable;
                    this.lcount.Text = all.Rows.Count.ToString();
                }
                
            });
        }

        void runDataPrepare(object wParam, object lParam)
        {
            if (string.IsNullOrEmpty(lParam.ToString()))
            {
                return;
            }
            if (string.Compare("ok",lParam.ToString(),true) == 0)
            {
                _flag_send = true;
                this.Invoke((EventHandler)delegate
                {
                    lbLog.Text = "数据已准备好";
                });
            }
        }

        void procCase(object wParam, object lParam)
        {
            switch ((string)wParam)
            {
                case "BoardCase":
                    runBoardCase(wParam, lParam);
                    break;
                case "CDataPrepare":
                    runDataPrepare(wParam, lParam);
                    break;
            }
        }

        public void MsgProc(int msg, object wParam, object lParam)
        {
            switch (msg)
            {
                case MsgCenter.MSG_PROC_CMD:
                    procCase(wParam, lParam);
                    break;
                case MsgCenter.MSG_CMD_BOARDCASE:
                    while (true)
                    {
                        if (_flag_board)
                        {
                            _controle.Send(getExcuteCmd("CBoardCase", "run", "CBoardCase"));
                            Thread.Sleep(1000 * 10);
                        }
                        else
                        {
                            break;
                        }
                       
                    }
                    break;
                case MsgCenter.MSG_CMD_DATAPREPARE:
                    DateTime dt = DateTime.Now;
                    dt = DateTime.ParseExact("20250530", "yyyyMMdd", CultureInfo.InvariantCulture);
                    _controle.Send(getExcuteCmd("CDataPrepare", "run", dt.ToString("yyyyMMdd")));
                    break;
            }
        }
    }
}
