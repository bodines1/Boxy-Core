using Boxy_Core.DialogService;
using Boxy_Core.Model.ScryfallData;
using Boxy_Core.Utilities;
using Boxy_Core.ViewModels.Dialogs;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace Boxy_Core.Model
{
    public class ScryfallService
    {
        public ScryfallService(IDialogService dialogService)
        {
            _dialogService = dialogService;

            Version? version = null;

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                version = ApplicationDeployment.CurrentDeployment?.CurrentVersion;
            }

            version ??= Assembly.GetEntryAssembly()?.GetName().Version ?? Assembly.GetExecutingAssembly().GetName().Version ?? throw new ApplicationException("Could not get application version for Scryfall services.");

            _httpClient = new();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", $"Boxy/{version}");
            _httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
        }

        private IDialogService _dialogService;
        private HttpClient _httpClient;

        #region Services

        private void DisplayError(string message, Exception? exception = null)
        {
            var messageBuilder = new StringBuilder(message);

            while (exception is not null)
            {
                messageBuilder.AppendLine(exception.Message);
                exception = exception.InnerException;
            }

            var infoVm = new MessageDialogViewModel(messageBuilder.ToString().Trim(), "Error using Scryfall API");
            _dialogService.Show(infoVm);
        }

        public async Task<Card?> GetFuzzyCardAsync(string search, IProgress<string> reporter)
        {
            // Return nothing if search value is meaningless.
            if (string.IsNullOrWhiteSpace(search))
            {
                return null;
            }

            // The API expects search terms to be separated by the + symbol in place of whitespace.
            string[] searchTerms = search.Trim().Split();
            search = string.Join("+", searchTerms);
            string request = FuzzyCardSearch + search;
            HttpResponseMessage response;

            try
            {
                response = await _httpClient.GetAsync(request);
            }
            catch (Exception exc)
            {
                DisplayError($"{request}\r\nAPI Error", exc);
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                DisplayError($"{request}\r\nHTTP Error {(int)response.StatusCode}: {response.ReasonPhrase}");
                return null;
            }

            string serialized = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Card>(serialized);

            if (result is null)
            {
                DisplayError($"{request}\r\nAPI Response Error: Result was null/empty.");
                return null;
            }

            return result;
        }

        public async Task<List<Card>?> GetAllPrintingsAsync(Card card, IProgress<string> reporter)
        {
            // Return nothing if search value is meaningless.
            if (card == null)
            {
                throw new ArgumentNullException(nameof(card), "Card object cannot be null. Consumer must check card before using this method.");
            }

            string request = ExactCardSearchWithPrintings + card.OracleId;
            HttpResponseMessage response;
            var result = new List<Card>();

            try
            {
                response = await _httpClient.GetAsync(request);
            }
            catch (Exception exc)
            {
                DisplayError($"{request}\r\nAPI Error", exc);
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                DisplayError($"{request}\r\nHTTP Error {(int)response.StatusCode}: {response.ReasonPhrase}");
                return null;
            }

            string serialized = await response.Content.ReadAsStringAsync();
            var scryfallList = JsonSerializer.Deserialize<ScryfallList<Card>>(serialized);

            if (scryfallList is null)
            {
                DisplayError($"{request}\r\nAPI Response Error: Result was null/empty.");
                return null;
            }

            result.AddRange(scryfallList.Data);

            while (scryfallList.HasMore)
            {
                request = scryfallList.NextPage;

                try
                {
                    response = await _httpClient.GetAsync(request);
                }
                catch (Exception exc)
                {
                    DisplayError($"{request}\r\nAPI Error", exc);
                    return null;
                }

                if (!response.IsSuccessStatusCode)
                {
                    DisplayError($"{request}\r\nHTTP Error {(int)response.StatusCode}: {response.ReasonPhrase}");
                    return null;
                }

                serialized = await response.Content.ReadAsStringAsync();
                scryfallList = JsonSerializer.Deserialize<ScryfallList<Card>>(serialized);

                if (scryfallList is null)
                {
                    DisplayError($"{request}\r\nAPI Response Error: Result was null/empty.");
                    return null;
                }

                result.AddRange(scryfallList.Data);
            }

            result.RemoveAll(crd => crd.CollectorNumber.Any(ch => ch == 's' || ch == 'p' || crd.Digital));
            return result;
        }

        public async Task<Bitmap?> GetImageAsync(string imageUri, IProgress<string> reporter)
        {
            // Can't find image without a valid card.
            if (string.IsNullOrWhiteSpace(imageUri))
            {
                throw new ArgumentNullException(nameof(imageUri), "Image request URI cannot be null or empty/whitespace. Consumer must check before using this method.");
            }

            string request = imageUri;
            HttpResponseMessage response;

            try
            {
                response = await _httpClient.GetAsync(request);
            }
            catch (Exception exc)
            {
                DisplayError($"{request}\r\nAPI Error", exc);
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                DisplayError($"{request}\r\nHTTP Error {(int)response.StatusCode}: {response.ReasonPhrase}");
                return null;
            }

            try
            {
                using (Stream stream = await response.Content.ReadAsStreamAsync())
                {
                    var bitmap = new Bitmap(stream ?? throw new InvalidOperationException("File stream from service was null, ensure the URI is correct."));
                    await stream.FlushAsync();
                    return bitmap;
                }
            }
            catch (Exception exc)
            {
                DisplayError($"{request}\r\nBitmap read error.", exc);
                return null;
            }
        }

        public async Task<ScryfallList<BulkData>?> GetBulkDataInfo(IProgress<string> reporter)
        {
            string request = BulkData.ToString();
            HttpResponseMessage response;

            try
            {
                response = await _httpClient.GetAsync(request);
            }
            catch (Exception exc)
            {
                DisplayError($"{request}\r\nAPI Error", exc);
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                DisplayError($"{request}\r\nHTTP Error {(int)response.StatusCode}: {response.ReasonPhrase}");
                return null;
            }

            string serialized = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ScryfallList<BulkData>>(serialized);

            if (result is null)
            {
                DisplayError($"{request}\r\nAPI Response Error: Result was null/empty.");
                return null;
            }

            return result;
        }

        public async Task<List<Card>?> GetBulkCards(Uri catalogUri, IProgress<string> reporter)
        {
            string request = catalogUri.ToString();
            HttpResponseMessage response;

            try
            {
                response = await _httpClient.GetAsync(request);
            }
            catch (Exception exc)
            {
                DisplayError($"{request}\r\nAPI Error", exc);
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                DisplayError($"{request}\r\nHTTP Error {(int)response.StatusCode}: {response.ReasonPhrase}");
                return null;
            }

            string serialized = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<Card>>(serialized);

            if (result is null)
            {
                DisplayError($"{request}\r\nAPI Response Error: Result was null/empty.");
                return null;
            }

            return result;
        }

        public async Task<Card?> GetRandomCard(IProgress<string> reporter)
        {
            string name = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            reporter.Report(name);
            string request = RandomCard.ToString();
            HttpResponseMessage response;

            try
            {
                response = await _httpClient.GetAsync(request);
            }
            catch (Exception exc)
            {
                DisplayError($"{request}\r\nAPI Error", exc);
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                DisplayError($"{request}\r\nHTTP Error {(int)response.StatusCode}: {response.ReasonPhrase}");
                return null;
            }

            string serialized = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Card>(serialized);

            if (result is null)
            {
                DisplayError($"{request}\r\nAPI Response Error: Result was null/empty.");
                return null;
            }

            return result;
        }

        #endregion Services

        #region URIs

        /// <summary>
        /// Returns <see cref="Card"/>.
        /// </summary>
        private static Uri FuzzyCardSearch { get; } = new Uri("https://api.scryfall.com/cards/named?fuzzy=");

        /// <summary>
        /// Returns <see cref="Card"/>.
        /// </summary>
        private static Uri BulkData { get; } = new Uri("https://api.scryfall.com/bulk-data");
        
        /// <summary>
        /// Returns <see cref="ScryfallList{T}"/> where data is <see cref="Card"/> objects.
        /// </summary>
        private static Uri ExactCardSearchWithPrintings { get; } = new Uri("https://api.scryfall.com/cards/search?order=released&unique=prints&q=digital%3Afalse+oracle_id%3A");
        
        /// <summary>
        /// Returns a random <see cref="Card"/> from Scryfall.
        /// </summary>
        private static Uri RandomCard { get; } = new Uri("https://api.scryfall.com/cards/random?");

        #endregion URIs
    }
}