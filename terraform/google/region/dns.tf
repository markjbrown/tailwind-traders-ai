resource "aws_route53_record" "eks" {
  zone_id = var.zone_id
  name    = "eks.${var.domain_name}"
  type    = "A"
  ttl     = "300"
  records = [google_compute_address.ip.address]
}