resource "azurerm_resource_group" "app_rg" {
  location = local.location
  name     = "${local.resource_prefix}-APP-rg"
}

resource "azurerm_resource_group" "net_rg" {
  location = local.location
  name     = "${local.resource_prefix}-NET-rg"
}
