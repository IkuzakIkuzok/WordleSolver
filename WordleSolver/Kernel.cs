
// (c) 2021 Kazuki KOHZUKI

using System;
using System.Windows.Forms;
using Wordle.Controls;

namespace Wordle
{
    internal static class Kernel
    {
        [STAThread]
        private static void Main()
        {
            try
            {
                Application.Run(new MainForm());
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        } // private static void Main ()
    } // internal static class Kernel
} // namespace Wordle
