using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Edge
    {
        private double length;

        public Edge(Node node1, Node node2)
        {
            this.node1 = node1;
            this.node2 = node2;
        }

        public Node node2
        {
            get; set;
        }

        public Node node1
        {
            get; set;
        }

        public int id
        {
            get; set;
        }

        public double getLength()
        {
            int xDist = Math.Abs(node1.getX() - node2.getX());
            int yDist = Math.Abs(node1.getY() - node2.getY());
            length = (int)Math.Sqrt(Math.Pow(xDist, 2) + Math.Pow(yDist, 2));
            return length;
        }
    }
}
