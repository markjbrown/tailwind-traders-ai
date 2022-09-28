resource "google_app_engine_application" "datastore" {
  database_type = "CLOUD_FIRESTORE"
  location_id = "us-central"
}