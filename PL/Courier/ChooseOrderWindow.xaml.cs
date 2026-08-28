using BlApi;
using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows;
using PL.Helpers;

namespace PL.Courier;

public partial class ChooseOrderWindow : Window
{
    private static readonly IBl s_bl = Factory.Get();

    private readonly ObserverMutex _ordersMutex = new(); //stage 7

    private CancellationTokenSource? _loadCts;

    public int CourierId { get; }

    public ChooseOrderWindow(int courierId)
    {
        CourierId = courierId;
        InitializeComponent();
        Orders = new ObservableCollection<BO.OpenOrderInList>();
        _ = RefreshListAsync();
    }

    private void ordersObserver()
    {
        #region Stage 7 (for multithreading)
        if (_ordersMutex.CheckAndSetLoadInProgressOrRestartRequired())
            return;

        _ = Dispatcher.BeginInvoke(async () =>
        {
            await RefreshListAsync();
            if (await _ordersMutex.UnsetLoadInProgressAndCheckRestartRequested())
                ordersObserver();
        });
        #endregion Stage 7 (for multithreading)
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        s_bl.Order.AddObserver(ordersObserver);
        _ = RefreshListAsync();
    }

    private void Window_Closed(object? sender, EventArgs e)
    {
        s_bl.Order.RemoveObserver(ordersObserver);
        _loadCts?.Cancel();
        _loadCts?.Dispose();
        _loadCts = null;
    }

    public ObservableCollection<BO.OpenOrderInList> Orders
    {
        get => (ObservableCollection<BO.OpenOrderInList>)GetValue(OrdersProperty);
        set => SetValue(OrdersProperty, value);
    }

    public static readonly DependencyProperty OrdersProperty =
        DependencyProperty.Register(nameof(Orders), typeof(ObservableCollection<BO.OpenOrderInList>), typeof(ChooseOrderWindow), new PropertyMetadata(new ObservableCollection<BO.OpenOrderInList>()));

    public BO.OpenOrderInList? SelectedOrder
    {
        get => (BO.OpenOrderInList?)GetValue(SelectedOrderProperty);
        set => SetValue(SelectedOrderProperty, value);
    }

    public static readonly DependencyProperty SelectedOrderProperty =
        DependencyProperty.Register(nameof(SelectedOrder), typeof(BO.OpenOrderInList), typeof(ChooseOrderWindow), new PropertyMetadata(null));

    private async Task RefreshListAsync()
    {
        try
        {
            _loadCts?.Cancel();
            _loadCts?.Dispose();
            _loadCts = new CancellationTokenSource();

            Orders.Clear();
            await foreach (var item in s_bl.Order.StreamOpenOrdersForCourierAsync(
                               "0",
                               CourierId.ToString(),
                               null,
                               null,
                               _loadCts.Token))
            {
                Orders.Add(item);
            }
        }
        catch
        {
            Orders.Clear();
        }
    }

    private void btnSelect_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (SelectedOrder == null)
            {
                MessageBox.Show("Please select an order.", "Select order", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            s_bl.Order.SelectOrderForHandling("0", CourierId.ToString(), SelectedOrder.OrderID);
            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Select failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void btnRefresh_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            _ = RefreshListAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Refresh failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void btnClose_Click(object sender, RoutedEventArgs e) => Close();
}
