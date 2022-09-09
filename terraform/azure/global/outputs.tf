output "kubelet_identity" {
  value = azurerm_user_assigned_identity.aks_kubelet_identity
}

output "deploy_identity" {
  value = azurerm_user_assigned_identity.aks_deploy_identity
}
