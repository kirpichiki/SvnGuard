using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Bia.StylecopWrapper
{
    public class SvnLook
    {
        private readonly ProcessStartInfo _svnLookPsi;
        private readonly Process _svnLookProcess;

        public SvnLook(string path)
        {
            _svnLookPsi = new ProcessStartInfo(path);
            _svnLookPsi.UseShellExecute = _svnLookPsi.ErrorDialog = false;
            _svnLookPsi.RedirectStandardOutput = true;
            _svnLookProcess = new Process
            {
                StartInfo = _svnLookPsi
            };
        }

        // http://svnbook.red-bean.com/en/1.7/svn.ref.svnlook.c.log.html
        public string GetCommitComment(string repositoryPath, string transaction)
        {
            _svnLookPsi.Arguments = String.Format("log --transaction {0} \"{1}\"", transaction, repositoryPath);
            _svnLookProcess.Start();
            return _svnLookProcess.StandardOutput.ReadToEnd();
        }

        // http://svnbook.red-bean.com/en/1.7/svn.ref.svnlook.c.changed.html
        public IEnumerable<string> GetChangedFiles(string repositoryPath, string transaction)
        {
            _svnLookPsi.Arguments = string.Format("changed \"{0}\" {1}", repositoryPath, string.Format("-t {0}", transaction));
            _svnLookProcess.Start();
            string[] files = _svnLookProcess.StandardOutput.ReadToEnd().Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            // added or modified files only
            var fileNameExtractor = new Regex("^[AU] *(.*)");
            return files.Select(f => fileNameExtractor.Match(f).Groups[1].Value.Replace("/", "\\"));
        }

        // http://svnbook.red-bean.com/en/1.7/svn.ref.svnlook.c.cat.html
        public void DownloadFile(string repositoryPath, string transaction, string sourceFile, string destinationFile)
        {
            _svnLookPsi.Arguments = string.Format("cat \"{0}\" \"{1}\" {2}", repositoryPath, sourceFile, string.Format("-t {0}", transaction));
            _svnLookProcess.Start();
            var outFile = new StreamWriter(destinationFile, false, Encoding.UTF8);
            outFile.Write(_svnLookProcess.StandardOutput.ReadToEnd());
            outFile.Close();
        }
    }
}