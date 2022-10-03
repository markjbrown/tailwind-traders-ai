module "global" {
  source = "./global"

  location        = "East US 2"
  resource_prefix = "GLB-${local.resource_prefix}"
}