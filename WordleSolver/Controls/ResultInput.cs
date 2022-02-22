
// (c) 2021 Kazuki KOHZUKI

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Wordle.Controls
{
    [DesignerCategory("Code")]
    internal sealed class ResultInput : GroupBox
    {
        private static readonly XorShift128 xorShift = new(DateTime.UtcNow);

        private string[] words;

        private readonly ComboBox candidates;
        private readonly ResultBoxes results;
        private readonly Button submit;
        private readonly Label statistics;

        internal event EventHandler Submitted;

        internal bool IsSubmitted { get; private set; } = false;

        internal string[] Words
        {
            get => this.words;
            set
            {
                if (this.IsSubmitted) return;
                if (value.Length == 0) return;
                value = value.Select(w => w.ToUpper()).ToArray();
                if (this.words?.SequenceEqual(value) ?? false) return;
                this.words = value;
                this.candidates.Items.Clear();
                this.candidates.Items.AddRange(this.words);
                this.candidates.SelectedIndex = 0;
            }
        }

        internal string SelectedWord
            => (string)this.candidates.SelectedItem;

        internal Filter Filter
            => new(this.SelectedWord, this.results.Colors);

        internal ResultInput()
        {
            this.Size = new(380, 80);

            this.candidates = new()
            {
                Top = 20,
                Left = 20,
                Width = 80,
                DropDownStyle = ComboBoxStyle.DropDownList,
                TabStop = false,
                Parent = this,
            };
            this.candidates.SelectedIndexChanged += (sender, e)
                    => this.submit.Enabled = (this.candidates.SelectedItem?.ToString().Length ?? -1) == Word.LENGTH;

            this.results = new()
            {
                Top = 20,
                Left = 120,
                Parent = this,
            };

            this.submit = new()
            {
                Text = "OK",
                Top = 18,
                Left = 330,
                Size = new(40, 25),
                Parent = this,
            };
            this.submit.Click += Submit;

            this.statistics = new()
            {
                Top = 52,
                Left = 20,
                Width = 300,
                Parent = this,
            };
        } // ctor ()

        internal void Seal(IEnumerable<int> indices)
            => this.results.Seal(indices);

        override protected void OnEnabledChanged(EventArgs e)
        {
            this.candidates.Enabled = this.results.Enabled = this.Enabled;
            this.submit.Enabled &= this.Enabled;
            base.OnEnabledChanged(e);
        } // override protected void OnEnabledChanged (EventArgs)

        internal void SetAsFirst()
        {
            this.Words = new[] { "tares" };
            this.Enabled = true;
            this.candidates.SelectedIndex = xorShift.NextInt32(0, this.words.Length);
            this.submit.Enabled = true;
        } // internal void SetAsFirst ()

        internal void SetAsLast()
            => this.submit.Enabled = false;

        internal void SetStatisticsInfo(int before, int after, double expected)
        {
            var info = Math.Log2((double)before / after);
            this.statistics.Text = $"{before} --> {after} (expected {expected:f4} bits, got {info:f4} bits)";
        } // internal void SetStatisticsInfo (int, int)

        internal void Reset()
        {
            this.candidates.Items.Clear();
            this.words = null;
            this.candidates.SelectedIndex = -1;
            this.statistics.Text = string.Empty;
            this.results.Reset();
            this.Enabled = false;
            this.IsSubmitted = false;
        } // internal void Reset ()

        private void Submit(object sender, EventArgs e)
        {
            this.Enabled = false;
            this.IsSubmitted = true;
            Submitted?.Invoke(this, e);
        } // private void Submit (object, EventArgs)

        internal void SetResult(ResultColors colors)
        {
            this.results.Colors = colors;
            this.submit.PerformClick();
        } // internal void SetResult (ResultColors)
    } // internal sealed class ResultInput : GroupBox
} // namespace Wordle.Controls
