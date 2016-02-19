using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class SameFloorAlgorithm
    {
        private Floor floor;
        private Node startNode, endNode;
        private List<Node> visitedNodeList = new List<Node>();
        private Queue<Node> toBeVisitedNodeQueue = new Queue<Node>();
        private const float DIJKSTRA_COEFFICIENT = 1;
        private const float HEURISTIC_COEFFICIENT = 0;

        public SameFloorAlgorithm(Floor floor, Node startNode, Node endNode)
        {
            this.floor = floor;
            this.startNode = startNode;
            this.endNode = endNode;
        }

        public void initializeNodes()
        {
            foreach (Node node in floor.getNodeList())
            {
                node.isVisited = false;
                node.distance = float.MaxValue;//set all distances to the max value first  
                node.heuristicDistance = HEURISTIC_COEFFICIENT * getDistance(node, endNode); //set all heuristic distance              
            }
            startNode.dijkstraDistance = 0;
            startNode.updateDistance();
            toBeVisitedNodeQueue.Enqueue(startNode);
        }

        public float getDistance(Node node1, Node node2)
        {
            int xDist = Math.Abs(node1.getX() - node2.getX());
            int yDist = Math.Abs(node1.getY() - node2.getY());
            float distance = (float)Math.Sqrt(Math.Pow(xDist, 2) + Math.Pow(yDist, 2));
            return distance;
        }

        public void visit(Node node)
        {
            //visit the node
            node.isVisited = true;
            visitedNodeList.Add(node);

            List<Node> unvisitedNeighborList = new List<Node>();//a list of unvisited neighbor
            foreach (Node neighbor in node.getNeighbors())
            {
                if (!neighbor.isVisited)//if the neighbor is not visited
                {
                    float tempDijkstraDistance = node.dijkstraDistance + getDistance(node, neighbor);
                    float tempDistance = tempDijkstraDistance + neighbor.heuristicDistance;//the temp path distance via node
                    if (tempDistance < neighbor.distance)//if this is a better path
                    {
                        neighbor.dijkstraDistance = tempDijkstraDistance;//update the dijkstraDistance and total distance
                        neighbor.updateDistance();
                        neighbor.previousNode = node;
                    }
                    unvisitedNeighborList.Add(neighbor);
                }
            }
            List<Node> sortedNeighborList = unvisitedNeighborList.OrderBy(o => o.distance).ToList();//sort these neighbors ascendingly by distance property
            foreach (Node neighbor in sortedNeighborList)
            {
                toBeVisitedNodeQueue.Enqueue(neighbor);//add thses unvisited neighbors to the queue, order by distance
            }
        }

        public Path runAlgorithm()
        {
            initializeNodes();
            while (!endNode.isVisited)
            {
                Node currentNode = toBeVisitedNodeQueue.Dequeue();
                visit(currentNode);
            }
            return getPath(endNode);                     
        }

        public Path getPath(Node node)
        {
            Path path = new Path();
            path.buildingName = floor.buildingName;
            path.floorName = floor.floorName;
            path.distance = node.distance;
            //get an array of node id on the best path
            int[] nodeIdArray = new int[floor.getNodeList().Count];
            for (int i = 0; i < nodeIdArray.Length; i++)
            {
                nodeIdArray[i] = -1; //all elements are initialized to -1
            }
            int counter = 0;
            while (node.getId() != startNode.getId())
            {
                nodeIdArray[counter] = node.getId();
                node = node.previousNode;
                counter++;
            }
            nodeIdArray[counter] = startNode.getId();

            //print these IDs in reverse order
            string pathString = path.buildingName + "-" + path.floorName + "(length: " + path.distance + ")" +  ": ";
            int pathLength = nodeIdArray.Count(e => e != -1);
            for (int i = pathLength - 1; i >= 0; i--)
            {
                if (i == 0)
                {
                    pathString += nodeIdArray[0];
                }
                else
                {
                    pathString += nodeIdArray[i] + "->";
                }                
            }
            path.path = pathString;
            resetNodes();
            return path;
        }

        public void resetNodes()
        {
            foreach (Node node in floor.getNodeList())
            {
                node.reset();
            }
        }
    }
}
