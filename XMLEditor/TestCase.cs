using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLEditor
{
    public class TestCase
    {
        private string tcNo;
        private string desc;
        public List<SqNum> seqNo = new List<SqNum>();

        public TestCase() { }

        public TestCase(string tcNo, string desc)
        {
            setTcNo(tcNo);
            setDesc(desc);
        }

        public string getTcNo() { return tcNo; }
        public string getDesc() { return desc; }

        public void setTcNo(string tc) { tcNo = tc; }
        public void setDesc(string d) { desc = d; }
    }
}
