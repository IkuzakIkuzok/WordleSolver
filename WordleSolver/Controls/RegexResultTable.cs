
// (c) 2021 Kazuki KOHZUKI

using System.ComponentModel;
using System.Windows.Forms;

namespace Wordle.Controls
{
    [DesignerCategory("Code")]
    internal sealed class RegexResultTable : DataGridView
    {
        internal RegexResultTable()
        {
            this.ScrollBars = ScrollBars.Vertical;
            this.AllowUserToDeleteRows = false;
            this.AllowUserToAddRows = false;
            this.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.RowHeadersWidth = 30;
            this.AutoGenerateColumns = false;

            var word = new DataGridViewTextBoxColumn()
            {
                Name = "Word",
                ReadOnly = true,
                Resizable = DataGridViewTriState.False,
                Width = 100,
            };
            this.Columns.Add(word); // 0

            var priority = new DataGridViewTextBoxColumn()
            {
                Name = "Priority",
                ReadOnly = true,
                Resizable = DataGridViewTriState.False,
                Width = 120,
            };
            priority.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.Columns.Add(priority); // 1
        } // ctor ()

        override protected void OnSortCompare(DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.DisplayIndex == 0)
            {
                e.Handled = false;
                return;
            }

            var p1 = float.Parse(e.CellValue1.ToString());
            var p2 = float.Parse(e.CellValue2.ToString());
            e.Handled = true;
            e.SortResult = p1.CompareTo(p2);
        } // override protected void OnSortCompare (DataGridViewSortCompareEventArgs)
    } // internal sealed class RegexResultTable : DataGridView
} // namespace Wordle.Controls
