resource "aws_resourcegroups_group" "ecr_rg" {
  name = "${local.resource_prefix}-ECR-rg"
  resource_query {
    query = jsonencode({
      ResourceTypeFilters = ["*"]
      TagFilters = [
        {
          Key = "resource_group"
          Values = ["${local.resource_prefix}-ECR-rg"]
        }
      ]
    })
  }
}

resource "aws_resourcegroups_group" "app_rg" {
    name = "${local.resource_prefix}-APP-rg"
    resource_query {
        query = jsonencode({
        ResourceTypeFilters = ["*"]
        TagFilters = [
            {
            Key = "resource_group"
            Values = ["${local.resource_prefix}-APP-rg"]
            }
        ]
        })
    }
}

resource "aws_resourcegroups_group" "data_rg" {
  name = "${local.resource_prefix}-DATA-rg"
  resource_query {
    query = jsonencode({
      ResourceTypeFilters = ["*"]
      TagFilters = [
        {
          Key = "resource_group"
          Values = ["${local.resource_prefix}-DATA-rg"]
        }
      ]
    })
  }
}