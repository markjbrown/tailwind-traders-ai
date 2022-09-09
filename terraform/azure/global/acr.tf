resource "azurerm_container_registry" "acr" {
  name = "${lower(join("", split("-", local.resource_prefix)))}cr"
  resource_group_name = azurerm_resource_group.acr_rg.name
  location = azurerm_resource_group.acr_rg.location
  sku = "Basic"
  admin_enabled = false
}

resource "azurerm_role_assignment" "aks_acr_pull_role" {
  principal_id = azurerm_user_assigned_identity.aks_kubelet_identity.principal_id
  scope        = azurerm_container_registry.acr.id
  role_definition_name = "AcrPull"
}
