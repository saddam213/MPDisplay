using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GUISkinFramework.Property
{
    public class XmlPropertyInfo 
    {
        private List<XmlProperty> _properties = new List<XmlProperty>();
        private ObservableCollection<XmlProperty> _allPropertes;

        public List<XmlProperty> Properties
        {
            get { return _properties; }
            set { _properties = value; }
        }


        
        [XmlIgnore]
        public ObservableCollection<XmlProperty> AllProperties
        {
            get
            {
                if (_allPropertes == null)
                {
                    _allPropertes = new ObservableCollection<XmlProperty>(InternalPropertiesProperties.Concat(_properties));
                }
                return _allPropertes; 
            } 
        }

        [XmlIgnore]
        public IEnumerable<XmlProperty> InternalPropertiesProperties
        {
            get
            {
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MPD.Guide.Group", DesignerValue = "All Channels" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MPD.Guide.Channel", DesignerValue = "TV ONE" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MPD.Guide.ChannelThumb" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MPD.Guide.Program", DesignerValue = "The Walking Dead" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MPD.Guide.Description", DesignerValue = "Police officer Rick Grimes leads a group of survivors in a world overrun by zombies. " };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MPD.Guide.EndTime", DesignerValue = DateTime.Now.AddMinutes(30).ToString("hh:mm d/M") };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MPD.Guide.StartTime", DesignerValue = DateTime.Now.ToString("hh:mm d/M") };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MPD.ListControl.Count", DesignerValue = "154" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MPD.ListControl.Selecteditem", DesignerValue = "Selecteditem" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MPD.ListControl.Selecteditem2", DesignerValue = "Selecteditem2" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MPD.ListControl.Selecteditem3", DesignerValue = "Selecteditem3" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MPD.ListControl.Selectedindex", DesignerValue = "3" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MPD.ListControl.Selectedthumb"  };

                yield return new XmlProperty { IsInternal = true, SkinTag = "#MPD.SystemInfo.Number.PhysicalPercent", DesignerValue = "48.49", PropertyType = XmlPropertyType.Number };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MPD.SystemInfo.Number.VirtualPercent", DesignerValue = "80.92", PropertyType = XmlPropertyType.Number };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MPD.SystemInfo.Number.CPU", DesignerValue = "27.12", PropertyType = XmlPropertyType.Number };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MPD.SystemInfo.Label.PhysicalPercent", DesignerValue = "48.49 %" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MPD.SystemInfo.Label.VirtualPercent", DesignerValue = "80.92 %" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MPD.SystemInfo.Label.CPU", DesignerValue = "27.12 %" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MPD.SystemInfo.Label.Date", DesignerValue = "Thursday, November 14, 2013" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MPD.SystemInfo.Label.Date2", DesignerValue = "Thu 14 Nov" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MPD.SystemInfo.Label.Date3", DesignerValue = "Thursday 14 November" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MPD.SystemInfo.Label.Date4", DesignerValue = "11/14/2013" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MPD.SystemInfo.Label.Time", DesignerValue = "10:30 PM" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MPD.SystemInfo.Label.Time2", DesignerValue = "22:30" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MPD.SystemInfo.Label.Time3", DesignerValue = "10:30:45 PM" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MPD.SystemInfo.Label.Time4", DesignerValue = "22:30:45" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MPD.SystemInfo.Label.DateMonth", DesignerValue = "11" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MPD.SystemInfo.Label.DateMonthShort", DesignerValue = "Nov" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MPD.SystemInfo.Label.DateMonthLong", DesignerValue = "November" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MPD.SystemInfo.Label.DateDay", DesignerValue = "14" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MPD.SystemInfo.Label.DateDayShort", DesignerValue = "Thu" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MPD.SystemInfo.Label.DateDayLong", DesignerValue = "Thursday" }; 
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MPD.SystemInfo.Label.DateYear", DesignerValue = "2013" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MPD.SystemInfo.Label.DateYearShort", DesignerValue = "13" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MPD.SystemInfo.Label.OSFullName", DesignerValue = "Microsoft Windows 7 Ultimate" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MPD.SystemInfo.Label.OSPlatform", DesignerValue = "Win32NT" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MPD.SystemInfo.Label.OSVersion", DesignerValue = "6.1.7601.65536" };

                yield return new XmlProperty { IsInternal = true, SkinTag = "#MP.SystemInfo.Number.PhysicalPercent", DesignerValue = "28.75", PropertyType = XmlPropertyType.Number };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MP.SystemInfo.Number.VirtualPercent", DesignerValue = "40.25", PropertyType = XmlPropertyType.Number };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MP.SystemInfo.Number.CPU", DesignerValue = "5.56", PropertyType = XmlPropertyType.Number };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MP.SystemInfo.Label.PhysicalPercent", DesignerValue = "28.75 %" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MP.SystemInfo.Label.VirtualPercent", DesignerValue = "40.25 %" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MP.SystemInfo.Label.CPU", DesignerValue = "5.56 %" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MP.SystemInfo.Label.Date", DesignerValue = "Friday, November 15, 2013" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MP.SystemInfo.Label.Date2", DesignerValue = "Fri 15 Nov" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MP.SystemInfo.Label.Date3", DesignerValue = "Friday 15 November" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MP.SystemInfo.Label.Date4", DesignerValue = "11/15/2013" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MP.SystemInfo.Label.Time", DesignerValue = "11:30 PM" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MP.SystemInfo.Label.Time2", DesignerValue = "23:30" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MP.SystemInfo.Label.Time3", DesignerValue = "11:30:45 PM" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MP.SystemInfo.Label.Time4", DesignerValue = "23:30:45" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MP.SystemInfo.Label.DateMonth", DesignerValue = "11" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MP.SystemInfo.Label.DateMonthShort", DesignerValue = "Nov" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MP.SystemInfo.Label.DateMonthLong", DesignerValue = "November" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MP.SystemInfo.Label.DateDay", DesignerValue = "15" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MP.SystemInfo.Label.DateDayShort", DesignerValue = "Fri" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MP.SystemInfo.Label.DateDayLong", DesignerValue = "Friday" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MP.SystemInfo.Label.DateYear", DesignerValue = "2013" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MP.SystemInfo.Label.DateYearShort", DesignerValue = "13" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MP.SystemInfo.Label.OSFullName", DesignerValue = "Microsoft Windows 8" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MP.SystemInfo.Label.OSPlatform", DesignerValue = "Win32NT" };
                yield return new XmlProperty { IsInternal = true, SkinTag = "#MP.SystemInfo.Label.OSVersion", DesignerValue = "6.2.7601.65536" };


                Random ran = new Random();
                var letters = new string[] { "C", "D", "E", "F", "G", "H", "I", "J", "K", "L" };
               
                for (int i = 0; i < 4; i++)
                {
                    double size = Math.Round(((double)(ran.Next(50, 900))) + ran.NextDouble(),2);
                    double size2 = Math.Round(((double)(ran.Next(50, 900))) + ran.NextDouble(),2);
                    double free = Math.Round(((double)(ran.Next(10, (int)size))) + ran.NextDouble(),2);
                    double free2 = Math.Round(((double)(ran.Next(10, (int)size2))) + ran.NextDouble(), 2);
                    double percentFree = Math.Round(100 * free / size, 2);
                    double percentFree2 = Math.Round(100 * free2 / size2, 2);

                    yield return new XmlProperty { IsInternal = true, SkinTag = string.Format("#MPD.SystemInfo.Label.Drive{0}.Name", i), DesignerValue = letters[i] + ":\\" };
                    yield return new XmlProperty { IsInternal = true, SkinTag = string.Format("#MPD.SystemInfo.Label.Drive{0}.VolumeLabel", i), DesignerValue = "Drive " + i };
                    yield return new XmlProperty { IsInternal = true, SkinTag = string.Format("#MPD.SystemInfo.Label.Drive{0}.TotalSpace", i), DesignerValue = size+"Gb" };
                    yield return new XmlProperty { IsInternal = true, SkinTag = string.Format("#MPD.SystemInfo.Label.Drive{0}.FreeSpace", i), DesignerValue = free+"Gb" };
                    yield return new XmlProperty { IsInternal = true, SkinTag = string.Format("#MPD.SystemInfo.Label.Drive{0}.PercentFree", i), DesignerValue = percentFree+" %" };
                    yield return new XmlProperty { IsInternal = true, SkinTag = string.Format("#MPD.SystemInfo.Number.Drive{0}.PercentFree", i), DesignerValue = percentFree.ToString(), PropertyType = XmlPropertyType.Number };

                    yield return new XmlProperty { IsInternal = true, SkinTag = string.Format("#MP.SystemInfo.Label.Drive{0}.Name", i), DesignerValue = letters[i] + ":\\" };
                    yield return new XmlProperty { IsInternal = true, SkinTag = string.Format("#MP.SystemInfo.Label.Drive{0}.VolumeLabel", i), DesignerValue = "Drive " + i };
                    yield return new XmlProperty { IsInternal = true, SkinTag = string.Format("#MP.SystemInfo.Label.Drive{0}.TotalSpace", i), DesignerValue = size2+"Gb" };
                    yield return new XmlProperty { IsInternal = true, SkinTag = string.Format("#MP.SystemInfo.Label.Drive{0}.FreeSpace", i), DesignerValue = free2+"Gb" };
                    yield return new XmlProperty { IsInternal = true, SkinTag = string.Format("#MP.SystemInfo.Label.Drive{0}.PercentFree", i), DesignerValue = percentFree2+" %" };
                    yield return new XmlProperty { IsInternal = true, SkinTag = string.Format("#MP.SystemInfo.Number.Drive{0}.PercentFree", i), DesignerValue = percentFree2.ToString(), PropertyType = XmlPropertyType.Number };
                }
            }
        }
    }
}
