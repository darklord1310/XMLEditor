using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO;

namespace XMLEditor
{
    public class DropDownTreeNode : TreeNode
    {
            // *snip* Constructors go here
            public DropDownTreeNode(string name)
            {
                this.Text = name;
                this.Name = name;
            }

            private ComboBox m_ComboBox = new ComboBox();
            public ComboBox ComboBox
            {
                get
                {
                    this.m_ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                    return this.m_ComboBox;
                }
                set
                {
                    this.m_ComboBox = value;
                    this.m_ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                }
            }
     }







        
}
