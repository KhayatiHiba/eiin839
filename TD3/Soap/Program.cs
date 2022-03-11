using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soap.ServiceReference1;

namespace Soap
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CalculatorSoapClient client = new CalculatorSoapClient();
            Console.WriteLine(client.Add(2,5));
            Console.ReadLine();

        }
    }
}
