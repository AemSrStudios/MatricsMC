using System.Windows;

namespace MatricsMC
{
    public partial class AddAccountWindow : Window
    {
        public string AccountName { get; private set; } = "";

        public AddAccountWindow()
        {
            InitializeComponent();

            Loaded += AddAccountWindow_Loaded;
        }

        private void AddAccountWindow_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            UsernameTextBox.Focus();
        }

        private void Add_Click(
            object sender,
            RoutedEventArgs e)
        {
            string username =
                UsernameTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show(
                    "You need to enter a username.",
                    "MatricsMC",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                UsernameTextBox.Focus();

                return;
            }

            if (username.Length > 16)
            {
                MessageBox.Show(
                    "The username cannot be longer than 16 characters.",
                    "MatricsMC",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                UsernameTextBox.Focus();

                return;
            }

            foreach (char character in username)
            {
                bool valid =
                    char.IsLetterOrDigit(character) ||
                    character == '_';

                if (!valid)
                {
                    MessageBox.Show(
                        "The username can only contain letters, numbers, and underscores.",
                        "MatricsMC",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    UsernameTextBox.Focus();

                    return;
                }
            }

            AccountName = username;

            DialogResult = true;

            Close();
        }

        private void Cancel_Click(
            object sender,
            RoutedEventArgs e)
        {
            DialogResult = false;

            Close();
        }
    }
}
