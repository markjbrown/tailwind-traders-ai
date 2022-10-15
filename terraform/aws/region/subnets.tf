resource "aws_subnet" "app_eks_subnet1" {
  vpc_id                  = aws_vpc.app_vpc.id
  cidr_block              = local.app_eks_subnet_cidrs[0]
  availability_zone       = "us-east-1a"
  map_public_ip_on_launch = true

  tags = {
    resource_group                                           = aws_resourcegroups_group.net_rg.name
    "kubernetes.io/cluster/${local.resource_prefix}-APP-eks" = "shared"
  }
}

resource "aws_subnet" "app_eks_subnet2" {
  vpc_id                  = aws_vpc.app_vpc.id
  cidr_block              = local.app_eks_subnet_cidrs[1]
  availability_zone       = "us-east-1b"
  map_public_ip_on_launch = true

  tags = {
    resource_group                                           = aws_resourcegroups_group.net_rg.name
    "kubernetes.io/cluster/${local.resource_prefix}-APP-eks" = "shared"
  }
}
