using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using PL.Helpers;

namespace PL.Order
{
    public partial class OrderListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        private readonly ObserverMutex _ordersMutex = new(); //stage 7

        private readonly string _requesterId;

        // keep last filter property so we can detect changes reliably
        private BO.OrderListFilterProperty? _lastFilterProperty;

        private Predicate<object>? _compositeFilter;

        public OrderListWindow(string requesterId = "0")
        {
            _requesterId = requesterId;
            InitializeComponent();

            // default is "no filter" and "no explicit sort"
            FilterProperty = null;
            FilterValue = null;
            SortProperty = null;

            _lastFilterProperty = FilterProperty;
            BuildFilterValues(resetSelection: true);
        }

        public IEnumerable<BO.OrderInList> OrderList
        {
            get { return (IEnumerable<BO.OrderInList>)GetValue(OrderListProperty); }
            set { SetValue(OrderListProperty, value); }
        }

        public static readonly DependencyProperty OrderListProperty =
            DependencyProperty.Register("OrderList", typeof(IEnumerable<BO.OrderInList>), typeof(OrderListWindow), new PropertyMetadata(null));

        public BO.OrderInList? SelectedOrder { get; set; }

        public BO.OrderListFilterProperty? FilterProperty { get; set; }

        public object? FilterValue { get; set; }

        public BO.OrderListSortProperty? SortProperty { get; set; }

        public IEnumerable<object> FilterValues
        {
            get { return (IEnumerable<object>)GetValue(FilterValuesProperty); }
            set { SetValue(FilterValuesProperty, value); }
        }

        public static readonly DependencyProperty FilterValuesProperty =
            DependencyProperty.Register("FilterValues", typeof(IEnumerable<object>), typeof(OrderListWindow), new PropertyMetadata(Array.Empty<object>()));

        public void ApplyCompositeFilter(BO.OrderStatus status, BO.ScheduleStatus schedule)
        {
            _compositeFilter = obj =>
            {
                if (obj is not BO.OrderInList o)
                    return false;

                return o.orderStatus == status && o.scheduleStatus == schedule;
            };

            // Clear built-in BL filters so we don't show conflicting UI state
            FilterProperty = null;
            FilterValue = null;

            QueryOrderList();
        }

        public void ApplySummaryFilter(BO.OrderStatus? status, BO.ScheduleStatus? schedule)
        {
            _compositeFilter = obj =>
            {
                if (obj is not BO.OrderInList o)
                    return false;

                if (status.HasValue && o.orderStatus != status.Value)
                    return false;

                if (schedule.HasValue && o.scheduleStatus != schedule.Value)
                    return false;

                return true;
            };

            FilterProperty = null;
            FilterValue = null;

            QueryOrderList();
        }

        private void ApplyViewFilterIfNeeded()
        {
            var view = CollectionViewSource.GetDefaultView(OrderList);
            if (view == null)
                return;

            view.Filter = _compositeFilter;
            view.Refresh();
        }

        private void BuildFilterValues(bool resetSelection)
        {
            // IMPORTANT: BL implementation currently supports only Status and OrderType filters.
            // DeliveryType appears in the enum but is not part of OrderInList and is ignored in BL.
            FilterValues = FilterProperty switch
            {
                BO.OrderListFilterProperty.Status => Enum.GetValues(typeof(BO.OrderStatus)).Cast<object>().ToArray(),
                BO.OrderListFilterProperty.OrderType => Enum.GetValues(typeof(BO.OrderType)).Cast<object>().ToArray(),
                _ => Array.Empty<object>()
            };

            if (!FilterValues.Any())
            {
                FilterValue = null;
            }
            else if (resetSelection || FilterValue == null || !FilterValues.Contains(FilterValue))
            {
                FilterValue = FilterValues.First();
            }

            QueryOrderList();
        }

        private void QueryOrderList()
        {
            try
            {
                // If filter property is set but value is missing, treat it as no-filter
                var effectiveFilterProperty = (FilterProperty.HasValue && FilterValue != null) ? FilterProperty : null;
                var effectiveFilterValue = effectiveFilterProperty.HasValue ? FilterValue : null;

                OrderList = s_bl.Order.GetOrdersList(_requesterId, effectiveFilterProperty, effectiveFilterValue, SortProperty);
                ApplyViewFilterIfNeeded();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FilterChanged(object sender, SelectionChangedEventArgs e)
        {
            // user interacted with filter UI, so remove any composite filter from the summary screen
            _compositeFilter = null;

            // Detect FilterProperty changes (works reliably regardless of ItemsSource type)
            if (!Equals(_lastFilterProperty, FilterProperty))
            {
                _lastFilterProperty = FilterProperty;
                BuildFilterValues(resetSelection: true);
                return;
            }

            QueryOrderList();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            new OrderWindow(_requesterId).Show();
        }

        private void Update_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedOrder != null)
            {
                new OrderWindow(_requesterId, SelectedOrder.OrderID).Show();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn)
                return;

            if (btn.DataContext is not BO.OrderInList order)
                return;

            if (!CanCancel(order))
                return;

            var result = MessageBox.Show($"Cancel order {order.OrderID}?", "Cancel Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                s_bl.Order.CancelOrder(_requesterId, order.OrderID);
                QueryOrderList();
                MessageBox.Show("Order cancelled successfully.", "Cancel", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to cancel order: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                QueryOrderList();
            }
        }

        private static bool CanCancel(BO.OrderInList order)
        {
            return order.orderStatus != BO.OrderStatus.Delivered
                && order.orderStatus != BO.OrderStatus.Cancelled;
        }

        private void ordersObserver()
        {
            #region Stage 7 (for multithreading)
            if (_ordersMutex.CheckAndSetLoadInProgressOrRestartRequired())
                return;

            _ = Dispatcher.BeginInvoke(async () =>
            {
                QueryOrderList();
                if (await _ordersMutex.UnsetLoadInProgressAndCheckRestartRequested())
                    ordersObserver();
            });
            #endregion Stage 7 (for multithreading)
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Order.AddObserver(ordersObserver);
            QueryOrderList();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Order.RemoveObserver(ordersObserver);
        }
    }
}
