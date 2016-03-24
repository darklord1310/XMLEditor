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
            treeView1.Nodes.Add(createNormalTreeNode("asdasdsadasd"));
            treeView1.AllowDrop = true;
            // Add some additional nodes.
            treeView1.Nodes[0].Nodes.Add(createNormalTreeNode("aaaaaa"));
            treeView1.Nodes.Add(createNormalTreeNode("resume.doc"));
            string[] str = new string[4];
            createDropDownTreeNode("",str);
            treeView1.EndUpdate();
            treeView1.ExpandAll();
        }

        public TreeNode createNormalTreeNode(string nodeName)
        {
            TreeNode node = new TreeNode(nodeName);
            node.Name = nodeName;
            return node;
        }

        public DropDownTreeNode createDropDownTreeNode(string nodeName, string[] comboBoxValues)
        {
            //treeView1.Nodes.Add(new DropDownTreeNode("adsad"));
            //MessageBox.Show(treeView1.Nodes[2]);
            //treeView1.Nodes[2].

            /*
            DropDownTreeNode newnode = new DropDownTreeNode(nodeName);
            newnode.Name = nodeName;
            newnode.addValuesToComboBox(comboBoxValues);
            return newnode;
            */
            
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

            this.treeView1.Nodes.Add(meatNode);
            return null;
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
                treeView1.BeginUpdate();
                treeView1.ExpandNodeComboBox(treeView1.SelectedNode);
                treeView1.EndUpdate();
            }
            else if(item.ToString() == "Add")
            {
                DropDownTreeNode newnode = new DropDownTreeNode("");
                // load the data into combobox
                // select the default value for the combobox
                addAtSelectedTreeNode(treeView1.TopNode, treeView1.SelectedNode.Name, treeView1.SelectedNode.Level, newnode);
                //treeView1.SelectedNode.Nodes.Add(newnode);
                //treeView1.Nodes[treeView1.SelectedNode.Index].Nodes.Add(newnode);
                //treeView1.EndUpdate();
                
            }
            else if(item.ToString() == "Delete")
            {
                treeView1.Nodes[treeView1.SelectedNode.Index].Remove();
            }
        }
        
        /*
         *  root        is the first node also known as the root of the node
         *  name        is the name of the selected tree node which will add an
         *              additional child node to it
         *  nodeLevel   is the node level of the selected tree node
         *  newnode     is the new child node going to add to the selected node
         * 
         */
        private void addAtSelectedTreeNode(TreeNode root, String name, int nodeLevel, TreeNode newnode)
        {
            if (root.Name.Equals(name) && root.Level == nodeLevel)
            {
                root.Nodes.Add(newnode);
                treeView1.ExpandAll();
                treeView1.ExpandNodeComboBox(newnode);
                Console.WriteLine("added");
            }
            else
            {
                foreach (TreeNode node in root.Nodes)
                {
                    if (node.Name.Equals(name) && node.Level == nodeLevel)
                    {
                        node.Nodes.Add(newnode);
                        treeView1.ExpandAll();
                        treeView1.ExpandNodeComboBox(newnode);
                        Console.WriteLine("added");
                    }
                    else
                    {
                        if (node.Nodes.Count > 0)
                            addAtSelectedTreeNode(node, name, nodeLevel, newnode);
                    }
                }
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
