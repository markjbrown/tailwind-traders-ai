module "region" {
  source          = "./region"

  domain_name     = var.domain_name
  location        = "us-east1"
  resource_prefix = "EU2-${local.resource_prefix}"
  zone_id              = module.global.zone_id
}