using ChangeBook.BookInfo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeBook
{
    /// <summary>
    /// Для изменения старого имени файла книги на новое в каталоге
    /// </summary>
    internal static class RenameBookFiles
    {
        /// <summary>
        /// Переименовывает книгу в каталоге
        /// </summary>
        /// <param name="book">Книга, которую надо переименовать</param>
        public static void RenameFiles(BookExample book)
        {
            if (book.IsChecked)
                try
                {
                    File.Move(book.BookFullName, book.NewBookFullName);
                }
                catch (Exception ex)
                {
                    book.Exception = ex.Message;
                }
        }
    }
}
