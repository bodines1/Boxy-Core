﻿using Boxy_Core.Mvvm;
using Boxy_Core.Settings;
using Boxy_Core.Utilities;
using System.Text;
using System.Windows;

namespace Boxy_Core
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public App()
        {
            // Initialize static resources
            DispatcherHelper.Initialize();
            DefaultSettings.Initialize();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var message = new StringBuilder();
            message.AppendLine("Oopsie! An unhandled exception occurred!");
            message.AppendLine();
            message.AppendLine(e.ExceptionObject.ToString());

            MessageBox.Show(message.ToString());

            Shutdown(1);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ImageCaching.Clear();
            base.OnExit(e);
        }
    }
}
