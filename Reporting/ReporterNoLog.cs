using Boxy_Core.Mvvm;

namespace Boxy_Core.Reporting
{
    public class ReporterNoLog : NotifyPropertyBase, IReporter
    {
        /// <inheritdoc />
        public bool IsSystemBusy { get; private set; }

        /// <inheritdoc />
        public bool IsProgressActive { get; private set; }

        /// <inheritdoc />
        public void StartBusy(string busyReason)
        {
            IsSystemBusy = true;
            Report(busyReason);
            OnPropertyChanged(nameof(IsSystemBusy));
        }

        /// <inheritdoc />
        public void StopBusy()
        {
            IsSystemBusy = false;
            OnPropertyChanged(nameof(IsSystemBusy));
        }

        /// <inheritdoc />
        public void StartProgress()
        {
            IsProgressActive = true;
            OnPropertyChanged(nameof(IsProgressActive));
        }

        /// <inheritdoc />
        public void StopProgress()
        {
            IsProgressActive = false;
            OnPropertyChanged(nameof(IsProgressActive));
        }

        /// <inheritdoc />
        public void Report(string value)
        {
            StatusReported?.Invoke(this, new CardMimicStatusEventArgs(value, false));
        }

        /// <inheritdoc />
        public void Report(string message, bool isError)
        {
            StatusReported?.Invoke(this, new CardMimicStatusEventArgs(message, isError));
        }

        /// <inheritdoc />
        public void Progress(double progressValue, double progressMin, double progressMax)
        {
            ProgressReported?.Invoke(this, new CardMimicProgressEventArgs(progressValue, progressMin, progressMax));
        }

        /// <inheritdoc />
        public event EventHandler<CardMimicStatusEventArgs>? StatusReported;

        /// <inheritdoc />
        public event EventHandler<CardMimicProgressEventArgs>? ProgressReported;
    }
}
