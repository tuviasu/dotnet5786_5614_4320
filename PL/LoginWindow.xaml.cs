using BlApi;
using BO;
using PL.Courier;
using System;
using System.Windows;
using System.Windows.Controls;

namespace PL
{
    public partial class LoginWindow : Window
    {
        private readonly IBl bl = Factory.Get();

        public string IdNumber { get; set; } = string.Empty;
        private string password = string.Empty;

        public LoginWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            password = ((PasswordBox)sender).Password;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(IdNumber, out int id))
                {
                    MessageBox.Show(
                        "Please enter a valid ID number",
                        "Invalid Input",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                // BL authentication
                UserType userType = bl.Admin.Login(id, password);

                // Navigation by user type
                if (userType == UserType.Admin)
                {
                    new MainWindow().Show();
                }
                else
                {
                    new CourierWindow(id).Show();
                }

                Close();
            }
            catch (Exception ex)
            {
                // BO.BlInvalidInputException is not public/inaccessible; inspect runtime type instead.
                try
                {
                    string exFullName = ex.GetType()?.FullName ?? string.Empty;
                    if (exFullName == "BO.BlInvalidInputException" || exFullName.EndsWith(".BlInvalidInputException"))
                    {
                        MessageBox.Show(
                            ex.Message,
                            "Login Failed",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        return;
                    }
                }
                catch
                {
                    // ignore inspection errors and fall through to generic handler
                }

                MessageBox.Show(
                    "Unexpected error during login",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}









