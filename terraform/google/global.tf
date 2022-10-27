module "global" {
  source          = "./global"
  location        = "us-east1"
  resource_prefix = "GLB-${local.resource_prefix}"
  collection      = local.collection
}