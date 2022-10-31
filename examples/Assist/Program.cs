using System;
using System.Windows.Forms;
using Wit.Input;
using static System.Windows.Forms.MessageBoxButtons;
using static System.Windows.Forms.MessageBoxIcon;

namespace Assist
{
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            const string nsp = nameof(Celebrities);
            if (Config.Load().TryGetValue(nsp, out var ct))
            {
                args = new[] { ct };
            }

            if (args == null || args.Length == 0)
            {
                var txt = $"Usage: {nsp} <wit-access-token>";
                MessageBox.Show(txt, nameof(Error), OK, Error);
                Environment.Exit(1);
                return;
            }

            ApplicationConfiguration.Initialize();
            var form = new MainForm { AccessToken = args[0] };
            Application.Run(form);
        }
    }
}