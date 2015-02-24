using System.IO;
using System.Windows.Forms;

namespace Bia.SvnGuard.Services
{
    public class FileSystemService
    {
        public string SelectFolder()
        {
            var dialog = new FolderBrowserDialog();
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                return dialog.SelectedPath;
            }

            return null;
        }

        public string SelectFile()
        {
            var dialog = new OpenFileDialog();
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                return dialog.FileName;
            }

            return null;
        }

        public string[] ListFolders(string path)
        {
            return Directory.GetDirectories(path);
        }
    }
}
