// file: E:\Repos\WolfOfKauai\CopilotAddInTest\MyCopilotAddin\LotCreationDockPaneViewModel.cs
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Mapping.Events;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Editing;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MyCopilotAddin
{
  internal class LotCreationDockPaneViewModel : DockPane
  {
    private const string _dockPaneID = "MyCopilotAddin_3DLotCreationDockPane";
    
    private string _parcelId;
    public string ParcelId
    {
      get => _parcelId;
      set => SetProperty(ref _parcelId, value, () => ParcelId);
    }

    public ObservableCollection<int> Resolutions { get; } =
      new ObservableCollection<int> { 0, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };

    private int _selectedResolution = 50;
    public int SelectedResolution
    {
      get => _selectedResolution;
      set => SetProperty(ref _selectedResolution, value, () => SelectedResolution);
    }

    public ICommand Create3DLotCommand { get; }

    public LotCreationDockPaneViewModel()
    {
      Create3DLotCommand = new RelayCommand(() => Create3DLot(), () => !string.IsNullOrEmpty(ParcelId));
      MapSelectionChangedEvent.Subscribe(OnMapSelectionChanged);
    }

    private void Create3DLot()
    {
      MessageBox.Show(
        $"Creating 3D Lots for Parcel ID: {ParcelId} with resolution: {SelectedResolution}");
      // TODO: Implement 3D Lots creation logic here
    }

    private void OnMapSelectionChanged(MapSelectionChangedEventArgs args)
    {
      _ = UpdateParcelIdAsync();
    }

    private async Task UpdateParcelIdAsync()
    {
      var mapView = MapView.Active;
      if (mapView == null)
      {
        ParcelId = string.Empty;
        return;
      }

      await ArcGIS.Desktop.Framework.Threading.Tasks.QueuedTask.Run(() =>
      {
        // Find the "Lots" feature layer
        var lotLayer = mapView.Map?.Layers.OfType<FeatureLayer>().FirstOrDefault(l => l.Name == "Lots");
        if (lotLayer == null)
        {
          ParcelId = string.Empty;
          return;
        }

        // Get selected ObjectIDs for the "Lots" layer
        var selection = lotLayer.GetSelection();
        var selectedOids = selection?.GetObjectIDs();
        if (selectedOids != null && selectedOids.Count > 0)
        {
          var objectId = selectedOids.First();
          // Query the Name field for the first selected feature
          var table = lotLayer.GetTable();
          var queryFilter = new QueryFilter
          {
            ObjectIDs = new List<long> { objectId },
            SubFields = "Name"
          };
          using (var rowCursor = table.Search(queryFilter, false))
          {
            if (rowCursor.MoveNext())
            {
              using (var row = rowCursor.Current)
              {
                var nameValue = row["Name"]?.ToString();
                ParcelId = nameValue ?? string.Empty;
                return;
              }
            }
          }
        }
        ParcelId = string.Empty;
      });
    }

    /// <summary>
    /// Call this to show the DockPane
    /// </summary>
    internal static void Show()
    {
      var pane = FrameworkApplication.DockPaneManager.Find(_dockPaneID);
      if (pane != null)
        pane.Activate();
    }
  }
}