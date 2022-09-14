data "azurerm_resource_group" "app_rg" {
  name = "${local.resource_prefix}-APP-rg"
}

data "azurerm_resource_group" "net_rg" {
  name = "${local.resource_prefix}-NET-rg"
}