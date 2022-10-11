locals {
  location        = var.location
  resource_prefix = var.resource_prefix
}

data "azurerm_client_config" "current" {}