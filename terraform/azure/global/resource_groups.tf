resource "azurerm_resource_group" "acr_rg" {
  location = local.location
  name = "${local.resource_prefix}-ACR-rg"
}

resource "azurerm_resource_group" "app_rg" {
  location = local.location
  name = "${local.resource_prefix}-APP-rg"
}

resource "azurerm_resource_group" "data_rg" {
  location = local.location
  name = "${local.resource_prefix}-DATA-rg"
}
