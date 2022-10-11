resource "azurerm_key_vault" "app_kv" {
  name                        = "${local.resource_prefix}-APP-kv"
  location                    = azurerm_resource_group.app_rg.location
  resource_group_name         = azurerm_resource_group.app_rg.name
  enabled_for_disk_encryption = true
  enable_rbac_authorization   = true
  soft_delete_retention_days  = 7
  purge_protection_enabled    = false
  tenant_id                   = data.azurerm_client_config.current.tenant_id

  sku_name = "standard"
}

resource "azurerm_role_assignment" "deploy_kv_role" {
  principal_id         = data.azurerm_client_config.current.object_id
  scope                = azurerm_key_vault.app_kv.id
  role_definition_name = "Key Vault Crypto Officer"
}
