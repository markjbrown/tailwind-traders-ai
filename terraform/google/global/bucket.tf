resource "google_storage_bucket" "image_store" {
  name          = lower(join("-", [local.resource_prefix, "IMG", "bucket"]))
  location      = "US"
  force_destroy = true
}