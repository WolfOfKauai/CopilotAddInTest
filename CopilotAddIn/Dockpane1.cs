using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CopilotAddIn
{
    /// <summary>
    /// Sample dockpane view model
    /// Demonstrates dockpane functionality in ArcGIS Pro Add-in
    /// Dockpanes are persistent UI panels that can be docked or floating
    /// </summary>
    internal class Dockpane1ViewModel : INotifyPropertyChanged
    {
        private string _heading = "Sample Dockpane";
        private string _message = "This is a sample dockpane";

        /// <summary>
        /// Constructor
        /// </summary>
        public Dockpane1ViewModel()
        {
        }

        /// <summary>
        /// Property for the dockpane heading
        /// </summary>
        public string Heading
        {
            get { return _heading; }
            set
            {
                if (_heading != value)
                {
                    _heading = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Property for the message displayed in the dockpane
        /// </summary>
        public string Message
        {
            get { return _message; }
            set
            {
                if (_message != value)
                {
                    _message = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Show the dockpane
        /// </summary>
        internal static void Show()
        {
            // TODO: Show dockpane logic
            // Example: DockpaneManager.Show(dockpaneId);
        }

        /// <summary>
        /// Called when the dockpane is first initialized
        /// </summary>
        protected void Initialize()
        {
            // TODO: Add initialization logic
            // Load data, setup event handlers, etc.
        }

        /// <summary>
        /// Called when the dockpane is closed
        /// </summary>
        protected void OnClose()
        {
            // TODO: Add cleanup logic
            // Unsubscribe from events, save state, etc.
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event
        /// </summary>
        /// <param name="propertyName">Name of the property that changed</param>
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
