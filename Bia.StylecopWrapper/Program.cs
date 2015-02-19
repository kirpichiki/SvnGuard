using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
                var path = @"C:\_SVN\SintekMonitor";
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

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        public static void OnViolationEncountered(object sender, object violationEventArgs)
        {
            Console.WriteLine(violationEventArgs.GetType().GetProperty("Message").GetValue(violationEventArgs));
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

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
