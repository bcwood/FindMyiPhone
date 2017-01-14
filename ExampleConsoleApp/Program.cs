using System;
using FindMyiPhone;

namespace ExampleConsoleApp
{
    public class Program
    {
        private static iCloudService _service;

        public static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Find My iPhone. Please login to continue.");

            Authenticate();
            ListDevices();
        }

        private static void Authenticate()
        {
            while (true)
            {
                Console.Write("\nApple ID: ");
                string appleId = Console.ReadLine();
                Console.Write("Password: ");
                string password = null;
                while (true)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Enter)
                        break;
                    password += key.KeyChar;
                }

                try
                {
                    _service = new iCloudService(appleId, password);
                    Console.WriteLine("\n\nLogin successful!");
                    return;
                }
                catch (System.Security.SecurityException ex)
                {
                    Console.WriteLine($"\n\nLogin failed: {ex.Message}");
                }
            }
        }

        private static void ListDevices()
        {
            Console.WriteLine("\nSelect from your list of devices:");

            var devices = _service.GetDevices();
            int index = 1;
            foreach (var device in devices)
            {
                Console.WriteLine($"{index}) {device.Name}");
                index++;
            }

            Console.Write("\nEnter the device # > ");
            string deviceIndex = Console.ReadLine();

            // TODO: display device options (view details, play sound, etc.)
        }
    }
}
