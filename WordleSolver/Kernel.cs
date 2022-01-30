
// (c) 2021 Kazuki KOHZUKI

using System;
using System.Diagnostics;
using System.Windows.Forms;
using Wordle.Controls;

namespace Wordle
{
    internal static class Kernel
    {
        internal static string GameUrl = @"https://www.powerlanguage.co.uk/wordle/";
        internal static string GitHubUrl = @"https://github.com/IkuzakIkuzok/WordleSolver";

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

        internal static void OpenBrowser(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo()
                {
                    UseShellExecute = true,
                    FileName = url,
                });
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        } // internal static void OpenBrowser (string)
    } // internal static class Kernel
} // namespace Wordle
