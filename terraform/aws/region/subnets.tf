resource "aws_subnet" "app_eks_subnet" {
  vpc_id     = aws_vpc.app_vpc.id
  cidr_block = local.app_eks_subnet_cidr

  tags = {
    resource_group = aws_resourcegroups_group.net_rg.name
  }
}