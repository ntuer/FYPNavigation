using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Floor
    {
        private List<Node> nodeList;//all nodes are in this list
        private List<Node> elevatorList;
        private List<Node> connectorList;
        private List<Edge> edgeList;//all edges

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


        public Floor(string id)
        {
            this.id = id;
        }

        public void addNode(Node node)
        {
            nodeList.Add(node);
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

    }
}
