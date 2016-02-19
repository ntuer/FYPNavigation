using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Path
    {
        public string buildingName
        {
            get; set;
        }

        public string floorName
        {
            get; set;
        }

        public string path
        {
            get; set;
        }

        public float distance
        {
            get; set;
        }

        public Path concatenate(Path path)
        {
            Path newPath = new Path();
            newPath.buildingName = this.buildingName + "-" + path.buildingName;
            newPath.floorName = this.floorName + "-" + path.floorName;
            newPath.distance = this.distance + path.distance;
            newPath.path = this.path + "\n" + path.path;

            return newPath;
        }

        public void print()
        {
            Console.WriteLine("Path"+ "("+ this.distance + "): " + this.path);
        }

        
    }
}
