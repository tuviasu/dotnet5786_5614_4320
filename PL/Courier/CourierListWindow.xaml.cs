using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PL.Courier
{
    /// <summary>
    /// Interaction logic for CourierListWindow.xaml
    /// </summary>
    public partial class CourierListWindow : Window
    {
        // Access to Business Logic layer
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public CourierListWindow()
        {
            InitializeComponent();
            
        }
        // Dependency Property that holds the list of couriers to display
        public IEnumerable<BO.CourierInList> CourierList
        {
            get { return (IEnumerable<BO.CourierInList>)GetValue(CourierListProperty); }
            set { SetValue(CourierListProperty, value); }
        }

        // Registration of the Dependency Property
        public static readonly DependencyProperty CourierListProperty =
            DependencyProperty.Register(
                "CourierList",
                typeof(IEnumerable<BO.CourierInList>),
                typeof(CourierListWindow),
                new PropertyMetadata(null)
            );
        public BO.DeliveryTransport Transport { get; set; } = BO.DeliveryTransport.None;

        private void OnTransportSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CourierList = (Transport == BO.DeliveryTransport.None)
                ? s_bl.Courier.ReadAll(0)
                : s_bl.Courier.ReadAll(
                      0,
                      BO.CourierFieldFilter.Transport,
                      Transport);


        }
        private void QueryCourierList()
        {
            CourierList = (Transport == BO.DeliveryTransport.None)
                ? s_bl.Courier.ReadAll(0)
                : s_bl.Courier.ReadAll(
                      0,
                      BO.CourierFieldFilter.Transport,
                      Transport);
        }

        private void CourierListObserver()
        {
            QueryCourierList();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Courier.AddObserver(CourierListObserver);
            QueryCourierList(); // טעינה ראשונית
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Courier.RemoveObserver(CourierListObserver);
        }
        // Holds the selected courier from the list
        public BO.CourierInList? SelectedCourier { get; set; }
        /// <summary>
        /// Opens CourierWindow in Update mode
        /// </summary>
        private void dgCouriers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedCourier != null)
            {
                new CourierWindow(SelectedCourier.Id).Show();
            }
        }
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            new CourierWindow().Show();
        }
        private void btnDeleteCourier_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is BO.CourierInList courier)
            {
                var result = MessageBox.Show(
                    "Are you sure you want to delete this courier?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        s_bl.Courier.Delete(0, courier.Id);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }




    }


}