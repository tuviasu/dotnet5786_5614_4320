using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PL.Helpers;

namespace PL.Courier
{
    /// <summary>
    /// Interaction logic for CourierListWindow.xaml
    /// </summary>
    public partial class CourierListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        private readonly ObserverMutex _couriersMutex = new(); //stage 7
        public CourierListWindow()
        {
            InitializeComponent();
        }


        public IEnumerable<BO.CourierInList> CourierList
        {
            get { return (IEnumerable<BO.CourierInList>)GetValue(CourierListProperty); }
            set { SetValue(CourierListProperty, value); }
        }

        public static readonly DependencyProperty CourierListProperty =
            DependencyProperty.Register("CourierList", typeof(IEnumerable<BO.CourierInList>), typeof(CourierListWindow), new PropertyMetadata(null));

        public BO.DeliveryTransport TransportType { get; set; } = BO.DeliveryTransport.All;

        private void FilterChanged(object sender, SelectionChangedEventArgs e)
        {
            queryCourierList();
        }

        private void queryCourierList()
        {
            if (TransportType == BO.DeliveryTransport.All)
            {
                CourierList = s_bl?.Courier.GetCouriersList(0)!;
            }
            else
            {
                var allCouriers = s_bl?.Courier.GetCouriersList(0)!;
                CourierList = allCouriers.Where(c =>
                {
                    var courierDetails = s_bl?.Courier.GetCourierDetails(0, c.CourierID);
                    return courierDetails?.TransportType == TransportType;
                });
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            new CourierWindow().Show();
        }

        private void courierListObserver()
        {
            #region Stage 7 (for multithreading)
            if (_couriersMutex.CheckAndSetLoadInProgressOrRestartRequired())
                return;

            _ = Dispatcher.BeginInvoke(async () =>
            {
                queryCourierList();
                if (await _couriersMutex.UnsetLoadInProgressAndCheckRestartRequested())
                    courierListObserver();
            });
            #endregion Stage 7 (for multithreading)
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Courier.AddObserver(courierListObserver);
            queryCourierList();
        }

        private void Window_Closed(object sender, EventArgs e) => s_bl.Courier.RemoveObserver(courierListObserver);

        public BO.CourierInList? SelectedCourier { get; set; }

        private void Update_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedCourier != null)
                new CourierWindow(SelectedCourier.CourierID).Show();
        }


        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            var courierToDelete = btn.DataContext as BO.CourierInList;
            if (courierToDelete == null) return;

            MessageBoxResult result = MessageBox.Show(
                $"Are you sure you want to delete courier {courierToDelete.CourierID} ({courierToDelete.FullName})?",
                "Delete Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    s_bl.Courier.DeleteCourier(0, courierToDelete.CourierID);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to delete courier: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

    }
}
