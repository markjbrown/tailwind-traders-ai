locals {
  config = <<EOF
controller:
  service:
    loadBalancerIP: ${azurerm_public_ip.app_aks_ingress_ip.ip_address}
    annotations:
      service.beta.kubernetes.io/azure-dns-label-name: ${lower(data.azurerm_kubernetes_cluster.app_aks.dns_prefix)}
EOF

}

resource "azurerm_public_ip" "app_aks_ingress_ip" {
  name                = "${local.resource_prefix}-APP-INGRESS-pip"
  sku                 = "Standard"
  allocation_method   = "Static"
  resource_group_name = data.azurerm_kubernetes_cluster.app_aks.node_resource_group
  domain_name_label   = lower(data.azurerm_kubernetes_cluster.app_aks.dns_prefix)
  location            = data.azurerm_kubernetes_cluster.app_aks.location
}

resource "helm_release" "ingress" {
  chart      = "nginx-ingress"
  name       = "nginx-ingress"
  namespace  = "kube-system"
  repository = "https://helm.nginx.com/stable"
  version    = "0.14.0"

  set {
    name  = "rbac.create"
    value = "true"
  }

  set {
    name  = "service.enableHttp"
    value = "false"
  }

  set {
    name  = "controller.nodeSelector.beta\\.kubernetes\\.io/os"
    value = "linux"
  }

  set {
    name  = "defaultBackend.nodeSelector.beta\\.kubernetes\\.io/os"
    value = "linux"
  }

  set {
    name  = "controller.service.externalTrafficPolicy"
    value = "Local"
  }

  values = [local.config]
}

resource "helm_release" "cert_manager" {
  chart      = "cert-manager"
  name       = "cert-manager"
  namespace  = "kube-system"
  repository = "https://charts.jetstack.io"
  version    = "v1.9.1"

  set {
    name  = "installCRDs"
    value = "true"
  }
}

resource "kubernetes_manifest" "cluster_issuer" {
  depends_on = [helm_release.cert_manager]

  manifest = {
    "apiVersion" = "cert-manager.io/v1"
    "kind"       = "ClusterIssuer"
    "metadata" = {
      "name" = "letsencrypt-prod"
      "labels" = {
        "name" = "letsencrypt-prod"
      }
    }
    "spec" = {
      "acme" = {
        "email" = "mgray@solliance.net"
        "privateKeySecretRef" = {
          "name" = "letsencrypt-prod"
        }
        "server" = "https://acme-v02.api.letsencrypt.org/directory"
        "solvers" = [
          {
            "http01" = {
              "ingress" = {
                "class" = "nginx"
              }
            }
          },
        ]
      }
    }
  }
}