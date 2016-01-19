using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ConsoleApplication1
{
    class App
    {
        static void Main(string[] args)
        {
            FYPNavigation app = new FYPNavigation();
            app.readInputs();
            app.readFiles();
            Console.ReadLine();            
        }


        
    }
}
