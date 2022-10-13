provider "kubernetes" {
  host = aws_eks_cluster.app_eks.endpoint
  cluster_ca_certificate = base64decode(aws_eks_cluster.app_eks.certificate_authority.0.data)
  exec {
    api_version = "client.authentication.k8s.io/v1alpha1"
    args= ["eks", "get-token", "--cluster-name", aws_eks_cluster.app_eks.name]
    command = "aws"
  }
}