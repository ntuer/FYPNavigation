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
            string[] originalBuildingFiles = Directory.GetFiles(path, originalLocation.buildingName + "*");//get the file names with the same building name
            string[] destinationBuildingFiles = Directory.GetFiles(path, destinationLocation.buildingName + "*");
            loadUsefulFiles(originalBuildingFiles, destinationLocation, usefulOriginalFiles);
            loadUsefulFiles(destinationBuildingFiles, originalLocation, usefulDestinationFiles);//the testing result for usefulOriginalFiles is positive
            readUsefulFiles(usefulOriginalFiles);
            readUsefulFiles(usefulDestinationFiles);
        }


        public void loadUsefulFiles(string[] filesToSelect, Location selectionCritiriaByDestination, List<string> selectionResultFiles)
        {
            foreach (string file in filesToSelect)
            {
                XmlDocument confFile = new XmlDocument();
                confFile.Load(file);
                XmlNode NTUTag = confFile.DocumentElement.SelectSingleNode("/NTU");
                foreach (XmlNode pointNode in NTUTag.SelectNodes("/NTU/p"))
                {
                    if (pointNode.Attributes["type"].InnerText == "Connector" && pointNode.Attributes["typeValue"].InnerText == selectionCritiriaByDestination.buildingName)
                    {
                        selectionResultFiles.Add(file);
                        break;
                    }
                }
            }
        }

        public void readUsefulFiles(List<string> filesToRead)
        {
            foreach (string file in filesToRead)
            {
                string floorName = file.Split('/')[file.Split('/').Length - 1];
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
                            floor.getNodeList()[i].addNeighbor(floor.getNodeList()[neighborID]);
                            floor.getNodeList()[neighborID].addNeighbor(floor.getNodeList()[i]);
                        }
                    }
                }

                //add the floor to the floor list
                floors.Add(floor);
            }
        }        
       
    }
}
