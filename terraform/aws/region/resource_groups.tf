resource "aws_resourcegroups_group" "app_rg" {
  name = "${local.resource_prefix}-APP-rg"
  resource_query {
    query = jsonencode({
      ResourceTypeFilters = ["AWS::AllSupported"]
      TagFilters = [
        {
          Key    = "resource_group"
          Values = ["${local.resource_prefix}-APP-rg"]
        }
      ]
    })
  }
}

resource "aws_resourcegroups_group" "net_rg" {
  name = "${local.resource_prefix}-NET-rg"
  resource_query {
    query = jsonencode({
      ResourceTypeFilters = ["AWS::AllSupported"]
      TagFilters = [
        {
          Key    = "resource_group"
          Values = ["${local.resource_prefix}-NET-rg"]
        }
      ]
    })
  }
}

