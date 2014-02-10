using System.Collections.Generic;
using System.IO;
using System.Windows;
namespace DC
{
    /// <summary>
    /// Логика взаимодействия для SystemSettings.xaml
    /// </summary>
    public partial class SystemSettingsWindow : Window
    {
        ProgramWindow parent;
        public SystemSettingsWindow()
        {
            InitializeComponent();

        }
        Dictionary<string, string> settings;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            parent = this.Owner as ProgramWindow;
            settings = StreamWorker.ReadDictionary("SystemSettings.ini");
            try
            {
                ServerBox.Text = settings["Server"];
                DataBaseBox.Text = settings["DataBase"];
                if (settings["IntegratedSecurity"] == "True")
                {
                    IntegratedSecurityBox.IsChecked = true;
                }
            }
            catch { }
        }

        private void Button_Close(object sender, RoutedEventArgs e)
        {
            parent.Visibility = Visibility.Visible;
            this.Close();
        }

        private void Button_Commit(object sender, RoutedEventArgs e)
        {

            settings["Server"] = ServerBox.Text;
            settings["DataBase"] = DataBaseBox.Text;
            settings["IntegratedSecurity"] = IntegratedSecurityBox.IsChecked.ToString();
            StreamWriter settingsWriter = new StreamWriter("SystemSettings.ini",false);
            foreach (string s in settings.Keys)
            {
                settingsWriter.WriteLine(s + "|" + settings[s]);
            }
            settingsWriter.Close();
            this.Close();
            parent.Visibility = Visibility.Visible;
        }
    }
}
