using System;
using System.Text;
using System.IO;
using System.CodeDom.Compiler;
using System.Reflection;

using Microsoft.CSharp;

using CodeArt.IO;

namespace CodeArt.Runtime
{
    /// <summary>
    /// 代码编译器
    /// 如果要使用调试功能，必须满足3个条件：
    /// 1.CompilerParameters.IncludeDebugInformation=ture(生成pdb文件)
    /// 2.以源文件的形式编译(CompileFromFile)，而不是源代码
    /// 3.以Assembly.LoadFrom(assemblyFileName)的方法加载程序集
    /// </summary>
    public class CodeCompiler : MarshalByRefObject
    {
        private CodeDomProvider _provider;
        private CompilerParameters _params;

        /// <summary>
        /// 添加引用对象
        /// </summary>
        /// <param name="assemblyName">引用的文件名</param>
        public void AddReference(string assemblyName)
        {
            _params.ReferencedAssemblies.Add(assemblyName);
        }

        public CodeCompiler(CodeDomProvider provider, bool debug)
        {
            SetSetupInfo(provider, debug);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="debugMode">是否为调试模式</param>
        /// <param name="assemblyFileName">生成的程序集文件全名</param>
        private void SetSetupInfo(CodeDomProvider provider,bool debugMode)
        {
            _params = new CompilerParameters();
            _params.GenerateExecutable = false;
            _params.GenerateInMemory = false;

            if (debugMode)
            {
                _params.IncludeDebugInformation = true; //生成pdb，可以调试
                this._params.CompilerOptions += "/define:optimize /define:DEBUG=1 ";
            }
            else
            {
                _params.IncludeDebugInformation = false;
                _params.CompilerOptions += "/optimize";
            }
            _provider = provider;
        }

        /// <summary>
        /// 编译源代码
        /// </summary>
        /// <param name="outputAssembly"></param>
        /// <param name="sourceCodes"></param>
        internal void CompileFromSource(string outputAssembly,params string[] sourceCodes)
        {
            Compile(outputAssembly, () =>
            {
                return _provider.CompileAssemblyFromSource(_params, sourceCodes);
            });
        }

        /// <summary>
        /// /编译文件
        /// </summary>
        /// <param name="outputAssembly"></param>
        /// <param name="sourceFileNames"></param>
        internal void CompileFromFile(string outputAssembly,params string[] sourceFileNames)
        {
            Compile(outputAssembly, () =>
             {
                 return _provider.CompileAssemblyFromFile(_params, sourceFileNames);
             });
        }

        /// <summary>
        /// 编译
        /// </summary>
        /// <param name="outputAssembly"></param>
        private void Compile(string outputAssembly,Func<CompilerResults> compileAssembly)
        {
            _params.OutputAssembly = outputAssembly;

            //检查目录是否存在
            DirectoryInfo dir = Directory.GetParent(outputAssembly);
            if (!dir.Exists)
                Directory.CreateDirectory(dir.FullName);

            CompilerResults result = compileAssembly();
            if (result.Errors.Count > 0)
            {
                CompilerErrorCollection errors = result.Errors;
                StringBuilder message = new StringBuilder();
                message.AppendLine("编译出错");
                foreach (CompilerError item in errors)
                    message.AppendLine(item.ErrorText);

                throw new CompileException(message.ToString());
            }
        }

        public static void CompileCSharp(string outputAssembly, params string[] sourceFileNames)
        {
            //动态创建和卸载appDomain来编译代码，防止锁定生成的文件
            AppDomain domain = null;
            try
            {
                AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
                domain = AppDomain.CreateDomain("Domain #Compile", null, setup);
                var compiler = CodeCompiler.Create<CSharpCodeCompiler>(domain);

                AssemblyUtil.Each((assembly) =>
                {
                    compiler.AddReference(assembly.Location);
                });

                compiler.CompileFromFile(outputAssembly, sourceFileNames);

                IOUtil.SetEveryoneFullControl(outputAssembly);//设置文件权限为所有人可以访问
                IOUtil.SetEveryoneFullControl(GetPDB(outputAssembly));//设置文件权限为所有人可以访问
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                AppDomain.Unload(domain);
            }
        }

        //public static void CompileCSharp(string outputAssembly,string[] referencedAssemblies, params string[] sourceFileNames)
        //{
        //    //动态创建和卸载appDomain来编译代码，防止锁定生成的文件
        //    AppDomain domain = null;
        //    try
        //    {
        //        AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
        //        domain = AppDomain.CreateDomain("Domain #Compile", null, setup);
        //        var compiler = CodeCompiler.Create<CSharpCodeCompiler>(domain);

        //        //添加bin下的引用
        //        string binFolder = AppDomain.CurrentDomain.RelativeSearchPath;
        //        var refs = Directory.GetFiles(binFolder, "*.dll");
        //        foreach (string item in refs) compiler.AddReference(item);

        //        foreach (string item in referencedAssemblies) compiler.AddReference(item);

        //        compiler.CompileFromFile(outputAssembly, sourceFileNames);

        //        IOUtil.SetEveryoneFullControl(outputAssembly);//设置文件权限为所有人可以访问
        //        IOUtil.SetEveryoneFullControl(GetPDB(outputAssembly));//设置文件权限为所有人可以访问
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        AppDomain.Unload(domain);
        //    }
        //}

        private static string GetPDB(string assemblyFileName)
        {
            var temp = assemblyFileName.Remove(assemblyFileName.Length - 3, 3);
            return string.Format("{0}pdb", temp);
        }




        private static CodeCompiler Create<T>(AppDomain domain) where T : CodeCompiler
        {
#if(DEBUG)
            bool isDebugMode = true;
#endif
#if(!DEBUG)
            bool isDebugMode = false;
#endif
            var compilerType = typeof(T);
            return domain.CreateInstanceAndUnwrap(Assembly.GetAssembly(compilerType).FullName
                                                        , compilerType.FullName
                                                        , true, 0, null
                                                        , new object[] { isDebugMode }, null, null)
                                                        as CodeCompiler;
        }

    }

    public class CSharpCodeCompiler : CodeCompiler
    {
        public CSharpCodeCompiler(bool debug)
            : base(new CSharpCodeProvider(), debug)
        {
        }
    }
}
