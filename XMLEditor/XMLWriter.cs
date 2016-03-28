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

        public void writeTestCase(XElement rootNode, 
                                  string   testCaseNo,
                                  string   testCaseDesc,
                                  string   sequenceNo
                                  )
        {
            
        }

        public void writeToXML()
        {
            XElement authors = new XElement("Authors");
            // Add child nodes
            XAttribute name = new XAttribute("Author", "Mahesh Chand");
            XElement book = new XElement("Book", "GDI+ Programming");
            XElement cost = new XElement("Cost", "$49.95");
            XElement publisher = new XElement("Publisher", "Addison-Wesley");
            XElement author = new XElement("Author");
            author.Add(name);
            author.Add(book);
            author.Add(cost);
            author.Add(publisher);
            authors.Add(author);

            name = new XAttribute("Name", "Mike Gold");
            book = new XElement("Book", "Programmer's Guide to C#");
            cost = new XElement("Cost", "$44.95");
            publisher = new XElement("Publisher", "Microgold Publishing");
            author = new XElement("Author");
            author.Add(name);
            author.Add(book);
            author.Add(cost);
            author.Add(publisher);
            authors.Add(author);

            name = new XAttribute("Name", "Scott Lysle");
            book = new XElement("Book", "Custom Controls");
            cost = new XElement("Cost", "$39.95");
            publisher = new XElement("Publisher", "C# Corner");
            author = new XElement("Author");
            author.Add(name);
            author.Add(book);
            author.Add(cost);
            author.Add(publisher);
            authors.Add(author);

            authors.Save(@"Authors.xml");
        }
    }
}
