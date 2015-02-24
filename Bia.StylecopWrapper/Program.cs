using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CommandLine;

namespace Bia.StylecopWrapper
{
    class Program
    {
        static readonly List<string> Violations = new List<string>();
 
        static int Main(string[] args)
        {
            var options = new Options();
            if (Parser.Default.ParseArguments(args, options))
            {
                DeleteTempFolder(options);
                
                var svnLook = new SvnLook(options.SvnLookPath);
                if (!CheckComment(svnLook, options))
                {
                    Console.Error.WriteLine("Please put a comment that describes changes you made in the code");
                    return (int) ReturnCode.EmptyComment;
                }

                SaveCommittedFilesToTemp(svnLook, options);
                ValidateFiles(options, options.TempFolder);

                if (Violations.Count > 0)
                {
                    Console.Error.WriteLine("There are StyleCop violations in your code");
                    foreach (var violation in Violations)
                    {
                        Console.Error.WriteLine(violation);
                    }

                    return (int) ReturnCode.ViolationsFound;
                }

                return (int) ReturnCode.Ok;
            }

            Console.Error.WriteLine(options.LastParserState.Errors.First().BadOption.LongName);
            return (int) ReturnCode.WrongParameters;
        }

        public static void OnViolationEncountered(object sender, object violationEventArgs)
        {
            var violation = violationEventArgs.GetValue("Violation");
            var violationDescription = String.Format(
                "{0}: {1} in {3}:{2}",
                violation.GetValue("Rule.CheckId"),
                violation.GetValue("Message"),
                violation.GetValue("Line"),
                violation.GetValue("SourceCode.Name"));
            Violations.Add(violationDescription);
        }

        private static void ValidateFiles(Options options, string path)
        {
            var asm = Assembly.LoadFile(Path.Combine(options.StylecopPath, "StyleCop.dll"));
            var styleCopConsole = asm.CreateInstance("StyleCop.StyleCopConsole", options.StylecopSettingsPath, false, null, null, true);
            var configuration = asm.CreateInstance("StyleCop.Configuration", new object[] {null});
            var codeProject = asm.CreateInstance("StyleCop.CodeProject", 0, path, configuration);

            string[] files;
            try
            {
                files = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);
            }
            catch
            {
                return;
            }
            
            foreach (var file in files)
            {
                styleCopConsole.GetValue("Core.Environment").InvokeMethod("AddSourceCode", codeProject, file, null);
            }

            var projects = asm.GetType("StyleCop.CodeProject").CreateList(codeProject);
            styleCopConsole.AddEventHandler("ViolationEncountered", typeof(Program).GetMethod("OnViolationEncountered"));
            styleCopConsole.InvokeMethod("Start", projects, true);
        }

        private static void SaveCommittedFilesToTemp(SvnLook svnLook, Options options)
        {
            var files = svnLook.GetChangedFiles(options.Repository, options.Transaction);
            foreach (string file in files)
            {
                string outFileName = Path.Combine(options.TempFolder, file);
                Directory.CreateDirectory(Path.GetDirectoryName(outFileName));
                svnLook.DownloadFile(options.Repository, options.Transaction, file, outFileName);
            }
        }

        private static bool CheckComment(SvnLook svnLook, Options options)
        {
            var comment = svnLook.GetCommitComment(options.Repository, options.Transaction);
            return !String.IsNullOrWhiteSpace(comment) || options.AllowEmptyComment;
        }

        private static void DeleteTempFolder(Options options)
        {
            if (Directory.Exists(options.TempFolder))
            {
                Directory.Delete(options.TempFolder, true);
            }
        }
    }
}
