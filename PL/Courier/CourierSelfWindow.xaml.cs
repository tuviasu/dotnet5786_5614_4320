using BlApi;
using BO;
using System;
using System.Windows;
using PL.Helpers;

namespace PL.Courier;

public partial class CourierSelfWindow : Window
{
    private static readonly IBl s_bl = Factory.Get();

    private readonly ObserverMutex _courierMutex = new(); //stage 7

    public int CourierId { get; }

    public CourierSelfWindow(int courierId)
    {
        CourierId = courierId;
        InitializeComponent();
        QueryCourier();
    }

    public BO.Courier? CurrentCourier
    {
        get => (BO.Courier?)GetValue(CurrentCourierProperty);
        set => SetValue(CurrentCourierProperty, value);
    }

    public static readonly DependencyProperty CurrentCourierProperty =
        DependencyProperty.Register(nameof(CurrentCourier), typeof(BO.Courier), typeof(CourierSelfWindow), new PropertyMetadata(null));

    public bool CanChooseOrder
    {
        get => (bool)GetValue(CanChooseOrderProperty);
        set => SetValue(CanChooseOrderProperty, value);
    }

    public static readonly DependencyProperty CanChooseOrderProperty =
        DependencyProperty.Register(nameof(CanChooseOrder), typeof(bool), typeof(CourierSelfWindow), new PropertyMetadata(false));

    public bool CanFinishHandling
    {
        get => (bool)GetValue(CanFinishHandlingProperty);
        set => SetValue(CanFinishHandlingProperty, value);
    }

    public static readonly DependencyProperty CanFinishHandlingProperty =
        DependencyProperty.Register(nameof(CanFinishHandling), typeof(bool), typeof(CourierSelfWindow), new PropertyMetadata(false));

    private void QueryCourier()
    {
        CurrentCourier = s_bl.Courier.GetCourierDetails(CourierId, CourierId);
        RefreshButtons();
    }

    private void RefreshButtons()
    {
        bool hasOrder = CurrentCourier?.orderInProgress != null;
        bool isActive = CurrentCourier?.IsActive == true;

        CanChooseOrder = isActive && !hasOrder;
        CanFinishHandling = hasOrder;
    }

    private void CourierObserver()
    {
        #region Stage 7 (for multithreading)
        if (_courierMutex.CheckAndSetLoadInProgressOrRestartRequired())
            return;

        _ = Dispatcher.BeginInvoke(async () =>
        {
            QueryCourier();
            if (await _courierMutex.UnsetLoadInProgressAndCheckRestartRequested())
                CourierObserver();
        });
        #endregion Stage 7 (for multithreading)
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        s_bl.Courier.AddObserver(CourierId, CourierObserver);
    }

    private void Window_Closed(object? sender, EventArgs e)
    {
        s_bl.Courier.RemoveObserver(CourierId, CourierObserver);
    }

    private void btnUpdate_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (CurrentCourier == null)
                return;

            s_bl.Courier.UpdateCourier(CourierId, CurrentCourier);
            QueryCourier();
            MessageBox.Show("Updated successfully.", "Update", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Update failed", MessageBoxButton.OK, MessageBoxImage.Error);
            QueryCourier();
        }
    }

    private void btnChooseOrder_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (CurrentCourier?.IsActive != true)
                return;

            var win = new ChooseOrderWindow(CourierId);
            win.Owner = this;
            win.ShowDialog();

            QueryCourier();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Choose order", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void btnFinishHandling_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (CurrentCourier?.orderInProgress == null)
                return;

            if (cmbFinishType.SelectedItem is not BO.DeliveryDoneType doneType)
            {
                MessageBox.Show("Please select finish type.", "Finish", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int deliveryId = CurrentCourier.orderInProgress.DeliveryID;
            if (deliveryId <= 0)
            {
                MessageBox.Show("Invalid delivery ID.", "Finish", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            s_bl.Order.CompleteOrderHandling(doneType.ToString(), CourierId.ToString(), deliveryId);
            QueryCourier();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Finish failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void btnHistory_Click(object sender, RoutedEventArgs e)
    {
        var win = new CourierHistoryWindow(CourierId);
        win.Owner = this;
        win.ShowDialog();
    }

    private void btnLOGOUT_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show("Are you sure you want to logout?", "Logout", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result == MessageBoxResult.Yes)
        {
            Close();
        }
    }
}
