﻿using System;
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
using System.Reflection;

namespace XMLEditor
{
    public partial class Form1 : Form
    {
        int num = 0;
        string filename = "V5S_Diag Command";
        string moduleName = string.Empty;
        string moduleID = string.Empty;
        string funcName = string.Empty;
        string comboBoxSelectedItem = string.Empty;
        TestMenu[] tm = new TestMenu[7];
        Category cat = new Category();

        private string NodeMap;
        private const int MAPSIZE = 128;
        private StringBuilder NewNodeMap = new StringBuilder(MAPSIZE);

        Excel.Application xlApp = new Excel.Application();
        Excel.Workbook xlWorkbook;
        Excel._Worksheet xlWorksheet;
        Excel.Range xlRange;

        string appPath, folderPath, xmlPath;
        ContextMenuStrip docMenu;
        enum Images { NODE, ADD, DELETE, EDIT };

        public class ShowErrorMessageException : Exception
        {
            public ShowErrorMessageException(string message)
            {
                MessageBox.Show(message);
            }
        }

        public Form1()
        {
            InitializeComponent();
            this.treeView1.ImageList = TreeviewIL;
            createTreeView();
            createXmlFolderPath();
            cBoxFunc.DropDownStyle = ComboBoxStyle.DropDownList;
            createXML.Enabled = false;
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
                    throw new ShowErrorMessageException("No Excel selected!");
           
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
                    if (node.Level == 2)
                    {
                        cat.setCategory(node.Parent.Text);
                        cat.setModule(node.Text);
                        dNode = createNormalTreeNode("TC_" + (node.Nodes.Count + 1).ToString("D4"));
                        txtBoxTcDesc.ReadOnly = false;
                        txtBoxTcDesc.Text = string.Empty;
                        txtBoxTcDesc.Focus();
                        lblTcDesc.Text += " (Press enter to continue)";
                        treeView1.Enabled = false;
                        cat.tc.Add(new TestCase());
                        cat.tc[node.Nodes.Count].setTcNo(dNode.Text);
                    }
                    else
                    {
                        dNode = createNormalTreeNode("SN_" + (node.Nodes.Count + 1).ToString("D4"));
                        txtBoxSqDesc.ReadOnly = false;
                        txtBoxSqDesc.Focus();
                        txtBoxSqDesc.Text = string.Empty;
                        txtBoxPara.Text = string.Empty;
                        txtBoxExpOut.Text = string.Empty;
                        lblSqDesc.Text += " (Press enter to continue)";
                        treeView1.Enabled = false;
                        cat.tc[node.Index].seqNo.Add(new SqNum());
                        cat.tc[node.Index].seqNo[node.Nodes.Count].setSeqNo(dNode.Text.Substring(dNode.Text.LastIndexOf('_') + 1));
                        updateDiagCmd(node);
                    }

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

                        if(node.Level == 1)                 // if module is added then only enable the write to xml button
                            createXML.Enabled = true;
                    }
                    else
                    {
                        showMsgBox("No module available for " + node.Text + "!", MessageBoxIcon.Warning);
                    }
                }
            }
            catch (ShowErrorMessageException ex) { }
        }

        private void updateDiagCmd(TreeNode node)
        {
            try
            {
                int num = getTestMenuNum(node.Parent.Parent.Text);
                txtBoxCat.Text = tm[num].getCategoryID().ToString();
                string[] modules = tm[num].getModuleName().Split('|');
                string[] moduleID = tm[num].getModuleID().Split('|');
                int index = getModuleId(modules, node.Parent.Text);
                if (index != 99)
                    txtBoxMod.Text = moduleID[index];
                else
                    throw new ShowErrorMessageException("Module not available in class!");
                string[] funcNames = tm[num].getFuncName().Split('|');
                string[] cFuncNames = funcNames[index].Split(',');
                cBoxFunc.Items.Clear();

                for (int i = 0; i < cFuncNames.Count(); i++)
                {
                    cBoxFunc.Items.Add(cFuncNames[i]);
                }
            }
            catch (ShowErrorMessageException ex) { }
        }

        private int getModuleId(string[] modules, string module)
        {
            for(int i = 0; i < modules.Count(); i++)
            {
                if(string .Equals(modules[i], module))
                {
                    return i;
                }
            }

            return 99;
        }

        private void clearAllTextbox()
        {
            txtBoxTcDesc.Text = string.Empty;
            txtBoxSqDesc.Text = string.Empty;
            txtBoxCat.Text = string.Empty;
            txtBoxMod.Text = string.Empty;
            txtBoxPara.Text = string.Empty;
            txtBoxExpOut.Text = string.Empty;
        }

        private void deleteNode(TreeNode node)
        {
            TreeNode parent = node.Parent;
            int index = node.Index;
            node.Remove();

            if(node.Level < 1)
                createXML.Enabled = false;

            if (index == 3)
                cat.tc.RemoveAt(node.Index);
            else if (index == 4)
                cat.tc[node.Parent.Index].seqNo.RemoveAt(node.Index);

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
            if (node.Nodes.Count > 0)
            {
                for (int i = node.Nodes.Count - 1; i >= 0; i--)
                {
                    node.Nodes[i].Remove();
                }
            }
        }

        void contextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripItem item = e.ClickedItem;

            if (item.ToString() == "Edit")
            {
                treeView1.BeginUpdate();

                if (treeView1.SelectedNode.Level < 3)
                {
                    if (treeView1.SelectedNode.Nodes.Count > 0)
                    {
                        if (MessageBox.Show("Are you sure?\nAll child nodes will be remove", "Confirmation",
                                             MessageBoxButtons.YesNo,
                                             MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            removeAllChildNode(treeView1.SelectedNode);
                        }
                    }
                    treeView1.ExpandNodeComboBox(treeView1.SelectedNode);
                }
                else
                {
                    treeView1.Enabled = false;

                    if (treeView1.SelectedNode.Level == 3)
                    {
                        txtBoxTcDesc.ReadOnly = false;
                        txtBoxTcDesc.Focus();
                        lblTcDesc.Text += " (Press enter to continue)";
                    }
                    else
                    {
                        txtBoxSqDesc.ReadOnly = false;
                        txtBoxSqDesc.Focus();
                        lblSqDesc.Text += " (Press enter to continue)";
                    }
                }

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
                    Point p = new Point(treeView1.SelectedNode.Bounds.Right + 16, treeView1.SelectedNode.Bounds.Bottom + 26);
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
                showMsgBox("AppData folder not found. It will be created automatically", MessageBoxIcon.Information);
                Directory.CreateDirectory(folderPath);
            }
            else
            {
                if(File.Exists(Path.Combine(folderPath, filename + ".xls")) )
                {
                    xlWorkbook = xlApp.Workbooks.Open(Path.Combine(folderPath, filename));
                    Progress_Bar f = new Progress_Bar();
                    f.StartPosition = FormStartPosition.Manual;
                    f.Location = new Point(Location.X + (Width - f.Width) / 2, Location.Y + (Height - f.Height) / 2);
                    f.Show(this);         //Make sure we're the owner
                    this.Enabled = false; //Disable ourselves
                    extractDataFromExcel();
                    xlWorkbook.Close();
                    this.Enabled = true;  //We're done, enable ourselves
                    f.Close();            //Dispose message form
                }
                else
                {
                    showMsgBox("Excel file with name " + filename + " not found in AppData folder. Please load it manually.", MessageBoxIcon.Warning);
                }
            }
        }


        public void createXmlFolderPath()
        {
            appPath = Path.GetDirectoryName(Application.ExecutablePath);  // get the root path of the dir
            xmlPath = Path.Combine(appPath, "XML");                // get the path to the AppData folder

            if (!Directory.Exists(xmlPath))
            {
                Directory.CreateDirectory(xmlPath);
            }
        }

        private void createContextMenuStrip(int nodeLevel)
        {
            // Create the ContextMenuStrip.
            docMenu = new ContextMenuStrip();

            //Create some menu items.
            ToolStripMenuItem deleteLabel = new ToolStripMenuItem();
            deleteLabel.Text = "Delete";
            deleteLabel.Image = TreeviewIL.Images[(int)Images.DELETE];
            ToolStripMenuItem renameLabel = new ToolStripMenuItem();
            renameLabel.Text = "Edit";
            renameLabel.Image = TreeviewIL.Images[(int)Images.EDIT];
            ToolStripMenuItem addLabel = new ToolStripMenuItem();
            addLabel.Text = "Add";
            addLabel.Image = TreeviewIL.Images[(int)Images.ADD];

            //Add the menu items to the menu.
            if (nodeLevel == 4) //Sequence Number
                docMenu.Items.AddRange(new ToolStripMenuItem[] { renameLabel, deleteLabel });
            else if (nodeLevel == 0 && treeView1.Nodes["TestMenu"].Nodes.Count > 0)
                docMenu.Items.AddRange(new ToolStripMenuItem[] { });
            else if (nodeLevel == 1 && treeView1.Nodes["TestMenu"].Nodes[0].Nodes.Count > 0)
                docMenu.Items.AddRange(new ToolStripMenuItem[] { renameLabel, deleteLabel });
            else if (nodeLevel == 0)    //Test Menu
                docMenu.Items.AddRange(new ToolStripMenuItem[] { addLabel });
            else
                docMenu.Items.AddRange(new ToolStripMenuItem[] { addLabel, renameLabel, deleteLabel });

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

            catch (ShowErrorMessageException ex) { }
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
                    throw new ShowErrorMessageException("Invalid Category!");
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
                xlWorkbook.Close();
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

                if(DestinationNode != null)
                {
                    if (NewNode.Level == DestinationNode.Level)
                    {
                        handleNodeMoving(DestinationNode.Parent, NewNode.Index, DestinationNode.Index);
                        if (NewNode.Parent.Level == 2 || NewNode.Parent.Level == 3)
                            renumberAllNodes(NewNode.Parent);
                    } 
                }
                DestinationNode.BackColor = Color.White;
            }
            this.Refresh();
        }

        private void renumberAllNodes(TreeNode parent)
        {
            string name;
            int integer = 1;

            foreach (TreeNode element in parent.Nodes)
            {
                if (parent.Level == 2)
                {
                    name = "TC_" + (integer).ToString("D4");
                    cat.tc[element.Index].setTcNo(name);
                }
                else
                {
                    name = "SN_" + (integer).ToString("D4");
                    cat.tc[element.Parent.Index].seqNo[element.Index].setSeqNo((integer).ToString("D4"));
                }
                element.Name = name;
                element.Text = name;
                integer++;
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
            removeAllChildNode(parent);
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

        private List<TreeNode> swapDown(List<TreeNode> nodes, int fromIndex, int toIndex)
        {
            TreeNode temp;
            
            for (int i = fromIndex; i < toIndex; i++)
            {
                temp = nodes[i + 1];
                nodes[i + 1] = nodes[i];
                nodes[i] = temp;
            }

            if (nodes[0].Level == 3)
                reorderTestCaseSequnce(fromIndex, toIndex, false);
            else if (nodes[0].Level == 4)
                reorderSqNumSequnce(fromIndex, toIndex, nodes[0].Parent.Index, false);

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

            if(nodes[0].Level == 3)
                reorderTestCaseSequnce(fromIndex, toIndex, true);
            else if (nodes[0].Level == 4)
                reorderSqNumSequnce(fromIndex, toIndex, nodes[0].Parent.Index, true);

            return nodes;
        }

        private void reorderTestCaseSequnce(int fromIndex, int toIndex, bool swap)
        {
            TestCase temp;
            bool swapUp = true;

            if (swap == !swapUp)
            {
                for (int i = fromIndex; i < toIndex; i++)
                {
                    temp = cat.tc[i + 1];
                    cat.tc[i + 1] = cat.tc[i];
                    cat.tc[i] = temp;
                }
            }
            else
            {
                for (int i = fromIndex; i > toIndex; i--)
                {
                    temp = cat.tc[i - 1];
                    cat.tc[i - 1] = cat.tc[i];
                    cat.tc[i] = temp;
                }
            }
        }

        private void reorderSqNumSequnce(int fromIndex, int toIndex, int tcIndex, bool swap)
        {
            SqNum temp;
            bool swapUp = true;

            if (swap == !swapUp)
            {
                for (int i = fromIndex; i < toIndex; i++)
                {
                    temp = cat.tc[tcIndex].seqNo[i + 1];
                    cat.tc[tcIndex].seqNo[i + 1] = cat.tc[tcIndex].seqNo[i];
                    cat.tc[tcIndex].seqNo[i] = temp;
                }
            }
            else
            {
                for (int i = fromIndex; i > toIndex; i--)
                {
                    temp = cat.tc[tcIndex].seqNo[i - 1];
                    cat.tc[tcIndex].seqNo[i - 1] = cat.tc[tcIndex].seqNo[i];
                    cat.tc[tcIndex].seqNo[i] = temp;
                }
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            createDataPath();
        }

        private void showMsgBox(string content, MessageBoxIcon iconSelection)
        {
            //this.Activated -= TestServerGUI_Activated;
            //this.Deactivate -= TestServerGUI_Deactivate;
            MessageBox.Show(content, "", MessageBoxButtons.OK, iconSelection);
            //this.Deactivate += TestServerGUI_Deactivate;
        }

        private void treeView1_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
        {
            TreeNode NodeOver = this.treeView1.GetNodeAt(this.treeView1.PointToClient(Cursor.Position));
            TreeNode NodeMoving = (TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode");

            if(NodeOver != null)
            {
                if (NodeOver.PrevVisibleNode != null)
                {
                    NodeOver.PrevVisibleNode.BackColor = Color.White;
                }
                if (NodeOver.NextVisibleNode != null)
                {
                    NodeOver.NextVisibleNode.BackColor = Color.White;
                }
                NodeOver.BackColor = Color.Aquamarine;
            }


            // A bit long, but to summarize, process the following code only if the nodeover is null
            // and either the nodeover is not the same thing as nodemoving UNLESSS nodeover happens
            // to be the last node in the branch (so we can allow drag & drop below a parent branch)
            if (NodeOver != null && (NodeOver != NodeMoving || (NodeOver.Parent != null && NodeOver.Index == (NodeOver.Parent.Nodes.Count - 1))))
            {
                int OffsetY = this.treeView1.PointToClient(Cursor.Position).Y - NodeOver.Bounds.Top;
                int NodeOverImageWidth = this.treeView1.ImageList.Images[(int)Images.NODE].Size.Width + 8;
                Graphics g = this.treeView1.CreateGraphics();

                if (OffsetY < (NodeOver.Bounds.Height / 2))
                {
                    #region If NodeOver is a child then cancel
                    TreeNode tnParadox = NodeOver;
                    while (tnParadox.Parent != null)
                    {
                        if (tnParadox.Parent == NodeMoving)
                        {
                            this.NodeMap = "";
                            return;
                        }

                        tnParadox = tnParadox.Parent;
                    }
                    #endregion
                    #region Store the placeholder info into a pipe delimited string
                    SetNewNodeMap(NodeOver, false);
                    if (SetMapsEqual() == true)
                        return;
                    #endregion
                    #region Clear placeholders above and below
                    this.Refresh();
                    #endregion
                    #region Draw the placeholders
                    this.DrawLeafTopPlaceholders(NodeOver);
                    #endregion
                }
                else
                {
                    #region If NodeOver is a child then cancel
                    TreeNode tnParadox = NodeOver;
                    while (tnParadox.Parent != null)
                    {
                        if (tnParadox.Parent == NodeMoving)
                        {
                            this.NodeMap = "";
                            return;
                        }

                        tnParadox = tnParadox.Parent;
                    }
                    #endregion
                    #region Allow drag drop to parent branches
                    TreeNode ParentDragDrop = null;
                    // If the node the mouse is over is the last node of the branch we should allow
                    // the ability to drop the "nodemoving" node BELOW the parent node
                    if (NodeOver.Parent != null && NodeOver.Index == (NodeOver.Parent.Nodes.Count - 1))
                    {
                        int XPos = this.treeView1.PointToClient(Cursor.Position).X;
                        if (XPos < NodeOver.Bounds.Left)
                        {
                            ParentDragDrop = NodeOver.Parent;

                            if (XPos < (ParentDragDrop.Bounds.Left - this.treeView1.ImageList.Images[ParentDragDrop.ImageIndex].Size.Width))
                            {
                                if (ParentDragDrop.Parent != null)
                                    ParentDragDrop = ParentDragDrop.Parent;
                            }
                        }
                    }
                    #endregion
                    #region Store the placeholder info into a pipe delimited string
                    // Since we are in a special case here, use the ParentDragDrop node as the current "nodeover"
                    SetNewNodeMap(ParentDragDrop != null ? ParentDragDrop : NodeOver, true);
                    if (SetMapsEqual() == true)
                        return;
                    #endregion
                    #region Clear placeholders above and below
                    this.Refresh();
                    #endregion
                    #region Draw the placeholders
                    DrawLeafBottomPlaceholders(NodeOver, ParentDragDrop);
                    #endregion
                }
            }
        }

        private void txtBoxPara_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (string.IsNullOrEmpty(txtBoxPara.Text))
                {
                    MessageBox.Show("Please enter the parmeter!");
                    txtBoxSqDesc.Text = string.Empty;
                }
                else
                {
                    if (treeView1.SelectedNode.Level == 3)
                        cat.tc[treeView1.SelectedNode.Index].seqNo[treeView1.SelectedNode.Nodes.Count - 1].setPara(txtBoxPara.Text);
                    else
                        cat.tc[treeView1.SelectedNode.Parent.Index].seqNo[treeView1.SelectedNode.Index].setPara(txtBoxPara.Text);
                    txtBoxPara.ReadOnly = true;
                    lblPara.Text = "Parameter";
                    txtBoxExpOut.ReadOnly = false;
                    txtBoxExpOut.Focus();
                    lblExpOut.Text += " (Press enter to continue)";
                }
            }
        }

        private void DrawLeafTopPlaceholders(TreeNode NodeOver)
        {
            Graphics g = this.treeView1.CreateGraphics();

            int NodeOverImageWidth = this.treeView1.ImageList.Images[(int)Images.NODE].Size.Width + 8;
            int LeftPos = NodeOver.Bounds.Left - NodeOverImageWidth;
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

            int NodeOverImageWidth = this.treeView1.ImageList.Images[(int)Images.NODE].Size.Width + 8;
            // Once again, we are not dragging to node over, draw the placeholder using the ParentDragDrop bounds
            int LeftPos, RightPos;
            if (ParentDragDrop != null)
                LeftPos = ParentDragDrop.Bounds.Left - (this.treeView1.ImageList.Images[ParentDragDrop.ImageIndex].Size.Width + 8);
            else
                LeftPos = NodeOver.Bounds.Left - NodeOverImageWidth;
            RightPos = this.treeView1.Width - 4;

            Point[] LeftTriangle = new Point[5]{
												   new Point(LeftPos, NodeOver.Bounds.Bottom - 4),
												   new Point(LeftPos, NodeOver.Bounds.Bottom + 4),
												   new Point(LeftPos + 4, NodeOver.Bounds.Bottom),
												   new Point(LeftPos + 4, NodeOver.Bounds.Bottom - 1),
												   new Point(LeftPos, NodeOver.Bounds.Bottom - 5)};

            Point[] RightTriangle = new Point[5]{
													new Point(RightPos, NodeOver.Bounds.Bottom - 4),
													new Point(RightPos, NodeOver.Bounds.Bottom + 4),
													new Point(RightPos - 4, NodeOver.Bounds.Bottom),
													new Point(RightPos - 4, NodeOver.Bounds.Bottom - 1),
													new Point(RightPos, NodeOver.Bounds.Bottom - 5)};


            g.FillPolygon(System.Drawing.Brushes.Black, LeftTriangle);
            g.FillPolygon(System.Drawing.Brushes.Black, RightTriangle);
            g.DrawLine(new System.Drawing.Pen(Color.Black, 2), new Point(LeftPos, NodeOver.Bounds.Bottom), new Point(RightPos, NodeOver.Bounds.Bottom));
        }//eom

        private void SetNewNodeMap(TreeNode tnNode, bool boolBelowNode)
        {
            NewNodeMap.Length = 0;

            if (boolBelowNode)
                NewNodeMap.Insert(0, (int)tnNode.Index + 1);
            else
                NewNodeMap.Insert(0, (int)tnNode.Index);
            TreeNode tnCurNode = tnNode;

            while (tnCurNode.Parent != null)
            {
                tnCurNode = tnCurNode.Parent;

                if (NewNodeMap.Length == 0 && boolBelowNode == true)
                {
                    NewNodeMap.Insert(0, (tnCurNode.Index + 1) + "|");
                }
                else
                {
                    NewNodeMap.Insert(0, tnCurNode.Index + "|");
                }
            }
        }//oem

        private bool SetMapsEqual()
        {
            if (this.NewNodeMap.ToString() == this.NodeMap)
                return true;
            else
            {
                this.NodeMap = this.NewNodeMap.ToString();
                return false;
            }
        }

        // create xml button click
        private void createXML_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            XMLWriter writer = new XMLWriter();
            string path = Path.Combine(xmlPath, cat.getCategory() + "_" + cat.getModule() + ".xml");
            int status = writer.writeToXML(cat.getCategory(), cat.getModule(), cat.tc, path);
            this.Cursor = Cursors.Default;

            if (status == 1)
                showMsgBox(cat.getCategory() + "_" + cat.getModule() + ".xml" + " created successfully.", MessageBoxIcon.Information);
            else
                showMsgBox("Error occured! " + cat.getCategory() + "_" + cat.getModule() + ".xml" + " is not created.", MessageBoxIcon.Error);
        }


        private void txtBoxTcDesc_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (string.IsNullOrEmpty(txtBoxTcDesc.Text))
                {
                    MessageBox.Show("Please enter the description!");
                    txtBoxTcDesc.Text = string.Empty;
                }
                else
                {
                    if (treeView1.SelectedNode.Level == 2)
                        cat.tc[treeView1.SelectedNode.Nodes.Count - 1].setDesc(txtBoxTcDesc.Text);
                    else
                        cat.tc[treeView1.SelectedNode.Index].setDesc(txtBoxTcDesc.Text);
                    treeView1.Enabled = true;
                    txtBoxTcDesc.ReadOnly = true;
                    lblTcDesc.Text = "Test Case Description";
                    //displayTestCaseClass();
                }
            }
        }

        private void txtBoxSqDesc_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (string.IsNullOrEmpty(txtBoxSqDesc.Text))
                {
                    MessageBox.Show("Please enter the description!");
                    txtBoxSqDesc.Text = string.Empty;
                }
                else
                {
                    if (treeView1.SelectedNode.Level == 3)
                        cat.tc[treeView1.SelectedNode.Index].seqNo[treeView1.SelectedNode.Nodes.Count - 1].setDesc(txtBoxSqDesc.Text);
                    else
                        cat.tc[treeView1.SelectedNode.Parent.Index].seqNo[treeView1.SelectedNode.Index].setDesc(txtBoxSqDesc.Text);
                    txtBoxSqDesc.ReadOnly = true;
                    lblSqDesc.Text = "Seq No Description";
                    cBoxFunc.Enabled = true;
                    cBoxFunc.DroppedDown = true;
                }
            }
        }

        private void txtBoxExpOut_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (string.IsNullOrEmpty(txtBoxExpOut.Text))
                {
                    MessageBox.Show("Please enter the expected outcome!");
                    txtBoxSqDesc.Text = string.Empty;
                }
                else
                {
                    if (treeView1.SelectedNode.Level == 3)
                        cat.tc[treeView1.SelectedNode.Index].seqNo[treeView1.SelectedNode.Nodes.Count - 1].setExpected(txtBoxExpOut.Text);
                    else
                        cat.tc[treeView1.SelectedNode.Parent.Index].seqNo[treeView1.SelectedNode.Index].setExpected(txtBoxExpOut.Text);
                    txtBoxExpOut.ReadOnly = true;
                    lblExpOut.Text = "Expected Outcome";
                    txtBoxExpOut.ReadOnly = true;
                    cBoxFunc.Enabled = false;
                    treeView1.Enabled = true;
                    //displaySeqNumClass();
                }
            }
        }

        private void cBoxFunc_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cBoxFunc.SelectedItem != null && !string.Equals(comboBoxSelectedItem, cBoxFunc.SelectedItem.ToString()))
            {
                if (treeView1.SelectedNode.Level == 3)
                    cat.tc[treeView1.SelectedNode.Index].seqNo[treeView1.SelectedNode.Nodes.Count - 1].setDiagCmd(txtBoxCat.Text + txtBoxMod.Text + cBoxFunc.SelectedItem.ToString());
                else
                    cat.tc[treeView1.SelectedNode.Parent.Index].seqNo[treeView1.SelectedNode.Index].setDiagCmd(txtBoxCat.Text + txtBoxMod.Text + cBoxFunc.SelectedItem.ToString());

                if (treeView1.SelectedNode.Level == 3)
                {
                    txtBoxPara.ReadOnly = false;
                    txtBoxPara.Focus();
                    txtBoxExpOut.ReadOnly = true;
                    txtBoxPara.Text = string.Empty;
                    txtBoxExpOut.Text = string.Empty;
                    lblPara.Text = "Parameter";
                    lblExpOut.Text = "Expected Outcome";
                    lblPara.Text += " (Press enter to continue)";
                }
            }
        }

        private void cBoxFunc_DropDown(object sender, EventArgs e)
        {
            if (cBoxFunc.SelectedItem != null)
            {
                comboBoxSelectedItem = cBoxFunc.SelectedItem.ToString();
            }
        }

        private void displayTestCaseClass()
        {
            if (treeView1.SelectedNode.Level == 2)
                MessageBox.Show(cat.tc[treeView1.SelectedNode.Nodes.Count - 1].getDesc());
            else
                MessageBox.Show(cat.tc[treeView1.SelectedNode.Index].getDesc());
        }

        private void displaySeqNumClass()
        {
            if (treeView1.SelectedNode.Level == 3)
            {
                MessageBox.Show(cat.tc[treeView1.SelectedNode.Index].seqNo[treeView1.SelectedNode.Nodes.Count - 1].getSeqNo() + "\n" +
                                cat.tc[treeView1.SelectedNode.Index].seqNo[treeView1.SelectedNode.Nodes.Count - 1].getDesc() + "\n" +
                                cat.tc[treeView1.SelectedNode.Index].seqNo[treeView1.SelectedNode.Nodes.Count - 1].getDiagCmd() + "\n" +
                                cat.tc[treeView1.SelectedNode.Index].seqNo[treeView1.SelectedNode.Nodes.Count - 1].getPara() + "\n" +
                                cat.tc[treeView1.SelectedNode.Index].seqNo[treeView1.SelectedNode.Nodes.Count - 1].getExpected() + "\n");
            }
            else
            {
                MessageBox.Show(cat.tc[treeView1.SelectedNode.Parent.Index].seqNo[treeView1.SelectedNode.Index].getSeqNo() + "\n" +
                                cat.tc[treeView1.SelectedNode.Parent.Index].seqNo[treeView1.SelectedNode.Index].getDesc() + "\n" +
                                cat.tc[treeView1.SelectedNode.Parent.Index].seqNo[treeView1.SelectedNode.Index].getDiagCmd() + "\n" +
                                cat.tc[treeView1.SelectedNode.Parent.Index].seqNo[treeView1.SelectedNode.Index].getPara() + "\n" +
                                cat.tc[treeView1.SelectedNode.Parent.Index].seqNo[treeView1.SelectedNode.Index].getExpected() + "\n");
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            clearAllTextbox();

            if (treeView1.SelectedNode.Level == 3)
            {
                txtBoxTcDesc.Text = cat.tc[treeView1.SelectedNode.Index].getDesc();
                cBoxFunc.SelectedItem = null;
            }
            else if (treeView1.SelectedNode.Level == 4)
            {
                string funcName = cat.tc[treeView1.SelectedNode.Parent.Index].seqNo[treeView1.SelectedNode.Index].getDiagCmd().Substring(4);
                txtBoxTcDesc.Text = cat.tc[treeView1.SelectedNode.Parent.Index].getDesc();
                txtBoxSqDesc.Text = cat.tc[treeView1.SelectedNode.Parent.Index].seqNo[treeView1.SelectedNode.Index].getDesc();
                txtBoxCat.Text = cat.tc[treeView1.SelectedNode.Parent.Index].seqNo[treeView1.SelectedNode.Index].getDiagCmd().Substring(0, 1);
                txtBoxMod.Text = cat.tc[treeView1.SelectedNode.Parent.Index].seqNo[treeView1.SelectedNode.Index].getDiagCmd().Substring(1, 3);
                cBoxFunc.SelectedItem = funcName;
                txtBoxPara.Text = cat.tc[treeView1.SelectedNode.Parent.Index].seqNo[treeView1.SelectedNode.Index].getPara();
                txtBoxExpOut.Text = cat.tc[treeView1.SelectedNode.Parent.Index].seqNo[treeView1.SelectedNode.Index].getExpected();
            }
        }
    }
}


