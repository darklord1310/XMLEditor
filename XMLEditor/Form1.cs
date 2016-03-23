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
    public partial class Form1 : Form
    {
        string appPath, folderPath;
        ContextMenuStrip docMenu;
        string filename = "TestData";

        public Form1()
        {
            
            InitializeComponent();
            createDataPath();
            treeView1.BeginUpdate();
            treeView1.Nodes.Add("Test Menu");
            treeView1.EndUpdate();
            treeView1.AllowDrop = true;
            // Add some additional nodes.
            treeView1.Nodes[0].Nodes.Add("phoneList.doc");
            treeView1.Nodes.Add("resume.doc");
            createNode();

            treeView1.ExpandAll();
        }

        public void createNode()
        {
            //treeView1.Nodes.Add(new DropDownTreeNode("adsad"));
            //MessageBox.Show(treeView1.Nodes[2]);
            //treeView1.Nodes[2].

            DropDownTreeNode weightNode = new DropDownTreeNode("1/4 lb.");
            weightNode.ComboBox.Items.Add("1/4 lb.");
            weightNode.ComboBox.Items.Add("1/2 lb.");
            weightNode.ComboBox.Items.Add("3/4 lb.");
            weightNode.ComboBox.SelectedIndex = 0;

            DropDownTreeNode pattyNode = new DropDownTreeNode("All beef patty");
            pattyNode.ComboBox.Items.Add("All beef patty");
            pattyNode.ComboBox.Items.Add("All chicken patty");
            pattyNode.ComboBox.SelectedIndex = 0;

            TreeNode meatNode = new TreeNode("Meat Selection");
            meatNode.Nodes.Add(weightNode);
            meatNode.Nodes.Add(pattyNode);

            //TreeNode burgerNode = new TreeNode("Hamburger Selection");
            //burgerNode.Nodes.Add(condimentsNode);
            //burgerNode.Nodes.Add(meatNode);
            this.treeView1.Nodes.Add(meatNode);
             
        }

        public void createDataPath()
        {
            appPath = Path.GetDirectoryName(Application.ExecutablePath);  // get the root path of the dir
            folderPath = Path.Combine(appPath, "AppData");                // get the path to the AppData folder

            if (!System.IO.Directory.Exists(folderPath))
            {
                MessageBox.Show("AppData folder not found. It will be created automatically");
                System.IO.Directory.CreateDirectory(folderPath);
            }
        }

        private void createContextMenuStrip(int nodeLevel)
        {
            // Create the ContextMenuStrip.
            docMenu = new ContextMenuStrip();

            //Create some menu items.
            ToolStripMenuItem deleteLabel = new ToolStripMenuItem();
            deleteLabel.Text = "Delete";
            ToolStripMenuItem renameLabel = new ToolStripMenuItem();
            renameLabel.Text = "Edit";
            ToolStripMenuItem addLabel = new ToolStripMenuItem();
            addLabel.Text = "Add";

            //Add the menu items to the menu.
            if(nodeLevel == 4)
                docMenu.Items.AddRange(new ToolStripMenuItem[] { addLabel, deleteLabel});
            else
                docMenu.Items.AddRange(new ToolStripMenuItem[]{addLabel,deleteLabel, renameLabel});

            docMenu.ItemClicked += new ToolStripItemClickedEventHandler(contextMenu_ItemClicked);
        }

        void contextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripItem item = e.ClickedItem;
            
            if(item.ToString() == "Edit" )
            {
                //DropDownTreeView bla = new DropDownTreeView();
                treeView1.ExpandNodeComboBox(treeView1.SelectedNode);
            }
            
        }
        


        // handle the show context menu event at tree node
        private void treeView1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if(treeView1.GetNodeAt(e.X,e.Y) != null)
                    treeView1.SelectedNode = treeView1.GetNodeAt(e.X, e.Y);         // get the tree node on mouse right click
                treeView1.SelectedNode.BackColor = SystemColors.HighlightText;      // highlight the selected node
                Point p = new Point(treeView1.SelectedNode.Bounds.Right + 15, treeView1.SelectedNode.Bounds.Bottom + 25);
                createContextMenuStrip(treeView1.SelectedNode.Level);
                docMenu.Show(PointToScreen(p));     
            }
           
        }

    }
}
