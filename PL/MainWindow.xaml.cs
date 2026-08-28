using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
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
            var list = s_bl.Order.GetOrdersList(requesterId.ToString(), null, null, null).ToList();

            // Metric tiles: counts grouped by OrderStatus and ScheduleStatus (from BL data).
            Summary.Total = list.Count;
            Summary.NotDelivered = list.Count(o => o.orderStatus == BO.OrderStatus.NotDelivered);
            Summary.Delivered = list.Count(o => o.orderStatus == BO.OrderStatus.Delivered);
            Summary.Refused = list.Count(o => o.orderStatus == BO.OrderStatus.CustomerRefused);
            Summary.Cancelled = list.Count(o => o.orderStatus == BO.OrderStatus.Cancelled);
            Summary.InRisk = list.Count(o => o.scheduleStatus == BO.ScheduleStatus.InRisk);
            Summary.Late = list.Count(o => o.scheduleStatus == BO.ScheduleStatus.Late);

            // Manual filter count.
            SummaryFilteredCount = ApplySummaryFilter(list).Count();
        }
        catch
        {
            SummaryFilteredCount = 0;
        }
    }

    /// <summary>Bindable summary counts for the metric tiles.</summary>
    public sealed class SummaryCounts : INotifyPropertyChanged
    {
        private int _total, _notDelivered, _delivered, _refused, _cancelled, _inRisk, _late;

        public int Total { get => _total; set => Set(ref _total, value); }
        public int NotDelivered { get => _notDelivered; set => Set(ref _notDelivered, value); }
        public int Delivered { get => _delivered; set => Set(ref _delivered, value); }
        public int Refused { get => _refused; set => Set(ref _refused, value); }
        public int Cancelled { get => _cancelled; set => Set(ref _cancelled, value); }
        public int InRisk { get => _inRisk; set => Set(ref _inRisk, value); }
        public int Late { get => _late; set => Set(ref _late, value); }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public SummaryCounts Summary
    {
        get => (SummaryCounts)GetValue(SummaryProperty);
        set => SetValue(SummaryProperty, value);
    }
    public static readonly DependencyProperty SummaryProperty =
        DependencyProperty.Register(nameof(Summary), typeof(SummaryCounts), typeof(MainWindow),
            new PropertyMetadata(new SummaryCounts()));

    /// <summary>
    /// Clicking a summary metric tile opens the Order Management window pre-filtered
    /// by the status/schedule the tile represents (project spec: status metric -> filtered list).
    /// </summary>
    private void SummaryTile_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var tag = (sender as Button)?.Tag as string ?? string.Empty;
            tag = tag.Trim();

            BO.OrderStatus? status = null;
            BO.ScheduleStatus? schedule = null;

            if (Enum.TryParse<BO.OrderStatus>(tag, out var s))
                status = s;
            else if (Enum.TryParse<BO.ScheduleStatus>(tag, out var sc))
                schedule = sc;

            var win = new OrderListWindow(requesterId.ToString());
            win.ApplySummaryFilter(status, schedule);
            win.Show();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Orders", MessageBoxButton.OK, MessageBoxImage.Error);
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

            // The fields are already validated above; parse defensively and keep the
            // existing value if a parse somehow fails.
            cfg.MaxDeliveryTimeRange = TryParseStrictTimeSpan(EditableMaxDeliveryTimeRangeText, out var maxTime)
                ? maxTime : cfg.MaxDeliveryTimeRange;
            cfg.RiskRange = TryParseStrictTimeSpan(EditableRiskRangeText, out var risk)
                ? risk : cfg.RiskRange;
            cfg.InactivityTimeRange = TryParseStrictTimeSpan(EditableInactivityTimeRangeText, out var inact)
                ? inact : cfg.InactivityTimeRange;

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
        EditableMaxDeliveryTimeRangeText = FormatTimeSpan(Configuration.MaxDeliveryTimeRange);
        EditableRiskRangeText = FormatTimeSpan(Configuration.RiskRange);
        EditableInactivityTimeRangeText = FormatTimeSpan(Configuration.InactivityTimeRange);
    }

    // ----- Editable config fields (DependencyProperties, validated via INotifyDataErrorInfo) -----
    // Time fields are bound as strict "hh:mm:ss" strings so the format can be
    // validated precisely (hours 0-999, minutes/seconds 00-59) and parsed back.

    public static readonly DependencyProperty EditableMaxDeliveryDistanceProperty =
        DependencyProperty.Register(nameof(EditableMaxDeliveryDistance), typeof(double?), typeof(MainWindow),
            new PropertyMetadata(null, (d, e) => ((MainWindow)d).ValidateMaxDeliveryDistance((double?)e.NewValue)));

    public double? EditableMaxDeliveryDistance
    {
        get => (double?)GetValue(EditableMaxDeliveryDistanceProperty);
        set => SetValue(EditableMaxDeliveryDistanceProperty, value);
    }

    public static readonly DependencyProperty EditableMaxDeliveryTimeRangeTextProperty =
        DependencyProperty.Register(nameof(EditableMaxDeliveryTimeRangeText), typeof(string), typeof(MainWindow),
            new PropertyMetadata(null, (d, e) => ((MainWindow)d).ValidateTimeSpanText(
                nameof(EditableMaxDeliveryTimeRangeText), (string?)e.NewValue, "Max delivery time")));

    public string? EditableMaxDeliveryTimeRangeText
    {
        get => (string?)GetValue(EditableMaxDeliveryTimeRangeTextProperty);
        set => SetValue(EditableMaxDeliveryTimeRangeTextProperty, value);
    }

    public static readonly DependencyProperty EditableRiskRangeTextProperty =
        DependencyProperty.Register(nameof(EditableRiskRangeText), typeof(string), typeof(MainWindow),
            new PropertyMetadata(null, (d, e) => ((MainWindow)d).ValidateTimeSpanText(
                nameof(EditableRiskRangeText), (string?)e.NewValue, "Risk range")));

    public string? EditableRiskRangeText
    {
        get => (string?)GetValue(EditableRiskRangeTextProperty);
        set => SetValue(EditableRiskRangeTextProperty, value);
    }

    public static readonly DependencyProperty EditableInactivityTimeRangeTextProperty =
        DependencyProperty.Register(nameof(EditableInactivityTimeRangeText), typeof(string), typeof(MainWindow),
            new PropertyMetadata(null, (d, e) => ((MainWindow)d).ValidateTimeSpanText(
                nameof(EditableInactivityTimeRangeText), (string?)e.NewValue, "Inactivity time")));

    public string? EditableInactivityTimeRangeText
    {
        get => (string?)GetValue(EditableInactivityTimeRangeTextProperty);
        set => SetValue(EditableInactivityTimeRangeTextProperty, value);
    }

    private void ValidateMaxDeliveryDistance(double? value)
    {
        const string prop = nameof(EditableMaxDeliveryDistance);
        ClearErrors(prop);
        if (!value.HasValue || value.Value <= 0)
            AddError(prop, "Enter a positive distance (km).");
    }

    private void ValidateTimeSpanText(string prop, string? value, string label)
    {
        ClearErrors(prop);

        if (string.IsNullOrWhiteSpace(value))
        {
            AddError(prop, $"{label}: enter a time (format hh:mm:ss).");
            return;
        }

        if (!TryParseStrictTimeSpan(value, out var ts))
        {
            AddError(prop, "Invalid time format. Please enter hours:minutes:seconds (e.g., 01:30:00).");
            return;
        }

        if (ts <= TimeSpan.Zero)
        {
            AddError(prop, $"{label} must be greater than 00:00:00.");
            return;
        }
    }

    /// <summary>Strict "hh:mm:ss" parser: hours 0-999, minutes/seconds 00-59.</summary>
    private static bool TryParseStrictTimeSpan(string? text, out TimeSpan ts)
    {
        ts = TimeSpan.Zero;
        if (string.IsNullOrWhiteSpace(text))
            return false;

        var trimmed = text.Trim();
        if (!Regex.IsMatch(trimmed, @"^\d{1,3}:[0-5]\d:[0-5]\d$"))
            return false;

        var parts = trimmed.Split(':');
        int h = int.Parse(parts[0], CultureInfo.InvariantCulture);
        int m = int.Parse(parts[1], CultureInfo.InvariantCulture);
        int s = int.Parse(parts[2], CultureInfo.InvariantCulture);
        ts = new TimeSpan(h, m, s);
        return true;
    }

    /// <summary>Formats a TimeSpan as "h:mm:ss" (hours may exceed 23).</summary>
    private static string FormatTimeSpan(TimeSpan ts) =>
        $"{(int)ts.TotalHours}:{ts.Minutes:00}:{ts.Seconds:00}";

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




