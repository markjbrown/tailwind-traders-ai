resource "google_compute_network" "vpc" {
  name                    = lower("${local.resource_prefix}-APP-vpc")
  auto_create_subnetworks = false
}

resource "google_compute_subnetwork" "subnet" {
  name          = lower("${local.resource_prefix}-APP-subnet")
  ip_cidr_range = "10.0.1.0/24"
  region        = local.location
  network       = google_compute_network.vpc.self_link
}