using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Location
    {
        public Location(string buildingName, string floorName, string nodeId)
        {
            this.buildingName = buildingName;
            this.floorName = floorName;
            this.nodeId = nodeId;
        }

        public Location(string locationInfo)
        {
            String[] locationInfoArray = locationInfo.Split(' ');
            this.buildingName = locationInfoArray[0];
            this.floorName = locationInfoArray[1];
            this.nodeId = locationInfoArray[2];
        }

        public string buildingName
        {
            get; set;
        }

        public string floorName
        {
            get; set;
        }

        public string nodeId
        {
            get; set;
        }

        override
        public string ToString()
        {
            return this.buildingName + "-" + floorName + "-" + nodeId; 
        }
    }
}
