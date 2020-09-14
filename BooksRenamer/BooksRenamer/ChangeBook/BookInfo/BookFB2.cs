namespace BooksRenamer.ChangeBook.BookInfo
{
    /// <summary>
    /// Главная информация о файле FB2
    /// </summary>
    internal class BookFB2 : BookExample
    {
        public BookFB2(string path) : base(path) { }
        public override string NameOfEntryInZipRegex { get { return null; } }
        public override string NameOfElementTitle { get { return "book-title"; } }
        public override string NameOfElementAuthor { get { return "author"; } }
        public override string AuthorFName { get { return "first-name"; } }
        public override string AuthorMName { get { return "middle-name"; } }
        public override string AuthorLName { get { return "last-name"; } }
    }
}
