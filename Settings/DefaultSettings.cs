using Boxy_Core.Model.ScryfallData;
using Boxy_Core.Reporting;

namespace Boxy_Core.Settings
{
    public static class DefaultSettings
    {
        /// <summary>
        /// Initialize large/serialized objects that are stored as files.
        /// </summary>
        /// <param name="reporter">A way to report any messages created by the method.</param>
        public static void Initialize(IReporter? reporter = null)
        {
            try
            {
                var deserializedArtPrefs = ArtworkPreferences.CreateFromFile();

                if (deserializedArtPrefs != null)
                {
                    ArtworkPreferences = deserializedArtPrefs;
                }
                else
                {
                    ArtworkPreferences = new ArtworkPreferences();
                }
            }
            catch (Exception e)
            {
                reporter?.Report($"Error getting art preferences from file. A new file was created instead, but stored preferences may have been lost.\r\n\r\nException: {e.Message}", true);
                ArtworkPreferences = new ArtworkPreferences();
            }

            try
            {
                var deserializedCardCatalog = CardCatalog.CreateFromFile();

                if (deserializedCardCatalog != null)
                {
                    CardCatalog = deserializedCardCatalog;
                }
                else
                {
                    CardCatalog = new CardCatalog(new BulkData(), [], null);
                }
            }
            catch (Exception e)
            {
                reporter?.Report($"Error getting card catalog from file. A new file was created instead, but stored preferences may have been lost.\r\n\r\nException: {e.Message}", true);
                CardCatalog = new CardCatalog(new BulkData(), [], null);
            }

            try
            {
                var deserializedUserSettings = UserSettings.CreateFromFile();

                if (deserializedUserSettings != null)
                {
                    UserSettings = deserializedUserSettings;
                }
                else
                {
                    UserSettings = new UserSettings();
                }
            }
            catch (Exception e)
            {
                reporter?.Report($"Error getting user preferences from file. A new file was created instead, but stored preferences may have been lost.\r\n\r\nException: {e.Message}", true);
                UserSettings = new UserSettings();
            }
        }

        /// <summary>
        /// Serialize and save objects to file.
        /// </summary>
        /// <param name="reporter">A way to report any messages created by the method.</param>
        public static void Save(IReporter? reporter = null)
        {
            try
            {
                ArtworkPreferences.SaveToFile();
            }
            catch (Exception e)
            {
                reporter?.Report($"Error saving art preferences to file. Changes since application start may be lost.\r\n\r\nException: {e.Message}", true);
            }

            try
            {
                CardCatalog.SaveToFile();
            }
            catch (Exception e)
            {
                reporter?.Report($"Error saving card catalog to file. Changes since application start may be lost.\r\n\r\nException: {e.Message}", true);
            }

            try
            {
                UserSettings.SaveToFile();
            }
            catch (Exception e)
            {
                reporter?.Report($"Error saving user preferences to file. Changes since application start may be lost.\r\n\r\nException: {e.Message}", true);
            }
        }

        /// <summary>
        /// User's artwork preferences. Loaded from file on <see cref="Initialize"/>, saved to file on <see cref="Save"/>.
        /// </summary>
        public static ArtworkPreferences ArtworkPreferences { get; private set; } = [];

        public static CardCatalog CardCatalog { get; set; } = new CardCatalog(new BulkData(), [], null);

        public static UserSettings UserSettings { get; private set; } = new UserSettings();
    }
}
