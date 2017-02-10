using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Cruise.DashboardBroker.Config {

    [Serializable]
    [XmlType("server")]
    public class Server {
        [XmlAttribute("name")]
        public string Name;
        [XmlAttribute("url")]
        public string Url;
        [XmlAttribute("allowForceBuild")]
        public bool AllowForceBuild;
        [XmlAttribute("allowStartStopBuild")]
        public bool AllowStartStopBuild;
        [XmlAttribute("backwardsCompatible")]
        public bool BackwardsCompatible;
    }

    [Serializable]
    public class RemoteServices{
        [XmlArray("servers")]
        public Server[] Servers;
    }

    [Serializable]
    [XmlRoot("dashboard")]
    public class Dashboard {
        [XmlElement("remoteServices")]
        public RemoteServices RemoteServices;
    }
}
