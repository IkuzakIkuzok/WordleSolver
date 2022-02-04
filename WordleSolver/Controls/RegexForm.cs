
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
        private readonly CheckBox cb_inherit, cb_inclideValids;
        private readonly RegexResultTable results;
        private readonly Label lb_count;

        internal RegexForm()
        {
            this.Text = "Regex Serch";
            this.Size = this.MinimumSize = this.MaximumSize = new(300, 530);
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

            this.cb_inclideValids = new()
            {
                Text = "Inclide miscellaneous words",
                Top = 80,
                Left = 20,
                Width = 250,
                Parent = this,
            };
            this.cb_inclideValids.CheckedChanged += UpdateList;

            this.results = new()
            {
                Top = 120,
                Left = 20,
                Width = 240,
                Height = 320,
                
                Parent = this,
            };

            this.lb_count = new()
            {
                Text = "-",
                Top = 450,
                Left = 20,
                Width = 240,
                TextAlign = ContentAlignment.MiddleRight,
                Parent = this,
            };

            Solver.CandidatesUpdated += UpdateList;
            Solver.AlgorithmChanged += UpdateOnAlgorithmChanged;

            FormClosed += UnregisterEventHandlers;
        } // ctor ()

        private void UnregisterEventHandlers(object sender, EventArgs e)
        {
            Solver.CandidatesUpdated -= UpdateList;
            Solver.AlgorithmChanged -= UpdateOnAlgorithmChanged;
        } // private void UnregisterEventHandlers (object, EventArgs)

        private void UpdateList(object sender, EventArgs e)
        {
            if (sender == null && !this.cb_inherit.Checked) return;
            UpdateList();
        } // private void UpdateList (object, EventArgs)

        private void UpdateOnAlgorithmChanged(object sender, EventArgs e)
            => UpdateList();

        private void UpdateList()
        {
            using var _ = new ControlDrawingSuspender(this);
            this.results.Rows.Clear();

            var words = Solver.Regex(this.tb_pattern.Text, this.cb_inclideValids.Checked, out var succeeded, this.cb_inherit.Checked);
            this.tb_pattern.ForeColor = succeeded ? Color.Black : Color.Red;
            this.lb_count.Text = succeeded ? $"{words.Count()} word(s)" : "Regex error";
            if (!succeeded) return;

            if (!Solver.UseEntropy)
                words = words.NormalizePriorities();      
            foreach (var word in words)
                this.results.Rows.Add(word.Key, $"{word.Value:F2}");
        } // private void UpdateList ()
    } // internal sealed class RegexForm : Form
} // namespace Wordle.Controls
