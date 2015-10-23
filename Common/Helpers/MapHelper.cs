using System;
using System.Globalization;

namespace Common.Helpers
{
    public class MapHelper
    {

        // map related variables
        private int _mapHeight;
        private int _mapWidth;
        private string _googleApiKey;
        private int _zoom ;

       public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string MapType { get; set; }

        public string GoogleApiKey
        {
            get { return string.IsNullOrWhiteSpace(_googleApiKey) ? string.Empty : string.Format("&key={0}", _googleApiKey); }
            set { _googleApiKey = value; }
        }

        public string Url
        {
            get
            {
                if (!(Math.Abs(Latitude) > 0.0000001) || !(Math.Abs(Longitude) > 0.0000001)) return string.Empty;

                var location = Latitude.ToString("F6", CultureInfo.InvariantCulture) + "," +
                               Longitude.ToString("F6", CultureInfo.InvariantCulture);

                return string.Format("https://maps.googleapis.com/maps/api/staticmap?center={0}&size={1}x{2}&markers=size:mid%7Ccolor:red%7C{0}&zoom={3}&maptype={4}&sensor=false{5}",
                    location, _mapWidth, _mapHeight, _zoom, MapType, GoogleApiKey);
               
            }
        }

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MapHelper"/> class.
        /// </summary>
        public MapHelper(int height, int width, int defaultZoom)
        {
            _mapHeight = (height > 640) ? 640 : height;
            _mapWidth = (width > 640) ? 640 : width;

            _zoom = defaultZoom;
            if (_zoom < 0) _zoom = 0;
            if (_zoom > 21) _zoom = 21;
            MapType = "roadmap";
        }

        #endregion

        #region Methods

        // Move map right
        public bool MoveRight()
        {
            if (!(Longitude < 179)) return false;

            Longitude += ShiftMap();
            return true;
        }

        // Move map left
        public bool MoveLeft()
        {
            if (!(Longitude > -179)) return false;

            Longitude -= ShiftMap();
            return true;
        }

         // Move map up
        public bool MoveUp()
        {
            // Use 88 to avoid values beyond 90 degrees of lat.
            if (!(Latitude < 88)) return false;

            Latitude += ShiftMap();
            return true;
        }

         // Move map down
        public bool MoveDown()
        {
            // Use 88 to avoid values beyond 90 degrees of lat.
            if (!(Latitude > -88)) return false;

            Latitude -= ShiftMap();
            return true;
        }

       // move the map center by a defined distance dependent on zoom
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

        // Increment Zoom
        public bool ZoomIn()
        {
           if (_zoom >= 21) return false;
           _zoom++;
           return true;
        }

        // Decrement Zoom
        public bool ZoomOut()
        {
            if (_zoom <= 0) return false;
            _zoom--;
            return true;
        }

        // switch map to roadmap
        public bool ToRoadmap()
        {
            if (MapType.Equals("roadmap")) return false;
            MapType = "terrain";
            return true;
        }

        // switch map to terrain
        public bool ToTerrain()
        {
            if (MapType.Equals("terrain")) return false;
            MapType = "roadmap";
            return true;
        }

        #endregion
    }

}
