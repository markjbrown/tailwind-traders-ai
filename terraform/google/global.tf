module "global" {
  source          = "./global"

  domain_name     = var.domain_name
  location        = "us-east1"
  resource_prefix = "GLB-${local.resource_prefix}"
}