using System.Windows;
using MaterialDesignThemes.Wpf;
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
            InitializeComponent();

            DataContext = new MainViewModel();
            
            var paletteHelper = new PaletteHelper();
            var theme = paletteHelper.GetTheme();

            theme.SetBaseTheme(Theme.Light);
            paletteHelper.SetTheme(theme);
            
            theme.SetBaseTheme(Theme.Dark);
            paletteHelper.SetTheme(theme);
        }
    }
}
