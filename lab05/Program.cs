using System.Net;

namespace ipcheck;

public static class Program
{
	public static void Main()
	{
		// Check if network is available
		if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
		{
			Console.WriteLine("Current IP Addresses:");

			// Get host entry for current hostname
			string hostname = Dns.GetHostName();
			IPHostEntry host = Dns.GetHostEntry(hostname);

			// Iterate over each IP address and render their values
			foreach (var address in host.AddressList)
			{
				Console.WriteLine($"\t{address}");
			}
		}
		else
		{
			Console.WriteLine("No network connection");
		}
	}
}