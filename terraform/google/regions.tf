module "region" {
  source = "./region"
  location = "East US 2"
  resource_prefix = "EU2-${local.resource_prefix}"
}