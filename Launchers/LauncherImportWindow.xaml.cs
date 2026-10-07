using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MatricsMC
{
    public partial class LauncherImportWindow : Window
    {
        private readonly List<LauncherInfo> detectedLaunchers = new();

        private bool isImporting = false;

        // =========================================================
        // MATRICS MC DATA DIRECTORY
        // =========================================================

        private string GetMatricsDirectory()
        {
            string appData = Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData);

            return Path.Combine(
                appData,
                ".matricsmc");
        }

        private string GetImportDirectory()
        {
            return Path.Combine(
                GetMatricsDirectory(),
                "imports");
        }


        // =========================================================
        // CONSTRUCTOR
        // =========================================================

        public LauncherImportWindow()
        {
            InitializeComponent();

            // Make sure .matricsmc exists.
            CreateMatricsDirectory();

            ScanForLaunchers();
        }


        // =========================================================
        // CREATE MATRICS DIRECTORY
        // =========================================================

        private void CreateMatricsDirectory()
        {
            try
            {
                string matricsDirectory =
                    GetMatricsDirectory();

                string importsDirectory =
                    GetImportDirectory();

                Directory.CreateDirectory(
                    matricsDirectory);

                Directory.CreateDirectory(
                    importsDirectory);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "MatricsMC could not create its data folder.\n\n" +
                    ex.Message,
                    "MatricsMC",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // =========================================================
        // SCAN
        // =========================================================

        private void ScanButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (isImporting)
                return;

            ScanForLaunchers();
        }


        private void ScanForLaunchers()
        {
            LauncherList.Children.Clear();
            detectedLaunchers.Clear();

            ScanStatusText.Text =
                "Scanning for Minecraft launchers...";

            string appData =
                Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData);

            string localAppData =
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData);


            // Official Minecraft Launcher

            AddLauncher(
                "Official Minecraft Launcher",
                "Minecraft's official launcher",
                Path.Combine(
                    appData,
                    ".minecraft"),
                "Minecraft");


            // Prism Launcher

            AddLauncher(
                "Prism Launcher",
                "Multi-instance Minecraft launcher",
                Path.Combine(
                    appData,
                    "PrismLauncher"),
                "Prism");


            // MultiMC

            AddLauncher(
                "MultiMC",
                "Multi-instance Minecraft launcher",
                Path.Combine(
                    appData,
                    "MultiMC"),
                "MultiMC");


            // ATLauncher

            AddLauncher(
                "ATLauncher",
                "Minecraft launcher and modpack manager",
                Path.Combine(
                    appData,
                    "ATLauncher"),
                "ATLauncher");


            // CurseForge

            AddLauncher(
                "CurseForge",
                "Minecraft modpack launcher",
                Path.Combine(
                    localAppData,
                    "Overwolf",
                    "CurseForge"),
                "CurseForge");


            // Modrinth

            AddLauncher(
                "Modrinth App",
                "Minecraft launcher and mod manager",
                Path.Combine(
                    localAppData,
                    "ModrinthApp"),
                "Modrinth");


            // Lunar Client

            AddLauncher(
                "Lunar Client",
                "Minecraft client",
                Path.Combine(
                    localAppData,
                    "Programs",
                    "lunarclient"),
                "Lunar");


            // Badlion

            AddLauncher(
                "Badlion Client",
                "Minecraft client",
                Path.Combine(
                    localAppData,
                    "Badlion Client"),
                "Badlion");


            ScanStatusText.Text =
                $"Scan complete. Found {detectedLaunchers.Count} launcher(s).";

            UpdateSelectedCount();
        }


        // =========================================================
        // ADD LAUNCHER
        // =========================================================

        private void AddLauncher(
            string name,
            string description,
            string path,
            string type)
        {
            if (!Directory.Exists(path))
                return;

            LauncherInfo info = new LauncherInfo
            {
                Name = name,
                Description = description,
                Path = path,
                Type = type
            };

            detectedLaunchers.Add(info);

            CreateLauncherCard(info);
        }


        // =========================================================
        // LAUNCHER CARD
        // =========================================================

        private void CreateLauncherCard(
            LauncherInfo launcher)
        {
            Border card = new Border
            {
                Background =
                    new SolidColorBrush(
                        Color.FromRgb(21, 24, 29)),

                BorderBrush =
                    new SolidColorBrush(
                        Color.FromRgb(35, 39, 46)),

                BorderThickness =
                    new Thickness(1),

                CornerRadius =
                    new CornerRadius(12),

                Margin =
                    new Thickness(0, 0, 0, 10),

                Padding =
                    new Thickness(16)
            };


            Grid grid = new Grid();


            grid.ColumnDefinitions.Add(
                new ColumnDefinition
                {
                    Width = GridLength.Auto
                });


            grid.ColumnDefinitions.Add(
                new ColumnDefinition
                {
                    Width =
                        new GridLength(
                            1,
                            GridUnitType.Star)
                });


            grid.ColumnDefinitions.Add(
                new ColumnDefinition
                {
                    Width = GridLength.Auto
                });


            // =====================================================
            // CHECKBOX
            // =====================================================

            CheckBox checkBox = new CheckBox
            {
                VerticalAlignment =
                    VerticalAlignment.Center,

                Margin =
                    new Thickness(0, 0, 16, 0),

                IsChecked = true,

                Tag = launcher
            };


            checkBox.Checked += SelectionChanged;

            checkBox.Unchecked += SelectionChanged;


            Grid.SetColumn(
                checkBox,
                0);

            grid.Children.Add(
                checkBox);


            // =====================================================
            // INFORMATION
            // =====================================================

            StackPanel infoPanel =
                new StackPanel();


            TextBlock nameText =
                new TextBlock
                {
                    Text = launcher.Name,

                    FontSize = 16,

                    FontWeight =
                        FontWeights.SemiBold
                };


            TextBlock descriptionText =
                new TextBlock
                {
                    Text = launcher.Description,

                    FontSize = 12,

                    Foreground =
                        new SolidColorBrush(
                            Color.FromRgb(
                                119,
                                124,
                                133)),

                    Margin =
                        new Thickness(0, 3, 0, 0)
                };


            TextBlock pathText =
                new TextBlock
                {
                    Text = launcher.Path,

                    FontSize = 10,

                    Foreground =
                        new SolidColorBrush(
                            Color.FromRgb(
                                82,
                                87,
                                96)),

                    Margin =
                        new Thickness(0, 6, 0, 0)
                };


            infoPanel.Children.Add(
                nameText);

            infoPanel.Children.Add(
                descriptionText);

            infoPanel.Children.Add(
                pathText);


            Grid.SetColumn(
                infoPanel,
                1);

            grid.Children.Add(
                infoPanel);


            // =====================================================
            // STATUS
            // =====================================================

            TextBlock status =
                new TextBlock
                {
                    Text = "Detected",

                    Foreground =
                        new SolidColorBrush(
                            Color.FromRgb(
                                87,
                                214,
                                107)),

                    FontSize = 11,

                    VerticalAlignment =
                        VerticalAlignment.Center,

                    Margin =
                        new Thickness(
                            15,
                            0,
                            5,
                            0)
                };


            Grid.SetColumn(
                status,
                2);

            grid.Children.Add(
                status);


            card.Child = grid;

            LauncherList.Children.Add(
                card);
        }


        // =========================================================
        // SELECTION
        // =========================================================

        private void SelectionChanged(
            object sender,
            RoutedEventArgs e)
        {
            UpdateSelectedCount();
        }


        private void UpdateSelectedCount()
        {
            int count = 0;


            foreach (
                Border border
                in LauncherList.Children)
            {
                if (border.Child is Grid grid)
                {
                    foreach (
                        UIElement element
                        in grid.Children)
                    {
                        if (
                            element is CheckBox checkBox &&
                            checkBox.IsChecked == true)
                        {
                            count++;
                        }
                    }
                }
            }


            SelectedCountText.Text =
                count.ToString();
        }


        // =========================================================
        // GET SELECTED LAUNCHERS
        // =========================================================

        private List<LauncherInfo>
            GetSelectedLaunchers()
        {
            List<LauncherInfo> selected =
                new();


            foreach (
                Border border
                in LauncherList.Children)
            {
                if (border.Child is Grid grid)
                {
                    foreach (
                        UIElement element
                        in grid.Children)
                    {
                        if (
                            element is CheckBox checkBox &&
                            checkBox.IsChecked == true &&
                            checkBox.Tag is LauncherInfo launcher)
                        {
                            selected.Add(
                                launcher);
                        }
                    }
                }
            }


            return selected;
        }


        // =========================================================
        // IMPORT BUTTON
        // =========================================================

        private async void ImportButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (isImporting)
                return;


            List<LauncherInfo> selected =
                GetSelectedLaunchers();


            if (selected.Count == 0)
            {
                MessageBox.Show(
                    "Select at least one launcher to import.",
                    "MatricsMC",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            // Make sure our folders exist.

            CreateMatricsDirectory();


            // Start import UI.

            isImporting = true;

            ImportButton.IsEnabled = false;
            CancelButton.IsEnabled = false;


            LauncherList.Visibility =
                Visibility.Collapsed;

            ImportProgressPanel.Visibility =
                Visibility.Visible;


            ImportProgressBar.Value = 0;

            ImportPercentText.Text = "0%";

            ImportCurrentItemText.Text =
                "Preparing import...";

            ImportDetailsText.Text =
                "Creating MatricsMC storage...";


            try
            {
                await ImportLaunchersAsync(
                    selected);


                ImportProgressBar.Value =
                    100;

                ImportPercentText.Text =
                    "100%";

                ImportCurrentItemText.Text =
                    "Import complete!";

                ImportDetailsText.Text =
                    "Your original launcher data was not moved or deleted.";


                await Task.Delay(600);


                MessageBox.Show(
                    "Launcher data has been imported into MatricsMC.\n\n" +
                    "Stored in:\n" +
                    GetImportDirectory(),
                    "MatricsMC Import Complete",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);


                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "The import encountered an error.\n\n" +
                    ex.Message,
                    "MatricsMC Import",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);


                isImporting = false;

                ImportButton.IsEnabled = true;
                CancelButton.IsEnabled = true;

                LauncherList.Visibility =
                    Visibility.Visible;

                ImportProgressPanel.Visibility =
                    Visibility.Collapsed;
            }
        }


        // =========================================================
        // IMPORT LAUNCHERS
        // =========================================================

        private async Task ImportLaunchersAsync(
            List<LauncherInfo> launchers)
        {
            int total =
                launchers.Count;

            int current =
                0;


            foreach (
                LauncherInfo launcher
                in launchers)
            {
                current++;


                ImportCurrentItemText.Text =
                    $"Importing {launcher.Name}...";


                ImportDetailsText.Text =
                    $"Copying launcher data {current} of {total}";


                await CopyLauncherDataAsync(
                    launcher,
                    current,
                    total);


                int percentage =
                    (int)(
                        ((double)current /
                         total) *
                        100);


                ImportProgressBar.Value =
                    percentage;

                ImportPercentText.Text =
                    $"{percentage}%";


                await Task.Delay(150);
            }
        }


        // =========================================================
        // COPY LAUNCHER DATA
        // =========================================================

        private async Task
            CopyLauncherDataAsync(
                LauncherInfo launcher,
                int launcherNumber,
                int launcherTotal)
        {
            string safeName =
                MakeSafeFolderName(
                    launcher.Name);


            string destination =
                Path.Combine(
                    GetImportDirectory(),
                    safeName);


            // If this launcher was imported before,
            // create a unique folder instead of overwriting it.

            destination =
                GetUniqueDirectory(
                    destination);


            Directory.CreateDirectory(
                destination);


            ImportCurrentItemText.Text =
                $"Copying {launcher.Name}";


            ImportDetailsText.Text =
                "Reading launcher files...";


            await Task.Run(() =>
            {
                CopyDirectoryContents(
                    launcher.Path,
                    destination);
            });


            ImportDetailsText.Text =
                $"Finished copying {launcher.Name}";
        }


        // =========================================================
        // COPY DIRECTORY CONTENTS
        // =========================================================

        private void CopyDirectoryContents(
            string source,
            string destination)
        {
            if (!Directory.Exists(source))
                return;


            Directory.CreateDirectory(
                destination);


            // Copy files.

            foreach (
                string file
                in Directory.GetFiles(
                    source))
            {
                try
                {
                    string fileName =
                        Path.GetFileName(file);

                    string destinationFile =
                        Path.Combine(
                            destination,
                            fileName);


                    File.Copy(
                        file,
                        destinationFile,
                        false);
                }
                catch
                {
                    // Skip files that are locked
                    // or inaccessible.
                }
            }


            // Copy subdirectories.

            foreach (
                string directory
                in Directory.GetDirectories(
                    source))
            {
                try
                {
                    string directoryName =
                        Path.GetFileName(directory);

                    string destinationDirectory =
                        Path.Combine(
                            destination,
                            directoryName);


                    CopyDirectoryContents(
                        directory,
                        destinationDirectory);
                }
                catch
                {
                    // Skip inaccessible folders.
                }
            }
        }


        // =========================================================
        // SAFE DIRECTORY NAME
        // =========================================================

        private string MakeSafeFolderName(
            string name)
        {
            foreach (
                char invalid
                in Path.GetInvalidFileNameChars())
            {
                name =
                    name.Replace(
                        invalid,
                        '_');
            }


            return name.Trim();
        }


        // =========================================================
        // UNIQUE IMPORT DIRECTORY
        // =========================================================

        private string GetUniqueDirectory(
            string directory)
        {
            if (!Directory.Exists(directory))
                return directory;


            int number = 2;


            while (true)
            {
                string newDirectory =
                    directory +
                    "_" +
                    number;


                if (!Directory.Exists(
                        newDirectory))
                {
                    return newDirectory;
                }


                number++;
            }
        }


        // =========================================================
        // CANCEL
        // =========================================================

        private void CancelButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (isImporting)
                return;


            Close();
        }
    }


    // =============================================================
    // LAUNCHER INFO
    // =============================================================

    public class LauncherInfo
    {
        public string Name
        {
            get;
            set;
        } = "";


        public string Description
        {
            get;
            set;
        } = "";


        public string Path
        {
            get;
            set;
        } = "";


        public string Type
        {
            get;
            set;
        } = "";
    }
}
