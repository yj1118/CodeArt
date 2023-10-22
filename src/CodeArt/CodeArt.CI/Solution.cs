using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.IO;

namespace CodeArt.CI
{
    public class Solution
    {
        private string _fileName;

        public Solution(string fileName)
        {
            _fileName = fileName;
        }


        private void ReferralRedirect()
        {
            var sln = new Referral.SLN(_fileName);
            sln.Redirect();
            sln.Save();

            var folder = IOUtil.GetFileDirectory(_fileName);
            var projs = IOUtil.GetFiles(folder, false, "csproj");

            foreach(var proj in projs)
            {
                File.SetAttributes(proj, FileAttributes.Normal);
                var csproj = new Referral.CSPROJ(proj);
                csproj.Redirect();
                csproj.Save();
            }
        }


        public void Process()
        {
            //引用重定向
            this.ReferralRedirect();
        }

    }
}
