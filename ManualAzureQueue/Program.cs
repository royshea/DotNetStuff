using System;
using System.Threading.Tasks;

namespace AzureQueueDemo
{
    class Program
    {

        static void Main(string[] args)
        {
            var connection = new AzureQueueConnection();
            string result = Task.Run(() => connection.ListQueues()).Result;
            Console.WriteLine(result);
            Console.ReadLine();
            return;
        }
    }
}

