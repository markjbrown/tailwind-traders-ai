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

  aws_auth_roles = [
    {
      role_arn = aws_iam_role.eks-cluster-owner.arn
      username = "eks-cluster-owner"
      groups   = ["system:masters"]
    }
  ]

  tags = {
    resource_group = aws_resourcegroups_group.app_rg.name
  }
}

data "aws_caller_identity" "current" {}

resource "aws_iam_role" "eks-cluster-owner" {
  name = "eks-cluster-owner"
  assume_role_policy = "${data.aws_iam_policy_document.eks-cluster-owner-assume-role.json}"

  tags {
    Name        = "eks-cluster-owner"
  }
}

data "aws_iam_policy_document" "eks-cluster-owner-assume-role" {
  statement {
    sid = "AllowAssumeRole"
    effect = "Allow"
    actions = [
      "sts:AssumeRole",
    ]
    principals {
      type = "AWS"
      identifiers = [
        "arn:aws:iam::${data.aws_caller_identity.current.account_id}:root",
      ]
    }
  }
}

resource "aws_iam_role_policy_attachment" "eks-administration" {
  role       = "${aws_iam_role.eks-cluster-owner.name}"
  policy_arn = "${aws_iam_policy.eks-administration.arn}"
}

resource "aws_iam_policy" "eks-administration" {
  name   = "eks-administration"
  path   = "/"
  policy = "${data.aws_iam_policy_document.eks-administration.json}"
}

data "aws_iam_policy_document" "eks-administration" {
  statement {
    sid = "AllowEKSManagement"
    effect = "Allow"
    actions = [
      "eks:*",
    ]
    resources = [
      "*",
    ]
  }
}

