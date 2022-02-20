@description('Username for the Virtual Machine')
param adminUsername string

@description('Public key for admin user')
param adminPublicKey string

@description('Unique DNS Name for the Public IP used to access the Virtual Machine.')
param dnsLabelPrefix string = toLower('${vmName}-${uniqueString(resourceGroup().id, vmName)}')

@description('Name for the Public IP used to access the Virtual Machine.')
param publicIpName string = 'myPublicIP'

@description('Allocation method for the Public IP used to access the Virtual Machine.')
@allowed([
  'Dynamic'
  'Static'
])
param publicIPAllocationMethod string = 'Dynamic'

@description('SKU for the Public IP used to access the Virtual Machine.')
@allowed([
  'Basic'
  'Standart'
  ''
])
param publicIPSku string = 'Basic'

@description('Size of the virtual machine.')
param vmSize string = 'Standard_B1ms'

@description('Location for all resources.')
param location string = resourceGroup().location

@description('Name of the virtual machine.')
param vmName string = 'simple-vm'

var vmName_var = vmName
var vmSize_var = vmSize
var adminUsername_var = adminUsername
var adminPublicKey_var = adminPublicKey
var location_var = location
var storageAccountName_var = 'bootdiags${uniqueString(resourceGroup().id)}'
var publicIpName_var = publicIpName
var publicIpSku_var = publicIPSku
var publicIpAllocationMethod_var = publicIPAllocationMethod
var dnsLabelPrefix_var = dnsLabelPrefix
var nicName_var = 'myVMNic'
var addressPrefix = '10.0.0.0/16'
var subnetName = 'Subnet'
var subnetPrefix = '10.0.0.0/24'
var virtualNetworkName_var = 'MyVNET'
var networkSecurityGroupName_var = 'default-NSG'

resource storageAccountName 'Microsoft.Storage/storageAccounts@2021-04-01' = {
  name: storageAccountName_var
  location: location_var
  kind: 'Storage'
  sku: {
    name: 'Standard_LRS'
  }
}

resource publicIpName_resource 'Microsoft.Network/publicIPAddresses@2020-11-01' = {
  name: publicIpName_var
  location: location_var
  sku: {
    name: publicIpSku_var
  }
  properties: {
    publicIPAllocationMethod: publicIpAllocationMethod_var
    dnsSettings: {
      domainNameLabel: dnsLabelPrefix_var
    }
  }
}

resource networkSecurityGroupName 'Microsoft.Network/networkSecurityGroups@2020-11-01' = {
  name: networkSecurityGroupName_var
  location: location_var
  properties: {
    securityRules: [
      {
        name: 'default-allow-22'
        properties: {
          priority: 1000
          access: 'Allow'
          direction: 'Inbound'
          destinationPortRange: '22'
          protocol: 'Tcp'
          sourcePortRange: '*'
          sourceAddressPrefix: '*'
          destinationAddressPrefix: '*'
        }
      }
    ]
  }
}

resource virtualNetworkName 'Microsoft.Network/virtualNetworks@2020-11-01' = {
  name: virtualNetworkName_var
  location: location_var
  properties: {
    addressSpace: {
      addressPrefixes: [
        addressPrefix
      ]
    }
    subnets: [
      {
        name: subnetName
        properties: {
          addressPrefix: subnetPrefix
          networkSecurityGroup: {
            id: networkSecurityGroupName.id
          }
        }
      }
    ]
  }
}

resource nicName 'Microsoft.Network/networkInterfaces@2020-11-01' = {
  name: nicName_var
  location: location_var
  properties: {
    ipConfigurations: [
      {
        name: 'ipConfig1'
        properties: {
          privateIPAllocationMethod: 'Dynamic'
          publicIPAddress: {
            id: publicIpName_resource.id
          }
          subnet: {
            id: resourceId('Microsoft.Network/virtualNetworks/subnets', virtualNetworkName_var, subnetName)
          }
        }
      }
    ]
  }
  dependsOn: [
    virtualNetworkName
  ]
}

resource vmName_resource 'Microsoft.Compute/virtualMachines@2021-07-01' = {
  name: vmName_var
  location: location_var
  properties: {
    hardwareProfile: {
      vmSize: vmSize_var
    }
    osProfile: {
      computerName: vmName_var
      adminUsername: adminUsername_var
      linuxConfiguration: {
        disablePasswordAuthentication: true
        ssh: {
          publicKeys: [
            {
              path: '/home/${adminUsername}/.ssh/authorized_keys'
              keyData: adminPublicKey_var
            }
          ]
        }
      }
    }
    storageProfile: {
      imageReference: {
        publisher: 'SUSE'
        offer: 'opensuse-leap-15-3'
        sku: 'gen2'
        version: 'latest'
      }
      osDisk: {
        createOption: 'FromImage'
        managedDisk: {
          storageAccountType: 'StandardSSD_LRS'
        }
      }
      dataDisks: [
        {
          diskSizeGB: 128
          lun: 0
          createOption: 'Empty'
        }
      ]
    }
    networkProfile: {
      networkInterfaces: [
        {
          id: nicName.id
        }
      ]
    }
    diagnosticsProfile: {
      bootDiagnostics: {
        enabled: true
        storageUri: storageAccountName.properties.primaryEndpoints.blob
      }
    }
  }
}

output hostname string = publicIpName_resource.properties.dnsSettings.fqdn
