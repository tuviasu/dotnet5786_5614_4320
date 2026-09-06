using BO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private int _updatingDepth;
        private Predicate<object>? _compositeFilter;

        public OrderListWindow(string requesterId = "0")
        {
            _requesterId = requesterId;
            FilterValues = new ObservableCollection<object>();
            InitializeComponent();
        }

        public IEnumerable<BO.OrderInList> OrderList
        {
            get { return (IEnumerable<BO.OrderInList>)GetValue(OrderListProperty); }
            set { SetValue(OrderListProperty, value); }
        }

        public static readonly DependencyProperty OrderListProperty =
            DependencyProperty.Register("OrderList", typeof(IEnumerable<BO.OrderInList>), typeof(OrderListWindow), new PropertyMetadata(null));

        public BO.OrderInList? SelectedOrder { get; set; }

        public BO.OrderListFilterProperty? FilterProperty
        {
            get => (BO.OrderListFilterProperty?)GetValue(FilterPropertyProperty);
            set => SetValue(FilterPropertyProperty, value);
        }
        public static readonly DependencyProperty FilterPropertyProperty =
            DependencyProperty.Register(nameof(FilterProperty), typeof(BO.OrderListFilterProperty?), typeof(OrderListWindow),
                new PropertyMetadata(null, (d, _) => ((OrderListWindow)d).OnFilterPropertyChanged()));

        public object? FilterValue
        {
            get => GetValue(FilterValueProperty);
            set => SetValue(FilterValueProperty, value);
        }
        public static readonly DependencyProperty FilterValueProperty =
            DependencyProperty.Register(nameof(FilterValue), typeof(object), typeof(OrderListWindow),
                new PropertyMetadata(null, (d, _) => ((OrderListWindow)d).OnFilterValueChanged()));

        public BO.OrderListSortProperty? SortProperty
        {
            get => (BO.OrderListSortProperty?)GetValue(SortPropertyProperty);
            set => SetValue(SortPropertyProperty, value);
        }
        public static readonly DependencyProperty SortPropertyProperty =
            DependencyProperty.Register(nameof(SortProperty), typeof(BO.OrderListSortProperty?), typeof(OrderListWindow),
                new PropertyMetadata(null, (d, _) => ((OrderListWindow)d).OnSortPropertyChanged()));

        void OnSortPropertyChanged()
        {
            if (_updatingDepth > 0) return;
            _compositeFilter = null;
            QueryOrderList();
        }

        void OnFilterValueChanged()
        {
            // Refresh the list when FilterValue changes from any source (binding push,
            // programmatic set, etc.) — not just the ComboBox's SelectionChanged event.
            if (_updatingDepth > 0) return;
            _compositeFilter = null;
            QueryOrderList();
        }

        public ObservableCollection<object> FilterValues
        {
            get { return (ObservableCollection<object>)GetValue(FilterValuesProperty); }
            set { SetValue(FilterValuesProperty, value); }
        }

        public static readonly DependencyProperty FilterValuesProperty =
            DependencyProperty.Register("FilterValues", typeof(ObservableCollection<object>), typeof(OrderListWindow),
                new PropertyMetadata(null));

        public void ApplyCompositeFilter(BO.OrderStatus status, BO.ScheduleStatus schedule)
        {
            _updatingDepth++;
            try
            {
                _compositeFilter = obj =>
                {
                    if (obj is not BO.OrderInList o) return false;
                    return o.orderStatus == status && o.scheduleStatus == schedule;
                };
                FilterProperty = null;
                FilterValue = null;
            }
            finally { _updatingDepth--; }
            QueryOrderList();
        }

        public void ApplySummaryFilter(BO.OrderStatus? status, BO.ScheduleStatus? schedule)
        {
            _updatingDepth++;
            try
            {
                _compositeFilter = obj =>
                {
                    if (obj is not BO.OrderInList o) return false;
                    if (status.HasValue && o.orderStatus != status.Value) return false;
                    if (schedule.HasValue && o.scheduleStatus != schedule.Value) return false;
                    return true;
                };
                FilterProperty = null;
                FilterValue = null;
            }
            finally { _updatingDepth--; }
            QueryOrderList();
        }

        private void OnFilterPropertyChanged()
        {
            _updatingDepth++;
            try { BuildFilterValues(resetSelection: true); }
            finally { _updatingDepth--; }
        }

        private void ApplyViewFilterIfNeeded()
        {
            if (OrderList == null)
                return;
            var view = CollectionViewSource.GetDefaultView(OrderList);
            if (view == null)
                return;

            view.Filter = _compositeFilter;
            view.Refresh();
        }

        private void BuildFilterValues(bool resetSelection)
        {
            // Always mutate the existing ObservableCollection in-place rather than
            // reassigning the DP to a fresh array. Clearing + Add() raises
            // CollectionChanged, so the ComboBox's ItemsSource binding re-queries
            // every item and the dropdown reliably re-renders (fix for "empty Value
            // dropdown" symptom where reassigning a new array did not).
            var values = FilterValues ?? (FilterValues = new ObservableCollection<object>());
            values.Clear();

            object? first = null;
            switch (FilterProperty)
            {
                case BO.OrderListFilterProperty.Status:
                    foreach (var v in Enum.GetValues(typeof(BO.OrderStatus))) { values.Add(v); first ??= v; }
                    break;
                case BO.OrderListFilterProperty.ScheduleStatus:
                    foreach (var v in Enum.GetValues(typeof(BO.ScheduleStatus))) { values.Add(v); first ??= v; }
                    break;
                case BO.OrderListFilterProperty.OrderType:
                    foreach (var v in Enum.GetValues(typeof(BO.OrderType))) { values.Add(v); first ??= v; }
                    break;
                case BO.OrderListFilterProperty.DeliveryType:
                    foreach (var v in Enum.GetValues(typeof(BO.DeliveryType))) { values.Add(v); first ??= v; }
                    break;
            }

            if (values.Count == 0)
            {
                FilterValue = null;
                return;
            }

            if (resetSelection || FilterValue == null || !values.Contains(FilterValue))
                FilterValue = first;
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
            if (_updatingDepth > 0) return;
            _compositeFilter = null;
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

        // Re-query when the simulation clock advances so the Remaining Time countdown
        // for active orders stays in sync with the displayed system clock (Request #2).
        private void clockObserver()
        {
            #region Stage 7 (for multithreading)
            if (_ordersMutex.CheckAndSetLoadInProgressOrRestartRequired())
                return;

            _ = Dispatcher.BeginInvoke(async () =>
            {
                QueryOrderList();
                if (await _ordersMutex.UnsetLoadInProgressAndCheckRestartRequested())
                    clockObserver();
            });
            #endregion Stage 7 (for multithreading)
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Order.AddObserver(ordersObserver);
            s_bl.Admin.AddClockObserver(clockObserver);
            QueryOrderList();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Order.RemoveObserver(ordersObserver);
            s_bl.Admin.RemoveClockObserver(clockObserver);
        }
    }
}
