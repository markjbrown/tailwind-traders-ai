#resource "aws_route53_record" "eks" {
#  zone_id = var.zone_id
#  name    = "eks.${var.domain_name}"
#  type    = "A"
#
#  alias {
#    evaluate_target_health = true
#    name                   = data.aws_lb.ingress_lb.dns_name
#    zone_id                = data.aws_lb.ingress_lb.zone_id
#  }
#}