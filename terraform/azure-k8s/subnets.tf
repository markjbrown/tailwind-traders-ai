data "azurerm_subnet" "app_aks_subnet" {
  name                 = "aks"
  virtual_network_name = data.azurerm_virtual_network.app_vnet.name
  resource_group_name  = data.azurerm_virtual_network.app_vnet.resource_group_name
}
