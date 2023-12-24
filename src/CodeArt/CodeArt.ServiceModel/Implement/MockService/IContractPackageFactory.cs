using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml;
using System.IO;

using CodeArt;
using System.Text;
using CodeArt.IO;
using CodeArt.DTO;

namespace CodeArt.ServiceModel.Mock
{
    public interface IContractPackageFactory
    {
        IContractPackage Create();
    }
}
