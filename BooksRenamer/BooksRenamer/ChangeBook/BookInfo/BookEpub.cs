namespace ChangeBook.BookInfo
{
    /// <summary>
    /// Главная информация о файле Epub
    /// </summary>
    internal class BookEpub : BookExample
    {
        public BookEpub(string path) : base(path) { }
        public override string NameOfEntryInZipRegex { get { return @"[Cc][Oo][Nn][Tt][Ee][Nn][Tt].[Oo][Pp][Ff]"; } }
        public override string NameOfElementTitle { get { return "dc:title"; } }
        public override string NameOfElementAuthor { get { return "dc:creator"; } }
        public override string AuthorFName { get { return null; } }
        public override string AuthorMName { get { return null; } }
        public override string AuthorLName { get { return null; } }
    }
}
