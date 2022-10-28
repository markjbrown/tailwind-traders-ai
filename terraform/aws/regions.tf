module "primary_region" {
  source = "./region"

  domain_name     = local.domain_name
  location        = "us-east-1"
  resource_prefix = local.primary_resource_prefix

  app_eks_subnet_cidrs = ["192.168.0.0/24", "192.168.1.0/24"]
  app_vpc_cidr         = "192.168.0.0/16"
  zone_id              = module.global.zone_id
}