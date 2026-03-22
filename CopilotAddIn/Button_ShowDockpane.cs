using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopilotAddIn
{
    /// <summary>
    /// Button to show the sample dockpane
    /// Demonstrates how to open/activate a dockpane from a button click
    /// </summary>
    internal class Button_ShowDockpane
    {
        /// <summary>
        /// Button constructor
        /// </summary>
        public Button_ShowDockpane()
        {
        }

        /// <summary>
        /// Called when the button is clicked
        /// Opens or activates the dockpane
        /// </summary>
        protected void OnClick()
        {
            // TODO: Show dockpane
            // Example: Dockpane1ViewModel.Show();
            
            // In actual ArcGIS Pro Add-in, this would use:
            // var pane = FrameworkApplication.DockPaneManager.Find("CopilotAddIn_Dockpane1");
            // if (pane != null)
            //     pane.Activate();
            
            System.Windows.MessageBox.Show("This button would open the dockpane in a real ArcGIS Pro Add-in.",
                                          "Show Dockpane",
                                          System.Windows.MessageBoxButton.OK,
                                          System.Windows.MessageBoxImage.Information);
        }

        /// <summary>
        /// Called to determine if the button should be enabled or disabled
        /// </summary>
        /// <returns>True if button should be enabled, false otherwise</returns>
        protected bool OnUpdate()
        {
            return true;
        }
    }
}
