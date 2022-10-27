module "vpc" {
  source  = "terraform-aws-modules/vpc/aws"
  version = "3.16.1"

  name = "${local.resource_prefix}-APP-vpc"

  cidr = "10.0.0.0/16"
  azs  = slice(data.aws_availability_zones.available.names, 0, 3)

  private_subnets = ["10.0.1.0/24", "10.0.2.0/24", "10.0.3.0/24"]
  public_subnets  = ["10.0.4.0/24", "10.0.5.0/24", "10.0.6.0/24"]

  enable_nat_gateway   = true
  single_nat_gateway   = true
  enable_dns_hostnames = true

  public_subnet_tags = {
    "kubernetes.io/cluster/${local.resource_prefix}-APP-eks" = "shared"
    "kubernetes.io/role/elb"                                 = 1
  }

  private_subnet_tags = {
    "kubernetes.io/cluster/${local.resource_prefix}-APP-eks" = "shared"
    "kubernetes.io/role/internal-elb"                        = 1
  }

  tags = {
    resource_group = aws_resourcegroups_group.app_rg.name
  }
}
