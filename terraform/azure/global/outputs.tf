output "kubelet_identity" {
  value = azurerm_user_assigned_identity.aks_kubelet_identity
}

output "deploy_identity" {
  value = azurerm_user_assigned_identity.aks_deploy_identity
}

output "keyvault_identity" {
  value = azurerm_user_assigned_identity.aks_keyvault_identity
}

output "app_keyvault" {
  value = azurerm_key_vault.app_kv
}
