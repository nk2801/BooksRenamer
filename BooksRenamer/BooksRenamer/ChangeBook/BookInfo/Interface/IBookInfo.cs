namespace BooksRenamer.ChangeBook.BookInfo.Interface
{
    /// <summary>
    /// Содержит основную информацию о самой книге
    /// </summary>
    interface IBookInfo
    {
        /// <summary>
        /// Отметка, следует ли далее работать с этой книгой
        /// </summary>
        bool IsChecked { get; set; }
        /// <summary>
        /// Путь расположения книги
        /// </summary>
        string Directory { get; }
        /// <summary>
        /// Папка, непосредственно в которой находится книга
        /// </summary>
        string MainFolder { get; }
        /// <summary>
        /// Название файла книги в папке
        /// </summary>
        string FileName { get; }
        /// <summary>
        /// Расширение, FB2 или Epub
        /// </summary>
        string Extension { get; }
        /// <summary>
        /// Найденный автор книги
        /// </summary>
        string Author { get; } //set; }
        /// <summary>
        /// Найденное название книги
        /// </summary>
        string Title { get; } //set; }
        /// <summary>
        /// Функция проверки и присвоения нового названия файла книги
        /// </summary>
        /// <param name="author">Найденный автор книги</param>
        /// <param name="title">Найденное название книги</param>
        void AddNewName(string author, string title);
    }
}
