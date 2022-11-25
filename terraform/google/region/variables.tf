variable "domain_name" {
  type        = string
  description = "The domain name for the cluster."
}

variable "location" {
  type        = string
  description = "The location/region where the resource group should exist."
}

variable "resource_prefix" {
  type        = string
  description = "The prefix used for all resources in this example."
}

variable "zone_id" {
  type        = string
  description = "The ID of the zone to manage."
}
