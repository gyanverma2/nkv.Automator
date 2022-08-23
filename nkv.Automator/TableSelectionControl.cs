using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace nkv.Automator
{
    public partial class TableSelectionControl : UserControl
    {
        public TableSelectionControl()
        {
            InitializeComponent();
        }
        public List<string> TableList { get { return tableListBox.Items.Cast<string>().ToList(); } }
        public List<string> SelectedTableList { get { return SelectedTableListBox.Items.Cast<string>().ToList(); } }
        public void SetTableList(List<string> tables)
        {
            tableListBox.Items.Clear();
            foreach (var t in tables)
            {
                tableListBox.Items.Add(t);
            }
        }
        private void selectAllTableCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (selectAllTableCheckBox.Checked)
            {
                for (int intCount = tableListBox.Items.Count - 1; intCount >= 0; intCount--)
                {
                    SelectedTableListBox.Items.Add(tableListBox.Items[intCount]);
                    tableListBox.Items.Remove(tableListBox.Items[intCount]);
                }
            }
            else
            {
                for (int intCount = SelectedTableListBox.Items.Count - 1; intCount >= 0; intCount--)
                {
                    tableListBox.Items.Add(SelectedTableListBox.Items[intCount]);
                    SelectedTableListBox.Items.Remove(SelectedTableListBox.Items[intCount]);
                }

            }
        }
        private void addSelectionButton_Click(object sender, EventArgs e)
        {
            for (int intCount = tableListBox.SelectedItems.Count - 1; intCount >= 0; intCount--)
            {
                SelectedTableListBox.Items.Add(tableListBox.SelectedItems[intCount]);
                tableListBox.Items.Remove(tableListBox.SelectedItems[intCount]);
            }

        }
        private void removeButton_Click(object sender, EventArgs e)
        {
            for (int intCount = SelectedTableListBox.SelectedItems.Count - 1; intCount >= 0; intCount--)
            {
                tableListBox.Items.Add(SelectedTableListBox.SelectedItems[intCount]);
                SelectedTableListBox.Items.Remove(SelectedTableListBox.SelectedItems[intCount]);
            }
        }
    }
}
