resource "aws_s3_bucket" "image_storage" {
  bucket = lower(join("-", [local.resource_prefix, "IMG", "s3"]))
}

resource "aws_s3_bucket_acl" "image_storage" {
  bucket = aws_s3_bucket.image_storage.id
  acl    = "public-read"
}

resource "aws_s3_bucket_website_configuration" "image_storage" {
  bucket = aws_s3_bucket.image_storage.bucket

  index_document {
    suffix = "index.html"
  }

  error_document {
    key = "error.html"
  }
}

resource "aws_s3_bucket_policy" "image_storage" {
  bucket = aws_s3_bucket.image_storage.id
  policy = <<POLICY
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Sid": "PublicReadGetObject",
      "Effect": "Allow",
      "Principal": "*",
      "Action": "s3:GetObject",
      "Resource": "arn:aws:s3:::${aws_s3_bucket.image_storage.id}/*"
    }
  ]
}
POLICY
}
