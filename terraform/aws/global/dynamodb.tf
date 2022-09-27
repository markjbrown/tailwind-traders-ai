resource "aws_dynamodb_table" "product_context" {
  name           = "${local.resource_prefix}-ProductContext-db"
  billing_mode   = "PROVISIONED"
  hash_key       = "id"
  read_capacity  = 20
  write_capacity = 20

  attribute {
    name = "id"
    type = "S"
  }
}

resource "aws_dynamodb_table" "profile_context" {
  name           = "${local.resource_prefix}-ProfileContext-db"
  billing_mode   = "PROVISIONED"
  hash_key       = "id"
  read_capacity  = 20
  write_capacity = 20

  attribute {
    name = "id"
    type = "S"
  }
}

resource "aws_dynamodb_table" "cart_orders" {
  name           = "${local.resource_prefix}-CartOrders-db"
  billing_mode   = "PROVISIONED"
  hash_key       = "id"
  read_capacity  = 20
  write_capacity = 20

  attribute {
    name = "id"
    type = "S"
  }
}

resource "aws_dynamodb_table" "cart_products" {
  name           = "${local.resource_prefix}-CartProducts-db"
  billing_mode   = "PROVISIONED"
  hash_key       = "id"
  read_capacity  = 20
  write_capacity = 20

  attribute {
    name = "id"
    type = "S"
  }
}

resource "aws_dynamodb_table" "cart_reccomendations" {
  name           = "${local.resource_prefix}-CartRecommendations-db"
  billing_mode   = "PROVISIONED"
  hash_key       = "id"
  read_capacity  = 20
  write_capacity = 20

  attribute {
    name = "id"
    type = "S"
  }
}

resource "aws_dynamodb_table" "stock_collection" {
  name           = "${local.resource_prefix}-StockCollection-db"
  billing_mode   = "PROVISIONED"
  hash_key       = "id"
  read_capacity  = 20
  write_capacity = 20

  attribute {
    name = "id"
    type = "S"
  }
}