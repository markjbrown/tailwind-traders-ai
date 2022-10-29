output "kubelet_role" {
  value = aws_iam_role.eks_kubelet_role
}

output "zone_id" {
  value = aws_route53_zone.hostname.zone_id
}