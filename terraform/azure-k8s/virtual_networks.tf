data "azurerm_virtual_network" "app_vnet" {
  name                = "${local.resource_prefix}-APP-vnet"
  resource_group_name = data.azurerm_resource_group.net_rg.name
}