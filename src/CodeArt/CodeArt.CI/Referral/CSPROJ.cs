using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

using CodeArt.Util;
using CodeArt.Concurrent;

namespace CodeArt.CI.Referral
{
    public class CSPROJ : Redirector
    {
        private XmlDocument _document = null;

        public CSPROJ(string fileName)
            : base(fileName)
        {
            this.Init(fileName);
        }

        private void Init(string fileName)
        {
            _document = new XmlDocument();
            _document.LoadXml(File.ReadAllText(fileName));
        }

        public override void Redirect()
        {
            XmlNamespaceManager ns = new XmlNamespaceManager(_document.NameTable);
            ns.AddNamespace("ns", _document.DocumentElement.NamespaceURI);

            var projectNodes = _document.SelectNodes("ns:Project/ns:ItemGroup/ns:ProjectReference", ns);
            foreach (XmlNode projectNode in projectNodes)
            {
                var name = projectNode.SelectSingleNode("ns:Name", ns).InnerText;
                var project = Configuration.Current.Workspace.GetProject(name);
                if (project != null)
                {
                    var relativePath = GetRelativePath(project.Path);
                    projectNode.SetAttributeValue("Include", relativePath);
                }
            }
        }

        protected override string GetCode()
        {
            return _document.OuterXml;
        }

    }
}


//<? xml version="1.0" encoding="utf-8"?>
//<Project ToolsVersion = "15.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
//  <Import Project = "$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props" Condition="Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')" />
//  <PropertyGroup>
//    <Configuration Condition = " '$(Configuration)' == '' " > Debug </ Configuration >
//    < Platform Condition=" '$(Platform)' == '' ">AnyCPU</Platform>
//    <ProjectGuid>{A02F6B05-AFEC-4421-829B-050AC244B4DC}</ProjectGuid>
//    <OutputType>Library</OutputType>
//    <AppDesignerFolder>Properties</AppDesignerFolder>
//    <RootNamespace>PortalService.Util</RootNamespace>
//    <AssemblyName>PortalService.Util</AssemblyName>
//    <TargetFrameworkVersion>v4.7</TargetFrameworkVersion>
//    <FileAlignment>512</FileAlignment>
//    <Deterministic>true</Deterministic>
//    <SccProjectName>SAK</SccProjectName>
//    <SccLocalPath>SAK</SccLocalPath>
//    <SccAuxPath>SAK</SccAuxPath>
//    <SccProvider>SAK</SccProvider>
//  </PropertyGroup>
//  <PropertyGroup Condition = " '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' " >
//    < DebugSymbols > true </ DebugSymbols >
//    < DebugType > full </ DebugType >
//    < Optimize > false </ Optimize >
//    < OutputPath > bin\Debug\</OutputPath>
//    <DefineConstants>DEBUG;TRACE</DefineConstants>
//    <ErrorReport>prompt</ErrorReport>
//    <WarningLevel>4</WarningLevel>
//  </PropertyGroup>
//  <PropertyGroup Condition = " '$(Configuration)|$(Platform)' == 'Release|AnyCPU' " >
//    < DebugType > pdbonly </ DebugType >
//    < Optimize > true </ Optimize >
//    < OutputPath > bin\Release\</OutputPath>
//    <DefineConstants>TRACE</DefineConstants>
//    <ErrorReport>prompt</ErrorReport>
//    <WarningLevel>4</WarningLevel>
//  </PropertyGroup>
//  <ItemGroup>
//    <Reference Include = "AopSdk, Version=2.0.0.0, Culture=neutral, processorArchitecture=MSIL" >
//      < SpecificVersion > False </ SpecificVersion >
//      < HintPath > Pay\Lib\AopSdk.dll</HintPath>
//    </Reference>
//    <Reference Include = "Newtonsoft.Json, Version=6.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed, processorArchitecture=MSIL" />
//    < Reference Include="System" />
//    <Reference Include = "System.Core" />
//    < Reference Include="System.Web" />
//    <Reference Include = "System.Xml.Linq" />
//    < Reference Include="System.Data.DataSetExtensions" />
//    <Reference Include = "Microsoft.CSharp" />
//    < Reference Include="System.Data" />
//    <Reference Include = "System.Net.Http" />
//    < Reference Include="System.Xml" />
//  </ItemGroup>
//  <ItemGroup>
//    <Compile Include = "File\FileQuote.cs" />
//    < Compile Include="Pay\PayCommon.cs" />
//    <Compile Include = "Properties\AssemblyInfo.cs" />
//    < Compile Include="Tag\TagUtil.cs" />
//    <Compile Include = "UserPortal.cs" />
//  </ ItemGroup >
//  < ItemGroup >
//    < ProjectReference Include="..\..\..\CodeArt Framework\CodeArt\CodeArt.DomainDriven.Extensions\CodeArt.DomainDriven.Extensions.csproj">
//      <Project>{a45a1e35-6ce5-42c2-8fd3-19ce5b0bd5d6}</Project>
//      <Name>CodeArt.DomainDriven.Extensions</Name>
//    </ProjectReference>
//    <ProjectReference Include = "..\..\..\CodeArt Framework\CodeArt\CodeArt.DomainDriven\CodeArt.DomainDriven.csproj" >
//      < Project >{f62d7792-f4ee-4924-898e-c56309e6dfc2}</Project>
//      <Name>CodeArt.DomainDriven</Name>
//    </ProjectReference>
//    <ProjectReference Include = "..\..\..\CodeArt Framework\CodeArt\CodeArt.ServiceModel\CodeArt.ServiceModel.csproj" >
//      < Project >{ea6365f1-c027-416b-b4cc-a94572b5fc19}</Project>
//      <Name>CodeArt.ServiceModel</Name>
//    </ProjectReference>
//    <ProjectReference Include = "..\..\..\CodeArt Framework\CodeArt\CodeArt\CodeArt.csproj" >
//      < Project >{d2c9f430-6b47-482a-a49c-a2478d75f95f}</Project>
//      <Name>CodeArt</Name>
//    </ProjectReference>
//  </ItemGroup>
//  <ItemGroup>
//    <Content Include = "Pay\Lib\AopSdk.dll" />
//  </ ItemGroup >
//  < ItemGroup />
//  < Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets" />
//</Project>