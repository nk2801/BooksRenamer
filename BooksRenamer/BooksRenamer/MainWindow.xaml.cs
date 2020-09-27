using ChangeBook;
using ChangeBook.BookInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;

namespace BooksRenamer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Library library;
        CancellationTokenSource cts;
        CancellationToken token;
        public MainWindow()
        {
            library = new Library();
            InitializeComponent();
            DataContext = library;

            library.PropertyChanged += ChangeState;
            ChangeState(null, new PropertyChangedEventArgs("ChangeState"));

            cts = new CancellationTokenSource();
            token = cts.Token;
        }
        /// <summary>
        /// Нажатие кнопки "обзор" для выбора папки с книгами
        /// </summary>
        private void btnOverview_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult dRes = fbd.ShowDialog();
            if (dRes == System.Windows.Forms.DialogResult.OK)
            {
                library.Directory = fbd.SelectedPath.ToString();
            }
            //library.Directory = @"D:\!Books";
        }
        /// <summary>
        /// Проверка нажатия на кнопки, связанные с поиском и обработкой книг.
        /// Поиск книг
        /// Поиск названий книг
        /// Переименовать
        /// </summary>
        private void btn_Click(object sender, RoutedEventArgs e)
        {
            string nameButton = ((Button)sender).Name;
            switch (nameButton)
            {
                case "btnSearchBooks":
                    library.FindBooks(token);
                    break;
                case "btnSearchNewTitle":
                    library.FindBookInfo(token);
                    break;
                case "btnRename":
                    library.Rename(token);
                    break;
                default: break;
            }
        }
        /// <summary>
        /// Проверка статуса библиотеки для соответсвующего отображения активных кнопок и подсказки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChangeState(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "ChangeState")
                return;
            switch (library.ChangeState)
            {
                case State.Start:
                    tbState.Text = "Для начала работы выберите папку с файлами";
                    ChangeEnabled(false, false, false, false, false);
                    break;
                case State.Working:
                    tbState.Text = "Выполняется работа";
                    ChangeEnabled(false, false, false, false, false);
                    break;
                case State.FindFiles:
                    tbState.Text = "Можно выполнить поиск книг";
                    ChangeEnabled(true, false, false, true, true);
                    break;
                case State.FindTitle:
                    tbState.Text = "Можно выполнить поиск информации о книгах";
                    ChangeEnabled(true, true, false, true, true);
                    MainDataGrid.Items.Refresh();
                    break;
                case State.Rename:
                    tbState.Text = "Можно выполнить переименование";
                    ChangeEnabled(true, false, true, true, true);
                    break;
                case State.End:
                    tbState.Text = "Работа завершена. Можете выбрать новую папку.";
                    ChangeEnabled(true, false, false, true, true);
                    break;
            }
        }
        /// <summary>
        /// Изменение активности кнопок 
        /// </summary>
        /// <param name="x1">Кнопка Поиск книг</param>
        /// <param name="x2">Кнопка Поиск названий</param>
        /// <param name="x3">Кнопка Переименовать</param>
        /// <param name="x4">Радиобаттон Только одна папка</param>
        /// <param name="x5">Радиобаттон Проверка подпапок</param>
        private void ChangeEnabled(bool x1, bool x2, bool x3, bool x4, bool x5)
        {
            btnSearchBooks.IsEnabled = x1;
            btnSearchNewTitle.IsEnabled = x2;
            btnRename.IsEnabled = x3;
            rbtnOnlyRoot.IsEnabled = x4;
            rbtnAlsoSubFolders.IsEnabled = x5;
        }
        /// <summary>
        /// Изменение значения проверки всех папок или нет
        /// </summary>
        private void rbtnAllFolders_Checked(object sender, RoutedEventArgs e)
        {
            if ((sender as System.Windows.Controls.RadioButton).Name == "rbtnOnlyRoot")
            {
                library.AllFolders = false;
            }
            if ((sender as System.Windows.Controls.RadioButton).Name == "rbtnAlsoSubFolders")
            {
                library.AllFolders = true;
            }
        }
        /// <summary>
        /// Действите при закрытии приложения - если работа не закончена - вопрос на закрытие
        /// </summary>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (library.ChangeState != State.End && library.ChangeState != State.Start)
                if (OpenMessageBox())
                    cts.Cancel();
                else e.Cancel = true;
        }
        /// <summary>
        /// Информационное сообщение на уверенность закрытия приложения
        /// </summary>
        public bool OpenMessageBox()
        {
            MessageBoxResult result = MessageBox.Show($"Вы не завершили работу.\nВы действительно хотите закончить?",
                                                      "Внимание",
                                                      MessageBoxButton.YesNo,
                                                      MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                return true;
            }
            return false;
        }
    }
}
