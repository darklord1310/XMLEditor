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

        private void deleteNode(TreeNode node)
        {
            node.Remove();
        }

        void contextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripItem item = e.ClickedItem;
            
            if(item.ToString() == "Edit" )
            {
                //DropDownTreeView bla = new DropDownTreeView();
                treeView1.ExpandNodeComboBox(treeView1.SelectedNode);
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
                if(treeView1.GetNodeAt(e.X,e.Y) != null)
                    treeView1.SelectedNode = treeView1.GetNodeAt(e.X, e.Y);         // get the tree node on mouse right click
                treeView1.SelectedNode.BackColor = SystemColors.HighlightText;      // highlight the selected node
                Point p = new Point(treeView1.SelectedNode.Bounds.Right + 15, treeView1.SelectedNode.Bounds.Bottom + 25);
                createContextMenuStrip(treeView1.SelectedNode.Level);
                docMenu.Show(PointToScreen(p));     
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
            if (nodeLevel == 4)
                docMenu.Items.AddRange(new ToolStripMenuItem[] { addLabel, deleteLabel });
            else
                docMenu.Items.AddRange(new ToolStripMenuItem[] { addLabel, deleteLabel, renameLabel });

            docMenu.ItemClicked += new ToolStripItemClickedEventHandler(contextMenu_ItemClicked);
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
    }
}
