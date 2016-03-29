using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLEditor
{
    public class TestMenu
    {
        private string categoryName;
        private char categoryID;
        private string moduleName;
        private string moduleID;
        private string funcName;
        private string para;
        private string outcome;

        public TestMenu() { }

        public string getCategoryName() { return categoryName; }
        public char getCategoryID() { return categoryID; }
        public string getModuleName() { return moduleName; }
        public string getModuleID() { return moduleID; }
        public string getFuncName() { return funcName; }
        public string getPara() { return para; }
        public string getOutcome() { return outcome; }

        public void setCategoryName(string name) { categoryName = name; }
        public void setCategoryID(char id) { categoryID = id; }
        public void setModuleName(string name) { moduleName = name; }
        public void setModuleID(string id) { moduleID = id; }
        public void setFuncName(string name) { funcName = name; }
        public void setPara(string p) { para = p; }
        public void setOutcome(string ot) { outcome = ot; }
    }
}