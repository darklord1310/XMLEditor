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

        public XElement[] writeTestCase(List<TestCase> testCases)
        {
            XElement[] tcNames = new XElement[testCases.Count()];
            int index = 0;

            foreach (TestCase testcase in testCases)
            {
                XElement tcName = new XElement("TestCase");
                XAttribute tc = new XAttribute("tc", testcase.getTcNo());
                XElement testDesc = new XElement("Desc", testcase.getDesc() );
                XElement[] seqNo = writeSequenceNo(testcase.seqNo);

                tcName.Add(tc);
                tcName.Add(testDesc);
                for(int i = 0; i < seqNo.Count(); i++)
                {
                    tcName.Add(seqNo[i]);
                }

                tcNames[index] = tcName;
                index++;
            }

            return tcNames;
        }

        public XElement[] writeSequenceNo(List<SqNum> sequences)
        {
            XElement[] seqNo = new XElement[sequences.Count()];
            XElement param = null;
            XElement expect = null;
            int i = 0;

            foreach(SqNum sequence in sequences)
            {
                XElement sqNum = new XElement("SeqNum");
                XAttribute sn = new XAttribute("sn", sequence.getSeqNo().ToString() );
                XElement seqDesc = new XElement("Desc", sequence.getDesc().ToString() );
                XElement diagcmd = new XElement("DiagCmd", sequence.getDiagCmd().ToString() );
                if(!string.IsNullOrEmpty(sequence.getPara()))
                    param = new XElement("Param", sequence.getPara().ToString());
                else
                    param = new XElement("Param", string.Empty);

                if (!string.IsNullOrEmpty(sequence.getExpected()))
                    expect = new XElement("Expect", sequence.getExpected().ToString());
                else
                    expect = new XElement("Expect", string.Empty);

                sqNum.Add(sn);
                sqNum.Add(seqDesc);
                sqNum.Add(diagcmd);
                sqNum.Add(param);
                sqNum.Add(expect);
                seqNo[i] = sqNum;
                i++;
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
                XElement[] tCase;

                //add from root
                root.Add(categoryName);
                root.Add(moduleName);
                tCase = writeTestCase(tc);

                for (int i = 0; i < tCase.Count(); i++ )
                    root.Add(tCase[i]);

                // save changes
                root.Save(path);
                return 1;
            }
            catch(Exception ex)
            {
                Console.Write(ex.Message);
                return 0;
            }

        }
    }
}
