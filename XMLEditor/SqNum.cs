using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLEditor
{
    public class SqNum
    {
        private string sNo;
        private string description;
        private string diagCmd;
        private string para;
        private string expected;

        public SqNum() { }

        public SqNum(string no, string desc, string diagCmd, string para, string expected)
        {
            setSeqNo(no);
            setDesc(desc);
            setDiagCmd(diagCmd);
            setPara(para);
            setExpected(expected);
        }

        public string getSeqNo() { return sNo; }
        public string getDesc() { return description; }
        public string getDiagCmd() { return diagCmd; }
        public string getPara() { return para; }
        public string getExpected() { return expected; }

        public void setSeqNo(string no) { sNo = no; }
        public void setDesc(string desc) { description = desc; }
        public void setDiagCmd(string diagCmd) { this.diagCmd = diagCmd; }
        public void setPara(string para) { this.para = para; }
        public void setExpected(string exp) { expected = exp; }
    }
}
