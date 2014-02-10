using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для AddServer.xaml
    /// </summary>
    public partial class AddServer : Window
    {
        public AddServer()
        {
            InitializeComponent();
        }

        private void Button_Add(object sender, RoutedEventArgs e)
        {
            ServerManager parent = this.Owner as ServerManager;

            StreamWriter DataBaseWriter = new StreamWriter("ServerList.ini", true);
            DataBaseWriter.WriteLine(string.Format("{0}|{1}", TypeBox.Text, NameBox.Text));
            parent.servList.Add(string.Format("{0}|{1}", TypeBox.Text, NameBox.Text));
            DataBaseWriter.Close();

            parent.ServerListBox.ItemsSource = null;
            parent.ServerListBox.ItemsSource = parent.servList;
            this.Close();
        }

        private void Button_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
