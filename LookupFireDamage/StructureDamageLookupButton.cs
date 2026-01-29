using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;

namespace LookupFireDamage
{
    internal class StructureDamageLookupButton : Button
    {
        protected override void OnClick()
        {
            MessageBox.Show("Structure Damage Lookup button clicked.", "Fire Damage");
        }
    }
}
