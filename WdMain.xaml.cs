
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;


namespace files_to_one_dir
{
    /// <summary>
    /// WdMain.xaml 的交互逻辑
    /// </summary>
    public partial class WdMain : Window
    {
        public WdMain()
        {
            InitializeComponent();
        }
        static void GetAllFiles(string dir, List<string> allFiles)
        {
            DirectoryInfo di = new DirectoryInfo(dir);
            if (!di.Exists) return;
            allFiles.AddRange(di.GetFiles().Select(p => p.FullName));//将当前目录文件放到allFiles中
            di.GetDirectories().ToList().ForEach(p => GetAllFiles(p.FullName, allFiles));//将子目录中的文件放入allFiles中
        }

        private async void BtnSelect_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog m_Dialog = new FolderBrowserDialog();
            DialogResult result = m_Dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }

            var sourceDir = m_Dialog.SelectedPath.Trim();


            var files = new List<string>();
            GetAllFiles(sourceDir, files);

            var destDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\" + new DirectoryInfo(sourceDir).Name + "-Files";


            if (!Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }
            else
            {
                Console.WriteLine("already exists");
            }


            Dictionary<string, int> dictNameCount = new Dictionary<string, int>();
            foreach (var file in files)
            {
                var name = new FileInfo(file).Name;
                if (dictNameCount.ContainsKey(name))
                {
                    dictNameCount[name] += 1;
                }
                else
                {
                    dictNameCount[name] = 1;
                }
            }


            int index = 0;
            int count = files.ToList().Count;

            foreach (var file in files)
            {
                index++;
                var content = $"{index}/{count}\n{file}";
                TbInfo.Text = content;

                string destFile;
                var name = new FileInfo(file).Name;
                if (dictNameCount[name] == 1)
                {
                    destFile = destDir + "\\" + new FileInfo(file).Name;
                }
                else
                {
                    destFile = destDir + "\\" + file.Replace("\\", "-").Replace(":", "-");
                }

                await Task.Run(() => { File.Copy(file, destFile, overwrite: true); });
            }
            TbInfo.Text = sourceDir + "\n文件整理已完成 :)";

        }
    }
}
