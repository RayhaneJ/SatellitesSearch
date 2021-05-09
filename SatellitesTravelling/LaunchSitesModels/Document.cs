using System.Collections.Generic;
using System.Xml.Serialization;

namespace SatellitesTravelling.LaunchSitesModels
{
    [XmlRoot(ElementName = "Document", Namespace = "http://earth.google.com/kml/2.1")]
	public class Document
	{
		[XmlElement(ElementName = "name", Namespace = "http://earth.google.com/kml/2.1")]
		public string Name { get; set; }
		[XmlElement(ElementName = "open", Namespace = "http://earth.google.com/kml/2.1")]
		public string Open { get; set; }
		[XmlElement(ElementName = "Folder", Namespace = "http://earth.google.com/kml/2.1")]
		public List<Folder> Folder { get; set; }
	}
}
