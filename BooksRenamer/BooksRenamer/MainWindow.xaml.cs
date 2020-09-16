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
        List<BookExample> library;
        string directory;
        bool allFolders;
        public MainWindow()
        {
            //library = new List<BookExample>();
            allFolders = false;
            InitializeComponent();
        }
        /// <summary>
        /// Нажатие кнопки "обзор" для выбора папки с книгами
        /// </summary>
        private void btnOverview_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult dRes = fbd.ShowDialog();
            if (dRes == System.Windows.Forms.DialogResult.OK)
            {
                tbDirectory.Text = directory = fbd.SelectedPath.ToString();
            }
        }
        /// <summary>
        /// Нажатие кнопки "Поиск книг"
        /// </summary>
        private void btnSearchBooks_Click(object sender, RoutedEventArgs e)
        {
            FindBooks();
        }
        /// <summary>
        /// Асинхронное выполнение поиска книг в папках
        /// </summary>
        private async void FindBooks()
        {
            await Task.Run(() =>
            {
                library = ChangeBook.FindBookFiles.FindBookFilesInDirectory(directory, allFolders);
            }
            );
            MainDataGrid.ItemsSource = library;
            progressBar.Maximum = library.Count();
            progressBar.Value = 0;
        }
        /// <summary>
        /// Нажатие кнопки "Поиск названий книг"
        /// </summary>
        private void btnSearchNewTitle_Click(object sender, RoutedEventArgs e)
        {
            FindBookName();
        }
        /// <summary>
        /// Асинхронное выполнение поиска названий книг и авторов
        /// </summary>
        private async void FindBookName()
        {
            await Task.Run(() =>
            {
                foreach (var book in library)
                {
                    ChangeBook.FindNewBookInfo.FindNewName(book);
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(MoveProgressBar));
                }
            }
            );
        }
        /// <summary>
        /// Нажатие кнопки "Переименовать"
        /// </summary>
        private void btnRename_Click(object sender, RoutedEventArgs e)
        {
            Rename();
        }
        /// <summary>
        /// Асинхронное выполнение переименовывания файлов книг
        /// </summary>
        private async void Rename()
        {
            progressBar.Maximum = library.Count(x => x.IsChecked == true);
            progressBar.Value = 0;
            await Task.Run(() =>
            {
                foreach (var book in library)
                {
                    ChangeBook.RenameBookFiles.RenameFiles(book);
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(MoveProgressBar));
                }
            }
            );
        }
        /// <summary>
        /// Для ассинхронного изменения значения прогрессбара
        /// </summary>
        private void MoveProgressBar()
        {
            progressBar.Value++;
        }
        /// <summary>
        /// Изменение значения проверки всех папок или нет
        /// </summary>
        private void rbtnAllFolders_Checked(object sender, RoutedEventArgs e)
        {
            if ((sender as System.Windows.Controls.RadioButton).Name == "rbtnOnlyRoot")
            {
                allFolders = false;
            }
            if ((sender as System.Windows.Controls.RadioButton).Name == "rbtnAlsoSubFolders")
            {
                allFolders = true;
            }
        }
    }
}
