using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ConsoleApplication1
{
    class FYPNavigation
    {
        private Location originalLocation;
        private Location destinationLocation;
        private readonly string path = "C:\\Users\\owner\\Desktop\\FYP\\Mapping Software 15 OCT";
        List<string> usefulOriginalFiles = new List<string>();
        List<string> usefulDestinationFiles = new List<string>();
        List<Floor> floors = new List<Floor>();

        public void readInputs()
        {
            Console.WriteLine("Input the original location: ");
            string originalLocationString = Console.ReadLine();     //The format is buildingName + " " + floorName + " " + nodeID       
            Console.WriteLine("Input the destination location: ");
            string destinationLocationString = Console.ReadLine();

            if (originalLocationString != null)
            {
                originalLocation = new Location(originalLocationString);
            }

            if (destinationLocationString != null)
            {
                destinationLocation = new Location(destinationLocationString);
            }
        }

        public void readFiles()
        {
            if (originalLocation.buildingName == destinationLocation.buildingName)//same building navigation
            {
                string[] originalBuildingFiles = Directory.GetFiles(path, originalLocation.buildingName + "*");//get the file names with the same building name
                loadUsefulFiles(originalBuildingFiles, destinationLocation, usefulOriginalFiles);              
                readUsefulFiles(usefulOriginalFiles);
            }
            else//cross-building navigation
            {
                string[] originalBuildingFiles = Directory.GetFiles(path, originalLocation.buildingName + "*");//get the file names with the same building name
                string[] destinationBuildingFiles = Directory.GetFiles(path, destinationLocation.buildingName + "*");
                loadUsefulFiles(originalBuildingFiles, destinationLocation, usefulOriginalFiles);
                loadUsefulFiles(destinationBuildingFiles, originalLocation, usefulDestinationFiles);//the testing result for usefulOriginalFiles is positive                
                readUsefulFiles(usefulOriginalFiles);
                readUsefulFiles(usefulDestinationFiles);//results are stored in the floor list
            }
            printFloorInfo();//debug purpose to test if all nodes are read successfully
        }


        public void loadUsefulFiles(string[] filesToSelect, Location selectionCritiriaByDestination, List<string> selectionResultFiles)
        {
            foreach (string file in filesToSelect)
            {
                XmlDocument confFile = new XmlDocument();
                confFile.Load(file);

                //get building and floor info
                XmlNode buildingTag = confFile.SelectSingleNode("/NTU/building");
                XmlNode floorTag = confFile.SelectSingleNode("/NTU/floor");
                string buildingName = buildingTag.InnerText;
                string floorName = floorTag.InnerText;

                //if the file either contains the original location or the destination location, this file is useful in the later navigation algorithm
                if ((buildingName == originalLocation.buildingName && floorName == originalLocation.floorName) || (buildingName == destinationLocation.buildingName && floorName == destinationLocation.floorName))
                {
                    if (!selectionResultFiles.Contains(file))
                    {
                        selectionResultFiles.Add(file);
                    }
                    
                }
                else
                {
                    XmlNode NTUTag = confFile.DocumentElement.SelectSingleNode("/NTU");
                    foreach (XmlNode pointNode in NTUTag.SelectNodes("/NTU/p"))
                    {
                        //if the file contains connectors that are connected to the other building, it is useful
                        if (pointNode.Attributes["type"].InnerText == "Connector" && pointNode.Attributes["typeValue"].InnerText == selectionCritiriaByDestination.buildingName)
                        {
                            if (!selectionResultFiles.Contains(file))
                            {
                                selectionResultFiles.Add(file);
                            }
                            break;
                        }
                    }
                }          
            }
        }

        public void readUsefulFiles(List<string> filesToRead)
        {
            foreach (string file in filesToRead)
            {
                string floorName = file.Split('\\')[file.Split('\\').Length - 1];
                floorName = floorName.Split('.')[0];
                Floor floor = new Floor(floorName);//floor id, same as file name without path
                XmlDocument confFile = new XmlDocument();
                List<List<int>> nodeNeighborsList = new List<List<int>>();

                //load the file
                confFile.Load(file);

                //read tags
                XmlNode NTUTag = confFile.DocumentElement.SelectSingleNode("/NTU");

                //get building and floor info
                XmlNode buildingTag = confFile.SelectSingleNode("/NTU/building");
                XmlNode floorTag = confFile.SelectSingleNode("/NTU/floor");
                floor.buildingName = buildingTag.InnerText;
                floor.floorName = floorTag.InnerText;

                //get the map size info
                XmlNode widthTag = confFile.SelectSingleNode("/NTU/width");
                XmlNode heightTag = confFile.SelectSingleNode("/NTU/height");
                floor.width = int.Parse(widthTag.InnerText);
                floor.height = int.Parse(heightTag.InnerText);

                //create all node objects in the file and assign necessary properties to each node
                foreach (XmlNode pointNode in NTUTag.SelectNodes("/NTU/p"))
                {
                    List<int> neighborIDList = new List<int>();//a list of nodeID of neighbors

                    //read the node info
                    int id = Convert.ToInt32(pointNode.Attributes["id"].InnerText);
                    string type = pointNode.Attributes["type"].InnerText;
                    string typeValue = pointNode.Attributes["typeValue"].InnerText;
                    int x = Convert.ToInt32(pointNode.Attributes["x"].InnerText);
                    int y = Convert.ToInt32(pointNode.Attributes["y"].InnerText);

                    Node node = new Node(id, x, y);//a new node

                    if (type == Node.POINT_TYPE.Normal.ToString())
                    {
                        node.pointType = Node.POINT_TYPE.Normal;
                    }
                    else if (type == Node.POINT_TYPE.Elevator.ToString())
                    {
                        node.pointType = Node.POINT_TYPE.Elevator;
                        node.setElevatorGroupNum(int.Parse(typeValue));
                    }
                    else if (type == Node.POINT_TYPE.Connector.ToString())
                    {
                        node.pointType = Node.POINT_TYPE.Connector;
                        node.setConnectorName(typeValue);
                        floor.hasConnector = true;
                        floor.addConnectorName(typeValue);
                    }

                    floor.addNode(node);//add to the nodeList of the floor

                    //get its neighbors info and store into a composite list
                    string connect = pointNode.Attributes["connect"].InnerText;
                    if (connect != "")//has neighbors
                    {
                        if (connect.Contains(','))//more than one neighbor
                        {
                            string[] neighbors = connect.Split(',');
                            for (int i = 0; i < neighbors.Length; i++)
                            {
                                int neighborID = Convert.ToInt32(neighbors[i]);
                                neighborIDList.Add(neighborID);
                            }
                            nodeNeighborsList.Add(neighborIDList);
                        }
                        else//only one neighbor
                        {
                            neighborIDList.Add(Convert.ToInt32(connect));
                            nodeNeighborsList.Add(neighborIDList);
                        }

                    }
                    else//no neighbor
                    {
                        neighborIDList.Add(-1);//-1 means no neighbor
                        nodeNeighborsList.Add(neighborIDList);
                    }

                }//this ends the loop and all node objects should have been created.

                //add the neighbors for each node in the floor.getNodeList()
                for (int i = 0; i < nodeNeighborsList.Count; i++)
                {
                    for (int j = 0; j < nodeNeighborsList[i].Count; j++)
                    {
                        int neighborID = nodeNeighborsList[i][j];
                        if (neighborID != -1)
                        {
                            setNeighbors(floor, floor.getNodeList()[i], floor.getNodeList()[neighborID]);
                        }
                    }
                }

                //add the floor to the floor list
                floors.Add(floor);
            }
        }

        //set the node1 and node2 on floor as neighbors
        public void setNeighbors(Floor floor, Node node1, Node node2)
        {
            if (!node1.getNeighbors().Contains(node2))//if they are not neighbors yet
            {
                node1.addNeighbor(node2);
                node2.addNeighbor(node1);
                Edge edge = new Edge(node1, node2);
                floor.addEdge(edge);
            }
            
        }

        public void printFloorInfo()
        {
            Console.WriteLine("Original Location:" + originalLocation.buildingName + "-" + originalLocation.floorName + "-" + originalLocation.nodeId);
            Console.WriteLine("Destination Location:" + destinationLocation.buildingName + "-" + destinationLocation.floorName + "-" + destinationLocation.nodeId);
            foreach (Floor floor in floors)
            {
                Console.WriteLine("Floor ID: " + floor.id);
                Console.WriteLine("Building Name: " + floor.buildingName);
                Console.WriteLine("Floor Name: " + floor.floorName);
                Console.WriteLine("Floor Wdith: " + floor.width);
                Console.WriteLine("Floor Height: " + floor.height);
                Console.WriteLine("Floor Nodes: ");
                foreach (Node node in floor.getNodeList())
                {
                    Console.WriteLine("\tNode ID: " + node.getId());
                    Console.WriteLine("\tNode X-coordinate: " + node.getX());
                    Console.WriteLine("\tNode Y-coordinate: " + node.getY());
                    Console.WriteLine("\tNode Type: " + node.pointType);
                    Console.WriteLine("\tNode Neighbors: ");
                    foreach (Node neighbor in node.getNeighbors())
                    {
                        Console.Write("\t" + neighbor.getId());
                    }
                    Console.WriteLine("");
                }

            }
        }

        public void startNavigation()
        {
            Node startNode = null, endNode = null;
            if (floors.Count == 1)//same floor navigation-----SameFloorAlgorithm
            {
                foreach (Node node in floors[0].getNodeList())
                {
                    if (node.getId() == int.Parse(originalLocation.nodeId))
                    {
                        startNode = node;
                    }
                    else if (node.getId() == int.Parse(destinationLocation.nodeId))
                    {
                        endNode = node;
                    }
                }
                if (startNode != null && endNode != null)
                {
                    SameFloorAlgorithm navigationAlgorithm = new SameFloorAlgorithm(floors[0], startNode, endNode);
                    navigationAlgorithm.runAlgorithm().print();
                }
            }
            else if (originalLocation.buildingName == destinationLocation.buildingName && floors.Count == 2)//same building, cross-floor navigation--------CrossFloorAlgorithm
            {
                Floor originalFloor = null, destinationFloor = null;
                if (floors[0].floorName == originalLocation.floorName)//floor[0] is the original floor
                {
                    originalFloor = floors[0];
                    destinationFloor = floors[1];
                }
                else
                {
                    originalFloor = floors[1];
                    destinationFloor = floors[0];
                }


                foreach (Node node in originalFloor.getNodeList())//get startNode from original floor
                {
                    if (node.getId() == int.Parse(originalLocation.nodeId))
                    {
                        startNode = node;
                    }
                }
                foreach (Node node in destinationFloor.getNodeList())//get endNode from destination floor
                {
                    if (node.getId() == int.Parse(destinationLocation.nodeId))
                    {
                        endNode = node;
                    }
                }
                if (startNode != null && endNode != null)
                {
                    CrossFloorAlgorithm navigationAlgorithm = new CrossFloorAlgorithm(originalFloor, startNode, destinationFloor, endNode);
                    navigationAlgorithm.runAlgorithm().print();
                }
            }
            else if (originalLocation.buildingName != destinationLocation.buildingName)//cross-building navigation ------------CrossBuildingAlgorithm
            {
                Floor startFloor = null, endFloor = null;

                //get startFloor and endFloor
                foreach (Floor floor in floors)
                {
                    if (floor.buildingName == originalLocation.buildingName && floor.floorName == originalLocation.floorName)
                    {
                        startFloor = floor;
                        break;
                    }
                }

                foreach (Floor floor in floors)
                {
                    if (floor.buildingName == destinationLocation.buildingName && floor.floorName == destinationLocation.floorName)
                    {
                        endFloor = floor;
                        break;
                    }
                }
                foreach (Node node in startFloor.getNodeList())//get startNode from original floor
                {
                    if (node.getId() == int.Parse(originalLocation.nodeId))
                    {
                        startNode = node;
                    }
                }
                foreach (Node node in endFloor.getNodeList())//get endNode from destination floor
                {
                    if (node.getId() == int.Parse(destinationLocation.nodeId))
                    {
                        endNode = node;
                    }
                }

                if (startNode != null && endNode != null)
                {
                    CrossBuildingAlgorithm navigationAlgorithm = new CrossBuildingAlgorithm(startFloor, startNode, endFloor, endNode, floors);
                    navigationAlgorithm.runAlgorithm().print();//start running the algorithm
                }
            }
        }               
    }
}
