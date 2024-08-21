using Boxy_Core.Model.ScryfallData;
using Boxy_Core.Mvvm;
using Boxy_Core.Settings;
using Boxy_Core.Utilities;
using PdfSharp;
using PdfSharp.Drawing;

namespace Boxy_Core.ViewModels.Dialogs
{
    /// <summary>
    /// View model for interacting with a message dialog window.
    /// </summary>
    public class SettingsDialogViewModel : DialogViewModelBase
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="SettingsDialogViewModel"/>.
        /// </summary>
        public SettingsDialogViewModel()
        {
            FormatOptions = Enum.GetValues(typeof(FormatTypes)).Cast<FormatTypes>().ToList();
            PageSizeOptions = Enum.GetValues(typeof(PageSize)).Cast<PageSize>().ToList();
            PageSizeOptions.Remove(PageSize.Undefined);
            ColorOptions = Enum.GetValues(typeof(XKnownColor)).Cast<XKnownColor>().ToList();
            LineSizeOptions = Enum.GetValues(typeof(CutLineSizes)).Cast<CutLineSizes>().ToList();
            DefaultSettings.UserSettings.PropertyChanged += UserSettings_PropertyChanged;
        }

        private void UserSettings_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var temp = new CardPdfBuilder(DefaultSettings.UserSettings.PdfPageSize, DefaultSettings.UserSettings.PdfScalingPercent, DefaultSettings.UserSettings.PdfHasCutLines, DefaultSettings.UserSettings.CutLineSize, DefaultSettings.UserSettings.CutLineColor);
            CardsPerPage = temp.ExampleImageDrawer.ImagesPerPage;
        }

        #endregion Constructors

        #region Fields

        private int _cardsPerPage;

        #endregion Fields

        #region Properties

        /// <summary>
        /// List to populate the options for user to select from.
        /// </summary>
        public List<PageSize> PageSizeOptions { get; }

        /// <summary>
        /// List to populate the options for user to select from.
        /// </summary>
        public List<XKnownColor> ColorOptions { get; }

        /// <summary>
        /// List to populate the options for user to select from.
        /// </summary>
        public List<CutLineSizes> LineSizeOptions { get; }

        /// <summary>
        /// List to populate the options for user to select from.
        /// </summary>
        public List<FormatTypes> FormatOptions { get; }

        /// <summary>
        /// Way to display to user what the expected cards per page with their settings will be.
        /// </summary>
        public int CardsPerPage
        {
            get
            {
                return _cardsPerPage;
            }
            set
            {
                OnPropertyChanging();
                _cardsPerPage = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties

        #region Methods

        public override void Cleanup()
        {
            DefaultSettings.UserSettings.PropertyChanged -= UserSettings_PropertyChanged;
            base.Cleanup();
        }

        #endregion Methods
    }
}
