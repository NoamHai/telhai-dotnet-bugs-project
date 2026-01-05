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
using TelHai.CS.DotNet.GalHelner_NoamHai.HW5.Repositories;

namespace TelHai.CS.DotNet.GalHelner_NoamHai.HW5
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

        private void BtnOpenBugsWindow_Click(object sender, RoutedEventArgs e)
        {
            BugsWindow bugsWindow = new BugsWindow();
            bugsWindow.Show();
        }

        private void BtnOpenCategoriesWindow_Click(object sender, RoutedEventArgs e)
        {
            CategoriesWindow categoriesWindow = new CategoriesWindow();
            categoriesWindow.Show();
        }

        private void BtnWriteCompositeToFile_Click(object sender, RoutedEventArgs e)
        {
            CategorySqlRepository categorySqlRepository = CategorySqlRepository.GetInstance();
            categorySqlRepository.WriteCompositeTreeToFile();
        }
    }
}
