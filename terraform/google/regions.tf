module "region" {
  source = "./region"
  location = "us-east1"
  resource_prefix = "EU2-${local.resource_prefix}"
}