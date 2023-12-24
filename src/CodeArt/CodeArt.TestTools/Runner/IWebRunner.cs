using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.TestTools
{
    public interface IWebRunner
    {
        void AddCookie();

        void GetCookie(string name);

        void DeleteCookie(string name);

        void ClearCookie();
    }
}
