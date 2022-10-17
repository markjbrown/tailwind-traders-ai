module "eks" {
  source  = "terraform-aws-modules/eks/aws"
  version = "18.30.0"

  cluster_name    = "${local.resource_prefix}-APP-eks"
  cluster_version = "1.23"

  vpc_id     = module.vpc.vpc_id
  subnet_ids = module.vpc.private_subnets
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
      max_size     = 1
      desired_size = 1

      pre_bootstrap_user_data = <<-EOT
      echo 'foo bar'
      EOT
    }
  }

  aws_auth_users = [
    {
      userarn = aws_iam_group.eks_admin.arn
      username = aws_iam_group.eks_admin.name
      groups = ["system:masters"]
    },
    {
      userarn = "arn:aws:iam::825357943300:user/mgray@solliance.net"
      username = "mgray@solliance.net"
      groups = ["system:masters"]
    }
  ]

  tags = {
    resource_group = aws_resourcegroups_group.app_rg.name
  }
}

module "lb_role" {
  source = "terraform-aws-modules/iam/aws//modules/iam-role-for-service-accounts-eks"

  role_name = "${local.resource_prefix}-APP-lb-role"
  attach_load_balancer_controller_policy = true

  oidc_providers = {
    main = {
      provider_arn = module.eks.oidc_provider_arn
      namespace_service_accounts = ["kube-system:aws-load-balancer-controller"]
    }
  }
}

resource "kubernetes_service_account" "service_account" {
  metadata {
    name = "aws-load-balancer-controller"
    namespace = "kube-system"
    labels = {
      "app.kubernetes.io/name" = "aws-load-balancer-controller"
      "app.kubernetes.io/component" = "controller"
    }
    annotations = {
      "eks.amazonaws.com/role-arn" = module.lb_role.role_arn
      "eks.amazonaws.com/sts-regional-endpoints" = "true"
    }
  }
}

resource "helm_release" "lb" {
    name = "aws-load-balancer-controller"
    namespace = "kube-system"
    chart = "aws-load-balancer-controller"
    repository = "https://aws.github.io/eks-charts"
    version = "2.4.0"
    depends_on = [
        kubernetes_service_account.service_account
    ]

  set {
    name  = "region"
    value = "us-east-1"
  }

  set {
    name  = "vpcId"
    value = module.vpc.vpc_id
  }

  set {
    name  = "image.repository"
    value = "602401143452.dkr.ecr.us-east-1.amazonaws.com/amazon/aws-load-balancer-controller"
  }

  set {
    name  = "serviceAccount.create"
    value = "false"
  }

  set {
    name  = "serviceAccount.name"
    value = "aws-load-balancer-controller"
  }

  set {
    name  = "clusterName"
    value = module.eks.cluster_id
  }
}

resource "aws_iam_group" "eks_admin" {
  name = "eks-admin"
}
