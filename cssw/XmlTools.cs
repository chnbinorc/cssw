using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace cssw
{

    public class NoticStocks
    {
        List<string> _stocks = new List<string>();
        XmlTools tools = null;
        public NoticStocks()
        {
            tools = new XmlTools(System.IO.Directory.GetCurrentDirectory() + "\\stockconfigs.xml");
        }

        public List<string> getStocks()
        {
            return _stocks;
        }

        public void setStocks(List<string> stocks)
        {
            _stocks = stocks;
        }

        public void add(string code)
        {
            foreach(string s in _stocks)
            {
                if (string.Compare(s, code) == 0)
                {
                    return;
                }
            }
            _stocks.Add(code);
        }

        public void del(string code)
        {
            if (_stocks.Contains(code))
            {
                _stocks.Remove(code);
            }
        }

        public void Save()
        {
        }

    }

    public class XmlTools
    {
        XmlDocument doc = new XmlDocument();

        XmlElement _Root = null;
        String _file = "";

        public XmlTools(String sName)
        {
            _file = sName;
            try
            {
                doc.Load(sName);
                _Root = doc.DocumentElement;

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public XmlElement Root
        {
            get { return _Root; }
        }

        public void Save()
        {

        }

        public List<string> GetStocks()
        {
            return null;
        }
    }

    
}
