using BooksRenamer.ChangeBook.BookInfo;
using BooksRenamer.ChangeBook.BookInfo.Interface;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml;
using System.Xml.Resolvers;

namespace BooksRenamer.ChangeBook
{
    /// <summary>
    /// Для нахождения автора и названия книги в файле книги
    /// </summary>
    internal static class FindNewBookInfo
    {
        /// <summary>
        /// Находит автора и название книги
        /// </summary>
        /// <param name="book">Книга, для которой надо выполнить поиск</param>
        public static void FindNewName(BookExample book)
        {
            string author = "";
            string title = "";
            ParseBook(book, ref author, ref title);
            IsGoodName(ref title, ref author);
            book.AddNewName(author, title);
        }

        #region ParseBook
        /// <summary>
        /// Открывает и загружает файл книги, ловит все ошибки при работе с файлом
        /// </summary>
        /// <param name="book">Книга, для которой надо выполнить поиск</param>
        /// <param name="author">ref. искомое имя автора</param>
        /// <param name="title">ref. искомое название книги</param>
        private static void ParseBook(IFileInfo book, ref string author, ref string title)
        {
            try
            {
                XmlDocument xDoc = LoadBookFile(book);
                if (xDoc != null)
                {
                    author = FindAuthor(xDoc, book);
                    title = FindTitle(xDoc, book);
                }
            }
            catch (System.IO.FileNotFoundException)
            {
                book.Exception = "Файл не найден";
            }
            catch (System.IO.InvalidDataException)
            {
                book.Exception = "Файл поврежден";
            }
            catch (System.Xml.Schema.XmlSchemaException)
            {
                book.Exception = "Файл поврежден";
            }
            catch (System.Xml.XmlException)
            {
                book.Exception = "Файл поврежден";
            }
            //catch (System.NullReferenceException)
            //{
            //    book.Exception = "Файл поврежден";
            //}
            catch (Exception ex)
            {
                book.Exception = ex.Message;
            }
        }

        /// <summary>
        /// Загружает книгу для работы с данными
        /// </summary>
        /// <param name="book">Книга, для которой надо выполнить поиск</param>
        /// <returns></returns>
        private static XmlDocument LoadBookFile(IFileInfo book)
        {
            XmlDocument xDoc = new XmlDocument();
            if (book.NameOfEntryInZipRegex == null)
            {
                xDoc.Load(CreateReaderToLoadBookFB2(book));
            }
            else
            {
                LoadBookFromEpubArchive(book, xDoc);
            }
            return xDoc;
        }

        /// <summary>
        /// Создает ридер для корректной загрузки FB2 (&nbsp; Спецсимволы - неразрывный пробел и прочее)
        /// </summary>
        /// <param name="book">Книга, для которой надо выполнить поиск</param>
        /// <returns></returns>
        private static XmlReader CreateReaderToLoadBookFB2(IFileInfo book)
        {
            XmlParserContext context = new XmlParserContext(null, new XmlNamespaceManager(new NameTable()), null, XmlSpace.None);
            context.DocTypeName = "html";
            context.PublicId = "-//W3C//DTD XHTML 1.0 Strict//EN";
            context.SystemId = "xhtml1-strict.dtd";

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Parse;
            settings.ValidationType = ValidationType.DTD;
            settings.XmlResolver = new XmlPreloadedResolver(XmlKnownDtds.All);

            System.Text.Encoding enc = SearchEncodingFb2(book);

            XmlReader reader = XmlReader.Create(new StringReader(File.ReadAllText(book.BookFullName, enc)), settings, context);
            return reader;
        }

        /// <summary>
        /// Находит кодировку файла FB2 
        /// </summary>
        /// <param name="book">Книга, для которой надо выполнить поиск</param>
        /// <returns></returns>
        private static Encoding SearchEncodingFb2(IFileInfo book)
        {
            using (StreamReader sr = File.OpenText(book.BookFullName))
            {
                if (sr.ReadLine().Contains("1251"))
                {
                    return System.Text.Encoding.GetEncoding("windows-1251");
                }
            }
            return System.Text.Encoding.UTF8;
        }

        /// <summary>
        /// Загружает файл с информацией о книге из Epub архива
        /// </summary>
        /// <param name="book">Книга, для которой надо выполнить поиск</param>
        /// <param name="xDoc">Куда загружать данные из файла</param>
        private static void LoadBookFromEpubArchive(IFileInfo book, XmlDocument xDoc)
        {
            using (ZipArchive epubArchive = ZipFile.OpenRead(book.BookFullName))
            {
                ZipArchiveEntry readmeEntry = OpenEpubZipFile(book, epubArchive);
                if (readmeEntry != null)
                    xDoc.Load(readmeEntry.Open());
            }
        }
        /// <summary>
        /// Открывает Epub архив и находит файл с информацией о книге
        /// </summary>
        /// <param name="book">Книга, для которой надо выполнить поиск</param>
        /// <param name="archive">Архив, где искать файл с информацией о книге</param>
        /// <returns></returns>
        private static ZipArchiveEntry OpenEpubZipFile(IFileInfo book, ZipArchive archive)
        {
            foreach (var entry in archive.Entries)
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(entry.Name, book.NameOfEntryInZipRegex))
                {
                    return archive.GetEntry(entry.FullName);
                }
            }
            return null;
        }
        #endregion

        #region Find Book Info
        /// <summary>
        /// Ищет автора в загруженных данных
        /// </summary>
        /// <param name="book">Книга, для которой надо выполнить поиск</param>
        /// <param name="xDoc">Данные книги, где искать автора</param>
        /// <returns></returns>
        private static string FindAuthor(XmlDocument xDoc, IFileInfo book)
        {
            XmlNode xAuthor = xDoc.GetElementsByTagName(book.NameOfElementAuthor)[0];
            if (xAuthor == null)
                return "";
            return GiveAuthorName(book, xAuthor);
        }

        /// <summary>
        /// Ищет автора в загруженных данных
        /// </summary>
        /// <param name="book">Книга, для которой надо выполнить поиск</param>
        /// <param name="xAuthor">Данные книги, где искать автора </param>
        /// <returns></returns>
        private static string GiveAuthorName(IFileInfo book, XmlNode xAuthor)
        {
            string author = "";
            if (xAuthor.ChildNodes.Count > 1)
            {
                foreach (XmlNode item in xAuthor.ChildNodes)
                    if (item.Name == book.AuthorFName || item.Name == book.AuthorMName || item.Name == book.AuthorLName)
                        author += $"{item.InnerText} ";
            }
            else
                author += xAuthor.InnerText;
            return author;
        }

        /// <summary>
        /// Ищет название книги в загруженных данных
        /// </summary>
        /// <param name="xDoc">Данные книги, где искать название книги</param>
        /// <param name="book">Книга, для которой надо выполнить поиск</param>
        /// <returns></returns>
        private static string FindTitle(XmlDocument xDoc, IFileInfo book)
        {
            return xDoc.GetElementsByTagName(book.NameOfElementTitle)[0].InnerText;
        }
        #endregion

        #region Check string
        /// <summary>
        /// Проверка автора и названия книги для избавления от некорректных символов
        /// </summary>
        /// <param name="newTitle">Название для проверки</param>
        /// <param name="newAuthor">Автор для проверки</param>
        public static void IsGoodName(ref string newTitle, ref string newAuthor)
        {
            IsGoodName(ref newTitle);
            IsGoodName(ref newAuthor);
        }

        public static void IsGoodName(ref string newTitle)
        {
            char[] ex = System.IO.Path.GetInvalidFileNameChars();
            int pos = -1;
            while ((pos = newTitle.LastIndexOfAny(ex)) >= 0)
            {
                newTitle = newTitle.Remove(pos, 1);
            }
            while (newTitle.Contains("  "))
                newTitle = newTitle.Replace("  ", " ");
            newTitle = newTitle.Trim();
        }
        #endregion
    }
}
