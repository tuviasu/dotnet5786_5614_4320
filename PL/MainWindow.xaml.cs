using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
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
public partial class MainWindow : Window, INotifyDataErrorInfo
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
        try
        {
            // Sync the simulation clock with real time on startup so the dashboard
            // reflects the current moment (Request B #3). If the simulator is running
            // or the call fails, fall back to the persisted clock value below.
            try { s_bl.Admin.SetClock(requesterId, DateTime.Now); }
            catch { /* keep persisted clock if sync is not possible right now */ }

            CurrentTime = s_bl.Admin.GetClock(requesterId);
            Configuration = s_bl.Admin.GetConfig(requesterId);

            LoadConfigFieldsFromConfiguration();

            s_bl.Admin.AddClockObserver(ClockObserver);
            s_bl.Admin.AddConfigObserver(ConfigObserver);

            s_bl.Order.AddObserver(OrdersObserver);
            RefreshOrdersSummary();
        }
        catch (Exception ex)
        {
            // Defensive: a failure during initial data load must not crash the dashboard.
            // Keep the window open so the user can still navigate or log out.
            MessageBox.Show(this, $"Failed to load dashboard data:\n{ex.Message}",
                "Initialization error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
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
            try
            {
                CurrentTime = s_bl.Admin.GetClock(requesterId);
            }
            catch
            {
                // A transient BL error during a clock refresh must not crash the UI thread.
            }
            finally
            {
                if (await _clockMutex.UnsetLoadInProgressAndCheckRestartRequested())
                    ClockObserver();
            }
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
            try
            {
                Configuration = s_bl.Admin.GetConfig(requesterId);
                LoadConfigFieldsFromConfiguration();
            }
            catch
            {
                // A transient BL error during a config refresh must not crash the UI thread.
            }
            finally
            {
                if (await _configMutex.UnsetLoadInProgressAndCheckRestartRequested())
                    ConfigObserver();
            }
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
            try
            {
                RefreshOrdersSummary();
            }
            catch
            {
                // A transient BL error during an orders refresh must not crash the UI thread.
            }
            finally
            {
                if (await _ordersMutex.UnsetLoadInProgressAndCheckRestartRequested())
                    OrdersObserver();
            }
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
        // Push any in-flight edit into the source so validation sees the current text.
        foreach (var tb in ConfigTextBoxes())
            tb.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();

        if (HasErrors || ConfigTextBoxes().Any(tb => Validation.GetErrors(tb).Count > 0))
        {
            MessageBox.Show(this,
                "Some configuration values are invalid. Please fix the highlighted fields before saving.",
                "Invalid input", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            var cfg = s_bl.Admin.GetConfig(requesterId);
            cfg.MaxDeliveryDistance = EditableMaxDeliveryDistance;
            cfg.MaxDeliveryTimeRange = EditableMaxDeliveryTimeRange;
            cfg.RiskRange = EditableRiskRange;
            cfg.InactivityTimeRange = EditableInactivityTimeRange;
            s_bl.Admin.SetConfig(requesterId, cfg);

            Configuration = s_bl.Admin.GetConfig(requesterId);
            LoadConfigFieldsFromConfiguration();
            MessageBox.Show(this, "Configuration updated successfully!", "Update",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Failed to update configuration:\n{ex.Message}",
                "Update", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private TextBox[] ConfigTextBoxes() => new[]
    {
        tbMaxDeliveryDistance, tbMaxDeliveryTime, tbRiskRange, tbInactivityTime
    };

    private void LoadConfigFieldsFromConfiguration()
    {
        if (Configuration == null)
            return;

        EditableMaxDeliveryDistance = Configuration.MaxDeliveryDistance;
        EditableMaxDeliveryTimeRange = Configuration.MaxDeliveryTimeRange;
        EditableRiskRange = Configuration.RiskRange;
        EditableInactivityTimeRange = Configuration.InactivityTimeRange;
    }

    // ----- Editable config fields (validated via INotifyDataErrorInfo) -----

    public double? EditableMaxDeliveryDistance
    {
        get => _editableMaxDeliveryDistance;
        set
        {
            _editableMaxDeliveryDistance = value;
            ValidateMaxDeliveryDistance(value);
        }
    }
    private double? _editableMaxDeliveryDistance;

    public TimeSpan EditableMaxDeliveryTimeRange
    {
        get => _editableMaxDeliveryTimeRange;
        set
        {
            _editableMaxDeliveryTimeRange = value;
            ValidatePositiveTimeSpan(nameof(EditableMaxDeliveryTimeRange), value, "Max delivery time");
        }
    }
    private TimeSpan _editableMaxDeliveryTimeRange;

    public TimeSpan EditableRiskRange
    {
        get => _editableRiskRange;
        set
        {
            _editableRiskRange = value;
            ValidatePositiveTimeSpan(nameof(EditableRiskRange), value, "Risk range");
        }
    }
    private TimeSpan _editableRiskRange;

    public TimeSpan EditableInactivityTimeRange
    {
        get => _editableInactivityTimeRange;
        set
        {
            _editableInactivityTimeRange = value;
            ValidatePositiveTimeSpan(nameof(EditableInactivityTimeRange), value, "Inactivity time");
        }
    }
    private TimeSpan _editableInactivityTimeRange;

    private void ValidateMaxDeliveryDistance(double? value)
    {
        const string prop = nameof(EditableMaxDeliveryDistance);
        ClearErrors(prop);
        if (!value.HasValue || value.Value <= 0)
            AddError(prop, "Enter a positive distance (km).");
    }

    private void ValidatePositiveTimeSpan(string prop, TimeSpan value, string label)
    {
        ClearErrors(prop);
        if (value <= TimeSpan.Zero)
            AddError(prop, $"{label} must be greater than 00:00:00.");
    }

    // ----- INotifyDataErrorInfo -----

    private readonly Dictionary<string, List<string>> _errors = new();

    public bool HasErrors => _errors.Count > 0;

    public IEnumerable GetErrors(string? propertyName)
    {
        if (string.IsNullOrEmpty(propertyName) || !_errors.ContainsKey(propertyName))
            return Array.Empty<string>();
        return _errors[propertyName];
    }

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    private void AddError(string prop, string error)
    {
        if (!_errors.TryGetValue(prop, out var list))
        {
            list = new List<string>();
            _errors[prop] = list;
        }
        if (!list.Contains(error))
            list.Add(error);
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(prop));
    }

    private void ClearErrors(string prop)
    {
        if (_errors.Remove(prop))
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(prop));
    }

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




