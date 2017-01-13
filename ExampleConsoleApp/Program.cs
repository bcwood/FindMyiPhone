using FindMyiPhone;

namespace ExampleConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var icloud = new iCloudService("Apple ID", "password");
            icloud.PlaySound("Device Name", "Find My iPhone");
        }
    }
}
