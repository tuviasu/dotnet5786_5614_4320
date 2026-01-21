using System.Configuration;
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
using PL.Courier;
using PL.Order;
using PL.Helpers;

namespace PL;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    private int requesterId;

    private readonly ObserverMutex _clockMutex = new(); //stage 7
    private readonly ObserverMutex _configMutex = new(); //stage 7
    private readonly ObserverMutex _ordersMutex = new(); //stage 7

    public MainWindow(int requesterId = 0)
    {
        this.requesterId = requesterId;
        InitializeComponent();
        Loaded += MainWindow_Loaded;
        Closing += MainWindow_Closing;

        // Start with no selection so placeholders show
        SelectedSummaryDeliveryDoneType = null;
        SelectedSummaryScheduleStatus = null;
    }

    public object? SelectedSummaryDeliveryDoneType { get; set; }

    public object? SelectedSummaryScheduleStatus { get; set; }

    public int SummaryFilteredCount
    {
        get { return (int)GetValue(SummaryFilteredCountProperty); }
        set { SetValue(SummaryFilteredCountProperty, value); }
    }

    public static readonly DependencyProperty SummaryFilteredCountProperty =
        DependencyProperty.Register(nameof(SummaryFilteredCount), typeof(int), typeof(MainWindow), new PropertyMetadata(0));

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        CurrentTime = s_bl.Admin.GetClock(requesterId);
        Configuration = s_bl.Admin.GetConfig(requesterId);

        LoadConfigFieldsFromConfiguration();

        s_bl.Admin.AddClockObserver(ClockObserver);
        s_bl.Admin.AddConfigObserver(ConfigObserver);

        s_bl.Order.AddObserver(OrdersObserver);
        RefreshOrdersSummary();
    }

    private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        s_bl.Admin.RemoveClockObserver(ClockObserver);
        s_bl.Admin.RemoveConfigObserver(ConfigObserver);
        s_bl.Order.RemoveObserver(OrdersObserver);
    }

    private void ClockObserver()
    {
        #region Stage 7 (for multithreading)
        if (_clockMutex.CheckAndSetLoadInProgressOrRestartRequired())
            return;

        _ = Dispatcher.BeginInvoke(async () =>
        {
            CurrentTime = s_bl.Admin.GetClock(requesterId);

            if (await _clockMutex.UnsetLoadInProgressAndCheckRestartRequested())
                ClockObserver();
        });
        #endregion Stage 7 (for multithreading)
    }

    private void ConfigObserver()
    {
        #region Stage 7 (for multithreading)
        if (_configMutex.CheckAndSetLoadInProgressOrRestartRequired())
            return;

        _ = Dispatcher.BeginInvoke(async () =>
        {
            Configuration = s_bl.Admin.GetConfig(requesterId);
            LoadConfigFieldsFromConfiguration();

            if (await _configMutex.UnsetLoadInProgressAndCheckRestartRequested())
                ConfigObserver();
        });
        #endregion Stage 7 (for multithreading)
    }

    private void OrdersObserver()
    {
        #region Stage 7 (for multithreading)
        if (_ordersMutex.CheckAndSetLoadInProgressOrRestartRequired())
            return;

        _ = Dispatcher.BeginInvoke(async () =>
        {
            RefreshOrdersSummary();

            if (await _ordersMutex.UnsetLoadInProgressAndCheckRestartRequested())
                OrdersObserver();
        });
        #endregion Stage 7 (for multithreading)
    }

    private void RefreshOrdersSummary()
    {
        try
        {
            var list = s_bl.Order.GetOrdersList(requesterId.ToString(), null, null, null);
            SummaryFilteredCount = ApplySummaryFilter(list).Count();
        }
        catch
        {
            SummaryFilteredCount = 0;
        }
    }

    private IEnumerable<BO.OrderInList> ApplySummaryFilter(IEnumerable<BO.OrderInList> list)
    {
        var doneType = GetSummaryDeliveryDoneTypeOrNull();
        var schedule = GetSummaryScheduleStatusOrNull();

        var q = list;

        if (doneType.HasValue)
        {
            var targetStatus = doneType.Value switch
            {
                BO.DeliveryDoneType.Delivered => BO.OrderStatus.Delivered,
                BO.DeliveryDoneType.CustomerRefused => BO.OrderStatus.CustomerRefused,
                BO.DeliveryDoneType.Cancelled => BO.OrderStatus.Cancelled,
                _ => BO.OrderStatus.NotDelivered
            };

            q = q.Where(o => o.orderStatus == targetStatus);
        }

        if (schedule.HasValue)
            q = q.Where(o => o.scheduleStatus == schedule.Value);

        return q;
    }

    private void SummaryFilter_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var list = s_bl.Order.GetOrdersList(requesterId.ToString(), null, null, null).ToList();
            var filtered = ApplySummaryFilter(list).ToList();
            SummaryFilteredCount = filtered.Count;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Filter", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void SummaryFilteredCount_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var win = new OrderListWindow(requesterId.ToString());

            BO.OrderStatus? status = null;
            var doneType = GetSummaryDeliveryDoneTypeOrNull();
            if (doneType.HasValue)
            {
                status = doneType.Value switch
                {
                    BO.DeliveryDoneType.Delivered => BO.OrderStatus.Delivered,
                    BO.DeliveryDoneType.CustomerRefused => BO.OrderStatus.CustomerRefused,
                    BO.DeliveryDoneType.Cancelled => BO.OrderStatus.Cancelled,
                    _ => BO.OrderStatus.NotDelivered
                };
            }

            win.ApplySummaryFilter(status, GetSummaryScheduleStatusOrNull());
            win.Show();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Orders", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public DateTime CurrentTime
    {
        get { return (DateTime)GetValue(CurrentTimeProperty); }
        set { SetValue(CurrentTimeProperty, value); }
    }
    private static readonly DependencyProperty CurrentTimeProperty =
        DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow));

    private void btnAddOneMonth_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.ForwardClock(requesterId, BO.TimeUnit.Month);
    }

    private void btnAddOneMinute_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.ForwardClock(requesterId, BO.TimeUnit.Minutes);
    }

    private void btnAddOneHour_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.ForwardClock(requesterId, BO.TimeUnit.Hours);
    }

    private void btnAddOneDay_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.ForwardClock(requesterId, BO.TimeUnit.Days);
    }

    private void btnAddOneYear_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.ForwardClock(requesterId, BO.TimeUnit.Years);
    }

    public BO.Config Configuration
    {
        get { return (BO.Config)GetValue(ConfigurationProperty); }
        set { SetValue(ConfigurationProperty, value); }
    }

    public static readonly DependencyProperty ConfigurationProperty =
        DependencyProperty.Register("Configuration", typeof(BO.Config), typeof(MainWindow));

    private void btnUpdateConfig_Click(object sender, RoutedEventArgs e)
    {
        // Intentionally disabled for now.
        return;
    }

    private void LoadConfigFieldsFromConfiguration()
    {
        if (Configuration == null)
            return;

        EditableMaxDeliveryDistance = Configuration.MaxDeliveryDistance;
        EditableMaxDeliveryTimeRange = Configuration.MaxDeliveryTimeRange;
        EditableRiskRange = Configuration.RiskRange;
        EditableInactivityTimeRange = Configuration.InactivityTimeRange;
    }

    public double? EditableMaxDeliveryDistance { get; set; }
    public TimeSpan EditableMaxDeliveryTimeRange { get; set; }
    public TimeSpan EditableRiskRange { get; set; }
    public TimeSpan EditableInactivityTimeRange { get; set; }

    private void btnUpdateMaxDeliveryDistance_Click(object sender, RoutedEventArgs e)
    {
        var currentConfig = s_bl.Admin.GetConfig(requesterId);
        currentConfig.MaxDeliveryDistance = EditableMaxDeliveryDistance;
        s_bl.Admin.SetConfig(requesterId, currentConfig);
        Configuration = s_bl.Admin.GetConfig(requesterId);
        LoadConfigFieldsFromConfiguration();
        MessageBox.Show("Max Delivery Distance updated successfully!", "Update", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void btnUpdateMaxDeliveryTime_Click(object sender, RoutedEventArgs e)
    {
        var currentConfig = s_bl.Admin.GetConfig(requesterId);
        currentConfig.MaxDeliveryTimeRange = EditableMaxDeliveryTimeRange;
        s_bl.Admin.SetConfig(requesterId, currentConfig);
        Configuration = s_bl.Admin.GetConfig(requesterId);
        LoadConfigFieldsFromConfiguration();
        MessageBox.Show("Max Delivery Time updated successfully!", "Update", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void btnUpdateRiskRange_Click(object sender, RoutedEventArgs e)
    {
        var currentConfig = s_bl.Admin.GetConfig(requesterId);
        currentConfig.RiskRange = EditableRiskRange;
        s_bl.Admin.SetConfig(requesterId, currentConfig);
        Configuration = s_bl.Admin.GetConfig(requesterId);
        LoadConfigFieldsFromConfiguration();
        MessageBox.Show("Risk Range updated successfully!", "Update", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void btnUpdateInactivityTime_Click(object sender, RoutedEventArgs e)
    {
        var currentConfig = s_bl.Admin.GetConfig(requesterId);
        currentConfig.InactivityTimeRange = EditableInactivityTimeRange;
        s_bl.Admin.SetConfig(requesterId, currentConfig);
        Configuration = s_bl.Admin.GetConfig(requesterId);
        LoadConfigFieldsFromConfiguration();
        MessageBox.Show("Inactivity Time updated successfully!", "Update", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void btnInitDB_Click(object sender, RoutedEventArgs e)
    {
        MessageBoxResult result = MessageBox.Show("This action will delete all existing data and reinitialize the database. Are you sure you want to proceed?", "Confirm Database Initialization", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result == MessageBoxResult.Yes)
        {
            Cursor = Cursors.Wait;
            s_bl.Admin.InitializeDB(requesterId);
            Cursor = Cursors.Arrow;
            RefreshOrdersSummary();
        }
    }

    private void btnResetDB_Click(object sender, RoutedEventArgs e)
    {
        MessageBoxResult result = MessageBox.Show("This action will delete all existing data in the database. Are you sure you want to proceed?", "Confirm Database Reset", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result == MessageBoxResult.Yes)
        {
            Cursor = Cursors.Wait;
            s_bl.Admin.ResetDB(requesterId);
            Cursor = Cursors.Arrow;
            RefreshOrdersSummary();
        }
    }

    private void btnCouriersListWindow_Click(object sender, RoutedEventArgs e)
    {
        // TODO: Open list view window
        new CourierListWindow().Show();
    }

    private void btnOrdersListWindow_Click(object sender, RoutedEventArgs e)
    {
        new OrderListWindow(requesterId.ToString()).Show();
    }

    private void btnLOGOUT_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show("Are you sure you want to logout?", "Logout", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result == MessageBoxResult.Yes)
        {
            Close();
        }
    }

    private static string? SelectedContentAsString(object? selected)
    {
        return selected switch
        {
            ComboBoxItem cbi => cbi.Content?.ToString(),
            _ => selected?.ToString()
        };
    }

    private BO.DeliveryDoneType? GetSummaryDeliveryDoneTypeOrNull()
    {
        var s = SelectedContentAsString(SelectedSummaryDeliveryDoneType);
        if (string.IsNullOrWhiteSpace(s) || s == "Delivery Done Type")
            return null;

        return Enum.TryParse<BO.DeliveryDoneType>(s, out var v) ? v : null;
    }

    private BO.ScheduleStatus? GetSummaryScheduleStatusOrNull()
    {
        var s = SelectedContentAsString(SelectedSummaryScheduleStatus);
        if (string.IsNullOrWhiteSpace(s) || s == "Schedule Status")
            return null;

        return Enum.TryParse<BO.ScheduleStatus>(s, out var v) ? v : null;
    }
}




