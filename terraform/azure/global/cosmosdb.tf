resource "azurerm_cosmosdb_account" "datastore" {
  name                = lower("${local.resource_prefix}-DATA-cdb")
  location            = azurerm_resource_group.data_rg.location
  resource_group_name = azurerm_resource_group.data_rg.name
  offer_type          = "Standard"
  kind                = "GlobalDocumentDB"

  consistency_policy {
    consistency_level = "Eventual"
  }

  geo_location {
    failover_priority = 0
    location          = azurerm_resource_group.data_rg.location
  }
}