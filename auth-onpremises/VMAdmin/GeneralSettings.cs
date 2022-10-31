namespace VMAdmin;

public class GeneralSettings
{
	public string ResourceGroupName { get; set; } = null!;

	public string NetworkName { get; set; } = null!;
	public string NetworkAddressSpace  { get; set; } = null!;
	public string SubnetName { get; set; } = null!;
	public string SubnetAddressSpace { get; set; } = null!;
	public string NetworkInterfaceName { get; set; } = null!;
	public string NetworkSecurityGroupName { get; set; } = null!;
	public string VirtualMachineName { get; set; } = null!;
	public string AdminUsername { get; set; } = null!;
	public string AdminPublicKey { get; set; } = null!;
}