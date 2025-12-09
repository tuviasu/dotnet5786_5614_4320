using System;
using System.Windows;

namespace PL.Courier
{
    public partial class CourierWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        // =======================
        // Constructor
        // =======================
        public CourierWindow(int id = 0)
        {
            InitializeComponent();

            ButtonText = id == 0 ? "Add" : "Update";

            CurrentCourier = (id != 0)
                ? s_bl.Courier.Read(0, id)
                : new BO.Courier
                {
                    Id = 0,
                    IsActive = true,
                    Transport = BO.DeliveryTransport.Bicycle,
                    StartWorkingDate = s_bl.Admin.GetClock(),
                };

            // initialize mask for the initial courier
            UpdatePasswordMask();
        }
        // =======================
        // Dependency Property: CurrentCourier
        // =======================
        public BO.Courier? CurrentCourier
        {
            get => (BO.Courier?)GetValue(CurrentCourierProperty);
            set => SetValue(CurrentCourierProperty, value);
        }

        public static readonly DependencyProperty CurrentCourierProperty =
            DependencyProperty.Register(
                "CurrentCourier",
                typeof(BO.Courier),
                typeof(CourierWindow),
                new PropertyMetadata(null, OnCurrentCourierChanged));

        private static void OnCurrentCourierChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var win = (CourierWindow)d;
            win.UpdatePasswordMask();
        }

        private void UpdatePasswordMask()
        {
            var len = !string.IsNullOrEmpty(CurrentCourier?.Password)
                ? CurrentCourier!.Password.Length
                : 6;
            PasswordMask = new string('•', len);
        }

        // =======================
        // Dependency Property: ButtonText
        // =======================
        public string ButtonText
        {
            get => (string)GetValue(ButtonTextProperty);
            set => SetValue(ButtonTextProperty, value);
        }

        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register(
                "ButtonText",
                typeof(string),
                typeof(CourierWindow),
                new PropertyMetadata(string.Empty));


        // =======================
        // Add / Update
        // =======================
       
        /// <summary>
        /// Handles Add / Update button click
        /// </summary>
        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ButtonText == "Add")
                {
                    // Create new courier
                    s_bl.Courier.Create(0, CurrentCourier!);

                    MessageBox.Show(
                        "Courier added successfully",
                        "Success",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                else
                {
                    // Update existing courier
                    s_bl.Courier.Update(0, CurrentCourier!);

                    MessageBox.Show(
                        "Courier updated successfully",
                        "Success",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }

                // Close the single-item window
                Close();
            }
            catch (Exception ex)
            {
                // Handle BL exception
                MessageBox.Show(
                    ex.Message,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// Observer method for single courier updates
        /// </summary>
        private void courierObserver()
        {
            int id = CurrentCourier!.Id;

            // Force refresh of the dependency property
            CurrentCourier = null;
            CurrentCourier = s_bl.Courier.Read(0, id);

            // Ensure mask is updated after reload
            UpdatePasswordMask();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Register observer only in Update mode
            if (CurrentCourier != null && CurrentCourier.Id != 0)
            {
                s_bl.Courier.AddObserver(CurrentCourier.Id, courierObserver);
            }
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            // Unregister observer
            if (CurrentCourier != null && CurrentCourier.Id != 0)
            {
                s_bl.Courier.RemoveObserver(CurrentCourier.Id, courierObserver);
            }
        }
        public IEnumerable<BO.DeliveryTransport> CourierTypes =>
          Enum.GetValues(typeof(BO.DeliveryTransport)).Cast<BO.DeliveryTransport>();

        // PasswordMask is now writable so the UI can bind to it and it can be updated.
        public string PasswordMask
        {
            get => (string)GetValue(PasswordMaskProperty);
            set => SetValue(PasswordMaskProperty, value);
        }

        public static readonly DependencyProperty PasswordMaskProperty =
            DependencyProperty.Register(
                "PasswordMask",
                typeof(string),
                typeof(CourierWindow),
                new PropertyMetadata(string.Empty));


    }

}

