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

  aci_connector_linux {
    subnet_name = azurerm_subnet.app_aks_subnet.name
  }

  kubelet_identity {
    client_id = azurerm_user_assigned_identity.app_aks_kubelet_identity.client_id
    object_id = azurerm_user_assigned_identity.app_aks_kubelet_identity.principal_id
    user_assigned_identity_id = azurerm_user_assigned_identity.app_aks_kubelet_identity.id
  }
}

resource "azurerm_user_assigned_identity" "app_aks_identity" {
    name                = "${local.resource_prefix}-APP-id"
    resource_group_name = azurerm_resource_group.app_rg.name
    location            = azurerm_resource_group.app_rg.location
}

resource "azurerm_user_assigned_identity" "app_aks_kubelet_identity" {
    name                = "${local.resource_prefix}-APP-kubelet-id"
    resource_group_name = azurerm_resource_group.app_rg.name
    location            = azurerm_resource_group.app_rg.location
}

resource "azurerm_role_assignment" "app_aks_kubelet_identity_role_assignment" {
  principal_id = azurerm_user_assigned_identity.app_aks_identity.principal_id
  scope        = azurerm_user_assigned_identity.app_aks_kubelet_identity.id
  role_definition_name = "Managed Identity Operator"
}
