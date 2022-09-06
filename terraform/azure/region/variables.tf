variable "app_aks_subnet_address_prefixes" {
  type = list(string)
  description = "The address prefix to use for the AKS subnet."
}

variable "app_vnet_address_space" {
  type = list(string)
  description = "The address space that is used by the application virtual network."
}

variable "location" {
  type = string
  description = "The location/region where the resource group should exist."
}

variable "resource_prefix" {
  type = string
  description = "The prefix used for all resources in this example."
}