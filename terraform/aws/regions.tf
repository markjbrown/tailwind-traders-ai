module "primary_region" {
  source = "./region"

  location         = "us-east-1"
  resource_prefix  = local.primary_resource_prefix

  app_eks_subnet_cidr = "192.168.0.0/24"
  app_vpc_cidr = "192.168.0.0/16"
}