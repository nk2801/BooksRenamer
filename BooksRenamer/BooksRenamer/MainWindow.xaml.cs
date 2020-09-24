using ChangeBook;
using ChangeBook.BookInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;

namespace BooksRenamer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Library library;
        string directory;
        bool allFolders = false;
        public MainWindow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Нажатие кнопки "обзор" для выбора папки с книгами
        /// </summary>
        private void btnOverview_Click(object sender, RoutedEventArgs e)
        {
            //FolderBrowserDialog fbd = new FolderBrowserDialog();
            //DialogResult dRes = fbd.ShowDialog();
            //if (dRes == System.Windows.Forms.DialogResult.OK)
            //{
            //    tbDirectory.Text = directory = fbd.SelectedPath.ToString();
            //}
            tbDirectory.Text = directory = @"D:\!Books";
            library = new Library(directory, allFolders);
        }
        /// <summary>
        /// Нажатие кнопки "Поиск книг"
        /// Асинхронное выполнение поиска книг в папках
        /// </summary>
        private async void btnSearchBooks_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => library.FindBooks());
            DataContext = library;
            MainDataGrid.Items.Refresh();
        }
        /// <summary>
        /// Нажатие кнопки "Поиск названий книг"
        /// Асинхронное выполнение поиска названий книг и авторов
        /// </summary>
        private async void btnSearchNewTitle_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => library.FindBookInfo());
        }
        /// <summary>
        /// Нажатие кнопки "Переименовать"
        /// Асинхронное выполнение переименовывания файлов книг
        /// </summary>
        private async void btnRename_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => library.Rename());
        }
        /// <summary>
        /// Изменение значения проверки всех папок или нет
        /// </summary>
        private void rbtnAllFolders_Checked(object sender, RoutedEventArgs e)
        {
            if ((sender as System.Windows.Controls.RadioButton).Name == "rbtnOnlyRoot")
            {
                if (library == null)
                    allFolders = false;
                else
                    library.AllFolders = false;
            }
            if ((sender as System.Windows.Controls.RadioButton).Name == "rbtnAlsoSubFolders")
            {
                if (library == null)
                    allFolders = true;
                else
                    library.AllFolders = true;
            }
        }
    }
}
