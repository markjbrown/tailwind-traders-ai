variable "location" {
  type = string
  description = "The location/region where the resource group should exist."
}

variable "resource_prefix" {
  type = string
  description = "The prefix used for all resources in this example."
}

variable "collection" {
  type = list(string)
}
