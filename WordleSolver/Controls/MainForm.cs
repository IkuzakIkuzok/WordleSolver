﻿
// (c) 2021 Kazuki KOHZUKI

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Wordle.Controls
{
    [DesignerCategory("Code")]
    internal sealed class MainForm : Form
    {
        private const int CYCLE = 6;

        private readonly GroupBox gb_mode;
        private readonly ResultInput[] inputs;
        private int round = 0;

        internal MainForm()
        {
            this.Text = "WordleSolver";
            this.Size = this.MinimumSize = this.MaximumSize = new(430, 680);
            this.MaximizeBox = false;

            this.gb_mode = new()
            {
                Text = "Solver mode",
                Top = 40,
                Left = 20,
                Size = new(300, 115),
                Parent = this,
            };
            foreach ((var i, var mode) in ((IEnumerable<SolverMode>)Enum.GetValues(typeof(SolverMode))).Enumerate())
            {
                var rb = new RadioButton()
                {
                    Text = mode.GetDescription(),
                    Width = 250,
                    Top = 20 + 30 * i,
                    Left = 15,
                    Checked = i == 0,
                    Tag = mode,
                    Parent = this.gb_mode,
                };
                rb.CheckedChanged += ChangeSolverMode;
            }

            this.inputs = new ResultInput[CYCLE];
            for (var i = 0; i < CYCLE; i++)
            {
                this.inputs[i] = new()
                {
                    Text = (i + 1).ToString(),
                    Top = 70 * i + 170,
                    Left = 20,
                    Parent = this,
                };
                this.inputs[i].Submitted += MoveNext;
            }

            var reset = new Button()
            {
                Text = "Reset",
                Top = 590,
                Left = 270,
                Size = new(60, 30),
                Parent = this,
            };
            reset.Click += Reset;

            var close = new Button()
            {
                Text = "Close",
                Top = 590,
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

            var simulate = new ToolStripMenuItem()
            {
                Text = "&Simulate",
                ShortcutKeys = Keys.Control | Keys.R,
            };
            simulate.Click += Simulate;
            tool.DropDownItems.Add(simulate);

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

        private void ChangeSolverMode(object sender, EventArgs e)
        {
            Solver.SolverMode = (SolverMode)this.gb_mode.Controls
                                                        .OfType<RadioButton>()
                                                        .FirstOrDefault(rb => rb.Checked)
                                                        .Tag;
            if (this.round > 0)
                UpdateRound();
        } // private void ChangeSolverMode (object, EventArgs)

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

            if (!Solver.HardMode) return;
            var indices = filter.CorrectIndices;
            for (var i = this.round; i < CYCLE; i++)
                this.inputs[i].Seal(indices);
        } // private void MoveNext (object, EventArgs)

        private void UpdateRound()
        {
            if (this.round >= CYCLE) return;
            this.inputs[this.round].Enabled = true;
            var candidates = Solver.Candidates.Select(w => (string)w).ToArray();
            this.inputs[this.round].Words = candidates;

            if (candidates.Length <= 1)
                this.inputs[this.round].SetAsLast();
        } // private void UpdateRound ()

        private void Simulate(object sender, EventArgs e)
        {
            if (this.round > 0)
            {
                var dr = MessageBox.Show(
                    "The current prediction will be reset." +
                    "Are you sure you want to continue?",
                    "Simulation mode",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Exclamation
                );
                if (dr != DialogResult.OK) return;
            }

            var form = new SimulateWordForm();
            form.ShowDialog();
            var w = form.SimulateWord;
            if (w == null) return;
            var word = (Word)w;

            using var _ = new ControlDrawingSuspender(this);
            Reset();

            ResultInput input = null;
            ResultColors res;
            while (Solver.CandidatesCount > 1)
            {
                input = this.inputs[this.round];
                res = word.GetResults(input.SelectedWord.ToLower());
                input.SetResult(res);

                if (this.round >= CYCLE) return;
                if (res.GetHashCode() == ResultColors.Perfect) return;
            }

            input = this.inputs[this.round];
            res = word.GetResults(input.SelectedWord.ToLower());
            input.SetResult(res);
        } // private void Simulate (object, EventArgs)
    } // internal sealed class MainForm : Form
} // namespace Wordle.Controls
