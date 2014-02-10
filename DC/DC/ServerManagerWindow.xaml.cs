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
    /// Логика взаимодействия для ServerManager.xaml
    /// </summary>
    public partial class ServerManager : Window
    {
        public ServerManager()
        {
            InitializeComponent();
        }

        public List<string> servList = new List<string>();

        private void Button_AddServer(object sender, RoutedEventArgs e)
        {
            AddServer addServer = new AddServer();
            addServer.Owner = this;
            addServer.ShowDialog();
        }

        private void Button_Delete(object sender, RoutedEventArgs e)
        {
            servList.RemoveAt(ServerListBox.SelectedIndex);
            ServerListBox.ItemsSource = null;
            ServerListBox.ItemsSource = servList;

            StreamWriter serverListWriter = new StreamWriter("ServerList.ini", false);
            foreach (string s in servList)
            {
                serverListWriter.WriteLine(s);
            }
            serverListWriter.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!File.Exists("ServerList.ini"))
            {
                File.Create("ServerList.ini");
            }
            servList = StreamWorker.ReadList("ServerList.ini");
            ServerListBox.ItemsSource = servList;
        }

        private void ServerTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

        }

        private void Button_CLose(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
