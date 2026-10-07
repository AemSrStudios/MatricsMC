using System;
using System.Threading.Tasks;
using System.Windows;

namespace MatricsMC
{
    public partial class MicrosoftLoginWindow : Window
    {
        public MinecraftAuthenticationResult?
            AuthenticationResult { get; private set; }

        public MicrosoftLoginWindow()
        {
            InitializeComponent();

            AuthenticationResult = null;
        }

        private async void Connect_Click(
            object sender,
            RoutedEventArgs e)
        {
            try
            {
                StatusText.Text =
                    "Preparing Microsoft login...";

                InfoText.Text =
                    "Opening the official Microsoft authentication flow...";

                await Task.Delay(300);

                MessageBox.Show(
                    "The Microsoft authentication service is not connected yet.\n\n" +
                    "No fake account was created and no credentials were stored.",
                    "MatricsMC",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                StatusText.Text =
                    "Microsoft login is not configured yet.";

                InfoText.Text =
                    "The real Microsoft â†’ Xbox Live â†’ XSTS â†’ Minecraft Services flow will be connected here.";

                AuthenticationResult = null;
            }
            catch (Exception ex)
            {
                AuthenticationResult = null;

                StatusText.Text =
                    "Authentication failed.";

                MessageBox.Show(
                    "Microsoft authentication could not be started.\n\n" +
                    ex.Message,
                    "MatricsMC",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(
            object sender,
            RoutedEventArgs e)
        {
            AuthenticationResult = null;

            DialogResult = false;

            Close();
        }
    }
}
