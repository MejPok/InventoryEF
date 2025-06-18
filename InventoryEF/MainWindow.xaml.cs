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
            vm.AddNewBook();
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
        public string Price { get { return _price; } set { _price = value; OnPropertyChanged(nameof(_price)); price = int.Parse(_price); } }

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

               context.Books.Add(book);
               BooksInventory.Add(book);
               context.SaveChanges();
            }
            OnPropertyChanged(nameof(BooksInventory));
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