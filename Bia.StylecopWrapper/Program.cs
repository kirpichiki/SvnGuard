using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using CommandLine;
using CommandLine.Text;

namespace Bia.StylecopWrapper
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();
            if (Parser.Default.ParseArguments(args, options))
            {
                var localSourceFiles = new List<string>();
                string tempFolder = @"D:\Dropbox\Projects\Windows\Bia.SvnGuard\Bia.SvnGuard\bin\Debug\Temp";
                SaveCommittedFilesToTemp(tempFolder, localSourceFiles, options.Repository, options.Revision);
                ValidateFiles(options, tempFolder);
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        public static void OnViolationEncountered(object sender, object violationEventArgs)
        {
            Console.WriteLine(violationEventArgs.GetType().GetProperty("Message").GetValue(violationEventArgs));
        }

        private static void ValidateFiles(Options options, string path)
        {
            var asm = Assembly.LoadFile(Path.Combine(options.StylecopPath, "StyleCop.dll"));
            var styleCopConsoleType = asm.GetType("StyleCop.StyleCopConsole");
            var styleCopConsole = Activator.CreateInstance(styleCopConsoleType, null, false, null, null, true);

            var codeProjectType = asm.GetType("StyleCop.CodeProject");
            var configurationType = asm.GetType("StyleCop.Configuration");
            var configuration = Activator.CreateInstance(configurationType, new object[] {null});
            var codeProject = Activator.CreateInstance(codeProjectType, 0, path, configuration);
                
            var files = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var core = styleCopConsole.GetType().GetProperty("Core").GetValue(styleCopConsole);
                var environment = core.GetType().GetProperty("Environment").GetValue(core);
                environment.GetType().GetMethod("AddSourceCode").Invoke(environment, new[] { codeProject, file, null });
            }

            var projects = CreateList(codeProjectType, new [] { codeProject });
            var eventInfo = styleCopConsole.GetType().GetEvent("ViolationEncountered");
            eventInfo.AddEventHandler(styleCopConsole, Delegate.CreateDelegate(eventInfo.EventHandlerType, typeof(Program).GetMethod("OnViolationEncountered")));

            styleCopConsole.GetType().GetMethod("Start").Invoke(styleCopConsole, new object[] { projects, true });
        }

        private static void SaveCommittedFilesToTemp(string tempFolder, List<string> localSourceFiles , string repositoryPath, int revision)
        {
            ProcessStartInfo svnLookPsi = new ProcessStartInfo(@"C:\Program Files\TortoiseSVN\bin\svnlook.exe");
            svnLookPsi.UseShellExecute = svnLookPsi.ErrorDialog = false;
            svnLookPsi.RedirectStandardOutput = true;
            Process svnLookProcess = new Process();
            svnLookProcess.StartInfo = svnLookPsi;


            svnLookPsi.Arguments = String.Format("log --revision {0} \"{1}\"", revision, repositoryPath);
            svnLookProcess.Start();
            string comment = svnLookProcess.StandardOutput.ReadToEnd();
            if (String.IsNullOrWhiteSpace(comment))
            {
                return;
            }

            svnLookPsi.Arguments = string.Format("changed \"{0}\" {1}", repositoryPath, string.Format("-r {0}", revision));

            svnLookProcess.Start();

            string[] committedFiles = svnLookProcess.StandardOutput.ReadToEnd().Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            Regex fileNameExtractor = new Regex("^[AU] *(.*)");
            
            foreach (string committedRow in committedFiles)
            {
                string committedFile = fileNameExtractor.Match(committedRow).Groups[1].Value;
                if (true) // need to analyze file
                {
                    string outFileName = Path.Combine(tempFolder, committedFile.Replace("/", "\\"));
                    Directory.CreateDirectory(Path.GetDirectoryName(outFileName));

                    svnLookPsi.Arguments = string.Format("cat \"{0}\" \"{1}\" {2}", repositoryPath, committedFile, string.Format("-r {0}", revision));

                    svnLookProcess.Start();

                    StreamWriter outFile = new StreamWriter(outFileName, false, Encoding.UTF8);
                    outFile.Write(svnLookProcess.StandardOutput.ReadToEnd());
                    outFile.Close();

                    localSourceFiles.Add(outFileName);
                }
            }
        }

        static IList CreateList(Type type, object[] objects)
        {
            var genericListType = typeof(List<>);
            var specificListType = genericListType.MakeGenericType(type);
            var list = (IList)Activator.CreateInstance(specificListType);
            foreach (var o in objects)
            {
                list.Add(Convert.ChangeType(o, type));
            }
            return list;
        }
    }



    class Options
    {
        [Option('s', "stylecop", HelpText="Path to StyleCop Dlls", Required = true)]
        public string StylecopPath { get; set; }

        [Option('r', "repository", HelpText = "Repository which recieves commited files", Required = true)]
        public string Repository { get; set; }

        [Option('v', "revision", HelpText = "Revision number with files to analyze", Required = true)]
        public int Revision { get; set; }

        [Option('e', "allow-empty-comment", HelpText = "If you need to ignore empty comments", Required = false)]
        public bool AllowEmptyComment { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
