
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

        private readonly ResultInput[] inputs;
        private int round = 0;

        internal MainForm()
        {
            this.Text = "WordleSolver";
            this.Size = this.MinimumSize = this.MaximumSize = new(430, 520);
            this.MaximizeBox = false;

            this.inputs = new ResultInput[CYCLE];
            for (var i = 0; i < CYCLE; i++)
            {
                this.inputs[i] = new()
                {
                    Text = (i + 1).ToString(),
                    Top = 70 * i + 20,
                    Left = 20,
                    Parent = this,
                };
                this.inputs[i].Submitted += MoveNext;
            }

            var reset = new Button()
            {
                Text = "Reset",
                Top = 440,
                Left = 260,
                Size = new(60, 30),
                Parent = this,
            };
            reset.Click += Reset;

            var close = new Button()
            {
                Text = "Close",
                Top = 440,
                Left = 340,
                Size = new(60, 30),
                Parent = this,
            };
            close.Click += (sender, e) => Close();

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

            this.inputs[this.round].Enabled = true;
            var candidates = Solver.Candidates.Select(w => (string)w).ToArray();
            this.inputs[this.round].Words = candidates;

            var indices = filter.CorrectIndices;
            for (var i = this.round; i < CYCLE; i++)
                this.inputs[i].Seal(indices);

            if (candidates.Length <= 1)
                this.inputs[this.round].SetAsLast();
        } // private void MoveNext (object, EventArgs)
    } // internal sealed class MainForm : Form
} // namespace Wordle.Controls
