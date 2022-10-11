variable "app_eks_subnet_cidrs" {
  type        = list(string)
  description = "The CIDR blocks for the EKS subnets. The EKS subnets are used to run the EKS cluster."
}

variable "app_vpc_cidr" {
  type        = string
  description = "The CIDR block for the VPC."
}

variable "location" {
  type        = string
  description = "The location/region where the resource group should exist."
}

variable "resource_prefix" {
  type        = string
  description = "The prefix used for all resources in this example."
}