using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace CodeArt.CI
{

    /*
     *  <Target Name="BeforeBuild"><SimpleTask /></Target>
        <UsingTask TaskName="SimpleTask" AssemblyFile = "$(OutDir)CodeArt.CI.dll"   /> 
     * <Target Name="AfterBuild"><SimpleTask /></Target>
       <UsingTask TaskName="SimpleTask" AssemblyFile = "$(OutDir)CodeArt.CI.dll"   /> 
     */


    public class SimpleTask : Task
    {
        public override bool Execute()
        {
            return true;
        }
    }
}