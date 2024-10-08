﻿using Boxy_Core.Model.ScryfallData;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Boxy_Core.Settings
{
    /// <summary>
    /// Holds a mapping of Oracle Ids to Cards to store and retrieve a user's preferred printing of a card.
    /// </summary>
    [Serializable]
    public class ArtworkPreferences : Dictionary<string, Card>
    {
        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="ArtworkPreferences"/> by deserializing a file at the <see cref="SavePath"/> if it
        /// exists, or a new instance if deserialization fails.
        /// </summary>
        public static ArtworkPreferences? CreateFromFile()
        {
            return JsonSerializer.Deserialize<ArtworkPreferences>(File.ReadAllText(SavePath));
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Path to save the serialized <see cref="ArtworkPreferences"/> file to.
        /// </summary>
        private static string SavePath
        {
            get
            {
                return Environment.ExpandEnvironmentVariables("%AppData%/Boxy/artwork-preferences.json");
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets the card ID of the user's preferred (most recently selected) printing of a card. Stored persistently between sessions.
        /// </summary>
        public Card GetPreferredCard(Card? card)
        {
            if (card is null)
            {
                throw new ArgumentNullException(nameof(card), "Card cannot be null. Consumer must check object before using this method.");
            }

            if (!ContainsKey(card.OracleId))
            {
                Add(card.OracleId, card);
            }

            return this[card.OracleId];
        }

        /// <summary>
        /// Updates the preference dictionary with the passed in card being the user's "preferred" version for that Oracle ID.
        /// </summary>
        /// <param name="card">The card to set as preferred.</param>
        public void UpdatePreferredCard(Card? card)
        {
            if (card is null)
            {
                return;
            }

            if (ContainsKey(card.OracleId))
            {
                Remove(card.OracleId);
            }

            Add(card.OracleId, card);
        }

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
