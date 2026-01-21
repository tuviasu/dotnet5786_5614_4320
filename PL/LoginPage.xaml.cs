using System;
using System.Windows;
using PL.Courier;

namespace PL
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Window
    {
        private static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public LoginPage()
        {
            InitializeComponent();
        }

        private void ShowPassword_Checked(object sender, RoutedEventArgs e)
        {
            if (txtPasswordVisible == null || pwdBox == null)
                return;

            txtPasswordVisible.Text = pwdBox.Password;
            pwdBox.Visibility = Visibility.Collapsed;
            txtPasswordVisible.Visibility = Visibility.Visible;
            txtPasswordVisible.Focus();
            txtPasswordVisible.CaretIndex = txtPasswordVisible.Text.Length;
        }

        private void ShowPassword_Unchecked(object sender, RoutedEventArgs e)
        {
            if (txtPasswordVisible == null || pwdBox == null)
                return;

            pwdBox.Password = txtPasswordVisible.Text;
            txtPasswordVisible.Visibility = Visibility.Collapsed;
            pwdBox.Visibility = Visibility.Visible;
            pwdBox.Focus();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (btnShowPassword?.IsChecked == true)
                pwdBox.Password = txtPasswordVisible.Text;

            var userIdText = txtUserId?.Text?.Trim() ?? string.Empty;
            var password = pwdBox?.Password ?? string.Empty;

            if (!int.TryParse(userIdText, out int id) || id <= 0)
            {
                MessageBox.Show("Invalid ID", "Login", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // 1) Manager login
                if (s_bl.Admin.AuthenticateManager(id, password))
                {
                    var win = new MainWindow(id);
                    win.Show();
                    return;
                }

                // 2) Courier login
                _ = s_bl.Courier.AuthenticateCourier(userIdText, password);

                var courierWin = new CourierSelfWindow(id);
                courierWin.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Login failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e) => Close();
    }
}
