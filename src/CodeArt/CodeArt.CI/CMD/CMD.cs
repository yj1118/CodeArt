using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.CI
{
    public static class CMD
    {
        public static (string Log, int Code) Execute(string cmd)
        {
            string log = null;
            int code = 0;
            using (Process process = new Process())
            {
                ProcessStartInfo startInfo = new ProcessStartInfo("cmd.exe");
                startInfo.UseShellExecute = false;
                process.StartInfo = startInfo;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();

                process.StandardInput.WriteLine(cmd);
                process.StandardInput.WriteLine("exit");

                log = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                code = process.ExitCode;
                process.Close();
            }

            return (log, code);
        }


    }
}
