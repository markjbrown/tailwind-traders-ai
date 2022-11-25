data "aws_route53_zone" "hostname" {
  name = local.domain_name
}