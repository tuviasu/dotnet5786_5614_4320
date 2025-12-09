using PL.Courier;
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

namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public DateTime CurrentTime
        {
            get { return (DateTime)GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        }

        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register(
                "CurrentTime",
                typeof(DateTime),
                typeof(MainWindow)
            );

        public MainWindow()
        {
            InitializeComponent();
            Configuration = s_bl.Admin.GetConfig();
            //CurrentTime = DateTime.Now;

        }

        private void btnAddOneMinute_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.MINUTE);
        }

        private void btnAddOneMonth_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.MONTH);
        }

        private void btnAddOneYear_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.YEAR);
        }

        

        private void btnAddOneHour_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.HOUR);
        }

        private void btnAddOneDay_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.DAY);
        }

        public BO.Config Configuration
        {
            get { return (BO.Config)GetValue(ConfigurationProperty); }
            set { SetValue(ConfigurationProperty, value); }
        }

        public static readonly DependencyProperty ConfigurationProperty =
            DependencyProperty.Register(
                "Configuration",
                typeof(BO.Config),
                typeof(MainWindow)
            );

        private void UpdateConfig_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.SetConfig(Configuration);
        }
        // Observer method for system clock updates
        private void ClockObserver()
        {
            // Update the dependency property with the current clock value from BL
            CurrentTime = s_bl.Admin.GetClock();
        }

        // Observer method for configuration updates
        private void ConfigObserver()
        {
            // Update the dependency property with the current configuration from BL
            Configuration = s_bl.Admin.GetConfig();
        }
        // This method is called when the MainWindow is fully loaded
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Initialize the system clock display
            CurrentTime = s_bl.Admin.GetClock();

            // Initialize configuration values
            Configuration = s_bl.Admin.GetConfig();

            // Register clock observer
            s_bl.Admin.AddClockObserver(ClockObserver);

            // Register configuration observer
            s_bl.Admin.AddConfigObserver(ConfigObserver);
        }
        // Called when the main window is closed
        private void MainWindow_Closed(object sender, System.EventArgs e)
        {
            // Remove clock observer
            s_bl.Admin.RemoveClockObserver(ClockObserver);

            // Remove configuration observer
            s_bl.Admin.RemoveConfigObserver(ConfigObserver);
        }

        private void OpenCouriersList_Click(object sender, RoutedEventArgs e)
        {
            new CourierListWindow().Show();
        }
        private void btnResetDb_Click(object sender, RoutedEventArgs e)
        {
            // Ask the user for confirmation before resetting the database
            var result = MessageBox.Show(
                "Are you sure you want to RESET the database?\nAll data will be deleted!",
                "Confirm Reset",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            // If the user clicked "No" – do nothing
            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                // Show wait cursor while operation is running
                Mouse.OverrideCursor = Cursors.Wait;

                // Close all open windows except the main window
                CloseAllWindowsExceptMain();

                // Call BL to reset the database
                s_bl.Admin.ResetDatabase();
            }
            finally
            {
                // Restore default mouse cursor
                Mouse.OverrideCursor = null;
            }
        }
        private void btnInitializeDb_Click(object sender, RoutedEventArgs e)
        {
            // Ask the user for confirmation before initializing the database
            var result = MessageBox.Show(
                "Are you sure you want to INITIALIZE the database?",
                "Confirm Initialize",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            // If the user clicked "No" – do nothing
            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                // Show wait cursor while operation is running
                Mouse.OverrideCursor = Cursors.Wait;

                // Close all open windows except the main window
                CloseAllWindowsExceptMain();

                // Call BL to initialize the database
                s_bl.Admin.InitializeDatabase();
            }
            finally
            {
                // Restore default mouse cursor
                Mouse.OverrideCursor = null;
            }
        }

        private void CloseAllWindowsExceptMain()
        {
            // Iterate over all currently open windows
            foreach (Window window in Application.Current.Windows)
            {
                // Close every window except this one (MainWindow)
                if (window != this)
                    window.Close();
            }
        }



    }

}