// file: E:\Repos\WolfOfKauai\CopilotAddInTest\MyCopilotAddin\Show3DLotCreationButton.cs
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;

namespace MyCopilotAddin
{
    internal class Show3DLotCreationButton : Button
    {
        protected override void OnClick()
        {
            // show LotCreationDockPaneViewModel
            LotCreationDockPaneViewModel.Show();
        }
    }
}