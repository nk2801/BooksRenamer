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
    /// Состояние работы приложения
    /// </summary>
    public enum State
    {
        Start, Working, FindFiles, FindTitle, Rename, End
    }
    /// <summary>
    /// Содержит в себе все данные для отображения в UI
    /// </summary>
    class Library : INotifyPropertyChanged
    {
        List<BookExample> library;
        string directory;

        int countChecked = 0;
        int countAll = 0;
        int countEnd = 0;
        int countCheckedNow = 0;

        State state;

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
        public Library()
        {
            library = new List<BookExample>();
            directory = "";
            AllFolders = false;
            ChangeState = State.Start;
        }
        /// <summary>
        /// Поиск книг в выбранном каталоге
        /// Книги создаются здесь, чтобы выполнить подписку на событие изменения Checked книги, проверять ее далее или нет
        /// </summary>
        public async void FindBooks(CancellationToken token)
        {
            BeforeStart();
            library.Clear();
            await Task.Run(() =>
            {
                foreach (var book in FindBookFiles.FindBookFilesInDirectory(Directory, AllFolders))
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }
                    BookExample be = BookExample.CreateBook(book);
                    be.PropertyChanged += IsChecked;
                    Books.Add(be);
                }
            });
            CountChecked = CountAll = Books.Count;
            ChangeState = State.FindTitle;
        }
        /// <summary>
        /// Поиск новых данных о выбранных книгах
        /// </summary>
        public async void FindBookInfo(CancellationToken token)
        {
            BeforeStart();
            await Task.Run(() =>
            {
                for (int i = 0; i < countAll; i++)
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }
                    if (library[i].IsChecked)
                    {
                        FindNewBookInfo.FindNewName(library[i]);
                        CountEnd++;
                    }
                }
            });
            ChangeState = State.Rename;
        }
        /// <summary>
        /// переименование выбранных книг
        /// </summary>
        public async void Rename(CancellationToken token)
        {
            BeforeStart();
            await Task.Run(() =>
            {
                for (int i = 0; i < countAll; i++)
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }
                    if (library[i].IsChecked)
                    {
                        RenameBookFiles.RenameFiles(library[i]);
                        CountEnd++;
                    }
                }
            });
            ChangeState = State.End;
        }
        /// <summary>
        /// Работа, выполняемая перед началом каждого шага работы с файлами
        /// </summary>
        private void BeforeStart()
        {
            ChangeState = State.Working;
            CountEnd = 0;
            CountCheckedNow = CountChecked;
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
        public bool AllFolders { get; set; }
        public string Directory
        {
            get { return directory; }
            set
            {
                directory = System.IO.Directory.Exists(value) ? value : directory;
                if (ChangeState == State.Start || ChangeState == State.End)
                    ChangeState = State.FindFiles;
                NotifyPropertyChanged();
            }
        }
        public List<BookExample> Books { get { return library; } private set { library = value; NotifyPropertyChanged(); } }
        public int CountAll { get { return countAll; } private set { countAll = value; NotifyPropertyChanged(); } }
        public int CountChecked { get { return countChecked; } private set { countChecked = value; NotifyPropertyChanged(); } }
        public int CountCheckedNow { get { return countCheckedNow; } private set { countCheckedNow = value; NotifyPropertyChanged(); } }
        public int CountEnd { get { return countEnd; } private set { countEnd = value; NotifyPropertyChanged(); } }
        public State ChangeState { get { return state; } private set { state = value; NotifyPropertyChanged(); } }
    }
}
