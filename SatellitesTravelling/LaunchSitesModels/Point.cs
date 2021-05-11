using System.Xml.Serialization;

namespace SatellitesTravelling.LaunchSitesModels
{
    [XmlRoot(ElementName = "Point", Namespace = "http://earth.google.com/kml/2.1")]
    public class Point
    {
        [XmlElement(ElementName = "coordinates", Namespace = "http://earth.google.com/kml/2.1")]
        public string Coordinates { get; set; }
    }
}
