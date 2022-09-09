resource "azurerm_kubernetes_cluster" "app_aks" {
  depends_on = [
    azurerm_role_assignment.app_aks_kubelet_identity_role_assignment
  ]


  location            = azurerm_resource_group.app_rg.location
  name                = "${local.resource_prefix}-APP-aks"
  resource_group_name = azurerm_resource_group.app_rg.name
  dns_prefix = lower("${local.resource_prefix}-APP-aks")
  node_resource_group = "${local.resource_prefix}-APP-vm"
  network_profile {
    network_plugin = "azure"
    network_policy = "azure"
  }

  default_node_pool {
    name    = "default"
    node_count = 1
    vm_size = "Standard_D2_v2"
    vnet_subnet_id = azurerm_subnet.app_aks_subnet.id
  }

  identity {
    type = "UserAssigned"
    identity_ids = [azurerm_user_assigned_identity.app_aks_identity.id]
  }

  kubelet_identity {
    client_id = local.kubelet_identity.client_id
    object_id = local.kubelet_identity.principal_id
    user_assigned_identity_id = local.kubelet_identity.id
  }

  azure_active_directory_role_based_access_control {
    managed = true
    tenant_id = data.azurerm_client_config.current.tenant_id
    azure_rbac_enabled = true
  }
}

resource "azurerm_user_assigned_identity" "app_aks_identity" {
    name                = "${local.resource_prefix}-APP-id"
    resource_group_name = azurerm_resource_group.app_rg.name
    location            = azurerm_resource_group.app_rg.location
}

resource "azurerm_role_assignment" "aks_deploy_role_assignment" {
  principal_id = local.deploy_identity.principal_id
  scope        = azurerm_kubernetes_cluster.app_aks.id
  role_definition_name = "Azure Kubernetes Service RBAC Cluster Admin"
}

resource "azurerm_role_assignment" "ingress_lb_role_assignment" {
  principal_id = azurerm_user_assigned_identity.app_aks_identity.principal_id
  scope        = azurerm_subnet.app_aks_subnet.id
  role_definition_name = "Contributor"
}

resource "azurerm_role_assignment" "app_aks_kubelet_identity_role_assignment" {
  principal_id = azurerm_user_assigned_identity.app_aks_identity.principal_id
  scope        = local.kubelet_identity.id
  role_definition_name = "Managed Identity Operator"
}
