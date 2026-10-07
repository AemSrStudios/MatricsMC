using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace MatricsMC
{
    public partial class MainWindow : Window
    {
        private readonly string MatricsDirectory;

        private readonly string SettingsFile;

        private readonly MinecraftVersionManager VersionManager;

        private CancellationTokenSource?
            VersionLoadCancellation;

        private MatricsSettings Settings { get; set; } =
            new();

        public MainWindow()
        {
            InitializeComponent();

            MatricsDirectory =
                Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.ApplicationData),
                    ".matricsmc");

            SettingsFile =
                Path.Combine(
                    MatricsDirectory,
                    "settings.json");

            VersionManager =
                new MinecraftVersionManager();

            Directory.CreateDirectory(
                MatricsDirectory);

            AccountManager.Initialize();

            LoadSettings();

            ShowHomePage();

            UpdateHomeProfile();

            RefreshAccountLockState();

            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            await LoadLaunchersAsync();

            await LoadMinecraftVersionsAsync();
        }

        // ============================================================
        // NAVIGATION
        // ============================================================

        private void HomeNav_Click(
            object sender,
            RoutedEventArgs e)
        {
            ShowHomePage();
        }

        private void MinecraftNav_Click(
            object sender,
            RoutedEventArgs e)
        {
            MessageBox.Show(
                "Minecraft management is coming next.",
                "MatricsMC",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void InstallationsNav_Click(
            object sender,
            RoutedEventArgs e)
        {
            MessageBox.Show(
                "Installation management is coming next.",
                "MatricsMC",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void AccountsNav_Click(
            object sender,
            RoutedEventArgs e)
        {
            try
            {
                HomePage.Visibility =
                    Visibility.Collapsed;

                AccountsPage.Visibility =
                    Visibility.Visible;

                RefreshAccounts();

                RefreshAccountLockState();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "MatricsMC could not open the Accounts page.\n\n" +
                    ex.Message,
                    "MatricsMC",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                ShowHomePage();
            }
        }

        private void SettingsNav_Click(
            object sender,
            RoutedEventArgs e)
        {
            MessageBox.Show(
                "Settings are coming next.",
                "MatricsMC",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void ShowHomePage()
        {
            HomePage.Visibility =
                Visibility.Visible;

            AccountsPage.Visibility =
                Visibility.Collapsed;
        }

        // ============================================================
        // MINECRAFT VERSIONS
        // ============================================================

        private async Task LoadMinecraftVersionsAsync()
        {
            try
            {
                VersionComboBox.IsEnabled =
                    false;

                VersionComboBox.ItemsSource =
                    null;

                VersionStatusText.Text =
                    "Loading Minecraft versions...";

                VersionLoadCancellation?.Cancel();

                VersionLoadCancellation =
                    new CancellationTokenSource();

                MinecraftVersionManifest manifest =
                    await VersionManager.GetManifestAsync(
                        VersionLoadCancellation.Token);

                List<MinecraftVersion> releases =
                    manifest.Versions
                        .Where(
                            version =>
                                string.Equals(
                                    version.Type,
                                    "release",
                                    StringComparison.OrdinalIgnoreCase))
                        .ToList();

                VersionComboBox.ItemsSource =
                    releases;

                string selectedVersion =
                    Settings.SelectedVersion;

                MinecraftVersion? savedVersion =
                    releases.FirstOrDefault(
                        version =>
                            version.Id.Equals(
                                selectedVersion,
                                StringComparison.OrdinalIgnoreCase));

                if (savedVersion != null)
                {
                    VersionComboBox.SelectedItem =
                        savedVersion;
                }
                else if (releases.Count > 0)
                {
                    VersionComboBox.SelectedIndex =
                        0;

                    Settings.SelectedVersion =
                        releases[0].Id;

                    SaveSettings();
                }

                VersionStatusText.Text =
                    releases.Count > 0
                        ? $"Minecraft versions loaded • {releases.Count:N0} releases available"
                        : "No Minecraft releases were found.";

                VersionComboBox.IsEnabled =
                    releases.Count > 0;
            }
            catch (OperationCanceledException)
            {
                VersionStatusText.Text =
                    "Minecraft version loading cancelled.";
            }
            catch (Exception ex)
            {
                VersionStatusText.Text =
                    "Couldn't load Minecraft versions.";

                MessageBox.Show(
                    "MatricsMC could not load Minecraft versions.\n\n" +
                    ex.Message,
                    "Minecraft Versions",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private async void VersionComboBox_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (VersionComboBox.SelectedItem
                is not MinecraftVersion version)
            {
                return;
            }

            Settings.SelectedVersion =
                version.Id;

            SaveSettings();

            VersionStatusText.Text =
                $"Loading Minecraft {version.Id}...";

            try
            {
                MinecraftVersionDetails details =
                    await VersionManager.GetVersionDetailsAsync(
                        version);

                int libraryCount =
                    details.Libraries?.Count ?? 0;

                VersionStatusText.Text =
                    $"Minecraft {version.Id} • {libraryCount:N0} libraries";
            }
            catch
            {
                VersionStatusText.Text =
                    $"Minecraft {version.Id} selected.";
            }
        }

        // ============================================================
        // LAUNCHERS
        // ============================================================

        private async Task LoadLaunchersAsync()
        {
            try
            {
                LauncherComboBox.IsEnabled =
                    false;

                LauncherComboBox.ItemsSource =
                    null;

                List<DetectedLauncher> launchers =
                    await Task.Run(
                        () =>
                            LauncherManager.DetectLaunchers());

                LauncherComboBox.ItemsSource =
                    launchers;

                if (launchers.Count == 0)
                {
                    InstallStatusText.Text =
                        "No supported Minecraft launcher was detected.";

                    return;
                }

                DetectedLauncher? savedLauncher =
                    launchers.FirstOrDefault(
                        launcher =>
                            launcher.Type.Equals(
                                Settings.SelectedLauncherType,
                                StringComparison.OrdinalIgnoreCase));

                LauncherComboBox.SelectedItem =
                    savedLauncher ??
                    launchers[0];

                InstallStatusText.Text =
                    $"Detected {launchers.Count} supported launcher(s).";

                LauncherComboBox.IsEnabled =
                    true;
            }
            catch (Exception ex)
            {
                InstallStatusText.Text =
                    "Launcher detection failed.";

                MessageBox.Show(
                    "MatricsMC could not scan for Minecraft launchers.\n\n" +
                    ex.Message,
                    "Launcher Detection",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void LauncherComboBox_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (LauncherComboBox.SelectedItem
                is not DetectedLauncher launcher)
            {
                return;
            }

            Settings.SelectedLauncherType =
                launcher.Type;

            SaveSettings();

            InstallStatusText.Text =
                $"Launcher selected: {launcher.Name}";
        }

        // ============================================================
        // ACCOUNTS
        // ============================================================

        private void RefreshAccounts()
        {
            if (AccountsList == null)
                return;

            AccountsList.ItemsSource =
                null;

            AccountsList.ItemsSource =
                AccountManager.Data.Accounts;

            MatricsAccount? loaded =
                AccountManager.LoadedAccount;

            if (loaded != null)
            {
                AccountsList.SelectedItem =
                    loaded;

                AccountStatusText.Text =
                    $"Loaded: {loaded.Name} • {loaded.Type}";
            }
            else
            {
                AccountStatusText.Text =
                    "No account loaded";
            }

            RefreshAccountLockState();

            UpdateHomeProfile();
        }

        private void RefreshAccountLockState()
        {
            if (AddOfflineAccountButton == null)
                return;

            bool unlocked =
                AccountManager.HasOfficialAccount;

            AddOfflineAccountButton.IsEnabled =
                unlocked;

            if (unlocked)
            {
                OfflineLockText.Text =
                    "Offline profiles are unlocked because a legitimate Minecraft account is connected.";

                OfflineLockText.Foreground =
                    System.Windows.Media.Brushes.Gray;
            }
            else
            {
                OfflineLockText.Text =
                    "🔒 Offline profiles are locked until a legitimate Minecraft account is connected.";

                OfflineLockText.Foreground =
                    System.Windows.Media.Brushes.Orange;
            }
        }

        private void MicrosoftLogin_Click(
            object sender,
            RoutedEventArgs e)
        {
            try
            {
                MicrosoftLoginWindow window =
                    new MicrosoftLoginWindow
                    {
                        Owner = this
                    };

                bool? result =
                    window.ShowDialog();

                if (result != true ||
                    window.AuthenticationResult == null)
                {
                    return;
                }

                MinecraftAuthenticationResult auth =
                    window.AuthenticationResult;

                if (!auth.Success ||
                    !auth.OwnsMinecraft)
                {
                    MessageBox.Show(
                        auth.ErrorMessage,
                        "Minecraft Account",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    return;
                }

                MatricsAccount account =
                    AccountManager.AddMicrosoftAccount(
                        auth.MinecraftUsername,
                        auth.MinecraftUuid,
                        auth.MicrosoftUserId);

                AccountManager.LoadAccount(
                    account);

                RefreshAccounts();

                UpdateHomeProfile();

                RefreshAccountLockState();

                MessageBox.Show(
                    $"Minecraft account connected successfully.\n\n" +
                    $"Username: {auth.MinecraftUsername}\n" +
                    $"UUID: {auth.MinecraftUuid}\n\n" +
                    "Offline profiles are now unlocked.",
                    "MatricsMC",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "There was a problem connecting the Microsoft account.\n\n" +
                    ex.Message,
                    "MatricsMC",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void AddOfflineAccount_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (!AccountManager.HasOfficialAccount)
            {
                MessageBox.Show(
                    "Offline profiles are locked.\n\n" +
                    "Connect a legitimate Minecraft account first.",
                    "Offline Profiles Locked",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            try
            {
                AddAccountWindow window =
                    new AddAccountWindow
                    {
                        Owner = this
                    };

                bool? result =
                    window.ShowDialog();

                if (result != true)
                    return;

                string username =
                    window.AccountName.Trim();

                if (string.IsNullOrWhiteSpace(username))
                    return;

                MatricsAccount account =
                    AccountManager.AddOfflineAccount(
                        username);

                AccountManager.LoadAccount(
                    account);

                RefreshAccounts();

                UpdateHomeProfile();

                MessageBox.Show(
                    $"{account.Name} has been added and loaded.",
                    "MatricsMC",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "There was a problem adding the offline profile.\n\n" +
                    ex.Message,
                    "MatricsMC",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void LoadAccount_Click(
            object sender,
            RoutedEventArgs e)
        {
            try
            {
                if (AccountsList.SelectedItem
                    is not MatricsAccount account)
                {
                    MessageBox.Show(
                        "Select an account first.",
                        "MatricsMC",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }

                if (!AccountManager.LoadAccount(
                    account))
                {
                    MessageBox.Show(
                        "MatricsMC could not load that account.",
                        "MatricsMC",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    return;
                }

                RefreshAccounts();

                UpdateHomeProfile();

                MessageBox.Show(
                    $"{account.Name} is now loaded.",
                    "MatricsMC",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "There was a problem loading the account.\n\n" +
                    ex.Message,
                    "MatricsMC",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void AccountsList_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (AccountStatusText == null)
                return;

            if (AccountsList.SelectedItem
                is MatricsAccount account)
            {
                AccountStatusText.Text =
                    $"Selected: {account.Name} • {account.Type}";
            }
            else
            {
                AccountStatusText.Text =
                    "No account selected";
            }
        }

        private void RemoveAccount_Click(
            object sender,
            RoutedEventArgs e)
        {
            try
            {
                if (AccountsList.SelectedItem
                    is not MatricsAccount account)
                {
                    MessageBox.Show(
                        "Select an account to remove.",
                        "MatricsMC",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }

                MessageBoxResult result =
                    MessageBox.Show(
                        $"Remove \"{account.Name}\"?",
                        "Remove Account",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                if (result != MessageBoxResult.Yes)
                    return;

                AccountManager.RemoveAccount(
                    account.Id);

                RefreshAccounts();

                UpdateHomeProfile();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "There was a problem removing the account.\n\n" +
                    ex.Message,
                    "MatricsMC",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // ============================================================
        // PROFILE
        // ============================================================

        private void UpdateHomeProfile()
        {
            if (ProfileNameText == null ||
                ProfileAvatarText == null ||
                ProfileTypeText == null)
            {
                return;
            }

            MatricsAccount? account =
                AccountManager.LoadedAccount;

            if (account == null)
            {
                ProfileNameText.Text =
                    "No Account Loaded";

                ProfileAvatarText.Text =
                    "P";

                ProfileTypeText.Text =
                    "None";

                return;
            }

            ProfileNameText.Text =
                account.Name;

            ProfileTypeText.Text =
                account.Type;

            ProfileAvatarText.Text =
                string.IsNullOrWhiteSpace(
                    account.Name)
                    ? "?"
                    : account.Name
                        .Substring(0, 1)
                        .ToUpperInvariant();
        }

        private void ManageAccounts_Click(
            object sender,
            RoutedEventArgs e)
        {
            AccountsNav_Click(
                sender,
                e);
        }

        // ============================================================
        // IMPORT
        // ============================================================

        private void ImportLauncher_Click(
            object sender,
            RoutedEventArgs e)
        {
            try
            {
                LauncherImportWindow window =
                    new LauncherImportWindow
                    {
                        Owner = this
                    };

                window.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to open the launcher importer.\n\n" +
                    ex.Message,
                    "MatricsMC",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // ============================================================
        // DIRECTORY
        // ============================================================

        private void BrowseDirectory_Click(
            object sender,
            RoutedEventArgs e)
        {
            try
            {
                OpenFolderDialog dialog =
                    new OpenFolderDialog
                    {
                        Title =
                            "Select your Minecraft directory"
                    };

                if (dialog.ShowDialog() == true)
                {
                    GameDirectoryTextBox.Text =
                        dialog.FolderName;

                    Settings.MinecraftDirectory =
                        dialog.FolderName;

                    SaveSettings();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Could not open the folder browser.\n\n" +
                    ex.Message,
                    "MatricsMC",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // ============================================================
        // PLAY
        // ============================================================

        private void PlayButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            MatricsAccount? account =
                AccountManager.LoadedAccount;

            if (account == null)
            {
                MessageBox.Show(
                    "Load an account before playing.",
                    "MatricsMC",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                AccountsNav_Click(
                    sender,
                    e);

                return;
            }

            if (VersionComboBox.SelectedItem
                is not MinecraftVersion version)
            {
                MessageBox.Show(
                    "Select a Minecraft version first.",
                    "MatricsMC",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (LauncherComboBox.SelectedItem
                is not DetectedLauncher launcher)
            {
                MessageBox.Show(
                    "Select an installed launcher first.",
                    "MatricsMC",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (string.IsNullOrWhiteSpace(
                launcher.ExecutablePath) ||
                !File.Exists(
                    launcher.ExecutablePath))
            {
                MessageBox.Show(
                    $"{launcher.Name} was detected, but its executable could not be found.\n\n" +
                    "MatricsMC can detect the launcher data, but it cannot start it until the executable is located.",
                    "Launcher Executable",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            try
            {
                Process.Start(
                    new ProcessStartInfo
                    {
                        FileName =
                            launcher.ExecutablePath,

                        UseShellExecute =
                            true
                    });

                InstallStatusText.Text =
                    $"Opened {launcher.Name} for Minecraft {version.Id}.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "MatricsMC could not start the selected launcher.\n\n" +
                    ex.Message,
                    "MatricsMC",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // ============================================================
        // SETTINGS
        // ============================================================

        private void LoadSettings()
        {
            try
            {
                if (!File.Exists(
                    SettingsFile))
                {
                    Settings =
                        new MatricsSettings();

                    GameDirectoryTextBox.Text =
                        GetDefaultMinecraftDirectory();

                    return;
                }

                string json =
                    File.ReadAllText(
                        SettingsFile);

                Settings =
                    System.Text.Json.JsonSerializer
                        .Deserialize<MatricsSettings>(
                            json)
                    ?? new MatricsSettings();

                GameDirectoryTextBox.Text =
                    string.IsNullOrWhiteSpace(
                        Settings.MinecraftDirectory)
                    ? GetDefaultMinecraftDirectory()
                    : Settings.MinecraftDirectory;
            }
            catch
            {
                Settings =
                    new MatricsSettings();

                GameDirectoryTextBox.Text =
                    GetDefaultMinecraftDirectory();
            }
        }

        private void SaveSettings()
        {
            try
            {
                Directory.CreateDirectory(
                    MatricsDirectory);

                string json =
                    System.Text.Json.JsonSerializer
                        .Serialize(
                            Settings,
                            new System.Text.Json.JsonSerializerOptions
                            {
                                WriteIndented = true
                            });

                File.WriteAllText(
                    SettingsFile,
                    json);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Could not save MatricsMC settings.\n\n" +
                    ex.Message,
                    "MatricsMC",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private string GetDefaultMinecraftDirectory()
        {
            return Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData),
                ".minecraft");
        }

        public class MatricsSettings
        {
            public string MinecraftDirectory { get; set; } = "";

            public string Ram { get; set; } =
                "4 GB";

            public string SelectedAccount { get; set; } = "";

            public string SelectedVersion { get; set; } = "";

            public string SelectedLauncherType { get; set; } = "";
        }
    }
}
