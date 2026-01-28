using ArcGIS.Desktop.Framework.Contracts;
using System.Windows;

namespace LookupFireDamage
{
    public class StructureDamageLookupButton : Button
    {
        protected override void OnClick()
        {
            MessageBox.Show("Structure Damage Lookup button clicked.", "Fire Damage", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
