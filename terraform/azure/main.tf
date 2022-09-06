locals {
  environment = "DEMO"
  project_abbr = "TT"
  resource_prefix = "${local.project_abbr}-${local.environment}"
  primary_resource_prefix = "EUS2-${local.resource_prefix}"
}