locals {
  environment     = "DEMO"
  project_abbr    = "TT"
  resource_prefix = "EUS2-${local.project_abbr}-${local.environment}"
}

data "azurerm_kubernetes_cluster" "app_aks" {
  name                = "${local.resource_prefix}-APP-aks"
  resource_group_name = data.azurerm_resource_group.app_rg.name
}
