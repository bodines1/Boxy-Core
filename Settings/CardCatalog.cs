using Boxy_Core.Model.ScryfallData;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Boxy_Core.Settings
{
    /// <summary>
    /// Holds a queryable set of all oracle cards (no duplicate printings) to prevent excess queries to ScryfallAPI.
    /// </summary>
    [Serializable]
    public class CardCatalog
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of this class from code. Used to create a new catalog, but normal operation will have this created
        /// from deserializing a local file using <see cref="CreateFromFile"/>
        /// </summary>
        public CardCatalog(BulkData scryfallMetadata, List<Card> cards, DateTime? updateTime)
        {
            Cards = cards;
            UpdateTime = updateTime;
            ScryfallMetadata = scryfallMetadata;
        }

        /// <summary>
        /// Creates an instance of <see cref="SavePath"/> by deserializing a file at the <see cref="ArtworkPreferences"/> if it
        /// exists, or a new instance if deserialization fails.
        /// </summary>
        public static CardCatalog? CreateFromFile()
        {
            return JsonSerializer.Deserialize<CardCatalog>(File.ReadAllText(SavePath));
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Where to save the local serialized copy of this catalog.
        /// </summary>
        private static string SavePath
        {
            get
            {
                return Environment.ExpandEnvironmentVariables("%AppData%/Boxy/scryfall-oracle-cards.json");
            }
        }

        /// <summary>
        /// The queryable card collection.
        /// </summary>
        public List<Card> Cards { get; }

        /// <summary>
        /// The time when the user last updated this catalog.
        /// </summary>
        public DateTime? UpdateTime { get; }

        /// <summary>
        /// Metadata information about the catalog.
        /// </summary>
        public BulkData ScryfallMetadata { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Find a specific set of cards with an exact name search.
        /// </summary>
        /// <param name="name">The card name to search for.</param>
        /// <returns>The card objects with the matching</returns>
        public List<Card> FindExactCard(string name)
        {
            return Cards.FindAll(c => c.Name.ToUpper(CultureInfo.CurrentCulture).Trim() == name.ToUpper(CultureInfo.CurrentCulture).Trim());
        }

        /// <summary>
        /// Saves this object to the <see cref="SavePath"/> as a JSON string.
        /// </summary>
        public void SaveToFile()
        {
            string directory = Path.GetDirectoryName(SavePath) ?? string.Empty;

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            byte[] data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(this));

            using var fileStream = new FileStream(SavePath, FileMode.Create);
            fileStream.Write(data, 0, data.Length);
            fileStream.Flush();
        }

        #endregion Methods
    }
}
