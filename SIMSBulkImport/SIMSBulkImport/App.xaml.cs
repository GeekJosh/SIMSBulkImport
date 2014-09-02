/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows;

namespace Matt40k.SIMSBulkImport
{

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Mutex myMutex;

        protected override void OnStartup(StartupEventArgs theArgs)
        {
            bool aIsNewInstance = false;
            myMutex = new Mutex(true, "Matt40k.SIMSBulkImport", out aIsNewInstance);  //Creates a mutex for the application
            if (!aIsNewInstance)
            {
                App.Current.Shutdown();  //Shutdown when a new mutex with the same name is created
            }
        }
    }
}
