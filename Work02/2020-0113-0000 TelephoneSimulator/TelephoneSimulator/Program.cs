using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelephoneSimulator.Src;

namespace TelephoneSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Program START ================");

            try
            {
                var app = new Application(args);
                app.RunApplication();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("!!! Exception !!!");
                Console.Error.WriteLine(e.Message);
            }
            finally
            {
                Console.WriteLine("Program END ================");
                Console.ReadKey();
            }
        }
    }
}
/*******************************************************************************
 * EOF
 ******************************************************************************/
