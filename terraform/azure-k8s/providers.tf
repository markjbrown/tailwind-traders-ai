terraform {
  required_providers {
    azurerm = {
      source = "hashicorp/azurerm"
      version = "~>3.21.1"
    }

    azuread = {
      source = "hashicorp/azuread"
      version = "~>2.28.1"
    }

    kubernetes = {
      source = "hashicorp/kubernetes"
      version = "~>2.13.1"
    }
  }

  cloud {
    organization = "tailwind-traders"
    workspaces {
      name = "tailwind-traders-azure-k8s"
    }
  }
}

provider "azurerm" {
  features {}
}

provider "azuread" {}

provider "kubernetes" {
  client_certificate = base64decode(data.azurerm_kubernetes_cluster.app_aks.kube_admin_config.0.client_certificate)
  client_key         = base64decode(data.azurerm_kubernetes_cluster.app_aks.kube_admin_config.0.client_key)
  cluster_ca_certificate = base64decode(data.azurerm_kubernetes_cluster.app_aks.kube_admin_config.0.cluster_ca_certificate)
  host = data.azurerm_kubernetes_cluster.app_aks.kube_admin_config.0.host
}

provider "helm" {
  kubernetes {
    client_certificate = base64decode(data.azurerm_kubernetes_cluster.app_aks.kube_admin_config.0.client_certificate)
    client_key         = base64decode(data.azurerm_kubernetes_cluster.app_aks.kube_admin_config.0.client_key)
    cluster_ca_certificate = base64decode(data.azurerm_kubernetes_cluster.app_aks.kube_admin_config.0.cluster_ca_certificate)
    host = data.azurerm_kubernetes_cluster.app_aks.kube_admin_config.0.host
  }
}