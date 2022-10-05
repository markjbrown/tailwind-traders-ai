resource "google_app_engine_application" "datastore" {
  project       = "tailwind-traders-363214"
  database_type = "CLOUD_FIRESTORE"
  location_id = "us-central"
}

resource "google_firestore_document" "mydoc" {
  project     = "tailwind-traders-363214"
  count = length(var.collection)
  collection  = "${var.resource_prefix}-${var.collection[count.index]}"
  document_id = "data"
  fields      = file("./data.json")
}
