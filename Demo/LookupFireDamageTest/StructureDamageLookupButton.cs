using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;

namespace LookupFireDamageTest
{
	public class StructureDamageLookupButton : Button
	{
		protected override void OnClick()
		{
			// Activate the ParcelDockpane
			var pane = ArcGIS.Desktop.Framework.FrameworkApplication.DockPaneManager.Find("LookupFireDamageTest_ParcelDockpane");
			pane?.Activate();
		}
	}
}
