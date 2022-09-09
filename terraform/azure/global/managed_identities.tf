resource "azurerm_user_assigned_identity" "aks_kubelet_identity" {
  name = "${local.resource_prefix}-APP-kubelet-id"
  resource_group_name = azurerm_resource_group.app_rg.name
  location = azurerm_resource_group.app_rg.location
}

resource "azurerm_user_assigned_identity" "aks_deploy_identity" {
  name = "${local.resource_prefix}-APP-deploy-id"
  resource_group_name = azurerm_resource_group.app_rg.name
  location = azurerm_resource_group.app_rg.location
}
