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
using System.Windows.Shapes;

namespace DC
{
    /// <summary>
    /// Логика взаимодействия для Program.xaml
    /// </summary>
    public partial class ProgramWindow : Window
    {
        public ProgramWindow()
        {
            InitializeComponent();
        }

        private void MenuItem_Exit(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MenuItem_DataBases(object sender, RoutedEventArgs e)
        {
            DBWindow db = new DBWindow();
            db.Owner = this;
            this.Visibility = Visibility.Hidden;
            db.Show();
        }

        private void MenuItem_AddServer(object sender, RoutedEventArgs e)
        {
            ServerManager severManager = new ServerManager();
            severManager.ShowDialog();
        }

        private void MenuItem_SystemSettings(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            SystemSettingsWindow ss = new SystemSettingsWindow();
            ss.Owner = this;
            ss.ShowDialog();
        }
    }
}
