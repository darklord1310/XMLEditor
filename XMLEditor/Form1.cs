using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data.SQLite;
using System.IO;

namespace XMLEditor
{
    public partial class Form1 : Form
    {
        int num = 0;
        string moduleName = string.Empty;
        string moduleID = string.Empty;
        string funcName = string.Empty;
        TestMenu[] tm = new TestMenu[7];
        Category cat = new Category();

        Excel.Application xlApp = new Excel.Application();
        Excel.Workbook xlWorkbook;
        Excel._Worksheet xlWorksheet;
        Excel.Range xlRange;

        string appPath, folderPath;
        ContextMenuStrip docMenu;

        public class InvalidTestMenu : Exception
        {
            public InvalidTestMenu(string message)
            {
                MessageBox.Show(message);
            }
        }

        public class NoExcelSelected : Exception
        {
            public NoExcelSelected(string message)
            {
                MessageBox.Show(message);
            }
        }

        public Form1()
        {
            InitializeComponent();
            createDataPath();
            createTreeView();
        }

        private void createTreeView()
        {
            treeView1.BeginUpdate();
            treeView1.Nodes.Add(createNormalTreeNode("TestMenu"));
            treeView1.AllowDrop = true;
            treeView1.EndUpdate();
        }

        public TreeNode createNormalTreeNode(string nodeName)
        {
            TreeNode node = new TreeNode(nodeName);
            node.Name = nodeName;
            return node;
        }

        public DropDownTreeNode createDropDownTreeNode(string nodeName, string[] comboBoxValues)
        {
            DropDownTreeNode newnode = new DropDownTreeNode(nodeName);
            newnode.Name = nodeName;
            newnode.addValuesToComboBox(comboBoxValues);
            return newnode;
        }

        private void addNode(TreeNode node)
        {
            try
            {
                if (xlWorkbook == null)
                    throw new NoExcelSelected("No Excel selected!");
           
                string nodeName = string.Empty;
                string[] strArr = getStringArray(node);
            
                if(node.Level == 0)
                    nodeName = "Category";
                else if(node.Level == 1)
                    nodeName = "Module";
                else if(node.Level == 2)
                    nodeName = "TC";
                else if(node.Level == 3)
                    nodeName = "SQN";

                if (node.Level >= 2)
                {
                    TreeNode dNode = null;
                    if(node.Level == 2)
                        dNode = createNormalTreeNode("TC_" + (node.Nodes.Count + 1).ToString("D4"));
                    else
                        dNode = createNormalTreeNode("SN_" + (node.Nodes.Count + 1).ToString("D4"));

                    node.Nodes.Add(dNode);
                    node.Expand();
                }
                else
                {
                    if (strArr != null)
                    {
                        DropDownTreeNode dNode = createDropDownTreeNode(nodeName, strArr);
                        node.Nodes.Add(dNode);
                        node.Expand();
                        treeView1.ExpandNodeComboBox(dNode);
                    }
                    else
                        MessageBox.Show("No module available!");
                }
            }
            catch (NoExcelSelected ex) { }
        }

        private void deleteNode(TreeNode node)
        {
            TreeNode parent = node.Parent;
            int index = node.Index;
            node.Remove();

            for (int i = index; i < parent.Nodes.Count; i++)
            {
                if (parent.Level == 2)
                {
                    parent.Nodes[i].Name = "TC_" + (i + 1).ToString("D4");
                    parent.Nodes[i].Text = "TC_" + (i + 1).ToString("D4");
                }
                else
                {
                    parent.Nodes[i].Name = "SN_" + (i + 1).ToString("D4");
                    parent.Nodes[i].Text = "SN_" + (i + 1).ToString("D4");
                }
            }
        }

        private void removeAllChildNode(TreeNode node)
        {
            for (int i = node.Nodes.Count - 1; i >= 0; i--)
            {
                node.Nodes[i].Remove();
            }
        }

        void contextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripItem item = e.ClickedItem;

            if (item.ToString() == "Edit")
            {
                treeView1.BeginUpdate();

                if(treeView1.SelectedNode.Nodes.Count > 0)
                {
                    if (MessageBox.Show("Are you sure?\nAll child nodes will be remove", "Confirmation",
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        removeAllChildNode(treeView1.SelectedNode);
                    }
                }
                treeView1.ExpandNodeComboBox(treeView1.SelectedNode);

                treeView1.EndUpdate();
            }
            else if (item.ToString() == "Add")
            {
                addNode(treeView1.SelectedNode);
            }
            else if (item.ToString() == "Delete")
            {
                deleteNode(treeView1.SelectedNode);
            }
        }

        // handle the show context menu event at tree node
        private void treeView1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (treeView1.GetNodeAt(e.X, e.Y) != null)
                {
                    treeView1.SelectedNode = treeView1.GetNodeAt(e.X, e.Y);         // get the tree node on mouse right click
                    treeView1.SelectedNode.BackColor = SystemColors.HighlightText;      // highlight the selected node
                    Point p = new Point(treeView1.SelectedNode.Bounds.Right + 15, treeView1.SelectedNode.Bounds.Bottom + 25);
                    createContextMenuStrip(treeView1.SelectedNode.Level);
                    docMenu.Show(PointToScreen(p));
                }
            }

        }

        public void createDataPath()
        {
            appPath = Path.GetDirectoryName(Application.ExecutablePath);  // get the root path of the dir
            folderPath = Path.Combine(appPath, "AppData");                // get the path to the AppData folder

            if (!Directory.Exists(folderPath))
            {
                MessageBox.Show("AppData folder not found. It will be created automatically");
                Directory.CreateDirectory(folderPath);
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
            if (nodeLevel == 4) //Sequence Number
                docMenu.Items.AddRange(new ToolStripMenuItem[] { deleteLabel });
            else if (nodeLevel == 0 && treeView1.Nodes["TestMenu"].Nodes.Count > 0)
                docMenu.Items.AddRange(new ToolStripMenuItem[] { });
            else if (nodeLevel == 1 && treeView1.Nodes["TestMenu"].Nodes[0].Nodes.Count > 0)
                docMenu.Items.AddRange(new ToolStripMenuItem[] { deleteLabel, renameLabel });
            else if (nodeLevel == 0)    //Test Menu
                docMenu.Items.AddRange(new ToolStripMenuItem[] { addLabel });
            else if (nodeLevel == 3)    //Test Case
                docMenu.Items.AddRange(new ToolStripMenuItem[] { addLabel, deleteLabel });
            else
                docMenu.Items.AddRange(new ToolStripMenuItem[] { addLabel, deleteLabel, renameLabel });

            docMenu.ItemClicked += new ToolStripItemClickedEventHandler(contextMenu_ItemClicked);
        }

        private void extractDataFromExcel()
        {
            try
            {
                for (int i = 2; i <= xlWorkbook.Sheets.Count; i++)
                {
                    xlWorksheet = xlWorkbook.Sheets[i];
                    xlRange = xlWorksheet.UsedRange;

                    int rowCount = xlRange.Rows.Count;

                    for (int row = 1; row <= rowCount; row++)
                    {
                        switch (xlWorksheet.Name)
                        {
                            case "Category ID":
                                updateCategory(row);
                                break;
                            case "Module ID":
                                updateModule(row);
                                break;
                            case "Driver":
                                updateFuncName(row, "Driver");
                                break;
                            case "Library":
                                updateFuncName(row, "Library");
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (InvalidTestMenu ex) { }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

            //MessageBox.Show(tm[1].getModuleName());
            //MessageBox.Show(tm[1].getFuncName());
        }

        private void updateCategory(int row)
        {
            if (xlRange.Cells[row, 2].Value2 != null)
            {
                tm[row - 2] = new TestMenu();
                tm[row - 2].setCategoryName(xlRange.Cells[row, 1].Value2);
                tm[row - 2].setCategoryID((char)xlRange.Cells[row, 2].Value2[0]);
            }
        }

        private void updateModule(int row)
        {
            if (xlRange.Cells[row, 2].Value2 != null)
            {
                string categoryName = xlRange.Cells[row, 1].Value2.Substring(0, xlRange.Cells[row, 1].Value2.IndexOf('-') - 1);
                num = getTestMenuNum(categoryName);
                if (num == 99)
                    throw new InvalidTestMenu("Invalid Category!");
                moduleName += xlRange.Cells[row, 1].Value2.Substring(xlRange.Cells[row, 1].Value2.LastIndexOf('-') + 2) + "|";
                moduleID += xlRange.Cells[row, 2].Value2 + "|";

                if (xlRange.Cells[row + 1, 1].Value2 == null)
                {
                    moduleName = moduleName.Remove(moduleName.Length - 1);
                    moduleID = moduleID.Remove(moduleID.Length - 1);
                    tm[num].setModuleName(moduleName);
                    tm[num].setModuleID(moduleID);
                    moduleName = string.Empty;
                    moduleID = string.Empty;
                }
            }
        }

        private void updateFuncName(int row, string module)
        {
            if (!string.Equals(xlRange.Cells[row, 2].Value2, "Function Name") &&
                                    !string.IsNullOrEmpty(xlRange.Cells[row, 2].Value2))
            {
                funcName += xlRange.Cells[row, 2].Value2 + ",";

                if (xlRange.Cells[row + 1, 1].Value2 == null)
                {
                    funcName = funcName.Remove(funcName.Length - 1);
                    funcName += "|";

                    if (xlRange.Cells[row + 2, 1].Value2 == null)
                    {
                        funcName = funcName.Remove(funcName.Length - 1);
                        tm[getTestMenuNum(module)].setFuncName(funcName);
                        funcName = string.Empty;
                    }
                }
            }
        }

        private int getTestMenuNum(string categoryName)
        {
            for (int i = 0; i < tm.Count(); i++)
            {
                if (string.Equals(tm[i].getCategoryName(), categoryName))
                    return i;
            }

            return 99;
        }

        private string[] getStringArray(TreeNode node)
        {
            string[] strArr = null;

            switch(node.Level)
            {
                case 0: //Categories
                    strArr = new string[tm.Count()];

                    for (int i = 0; i < tm.Count(); i++ )
                    {
                        strArr[i] = tm[i].getCategoryName();
                    }
                    break;
                case 1: //Module
                    int num = getTestMenuNum(node.Text);
                    if (!string.IsNullOrEmpty(tm[num].getModuleName()))
                        strArr = tm[num].getModuleName().Split('|');
                    break;
                default:
                    break;
            }

            return strArr;
        }

        private void editCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Create a new instance of the OpenFileDialog
            OpenFileDialog dialog = new OpenFileDialog();

            //Set the file filter
            dialog.Filter = "Excel files (*.xls)|*.xls";

            //Set Initial Directory
            dialog.InitialDirectory = folderPath;
            dialog.Title = "Select a Excel file";

            //Present to the user. 
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                xlWorkbook = xlApp.Workbooks.Open(dialog.FileName);
                this.Cursor = Cursors.WaitCursor;
                extractDataFromExcel();
                this.Cursor = Cursors.Default;
            }
        }

        private void treeView1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void treeView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            treeView1.DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private void treeView1_DragDrop(object sender, DragEventArgs e)
        {
            TreeNode NewNode;

            if (e.Data.GetDataPresent("System.Windows.Forms.TreeNode", false))
            {
                Point pt = ((TreeView)sender).PointToClient(new Point(e.X, e.Y));
                TreeNode DestinationNode = ((TreeView)sender).GetNodeAt(pt);
                NewNode = (TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode");
                handleNodeMoving(DestinationNode.Parent, NewNode.Index, DestinationNode.Index);
            }
        }

        private void handleNodeMoving(TreeNode parent, int fromIndex, int toIndex)
        {
            int status = toIndex - fromIndex;
            List<TreeNode> nodes;

            nodes = getNodeValues(parent);
            
            if(status > 0)
                nodes = swapDown(nodes, fromIndex, toIndex);
            else
                nodes = swapUp(nodes, fromIndex, toIndex);

            rearrangeTreeNodes(parent, nodes);
        }

        private void rearrangeTreeNodes(TreeNode parent, List<TreeNode> nodes)
        {
            int i = 0;
            RemoveChildNodes(parent);
            foreach (TreeNode element in nodes)
            {
                parent.Nodes.Add(nodes[i]);
                i++;
            }
        }

        private List<TreeNode> getNodeValues(TreeNode parentNode)
        {
            List<TreeNode> nodes = new List<TreeNode>();
            foreach (TreeNode node in parentNode.Nodes)
            {
                nodes.Add(node);
            }
            return nodes;
        }

        private void RemoveChildNodes(TreeNode aNode)
        {
            if (aNode.Nodes.Count > 0)
            {
                for (int i = aNode.Nodes.Count - 1; i >= 0; i--)
                {
                    aNode.Nodes[i].Remove();
                }
            }

        }

        private List<TreeNode> swapDown(List<TreeNode> nodes, int fromIndex, int toIndex)
        {
            TreeNode temp;
            for (int i = fromIndex; i < toIndex; i++)
            {
                temp = nodes[i + 1];
                nodes[i + 1] = nodes[i];
                nodes[i] = temp;
            }

            return nodes;
        }

        private List<TreeNode> swapUp(List<TreeNode> nodes, int fromIndex, int toIndex)
        {
            TreeNode temp;
            for (int i = fromIndex; i > toIndex; i--)
            {
                temp = nodes[i-1];
                nodes[i - 1] = nodes[i];
                nodes[i] = temp;
            }

            return nodes;
        }

        private void DrawLeafTopPlaceholders(TreeNode NodeOver)
        {
            Graphics g = this.treeView1.CreateGraphics();
            int LeftPos = NodeOver.Bounds.Left;
            int RightPos = this.treeView1.Width - 4;

            Point[] LeftTriangle = new Point[5]{
												   new Point(LeftPos, NodeOver.Bounds.Top - 4),
												   new Point(LeftPos, NodeOver.Bounds.Top + 4),
												   new Point(LeftPos + 4, NodeOver.Bounds.Y),
												   new Point(LeftPos + 4, NodeOver.Bounds.Top - 1),
												   new Point(LeftPos, NodeOver.Bounds.Top - 5)};

            Point[] RightTriangle = new Point[5]{
													new Point(RightPos, NodeOver.Bounds.Top - 4),
													new Point(RightPos, NodeOver.Bounds.Top + 4),
													new Point(RightPos - 4, NodeOver.Bounds.Y),
													new Point(RightPos - 4, NodeOver.Bounds.Top - 1),
													new Point(RightPos, NodeOver.Bounds.Top - 5)};


            g.FillPolygon(System.Drawing.Brushes.Black, LeftTriangle);
            g.FillPolygon(System.Drawing.Brushes.Black, RightTriangle);
            g.DrawLine(new System.Drawing.Pen(Color.Black, 2), new Point(LeftPos, NodeOver.Bounds.Top), new Point(RightPos, NodeOver.Bounds.Top));

        }//eom

        private void DrawLeafBottomPlaceholders(TreeNode NodeOver, TreeNode ParentDragDrop)
        {
            Graphics g = this.treeView1.CreateGraphics();

            // Once again, we are not dragging to node over, draw the placeholder using the ParentDragDrop bounds
            int LeftPos, RightPos;
            if (ParentDragDrop != null)
                LeftPos = ParentDragDrop.Bounds.Left + 8;
            else
                LeftPos = NodeOver.Bounds.Left;
            RightPos = this.treeView1.Width - 4;

            Point[] LeftTriangle = new Point[5]{
												   new Point(LeftPos, NodeOver.Bounds.Bottom),
												   new Point(LeftPos, NodeOver.Bounds.Bottom),
												   new Point(LeftPos, NodeOver.Bounds.Bottom),
												   new Point(LeftPos, NodeOver.Bounds.Bottom),
												   new Point(LeftPos, NodeOver.Bounds.Bottom)};

            Point[] RightTriangle = new Point[5]{
													new Point(RightPos, NodeOver.Bounds.Bottom),
													new Point(RightPos, NodeOver.Bounds.Bottom),
													new Point(RightPos, NodeOver.Bounds.Bottom),
													new Point(RightPos, NodeOver.Bounds.Bottom),
													new Point(RightPos, NodeOver.Bounds.Bottom)};


            g.FillPolygon(System.Drawing.Brushes.Black, LeftTriangle);
            g.FillPolygon(System.Drawing.Brushes.Black, RightTriangle);
            g.DrawLine(new System.Drawing.Pen(Color.Black, 2), new Point(LeftPos, NodeOver.Bounds.Bottom), new Point(RightPos, NodeOver.Bounds.Bottom));
        }//eom

    }

}


