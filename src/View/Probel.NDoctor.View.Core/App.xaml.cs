﻿/*
    This file is part of NDoctor.

    NDoctor is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    NDoctor is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with NDoctor.  If not, see <http://www.gnu.org/licenses/>.
*/
namespace Probel.NDoctor.View.Core
{
    using System;
    using System.Configuration;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Markup;
    using System.Windows.Threading;

    using log4net;
    using log4net.Config;

    using Probel.Mvvm.DataBinding;
    using Probel.Mvvm.Gui;
    using Probel.NDoctor.Domain.Components.Statistics;
    using Probel.NDoctor.Domain.DAL.Remote;
    using Probel.NDoctor.Domain.DTO.Components;
    using Probel.NDoctor.Domain.DTO.Helpers;
    using Probel.NDoctor.View.Core.Helpers;
    using Probel.NDoctor.View.Core.Properties;
    using Probel.NDoctor.View.Core.View;
    using Probel.NDoctor.View.Core.ViewModel;
    using Probel.NDoctor.View.Plugins;
    using Probel.NDoctor.View.Plugins.MenuData;
    using Probel.NDoctor.View.Toolbox.Navigation;

    using MySplashScreen = Probel.NDoctor.View.Core.View.SplashScreen;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Fields

        private static RibbonData ribbonData = new RibbonData();
        private static DateTime StartTime;

        private Arguments arguments;

        #endregion Fields

        #region Constructors

        static App()
        {
            StartTime = DateTime.Now.ToUniversalTime();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
        {
            this.MainWindow = new MainWindow();
            ViewService.Configure(e => e.RootWindow = this.MainWindow);

            PluginContext.Configuration.BenchmarkEnabled = bool.Parse(ConfigurationManager.AppSettings["BenchmarkEnabled"]);
            PluginContext.Configuration.AutomaticContextMenu = Settings.Default.AutomaticContextMenu;
            PluginContext.Configuration.ExecutionTimeThreshold = uint.Parse(ConfigurationManager.AppSettings["ExecutionTimeThreshold"]);
            PluginContext.Configuration.SearchType = Settings.Default.SearchType;
            PluginContext.Configuration.DownloadSite = ConfigurationManager.AppSettings["DownloadSite"];

            if (this.arguments.DebugTools) { this.Logger.Warn("Debug statistics are activated!"); }
            if (this.arguments.AdminTool) { this.Logger.Warn("Debug tools are activated!"); }

            var splash = new MySplashScreen();
            try { splash.ShowDialog(); }
            catch (Exception ex)
            {
                this.Logger.Error("An error occured in the spashscreen", ex);
                throw ex;
            }

            if (!splash.IsOnError)
            {
                this.Logger.Info(PluginContext.DbConfiguration.IsRemoteStatisticsEnabled
                    ? "Remote statistics are enabled"
                    : "Remote statistics are disabled");

                this.Logger.Info(PluginContext.DbConfiguration.NotifyOnNewVersion
                    ? "Notify on new version"
                    : "Do NOT notify on new version");

                this.SetConfiguration();

                this.MainWindow.Show();
                this.CheckDebugTools();
                this.CleanGui();
                this.MainWindow.As<MainWindowViewModel>().CheckNewVersion();
            }
            else { Application.Current.Shutdown(); }
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the ribbon data.
        /// </summary>
        public static RibbonData RibbonData
        {
            get { return ribbonData; }
        }

        /// <summary>
        /// Gets a value indicating whether the uncaught exception are handled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if uncaught exceptions are handled; otherwise, <c>false</c>.
        /// </value>
        private bool DoHandle
        {
            get
            {
                var value = ConfigurationManager.AppSettings["HandleUncaughtException"];
                bool result;

                if (bool.TryParse(value, out result)) return result;
                else return false;
            }
        }

        private ILog Logger
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// The fix to the wrong string.Format in the Xaml code
        /// http://stackoverflow.com/questions/2764615/wpf-stringformat-0c-showing-as-dollars
        /// </summary>
        protected override void OnStartup(StartupEventArgs e)
        {
            this.Logger = LogManager.GetLogger(typeof(LogManager));
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(
                    CultureInfo.CurrentCulture.IetfLanguageTag)));

            this.ManageArgs(e.Args);

            if (arguments.HookConsole)
            {
                //Hook the console to the application to have logging features
                AllocConsole();
            }

            //Configure Log4Net
            XmlConfigurator.Configure();

            base.OnStartup(e);
        }

        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        /// <summary>
        /// Handles the DispatcherUnhandledException event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Threading.DispatcherUnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            this.Logger.Fatal("An uncaught exception was thrown", e.Exception);

            new ErrorHandlerFactory().New(this).Fatal(e.Exception);
            e.Handled = this.DoHandle;
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            new NDoctorStatistics(PluginContext.DbConfiguration.IsRemoteStatisticsEnabled, (DateTime.Now.ToUniversalTime() - StartTime))
                .Flush();
        }

        private void CheckDebugTools()
        {
            if (this.arguments.DebugTools && this.MainWindow.DataContext is MainWindowViewModel)
            {
                var vm = this.MainWindow.DataContext as MainWindowViewModel;
                vm.ShowDebugMenu(this.arguments.AdminTool);
            }
        }

        private void CleanGui()
        {
            foreach (var item in App.RibbonData.TabDataCollection)
            {
                foreach (var subitem in item.GroupDataCollection)
                {
                    if (subitem.ButtonDataCollection.Count == 0)
                    {
                        subitem.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        private void ManageArgs(string[] args)
        {
            this.arguments.DebugTools = (from arg in args
                                         where arg.ToLower() == "-debugtools"
                                         select arg).Count() > 0;
            this.arguments.HookConsole = (from arg in args
                                          where arg.ToLower() == "-hookconsole"
                                          select arg).Count() > 0;
            this.arguments.AdminTool = (from arg in args
                                        where arg.ToLower() == "-admin"
                                        select arg).Count() > 0;
        }

        private void SetConfiguration()
        {
            DebugMode.Value = PluginContext.DbConfiguration.IsDebug;
        }

        #endregion Methods

        #region Nested Types

        private struct Arguments
        {
            #region Fields

            public bool AdminTool;
            public bool DebugTools;
            public bool HookConsole;

            #endregion Fields
        }

        #endregion Nested Types
    }
}