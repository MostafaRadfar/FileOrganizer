using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Arash;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace FileOrganizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var source = TbSourceDirectory.Text;
            var des = TbDestinationDirectory.Text;
            var t1 = new Task(() => { OrganizeFiles(source, des); });
            t1.Start();

        }

        private static Regex r = new Regex(":");
        public static DateTime? GetDateTakenFromImage(string path)
        {
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                using (System.Drawing.Image myImage = System.Drawing.Image.FromStream(fs, false, false))
                {
                    var propItem = myImage.GetPropertyItem(36867);
                    string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                    return DateTime.Parse(dateTaken);
                }
            }
            catch (Exception a)
            {

                return null;
            }
           
        }

        private void OrganizeFiles(string sourceDir,string DesDir)
        {
            try
            {
                var files = System.IO.Directory.GetFiles(sourceDir);
                var directories = Directory.GetDirectories(sourceDir);

                foreach(var dir in directories)
                {
                    OrganizeFiles(dir, DesDir);
                }
                Dispatcher.BeginInvoke((Action)(() => {
                    Pb1.Maximum = files.Count();
                    Pb1.Value = 0;
                    lblStatus.Content = DesDir;
                }));

                foreach (var file in files)
                {
                    var info = new FileInfo(file);

                    var dateModified = GetDateTakenFromImage(file);
                    if (dateModified.HasValue == false) dateModified= info.LastWriteTime;

                    var persinmodifdate = new PersianDate(dateModified.Value);


                    Directory.CreateDirectory(DesDir + "/" + persinmodifdate.Year.ToString() + "/" + persinmodifdate.Month.ToString());
                    string Newpath = DesDir + "/" + persinmodifdate.Year.ToString() + "/" + persinmodifdate.Month.ToString() + "/" + info.Name ;



                    info.MoveTo(Newpath);

                    Dispatcher.BeginInvoke((Action)(() => {
                       
                        Pb1.Value += 1;
                        lblStatus.Content = DesDir;
                    }));
                }

            }
            catch (Exception a)
            {

                Dispatcher.BeginInvoke((Action)(() => {
                    lblStatus.Foreground = Brushes.Red;
                    lblStatus.Content = a.Message;
                }));
            }
        }

        private void BtnBrowseSource_Click(object sender, RoutedEventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    TbSourceDirectory.Text = fbd.SelectedPath;

                }
            }
        }

        private void BtnBrowseDes_Click(object sender, RoutedEventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    TbDestinationDirectory.Text = fbd.SelectedPath;

                }
            }
        }
    }
}
