using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopilotAddIn
{
    /// <summary>
    /// Sample button implementation
    /// Demonstrates basic button functionality in ArcGIS Pro Add-in
    /// </summary>
    internal class Button1
    {
        /// <summary>
        /// Button constructor
        /// </summary>
        public Button1()
        {
        }

        /// <summary>
        /// Called when the button is clicked
        /// </summary>
        protected void OnClick()
        {
            // TODO: Add button click logic
            // Example: Show a message box, perform a map operation, etc.
            System.Windows.MessageBox.Show("Button clicked! This is where your custom logic goes.",
                                          "CopilotAddIn",
                                          System.Windows.MessageBoxButton.OK,
                                          System.Windows.MessageBoxImage.Information);
        }

        /// <summary>
        /// Called to determine if the button should be enabled or disabled
        /// </summary>
        /// <returns>True if button should be enabled, false otherwise</returns>
        protected bool OnUpdate()
        {
            // TODO: Add logic to determine if button should be enabled
            // Example: Check if a map view is active
            return true;
        }
    }
}
