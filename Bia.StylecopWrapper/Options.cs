using CommandLine;
using CommandLine.Text;

namespace Bia.StylecopWrapper
{
    class Options
    {
        [Option('s', "stylecop", HelpText="Path to StyleCop Dlls", Required = true)]
        public string StylecopPath { get; set; }

        [Option('r', "repository", HelpText = "Repository which recieves commited files", Required = true)]
        public string Repository { get; set; }

        [Option('v', "revision", HelpText = "Transaction number with files to analyze", Required = true)]
        public string Transaction { get; set; }

        [Option('l', "svnlook", HelpText = "Path to svnlook utility", Required = true)]
        public string SvnLookPath { get; set; }

        [Option('t', "tempfolder", HelpText = "Path to temp folder", Required = true)]
        public string TempFolder { get; set; }

        [Option('e', "allow-empty-comment", HelpText = "If you need to ignore empty comments", Required = false)]
        public bool AllowEmptyComment { get; set; }

        [Option('p', "settings", HelpText = "StyleCop settings file", Required = true)]
        public string StylecopSettingsPath { get; set; }

        [Option('f', "target-framework", HelpText = ".NET Framework version of code in SVN repository", DefaultValue = 4.5)]
        public double TargetFrameworkVersion { get; set; }

        [ParserState]
        public IParserState LastParserState
        {
            get;
            set;
        }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}