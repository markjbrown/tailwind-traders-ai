resource "aws_eks_cluster" "app_eks" {
  name     = "${local.resource_prefix}-APP-eks"
  role_arn = aws_iam_role.eks_cluster_role.arn
  vpc_config {
    subnet_ids = [aws_subnet.app_eks_subnet1.id, aws_subnet.app_eks_subnet2.id]
  }
  tags = {
    resource_group = aws_resourcegroups_group.app_rg.name
  }

  depends_on = [
    aws_iam_role_policy_attachment.eks_cluster_role_policy,
    aws_iam_role_policy_attachment.eks_service_role_policy
  ]
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

resource "aws_iam_role_policy_attachment" "eks_service_role_policy" {
  role       = aws_iam_role.eks_cluster_role.name
  policy_arn = "arn:aws:iam::aws:policy/AmazonEKSServicePolicy"
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

resource "aws_eks_node_group" "app_eks_node_group" {
    cluster_name    = aws_eks_cluster.app_eks.name
    node_group_name = "${local.resource_prefix}-APP-eksng"
    node_role_arn   = aws_iam_role.app_eks_node_role.arn
    subnet_ids      = [aws_subnet.app_eks_subnet1.id, aws_subnet.app_eks_subnet2.id]
    scaling_config {
        desired_size = 1
        max_size     = 1
        min_size     = 1
    }

    tags = {
        resource_group = aws_resourcegroups_group.app_rg.name
    }

  depends_on = [
    aws_iam_role_policy_attachment.app_eks_node_role_policy,
    aws_iam_role_policy_attachment.app_eks_node_role_cni_policy,
    aws_iam_role_policy_attachment.app_eks_node_role_ecr_policy,
  ]
}

resource "aws_iam_role" "app_eks_node_role" {
  name               = "app_eks_node_role"
  assume_role_policy = <<EOF
{
    "Version": "2012-10-17",
    "Statement": [
        {
        "Effect": "Allow",
        "Principal": {
            "Service": "ec2.amazonaws.com"
        },
        "Action": "sts:AssumeRole"
        }
    ]
}
EOF
}

resource "aws_iam_role_policy_attachment" "app_eks_node_role_policy" {
  role       = aws_iam_role.app_eks_node_role.name
  policy_arn = "arn:aws:iam::aws:policy/AmazonEKSWorkerNodePolicy"
}

resource "aws_iam_role_policy_attachment" "app_eks_node_role_cni_policy" {
  role       = aws_iam_role.app_eks_node_role.name
  policy_arn = "arn:aws:iam::aws:policy/AmazonEKS_CNI_Policy"
}

resource "aws_iam_role_policy_attachment" "app_eks_node_role_ecr_policy" {
  role       = aws_iam_role.app_eks_node_role.name
  policy_arn = "arn:aws:iam::aws:policy/AmazonEC2ContainerRegistryReadOnly"
}

resource "aws_security_group" "cluster_sg" {
  name        = "${local.resource_prefix}-APP-csg"
  description = "Security group for the cluster"
  vpc_id      = aws_vpc.app_vpc.id

  egress {
    from_port = 0
    protocol  = "-1"
    to_port   = 0
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = {
    Name = "${local.resource_prefix}-APP-csg"
      resource_group = aws_resourcegroups_group.app_rg.name
  }
}

resource "aws_security_group" "node_sg" {
  name = "${local.resource_prefix}-APP-nsg"
  vpc_id = aws_vpc.app_vpc.id

  egress {
    from_port = 0
    protocol  = "-1"
    to_port   = 0
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = map(
    "Name", "${local.resource_prefix}-APP-nsg",
    "kubernetes.io/cluster/${aws_eks_cluster.app_eks.name}", "owned",
    "resource_group", aws_resourcegroups_group.app_rg.name
  )
}

resource "aws_security_group_rule" "cluster_ingress" {
  cidr_blocks = ["0.0.0.0/0"]
  description = "Allow inbound traffic from anywhere"
  from_port   = 443
  protocol    = "tcp"
  security_group_id = aws_security_group.cluster_sg.id
  to_port     = 443
  type        = "ingress"
}

resource "aws_security_group_rule" "node_ingress_self" {
    description              = "Allow node to communicate with each other"
    from_port                = 0
    protocol                 = "-1"
    security_group_id        = aws_security_group.node_sg.id
    source_security_group_id = aws_security_group.node_sg.id
    to_port                  = 65535
    type                     = "ingress"
}

resource "aws_security_group_rule" "node_ingress_cluster_https" {
    description              = "Allow worker Kubelets and pods to receive communication from the cluster control plane"
    from_port                = 443
    protocol                 = "tcp"
    security_group_id        = aws_security_group.node_sg.id
    source_security_group_id = aws_security_group.cluster_sg.id
    to_port                  = 443
    type                     = "ingress"
}

resource "aws_security_group_rule" "node_ingress_cluster" {
    description              = "Allow worker Kubelets and pods to receive communication from the cluster control plane"
    from_port                = 1025
    protocol                 = "tcp"
    security_group_id        = aws_security_group.node_sg.id
    source_security_group_id = aws_security_group.cluster_sg.id
    to_port                  = 65535
    type                     = "ingress"
}

resource "aws_security_group_rule" "cluster_ingress_node" {
  description              = "Allow pods to communicate with the worker node"
  from_port                = 1025
  protocol                 = "tcp"
  security_group_id        = aws_security_group.cluster_sg.id
  source_security_group_id = aws_security_group.node_sg.id
  to_port                  = 65535
  type                     = "ingress"
}

data "aws_caller_identity" "current" {}

resource "kubernetes_config_map" "aws_auth" {
  metadata {
    name = "aws-auth"
    namespace = "kube-system"
  }

  data = {
    mapRoles = <<EOT
    - rolearn: ${aws_iam_role.app_eks_node_role.arn}
      username: system:node:{{EC2PrivateDNSName}}
      groups:
        - system:bootstrappers
        - system:nodes
    EOT
    mapUsers = <<EOT
    - userarn: ${data.aws_caller_identity.current.arn}
      username: ${data.aws_caller_identity.current.id}
      groups:
        - system:masters
    - userarn: arn:aws:iam::825357943300:user/mgray@solliance.net
      username: mgray@solliance.net
      groups:
        - system:masters
EOT
  }
}
