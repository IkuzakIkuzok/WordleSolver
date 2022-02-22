
// (c) 2021 Kazuki KOHZUKI

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace Wordle.Controls
{
    [DesignerCategory("Code")]
    internal sealed class SimulateWordForm : Form
    {
        private readonly TextBox tb_word;
        private readonly Button cancel, ok;

        internal string SimulateWord { get; private set; } = null;

        internal SimulateWordForm()
        {
            this.Text = "Simulation";
            this.Size = this.MinimumSize = this.MaximumSize = new(240, 140);
            this.MaximizeBox = false;
            this.SizeGripStyle = SizeGripStyle.Hide;

            this.tb_word = new()
            {
                Top = 20,
                Left = 20,
                Width = 190,
                MaxLength = Word.LENGTH,
                Parent = this,
            };
            this.tb_word.TextChanged += (sender, e)
                => this.ok.Enabled = this.tb_word.TextLength == Word.LENGTH && Solver.CandidateWords.Contains((Word)this.tb_word.Text);
            this.tb_word.KeyDown += (sender, e) =>
            {
                if (e.Control && e.KeyCode == Keys.A)
                    this.tb_word.SelectAll();
            };

            this.cancel = new()
            {
                Text = "Cancel",
                Top = 55,
                Left = 70,
                Size = new(60, 30),
                Parent = this,
            };
            this.cancel.Click += (sender, e) => Close();
            this.CancelButton = this.cancel;

            this.ok = new()
            {
                Text = "OK",
                Enabled = false,
                Top = 55,
                Left = 150,
                Size = new(60, 30),
                Parent = this,
            };
            this.ok.Click += (sender, e) =>
            {
                this.SimulateWord = this.tb_word.Text;
                Close();
            };
            this.AcceptButton = this.ok;
        } // ctor ()
    } // internal sealed class SimulateWordForm : Form
} // namespace Wordle.Controls
