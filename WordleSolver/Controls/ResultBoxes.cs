
// (c) 2021 Kazuki KOHZUKI

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace Wordle.Controls
{
    [DesignerCategory("Code")]
    internal sealed class ResultBoxes : Label
    {
        private readonly ResultBox[] boxes;

        internal ResultColor[] Colors
            => this.boxes.Select(box => box.SelectedResult).ToArray();

        internal ResultBoxes()
        {
            this.Size = new(200, 20);

            this.boxes = new ResultBox[Word.LENGTH];
            for (var i = 0; i < Word.LENGTH; i++)
            {
                this.boxes[i] = new()
                {
                    Top = 0,
                    Left = 40 * i,
                    Parent = this,
                };
            }
        } // ctor ()

        override protected void OnEnabledChanged(EventArgs e)
        {
            foreach (var box in this.boxes)
                box.Enabled = this.Enabled;
            if (this.Enabled)
                this.boxes[0].Select();
            base.OnEnabledChanged(e);
        } // override protected void OnEnabledChanged (EventArgs)

        internal void Seal(IEnumerable<int> indices)
        {
            foreach (var index in indices)
                this.boxes[index].Seal();
        } // internal void Seal(IEnumerable<int>)

        internal void Reset()
        {
            foreach (var box in this.boxes)
                box.Reset();
        } // internal void Reset ()
    } // internal sealed class ResultBoxes : Label
} // namespace Wordle.Controls
