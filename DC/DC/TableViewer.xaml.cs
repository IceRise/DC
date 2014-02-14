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
    /// Логика взаимодействия для TableViewer.xaml
    /// </summary>
    public partial class TableViewer : Window
    {
        public TableViewer()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DG.ItemsSource = ExportTable.DT.DefaultView;
            DG.IsReadOnly = true;
        }
    }
}
