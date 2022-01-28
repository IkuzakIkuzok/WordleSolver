
// (c) 2021 Kazuki KOHZUKI

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Wordle.Controls
{
    internal sealed class ControlDrawingSuspender : IDisposable
    {
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(HandleRef hWnd, int msg, IntPtr wParam, IntPtr lParam);
        private const int WM_SETREDRAW = 0x000B;

        private bool _disposed;
        private readonly Control _control = null;

        internal ControlDrawingSuspender(Control control)
        {
            this._control = control;
            BeginControlUpdate(control);
        } // ctor (Control)

        public void Dispose()
            => Dispose(true);

        private void Dispose(bool disposing)
        {
            if (this._disposed) return;
            if (!disposing) return;

            EndControlUpdate(this._control);
            this._disposed = true;
        } // private void Dispose (bool disposing)

        internal static void BeginControlUpdate(Control control)
            => SendMessage(new HandleRef(control, control.Handle), WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);

        internal static void EndControlUpdate(Control control)
        {
            control.Invoke(new Action(() =>
            {
                SendMessage(new HandleRef(control, control.Handle), WM_SETREDRAW, new IntPtr(1), IntPtr.Zero);
                control.Refresh();
            }));
        } // internal static void EndControlUpdate (Control)
    } // internal sealed class ControlDrawingSuspender
} // namespace Wordle.Controls
