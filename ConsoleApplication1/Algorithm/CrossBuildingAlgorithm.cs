using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class CrossBuildingAlgorithm
    {
        private Floor startFloor, endFloor;
        private Node startNode, endNode;
        private List<Floor> floors;
        List<Floor> startConnetorFloors, endConnectorFloors;

        public CrossBuildingAlgorithm(Floor startFloor, Node startNode, Floor endFloor, Node endNode, List<Floor> floors)
        {
            this.startFloor = startFloor;
            this.startNode = startNode;
            this.endFloor = endFloor;
            this.endNode = endNode;
            this.floors = floors;
            this.startConnetorFloors = new List<Floor>();
            this.endConnectorFloors = new List<Floor>();
        }

        public void getConnectorFloors()
        {
            foreach (Floor floor in floors)
            {
                if (floor.hasConnector)// it has connectors
                {
                    if (floor.getConnectorNameList().Contains(endFloor.buildingName))//if this floor has connector to destination building
                    {
                        startConnetorFloors.Add(floor);
                    }
                    else if (floor.getConnectorNameList().Contains(startFloor.buildingName))//if the floor has connector to original building
                    {
                        endConnectorFloors.Add(floor);
                    }
                }
            }

        }

        public Path runAlgorithm()
        {
            getConnectorFloors();
            List<Path> pathList = new List<Path>();//a list of completed path

            foreach (Floor startConnectorFloor in startConnetorFloors)
            {
                Path startPath = null, endPath = null;
                //if this is the startFloor
                if (startConnectorFloor.Equals(startFloor))
                {
                    //get start connector
                    Node startConnector = null;
                    foreach (Node node in startFloor.getConnectorList())
                    {
                        if (node.getConnectorName() == endFloor.buildingName)
                        {
                            startConnector = node;
                            break;
                        }
                    }
                    //get the path using same floor algorithm
                    SameFloorAlgorithm startNavigationAlgorithm = new SameFloorAlgorithm(startFloor, startNode, startConnector);
                    startPath = startNavigationAlgorithm.runAlgorithm();
                }
                else//if this is not the startFloor
                {
                    //get the connector in the startConnectorFloor
                    Node startConnectorNode = null;
                    foreach (Node node in startConnectorFloor.getConnectorList())
                    {
                        if (node.getConnectorName() == endFloor.buildingName)//if this is the node that connects to the destination building
                        {
                            startConnectorNode = node;
                            break;
                        }
                    }

                    //get the path from startNode to connector across floors
                    CrossFloorAlgorithm startNavigationAlgorithm = new CrossFloorAlgorithm(startFloor, startNode, startConnectorFloor, startConnectorNode);
                    startPath = startNavigationAlgorithm.runAlgorithm();
                }
                

                //get the corresponding end floor with connectors
                Floor endConnectorFloor = null;
                foreach (Floor floor in endConnectorFloors)
                {
                    if (floor.floorName == startConnectorFloor.floorName)
                    {
                        endConnectorFloor = floor;
                        break;
                    }
                }

                //if this is the endFloor
                if (endConnectorFloor.Equals(endFloor))
                {
                    //get end connector
                    Node endConnector = null;
                    foreach (Node node in endFloor.getConnectorList())
                    {
                        if (node.getConnectorName() == startFloor.buildingName)
                        {
                            endConnector = node;
                            break;
                        }
                    }
                    SameFloorAlgorithm endNavigationAlgorithm = new SameFloorAlgorithm(endFloor, endConnector, endNode);
                    endPath = endNavigationAlgorithm.runAlgorithm();
                }
                else
                {
                    //get the connector in the endConnectorFloor
                    Node endConnectorNode = null;
                    foreach (Node node in endConnectorFloor.getConnectorList())
                    {
                        if (node.getConnectorName() == startFloor.buildingName)//if this is the node that connects to the start building
                        {
                            endConnectorNode = node;
                            break;
                        }
                    }

                    //get the path from connector node to end node
                    CrossFloorAlgorithm endNavigationAlgorithm = new CrossFloorAlgorithm(endConnectorFloor, endConnectorNode, endFloor, endNode);
                    endPath = endNavigationAlgorithm.runAlgorithm();
                }
                Path finalPath = startPath.concatenate(endPath);
                //finalPath.print();
                pathList.Add(finalPath);
            }

            List<Path> sortedPathList = pathList.OrderBy(o => o.distance).ToList();//sort these paths ascendingly by distance property
            foreach (Path path in sortedPathList)
            {
                path.print();
            }
            return sortedPathList[0];//return the first path with min distance
        }


    }
}
