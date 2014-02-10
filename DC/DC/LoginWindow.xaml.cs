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
using System.IO;
using System.Data.SqlClient;

namespace DC
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Enter(object sender, RoutedEventArgs e)
        {
            bool enter = false;
            SqlConnection login;
            try
            {
                if ((bool)Windows_Auth.IsChecked)
                {
                    login = new SqlConnection(string.Format("Server={0};Integrated Security=true;Connection Timeout=5",Server_Combo_box.Text));
                    login.Open();
                    login.Close();
                    enter = true;
                }
                else
                {
                    login = new SqlConnection(string.Format("Server={0}; UID={1}; Password={2}; Connection Timeout=5", Server_Combo_box.Text, LoginBox.Text, PasswordBox.Password));
                    login.Open();
                    login.Close();
                    enter = true;
                }
            }
            catch (SqlException)
            {
                MessageBox.Show("Помилка входу. Перевірте введені дані");

            }

            if (enter)
            {
                Dictionary<string, string> settings = new Dictionary<string, string>();

                //Створення файлу у випадку, коли його не існує
                if (!File.Exists("SystemSettings.ini"))
                {
                    StreamWriter fileCreator = new StreamWriter("SystemSettings.ini");
                    fileCreator.Close();
                }

                //Зчитування файлу параметрів
                settings = StreamWorker.ReadDictionary("SystemSettings.ini");
                //Присвоєння парамертів статичному класу
                SessionParameters.Login = LoginBox.Text;
                SessionParameters.Password = PasswordBox.Password;
                SessionParameters.IntegratedSecurity = (bool)Windows_Auth.IsChecked;
                bool access = true;
                try
                {
                    SessionParameters.EtalonServer = settings["Server"];
                    SessionParameters.EtalonDataBase = settings["DataBase"];
                }
                catch
                {
                    MessageBoxResult messageBoxResult = MessageBox.Show(
                        "Даних про еталонний сервер або базу даних не знайдено. Бажаєте перейти до налаштувань?",
                        "Дані не знайдено",
                        MessageBoxButton.YesNo);

                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        SystemSettingsWindow ss = new SystemSettingsWindow();
                        this.Visibility = Visibility.Hidden;
                        ss.ShowDialog();
                        settings = StreamWorker.ReadDictionary("SystemSettings.ini");
                        SessionParameters.EtalonServer = Server_Combo_box.Text;
                        SessionParameters.EtalonDataBase = settings["DataBase"];
                        try
                        {
                            SqlConnection test;
                            if (SessionParameters.IntegratedSecurity)
                                test = new SqlConnection(string.Format("Server={0};Integrated Security=true;", SessionParameters.EtalonServer));
                            else
                                test = new SqlConnection(string.Format("Server={0};UID={1};Password={2}", SessionParameters.EtalonServer));
                            access = true;
                        }
                        catch
                        {
                            MessageBox.Show("Помилка доступу. Перевірте правильність введеного серверу або логіну/пароля");
                            StreamWriter delete = new StreamWriter("SystemSettings.ini", false);
                            //delete.Write("");
                            delete.Close();
                            Application.Current.Shutdown();
                        }
                    }
                    else
                        Application.Current.Shutdown();
                }

                if (access)
                {
                    ProgramWindow workWindow = new ProgramWindow();
                    workWindow.Show();
                    this.Close();
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Dictionary<string, string> settings = StreamWorker.ReadDictionary("SystemSettings.ini");
                Windows_Auth.IsChecked = settings["IntegratedSecurity"] == "true";
            }
            catch { }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Windows_Auth_Click(object sender, RoutedEventArgs e)
        {
            Login_Grid.IsEnabled = !(bool)Windows_Auth.IsChecked;
        }
    }
}
