using System.Xml.Serialization;

namespace SatellitesTravelling.LaunchSitesModels
{
    [XmlRoot(ElementName = "Placemark", Namespace = "http://earth.google.com/kml/2.1")]
    public class Placemark
    {
        [XmlElement(ElementName = "name", Namespace = "http://earth.google.com/kml/2.1")]
        public string Name { get; set; }
        [XmlElement(ElementName = "Point", Namespace = "http://earth.google.com/kml/2.1")]
        public Point Point { get; set; }
        [XmlElement(ElementName = "Snippet", Namespace = "http://earth.google.com/kml/2.1")]
        public string Snippet { get; set; }
        [XmlElement(ElementName = "description", Namespace = "http://earth.google.com/kml/2.1")]
        public string Description { get; set; }
    }
}
