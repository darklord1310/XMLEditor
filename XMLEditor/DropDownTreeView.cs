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
    public class DropDownTreeView : TreeView
    {
        public DropDownTreeView()
            : base()
        {
        }

        private DropDownTreeNode m_CurrentNode = null;

        public void ExpandNodeComboBox(TreeNode selectedNode)
        {
            // Are we dealing with a dropdown node?
            if (selectedNode is DropDownTreeNode)
            {
                this.m_CurrentNode = (DropDownTreeNode)selectedNode;

                // Need to add the node's ComboBox to the
                // TreeView's list of controls for it to work
                this.Controls.Add(this.m_CurrentNode.ComboBox);

                // Set the bounds of the ComboBox, with
                // a little adjustment to make it look right
                this.m_CurrentNode.ComboBox.SetBounds(
                    this.m_CurrentNode.Bounds.X - 1,
                    this.m_CurrentNode.Bounds.Y - 2,
                    this.m_CurrentNode.Bounds.Width + 25,
                    this.m_CurrentNode.Bounds.Height - 200);

                // Listen to the SelectedValueChanged
                // event of the node's ComboBox
                this.m_CurrentNode.ComboBox.SelectedValueChanged +=
                   new EventHandler(ComboBox_SelectedValueChanged);
                this.m_CurrentNode.ComboBox.DropDownClosed +=
                   new EventHandler(ComboBox_DropDownClosed);

                // Now show the ComboBox
                this.m_CurrentNode.ComboBox.Show();
                this.m_CurrentNode.ComboBox.DroppedDown = true;
            }
            //base.OnNodeMouseClick(e);
        }

        void ComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (this.m_CurrentNode != null)
            {
                // Unregister the event listener
                this.m_CurrentNode.ComboBox.SelectedValueChanged -=
                                     ComboBox_SelectedValueChanged;
                this.m_CurrentNode.ComboBox.DropDownClosed -=
                                     ComboBox_DropDownClosed;

                // Copy the selected text from the ComboBox to the TreeNode
                this.m_CurrentNode.ComboBox.MouseClick += comboBox_MouseClick;


                // Hide the ComboBox
                this.m_CurrentNode.ComboBox.Hide();
                this.m_CurrentNode.ComboBox.DroppedDown = false;

                // Remove the control from the TreeView's
                // list of currently-displayed controls
                this.Controls.Remove(this.m_CurrentNode.ComboBox);

                // And return to the default state (no ComboBox displayed)
                this.m_CurrentNode = null;
            }
        }


        void comboBox_MouseClick(object sender, EventArgs e)
        {
            this.m_CurrentNode.Text = this.m_CurrentNode.ComboBox.SelectedValue.ToString();
        }

        void ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (this.m_CurrentNode != null)
            {
                // Unregister the event listener
                this.m_CurrentNode.ComboBox.SelectedValueChanged -=
                                     ComboBox_SelectedValueChanged;
                this.m_CurrentNode.ComboBox.DropDownClosed -=
                                     ComboBox_DropDownClosed;

                // Copy the selected text from the ComboBox to the TreeNode
                this.m_CurrentNode.Text = this.m_CurrentNode.ComboBox.Text;

                // Hide the ComboBox
                this.m_CurrentNode.ComboBox.Hide();
                this.m_CurrentNode.ComboBox.DroppedDown = false;

                // Remove the control from the TreeView's
                // list of currently-displayed controls
                this.Controls.Remove(this.m_CurrentNode.ComboBox);

                // And return to the default state (no ComboBox displayed)
                this.m_CurrentNode = null;
            }
        }
    }
}
