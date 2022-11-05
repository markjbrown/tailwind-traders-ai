locals {
  location        = var.location
  resource_prefix = var.resource_prefix
}

data "google_client_config" "current" {}