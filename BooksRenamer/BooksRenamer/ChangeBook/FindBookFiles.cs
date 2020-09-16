using ChangeBook.BookInfo;
using System.Collections.Generic;
using System.IO;

namespace ChangeBook
{
    /// <summary>
    /// Для нахождения всех файлов книг в каталоге и подпапках
    /// </summary>
    internal static class FindBookFiles
    {
        /// <summary>
        /// Паттерн для нахождения файлов только расширения FB2 и Epub
        /// </summary>
        private static readonly string bookExtPattern = @"([.]([Ff][Bb][2])|([Ee][Pp][Uu][Bb]))$";

        /// <summary>
        /// Находит все файлы книг в каталоге. Возвращает Лист книг.
        /// Если есть хоть одна книга, она будет добавлена в Лист.
        /// Если allFolders - true, будут проверены все подпапки.
        /// </summary>
        /// <param name="directory">Путь в каталоге, где будет проверяться наличие книг</param>
        /// <param name="allFolders">Если true, будут проверены подпапки</param>
        /// <returns></returns>
        public static List<BookExample> FindBookFilesInDirectory(string directory, bool allFolders)
        {
            return ScanFolders(new DirectoryInfo(directory), allFolders);
        }
        /// <summary>
        /// Проверяет каталог на наличие книг и подпапок
        /// </summary>
        /// <param name="dInfo">Путь в каталоге, где будет проверяться наличие книг</param>
        /// <param name="allFolders">Если true, будут проверены все подпапки</param>
        /// <returns></returns>
        private static List<BookExample> ScanFolders(DirectoryInfo dInfo, bool allFolders)
        {
            List<BookExample> library = new List<BookExample>();
            try
            {
                library = AddBookInLibrary(dInfo);
                if (allFolders && dInfo.GetDirectories() != null)
                    foreach (var item in dInfo.GetDirectories())
                    {
                        library.AddRange(ScanFolders(new DirectoryInfo(item.FullName.ToString()), allFolders));
                    }
            }
            catch { }
            return library;
        }
        /// <summary>
        /// Просматривает каталог и добавляет книги в Лист
        /// </summary>
        /// <param name="dInfo">Путь в каталоге, где будет проверяться наличие книг</param>
        /// <returns></returns>
        private static List<BookExample> AddBookInLibrary(DirectoryInfo dInfo)
        {
            List<BookExample> lib = new List<BookExample>();
            foreach (var file in dInfo.GetFiles())
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(file.Name, bookExtPattern))
                {
                    lib.Add(BookExample.CreateBook(file.FullName));
                }
            }
            return lib;
        }
    }
}
