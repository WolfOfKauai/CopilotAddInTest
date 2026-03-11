using ArcGIS.Desktop.Framework.Controls;
using System.Windows.Input;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Mapping.Events;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace LookupFireDamageOrig
{
    public class DamageAssessmentRecord
    {
        public string RecordName { get; set; }
        public string DamageLevel { get; set; }
        public ImageSource Image { get; set; }
        public string DamageGlobalID { get; set; } // For linking to Damage Assessment
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
                    // If the record already has an image, use it; otherwise, try to load from Damage Assessment
                    if (value?.Image != null)
                    {
                        DamageImage = value.Image;
                    }
                    else if (!string.IsNullOrEmpty(value?.DamageGlobalID))
                    {
                        // Load the image asynchronously
                        _ = LoadDamageImageAsync(value);
                    }
                    else
                    {
                        DamageImage = null;
                    }
                }
            }
        }

        private async System.Threading.Tasks.Task LoadDamageImageAsync(DamageAssessmentRecord record)
        {
            if (record == null || string.IsNullOrEmpty(record.DamageGlobalID))
            {
                DamageImage = null;
                return;
            }
            var mapView = ArcGIS.Desktop.Mapping.MapView.Active;
            if (mapView == null)
            {
                DamageImage = null;
                return;
            }
            await ArcGIS.Desktop.Framework.Threading.Tasks.QueuedTask.Run(() =>
            {
                // Find the "Damage Assessment" feature layer
                var damageImagesLayer = mapView.Map?.Layers.OfType<ArcGIS.Desktop.Mapping.FeatureLayer>().FirstOrDefault(l => l.Name == "Damage Assessment");
                if (damageImagesLayer == null)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() => DamageImage = null);
                    return;
                }
                var table = damageImagesLayer.GetTable();
                // Query by GlobalID
                var queryFilter = new ArcGIS.Core.Data.QueryFilter
                {
                    WhereClause = $"GlobalID = '{record.DamageGlobalID}'"
                };
                using (var rowCursor = damageImagesLayer.Search(queryFilter))
                {
                    if (rowCursor.MoveNext())
                    {
                        using var row = rowCursor.Current;
                        // Get attachments
                        var attachments = row.GetAttachments();
                        if (attachments != null && attachments.Count > 0)
                        {
                            var attachment = attachments[0];
                            // Load image from attachment content
                            var data = attachment.GetData();
                            if (data != null)
                            {
                                data.Position = 0;
                                var bitmap = new System.Windows.Media.Imaging.BitmapImage();
                                bitmap.BeginInit();
                                bitmap.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                                bitmap.StreamSource = data;
                                bitmap.EndInit();
                                bitmap.Freeze();
                                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                                {
                                    record.Image = bitmap;
                                    DamageImage = bitmap;
                                });
                                return;
                            }
                        }
                    }
                }
                System.Windows.Application.Current.Dispatcher.Invoke(() => DamageImage = null);
            });
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
            //DamageAssessmentRecords.Add(new DamageAssessmentRecord { RecordName = "Record 1", DamageLevel = "Minor", Image = null });
            //DamageAssessmentRecords.Add(new DamageAssessmentRecord { RecordName = "Record 2", DamageLevel = "Major", Image = null });
            //DamageAssessmentRecords.Add(new DamageAssessmentRecord { RecordName = "Record 3", DamageLevel = "Destroyed", Image = null });
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
                    System.Windows.Application.Current.Dispatcher.Invoke(() => DamageAssessmentRecords.Clear());
                    return;
                }

                // Get selected ObjectIDs for the "Parcels" layer
                var selection = parcelLayer.GetSelection();
                var selectedOids = selection?.GetObjectIDs();
                if (selectedOids != null && selectedOids.Count > 0)
                {
                    var objectId = selectedOids.First();
                    // Query the APN, SitusAddress, and Shape fields for the first selected feature
                    var table = parcelLayer.GetTable();
                    var queryFilter = new ArcGIS.Core.Data.QueryFilter
                    {
                        ObjectIDs = new System.Collections.Generic.List<long> { objectId },
                        SubFields = "APN,SitusAddress,Shape"
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

                                // Get the parcel geometry
                                var shape = row["Shape"] as ArcGIS.Core.Geometry.Geometry;
                                if (shape != null)
                                {
                                    // Find the "Fire Damage" feature layer
                                    var fireDamageLayer = mapView.Map?.Layers.OfType<ArcGIS.Desktop.Mapping.FeatureLayer>().FirstOrDefault(l => l.Name == "Damage Assessment");
                                    if (fireDamageLayer != null)
                                    {
                                        var fireTable = fireDamageLayer.GetTable();
                                        // Use a spatial query filter
                                        var spatialFilter = new ArcGIS.Core.Data.SpatialQueryFilter
                                        {
                                            FilterGeometry = shape,
                                            SpatialRelationship = ArcGIS.Core.Data.SpatialRelationship.Intersects,
                                            SubFields = "Damage,StructureType,GlobalID"
                                        };
                                        var damageRecords = new System.Collections.Generic.List<DamageAssessmentRecord>();
                                        using (var fireCursor = fireTable.Search(spatialFilter, false))
                                        {
                                            while (fireCursor.MoveNext())
                                            {
                                                using (var fireRow = fireCursor.Current)
                                                {
                                                    var structureType = fireRow["StructureType"]?.ToString() ?? string.Empty;
                                                    var damage = fireRow["Damage"]?.ToString() ?? string.Empty;
                                                    var damageGlobalID = fireRow.GetTable().GetDefinition().GetFields().Any(f => f.Name.Equals("GlobalID", System.StringComparison.CurrentCultureIgnoreCase)) ? fireRow["GlobalID"]?.ToString() : string.Empty;
                                                    var recordName = string.IsNullOrEmpty(structureType) && string.IsNullOrEmpty(damage)
                                                        ? $"Fire Damage OID {fireRow.GetObjectID()}"
                                                        : $"{structureType} - {damage}";
                                                    damageRecords.Add(new DamageAssessmentRecord
                                                    {
                                                        RecordName = recordName,
                                                        DamageLevel = damage,
                                                        Image = null,
                                                        DamageGlobalID = damageGlobalID
                                                    });
                                                }
                                            }
                                        }
                                        // Update the collection on the UI thread
                                        System.Windows.Application.Current.Dispatcher.Invoke(() =>
                                        {
                                            DamageAssessmentRecords.Clear();
                                            foreach (var rec in damageRecords)
                                                DamageAssessmentRecords.Add(rec);
                                            if (DamageAssessmentRecords.Count > 1)
                                                SelectedDamageAssessmentRecord = DamageAssessmentRecords.First();
                                        });
                                    }
                                    else
                                    {
                                        // No fire damage layer found, clear records
                                        System.Windows.Application.Current.Dispatcher.Invoke(() => DamageAssessmentRecords.Clear());
                                    }
                                }
                                else
                                {
                                    // No shape found, clear records
                                    System.Windows.Application.Current.Dispatcher.Invoke(() => DamageAssessmentRecords.Clear());
                                }
                                return;
                            }
                        }
                    }
                }
                APN = string.Empty;
                Address = string.Empty;
                // Clear the damage records if no parcel is selected
                System.Windows.Application.Current.Dispatcher.Invoke(() => DamageAssessmentRecords.Clear());
            });
        }
    }
}
