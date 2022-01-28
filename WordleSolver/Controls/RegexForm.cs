
// (c) 2021 Kazuki KOHZUKI

using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Wordle.Controls
{
    [DesignerCategory("Code")]
    internal sealed class RegexForm : Form
    {
        private readonly CustomTextBox tb_pattern;
        private readonly CheckBox cb_inherit;
        private readonly RegexResultTable results;
        private readonly Label lb_count;

        internal RegexForm()
        {
            this.Text = "Regex Serch";
            this.Size = this.MinimumSize = this.MaximumSize = new(300, 500);
            this.MaximizeBox = false;

            _ = new Label()
            {
                Text = "Pattern",
                Top = 20,
                Left = 20,
                Width = 60,
                Parent = this,
            };

            this.tb_pattern = new()
            {
                Top = 20,
                Left = 80,
                Width = 120,
                Parent = this,
            };
            this.tb_pattern.DelayedTextChanged += UpdateList;

            this.cb_inherit = new()
            {
                Text = "Inherit filters from main form",
                Top = 50,
                Left = 20,
                Width = 250,
                Parent = this,
            };
            this.cb_inherit.CheckedChanged += UpdateList;

            this.results = new()
            {
                Top = 90,
                Left = 20,
                Width = 240,
                Height = 320,
                
                Parent = this,
            };

            this.lb_count = new()
            {
                Text = "-",
                Top = 420,
                Left = 20,
                Width = 240,
                TextAlign = ContentAlignment.MiddleRight,
                Parent = this,
            };

            Solver.CandidatesUpdated += UpdateList;
        } // ctor ()

        private void UpdateList(object sender, EventArgs e)
        {
            if (sender == null && !this.cb_inherit.Checked) return;

            using var _ = new ControlDrawingSuspender(this);
            var words = Solver.Regex(this.tb_pattern.Text, this.cb_inherit.Checked).NormalizePriorities();
            this.results.Rows.Clear();
            foreach (var word in words)
                this.results.Rows.Add(word.Key, $"{word.Value:F2}");
            this.lb_count.Text = $"{words.Count()} word(s)";
        } // private void UpdateList (object, EventArgs)
    } // internal sealed class RegexForm : Form
} // namespace Wordle.Controls
