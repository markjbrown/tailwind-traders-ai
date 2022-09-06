resource "azurerm_subnet" "app_aks_subnet" {
  address_prefixes     = local.app_aks_subnet_address_prefixes
  name                 = "aks"
  resource_group_name  = azurerm_virtual_network.app_vnet.resource_group_name
  virtual_network_name = azurerm_virtual_network.app_vnet.name

#  delegation {
#    name = "aciDelegation"
#    service_delegation {
#      name = "Microsoft.ContainerInstance/containerGroups"
#      actions = [
#        "Microsoft.Network/virtualNetworks/subnets/action",
#      ]
#    }
#  }
}
