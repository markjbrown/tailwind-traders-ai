resource "google_storage_bucket" "image_store" {
  name          = lower(join("-", [local.resource_prefix, "IMG", "bucket"]))
  location      = "US"
  force_destroy = true
}

resource "google_storage_bucket_access_control" "public_read" {
  bucket = google_storage_bucket.image_store.name
  role   = "READER"
  entity = "allUsers"
}

resource "google_storage_bucket_acl" "public_read" {
  bucket         = google_storage_bucket.image_store.name
  predefined_acl = "publicRead"
}

resource "google_storage_default_object_access_control" "public_read" {
  bucket = google_storage_bucket.image_store.name
  role   = "READER"
  entity = "allUsers"
}

resource "google_storage_bucket_iam" "public_read" {
  bucket = google_storage_bucket.image_store.name
  role   = "roles/storage.objectViewer"
  members = [
    "allUsers",
  ]
}