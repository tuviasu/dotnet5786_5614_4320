using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using PL;
using PL.Courier;
using PL.Order;

internal static class Program
{
    // Headless UI verification harness: instantiates every window on the STA thread,
    // pumps the Dispatcher so Loaded + observer callbacks fire, then closes the window.
    // Exits non-zero if ANY window throws during init/bind/close.

    private const int ADMIN = 111111111;
    private const int COURIER = 300000000; // exists in xml/couriers.xml
    private const int ORDER = 1000;         // exists in xml/orders.xml

    [STAThread]
    private static int Main()
    {
        var failures = new List<string>();
        var dispatcherErrors = new List<string>();

        Dispatcher.CurrentDispatcher.UnhandledException += (s, e) =>
        {
            dispatcherErrors.Add(e.Exception.GetType().Name + ": " + e.Exception.Message);
            e.Handled = true; // keep the harness alive
        };
        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
        {
            if (e.ExceptionObject is Exception ex)
                dispatcherErrors.Add("AppDomain: " + ex.GetType().Name + ": " + ex.Message);
        };
        TaskScheduler.UnobservedTaskException += (s, e) =>
        {
            dispatcherErrors.Add("UnobservedTask: " + e.Exception.GetType().Name + ": " + e.Exception.Message);
            e.SetObserved();
        };

        // Use PL's own Application so the shared resources defined in PL/App.xaml
        // (converters, enum collections, and the modern theme styles: AppTextBox,
        // AppPrimaryButton, AppCard, ...) are loaded and available to every window.
        // InitializeComponent loads those resources; we deliberately do NOT call Run(),
        // so no StartupUri navigation happens and we keep manual control of the pump.
        var app = new PL.App();
        app.InitializeComponent();

        var specs = new (string Name, Func<Window> Factory)[]
        {
            ("LoginPage",            () => new LoginPage()),
            ("MainWindow",            () => new MainWindow(ADMIN)),
            ("OrderListWindow",       () => new OrderListWindow(ADMIN.ToString())),
            ("OrderWindow",           () => new OrderWindow(ADMIN.ToString(), ORDER)),
            ("CourierListWindow",     () => new CourierListWindow()),
            ("CourierSelfWindow",     () => new CourierSelfWindow(COURIER)),
            ("CourierWindow",         () => new CourierWindow(COURIER)),
            ("ChooseOrderWindow",     () => new ChooseOrderWindow(COURIER)),
            ("CourierHistoryWindow",  () => new CourierHistoryWindow(COURIER)),
        };

        int pass = 0;
        foreach (var (name, factory) in specs)
        {
            dispatcherErrors.Clear();
            Window? win = null;
            try
            {
                win = factory();
                win.Show();
                PumpDispatcher(600);
                win.Close();
                PumpDispatcher(200);

                if (dispatcherErrors.Count > 0)
                {
                    failures.Add($"{name}: dispatcher exception(s): {string.Join(" | ", dispatcherErrors)}");
                    Console.WriteLine($"  FAIL {name} -> dispatcher exception(s): {string.Join(" | ", dispatcherErrors)}");
                }
                else
                {
                    Console.WriteLine($"  PASS {name}");
                    pass++;
                }
            }
            catch (Exception ex)
            {
                failures.Add($"{name}: {ex.GetType().Name}: {ex.Message}");
                Console.WriteLine($"  FAIL {name} -> {ex.GetType().Name}: {ex.Message}");
                try { win?.Close(); } catch { }
            }
        }

        Console.WriteLine();
        Console.WriteLine($"RESULT: {pass}/{specs.Length} windows loaded cleanly.");
        if (failures.Count > 0)
        {
            Console.WriteLine("FAILURES:");
            foreach (var f in failures) Console.WriteLine("  - " + f);
            return 1;
        }
        Console.WriteLine("ALL WINDOWS OK — 0 exceptions.");
        return 0;
    }

    private static void PumpDispatcher(int milliseconds)
    {
        var deadline = Environment.TickCount + milliseconds;
        while (Environment.TickCount < deadline)
        {
            var frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => frame.Continue = false));
            Dispatcher.PushFrame(frame);
            Thread.Sleep(15);
        }
    }
}