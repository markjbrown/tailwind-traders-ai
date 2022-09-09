locals {
  app_aks_subnet_address_prefixes = var.app_aks_subnet_address_prefixes
  app_vnet_address_space = var.app_vnet_address_space
  deploy_identity = var.deploy_identity
  kubelet_identity = var.kubelet_identity
  location = var.location
  resource_prefix = var.resource_prefix
}

data "azurerm_client_config" "current" {}
