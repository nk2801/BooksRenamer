using ChangeBook.BookInfo.Interface;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace ChangeBook.BookInfo
{
    /// <summary>
    /// Абстрактный класс, включающий информацию о книге и о файле
    /// </summary>
    public abstract class BookExample : IBookInfo, IFileInfo, INotifyPropertyChanged
    {
        string author;
        string title;
        bool isChecked;
        string exception;
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Событие, сообщает, если какие-либо данные были изменены
        /// </summary>
        /// <param name="propertyName"></param>
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// Создать новую книгу FB2 или Epub
        /// </summary>
        /// <param name="path">Полный путь к книге в каталоге</param>
        /// <returns></returns>
        public static BookExample CreateBook(string path)
        {
            switch (Path.GetExtension(path).ToLower())
            {
                case ".fb2":
                    return new BookFB2(path);
                case ".epub":
                    return new BookEpub(path);
                default:
                    return null;//throw new ArgumentException();
            }
        }
        /// <summary>
        /// Создать новую книгу
        /// </summary>
        /// <param name="path">Полный путь к книге в каталоге</param>
        protected BookExample(string path)
        {
            BookFullName = path;
            Directory = Path.GetDirectoryName(BookFullName);
            FileName = Path.GetFileNameWithoutExtension(BookFullName);
            Extension = Path.GetExtension(BookFullName).ToLower();
            MainFolder = Directory.Substring(Directory.LastIndexOf('\\') + 1);
            Author = "";
            Title = "";
            IsChecked = true;
        }

        #region Properties
        public string Directory { get; private set; }
        public string MainFolder { get; private set; }
        public string FileName { get; private set; }
        public string Extension { get; private set; }
        public string BookFullName { get; private set; }
        public string NewBookFullName { get; private set; }
        public string Exception
        {
            get { return exception; }
            set { exception = value; IsChecked = false; NotifyPropertyChanged(); }
        }
        public bool IsChecked
        {
            get { return isChecked; }
            set { isChecked = value; NotifyPropertyChanged(); }
        }
        public string Author
        {
            get { return author; }
            private set { this.author = value; NotifyPropertyChanged(); }
        }
        public string Title
        {
            get { return title; }
            private set { this.title = value; NotifyPropertyChanged(); }
        }
        #endregion

        public void AddNewName(string author, string title)
        {
            if (author != "" && title != "")
            {
                Author = author;
                Title = title;
                NewBookFullName = Directory + "\\" + Title + " - " + Author + Extension;
            }
            else if (Exception == null)
            {
                Exception = "Данные об авторе и/или названии книги отсуствуют";
                NewBookFullName = BookFullName;
            }
        }

        #region abstract FileInfo
        public abstract string NameOfEntryInZipRegex { get; }
        public abstract string NameOfElementAuthor { get; }
        public abstract string NameOfElementTitle { get; }
        public abstract string AuthorFName { get; }
        public abstract string AuthorMName { get; }
        public abstract string AuthorLName { get; }
        #endregion
    }
}
