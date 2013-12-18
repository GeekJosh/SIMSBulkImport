/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;

namespace Matt40k.SIMSBulkImport
{
    /// <summary>
    /// Interaction logic for Submit.xaml
    /// </summary>
    public partial class Submit
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public Submit()
        {
            InitializeComponent();
            StatID.Text = getStatId;
        }

        private string getStatId
        {
            get
            {
                Stats stats = new Stats();
                logger.Log(NLog.LogLevel.Info, "ID for Stats: " + stats.ReadID);
                return stats.ReadID;
            }
        }

        private void backClick(object sender, System.Windows.RoutedEventArgs e)
        {
            Switcher.Switch(new Logs());
        }

        private void submitClick(object sender, System.Windows.RoutedEventArgs e)
        {
            Switcher.Switch(new Menu());
        }
    }
}
