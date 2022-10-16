module "eks" {
  source  = "terraform-aws-modules/eks/aws"
  version = "18.30.0"

  cluster_name    = "${local.resource_prefix}-APP-eks"
  cluster_version = "1.23"

  vpc_id     = module.vpc.vpc_id
  subnet_ids = module.vpc.private_subnets

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
      user_arn = aws_iam_group.eks_admin.arn
      username = "${local.resource_prefix}-APP-eks-admin"
      groups   = ["system:masters"]
    }
  ]

  tags = {
    resource_group = aws_resourcegroups_group.app_rg.name
  }
}

data "aws_caller_identity" "current" {}

resource "aws_iam_group" "eks_admin" {
  name = "${local.resource_prefix}-eks-admin"
}

resource "aws_iam_user_group_membership" "eks_admin" {
  groups = [aws_iam_group.eks_admin.name]
  user = data.aws_caller_identity.current.user_id
}
