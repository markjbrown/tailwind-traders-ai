resource "aws_eks_cluster" "app_eks" {
  name     = "${local.resource_prefix}-APP-eks"
  role_arn = aws_iam_role.eks_cluster_role.arn
  vpc_config {
    subnet_ids = [aws_subnet.app_eks_subnet.id]
  }
  tags = {
    resource_group = aws_resourcegroups_group.app_rg.name
  }
}

resource "aws_iam_role" "eks_cluster_role" {
  name               = "eks_cluster_role"
  assume_role_policy = <<EOF
{
    "Version": "2012-10-17",
    "Statement": [
        {
        "Effect": "Allow",
        "Principal": {
            "Service": "eks.amazonaws.com"
        },
        "Action": "sts:AssumeRole"
        }
    ]
}
EOF
}

resource "aws_iam_role_policy_attachment" "eks_cluster_role_policy" {
  role       = aws_iam_role.eks_cluster_role.name
  policy_arn = "arn:aws:iam::aws:policy/AmazonEKSClusterPolicy"
}

data "tls_certificate" "app_eks" {
  url = aws_eks_cluster.app_eks.identity[0].oidc[0].issuer
}

resource "aws_iam_openid_connect_provider" "app_eks" {
  client_id_list  = ["sts.amazonaws.com"]
  thumbprint_list = data.tls_certificate.app_eks.certificates.*.sha1_fingerprint
  url             = data.tls_certificate.app_eks.url
}

data "aws_iam_policy_document" "assume_role_policy" {
statement {
    effect  = "Allow"
    actions = ["sts:AssumeRoleWithWebIdentity"]

    condition {
      test     = "StringEquals"
      variable = "${aws_eks_cluster.app_eks.identity[0].oidc[0].issuer}/:sub"
      values   = ["system:serviceaccount:kube-system:aws-node"]
    }

    principals {
      identifiers = [aws_iam_openid_connect_provider.app_eks.arn]
      type        = "Federated"
    }
  }
}

resource "aws_iam_role" "app_eks" {
  name               = "app_eks"
  assume_role_policy = data.aws_iam_policy_document.assume_role_policy.json
}
