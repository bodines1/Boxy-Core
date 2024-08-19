using System.Text.Json.Serialization;
using System.Windows;
using System.Xml.Serialization;

namespace Boxy_Core.Mvvm
{
    /// <summary>
    /// Base implementation for view models.
    /// </summary>
    /// <inheritdoc />
    public class ViewModelBase : NotifyPropertyBase
    {
        private Window? _owner;

        /// <summary>
        /// Owning window.
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public Window Owner
        {
            get
            {
                return (_owner ??= Application.Current.MainWindow) ?? throw new InvalidOperationException("Application.Current.MainWindow was null during assignment.");
            }
            set
            {
                _owner = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// A place to put cleanup code for when a view model is no longer needed.
        /// </summary>
        public virtual void Cleanup()
        {
        }
    }
}
