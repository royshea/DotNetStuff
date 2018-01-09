using System;
using System.Configuration;

namespace Secrets
{
    class Program
    {
        static void Main(string[] args)
        {
            string value;
            
            value = ConfigurationManager.AppSettings["notSecretSetting"];
            Console.WriteLine($"This is an app configuration: {value}");

            value = ConfigurationManager.AppSettings["secretSetting"];
            Console.WriteLine($"This is a secret app configuration: {value}");

            value = ConfigurationManager.ConnectionStrings["secretConnection"].ConnectionString;
            Console.WriteLine($"This is a secret connection string: {value}");

            Console.ReadLine();
        }
    }
}
