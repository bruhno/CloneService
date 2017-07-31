using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Temp
{
    class Program
    {

        public struct Struct
        {
            public int I ;
            public string S;

        }
        static void Main(string[] args)
        {

            //var t = typeof(Struct);

            Struct s =Activator.CreateInstance<Struct>();
            s.S = "3";
            var prop = s.GetType().GetField("S");
            object boxed = s;
            prop.SetValue(boxed, "222");
            s = (Struct)boxed;

            
            Console.WriteLine(s.S);
            Console.ReadKey();
        }
    }
}
