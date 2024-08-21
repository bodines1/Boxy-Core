using Boxy_Core.Mvvm;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Boxy_Core.Settings
{
    /// <summary>
    /// User settings for storing preferences.
    /// </summary>
    [Serializable]
    public class UserSettings : NotifyPropertyBase
    {
        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="ArtworkPreferences"/> by deserializing a file at the <see cref="SavePath"/> if it
        /// exists, or a new instance if deserialization fails.
        /// </summary>
        public static UserSettings CreateFromFile()
        {
            try
            {
                var deserializedFromFile = JsonSerializer.Deserialize<UserSettings>(File.ReadAllText(SavePath));
                return deserializedFromFile ?? new UserSettings();
            }
            catch (Exception)
            {
                return new UserSettings();
            }
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Path to save the serialized <see cref="UserSettings"/> file to.
        /// </summary>
        private static string SavePath
        {
            get
            {
                return Environment.ExpandEnvironmentVariables("%AppData%/Boxy/user-settings.json");
            }
        }

        public double MainWindowLeft { get; set; }

        #endregion Properties

        #region Methods

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
