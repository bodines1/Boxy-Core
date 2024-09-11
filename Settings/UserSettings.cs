using Boxy_Core.Model.ScryfallData;
using Boxy_Core.Mvvm;
using Boxy_Core.Utilities;
using PdfSharp;
using PdfSharp.Drawing;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;

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
        /// Create a new instance of <see cref="UserSettings"/> with default values.
        /// </summary>
        public UserSettings()
        {
            MainWindowLeft = 0;
            MainWindowTop = 0;
            MainWindowWidth = 640;
            MainWindowHeight = 480;
            MainWindowState = WindowState.Normal;
            ZoomPercent = 100;
            PdfSaveFolder = "%USERPROFILE%\\Downloads";
            PdfHasCutLines = true;
            PdfScalingPercent = 100;
            PdfOpenWhenSaveDone = true;
            PdfPageSize = PageSize.Letter;
            Column0Width = 200;
            CutLineColor = XKnownColor.Gray;
            CutLineSize = CutLineSizes.Small;
            PrintTwoSided = false;
        }

        /// <summary>
        /// Creates an instance of <see cref="UserSettings"/> by deserializing a file at the <see cref="SavePath"/> if it
        /// exists, or a new instance if deserialization fails.
        /// </summary>
        public static UserSettings? CreateFromFile()
        {
            var deserialized = JsonSerializer.Deserialize<UserSettings>(File.ReadAllText(SavePath));
            if (deserialized?.PdfSaveFolder.Contains('/') ?? false)
            {
                //Small fix for an easy to make mistake.
                deserialized.PdfSaveFolder = deserialized.PdfSaveFolder.Replace('/', '\\');
            }

            return deserialized;
        }

        #endregion Constructors

        #region Fields

        private double _mainWindowLeft;
        private double _mainWindowTop;
        private double _mainWindowWidth;
        private double _mainWindowHeight;
        private WindowState _mainWindowState;
        private int _zoomPercent;
        private string _pdfSaveFolder = string.Empty;
        private bool _pdfHasCutLines;
        private double _pdfScalingPercent;
        private bool _pdfOpenWhenSaveDone;
        private PageSize _pdfPageSize;
        private double _column0Width;
        private XKnownColor _cutLineColor;
        private CutLineSizes _cutLineSize;
        private bool _printTwoSided;

        #endregion Fields

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

        public double MainWindowLeft
        { 
            get => _mainWindowLeft;
            set
            { 
                OnPropertyChanging();
                _mainWindowLeft = value;
                OnPropertyChanged();
            }
        }

        public double MainWindowTop
        {
            get => _mainWindowTop;
            set
            {
                OnPropertyChanging();
                _mainWindowTop = value;
                OnPropertyChanged();
            }
        }

        public double MainWindowWidth
        {
            get => _mainWindowWidth;
            set
            {
                OnPropertyChanging();
                _mainWindowWidth = value;
                OnPropertyChanged();
            }
        }

        public double MainWindowHeight
        {
            get => _mainWindowHeight;
            set
            {
                OnPropertyChanging();
                _mainWindowHeight = value;
                OnPropertyChanged();
            }
        }

        public WindowState MainWindowState
        {
            get => _mainWindowState;
            set
            {
                OnPropertyChanging();
                _mainWindowState = value;
                OnPropertyChanged();
            }
        }

        public int ZoomPercent
        {
            get => _zoomPercent;
            set
            {
                OnPropertyChanging();
                _zoomPercent = value;
                OnPropertyChanged();
            }
        }

        public string PdfSaveFolder
        {
            get => _pdfSaveFolder;
            set
            {
                OnPropertyChanging();
                _pdfSaveFolder = value;
                OnPropertyChanged();
            }
        }

        public bool PdfHasCutLines
        {
            get => _pdfHasCutLines;
            set
            {
                OnPropertyChanging();
                _pdfHasCutLines = value;
                OnPropertyChanged();
            }
        }

        public double PdfScalingPercent
        {
            get => _pdfScalingPercent;
            set
            {
                OnPropertyChanging();

                if (value < 90)
                {
                    _pdfScalingPercent = 90;
                }
                else if (value > 110)
                {
                    _pdfScalingPercent = 110;
                }
                else
                {
                    _pdfScalingPercent = value;
                }

                OnPropertyChanged();
            }
        }

        public bool PdfOpenWhenSaveDone
        {
            get => _pdfOpenWhenSaveDone;
            set
            {
                OnPropertyChanging();
                _pdfOpenWhenSaveDone = value;
                OnPropertyChanged();
            }
        }

        public PageSize PdfPageSize
        {
            get => _pdfPageSize;
            set
            {
                OnPropertyChanging();
                _pdfPageSize = value;
                OnPropertyChanged();
            }
        }

        public double Column0Width
        {
            get => _column0Width;
            set
            {
                OnPropertyChanging();
                _column0Width = value;
                OnPropertyChanged();
            }
        }

        public XKnownColor CutLineColor
        {
            get => _cutLineColor;
            set
            {
                OnPropertyChanging();
                _cutLineColor = value;
                OnPropertyChanged();
            }
        }

        public CutLineSizes CutLineSize
        {
            get => _cutLineSize;
            set
            {
                OnPropertyChanging();
                _cutLineSize = value;
                OnPropertyChanged();
            }
        }

        public bool PrintTwoSided
        {
            get => _printTwoSided;
            set
            {
                OnPropertyChanging();
                _printTwoSided = value;
                OnPropertyChanged();
            }
        }

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
