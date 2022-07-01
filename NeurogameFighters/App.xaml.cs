using NeurogameFighters.Controllers;
using System.Windows;

namespace NeurogameFighters
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ControllersStore controllersStore = new ControllersStore();
            controllersStore.CurrentController = new MainMenuController(controllersStore);

            MainWindow = new MainWindow()
            {
                DataContext = new MainWindowController(controllersStore)
            };
            MainWindow.Show();

            base.OnStartup(e);
        }
    }
}
