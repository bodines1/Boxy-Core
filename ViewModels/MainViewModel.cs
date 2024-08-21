using Boxy_Core.DialogService;
using Boxy_Core.Model;
using Boxy_Core.Model.ScryfallData;
using Boxy_Core.Mvvm;
using Boxy_Core.Properties;
using Boxy_Core.Reporting;
using Boxy_Core.Settings;
using Boxy_Core.Utilities;
using Boxy_Core.ViewModels.Dialogs;
using PdfSharp.Pdf;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Boxy_Core.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="MainViewModel"/>.
        /// </summary>
        /// <param name="dialogService">Service for resolving and showing dialog windows from viewmodels.</param>
        /// <param name="reporter">Reports status and progress events to subscribers.</param>
        /// <param name="cardCatalog">Holds a queryable set of all oracle cards (no duplicate printings) to prevent excess queries to ScryfallAPI.</param>
        /// <param name="artworkPreferences">Holds a mapping of Oracle Ids to Card Ids to store and retrieve a user's preferred printing of a card.</param>
        public MainViewModel(IDialogService dialogService, IReporter reporter)
        {
            DialogService = dialogService;
            Reporter = reporter;
            ZoomPercent = DefaultSettings.UserSettings.ZoomPercent;
            DecklistText = string.Empty;
            
            DisplayedCards = new ObservableCollection<CardViewModel>();
            DisplayedCards.CollectionChanged += OnDisplayedCardsOnCollectionChanged;
            DisplayErrors = new ObservableCollection<string>();
            SavedPdfFilePaths = new ObservableCollection<string>();

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                Version? version = ApplicationDeployment.CurrentDeployment?.CurrentVersion;
                SoftwareVersion = version is null
                    ? $"Unknown Version"
                    : $"{version.Major}.{version.Minor}.{version.Build}";
            }
            else
            {
                SoftwareVersion = "Debug";
            }

            Reporter.StatusReported += (_, args) => LastStatus = args;
            Reporter.ProgressReported += (_, args) => LastProgress = args;
        }

        #endregion Constructors

        #region Fields

        private string _decklistText;
        private CardMimicStatusEventArgs _lastStatus;
        private CardMimicProgressEventArgs _lastProgress;
        private int _zoomPercent;
        private int _totalCards;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Service for resolving and showing dialog windows from viewmodels.
        /// </summary>
        private IDialogService DialogService { get; }

        /// <summary>
        /// Reports status and progress events to subscribers.
        /// </summary>
        public IReporter Reporter { get; }

        /// <summary>
        /// The version of the software currently running.
        /// </summary>
        public string SoftwareVersion { get; }

        /// <summary>
        /// A collection of card view models to display, and later to build the PDF.
        /// </summary>
        public ObservableCollection<CardViewModel> DisplayedCards { get; }

        /// <summary>
        /// Error messages generated during the card building process, stored to display to user.
        /// </summary>
        public ObservableCollection<string> DisplayErrors { get; }

        /// <summary>
        /// A collection of all PDF files user has created since the app started.
        /// </summary>
        public ObservableCollection<string> SavedPdfFilePaths { get; }

        /// <summary>
        /// Last status args received from the <see cref="Reporter"/>.
        /// </summary>
        public CardMimicStatusEventArgs LastStatus
        {
            get
            {
                return _lastStatus;
            }

            private set
            {
                _lastStatus = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Last progress args received from the <see cref="Reporter"/>.
        /// </summary>
        public CardMimicProgressEventArgs LastProgress
        {
            get
            {
                return _lastProgress;
            }

            private set
            {
                _lastProgress = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Text in the decklist text box.
        /// </summary>
        public string DecklistText
        {
            get
            {
                return _decklistText;
            }

            set
            {
                _decklistText = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// How big, as a percent, to make the card images compared to their default size.
        /// </summary>
        public int ZoomPercent
        {
            get
            {
                return _zoomPercent;
            }

            set
            {
                _zoomPercent = value;

                foreach (CardViewModel card in DisplayedCards)
                {
                    card.ScaleToPercent(_zoomPercent);
                }

                DefaultSettings.UserSettings.ZoomPercent = _zoomPercent;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Total number of cards in the <see cref="DisplayedCards"/>, taking quantity into account.
        /// </summary>
        public int TotalCards
        {
            get
            {
                return _totalCards;
            }

            private set
            {
                _totalCards = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties

        #region Commands

        #region SearchSubmit

        private AsyncCommand? _searchSubmit;

        /// <summary>
        /// Command which builds the list of displayed cards from the user's entered data.
        /// </summary>
        public AsyncCommand SearchSubmit
        {
            get
            {
                return _searchSubmit ??= new AsyncCommand(SearchSubmit_ExecuteAsync);
            }
        }

        private async Task SearchSubmit_ExecuteAsync()
        {
            DisplayErrors.Clear();
            TimeSpan? timeSinceUpdate = DateTime.Now - DefaultSettings.CardCatalog.UpdateTime;

            if (timeSinceUpdate == null)
            {
                var yesNoDialog = new YesNoDialogViewModel("Card Catalog must be updated before continuing. Would you like to update the Card Catalog (~65 MB) now?", "Update?");
                if (!(DialogService.ShowDialog(yesNoDialog) ?? false))
                {
                    return;
                }

                await UpdateCatalog_ExecuteAsync();
            }

            if (timeSinceUpdate > TimeSpan.FromDays(7))
            {
                var yesNoDialog = new YesNoDialogViewModel($"Card Catalog is out of date ({timeSinceUpdate.Value.Days} days), it is recommended you get a new catalog now." +
                                                           "If you don't, cards may not appear in search results or you may receive old " +
                                                           "imagery. Click 'Yes' to update the Card Catalog (~65 MB) now or 'No' use the old catalog.", "Update?");
                if (DialogService.ShowDialog(yesNoDialog) ?? false)
                {
                    await UpdateCatalog_ExecuteAsync();
                }
            }

            DisplayedCards.Clear();
            await Task.Delay(100);
            Reporter.StartBusy(LovecraftianPhraseGenerator.RandomPhrase());
            Reporter.StartProgress();
            Reporter.StatusReported += BuildingCardsErrors;

            List<SearchLine> lines = DecklistText
                .Split(["\r", "\n"], StringSplitOptions.RemoveEmptyEntries)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(l => new SearchLine(l))
                .ToList();

            for (var i = 0; i < lines.Count; i++)
            {
                List<Card> cards = DefaultSettings.CardCatalog.FindExactCard(lines[i].SearchTerm);

                if (!cards.Any())
                {
                    cards.Add(await ScryfallService.GetFuzzyCardAsync(lines[i].SearchTerm, Reporter));
                }

                if (!cards.Any())
                {
                    Reporter.Report($"[{lines[i].SearchTerm}] returned no results", true);
                    continue;
                }

                Card preferredCard;

                if (cards.Count > 1)
                {
                    var cardChooser = new ChooseCardDialogViewModel(cards, Reporter);
                    await cardChooser.LoadImagesFromCards();

                    if (!(DialogService.ShowDialog(cardChooser) ?? false))
                    {
                        continue;
                    }

                    preferredCard = DefaultSettings.ArtworkPreferences.GetPreferredCard(cardChooser.ChosenCard);
                }
                else
                {
                    preferredCard = DefaultSettings.ArtworkPreferences.GetPreferredCard(cards.Single());
                }

                var cardVm = new CardViewModel(Reporter, DefaultSettings.ArtworkPreferences, preferredCard, lines[i].Quantity, ZoomPercent);
                DisplayedCards.Add(cardVm);
                await Task.Delay(1);
                Reporter.Progress(i, 0, lines.Count - 1);
            }

            Reporter.StatusReported -= BuildingCardsErrors;
            Reporter.StopBusy();
            Reporter.StopProgress();
        }

        #endregion SearchSubmit

        #region BuildPdf

        private AsyncCommand? _buildPdf;

        public AsyncCommand BuildPdf
        {
            get
            {
                return _buildPdf ??= new AsyncCommand(BuildPdf_ExecuteAsync);
            }
        }

        private async Task BuildPdf_ExecuteAsync()
        {
            if (DisplayedCards.Any(q => q.IsPopulatingPrints || q.IsLoadingImage))
            {
                DialogService.ShowDialog(new MessageDialogViewModel("Please wait for all images to finish loading before creating the card PDF.", "Wait before continuing..."));
                return;
            }

            Reporter.StartBusy(LovecraftianPhraseGenerator.RandomPhrase());
            var pdfBuilder = new CardPdfBuilder(DefaultSettings.UserSettings.PdfPageSize, DefaultSettings.UserSettings.PdfScalingPercent, DefaultSettings.UserSettings.PdfHasCutLines, DefaultSettings.UserSettings.CutLineSize, DefaultSettings.UserSettings.CutLineColor);
            PdfDocument pdfDoc;

            if (DefaultSettings.UserSettings.PrintTwoSided)
            {
                pdfDoc = await pdfBuilder.BuildPdfTwoSided(DisplayedCards.ToList(), Reporter);
            }
            else
            {
                pdfDoc = await pdfBuilder.BuildPdfSingleSided(DisplayedCards.ToList(), Reporter);
            }

            string directory = Environment.ExpandEnvironmentVariables(DefaultSettings.UserSettings.PdfSaveFolder);
            const string fileName = "CardProxies";
            const string ext = ".pdf";
            string fullPath = Path.Combine(directory, fileName + ext);
            var tries = 1;

            while (File.Exists(fullPath))
            {
                fullPath = Path.Combine(directory, fileName + $" ({tries})" + ext);
                tries += 1;
            }

            Reporter.Report(LovecraftianPhraseGenerator.RandomPhrase());
            var message = string.Empty;

            Reporter.StopBusy();
            Reporter.StopProgress();

            if (!pdfDoc.CanSave(ref message))
            {
                Reporter.Report(message, true);
                Reporter.StopBusy();
                Reporter.StopProgress();
                return;
            }

            try
            {
                pdfDoc.Save(fullPath);
            }
            catch (Exception e)
            {
                Reporter.Report(e.Message, true);
                DisplayError(e, "Could not save PDF to file.");
                return;
            }

            Reporter.Report(LovecraftianPhraseGenerator.RandomPhrase());
            SavedPdfFilePaths.Add(fullPath);

            if (!DefaultSettings.UserSettings.PdfOpenWhenSaveDone)
            {
                return;
            }

            try
            {
                Process.Start(fullPath);
            }
            catch (Exception e)
            {
                Reporter.Report(e.Message, true);
                DisplayError(e, "Could not open PDF. Do you have a PDF viewer installed?");
            }
        }

        #endregion BuildPdf

        #region UpdateCatalog

        private AsyncCommand? _updateCatalog;

        /// <summary>
        /// Command which updates the locally stored card catalog from Scryfall.
        /// </summary>
        public AsyncCommand UpdateCatalog
        {
            get
            {
                return _updateCatalog ??= new AsyncCommand(UpdateCatalog_ExecuteAsync);
            }
        }

        private async Task UpdateCatalog_ExecuteAsync()
        {
            Reporter.StartBusy(LovecraftianPhraseGenerator.RandomPhrase());

            BulkData oracleBulkData = (await ScryfallService.GetBulkDataInfo(Reporter)).Data.Single(bd => bd.Name.Contains("Oracle"));
            List<Card> cards = await ScryfallService.GetBulkCards(oracleBulkData.PermalinkUri, Reporter);

            if (cards == null || cards.Count == 0)
            {
                Reporter.Report("Cards not found or empty", true);
                Reporter.StopBusy();
                return;
            }

            var catalog = new CardCatalog(oracleBulkData, cards, DateTime.Now);

            try
            {
                catalog.SaveToFile();
            }
            catch (Exception e)
            {
                DisplayError(e, "Could not save card catalog to local disk.");
                Reporter.Report(e.Message, true);
                Reporter.StopBusy();
                return;
            }

            DefaultSettings.CardCatalog = catalog;
            Reporter.Report("Catalog updated.");
            Reporter.StopBusy();
        }

        #endregion UpdateCatalog
        
        #region OpenSettings

        private RelayCommand? _openSettings;

        /// <summary>
        /// Command which opens a dialog for the user to view app settings and change/save them.
        /// </summary>
        public RelayCommand OpenSettings
        {
            get
            {
                return _openSettings ??= new RelayCommand(OpenSettings_ExecuteAsync);
            }
        }

        private void OpenSettings_ExecuteAsync()
        {
            var settingsVm = new SettingsDialogViewModel();

            if (!(DialogService.ShowDialog(settingsVm) ?? false))
            {
                return;
            }
        }

        #endregion OpenSettings

        #region OpenSinglePdf

        private RelayCommand? _openSinglePdf;

        public RelayCommand OpenSinglePdf
        {
            get
            {
                return _openSinglePdf ??= new RelayCommand(OpenSinglePdf_ExecuteAsync);
            }
        }

        private void OpenSinglePdf_ExecuteAsync(object? parameter)
        {
            if (parameter is not string paramAsString)
            {
                return;
            }

            try
            {
                Process.Start(paramAsString);
            }
            catch (Exception exc)
            {
                DisplayError(exc, "Could not open file. It's possible that the file no longer exists, " +
                                  "or there is no PDF reader installed on this computer.");
            }
        }

        #endregion OpenSinglePdf

        #endregion Commands

        #region Methods

        private void OnDisplayedCardsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs args)
        {
            if (sender is not IEnumerable<CardViewModel> list)
            {
                return;
            }

            var countable = list.ToList();

            if (countable.Count == 0)
            {
                TotalCards = 0;
                return;
            }

            TotalCards = countable.Select(cvm => cvm.Quantity).Sum();
        }

        private void BuildingCardsErrors(object? sender, CardMimicStatusEventArgs e)
        {
            if (!e.IsError)
            {
                return;
            }

            DisplayErrors.Add(e.Message);
        }

        private void DisplayError(Exception? exc, string additionalInfo)
        {
            var message = new StringBuilder($"{additionalInfo}\r\n\r\n");

            while (exc != null)
            {
                message.AppendLine(exc.Message);
                exc = exc.InnerException;
            }

            DialogService.ShowDialog(new MessageDialogViewModel(message.ToString(), "Error Encountered"));
        }

        /// <inheritdoc />
        public override void Cleanup()
        {
            base.Cleanup();
        }

        #endregion Methods
    }
}
