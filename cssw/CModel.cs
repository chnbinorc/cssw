using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cssw
{
    class CModel
    {
    }

    public class CStockPriceTimes
    {
        public string code { get; set; }
        public float open { get; set; }
        public float close { get; set; }
        public float price { get; set; }
        public float high { get; set; }
        public float low { get; set; }
        public float done_num { get; set; }
        public float done_money { get; set; }
        public float levelW { get; set; }
        public float level { get; set; }
        public string time { get; set; }

        public string name { get; set; }
        public float total_mv { get; set; }
        public float float_mv { get; set; }
        public string ind_code { get; set; }
        public string ind_name { get; set; }
        public string pinyin { get; set; }
        public int risecount { get; set; }
        public float turn_over_avg { get; set; }
        public float turn_over { get; set; }
        public int pricelevel { get; set; }

    }
}
