using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using GUIFramework.Managers;
using GUIFramework.Repositories;
using GUISkinFramework.Skin;

namespace GUIFramework.GUI
{
    /// <summary>
    /// Interaction logic for GUIImage.xaml
    /// </summary>
    [GUISkinElement(typeof(XmlImage))]
    public partial class GUIImage
    {
        #region Fields

        private BitmapImage _image = new BitmapImage();
        private bool _mapControlsVisible;

        // map related variables
        private string _location;
        private int _zoom;
        private int _defaultZoom;
        private string _mapType;
        private double _latitude;
        private double _longitude;
        private int _mapHeight;
        private int _mapWidth;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GUIImage"/> class.
        /// </summary>
        public GUIImage()
        {

            InitializeComponent();

        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the skin XML.
        /// </summary>
        public XmlImage SkinXml
        {
            get { return BaseXml as XmlImage; }
        }

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        public BitmapImage Image
        {
            get { return _image; }
            set { _image = value; NotifyPropertyChanged("Image"); }
        }

        /// <summary>
        /// Gets the visibility of map controls
        /// </summary>
        /// 
        public bool MapControlsVisible
        {
            get { return _mapControlsVisible;}
            set { _mapControlsVisible = value; NotifyPropertyChanged("MapControlsVisible"); }
        }

        #endregion

        #region GUIControl Overrides

        /// <summary>
        /// Creates the control.
        /// </summary>
        public override void CreateControl()
        {
            base.CreateControl();
            RegisteredProperties = PropertyRepository.GetRegisteredProperties(this, SkinXml.Image);
            RegisteredProperties.AddRange(PropertyRepository.GetRegisteredProperties(this, SkinXml.MapData));

            _mapHeight = (SkinXml.Height > 640) ? 640 : SkinXml.Height;
            _mapWidth = (SkinXml.Width > 640) ? 640 : SkinXml.Width;

            _defaultZoom = SkinXml.DefaultMapZoom;
            if (_defaultZoom < 0) _defaultZoom = 0;
            if (_defaultZoom > 21) _defaultZoom = 21;
            _zoom = _defaultZoom;
            _mapType = "roadmap";

        }


        /// <summary>
        /// Registers the info data.
        /// </summary>
        public override void OnRegisterInfoData()
        {
            base.OnRegisterInfoData();
            PropertyRepository.RegisterPropertyMessage(this, SkinXml.Image);
            PropertyRepository.RegisterPropertyMessage(this, SkinXml.MapData);
        }

        /// <summary>
        /// Deregisters the info data.
        /// </summary>
        public override void OnDeregisterInfoData()
        {
            base.OnDeregisterInfoData();
             PropertyRepository.DeregisterPropertyMessage(this, SkinXml.Image);
             PropertyRepository.DeregisterPropertyMessage(this, SkinXml.MapData);
        }

        /// <summary>
        /// Updates the info data.
        /// </summary>
        public async override void UpdateInfoData()
        {
            base.UpdateInfoData();

            if (!string.IsNullOrEmpty(SkinXml.MapData))
            {
                var location = await PropertyRepository.GetProperty<string>(SkinXml.MapData, null);
                if (!string.IsNullOrEmpty(location) && !location.Equals("---"))
                {
                    _latitude = 0;
                    _longitude = 0;
                    try
                    {
                        var part = location.Split(',');
                        if (part.Count() == 4)
                        {
                            _latitude = double.Parse(part[1], CultureInfo.InvariantCulture);
                            _longitude = double.Parse(part[3], CultureInfo.InvariantCulture);
                            if (part[0].Equals("S")) _latitude = -_latitude;
                            if (part[2].Equals("W")) _longitude = -_longitude;
                        }
                    }
                    catch
                    {
                        // ignored
                    }

                    if (Math.Abs(_latitude) > 0.0000001 && Math.Abs(_longitude) > 0.0000001)
                    {
                        _location = _latitude.ToString("F6", CultureInfo.InvariantCulture) + "," +
                               _longitude.ToString("F6", CultureInfo.InvariantCulture);
                        _zoom = _defaultZoom;
                        LoadMapImage();
                        MapControlsVisible = SkinXml.ShowMapControls;
                        return;           
                    }
                }
            }

            var img = await PropertyRepository.GetProperty<byte[]>(SkinXml.Image, null) 
                   ?? await PropertyRepository.GetProperty<byte[]>(SkinXml.DefaultImage, null);
            MapControlsVisible = false;
            Image = GUIImageManager.GetImage(img);
        }

        /// <summary>
        /// Clears the info data.
        /// </summary>
        public override void ClearInfoData()
        {
            base.ClearInfoData();
            Image = null;
        }

        #endregion

        #region MapControls

        // load the map from Google Maps into Image property
        private void LoadMapImage()
        {
            if (Math.Abs(_latitude) > 0.0000001 && Math.Abs(_longitude) > 0.0000001)
            {
                var location = _latitude.ToString("F6", CultureInfo.InvariantCulture) + "," +
                               _longitude.ToString("F6", CultureInfo.InvariantCulture);
                var url = "http://maps.googleapis.com/maps/api/staticmap?center=" + location +
                          "&size=" + _mapWidth;
                url += "x" + _mapHeight + "&markers=size:mid%7Ccolor:red%7C";
                url += _location + "&zoom=" + _zoom;
                url += "&maptype=" + _mapType + "&sensor=false";
                Image = GUIImageManager.GetImage(url);
            }
        }

        private void ZoomInButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_zoom < 21)
            {
                _zoom++;
                LoadMapImage();
            }
        }

        private void ZoomOutButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_zoom > 0)
            {
                _zoom--;
                LoadMapImage();
            }
        }

       private void RoadmapToggleButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!_mapType.Equals("roadmap"))
            {
                _mapType = "roadmap";
                LoadMapImage();
            }
        }

       private void TerrainToggleButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!_mapType.Equals("terrain"))
            {
                _mapType = "terrain";
                LoadMapImage();
            }
        }

       private void MoveUpButton_OnClick(object sender, RoutedEventArgs e)
       {
           // Use 88 to avoid values beyond 90 degrees of lat.
           if (_latitude < 88)
           {
               _latitude += ShiftMap();
               LoadMapImage();
           }
       }

        private void MoveDownButton_OnClick(object sender, RoutedEventArgs e)
        {
           // Use 88 to avoid values beyond 90 degrees of lat.
           if (_latitude > -88)
           {
              _latitude -= ShiftMap();
              LoadMapImage();
           }
        }

        private void MoveLeftButton_OnClick(object sender, RoutedEventArgs e)
        {
           if (_longitude > -179)
           {
              _longitude -= ShiftMap();
              LoadMapImage();
           }
        }

        private void MoveRightButton_OnClick(object sender, RoutedEventArgs e)
        {
           if (_longitude < 179)
           {
              _longitude += ShiftMap();
              LoadMapImage();
           }
        }

        private double ShiftMap()
        {
            if (_zoom == 15)
            {
                return 0.003;
            }
            double diff;
            if (_zoom > 15)
            {
                diff = _zoom - 15;
                return ((15 - diff) * 0.003) / 15;
            }
            diff = 15 - _zoom;
            return ((15 + diff) * 0.003) / 15;
        }
        #endregion
    }
}
