using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;

namespace LookupFireDamage
{
	public class StructureDamageLookupButton : Button
	{
		protected override void OnClick()
		{
			MessageBox.Show("Structure Damage Lookup button clicked.", "Fire Damage");
		}
	}
}
