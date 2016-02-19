using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class CrossFloorAlgorithm
    {
        private Floor startFloor, endFloor;
        private Node startNode, endNode;
        private const float FLOOR_DISTANCE = 20;

        public CrossFloorAlgorithm(Floor startFloor, Node startNode, Floor endFloor, Node endNode)
        {
            this.startFloor = startFloor;
            this.startNode = startNode;
            this.endFloor = endFloor;
            this.endNode = endNode;
        }

        public Path runAlgorithm()
        {
            Path bestPath = null;
            int counter = 0;
            float minPathLength = 0;
            foreach (Node startElevatorNode in startFloor.getElevatorList())//for every elevator on the start floor
            {
                foreach (Node endElevatorNode in endFloor.getElevatorList())//for every elevator on the end floor
                {
                    if (startElevatorNode.getElevatorGroupNum() == endElevatorNode.getElevatorGroupNum())//if thses two elevators are connected
                    {
                        SameFloorAlgorithm startFloorNavigation = new SameFloorAlgorithm(startFloor, startNode, startElevatorNode);
                        Path startFloorPath = startFloorNavigation.runAlgorithm();
                        SameFloorAlgorithm endFloorNavigation = new SameFloorAlgorithm(endFloor, endElevatorNode, endNode);
                        Path endFloorPath = endFloorNavigation.runAlgorithm();
                        if (counter == 0)
                        {
                            minPathLength = startFloorPath.distance + endFloorPath.distance;
                            bestPath = startFloorPath.concatenate(endFloorPath);
                        }
                        else if (startFloorPath.distance + endFloorPath.distance < minPathLength)//if this combination is better, update the best path
                        {
                            minPathLength = startFloorPath.distance + endFloorPath.distance;
                            bestPath = startFloorPath.concatenate(endFloorPath);
                        }
                        counter++;                        
                    }
                }
            }
            float floorDistance = getFloorDistance(startFloor, endFloor);
            bestPath.distance += floorDistance;// add this distance to the total distance
            return bestPath;
        }

        public float getFloorDistance(Floor floor1, Floor floor2)
        {
            float floor1Value = 0, floor2Value = 0;
            if (floor1.floorName.ToCharArray()[0] == 'b' || floor1.floorName.ToCharArray()[0] == 'B')//if this is a basement floor
            {
                floor1Value = (-1) * float.Parse(floor1.floorName.Substring(1, 1));//if b4, converts to -4
            }
            else
            {
                floor1Value = float.Parse(floor1.floorName);//if level 2, converts to 2
            }

            if (floor2.floorName.ToCharArray()[0] == 'b' || floor2.floorName.ToCharArray()[0] == 'B')//if this is a basement floor
            {
                floor2Value = (-1) * float.Parse(floor2.floorName.Substring(1, 1));//if b4, converts to -4
            }
            else
            {
                floor2Value = float.Parse(floor2.floorName);//if level 2, converts to 2
            }

            return FLOOR_DISTANCE * Math.Abs(floor1Value - floor2Value);
        }
    }
}
