using ArcGIS.Desktop.Framework.Controls;
using System.Windows.Input;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Mapping.Events;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace LookupFireDamage
{
    public class DamageAssessmentRecord
    {
        public string RecordName { get; set; }
        public string DamageLevel { get; set; }
        public ImageSource Image { get; set; }
    }

    public class ParcelDockpaneViewModel : DockPane
    {
        private string _apn;
        private string _address;

        public string APN
        {
            get => _apn;
            set => SetProperty(ref _apn, value, () => APN);
        }

        public string Address
        {
            get => _address;
            set => SetProperty(ref _address, value, () => Address);
        }

        public ICommand SelectParcelCommand { get; }

        private ObservableCollection<DamageAssessmentRecord> _damageAssessmentRecords = new ObservableCollection<DamageAssessmentRecord>();
        public ObservableCollection<DamageAssessmentRecord> DamageAssessmentRecords
        {
            get => _damageAssessmentRecords;
            set => SetProperty(ref _damageAssessmentRecords, value, () => DamageAssessmentRecords);
        }

        private DamageAssessmentRecord _selectedDamageAssessmentRecord;
        public DamageAssessmentRecord SelectedDamageAssessmentRecord
        {
            get => _selectedDamageAssessmentRecord;
            set
            {
                if (SetProperty(ref _selectedDamageAssessmentRecord, value, () => SelectedDamageAssessmentRecord))
                {
                    DamageLevel = value?.DamageLevel ?? string.Empty;
                    DamageImage = value?.Image;
                }
            }
        }

        private string _damageLevel;
        public string DamageLevel
        {
            get => _damageLevel;
            set => SetProperty(ref _damageLevel, value, () => DamageLevel);
        }

        private ImageSource _damageImage;
        public ImageSource DamageImage
        {
            get => _damageImage;
            set => SetProperty(ref _damageImage, value, () => DamageImage);
        }

        public ParcelDockpaneViewModel()
        {
            SelectParcelCommand = new ArcGIS.Desktop.Framework.RelayCommand(SelectParcel);
            MapSelectionChangedEvent.Subscribe(OnMapSelectionChanged);

            // Example records for demonstration
            DamageAssessmentRecords.Add(new DamageAssessmentRecord { RecordName = "Record 1", DamageLevel = "Minor", Image = null });
            DamageAssessmentRecords.Add(new DamageAssessmentRecord { RecordName = "Record 2", DamageLevel = "Major", Image = null });
            DamageAssessmentRecords.Add(new DamageAssessmentRecord { RecordName = "Record 3", DamageLevel = "Destroyed", Image = null });
        }

        //protected override void OnHidden()
        //{
        //    MapSelectionChangedEvent.Unsubscribe(OnMapSelectionChanged);
        //    base.OnHidden();
        //}

        private void OnMapSelectionChanged(MapSelectionChangedEventArgs args)
        {
            // Only update if the 'Parcels' layer selection changed
            if (args == null || args.Selection == null)
                return;

            // Use ToDictionary() to access the selected layers
            var selectionDict = args.Selection.ToDictionary();
            var changed = selectionDict.Keys.Any(l => l.Name == "Parcels");
            if (changed)
                _ = UpdateParcelFieldsAsync();
        }

        //private void SelectParcel()
        //{
        //    // TODO: Implement parcel selection logic
        //    APN = "123-456-789";
        //    Address = "123 Main St, Anytown, USA";
        //}
        private async void SelectParcel()
        {
            // Set the current tool to the Select By Rectangle tool
            await FrameworkApplication.SetCurrentToolAsync("esri_mapping_selectByRectangleTool");
            // Optionally, clear or update APN/Address fields here if needed
        }
        private async System.Threading.Tasks.Task UpdateParcelFieldsAsync()
        {
            var mapView = ArcGIS.Desktop.Mapping.MapView.Active;
            if (mapView == null)
            {
                APN = string.Empty;
                Address = string.Empty;
                return;
            }

            await ArcGIS.Desktop.Framework.Threading.Tasks.QueuedTask.Run(() =>
            {
                // Find the "Parcels" feature layer
                var parcelLayer = mapView.Map?.Layers.OfType<ArcGIS.Desktop.Mapping.FeatureLayer>().FirstOrDefault(l => l.Name == "Parcels");
                if (parcelLayer == null)
                {
                    APN = string.Empty;
                    Address = string.Empty;
                    return;
                }

                // Get selected ObjectIDs for the "Parcels" layer
                var selection = parcelLayer.GetSelection();
                var selectedOids = selection?.GetObjectIDs();
                if (selectedOids != null && selectedOids.Count > 0)
                {
                    var objectId = selectedOids.First();
                    // Query the APN and SitusAddress fields for the first selected feature
                    var table = parcelLayer.GetTable();
                    var queryFilter = new ArcGIS.Core.Data.QueryFilter
                    {
                        ObjectIDs = new System.Collections.Generic.List<long> { objectId },
                        SubFields = "APN,SitusAddress"
                    };
                    using (var rowCursor = table.Search(queryFilter, false))
                    {
                        if (rowCursor.MoveNext())
                        {
                            using (var row = rowCursor.Current)
                            {
                                var apnValue = row["APN"]?.ToString();
                                var addressValue = row["SitusAddress"]?.ToString();
                                APN = apnValue ?? string.Empty;
                                Address = addressValue ?? string.Empty;
                                return;
                            }
                        }
                    }
                }
                APN = string.Empty;
                Address = string.Empty;
            });
        }
    }
}
