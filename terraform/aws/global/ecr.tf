locals {
  images = [
    "cart.api",
    "product.api",
    "profile.api",
    "login.api",
    "coupon.api",
    "popular-product.api",
    "stock.api",
    "image-classifier.api",
    "mobileapigw",
    "webapigw"
  ]
}

resource "aws_ecr_repository" "ecr" {
  count                = length(local.images)
  name                 = local.images[count.index]
  image_tag_mutability = "MUTABLE"
  tags = {
    resource_group = aws_resourcegroups_group.ecr_rg.name
  }
}