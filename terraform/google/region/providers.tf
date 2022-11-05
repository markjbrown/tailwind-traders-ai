terraform {
  required_providers {
    kubectl = {
      source  = "gavinbunney/kubectl"
      version = "~> 1.14.0"
    }
  }
}

provider "kubernetes" {
  host                   = google_container_cluster.gke.endpoint
  cluster_ca_certificate = base64decode(google_container_cluster.gke.master_auth.0.cluster_ca_certificate)
  client_certificate     = base64decode(google_container_cluster.gke.master_auth.0.client_certificate)
  client_key             = base64decode(google_container_cluster.gke.master_auth.0.client_key)
}

provider "helm" {
  kubernetes {
    host                   = google_container_cluster.gke.endpoint
    cluster_ca_certificate = base64decode(google_container_cluster.gke.master_auth.0.cluster_ca_certificate)
    client_certificate     = base64decode(google_container_cluster.gke.master_auth.0.client_certificate)
    client_key             = base64decode(google_container_cluster.gke.master_auth.0.client_key)
  }
}

provider "kubectl" {
  host                   = google_container_cluster.gke.endpoint
  cluster_ca_certificate = base64decode(google_container_cluster.gke.master_auth.0.cluster_ca_certificate)
  client_certificate     = base64decode(google_container_cluster.gke.master_auth.0.client_certificate)
  client_key             = base64decode(google_container_cluster.gke.master_auth.0.client_key)
  load_config_file       = false
}