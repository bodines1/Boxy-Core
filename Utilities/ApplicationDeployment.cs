using Boxy_Core.Reporting;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

// ReSharper disable UnusedMember.Global

namespace Boxy_Core.Utilities
{
    public class ApplicationDeployment
    {
        private static ApplicationDeployment? _currentDeployment;
        private static bool _currentDeploymentInitialized;
        private static bool _isNetworkDeployed;
        private static bool _isNetworkDeployedInitialized;

        public static bool IsNetworkDeployed
        {
            get
            {
                if (_isNetworkDeployedInitialized)
                {
                    return _isNetworkDeployed;
                }

                bool.TryParse(Environment.GetEnvironmentVariable("ClickOnce_IsNetworkDeployed"), out _isNetworkDeployed);
                _isNetworkDeployedInitialized = true;
                return _isNetworkDeployed;
            }
        }

        public static ApplicationDeployment? CurrentDeployment
        {
            get
            {
                if (_currentDeploymentInitialized)
                {
                    return _currentDeployment;
                }

                _currentDeployment = IsNetworkDeployed ? new ApplicationDeployment() : null;
                _currentDeploymentInitialized = true;
                return _currentDeployment;
            }
        }

        public Uri? ActivationUri
        {
            get
            {
                Uri.TryCreate(Environment.GetEnvironmentVariable("ClickOnce_ActivationUri"), UriKind.Absolute, out Uri? val);
                return val;
            }
        }

        public Version? CurrentVersion
        {
            get
            {
                Version.TryParse(Environment.GetEnvironmentVariable("ClickOnce_CurrentVersion"), out Version? val);
                return val;
            }
        }
        public string? DataDirectory
        {
            get
            {
                return Environment.GetEnvironmentVariable("ClickOnce_DataDirectory");
            }
        }

        public bool IsFirstRun
        {
            get
            {
                bool.TryParse(Environment.GetEnvironmentVariable("ClickOnce_IsFirstRun"), out bool val);
                return val;
            }
        }

        public DateTime TimeOfLastUpdateCheck
        {
            get
            {
                DateTime.TryParse(Environment.GetEnvironmentVariable("ClickOnce_TimeOfLastUpdateCheck"), out DateTime value);
                return value;
            }
        }
        public string? UpdatedApplicationFullName
        {
            get
            {
                return Environment.GetEnvironmentVariable("ClickOnce_UpdatedApplicationFullName");
            }
        }

        public Version? UpdatedVersion
        {
            get
            {
                Version.TryParse(Environment.GetEnvironmentVariable("ClickOnce_UpdatedVersion"), out Version? val);
                return val;
            }
        }

        public Uri? UpdateLocation
        {
            get
            {
                Uri.TryCreate(Environment.GetEnvironmentVariable("ClickOnce_UpdateLocation"), UriKind.Absolute, out Uri? val);
                return val;
            }
        }

        public Version? LauncherVersion
        {
            get
            {

                Version.TryParse(Environment.GetEnvironmentVariable("ClickOnce_LauncherVersion"), out Version? val);
                return val;
            }
        }

        public static async Task<bool> CheckForUpdate(IReporter reporter)
        {
            /* Environment variables added by ClickOnce deployment
            ClickOnce_IsNetworkDeployed
            ClickOnce_ActivationUri
            ClickOnce_CurrentVersion
            ClickOnce_DataDirectory
            ClickOnce_IsFirstRun
            ClickOnce_TimeOfLastUpdateCheck
            ClickOnce_UpdatedApplicationFullName
            ClickOnce_UpdatedVersion
            ClickOnce_UpdateLocation
            */
            if (!IsNetworkDeployed || CurrentDeployment is null)
            {
                return false;
            }

            try
            {
                var client = new HttpClient();
                await using Stream stream = await client.GetStreamAsync(CurrentDeployment.UpdateLocation);
                Version serverVersion = await ReadServerManifest(stream);
                return serverVersion > CurrentDeployment.CurrentVersion;
            }
            catch (Exception e)
            {
                reporter.Report($"Error while checking for application update.\r\n\r\nException: {e.Message}", true);
                return false;
            }
        }

        private static async Task<Version> ReadServerManifest(Stream stream)
        {
            XDocument xmlDoc = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
            XNamespace nsSys = "urn:schemas-microsoft-com:asm.v1";
            XElement xmlElement = xmlDoc.Descendants(nsSys.GetName("assemblyIdentity")).FirstOrDefault() ?? throw new ApplicationException("Invalid manifest document: CellManagerV2.Entry.application");
            string? version = xmlElement.Attribute("version")?.Value;

            if (string.IsNullOrWhiteSpace(version))
            {
                throw new ApplicationException("Version info is empty!");
            }

            return new Version(version);
        }
    }
}
