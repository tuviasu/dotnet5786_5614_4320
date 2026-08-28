using BlApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace PL.Courier;

public partial class CourierHistoryWindow : Window
{
    private static readonly IBl s_bl = Factory.Get();

    public int CourierId { get; }

    public CourierHistoryWindow(int courierId)
    {
        CourierId = courierId;
        InitializeComponent();
        RefreshList();
    }

    public IEnumerable<BO.ClosedDeliveryInList> Deliveries
    {
        get => (IEnumerable<BO.ClosedDeliveryInList>)GetValue(DeliveriesProperty);
        set => SetValue(DeliveriesProperty, value);
    }

    public static readonly DependencyProperty DeliveriesProperty =
        DependencyProperty.Register(nameof(Deliveries), typeof(IEnumerable<BO.ClosedDeliveryInList>), typeof(CourierHistoryWindow), new PropertyMetadata(Enumerable.Empty<BO.ClosedDeliveryInList>()));

    private void RefreshList()
    {
        Deliveries = s_bl.Order.GetClosedOrdersByCourier("0", CourierId.ToString(), null, null).ToList();
    }

    private void btnRefresh_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            RefreshList();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Refresh failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void btnClose_Click(object sender, RoutedEventArgs e) => Close();
}
