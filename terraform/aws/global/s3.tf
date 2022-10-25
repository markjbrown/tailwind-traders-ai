resource "aws_s3_bucket" "image_storage" {
  bucket = join("-", [local.resource_prefix, "IMG", "s3"])
}

resource "aws_s3_bucket_acl" "image_storage" {
  bucket = aws_s3_bucket.image_storage.id
  acl    = "public-read"
}
