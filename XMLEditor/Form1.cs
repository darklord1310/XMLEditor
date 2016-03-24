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
        Excel.Application xlApp = new Excel.Application();
        Excel.Workbook xlWorkbook;
        Excel._Worksheet xlWorksheet;
        Excel.Range xlRange;

        string appPath, folderPath;
        ContextMenuStrip docMenu;
        string filename = "TestData";

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
            treeView1.BeginUpdate();
            treeView1.Nodes.Add(createNormalTreeNode("asdasdsadasd"));
            treeView1.AllowDrop = true;
            // Add some additional nodes.
            treeView1.Nodes[0].Nodes.Add(createNormalTreeNode("1"));
            treeView1.Nodes[0].Nodes.Add(createNormalTreeNode("2"));
            treeView1.Nodes[0].Nodes.Add(createNormalTreeNode("3"));
            treeView1.Nodes[0].Nodes.Add(createNormalTreeNode("4"));
            treeView1.Nodes.Add(createNormalTreeNode("resume.doc"));
            string[] str = new string[4];
            str[0] = "bello";
            str[1] = "lello";
            str[2] = "gello";
            str[3] = "rello";
            createDropDownTreeNode("", str);
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
            DropDownTreeNode newnode = new DropDownTreeNode(nodeName);
            newnode.Name = nodeName;
            newnode.addValuesToComboBox(comboBoxValues);
            return newnode;
        }

        private void addNode(TreeNode node)
        {
            string[] str = new string[4];
            str[0] = "bello";
            str[1] = "lello";
            str[2] = "gello";
            str[3] = "rello";
            DropDownTreeNode dNode = createDropDownTreeNode("hello", str);
            node.Nodes.Add(dNode);
            node.Expand();
            treeView1.ExpandNodeComboBox(dNode);
        }

        private void deleteNode(TreeNode node)
        {
            node.Remove();
        }

        void contextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripItem item = e.ClickedItem;

            if (item.ToString() == "Edit")
            {
                treeView1.BeginUpdate();
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
            if (nodeLevel < 4)
            {
                if (nodeLevel == 3)
                    docMenu.Items.AddRange(new ToolStripMenuItem[] { addLabel, deleteLabel });
                else
                    docMenu.Items.AddRange(new ToolStripMenuItem[] { addLabel, deleteLabel, renameLabel });
                docMenu.ItemClicked += new ToolStripItemClickedEventHandler(contextMenu_ItemClicked);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (xlWorkbook == null)
                    throw new NoExcelSelected("No Excel selected!");

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
            catch (NoExcelSelected ex) { }
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

        private void editCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Create a new instance of the OpenFileDialog
            OpenFileDialog dialog = new OpenFileDialog();

            //Set the file filter
            dialog.Filter = "Excel files (*.xls)|*.xls";

            //Set Initial Directory
            dialog.InitialDirectory = Directory.GetCurrentDirectory() + "\\AppData";
            dialog.Title = "Select a Excel file";

            //Present to the user. 
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                xlWorkbook = xlApp.Workbooks.Open(dialog.FileName);
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

                if (NewNode.Level == DestinationNode.Level)
                    handleNodeMoving(DestinationNode.Parent, NewNode.Index, DestinationNode.Index);
            }
        }

        private void handleNodeMoving(TreeNode parent, int fromIndex, int toIndex)
        {
            int status = toIndex - fromIndex;
            List<TreeNode> nodes;

            nodes = getNodeValues(parent);

            if (status > 0)
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
                temp = nodes[i - 1];
                nodes[i - 1] = nodes[i];
                nodes[i] = temp;
            }

            return nodes;
        }

    }

}


