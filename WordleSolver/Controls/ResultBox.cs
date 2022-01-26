
// (c) 2021 Kazuki KOHZUKI

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Wordle.Controls
{
    [DesignerCategory("Code")]
    internal sealed class ResultBox : ComboBox
    {
        internal ResultColor SelectedResult
        {
            get => ((ResultItem)this.SelectedItem).Result;
            set => this.SelectedIndex = (int)value;
        }

        internal ResultBox()
        {
            this.Width = 40;
            this.DropDownStyle = ComboBoxStyle.DropDownList;

            this.Items.AddRange(new object[] {
                new ResultItem("×", ResultColor.Wrong, Color.LightGray),
                new ResultItem("△", ResultColor.Included, Color.Gold),
                new ResultItem("○", ResultColor.Correct, Color.LimeGreen),
            });
            this.SelectedIndex = 0;
        } // ctor ()

        override protected void OnSelectedIndexChanged(EventArgs e)
        {
            var item = (ResultItem)this.SelectedItem;
            this.BackColor = item.Color;
            Select(0, 0);

            base.OnSelectedIndexChanged(e);
        } // override protected void OnSelectedIndexChanged (EventArgs)

        override protected void OnDropDownClosed(EventArgs e)
        {
            base.OnDropDownClosed(e);
            BeginInvoke(new Action(() => Select(0, 0)));
        } // override protected void OnDropDownClosed (EventArgs)

        internal void Seal()
        {
            this.Enabled = false;
            this.SelectedResult = ResultColor.Correct;
        } // internal void Seal ()

        internal void Reset()
        {
            this.SelectedResult = ResultColor.Wrong;
            this.Enabled = false;
        } // internal void Reset ()

        private sealed class ResultItem
        {
            internal string Text { get; }
            internal ResultColor Result { get; }
            internal Color Color { get; }

            internal ResultItem(string text, ResultColor result, Color color)
            {
                this.Text = text;
                this.Result = result;
                this.Color = color;
            } // ctor (string, ResultColor, Color)

            override public string ToString()
                => this.Text;
        } // private sealed class ResultItem
    } // internal sealed class ResultBox : ComboBox
} // namespace Wordle.Controls
