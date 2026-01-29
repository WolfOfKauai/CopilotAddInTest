using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;

namespace LookupFireDamage
{
    internal class StructureDamageLookupButton : Button
    {
        protected override void OnClick()
        {
            // Show the ParcelDockpane
            var dockPane = FrameworkApplication.DockPaneManager.Find("ParcelDockpane");
            if (dockPane != null)
                dockPane.Activate();
            else
                MessageBox.Show("Parcel Dockpane could not be found.", "Error");
        }
    }
}
