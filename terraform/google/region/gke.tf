resource "google_container_cluster" "gke" {
  name     = lower("${local.resource_prefix}-APP-gke")
  location = local.location

  remove_default_node_pool = true
  initial_node_count       = 1

  network    = google_compute_network.vpc.name
  subnetwork = google_compute_subnetwork.subnet.name


  workload_identity_config {
    workload_pool = "${data.google_client_config.current.project}.svc.id.goog"
  }
}

resource "google_container_node_pool" "node_pool" {
  name       = lower("${local.resource_prefix}-APP-node-pool")
  location   = local.location
  cluster    = google_container_cluster.gke.name
  node_count = 1

  node_config {
    machine_type = "n1-standard-1"
    disk_size_gb = 10
    oauth_scopes = [
      "https://www.googleapis.com/auth/compute",
      "https://www.googleapis.com/auth/devstorage.read_only",
      "https://www.googleapis.com/auth/logging.write",
      "https://www.googleapis.com/auth/monitoring",
    ]
  }
}
