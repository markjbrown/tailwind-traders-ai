locals {
  app_eks_subnet_cidrs = var.app_eks_subnet_cidrs
  app_vpc_cidr         = var.app_vpc_cidr
  location             = var.location
  resource_prefix      = var.resource_prefix
}

data "aws_availability_zones" "available" {}