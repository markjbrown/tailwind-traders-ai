resource "google_app_engine_application" "datastore" {
  project = "tailwind-traders-363214"
  database_type = "CLOUD_FIRESTORE"
  location_id = "us-central"
}