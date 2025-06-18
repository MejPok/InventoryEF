using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace InventoryEF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowVM vm;
        public MainWindow()
        {
            InitializeComponent();
            vm = new MainWindowVM();
            this.DataContext = vm;

            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await vm.LoadInventoryFromDbAsync();
        }

        private void AddNewBook(object sender, RoutedEventArgs e)
        {
            vm.AddNewBook();
        }
        private void DeleteBook(object sender, RoutedEventArgs e)
        {
            
            vm.DeleteBook((Book)Inventory.SelectedItem);
        }

        bool buttonsVisible = true;
        private void EditBook(object sender, RoutedEventArgs e) 
        {
            buttonsVisible = !buttonsVisible;
            if (buttonsVisible)
            {
                AddBookButton.Visibility = Visibility.Visible;
                DeleteBookButton.Visibility = Visibility.Visible;

                SaveButton.Visibility = Visibility.Hidden;
            }
            else
            {
                AddBookButton.Visibility = Visibility.Hidden;
                DeleteBookButton.Visibility = Visibility.Hidden;

                SaveButton.Visibility = Visibility.Visible;
            }

            if (!buttonsVisible)
            {
                vm.StartEditing((Book)Inventory.SelectedItem);
            } else
            {
                vm.StopEditing();
            }
            

        }
        private void SaveBook(object sender, RoutedEventArgs e)
        {
            buttonsVisible = !buttonsVisible;
            if (buttonsVisible)
            {
                AddBookButton.Visibility = Visibility.Visible;
                DeleteBookButton.Visibility = Visibility.Visible;

                SaveButton.Visibility = Visibility.Hidden;
            }
            else
            {
                AddBookButton.Visibility = Visibility.Hidden;
                DeleteBookButton.Visibility = Visibility.Hidden;

                SaveButton.Visibility = Visibility.Visible;
            }

            vm.SaveEditChanges();
            

        }

    }


    class MainWindowVM : INotifyPropertyChanged
    {
        public ObservableCollection<Book> BooksInventory { get; set; }

        string title;
        public string Title { get { return title; } set { title = value; OnPropertyChanged(nameof(Title)); } }

        string author;

        public string Author { get { return author; } set { author = value; OnPropertyChanged(nameof(Author)); } }

        string _price;
        public string Price { get { return _price; } set 
            { 
                _price = value; 
                OnPropertyChanged(nameof(_price)); 
                if(int.TryParse(_price, out int priceParsed))
                {
                    price = priceParsed; 
                }
                
                
            } 
        }

        int price;

        public async Task LoadInventoryFromDbAsync()
        {
            using(var context = new LibraryContext())
            {
                BooksInventory = new ObservableCollection<Book>(await context.Books.ToListAsync());
            }
            OnPropertyChanged(nameof(BooksInventory));
        }


        public void AddNewBook()
        {

            using (var context = new LibraryContext())
            {
               var book = new Book() { Author = author, Price = price, Title = title };
                if (string.IsNullOrEmpty(book.Author) || book.Price == 0 || string.IsNullOrEmpty(book.Title) ) {
                    return;
                }
               context.Books.Add(book);
               BooksInventory.Add(book);
               context.SaveChanges();
            }
            OnPropertyChanged(nameof(BooksInventory));
        }

        public void DeleteBook(Book book)
        {


            using (var context = new LibraryContext())
            {
                if (BooksInventory.Contains(book))
                {
                    BooksInventory.Remove(book);
                    context.Books.Remove(book);
                }
                context.SaveChanges();
            }

            OnPropertyChanged(nameof(BooksInventory));
        }

        (string, string, string) SavedInput;
        Book editedBook;
        public void StartEditing(Book book)
        {

            SavedInput = (Title, Author, Price);

            if (BooksInventory.Contains(book))
            {
                Title = book.Title;
                Author = book.Author;
                Price = book.Price + "";
            }

            editedBook = book;
        }

        public void StopEditing()
        {
            Title = SavedInput.Item1;
            Author = SavedInput.Item2;
            Price = SavedInput.Item3;

            editedBook = null;
        }

        public void SaveEditChanges()
        {
            if (editedBook == null) { return; }

            if (BooksInventory.Contains(editedBook))
            {
                using (var context = new LibraryContext()) 
                {
                    Book foundBook = context.Books.ToList().Find(x => x.Id == editedBook.Id);

                    if (foundBook != null) {

                        foundBook.Title = Title;
                        foundBook.Author = Author;
                        foundBook.Price = double.Parse(Price);

                        
                        context.SaveChanges();
                    }
                }

                LoadInventoryFromDbAsync();
            }

            StopEditing();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public double Price {  get; set; }
    }

    

}