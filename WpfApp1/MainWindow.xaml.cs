using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public ObservableCollection<Thread> CreatedThreads { get; set; }
        public ObservableCollection<Thread> WaitingThreads { get; set; }
        public ObservableCollection<Thread> WorkingThreads { get; set; }

        private int _count { get; set; }
        Semaphore semaphore{get; set;}

        public int count
        {
            get { return _count; }
            set
            {
                _count = value;
                OnPropertyChanged();
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Combo.Items.Add(1);
            Combo.Items.Add(2);
            Combo.Items.Add(3);
            Combo.Items.Add(4);
            Combo.Items.Add(5);
            Combo.Items.Add(6);
            CreatedThreads = new ObservableCollection<Thread>();
            WaitingThreads = new ObservableCollection<Thread>();
            WorkingThreads = new ObservableCollection<Thread>();
          
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int? selectedValue = Combo.SelectedItem as int?;

                semaphore = new Semaphore(selectedValue.Value, selectedValue.Value, "Ismayilov");
                Thread thread = new Thread(() =>
            {
                var a = Thread.CurrentThread;
                System.Windows.Application.Current.Dispatcher.Invoke(() => {
                    CreatedThreads.Remove(a);
                    WaitingThreads.Add(a);
                });
                semaphore.WaitOne();

                Thread.Sleep(5000);
                System.Windows.Application.Current.Dispatcher.Invoke(() => {
                    WaitingThreads.Remove(a);
                    WorkingThreads.Add(a);
                });
                Thread.Sleep(5000);

                semaphore.Release();
                Thread.Sleep(5000);
                System.Windows.Application.Current.Dispatcher.Invoke(() => {
                    WorkingThreads.Remove(a);
                });

            });
            thread.Name = "Thread " + thread.ManagedThreadId;

            CreatedThreads.Add(thread);
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var thread = CreatedThreadss.SelectedItem as Thread;
            thread?.Start();
        }

        private void Combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            count = (int)Combo.SelectedItem;

        }
    }
}
