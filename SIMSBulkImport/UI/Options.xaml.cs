using System.Windows;
using System.Windows.Controls;
using MahApps.Metro;
using NLog;

namespace Matt40k.SIMSBulkImport
{
    /// <summary>
    ///     Interaction logic for Options.xaml
    /// </summary>
    public partial class Options
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// </summary>
        public Options()
        {
            logger.Log(LogLevel.Debug, "Trace:: Matt40k.SIMSBulkImport.Options()");
            InitializeComponent();
            LoadTheme();
            ReadConfig();
        }

        /// <summary>
        /// Reads the config file (.config.json) then sets the UI
        /// </summary>
        private void ReadConfig()
        {
            logger.Log(LogLevel.Debug, "Trace:: Matt40k.SIMSBulkImport.Options.ReadConfig()");
            checkBoxUpdates.IsChecked = Switcher.ConfigManClass.CheckForUpdates;
        }

        /// <summary>
        /// Saves the config to the file system
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            logger.Log(LogLevel.Debug, "Trace:: Matt40k.SIMSBulkImport.Options.buttonSave_Click()");

            bool? updateCheckBox = checkBoxUpdates.IsChecked;
            if (!updateCheckBox.HasValue)
            {
                updateCheckBox = true;
            }

            // Set the in-memory config
            Switcher.ConfigManClass.SetCheckUpdates = (bool)updateCheckBox;

            Switcher.ConfigManClass.SetTheme = _currentTheme;
            Switcher.ConfigManClass.SetAccent = _currentAccent;

            // Write the config file (.config.json) to the file-system
            Switcher.ConfigManClass.SaveConfig();

            // Return to menu UI
            Switcher.Switch(new Menu());
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            logger.Log(LogLevel.Debug, "Trace:: Matt40k.SIMSBulkImport.Options.buttonCancel_Click()");
            Switcher.Switch(new Menu());
        }

        private void LoadTheme()
        {
            logger.Log(LogLevel.Debug, "Trace:: Matt40k.SIMSBulkImport.Options.LoadTheme()");
            foreach (string theme in GetThemes)
            {
                this.comboBoxThemes.Items.Add(GetCleanTheme((theme)));
            }
            foreach (string accents in GetAccents)
            {
                this.comboBoxAccents.Items.Add(accents);
            }

            string defaultTheme = Switcher.ConfigManClass.GetTheme;
            string defaultAccent = Switcher.ConfigManClass.GetAccent;
            defaultTheme = GetCleanTheme(defaultTheme);
            this.comboBoxThemes.SelectedValue = defaultTheme;
            this.comboBoxAccents.SelectedValue = defaultAccent;
        }

        public string[] GetThemes
        {
            get
            {
                logger.Log(LogLevel.Debug, "Trace:: Matt40k.SIMSBulkImport.Options.GetThemes[]");
                return new string[]
                {
                    "BaseLight", "BaseDark"
                };
            }
        }

        public string[] GetAccents
        {
            get
            {
                logger.Log(LogLevel.Debug, "Trace:: Matt40k.SIMSBulkImport.Options.GetAccents[]");
                return new string[]
                {
                    "Red",
                    "Green",
                    "Blue",
                    "Purple",
                    "Orange",
                    "Lime",
                    "Emerald",
                    "Teal",
                    "Cyan",
                    "Cobalt",
                    "Indigo",
                    "Violet",
                    "Pink",
                    "Magenta",
                    "Crimson",
                    "Amber",
                    "Yellow",
                    "Brown",
                    "Olive",
                    "Steel",
                    "Mauve",
                    "Taupe",
                    "Sienna"
                };
            }
        }

        public string GetCleanTheme(string theme)
        {
            logger.Log(LogLevel.Debug, "Trace:: Matt40k.SIMSBulkImport.Options.GetCleanTheme(theme: " + theme + ")");
            return theme.Replace("Base", "");
        }

        public string GetThemeName(string theme)
        {
            logger.Log(LogLevel.Debug, "Trace:: Matt40k.SIMSBulkImport.Options.GetThemeName(theme: " + theme + ")");
            if (string.IsNullOrEmpty(theme))
                return "BaseLight";
            if (theme.StartsWith("Base"))
                return theme;
            return "Base" + theme;
        }

        public void SetTheme()
        {
            logger.Log(LogLevel.Debug, "Trace:: Matt40k.SIMSBulkImport.Options.SetTheme()");
            ThemeManager.ChangeAppStyle(Application.Current,
                            ThemeManager.GetAccent(_currentAccent),
                            ThemeManager.GetAppTheme(_currentTheme));
        }

        private void themeChanged(object sender, SelectionChangedEventArgs e)
        {
            logger.Log(LogLevel.Debug, "Trace:: Matt40k.SIMSBulkImport.Options.themeChanged()");

            string fieldTheme = GetThemeName((string)this.comboBoxThemes.SelectedValue);
            if (!string.IsNullOrEmpty((fieldTheme)))
                _currentTheme = fieldTheme;
            string fieldAccent = (string)this.comboBoxAccents.SelectedValue;
            if (!string.IsNullOrEmpty((fieldAccent)))
                _currentAccent = fieldAccent;
            SetTheme();
        }

        private string _currentAccent = "Blue";
        private string _currentTheme = "BaseLight";
    }
}