namespace ChangeBook.BookInfo.Interface
{
    /// <summary>
    /// Содержит всю информацию для правильной работы с файлом книги
    /// </summary>
    interface IFileInfo
    {
        /// <summary>
        /// Полный путь к файлу книги в каталоге (с названием и расширением)
        /// </summary>
        string BookFullName { get; }
        /// <summary>
        /// Новый полный путь к файлу книги в каталоге (с найденными названием книги и автором)
        /// </summary>
        string NewBookFullName { get; }
        /// <summary>
        /// Epub имя файла в архиве, в котором находится информация об авторе и названии книги 
        /// </summary>
        string NameOfEntryInZipRegex { get; }
        /// <summary>
        /// Элемент в разметке файла, где находится имя автора
        /// </summary>
        string NameOfElementAuthor { get; }
        /// <summary>
        /// Элемент в разметке файла, где находится имя название книги
        /// </summary>
        string NameOfElementTitle { get; }
        /// <summary>
        /// Fb2 Элемент в разметке файла, имя автора
        /// </summary>
        string AuthorFName { get; }
        /// <summary>
        /// Fb2 Элемент в разметке файла, отчество автора
        /// </summary>
        string AuthorMName { get; }
        /// <summary>
        /// Fb2 Элемент в разметке файла, фамилия автора
        /// </summary>
        string AuthorLName { get; }
        /// <summary>
        /// Наличие исключения при работе с файлом
        /// </summary>
        string Exception { get; set; }
    }
}
