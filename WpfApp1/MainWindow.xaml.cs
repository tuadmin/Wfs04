using System;
using System.Windows;
using WpfApp1.ViewModels.MainWindowViewModel;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            // ResourceDictionary resourceDictionary = Application.LoadComponent(new Uri("./Resources/Styles/Button/ButtonStyles.xaml", UriKind.Relative)) as ResourceDictionary;
            // Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
            InitializeComponent();

            DataContext = new MainViewModel();
        }
    }
}
