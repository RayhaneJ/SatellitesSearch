using System.Collections.Generic;
using System.Xml.Serialization;

namespace SatellitesTravelling.LaunchSitesModels
{
    [XmlRoot(ElementName = "Folder", Namespace = "http://earth.google.com/kml/2.1")]
	public class Folder
	{
		[XmlElement(ElementName = "name", Namespace = "http://earth.google.com/kml/2.1")]
		public string Name { get; set; }
		[XmlElement(ElementName = "open", Namespace = "http://earth.google.com/kml/2.1")]
		public string Open { get; set; }
		[XmlElement(ElementName = "Placemark", Namespace = "http://earth.google.com/kml/2.1")]
		public List<Placemark> Placemark { get; set; }
	}
}
