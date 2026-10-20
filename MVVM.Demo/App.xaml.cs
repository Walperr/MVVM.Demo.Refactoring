using System.Windows;
using MVVM.Demo.Viewmodel;

namespace MVVM.Demo;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var window = new MainWindow();
        window.DataContext = new MainViewModel(window);

        MainWindow = window;
        MainWindow.Show();
    }
}
