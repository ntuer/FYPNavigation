using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Floor
    {
        private List<Node> nodeList = new List<Node>();//all nodes are in this list
        private List<Node> elevatorList = new List<Node>();
        private List<Node> connectorList = new List<Node>();
        private List<Edge> edgeList = new List<Edge>();//all edges
        private List<string> connectorNameList = new List<string>();//all connector names on this floor

        public string id
        {
            get; set;
        }

        public int width
        {
            get; set;
        }

        public int height
        {
            get; set;
        }

        public string buildingName
        {
            get; set;
        }

        public string floorName
        {
            get; set;
        }


        public Floor(string id)//constructor
        {
            this.id = id;
            this.hasConnector = false;           
        }

        public bool hasConnector
        {
            get; set;
        }

        public void addNode(Node node)
        {
            nodeList.Add(node);
            if (node.pointType == Node.POINT_TYPE.Elevator)
            {
                elevatorList.Add(node);
            }
            else if (node.pointType == Node.POINT_TYPE.Connector)
            {
                connectorList.Add(node);
            }
        }

        public List<Node> getNodeList()
        {
            return this.nodeList;
        }

        public void addEdge(Edge edge)
        {
            edgeList.Add(edge);
        }

        public List<Edge> getEdgeList()
        {
            return this.edgeList;
        }

        public List<Node> getElevatorList()
        {
            return this.elevatorList;
        }

        public List<Node> getConnectorList()
        {
            return this.connectorList;
        }

        public void addConnectorName(string connectorName)
        {
            if (!this.connectorNameList.Contains(connectorName))
            {
                this.connectorNameList.Add(connectorName);
            }
        }

        public List<string> getConnectorNameList()
        {
            return this.connectorNameList;
        }

    }
}
