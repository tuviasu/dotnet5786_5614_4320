using BlApi;
using BO;
using System;
using System.ComponentModel;
using System.Windows;

namespace PL.Courier
{
    public partial class CourierWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public int CourierID { get; set; }

        // the entity id we observe (for update windows it's the courier id; for add window it's 0)
        private int CourierIdToObserve => CourierID;

        private bool _isClosing;
        private bool _isRefreshing;

        public CourierWindow(int courierId = 0)
        {
            CourierID = courierId;
            ButtonText = (CourierID == 0) ? "Add" : "Update";
            InitializeComponent();

            if (CourierID == 0)
            {
                CurrentCourier = new BO.Courier()
                {
                    DeliveredInTime = 0,
                    DeliveredNotInTime = 0,
                    StartWorkInCompany = DateTime.Now,
                    orderInProgress = null
                };
            }
            else
            {
                // manager window: requesterId is manager (0)
                CurrentCourier = s_bl.Courier.GetCourierDetails(0, CourierID);
            }
        }

        public BO.Courier? CurrentCourier
        {
            get { return (BO.Courier)GetValue(CurrentCourierProperty); }
            set { SetValue(CurrentCourierProperty, value); }
        }

        public static readonly DependencyProperty CurrentCourierProperty =
            DependencyProperty.Register("CurrentCourier", typeof(BO.Courier), typeof(CourierWindow), new PropertyMetadata(null));

        public string ButtonText
        {
            get { return (string)GetValue(ButtonTextProperty); }
            set { SetValue(ButtonTextProperty, value); }
        }

        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register("ButtonText", typeof(string), typeof(CourierWindow), new PropertyMetadata(string.Empty));

        public int? CurrentOrderID
        {
            get { return CurrentCourier?.orderInProgress?.OrderID; }
            set
            {
                if (CurrentCourier != null && value.HasValue)
                {
                    if (CurrentCourier.orderInProgress == null)
                    {
                        CurrentCourier.orderInProgress = new BO.OrderInProgress { OrderID = value.Value };
                    }
                    else
                    {
                        var existing = CurrentCourier.orderInProgress;
                        CurrentCourier.orderInProgress = new BO.OrderInProgress
                        {
                            OrderID = value.Value,
                            DeliveryID = existing.DeliveryID,
                            orderType = existing.orderType,
                            Description = existing.Description,
                            FullAddress = existing.FullAddress,
                            AirDistanceKM = existing.AirDistanceKM,
                            RealDistance = existing.RealDistance,
                            InviterFullName = existing.InviterFullName,
                            InviterPhone = existing.InviterPhone,
                            OrderOpeningTime = existing.OrderOpeningTime,
                            DeliveryStartTime = existing.DeliveryStartTime,
                            EstimatedDeliveryTime = existing.EstimatedDeliveryTime,
                            MaxDeliveryTime = existing.MaxDeliveryTime,
                            orderStatus = existing.orderStatus,
                            scheduleStatus = existing.scheduleStatus,
                            TotalTimeToCompleteAnOrder = existing.TotalTimeToCompleteAnOrder
                        };
                    }
                }
            }
        }

        private void btnAddOrUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentCourier == null)
                return;

            if (ButtonText == "Add")
            {
                try
                {
                    s_bl.Courier.AddCourier(0, CurrentCourier);

                    // refresh from BL to reflect server-side defaults (e.g., generated email)
                    CurrentCourier = s_bl.Courier.GetCourierDetails(0, CurrentCourier.CourierID);

                    MessageBox.Show("Courier added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                try
                {
                    s_bl.Courier.UpdateCourier(0, CurrentCourier);

                    // verify persistence (prevents false success messages)
                    var fresh = s_bl.Courier.GetCourierDetails(0, CourierID);
                    CurrentCourier = fresh;

                    MessageBox.Show("Courier updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RefreshCourierData()
        {
            if (CourierIdToObserve == 0)
                return;

            if (_isRefreshing)
                return;

            _isRefreshing = true;
            try
            {
                CurrentCourier = s_bl.Courier.GetCourierDetails(0, CourierIdToObserve);
            }
            finally
            {
                _isRefreshing = false;
            }
        }

        private void CourierObserver()
        {
            if (_isClosing)
                return;

            Dispatcher.BeginInvoke(new Action(RefreshCourierData));
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _isClosing = true;
            base.OnClosing(e);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CourierIdToObserve != 0)
                s_bl.Courier.AddObserver(CourierIdToObserve, CourierObserver);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (CourierIdToObserve != 0)
                s_bl.Courier.RemoveObserver(CourierIdToObserve, CourierObserver);
        }
    }
}
