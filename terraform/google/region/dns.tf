resource "aws_route53_record" "gke" {
  zone_id = var.zone_id
  name    = "gke.${var.domain_name}"
  type    = "A"
  ttl     = "300"
  records = [google_compute_address.ip.address]
}