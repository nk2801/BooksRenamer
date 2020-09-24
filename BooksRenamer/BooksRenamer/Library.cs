using ChangeBook.BookInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChangeBook
{
    /// <summary>
    /// Содержит в себе все данные для отображения в UI
    /// </summary>
    class Library : INotifyPropertyChanged
    {
        List<BookExample> library;
        int countChecked = 0;
        int countAll = 0;
        int countEnd = 0;
        int countCheckedNow = 0;

        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Событие изменения свойства для своевременного отображения
        /// </summary>
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// Создание нового экземпляра класса
        /// </summary>
        /// <param name="dir">Каталог, в котором происходит поиск книг</param>
        /// <param name="allFolders">True - проверять все подкаталоги. False - проверять только выбранную папку</param>
        public Library(string dir, bool allFolders)
        {
            library = new List<BookExample>();
            Directory = dir;
            AllFolders = allFolders;
        }
        /// <summary>
        /// Поиск книг в выбранном каталоге
        /// Книги создаются здесь, чтобы выполнить подписку на событие изменения Checked книги, проверять ее далее или нет
        /// </summary>
        public void FindBooks()
        {
            library.Clear();
            foreach (var book in FindBookFiles.FindBookFilesInDirectory(Directory, AllFolders))
            {
                BookExample be = BookExample.CreateBook(book);
                be.PropertyChanged += this.IsChecked;
                Books.Add(be);
            }
            CountChecked = CountCheckedNow = CountAll = Books.Count;
            CountEnd = 0;
        }
        /// <summary>
        /// Проверяет событие, если связано с изменением checked книги - изменяет значение в UI
        /// </summary>
        public void IsChecked(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsCheckedTrue")
            {
                CountChecked++;
            }
            if (e.PropertyName == "IsCheckedFalse")
            {
                CountChecked--;
            }
        }
        /// <summary>
        /// Поиск новых данных о выбранных книгах
        /// </summary>
        public void FindBookInfo()
        {
            CountEnd = 0;
            CountCheckedNow = CountChecked;
            foreach (var book in library)
            {
                if (book.IsChecked)
                {
                    FindNewBookInfo.FindNewName(book);
                    CountEnd++;
                }
            }
        }
        /// <summary>
        /// переименование выбранных книг
        /// </summary>
        public void Rename()
        {
            CountEnd = 0;
            CountCheckedNow = CountChecked;
            foreach (var book in library)
            {
                if (book.IsChecked)
                {
                    RenameBookFiles.RenameFiles(book);
                    CountEnd++;
                }
            }
        }
        public bool AllFolders { get; set; }
        public string Directory { get; private set; }
        public List<BookExample> Books { get { return library; } private set { library = value; NotifyPropertyChanged(); } }
        public int CountAll { get { return countAll; } private set { countAll = value; NotifyPropertyChanged(); } }
        public int CountChecked { get { return countChecked; } private set { countChecked = value; } }
        public int CountCheckedNow { get { return countCheckedNow; } private set { countCheckedNow = value; NotifyPropertyChanged(); } }
        public int CountEnd { get { return countEnd; } private set { countEnd = value; NotifyPropertyChanged(); } }

    }
}
