resource "google_firebase_project" "datastore" {
  provider = google-beta
  display_name = "${local.resource_prefix}-DATA-fbp"
}