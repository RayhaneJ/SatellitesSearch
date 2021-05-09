using System.Xml.Serialization;

namespace SatellitesTravelling.LaunchSitesModels
{
    [XmlRoot(ElementName = "kml", Namespace = "http://earth.google.com/kml/2.1")]
	public class Kml
	{
		[XmlElement(ElementName = "Document", Namespace = "http://earth.google.com/kml/2.1")]
		public Document Document { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}
}
