resource "azurerm_virtual_network" "app_vnet" {
  address_space       = local.app_vnet_address_space
  location            = azurerm_resource_group.net_rg.location
  name                = "${local.resource_prefix}-APP-vnet"
  resource_group_name = azurerm_resource_group.net_rg.name
}
