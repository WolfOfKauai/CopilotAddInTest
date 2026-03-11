using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Core.Events;
using ArcGIS.Core.Data;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace LookupFireDamageTest
{

	public class DamageAssessmentRecord
{
    public int Id { get; set; }
    public string StructureType { get; set; }
    public string Damage { get; set; }
    public string ImagePath { get; set; }
    public string GlobalID { get; set; }
    public override string ToString() => $"{StructureType} - {Damage}";
}
	public class ParcelDockpaneViewModel : DockPane
	{
		private string _apn;
		private string _address;
		private string _Damage;
		private System.Windows.Media.ImageSource _damageImage;
		private DamageAssessmentRecord _selectedDamageAssessmentRecord;
		public RelayCommand SelectParcelCommand => new RelayCommand(() =>
		{
			ArcGIS.Desktop.Framework.FrameworkApplication.SetCurrentToolAsync("esri_mapping_selectByRectangleTool");
		});

    public System.Collections.ObjectModel.ObservableCollection<DamageAssessmentRecord> DamageAssessmentRecords { get; } = new System.Collections.ObjectModel.ObservableCollection<DamageAssessmentRecord>();
    private readonly object _damageAssessmentRecordsLock = new object();

		public DamageAssessmentRecord SelectedDamageAssessmentRecord
		{
			get => _selectedDamageAssessmentRecord;
			set
			{
				SetProperty(ref _selectedDamageAssessmentRecord, value, () => SelectedDamageAssessmentRecord);
				UpdateDamageAssessmentDetails();
			}
		}

		public string Damage
		{
			get => _Damage;
			set => SetProperty(ref _Damage, value, () => Damage);
		}

		public System.Windows.Media.ImageSource DamageImage
		{
			get => _damageImage;
			set => SetProperty(ref _damageImage, value, () => DamageImage);
		}

		private SubscriptionToken _selectionEvent;

		public ParcelDockpaneViewModel()
        {
            _selectionEvent = ArcGIS.Desktop.Mapping.Events.MapSelectionChangedEvent.Subscribe(OnMapSelectionChanged);
            System.Windows.Data.BindingOperations.EnableCollectionSynchronization(DamageAssessmentRecords, _damageAssessmentRecordsLock);
        }

		private void UpdateDamageAssessmentDetails()
        {
            if (SelectedDamageAssessmentRecord != null)
            {
                Damage = SelectedDamageAssessmentRecord.Damage;
                // Update DamageImage using GlobalID
                DamageImage = null;
                if (!string.IsNullOrEmpty(SelectedDamageAssessmentRecord.GlobalID))
                {
                    ArcGIS.Desktop.Framework.Threading.Tasks.QueuedTask.Run(() =>
                    {
                        var mapView = ArcGIS.Desktop.Mapping.MapView.Active;
                        if (mapView == null) return;
                        var damageImagesLayer = mapView.Map.GetLayersAsFlattenedList().OfType<ArcGIS.Desktop.Mapping.FeatureLayer>()
                            .FirstOrDefault(l => l.Name == "Damage Assessment");
                        if (damageImagesLayer == null) return;
                        {
                            var queryFilter = new ArcGIS.Core.Data.QueryFilter
                            {
                                WhereClause = $"GlobalID = '{SelectedDamageAssessmentRecord.GlobalID}'"
                            };
                            using (var cursor = damageImagesLayer.Search(queryFilter))
                            {
                                if (cursor.MoveNext())
                                {
                                    var feature = cursor.Current as ArcGIS.Core.Data.Feature;
                                    if (feature != null)
                                    {
                                        var attachments = feature.GetAttachments();
                                        var attachment = attachments.FirstOrDefault();
                                        if (attachment != null)
                                        {
                                            using (var stream = attachment.GetData())
                                            {
                                                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                                                {
                                                    var bitmap = new System.Windows.Media.Imaging.BitmapImage();
                                                    bitmap.BeginInit();
                                                    bitmap.StreamSource = stream;
                                                    bitmap.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                                                    bitmap.EndInit();
                                                    bitmap.Freeze();
                                                    DamageImage = bitmap;
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    });
                }
            }
            else
            {
                Damage = string.Empty;
                DamageImage = null;
            }
        }

		private async void OnMapSelectionChanged(ArcGIS.Desktop.Mapping.Events.MapSelectionChangedEventArgs args)
		{
			var mapView = ArcGIS.Desktop.Mapping.MapView.Active;
			if (mapView == null) return;

			var parcelsLayer = mapView.Map.GetLayersAsFlattenedList().OfType<ArcGIS.Desktop.Mapping.FeatureLayer>()
				.FirstOrDefault(l => l.Name == "Parcels");
			if (parcelsLayer == null) return;
            await ArcGIS.Desktop.Framework.Threading.Tasks.QueuedTask.Run(() =>
            {
                var selection = parcelsLayer.GetSelection();
                if (selection.GetObjectIDs().Count == 0) return;

                var oid = selection.GetObjectIDs().First();
                var queryFilter = new ArcGIS.Core.Data.QueryFilter { ObjectIDs = new List<long> { oid } };

                ArcGIS.Core.Geometry.Geometry parcelGeometry = null;

                using (var table = parcelsLayer.GetTable())
                {
                    using (var cursor = table.Search(queryFilter, false))
                    {
                        if (cursor.MoveNext())
                        {
                            var row = cursor.Current as Feature;
                            if (row != null)
                            {
                                APN = row["APN"]?.ToString();
                                Address = row["SitusAddress"]?.ToString();
                                parcelGeometry = row.GetShape();
                            }
                        }
                    }
                }

                // Spatial query against 'Damage Assessment' layer
                var damageLayer = mapView.Map.GetLayersAsFlattenedList().OfType<ArcGIS.Desktop.Mapping.FeatureLayer>()
                    .FirstOrDefault(l => l.Name == "Damage Assessment");
                if (damageLayer != null && parcelGeometry != null)
                {
                    var spatialQuery = new ArcGIS.Core.Data.SpatialQueryFilter
                    {
                        FilterGeometry = parcelGeometry,
                        SpatialRelationship = ArcGIS.Core.Data.SpatialRelationship.Intersects
                    };
                    using (var table = damageLayer.GetTable())
                    {
                        using (var cursor = table.Search(spatialQuery, false))
                        {
                            DamageAssessmentRecords.Clear();
                            int id = 1;
                            while (cursor.MoveNext())
                            {
                                var row = cursor.Current;
                                var record = new DamageAssessmentRecord
                                {
                                    Id = id++,
                                    StructureType = row["StructureType"]?.ToString(),
                                    Damage = row["Damage"]?.ToString(),
                                    GlobalID = row["GLOBALID"]?.ToString(),
                                    ImagePath = null // Add image path logic if needed
                                };
                                DamageAssessmentRecords.Add(record);
                            }
                            if (DamageAssessmentRecords.Count > 1)
                            {
                                SelectedDamageAssessmentRecord = DamageAssessmentRecords[0];
                            }
                        }
                    }
                }
            });
		}

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


	}
}
