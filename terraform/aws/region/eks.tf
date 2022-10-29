module "eks" {
  source  = "terraform-aws-modules/eks/aws"
  version = "18.30.0"

  cluster_name    = "${local.resource_prefix}-APP-eks"
  cluster_version = "1.23"

  vpc_id                    = module.vpc.vpc_id
  subnet_ids                = module.vpc.private_subnets
  manage_aws_auth_configmap = true

  eks_managed_node_group_defaults = {
    ami_type = "AL2_x86_64"

    attach_cluster_primary_security_group = true

    create_security_group = true
  }

  eks_managed_node_groups = {
    default = {
      name = "${local.resource_prefix}-APP-eksng"

      instance_types = ["t3.medium"]

      min_size     = 1
      max_size     = 2
      desired_size = 2

      iam_role_additional_policies = [
        "arn:aws:iam::aws:policy/AmazonDynamoDBFullAccess"
      ]

      pre_bootstrap_user_data = <<-EOT
      echo 'foo bar'
      EOT
    }
  }

  node_security_group_tags = {
    "kubernetes.io/cluster/${local.resource_prefix}-APP-eks" = null
  }

  aws_auth_users = [
    {
      userarn  = aws_iam_group.eks_admin.arn
      username = aws_iam_group.eks_admin.name
      groups   = ["system:masters"]
    },
    {
      userarn  = "arn:aws:iam::825357943300:user/mgray@solliance.net"
      username = "mgray@solliance.net"
      groups   = ["system:masters"]
    }
  ]

  tags = {
    resource_group = aws_resourcegroups_group.app_rg.name
  }
}

resource "helm_release" "ingress-nginx" {
  name       = "ingress-nginx"
  repository = "https://kubernetes.github.io/ingress-nginx"
  chart      = "ingress-nginx"
  version    = "4.3.0"
  namespace  = "kube-system"

  values = [yamlencode({
    rbac = {
      create = true
    }

    controller = {
      metrics = {
        enabled = true
      }
      service = {
        targetPorts = {
          http  = "http"
          https = "https"
        }
        annotations = {
          "service.beta.kubernetes.io/aws-load-balancer-type"                    = "nlb"
          "service.beta.kubernetes.io/aws-load-balancer-ssl-cert"                = ""
          "service.beta.kubernetes.io/aws-load-balancer-backend-protocol"        = "https"
          "service.beta.kubernetes.io/aws-load-balancer-ssl-ports"               = "https"
          "service.beta.kubernetes.io/aws-load-balancer-connection-idle-timeout" = "3600"
          "nginx.ingress.kubernetes.io/configuration-snippet"                    = <<SNIPPET
if ($http_x_forwarded_proto != 'https') {
  return 301 https://$host$request_uri;
}
SNIPPET
        }
      }
    }
  })]

  set {
    name  = "cluster.enabled"
    value = "true"
  }

  set {
    name  = "metrics.enabled"
    value = "true"
  }

  depends_on = [
    module.eks
  ]
}

resource "kubernetes_namespace" "cert_manager" {
  metadata {
    name = "cert-manager"
  }
}

resource "helm_release" "cert_manager" {
  name       = "cert-manager"
  repository = "https://charts.jetstack.io"
  chart      = "cert-manager"
  version    = "1.10.0"
  namespace  = kubernetes_namespace.cert_manager.metadata[0].name


  set {
    name  = "installCRDs"
    value = "true"
  }

  depends_on = [
    module.eks,
    helm_release.ingress-nginx
  ]
}

resource "kubectl_manifest" "clusterissuer_le_prod" {
  yaml_body = yamlencode({
    "apiVersion" = "cert-manager.io/v1"
    "kind"       = "ClusterIssuer"
    "metadata" = {
      "name" = "letsencrypt-prod"
    }
    "spec" = {
      "acme" = {
        "email" = "myemail@email.com"
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
          }
        ]
      }
    }
  })

  depends_on = [helm_release.cert_manager]
}

data "kubernetes_service" "ingress_nginx" {
  metadata {
    name      = "ingress-nginx-controller"
    namespace = "kube-system"
  }

  depends_on = [helm_release.ingress-nginx]
}

data "aws_eks_cluster_auth" "eks" {
  name = module.eks.cluster_id
}

data "aws_lb" "ingress_lb" {
  name = regex(
    "(^[^-]+)",
    data.kubernetes_service.ingress_nginx.status[0].load_balancer[0].ingress[0].hostname
  )[0]
}

resource "aws_iam_group" "eks_admin" {
  name = "eks-admin"
}

