resource "aws_ecr_repository" "ecr" {
  name = "${lower(join("", split("-", local.resource_prefix)))}cr"
  image_tag_mutability = "MUTABLE"
  tags = {
    resource_group = aws_resourcegroups_group.ecr_rg.name
  }
}