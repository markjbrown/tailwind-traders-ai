resource "aws_iam_role" "eks_kubelet_role" {
  name = "${local.resource_prefix}-APP-kubelet-role"
  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Effect = "Allow"
        Principal = {
          Service = "ec2.amazonaws.com"
        }
        Action = "sts:AssumeRole"
      },
    ]
  })

  tags = {
    resource_group = "${local.resource_prefix}-APP-rg"
  }
}
