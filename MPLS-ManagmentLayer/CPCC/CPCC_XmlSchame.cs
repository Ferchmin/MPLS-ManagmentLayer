using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ControlPlane
{
    [XmlRoot("CPCC_Configuration")]
    public class CPCC_XmlSchame
    {
        [XmlElement("localHostName")]
        public string XML_localHostName { get; set; }

        [XmlElement("localPcIpAddress")]
        public string XML_localPcIpAddress { get; set; }

        [XmlElement("domainNccIpAddress")]
        public string XML_domainNccIpAddress { get; set; }

        [XmlArray("FreeLabel-List")]
        [XmlArrayItem("Label", typeof(int))]
        public List<int> XML_FreeLabelList{ get; set; }
       

        public CPCC_XmlSchame()
        {
            XML_FreeLabelList = new List<int>();
          
        }
    }


   
}
