using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Boxy_Core.Mvvm
{
    /// <summary>
    /// Base implementation of <see cref="INotifyPropertyChanged"/> and <see cref="INotifyPropertyChanging"/>.
    /// </summary>
    public abstract class NotifyPropertyBase : INotifyPropertyChanged, INotifyPropertyChanging
    {
        #region INotifyPropertyChanging Members

        /// <summary>
        /// Fires when a property is about to change to a new value.
        /// </summary>
        public event PropertyChangingEventHandler? PropertyChanging;
        
        /// <summary>
        /// Call to invoke <see cref="PropertyChanging"/>.
        /// </summary>
        /// <param name="propertyName">Property name of the changing property. If not used, the name of the member from which the call was made will be used instead.</param>
        protected virtual void OnPropertyChanging([CallerMemberName] string propertyName = null)
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanging Members

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Fires when a property has changed to a new value.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Call to invoke <see cref="PropertyChanged"/>.
        /// </summary>
        /// <param name="propertyName">Property name of the changed property. If not used, the name of the member from which the call was made will be used instead.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged Members
    }
}
