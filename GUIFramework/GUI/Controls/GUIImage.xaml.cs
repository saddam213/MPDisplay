using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Common.Helpers;
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

        private MapHelper _map;

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

            if (string.IsNullOrEmpty(SkinXml.MapData)) return;

            RegisteredProperties.AddRange(PropertyRepository.GetRegisteredProperties(this, SkinXml.MapData));
            _map = new MapHelper(SkinXml.Height, SkinXml.Width, SkinXml.DefaultMapZoom);
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

            if (_map != null)
            {
                var location = await PropertyRepository.GetProperty<string>(SkinXml.MapData, null);
                if (!string.IsNullOrEmpty(location) && !location.Equals("---"))
                {
                    var latitude = 0.0;
                    var longitude = 0.0;
                    try
                    {
                        var part = location.Split(',');
                        if (part.Count() == 4)
                        {
                            latitude = double.Parse(part[1], CultureInfo.InvariantCulture);
                            longitude = double.Parse(part[3], CultureInfo.InvariantCulture);
                            if (part[0].Equals("S")) latitude = -latitude;
                            if (part[2].Equals("W")) longitude = -longitude;
                        }
                    }
                    catch
                    {
                        // ignored
                    }

                    _map.Latitude = latitude;
                    _map.Longitude = longitude;

                    if (LoadMapImage())
                    {
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
        private bool LoadMapImage()
        {
            if (_map == null) return false;

            var url = _map.Url;
            var imageBytes = FileHelpers.ReadBytesFromFile(url);
            if (imageBytes == null || imageBytes.Length < 10) return false;

            Image = GUIImageManager.GetImage(imageBytes);
            return true;
        }

        private void ZoomInButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_map == null) return;

            if( _map.ZoomIn()) LoadMapImage();
        }

        private void ZoomOutButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_map == null) return;

            if( _map.ZoomOut()) LoadMapImage();
        }

       private void RoadmapToggleButton_OnClick(object sender, RoutedEventArgs e)
        {
          if (_map == null) return;

          if( _map.ToRoadmap()) LoadMapImage();
        }

       private void TerrainToggleButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_map == null) return;

            if( _map.ToTerrain()) LoadMapImage();
        }

       private void MoveUpButton_OnClick(object sender, RoutedEventArgs e)
       {
            if (_map == null) return;

            if( _map.MoveUp()) LoadMapImage();
       }

        private void MoveDownButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_map == null) return;
            if( _map.MoveDown()) LoadMapImage();
        }

        private void MoveLeftButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_map == null) return;
            if( _map.MoveLeft()) LoadMapImage();
        }

        private void MoveRightButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_map == null) return;
            LoadMapImage();
        }

        #endregion
    }
}
