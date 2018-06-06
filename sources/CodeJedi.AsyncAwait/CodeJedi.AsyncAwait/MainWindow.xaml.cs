using System.Windows;

namespace CodeJedi.AsyncAwait
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Processing.Initialize(ResultTextBlock);
            DataContext = new MainViewModel();
        }
    }
}
