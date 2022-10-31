using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Compute;
using Azure.ResourceManager.Compute.Models;
using Azure.ResourceManager.Network;
using Azure.ResourceManager.Network.Models;
using Azure.ResourceManager.Resources;
using Microsoft.Extensions.Configuration;

namespace VMAdmin;

public static class Program
{
	private static AuthSettings _authSettings = null!;
	private static GeneralSettings _settings = null!;

	private static async Task Main()
	{
		GetConfiguration();

		// Setup authentication
		var armClient = new ArmClient(GetCredential());
		SubscriptionResource subscription = await armClient.GetDefaultSubscriptionAsync();
		await Console.Out.WriteLineAsync($"Default subscription id: {subscription.Id}");

		await Console.Out.WriteLineAsync($"Verifying resource group {_settings.ResourceGroupName}");
		ResourceGroupResource resourceGroup = await subscription.GetResourceGroupAsync(_settings.ResourceGroupName);
		AzureLocation location = resourceGroup.Data.Location;
		await Console.Out.WriteLineAsync(
			$"Got resource group {resourceGroup.Data.Name}. Resource group located in {location.DisplayName} region");

		await Console.Out.WriteLineAsync($"Creating network security group {_settings.NetworkSecurityGroupName} ...");
		var nsgData = new NetworkSecurityGroupData
		{
			Location = location,
			SecurityRules =
			{
				new SecurityRuleData
				{
					Name = "Allow management ports in",
					Priority = 1000,
					Access = SecurityRuleAccess.Allow,
					Direction = SecurityRuleDirection.Inbound,
					Protocol = SecurityRuleProtocol.Tcp,
					DestinationAddressPrefix = "*",
					DestinationPortRanges = { "22", "3389" },
					SourceAddressPrefix = "*",
					SourcePortRange = "*"
				}
			}
		};
		NetworkSecurityGroupCollection nsgContainer = resourceGroup.GetNetworkSecurityGroups();
		NetworkSecurityGroupResource nsg = (
			await nsgContainer.CreateOrUpdateAsync(
				WaitUntil.Completed,
				_settings.NetworkSecurityGroupName,
				nsgData)
		).Value;
		await Console.Out.WriteLineAsync($"Created network security group {nsg.Data.Name}");


		await Console.Out.WriteLineAsync($"Creating virtual network {_settings.NetworkName} ...");
		var vnetData = new VirtualNetworkData
		{
			Location = location,
			AddressPrefixes = { _settings.NetworkAddressSpace },
			Subnets =
			{
				new SubnetData
				{
					Name = _settings.SubnetName,
					AddressPrefix = _settings.SubnetAddressSpace,
					NetworkSecurityGroup = nsg.Data
				}
			}
		};
		VirtualNetworkCollection vnetContainer = resourceGroup.GetVirtualNetworks();
		VirtualNetworkResource vnet = (
			await vnetContainer.CreateOrUpdateAsync(
				WaitUntil.Completed,
				_settings.NetworkName,
				vnetData)
		).Value;
		await Console.Out.WriteLineAsync($"Created virtual network {vnet.Data.Name}");

		await Console.Out.WriteLineAsync($"Creating network interface {_settings.NetworkInterfaceName} ...");
		var nicData = new NetworkInterfaceData
		{
			Location = location,
			IPConfigurations =
			{
				new NetworkInterfaceIPConfigurationData
				{
					Name = "Primary",
					Primary = true,
					Subnet = new SubnetData
					{
						Id = vnet.Data.Subnets.First(s => s.Name == _settings.SubnetName).Id
					},
					PrivateIPAllocationMethod = NetworkIPAllocationMethod.Dynamic
				}
			}
		};
		NetworkInterfaceCollection nicContainer = resourceGroup.GetNetworkInterfaces();
		NetworkInterfaceResource nic = (
			await nicContainer.CreateOrUpdateAsync(
				WaitUntil.Completed,
				_settings.NetworkInterfaceName,
				nicData)
		).Value;
		await Console.Out.WriteLineAsync($"Created network interface {nic.Data.Name}");

		await Console.Out.WriteLineAsync($"Creating virtual machine {_settings.VirtualMachineName} ...");
		var availabilitySetData = new AvailabilitySetData(location)
		{
			PlatformUpdateDomainCount = 5,
			PlatformFaultDomainCount = 2,
			Sku = new ComputeSku
			{
				Name = "Aligned"
			}
		};
		AvailabilitySetCollection availabilitySetCollection = resourceGroup.GetAvailabilitySets();
		AvailabilitySetResource availabilitySet = (
			await availabilitySetCollection.CreateOrUpdateAsync(
				WaitUntil.Completed,
				_settings.VirtualMachineName + "-as",
				availabilitySetData)
		).Value;
		var vmData = new VirtualMachineData(location)
		{
			HardwareProfile = new VirtualMachineHardwareProfile
			{
				VmSize = VirtualMachineSizeType.StandardB1S
			},
			OSProfile = new VirtualMachineOSProfile
			{
				AdminUsername = _settings.AdminUsername,
				ComputerName = _settings.VirtualMachineName,
				LinuxConfiguration = new LinuxConfiguration
				{
					DisablePasswordAuthentication = true,
					SshPublicKeys =
					{
						new SshPublicKeyConfiguration
						{
							Path = $"/home/{_settings.AdminUsername}/.ssh/authorized_keys",
							KeyData = _settings.AdminPublicKey
						}
					}
				}
			},
			NetworkProfile = new VirtualMachineNetworkProfile
			{
				NetworkInterfaces =
				{
					new VirtualMachineNetworkInterfaceReference
					{
						Id = nic.Id
					}
				}
			},
			StorageProfile = new VirtualMachineStorageProfile
			{
				OSDisk = new VirtualMachineOSDisk(DiskCreateOptionType.FromImage)
				{
					OSType = SupportedOperatingSystemType.Linux,
					Caching = CachingType.ReadWrite,
					ManagedDisk = new VirtualMachineManagedDisk
					{
						StorageAccountType = StorageAccountType.StandardSsdLrs
					}
				},
				ImageReference = new ImageReference
				{
					Publisher = "Canonical",
					Offer = "UbuntuServer",
					Sku = "18.04-LTS",
					Version = "latest"
				}
			},
			AvailabilitySetId = availabilitySet.Id
		};
		VirtualMachineCollection? vmCollection = resourceGroup.GetVirtualMachines();
		VirtualMachineResource? vm = (
			await vmCollection.CreateOrUpdateAsync(
				WaitUntil.Completed,
				_settings.VirtualMachineName,
				vmData)
		).Value;
		await Console.Out.WriteLineAsync($"Created virtual machine {vm.Data.Name} @ {vm.Data.TimeCreated}");
	}

	private static void GetConfiguration()
	{
		IConfigurationBuilder? configBuilder = new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile("appsettings.json");
		IConfigurationRoot? configuration = configBuilder.Build();

		_authSettings = configuration.GetRequiredSection("Authentication").Get<AuthSettings>();
		_settings = configuration.GetRequiredSection("General").Get<GeneralSettings>();
	}

	private static TokenCredential GetCredential()
	{
		bool useDefaultEnvironment = _authSettings.UseSdkDefaultAuthenticationEnvironment;

		return useDefaultEnvironment
			? GetAzureCredentialsUsingDefaultEnvironment()
			: GetCredentialsUsingModifiedEnvironment();
	}

	private static DefaultAzureCredential GetAzureCredentialsUsingDefaultEnvironment()
	{
		return GetAzureCredentials();
	}

	private static DefaultAzureCredential GetCredentialsUsingModifiedEnvironment()
	{
		AuthEnvironment.WriteCustomAuthEnvironment();
		DefaultAzureCredential azureCredentials = GetAzureCredentials();
		AuthEnvironment.RestoreAuthEnvironment();

		return azureCredentials;
	}

	private static DefaultAzureCredential GetAzureCredentials()
	{
		var azureCredentials = new DefaultAzureCredential(new DefaultAzureCredentialOptions
		{
			ExcludeAzureCliCredential = true,
			ExcludeInteractiveBrowserCredential = true,
			ExcludeVisualStudioCredential = true,
			ExcludeAzurePowerShellCredential = true,
			ExcludeSharedTokenCacheCredential = true,
			ExcludeVisualStudioCodeCredential = true
		});

		return azureCredentials;
	}
}