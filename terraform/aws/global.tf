module "global" {
  source = "./global"

  domain_name     = var.domain_name 
  location        = "East US 2"
  resource_prefix = "GLB-${local.resource_prefix}"
}