locals {
  environment             = "DEMO"
  project_abbr            = "TT"
  resource_prefix         = "${local.project_abbr}-${local.environment}"
  primary_resource_prefix = "EUS1-${local.resource_prefix}"
  collection              = ["ProductContext-db", "ProfileContext-db", "CartOrders-db", "CartRecommendations-db", "StockCollection-db"]
}