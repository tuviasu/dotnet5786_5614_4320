using System;
using System.Windows;
using System.Windows.Input;
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
            txtUserId.KeyDown += Field_KeyDown;
            pwdBox.KeyDown += Field_KeyDown;
            txtPasswordVisible.KeyDown += Field_KeyDown;
            txtUserId.TextChanged += (_, _) => HideError();
            pwdBox.PasswordChanged += (_, _) => HideError();
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

        private void Field_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnLogin_Click(sender, e);
                e.Handled = true;
            }
        }

        /// <summary>Show an inline validation message on the login card.</summary>
        private void ShowError(string message)
        {
            if (txtError == null || errorBar == null)
                return;

            txtError.Text = message;
            errorBar.Visibility = Visibility.Visible;
        }

        private void HideError()
        {
            if (errorBar == null)
                return;

            errorBar.Visibility = Visibility.Collapsed;
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (btnShowPassword?.IsChecked == true)
                pwdBox.Password = txtPasswordVisible.Text;

            var userIdText = txtUserId?.Text?.Trim() ?? string.Empty;
            var password = pwdBox?.Password ?? string.Empty;

            if (string.IsNullOrWhiteSpace(userIdText))
            {
                ShowError("Please enter your User ID.");
                txtUserId!.Focus();
                return;
            }

            if (!int.TryParse(userIdText, out int id) || id <= 0)
            {
                ShowError("User ID must be a positive number.");
                txtUserId!.Focus();
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                ShowError("Please enter your password.");
                pwdBox!.Focus();
                return;
            }

            try
            {
                // 1) Manager login
                if (s_bl.Admin.AuthenticateManager(id, password))
                {
                    var win = new MainWindow(id);
                    win.Show();
                    Close();
                    return;
                }

                // 2) Courier login
                _ = s_bl.Courier.AuthenticateCourier(userIdText, password);

                var courierWin = new CourierSelfWindow(id);
                courierWin.Show();
                Close();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e) => Close();
    }
}