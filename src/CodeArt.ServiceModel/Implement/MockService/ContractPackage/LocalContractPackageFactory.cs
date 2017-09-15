using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

using CodeArt.Concurrent;

namespace CodeArt.ServiceModel.Mock
{
    [SafeAccess]
    public class LocalContractPackageFactory : IContractPackageFactory
    {
        private static LocalContractPackage _package;

        static LocalContractPackageFactory()
        {
            _package = new LocalContractPackage();
            _package.Load(ServiceConfiguration.Current.LocalPath);
        }


        public IContractPackage Create()
        {
            return _package;
        }

        public static readonly LocalContractPackageFactory Instance = new LocalContractPackageFactory();

    }
}
