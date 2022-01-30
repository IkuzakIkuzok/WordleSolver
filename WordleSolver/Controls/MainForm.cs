
// (c) 2021 Kazuki KOHZUKI

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace Wordle.Controls
{
    [DesignerCategory("Code")]
    internal sealed class MainForm : Form
    {
        private const int CYCLE = 6;

        private readonly CheckBox cb_entropy;
        private readonly ResultInput[] inputs;
        private int round = 0;

        internal MainForm()
        {
            this.Text = "WordleSolver";
            this.Size = this.MinimumSize = this.MaximumSize = new(430, 580);
            this.MaximizeBox = false;

            this.cb_entropy = new()
            {
                Text = "Use entropy instead of simplified score",
                Top = 40,
                Left = 20,
                Width = 300,
                Parent = this,
            };
            this.cb_entropy.CheckedChanged += (sender, e) =>
            {
                Solver.UseEntropy = this.cb_entropy.Checked;
                if (this.round > 0)
                    UpdateRound();
            };

            this.inputs = new ResultInput[CYCLE];
            for (var i = 0; i < CYCLE; i++)
            {
                this.inputs[i] = new()
                {
                    Text = (i + 1).ToString(),
                    Top = 70 * i + 70,
                    Left = 20,
                    Parent = this,
                };
                this.inputs[i].Submitted += MoveNext;
            }

            var reset = new Button()
            {
                Text = "Reset",
                Top = 490,
                Left = 270,
                Size = new(60, 30),
                Parent = this,
            };
            reset.Click += Reset;

            var close = new Button()
            {
                Text = "Close",
                Top = 490,
                Left = 340,
                Size = new(60, 30),
                Parent = this,
            };
            close.Click += (sender, e) => Close();

            #region menu

            var ms = new MenuStrip()
            {
                Parent = this,
            };

            var file = new ToolStripMenuItem()
            {
                Text = "&File",
            };
            ms.Items.Add(file);

            var openGame = new ToolStripMenuItem()
            {
                Text = "&Open game",
                ShortcutKeys = Keys.Control | Keys.O,
            };
            openGame.Click += (sender, e) => Kernel.OpenBrowser(Kernel.GameUrl);
            file.DropDownItems.Add(openGame);

            file.DropDownItems.Add(new ToolStripSeparator());

            var exit = new ToolStripMenuItem()
            {
                Text = "E&xit",
                ShortcutKeys = Keys.Alt | Keys.F4,
            };
            exit.Click += (sender, e) => Close();
            file.DropDownItems.Add(exit);

            var tool = new ToolStripMenuItem()
            {
                Text = "&Tool",
            };
            ms.Items.Add(tool);

            var regex = new ToolStripMenuItem()
            {
                Text = "&Regex",
                ShortcutKeys = Keys.Control | Keys.F,
            };
            regex.Click += (sender, e) => new RegexForm().Show();
            tool.DropDownItems.Add(regex);

            var help = new ToolStripMenuItem()
            {
                Text = "&Help",
            };
            ms.Items.Add(help);

            var openGitHub = new ToolStripMenuItem()
            {
                Text = "Open &GitHub repository",
            };
            openGitHub.Click += (sender, e) => Kernel.OpenBrowser(Kernel.GitHubUrl);
            help.DropDownItems.Add(openGitHub);

            #endregion menu

            Reset();
        } // ctor ()

        private void Reset(object sender, EventArgs e)
            => Reset();

        internal void Reset()
        {
            this.round = 0;
            Solver.Reset();
            foreach (var input in this.inputs)
            {
                input.Enabled = true;
                input.Reset();
            }
            this.inputs[0].SetAsFirst();
        } // internal void Reset ()

        private void MoveNext(object sender, EventArgs e)
        {
            if (sender is not ResultInput input) return;
            this.round += 1;
            if (this.round >= CYCLE) return;

            var filter = input.Filter;
            Solver.ApplyFilter(filter);

            UpdateRound();
            
            var indices = filter.CorrectIndices;
            for (var i = this.round; i < CYCLE; i++)
                this.inputs[i].Seal(indices);
        } // private void MoveNext (object, EventArgs)

        private void UpdateRound()
        {
            this.inputs[this.round].Enabled = true;
            var candidates = Solver.Candidates.Select(w => (string)w).ToArray();
            this.inputs[this.round].Words = candidates;

            if (candidates.Length <= 1)
                this.inputs[this.round].SetAsLast();
        } // private void UpdateRound ()
    } // internal sealed class MainForm : Form
} // namespace Wordle.Controls
