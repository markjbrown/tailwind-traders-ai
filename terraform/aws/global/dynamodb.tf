resource "aws_dynamodb_table" "product_context" {
  name           = "${local.resource_prefix}-ProductContext-db"
  billing_mode   = "PROVISIONED"
  hash_key       = "id"
  read_capacity  = 20
  write_capacity = 20

  attribute {
    name = "id"
    type = "N"
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
    type = "N"
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
    type = "N"
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
    type = "N"
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
    type = "N"
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
    type = "N"
  }
}

resource "aws_dynamodb_table" "product_brand" {
  name           = "${local.resource_prefix}-ProductBrand-db"
  billing_mode   = "PROVISIONED"
  hash_key       = "id"
  read_capacity  = 20
  write_capacity = 20

  attribute {
    name = "id"
    type = "N"
  }
}

resource "aws_dynamodb_table" "product_tag" {
  name           = "${local.resource_prefix}-ProductTag-db"
  billing_mode   = "PROVISIONED"
  hash_key       = "id"
  read_capacity  = 20
  write_capacity = 20

  attribute {
    name = "id"
    type = "N"
  }
}

resource "aws_dynamodb_table" "product_type" {
  name           = "${local.resource_prefix}-ProductType-db"
  billing_mode   = "PROVISIONED"
  hash_key       = "id"
  read_capacity  = 20
  write_capacity = 20

  attribute {
    name = "id"
    type = "N"
  }
}

resource "aws_dynamodb_table" "product_feature" {
  name           = "${local.resource_prefix}-ProductFeature-db"
  billing_mode   = "PROVISIONED"
  hash_key       = "id"
  read_capacity  = 20
  write_capacity = 20

  attribute {
    name = "id"
    type = "N"
  }
}