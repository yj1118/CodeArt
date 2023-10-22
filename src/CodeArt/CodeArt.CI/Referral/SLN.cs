using CodeArt.Concurrent;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CodeArt.CI.Referral
{
    public class SLN  : Redirector
    {
        private List<Line> _lines;

        public SLN(string fileName)
            :base(fileName)
        {
            this.Init(fileName);
        }

        private void Init(string fileName)
        {
            _lines = new List<Line>();
            using (StringReader reader = new StringReader(File.ReadAllText(fileName)))
            {
                while (true)
                {
                    var code = reader.ReadLine();
                    if (code == null) break;

                    var project = TryProject(code);
                    if (project != null)
                    {
                        _lines.Add(project);
                    }
                    else
                    {
                        _lines.Add(new Line(code));
                    }
                }
            }
        }

        private Regex _projectRegex = new Regex("Project\\(\"(.*)\"\\)[ ]*=[ ]*\"(.*?)\"[ ]*,[ ]*\"(.*?)\"[ ]*,[ ]*\"(.*?)\"");

        private ProjectLine TryProject(string code)
        {
            var match = _projectRegex.Match(code);
            if (!match.Success) return null;
            var startId = match.Groups[1].Value;
            var name = match.Groups[2].Value;
            var path = match.Groups[3].Value;
            var endId = match.Groups[4].Value;
            return new ProjectLine(code, startId, name, path, endId);
        }

        public override void Redirect()
        {
            foreach (var line in _lines)
            {
                var projectLine = line as ProjectLine;
                if (projectLine != null)
                {
                    var project = Configuration.Current.Workspace.GetProject(projectLine.Name);
                    if (project != null)
                    {
                        projectLine.RelativePath = GetRelativePath(project.Path);
                    }
                }
            }
        }

        protected override string GetCode()
        {
            string content = null;
            using (var temp = StringPool.Borrow())
            {
                var code = temp.Item;
                foreach (var line in _lines)
                {
                    code.AppendLine(line.Code);
                }
                content = code.ToString();
            }
            return content;
        }

        private class Line
        {
            public string RawCode
            {
                get;
                private set;
            }

            public virtual string Code
            {
                get
                {
                    return this.RawCode;
                }
            }

            public Line(string rawCode)
            {
                this.RawCode = rawCode;
            }
        }


        private class ProjectLine : Line
        {
            public string StartId
            {
                get;
                private set;
            }

            public string Name
            {
                get;
                private set;
            }

            public string RelativePath
            {
                get;
                set;
            }

            public string EndId
            {
                get;
                private set;
            }

            public override string Code
            {
                get
                {
                    return string.Format("Project(\"{0}\") = \"{1}\", \"{2}\", \"{3}\"", this.StartId, this.Name, this.RelativePath, this.EndId);
                }
            }


            public ProjectLine(string rawCode, string startId, string name, string relativePath, string endId)
                : base(rawCode)
            {
                this.StartId = startId;
                this.Name = name;
                this.RelativePath = relativePath;
                this.EndId = endId;
            }
        }

    }
}



//Microsoft Visual Studio Solution File, Format Version 12.00
//# Visual Studio 15
//VisualStudioVersion = 15.0.27130.2036
//MinimumVisualStudioVersion = 10.0.40219.1
//Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "Framework", "Framework", "{99ECAE38-204D-4947-B45A-6B4F8C963C2E}"
//EndProject
//Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "CodeArt", "..\..\CodeArt Framework\CodeArt\CodeArt\CodeArt.csproj", "{D2C9F430-6B47-482A-A49C-A2478D75F95F}"
//EndProject
//Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "CodeArt.DomainDriven", "..\..\CodeArt Framework\CodeArt\CodeArt.DomainDriven\CodeArt.DomainDriven.csproj", "{F62D7792-F4EE-4924-898E-C56309E6DFC2}"
//EndProject
//Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "CodeArt.DomainDriven.DataAccess", "..\..\CodeArt Framework\CodeArt\CodeArt.DomainDriven.DataAccess\CodeArt.DomainDriven.DataAccess.csproj", "{C0BAC0EB-7068-42AF-ACAC-09DF677CF980}"
//EndProject
//Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "CodeArt.DomainDriven.Extensions", "..\..\CodeArt Framework\CodeArt\CodeArt.DomainDriven.Extensions\CodeArt.DomainDriven.Extensions.csproj", "{A45A1E35-6CE5-42C2-8FD3-19CE5B0BD5D6}"
//EndProject
//Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "CodeArt.ServiceModel", "..\..\CodeArt Framework\CodeArt\CodeArt.ServiceModel\CodeArt.ServiceModel.csproj", "{EA6365F1-C027-416B-B4CC-A94572B5FC19}"
//EndProject
//Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "Subsystems", "Subsystems", "{5F589A1E-007A-4A15-BE6D-EEB69A7DBCFC}"
//EndProject
//Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "AccountSubsystem", "..\..\Subsystems Framework\AccountSubsystem\AccountSubsystem\AccountSubsystem.csproj", "{F9F4EF44-D894-4B0C-8A6F-AE422D98576C}"
//EndProject
//Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "PortalService.Application", "PortalService.Application\PortalService.Application.csproj", "{10F7C080-CF92-4AF1-B12A-32B55FBDFA34}"
//EndProject
//Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "CodeArt.RabbitMQ", "..\..\CodeArt Framework\CodeArt\CodeArt.RabbitMQ\CodeArt.RabbitMQ.csproj", "{89C4BA5C-358B-4095-8FD2-0BAE32C2178E}"
//EndProject
//Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "CodeArt.EasyMQ", "..\..\CodeArt Framework\CodeArt\CodeArt.EasyMQ\CodeArt.EasyMQ.csproj", "{AFB15952-0DFA-4919-9D4B-46C2737654EC}"
//EndProject
//Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "PortalService", "PortalService\PortalService.csproj", "{1E5CEFF5-C803-4632-8E65-85CF3E5CE672}"
//EndProject
//Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "CodeArt.ServiceModel.MQ", "..\..\CodeArt Framework\CodeArt\CodeArt.ServiceModel.MQ\CodeArt.ServiceModel.MQ.csproj", "{CEBC603E-59AD-4BD7-839D-CCFE7C0841B1}"
//EndProject
//Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "LocationSubsystem", "..\..\Subsystems Framework\LocationSubsystem\LocationSubsystem\LocationSubsystem.csproj", "{D555A345-1D09-42F6-B090-15A08E763B1D}"
//EndProject
//Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "FileSubsystem", "..\..\Subsystems Framework\FileSubsystem\FileSubsystem\FileSubsystem.csproj", "{BCE782F2-71EA-43C0-A361-FE79666CCC06}"
//EndProject
//Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "UserSubsystem", "..\..\Subsystems Framework\UserSubsystem\UserSubsystem\UserSubsystem.csproj", "{36C405AE-737F-4EC4-8568-84CBD0AE9B9A}"
//EndProject
//Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "CodeArt.Web", "..\..\CodeArt Framework\CodeArt\CodeArt.Web\CodeArt.Web.csproj", "{FC477F23-2BCC-4917-841E-691726EB0894}"
//EndProject
//Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "UtilSubsystem", "..\..\Subsystems Framework\UtilSubsystem\UtilSubsystem\UtilSubsystem.csproj", "{BE5EE943-1E0F-4E41-AF76-9A90DB103CC8}"
//EndProject
//Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "MessageSubsystem", "..\..\Subsystems Framework\MessageSubsystem\MessageSubsystem\MessageSubsystem.csproj", "{1FACC9A8-230C-45D6-A195-22F5B4D6A0D2}"
//EndProject
//Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "CodeArt.TestPlatform", "..\..\CodeArt Framework\CodeArt\CodeArt.TestPlatform\CodeArt.TestPlatform.csproj", "{556218EB-5B3E-437F-8533-C9B22335AEC5}"
//EndProject
//Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "PortalService.Util", "PortalService.Util\PortalService.Util.csproj", "{A02F6B05-AFEC-4421-829B-050AC244B4DC}"
//EndProject
//Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "MessageSubsystem.Util", "..\..\Subsystems Framework\MessageSubsystem\MessageSubsystem.Util\MessageSubsystem.Util.csproj", "{97AA2ECD-0B72-417A-A1D3-60B154F1653B}"
//EndProject
//Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "CodeArt.MessagePush", "..\..\CodeArt Framework\CodeArt\CodeArt.MessagePush\CodeArt.MessagePush.csproj", "{85E30F16-2131-40AC-BB1C-2D5873E1D9A5}"
//EndProject
//Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "LogService.Util", "..\LogService\LogService.Util\LogService.Util.csproj", "{465923F8-4AD3-4507-AB2D-64F1B778225C}"
//EndProject
//Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "QualityService.Util", "..\QualityService\QualityService.Util\QualityService.Util.csproj", "{2CF4F063-181D-48F7-A2F4-FB73E77183E9}"
//EndProject
//Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "TagSubsystem", "..\..\Subsystems Framework\TagSubsystem\TagSubsystem\TagSubsystem.csproj", "{74795D16-CBD3-46B5-91F9-EEAF682F681D}"
//EndProject
//Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "CodeArt.TestTools", "..\..\CodeArt Framework\CodeArt\CodeArt.TestTools\CodeArt.TestTools.csproj", "{A55DD9DB-5254-4D31-888D-05474318035D}"
//EndProject
//Global
//	GlobalSection(SolutionConfigurationPlatforms) = preSolution
//		Debug|Any CPU = Debug|Any CPU
//		Release|Any CPU = Release|Any CPU
//	EndGlobalSection
//	GlobalSection(ProjectConfigurationPlatforms) = postSolution
//		{D2C9F430-6B47-482A-A49C-A2478D75F95F}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
//		{D2C9F430-6B47-482A-A49C-A2478D75F95F}.Debug|Any CPU.Build.0 = Debug|Any CPU
//		{D2C9F430-6B47-482A-A49C-A2478D75F95F}.Release|Any CPU.ActiveCfg = Release|Any CPU
//		{D2C9F430-6B47-482A-A49C-A2478D75F95F}.Release|Any CPU.Build.0 = Release|Any CPU
//		{F62D7792-F4EE-4924-898E-C56309E6DFC2}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
//		{F62D7792-F4EE-4924-898E-C56309E6DFC2}.Debug|Any CPU.Build.0 = Debug|Any CPU
//		{F62D7792-F4EE-4924-898E-C56309E6DFC2}.Release|Any CPU.ActiveCfg = Release|Any CPU
//		{F62D7792-F4EE-4924-898E-C56309E6DFC2}.Release|Any CPU.Build.0 = Release|Any CPU
//		{C0BAC0EB-7068-42AF-ACAC-09DF677CF980}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
//		{C0BAC0EB-7068-42AF-ACAC-09DF677CF980}.Debug|Any CPU.Build.0 = Debug|Any CPU
//		{C0BAC0EB-7068-42AF-ACAC-09DF677CF980}.Release|Any CPU.ActiveCfg = Release|Any CPU
//		{C0BAC0EB-7068-42AF-ACAC-09DF677CF980}.Release|Any CPU.Build.0 = Release|Any CPU
//		{A45A1E35-6CE5-42C2-8FD3-19CE5B0BD5D6}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
//		{A45A1E35-6CE5-42C2-8FD3-19CE5B0BD5D6}.Debug|Any CPU.Build.0 = Debug|Any CPU
//		{A45A1E35-6CE5-42C2-8FD3-19CE5B0BD5D6}.Release|Any CPU.ActiveCfg = Release|Any CPU
//		{A45A1E35-6CE5-42C2-8FD3-19CE5B0BD5D6}.Release|Any CPU.Build.0 = Release|Any CPU
//		{EA6365F1-C027-416B-B4CC-A94572B5FC19}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
//		{EA6365F1-C027-416B-B4CC-A94572B5FC19}.Debug|Any CPU.Build.0 = Debug|Any CPU
//		{EA6365F1-C027-416B-B4CC-A94572B5FC19}.Release|Any CPU.ActiveCfg = Release|Any CPU
//		{EA6365F1-C027-416B-B4CC-A94572B5FC19}.Release|Any CPU.Build.0 = Release|Any CPU
//		{F9F4EF44-D894-4B0C-8A6F-AE422D98576C}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
//		{F9F4EF44-D894-4B0C-8A6F-AE422D98576C}.Debug|Any CPU.Build.0 = Debug|Any CPU
//		{F9F4EF44-D894-4B0C-8A6F-AE422D98576C}.Release|Any CPU.ActiveCfg = Release|Any CPU
//		{F9F4EF44-D894-4B0C-8A6F-AE422D98576C}.Release|Any CPU.Build.0 = Release|Any CPU
//		{10F7C080-CF92-4AF1-B12A-32B55FBDFA34}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
//		{10F7C080-CF92-4AF1-B12A-32B55FBDFA34}.Debug|Any CPU.Build.0 = Debug|Any CPU
//		{10F7C080-CF92-4AF1-B12A-32B55FBDFA34}.Release|Any CPU.ActiveCfg = Release|Any CPU
//		{10F7C080-CF92-4AF1-B12A-32B55FBDFA34}.Release|Any CPU.Build.0 = Release|Any CPU
//		{89C4BA5C-358B-4095-8FD2-0BAE32C2178E}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
//		{89C4BA5C-358B-4095-8FD2-0BAE32C2178E}.Debug|Any CPU.Build.0 = Debug|Any CPU
//		{89C4BA5C-358B-4095-8FD2-0BAE32C2178E}.Release|Any CPU.ActiveCfg = Release|Any CPU
//		{89C4BA5C-358B-4095-8FD2-0BAE32C2178E}.Release|Any CPU.Build.0 = Release|Any CPU
//		{AFB15952-0DFA-4919-9D4B-46C2737654EC}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
//		{AFB15952-0DFA-4919-9D4B-46C2737654EC}.Debug|Any CPU.Build.0 = Debug|Any CPU
//		{AFB15952-0DFA-4919-9D4B-46C2737654EC}.Release|Any CPU.ActiveCfg = Release|Any CPU
//		{AFB15952-0DFA-4919-9D4B-46C2737654EC}.Release|Any CPU.Build.0 = Release|Any CPU
//		{1E5CEFF5-C803-4632-8E65-85CF3E5CE672}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
//		{1E5CEFF5-C803-4632-8E65-85CF3E5CE672}.Debug|Any CPU.Build.0 = Debug|Any CPU
//		{1E5CEFF5-C803-4632-8E65-85CF3E5CE672}.Release|Any CPU.ActiveCfg = Release|Any CPU
//		{1E5CEFF5-C803-4632-8E65-85CF3E5CE672}.Release|Any CPU.Build.0 = Release|Any CPU
//		{CEBC603E-59AD-4BD7-839D-CCFE7C0841B1}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
//		{CEBC603E-59AD-4BD7-839D-CCFE7C0841B1}.Debug|Any CPU.Build.0 = Debug|Any CPU
//		{CEBC603E-59AD-4BD7-839D-CCFE7C0841B1}.Release|Any CPU.ActiveCfg = Release|Any CPU
//		{CEBC603E-59AD-4BD7-839D-CCFE7C0841B1}.Release|Any CPU.Build.0 = Release|Any CPU
//		{D555A345-1D09-42F6-B090-15A08E763B1D}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
//		{D555A345-1D09-42F6-B090-15A08E763B1D}.Debug|Any CPU.Build.0 = Debug|Any CPU
//		{D555A345-1D09-42F6-B090-15A08E763B1D}.Release|Any CPU.ActiveCfg = Release|Any CPU
//		{D555A345-1D09-42F6-B090-15A08E763B1D}.Release|Any CPU.Build.0 = Release|Any CPU
//		{BCE782F2-71EA-43C0-A361-FE79666CCC06}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
//		{BCE782F2-71EA-43C0-A361-FE79666CCC06}.Debug|Any CPU.Build.0 = Debug|Any CPU
//		{BCE782F2-71EA-43C0-A361-FE79666CCC06}.Release|Any CPU.ActiveCfg = Release|Any CPU
//		{BCE782F2-71EA-43C0-A361-FE79666CCC06}.Release|Any CPU.Build.0 = Release|Any CPU
//		{36C405AE-737F-4EC4-8568-84CBD0AE9B9A}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
//		{36C405AE-737F-4EC4-8568-84CBD0AE9B9A}.Debug|Any CPU.Build.0 = Debug|Any CPU
//		{36C405AE-737F-4EC4-8568-84CBD0AE9B9A}.Release|Any CPU.ActiveCfg = Release|Any CPU
//		{36C405AE-737F-4EC4-8568-84CBD0AE9B9A}.Release|Any CPU.Build.0 = Release|Any CPU
//		{FC477F23-2BCC-4917-841E-691726EB0894}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
//		{FC477F23-2BCC-4917-841E-691726EB0894}.Debug|Any CPU.Build.0 = Debug|Any CPU
//		{FC477F23-2BCC-4917-841E-691726EB0894}.Release|Any CPU.ActiveCfg = Release|Any CPU
//		{FC477F23-2BCC-4917-841E-691726EB0894}.Release|Any CPU.Build.0 = Release|Any CPU
//		{BE5EE943-1E0F-4E41-AF76-9A90DB103CC8}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
//		{BE5EE943-1E0F-4E41-AF76-9A90DB103CC8}.Debug|Any CPU.Build.0 = Debug|Any CPU
//		{BE5EE943-1E0F-4E41-AF76-9A90DB103CC8}.Release|Any CPU.ActiveCfg = Release|Any CPU
//		{BE5EE943-1E0F-4E41-AF76-9A90DB103CC8}.Release|Any CPU.Build.0 = Release|Any CPU
//		{1FACC9A8-230C-45D6-A195-22F5B4D6A0D2}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
//		{1FACC9A8-230C-45D6-A195-22F5B4D6A0D2}.Debug|Any CPU.Build.0 = Debug|Any CPU
//		{1FACC9A8-230C-45D6-A195-22F5B4D6A0D2}.Release|Any CPU.ActiveCfg = Release|Any CPU
//		{1FACC9A8-230C-45D6-A195-22F5B4D6A0D2}.Release|Any CPU.Build.0 = Release|Any CPU
//		{556218EB-5B3E-437F-8533-C9B22335AEC5}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
//		{556218EB-5B3E-437F-8533-C9B22335AEC5}.Debug|Any CPU.Build.0 = Debug|Any CPU
//		{556218EB-5B3E-437F-8533-C9B22335AEC5}.Release|Any CPU.ActiveCfg = Release|Any CPU
//		{556218EB-5B3E-437F-8533-C9B22335AEC5}.Release|Any CPU.Build.0 = Release|Any CPU
//		{A02F6B05-AFEC-4421-829B-050AC244B4DC}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
//		{A02F6B05-AFEC-4421-829B-050AC244B4DC}.Debug|Any CPU.Build.0 = Debug|Any CPU
//		{A02F6B05-AFEC-4421-829B-050AC244B4DC}.Release|Any CPU.ActiveCfg = Release|Any CPU
//		{A02F6B05-AFEC-4421-829B-050AC244B4DC}.Release|Any CPU.Build.0 = Release|Any CPU
//		{97AA2ECD-0B72-417A-A1D3-60B154F1653B}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
//		{97AA2ECD-0B72-417A-A1D3-60B154F1653B}.Debug|Any CPU.Build.0 = Debug|Any CPU
//		{97AA2ECD-0B72-417A-A1D3-60B154F1653B}.Release|Any CPU.ActiveCfg = Release|Any CPU
//		{97AA2ECD-0B72-417A-A1D3-60B154F1653B}.Release|Any CPU.Build.0 = Release|Any CPU
//		{85E30F16-2131-40AC-BB1C-2D5873E1D9A5}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
//		{85E30F16-2131-40AC-BB1C-2D5873E1D9A5}.Debug|Any CPU.Build.0 = Debug|Any CPU
//		{85E30F16-2131-40AC-BB1C-2D5873E1D9A5}.Release|Any CPU.ActiveCfg = Release|Any CPU
//		{85E30F16-2131-40AC-BB1C-2D5873E1D9A5}.Release|Any CPU.Build.0 = Release|Any CPU
//		{465923F8-4AD3-4507-AB2D-64F1B778225C}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
//		{465923F8-4AD3-4507-AB2D-64F1B778225C}.Debug|Any CPU.Build.0 = Debug|Any CPU
//		{465923F8-4AD3-4507-AB2D-64F1B778225C}.Release|Any CPU.ActiveCfg = Release|Any CPU
//		{465923F8-4AD3-4507-AB2D-64F1B778225C}.Release|Any CPU.Build.0 = Release|Any CPU
//		{2CF4F063-181D-48F7-A2F4-FB73E77183E9}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
//		{2CF4F063-181D-48F7-A2F4-FB73E77183E9}.Debug|Any CPU.Build.0 = Debug|Any CPU
//		{2CF4F063-181D-48F7-A2F4-FB73E77183E9}.Release|Any CPU.ActiveCfg = Release|Any CPU
//		{2CF4F063-181D-48F7-A2F4-FB73E77183E9}.Release|Any CPU.Build.0 = Release|Any CPU
//		{74795D16-CBD3-46B5-91F9-EEAF682F681D}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
//		{74795D16-CBD3-46B5-91F9-EEAF682F681D}.Debug|Any CPU.Build.0 = Debug|Any CPU
//		{74795D16-CBD3-46B5-91F9-EEAF682F681D}.Release|Any CPU.ActiveCfg = Release|Any CPU
//		{74795D16-CBD3-46B5-91F9-EEAF682F681D}.Release|Any CPU.Build.0 = Release|Any CPU
//		{A55DD9DB-5254-4D31-888D-05474318035D}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
//		{A55DD9DB-5254-4D31-888D-05474318035D}.Debug|Any CPU.Build.0 = Debug|Any CPU
//		{A55DD9DB-5254-4D31-888D-05474318035D}.Release|Any CPU.ActiveCfg = Release|Any CPU
//		{A55DD9DB-5254-4D31-888D-05474318035D}.Release|Any CPU.Build.0 = Release|Any CPU
//	EndGlobalSection
//	GlobalSection(SolutionProperties) = preSolution
//		HideSolutionNode = FALSE
//	EndGlobalSection
//	GlobalSection(NestedProjects) = preSolution
//		{D2C9F430-6B47-482A-A49C-A2478D75F95F} = {99ECAE38-204D-4947-B45A-6B4F8C963C2E}
//		{F62D7792-F4EE-4924-898E-C56309E6DFC2} = {99ECAE38-204D-4947-B45A-6B4F8C963C2E}
//		{C0BAC0EB-7068-42AF-ACAC-09DF677CF980} = {99ECAE38-204D-4947-B45A-6B4F8C963C2E}
//		{A45A1E35-6CE5-42C2-8FD3-19CE5B0BD5D6} = {99ECAE38-204D-4947-B45A-6B4F8C963C2E}
//		{EA6365F1-C027-416B-B4CC-A94572B5FC19} = {99ECAE38-204D-4947-B45A-6B4F8C963C2E}
//		{F9F4EF44-D894-4B0C-8A6F-AE422D98576C} = {5F589A1E-007A-4A15-BE6D-EEB69A7DBCFC}
//		{89C4BA5C-358B-4095-8FD2-0BAE32C2178E} = {99ECAE38-204D-4947-B45A-6B4F8C963C2E}
//		{AFB15952-0DFA-4919-9D4B-46C2737654EC} = {99ECAE38-204D-4947-B45A-6B4F8C963C2E}
//		{CEBC603E-59AD-4BD7-839D-CCFE7C0841B1} = {99ECAE38-204D-4947-B45A-6B4F8C963C2E}
//		{D555A345-1D09-42F6-B090-15A08E763B1D} = {5F589A1E-007A-4A15-BE6D-EEB69A7DBCFC}
//		{BCE782F2-71EA-43C0-A361-FE79666CCC06} = {5F589A1E-007A-4A15-BE6D-EEB69A7DBCFC}
//		{36C405AE-737F-4EC4-8568-84CBD0AE9B9A} = {5F589A1E-007A-4A15-BE6D-EEB69A7DBCFC}
//		{FC477F23-2BCC-4917-841E-691726EB0894} = {99ECAE38-204D-4947-B45A-6B4F8C963C2E}
//		{BE5EE943-1E0F-4E41-AF76-9A90DB103CC8} = {5F589A1E-007A-4A15-BE6D-EEB69A7DBCFC}
//		{1FACC9A8-230C-45D6-A195-22F5B4D6A0D2} = {5F589A1E-007A-4A15-BE6D-EEB69A7DBCFC}
//		{556218EB-5B3E-437F-8533-C9B22335AEC5} = {99ECAE38-204D-4947-B45A-6B4F8C963C2E}
//		{97AA2ECD-0B72-417A-A1D3-60B154F1653B} = {5F589A1E-007A-4A15-BE6D-EEB69A7DBCFC}
//		{85E30F16-2131-40AC-BB1C-2D5873E1D9A5} = {99ECAE38-204D-4947-B45A-6B4F8C963C2E}
//		{465923F8-4AD3-4507-AB2D-64F1B778225C} = {5F589A1E-007A-4A15-BE6D-EEB69A7DBCFC}
//		{74795D16-CBD3-46B5-91F9-EEAF682F681D} = {5F589A1E-007A-4A15-BE6D-EEB69A7DBCFC}
//		{A55DD9DB-5254-4D31-888D-05474318035D} = {99ECAE38-204D-4947-B45A-6B4F8C963C2E}
//	EndGlobalSection
//	GlobalSection(ExtensibilityGlobals) = postSolution
//		SolutionGuid = {948E3B0B-6C0A-4B59-A7CD-3A3583756279}
//	EndGlobalSection
//	GlobalSection(TeamFoundationVersionControl) = preSolution
//		SccNumberOfProjects = 26
//		SccEnterpriseProvider = {4CA58AB2-18FA-4F8D-95D4-32DDF27D184C}
//		SccTeamFoundationServer = http://120.27.140.63:8080/tfs/workspace
//		SccLocalPath0 = .
//		SccProjectUniqueName1 = ..\\..\\CodeArt\u0020Framework\\CodeArt\\CodeArt\\CodeArt.csproj
//		SccProjectTopLevelParentUniqueName1 = PortalService.sln
//		SccProjectName1 = ../../CodeArt\u0020Framework/CodeArt/CodeArt
//		SccLocalPath1 = ..\\..\\CodeArt\u0020Framework\\CodeArt\\CodeArt
//		SccProjectUniqueName2 = ..\\..\\CodeArt\u0020Framework\\CodeArt\\CodeArt.DomainDriven\\CodeArt.DomainDriven.csproj
//		SccProjectTopLevelParentUniqueName2 = PortalService.sln
//		SccProjectName2 = ../../CodeArt\u0020Framework/CodeArt/CodeArt.DomainDriven
//		SccLocalPath2 = ..\\..\\CodeArt\u0020Framework\\CodeArt\\CodeArt.DomainDriven
//		SccProjectUniqueName3 = ..\\..\\CodeArt\u0020Framework\\CodeArt\\CodeArt.DomainDriven.DataAccess\\CodeArt.DomainDriven.DataAccess.csproj
//		SccProjectTopLevelParentUniqueName3 = PortalService.sln
//		SccProjectName3 = ../../CodeArt\u0020Framework/CodeArt/CodeArt.DomainDriven.DataAccess
//		SccLocalPath3 = ..\\..\\CodeArt\u0020Framework\\CodeArt\\CodeArt.DomainDriven.DataAccess
//		SccProjectUniqueName4 = ..\\..\\CodeArt\u0020Framework\\CodeArt\\CodeArt.DomainDriven.Extensions\\CodeArt.DomainDriven.Extensions.csproj
//		SccProjectTopLevelParentUniqueName4 = PortalService.sln
//		SccProjectName4 = ../../CodeArt\u0020Framework/CodeArt/CodeArt.DomainDriven.Extensions
//		SccLocalPath4 = ..\\..\\CodeArt\u0020Framework\\CodeArt\\CodeArt.DomainDriven.Extensions
//		SccProjectUniqueName5 = ..\\..\\CodeArt\u0020Framework\\CodeArt\\CodeArt.ServiceModel\\CodeArt.ServiceModel.csproj
//		SccProjectTopLevelParentUniqueName5 = PortalService.sln
//		SccProjectName5 = ../../CodeArt\u0020Framework/CodeArt/CodeArt.ServiceModel
//		SccLocalPath5 = ..\\..\\CodeArt\u0020Framework\\CodeArt\\CodeArt.ServiceModel
//		SccProjectUniqueName6 = ..\\..\\Subsystems\u0020Framework\\AccountSubsystem\\AccountSubsystem\\AccountSubsystem.csproj
//		SccProjectTopLevelParentUniqueName6 = PortalService.sln
//		SccProjectName6 = ../../Subsystems\u0020Framework/AccountSubsystem/AccountSubsystem
//		SccLocalPath6 = ..\\..\\Subsystems\u0020Framework\\AccountSubsystem\\AccountSubsystem
//		SccProjectUniqueName7 = PortalService.Application\\PortalService.Application.csproj
//		SccProjectName7 = PortalService.Application
//		SccLocalPath7 = PortalService.Application
//		SccProjectUniqueName8 = ..\\..\\CodeArt\u0020Framework\\CodeArt\\CodeArt.RabbitMQ\\CodeArt.RabbitMQ.csproj
//		SccProjectTopLevelParentUniqueName8 = PortalService.sln
//		SccProjectName8 = ../../CodeArt\u0020Framework/CodeArt/CodeArt.RabbitMQ
//		SccLocalPath8 = ..\\..\\CodeArt\u0020Framework\\CodeArt\\CodeArt.RabbitMQ
//		SccProjectUniqueName9 = ..\\..\\CodeArt\u0020Framework\\CodeArt\\CodeArt.EasyMQ\\CodeArt.EasyMQ.csproj
//		SccProjectTopLevelParentUniqueName9 = PortalService.sln
//		SccProjectName9 = ../../CodeArt\u0020Framework/CodeArt/CodeArt.EasyMQ
//		SccLocalPath9 = ..\\..\\CodeArt\u0020Framework\\CodeArt\\CodeArt.EasyMQ
//		SccProjectUniqueName10 = PortalService\\PortalService.csproj
//		SccProjectName10 = PortalService
//		SccLocalPath10 = PortalService
//		SccProjectUniqueName11 = ..\\..\\CodeArt\u0020Framework\\CodeArt\\CodeArt.ServiceModel.MQ\\CodeArt.ServiceModel.MQ.csproj
//		SccProjectTopLevelParentUniqueName11 = PortalService.sln
//		SccProjectName11 = ../../CodeArt\u0020Framework/CodeArt/CodeArt.ServiceModel.MQ
//		SccLocalPath11 = ..\\..\\CodeArt\u0020Framework\\CodeArt\\CodeArt.ServiceModel.MQ
//		SccProjectUniqueName12 = ..\\..\\Subsystems\u0020Framework\\LocationSubsystem\\LocationSubsystem\\LocationSubsystem.csproj
//		SccProjectTopLevelParentUniqueName12 = PortalService.sln
//		SccProjectName12 = ../../Subsystems\u0020Framework/LocationSubsystem/LocationSubsystem
//		SccLocalPath12 = ..\\..\\Subsystems\u0020Framework\\LocationSubsystem\\LocationSubsystem
//		SccProjectUniqueName13 = ..\\..\\Subsystems\u0020Framework\\FileSubsystem\\FileSubsystem\\FileSubsystem.csproj
//		SccProjectTopLevelParentUniqueName13 = PortalService.sln
//		SccProjectName13 = ../../Subsystems\u0020Framework/FileSubsystem/FileSubsystem
//		SccLocalPath13 = ..\\..\\Subsystems\u0020Framework\\FileSubsystem\\FileSubsystem
//		SccProjectUniqueName14 = ..\\..\\Subsystems\u0020Framework\\UserSubsystem\\UserSubsystem\\UserSubsystem.csproj
//		SccProjectTopLevelParentUniqueName14 = PortalService.sln
//		SccProjectName14 = ../../Subsystems\u0020Framework/UserSubsystem/UserSubsystem
//		SccLocalPath14 = ..\\..\\Subsystems\u0020Framework\\UserSubsystem\\UserSubsystem
//		SccProjectUniqueName15 = ..\\..\\CodeArt\u0020Framework\\CodeArt\\CodeArt.Web\\CodeArt.Web.csproj
//		SccProjectTopLevelParentUniqueName15 = PortalService.sln
//		SccProjectName15 = ../../CodeArt\u0020Framework/CodeArt/CodeArt.Web
//		SccLocalPath15 = ..\\..\\CodeArt\u0020Framework\\CodeArt\\CodeArt.Web
//		SccProjectUniqueName16 = ..\\..\\Subsystems\u0020Framework\\UtilSubsystem\\UtilSubsystem\\UtilSubsystem.csproj
//		SccProjectTopLevelParentUniqueName16 = PortalService.sln
//		SccProjectName16 = ../../Subsystems\u0020Framework/UtilSubsystem/UtilSubsystem
//		SccLocalPath16 = ..\\..\\Subsystems\u0020Framework\\UtilSubsystem\\UtilSubsystem
//		SccProjectUniqueName17 = ..\\..\\Subsystems\u0020Framework\\MessageSubsystem\\MessageSubsystem\\MessageSubsystem.csproj
//		SccProjectTopLevelParentUniqueName17 = PortalService.sln
//		SccProjectName17 = ../../Subsystems\u0020Framework/MessageSubsystem/MessageSubsystem
//		SccLocalPath17 = ..\\..\\Subsystems\u0020Framework\\MessageSubsystem\\MessageSubsystem
//		SccProjectUniqueName18 = ..\\..\\CodeArt\u0020Framework\\CodeArt\\CodeArt.TestPlatform\\CodeArt.TestPlatform.csproj
//		SccProjectTopLevelParentUniqueName18 = PortalService.sln
//		SccProjectName18 = ../../CodeArt\u0020Framework/CodeArt/CodeArt.TestPlatform
//		SccLocalPath18 = ..\\..\\CodeArt\u0020Framework\\CodeArt\\CodeArt.TestPlatform
//		SccProjectUniqueName19 = PortalService.Util\\PortalService.Util.csproj
//		SccProjectName19 = PortalService.Util
//		SccLocalPath19 = PortalService.Util
//		SccProjectUniqueName20 = ..\\..\\Subsystems\u0020Framework\\MessageSubsystem\\MessageSubsystem.Util\\MessageSubsystem.Util.csproj
//		SccProjectTopLevelParentUniqueName20 = PortalService.sln
//		SccProjectName20 = ../../Subsystems\u0020Framework/MessageSubsystem/MessageSubsystem.Util
//		SccLocalPath20 = ..\\..\\Subsystems\u0020Framework\\MessageSubsystem\\MessageSubsystem.Util
//		SccProjectUniqueName21 = ..\\..\\CodeArt\u0020Framework\\CodeArt\\CodeArt.MessagePush\\CodeArt.MessagePush.csproj
//		SccProjectTopLevelParentUniqueName21 = PortalService.sln
//		SccProjectName21 = ../../CodeArt\u0020Framework/CodeArt/CodeArt.MessagePush
//		SccLocalPath21 = ..\\..\\CodeArt\u0020Framework\\CodeArt\\CodeArt.MessagePush
//		SccProjectUniqueName22 = ..\\LogService\\LogService.Util\\LogService.Util.csproj
//		SccProjectTopLevelParentUniqueName22 = PortalService.sln
//		SccProjectName22 = ../LogService/LogService.Util
//		SccLocalPath22 = ..\\LogService\\LogService.Util
//		SccProjectUniqueName23 = ..\\QualityService\\QualityService.Util\\QualityService.Util.csproj
//		SccProjectName23 = ../QualityService/QualityService.Util
//		SccLocalPath23 = ..\\QualityService\\QualityService.Util
//		SccProjectUniqueName24 = ..\\..\\Subsystems\u0020Framework\\TagSubsystem\\TagSubsystem\\TagSubsystem.csproj
//		SccProjectTopLevelParentUniqueName24 = PortalService.sln
//		SccProjectName24 = ../../Subsystems\u0020Framework/TagSubsystem/TagSubsystem
//		SccLocalPath24 = ..\\..\\Subsystems\u0020Framework\\TagSubsystem\\TagSubsystem
//		SccProjectUniqueName25 = ..\\..\\CodeArt\u0020Framework\\CodeArt\\CodeArt.TestTools\\CodeArt.TestTools.csproj
//		SccProjectTopLevelParentUniqueName25 = PortalService.sln
//		SccProjectName25 = ../../CodeArt\u0020Framework/CodeArt/CodeArt.TestTools
//		SccLocalPath25 = ..\\..\\CodeArt\u0020Framework\\CodeArt\\CodeArt.TestTools
//	EndGlobalSection
//EndGlobal
