resource "aws_vpc" "app_vpc" {
  cidr_block = local.app_vpc_cidr
  tags = {
    resource_group = aws_resourcegroups_group.net_rg.name
  }
}