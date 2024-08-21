using Boxy_Core.Model;
using Boxy_Core.Model.ScryfallData;
using Boxy_Core.Mvvm;
using Boxy_Core.Properties;
using Boxy_Core.Settings;
using Boxy_Core.ViewModels;
using Boxy_Core.Views.Resources;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Boxy_Core.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView
    {
        public MainView()
        {
            InitializeComponent();

            Left = DefaultSettings.UserSettings.MainWindowLeft;
            Top = DefaultSettings.UserSettings.MainWindowTop;
            Width = DefaultSettings.UserSettings.MainWindowWidth;
            Height = DefaultSettings.UserSettings.MainWindowHeight;
            WindowState = DefaultSettings.UserSettings.MainWindowState;
            WindowFixer.SizeToFit(this);
            WindowFixer.MoveIntoView(this);

            switch (DefaultSettings.UserSettings.Column0Width)
            {
                case < 150:
                    DefaultSettings.UserSettings.Column0Width = 150;
                    break;
                case > 400:
                    DefaultSettings.UserSettings.Column0Width = 400;
                    break;
            }

            GridColumn0.Width = new GridLength(DefaultSettings.UserSettings.Column0Width);
        }

        /// <summary>
        /// Override of OnClosing to write the current window position/size values to the settings.
        /// </summary>
        /// <param name="e">CancelEventArgs.</param>
        protected override void OnClosing(CancelEventArgs e)
        {
            if (WindowState != WindowState.Minimized)
            {
                DefaultSettings.UserSettings.MainWindowLeft = Left;
                DefaultSettings.UserSettings.MainWindowTop = Top;
                DefaultSettings.UserSettings.MainWindowWidth = ActualWidth;
                DefaultSettings.UserSettings.MainWindowHeight = ActualHeight;
                DefaultSettings.UserSettings.MainWindowState = WindowState;
                DefaultSettings.Save();
            }

            (DataContext as ViewModelBase)?.Cleanup();
            base.OnClosing(e);
        }

        private async void ButtonBase_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = false;

            if (!(sender is Button))
            {
                return;
            }

            var random = new Random();

            while (e.LeftButton == MouseButtonState.Pressed)
            {
                await Task.Delay(50);
                var scryfallService = ((MainViewModel)DataContext).ScryfallService;
                Card? card = await scryfallService.GetRandomCard(((MainViewModel)DataContext).Reporter);

                if (card is null)
                {
                    return;
                }

                int qty = random.Next(1, 5);
                string qtyAsString = qty == 1 ? string.Empty : $"{qty} ";
                DecklistTextBox.AppendText(qtyAsString + card.Name + "\r\n");
            }
        }

        private void Thumb_OnDragCompleted(object? sender, DragCompletedEventArgs e)
        {
            if (sender is not GridSplitter)
            {
                return;
            }

            DefaultSettings.UserSettings.Column0Width = GridColumn0.Width.Value;
        }
    }
}
