﻿
// (c) 2021 Kazuki KOHZUKI

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Wordle.Controls
{
    [DesignerCategory("Code")]
    internal class CustomTextBox : TextBox
    {
        private Timer delayedTextChangedTimer;

        /// <summary>
        /// Gets or set the delay time for the <see cref="DelayedTextChanged"/> event to occur.
        /// </summary>
        internal int DelayedTextChangedTimeout { get; set; }

        /// <summary>
        /// Occurs when the specified time passed after <see cref="Text"/> property value changes.
        /// </summary>
        internal event EventHandler DelayedTextChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomTextBox"/> class.
        /// </summary>
        internal CustomTextBox() : base()
        {
            this.DelayedTextChangedTimeout = 1_000;
        } // ctor ()

        protected virtual void OnDelayedTextChanged(EventArgs e)
            => DelayedTextChanged?.Invoke(this, e);

        override protected void OnTextChanged(EventArgs e)
        {
            InitializeDelayedTextChangedEvent();
            base.OnTextChanged(e);
        } // override protected void OnTextChanged (EventArgs)

        private void InitializeDelayedTextChangedEvent()
        {
            this.delayedTextChangedTimer?.Stop();

            if (this.delayedTextChangedTimer == null || this.delayedTextChangedTimer.Interval != this.DelayedTextChangedTimeout)
            {
                this.delayedTextChangedTimer = new Timer()
                {
                    Interval = this.DelayedTextChangedTimeout,
                };
                this.delayedTextChangedTimer.Tick += HandleDelayedTextChangedTimerTick;
            }

            this.delayedTextChangedTimer.Start();
        } // private void InitializeDelayedTextChangedEvent ()

        private void HandleDelayedTextChangedTimerTick(object sender, EventArgs e)
        {
            if (sender is Timer timer)
                timer.Stop();

            OnDelayedTextChanged(EventArgs.Empty);
        } // private void HandleDelayedTextChangedTimerTick (object, EventArgs)

        override protected void Dispose(bool disposing)
        {
            if (this.delayedTextChangedTimer != null)
            {
                this.delayedTextChangedTimer.Stop();
                if (disposing)
                    this.delayedTextChangedTimer.Dispose();
            }

            base.Dispose(disposing);
        } // override protected void Dispose (bool)
    } // internal class CustomTextBox : TextBox
} // namespace Wordle.Controls
