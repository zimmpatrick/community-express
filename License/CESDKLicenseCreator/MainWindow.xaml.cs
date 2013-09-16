using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Xml;
using System.Diagnostics;
using System.IO;

namespace CESDKLicenseCreator
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            string strAppId = textBox1.Text;
            int appId = 0;

            if (string.IsNullOrWhiteSpace(strAppId))
            {
                MessageBox.Show("App Id must be entered");
                textBox1.Focus();
                return;
            }
            else if (!int.TryParse(strAppId, out appId))
            {
                MessageBox.Show("App Id must contain only digits");
                textBox1.Focus();
                return;
            }

            XmlDocument document = new XmlDocument();

            XmlElement licenseElement = document.CreateElement("License");
            XmlElement AppIdElement = document.CreateElement("AppId");
            AppIdElement.InnerText = strAppId;
            licenseElement.AppendChild(AppIdElement);
            document.AppendChild(licenseElement);

            // license the document
            CESDKLicense.CESDKLicense.AppendLicenseData(document);

            // get the desktop directory
            // make a subdirectory called \Licenses
            // make a subdirectory called \Licenses\[appid]
            // write the file to \Licenses\[appid]\CESDKLicense.xml

            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string subPath = Path.Combine(path, @"Licenses");
            string subsubPath = Path.Combine(subPath, strAppId);
            string finalPath = Path.Combine(subsubPath, @"CESDKLicense.xml");

            Directory.CreateDirectory(subsubPath);

            using (XmlTextWriter writer = new XmlTextWriter(finalPath, Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                document.WriteTo(writer);
            }

            // open the folder \Licenses\[appid]\
            ProcessStartInfo psi = new ProcessStartInfo(subsubPath);
            psi.UseShellExecute = true;
            Process.Start(psi);
        }
    }
}
