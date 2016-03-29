using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;

namespace XMLEditor
{
    class XMLWriter
    {
        public XMLWriter()
        {

        }

        public XElement writeTestCase(List<TestCase> testCases)
        {
            XElement tcName = new XElement("TestCase");

            foreach (TestCase testcase in testCases)
            {
                XAttribute tc = new XAttribute("tc", "TC_0001");
                XElement testDesc = new XElement("Desc", testcase.getDesc() );
                XElement seqNo = writeSequenceNo(testcase.seqNo);
                tcName.Add(seqNo);
            }

            return tcName;
        }

        public XElement writeSequenceNo(List<SqNum> sequences)
        {
            XElement seqNo = new XElement("SeqNum");

            foreach(SqNum sequence in sequences)
            {
                XAttribute sn = new XAttribute("sn", sequence.getSeqNo() );
                XElement seqDesc = new XElement("Desc", sequence.getDesc() );
                XElement diagcmd = new XElement("DiagCmd", sequence.getDiagCmd());
                XElement param = new XElement("Param", sequence.getPara());
                XElement expect = new XElement("Expect", sequence.getExpected());
                
                seqNo.Add(sn);
                seqNo.Add(seqDesc);
                seqNo.Add(diagcmd);
                seqNo.Add(param);
                seqNo.Add(expect);              
            }

            return seqNo;
        }

        public int writeToXML(string category, string module, List<TestCase> tc, string path)
        {
            try
            {
                XElement root = new XElement("TestMenu");
                XElement categoryName = new XElement("Category", category);
                XElement moduleName = new XElement("Module", module);

                //add from root
                root.Add(categoryName);
                root.Add(moduleName);
                root.Add(writeTestCase(tc));

                // save changes
                root.Save(path);
                return 1;
            }
            catch(Exception)
            {
                return 0;
            }

        }
    }
}
