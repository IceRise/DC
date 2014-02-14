using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;
using System.Data;
using System.ComponentModel;
using System.Linq;
namespace DC
{
    /// <summary>
    /// Вікно для роботи з еталонною базою даних
    /// </summary>
    public partial class DBWindow : Window
    {

        //Екземпляр, котрий є джерелом данних для основної таблиці DataGrid
        GetMSSQLData fillDataGrid;
        DataTable dt = new DataTable();

        //Таблиці, які були знайдені в процесі пошуку на інших серверах
        List<string> foundTables = new List<string>();

        public DBWindow()
        {
            InitializeComponent();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.Owner.Visibility = Visibility.Visible;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SessionParameters.IntegratedSecurity)
                    SessionParameters.Connection = string.Format("Server={0};Database={1};Integrated security=true",
                        SessionParameters.EtalonServer,
                        SessionParameters.EtalonDataBase);
                else
                    SessionParameters.Connection = string.Format("Server={0};Database={1};UID={2};Password={3}",
                        SessionParameters.EtalonServer,
                        SessionParameters.EtalonDataBase,
                        SessionParameters.Login,
                        SessionParameters.Password);

                //Отримання списку таблиць з еталонної БД і присвоєння їх до DBList
                SqlCommand command = new SqlCommand("select * from sys.tables where type_desc = 'USER_TABLE';");
                GetMSSQLData getDataBases = new GetMSSQLData(command);
                DBList.ItemsSource = getDataBases.GetColumnData(0).Where(s => s != "sysdiagrams");
                DBList.Items.SortDescriptions.Add(new SortDescription(null, ListSortDirection.Ascending));
            }
            catch
            {
                MessageBox.Show("Неможливо отримати список серверів", "Помилка", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            Commit.IsEnabled = false;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Пошук таблиць по серверам
            SearchTable();
            MS.Items.Clear();
            Composer();
            //MS_ListBox.ItemsSource = 
            //Отримання даних з обраної таблиці та приписування їх до DataGridу
            fillDataGrid = new GetMSSQLData((string)DBList.SelectedItem);
            dataGrid.ItemsSource = fillDataGrid.Table.DefaultView;
            try
            {
                fillDataGrid.Table.Columns[fillDataGrid.PrimaryKey].ReadOnly = true;
                dataGrid.Columns[fillDataGrid.Table.Columns["vcChangeDate"].Ordinal].IsReadOnly = true;
            }
            catch { }
            dt = fillDataGrid.Table;
            dt.TableNewRow += dt_TableNewRow;
        }

        private void Button_Commit(object sender, RoutedEventArgs e)
        {
            try
            {
                SQLUpdater s = new SQLUpdater();
                s.Update(fillDataGrid, MS.Items);
                fillDataGrid.Table.AcceptChanges();
                Commit.IsEnabled = false;
                Messager.Info("Оновлення виконано успішно", "Успішно");
            }
            catch (Exception ex)
            {
                Messager.Error(ex.Message, "Помилка");
            }
        }

        private void Composer()
        {
            List<string> servers = new List<string>(new string[] { SessionParameters.EtalonServer });

            foundTables.ForEach(element =>
            {
                if (!servers.Exists(item => item == element.Split('/')[0]))
                {
                    servers.Add(element);
                }
            });

            servers.ForEach(server =>
            {
                TreeViewItem tvi = new TreeViewItem();
                tvi.Name = server;
                tvi.Header = server;
                foreach (string s in foundTables)
                {
                    if (s.Split('/')[0] == server)
                    {
                        TreeViewItem child = new TreeViewItem();
                        child.Header = s.Split('/')[1];
                        child.MouseDoubleClick += child_MouseDoubleClick;

                        tvi.Items.Add(child);
                    }
                }
                if (tvi.Items.Count > 0)
                    MS.Items.Add(tvi);
            });
        }

        void child_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ShowDB();
        }


        /// <summary>
        /// Пошук таблиць з тим самим ім'ям, шо й таблиця, вибрана в DBList
        /// </summary>
        private List<string> SearchTable()
        {
            //Для динамічного оновлення результату пошуку
            MS.ItemsSource = null;
            foundTables.Clear();

            List<string> List = StreamWorker.ReadList("ServerList.ini");
            List<string> serverList = new List<string>();
            serverList.Add(SessionParameters.EtalonServer);

            //Відділення типу БД від назви серверу
            foreach (string s in serverList)
            {
                if (s.Split('|')[0] == "MSSQL")
                    serverList.Add(s.Split('|')[1]);
            }

            //Початок пошуку 
            string connection;
            //Для кожного серверу...
            foreach (string server in serverList)
            {
                if (SessionParameters.IntegratedSecurity)
                { connection = string.Format("Server={0};Integrated security=true", server); }
                else
                { connection = string.Format("Server={0};UID={1};Password={2}", server, SessionParameters.Login, SessionParameters.Password); }

                //Отримання списку баз даних та їх запис до dataBaseList
                BaseDataTable dataBasesTable = new GetMSSQLData();
                using (SqlConnection getDataBases = new SqlConnection(connection))
                {
                    getDataBases.Open();
                    dataBasesTable.Table = getDataBases.GetSchema("Databases");
                    getDataBases.Close();
                }

                string[] dataBaseList = dataBasesTable.GetColumnData(0);

                //Для кожної БД...
                foreach (string dataBase in dataBaseList)
                {
                    string selectDBCommand = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES;";
                    string selectDBConnection = connection + "; DataBase=" + dataBase;
                    GetMSSQLData takenDataTables = new GetMSSQLData(selectDBConnection, selectDBCommand);

                    //Для кожної таблиці...
                    if (takenDataTables.GetColumnData(0).Where((s) => s == DBList.SelectedItem.ToString()).Count() > 0)
                    {
                        foundTables.Add(server + "/" + dataBase);
                    }
                }

            }
            return foundTables;
        }

        void dt_TableNewRow(object sender, DataTableNewRowEventArgs e)
        {
            try
            {
                dt = null;
                DateTime d = DateTime.Now;
                e.Row[fillDataGrid.Table.Columns["vcChangeDate"].Ordinal] = DateTime.Now;
            }
            catch { }
        }

        private void dataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            dt = fillDataGrid.Table;
        }

        private void Button_RejectChanges(object sender, RoutedEventArgs e)
        {

            try
            {
                fillDataGrid.Table.RejectChanges();
                Commit.IsEnabled = false;
            }
            catch (Exception)
            { }

        }

        private void dataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {

        }

        private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            Commit.IsEnabled = true;
        }

        private void ShowDB()
        {
            TreeViewItem dataBase = (TreeViewItem)ServerTree.SelectedItem;
            TreeViewItem server = (TreeViewItem)dataBase.Parent;
            TreeViewItem serverType = (TreeViewItem)server.Parent;

            SqlConnection sc = new SqlConnection(String.Format("Server={0}; Database={1}; Integrated security=true", server.Header, dataBase.Header));
            SqlCommand command = sc.CreateCommand();
            command.CommandText = "SELECT * FROM " + DBList.SelectedValue.ToString();
            sc.Open();
            using (SqlDataReader r = command.ExecuteReader())
            {

                DataTable dt = new DataTable();
                dt.Load(r);
                ExportTable.DT = dt;
            }
            sc.Close();

            TableViewer tv = new TableViewer();
            tv.Show();

        }
    }

    public static class ExportTable
    {
        public static DataTable DT { get; set; }
    }
}