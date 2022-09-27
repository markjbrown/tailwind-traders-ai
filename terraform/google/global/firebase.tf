resource "google_firebase_project" "datastore" {
  display_name = "${local.resource_prefix}-DATA-fbp"
}