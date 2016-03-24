using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLEditor
{
    public class Category
    {
        private string category;
        private string module;
        public List<TestCase> tc = new List<TestCase>();

        public Category() { }

        public Category(string cg, string mod)
        {
            setCategory(cg);
            setModule(mod);
        }

        public string getCategory() { return category; }
        public string getModule() { return module; }

        public void setCategory(string category) { this.category = category; }
        public void setModule(string mod) { module = mod; }
    }
}
