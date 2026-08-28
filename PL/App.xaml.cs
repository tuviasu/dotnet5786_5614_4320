using System.Configuration;
using System.Data;
using System.Windows;
//using Admin;

namespace PL
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Open Login window as the first screen
           // new LoginWindow().Show();
        }
    }
}
