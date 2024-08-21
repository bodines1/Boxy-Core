using Boxy_Core.Reporting;
using Boxy_Core.ViewModels;
using Boxy_Core.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Boxy_Core.ViewModels.Dialogs;
using Boxy_Core.Settings;
using Boxy_Core.Model;

namespace Boxy_Core.Views.Resources
{
    /// <summary>
    /// This class contains references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelLocator"/> class.
        /// </summary>
        public ViewModelLocator()
        {
            // Initialize other
            DialogService = new();

            // Initialize container dependencies.
            DialogService.Register<MessageDialogViewModel, MessageDialogView>();
            DialogService.Register<YesNoDialogViewModel, YesNoDialogView>();
            DialogService.Register<SettingsDialogViewModel, SettingsDialogView>();
            DialogService.Register<ChooseCardDialogViewModel, ChooseCardDialogView>();
            Reporter = new ReporterNoLog();
            ScryfallService = new(DialogService);

            // Initialize View Models
            MainVm = new MainViewModel(DialogService, Reporter, ScryfallService);
        }

        #endregion Constructors

        #region Dependencies

        private DialogService.DialogService DialogService { get; }

        private IReporter Reporter { get; }

        private ScryfallService ScryfallService { get; }

        #endregion Dependencies

        #region View First ViewModels

        /// <summary>
        /// Gets a new OperationTasksMainViewModel.
        /// </summary>
        public MainViewModel MainVm { get; }

        #endregion View First ViewModels
    }
}
