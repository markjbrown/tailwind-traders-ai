terraform {
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 4.0.0"
    }
  }

  cloud {
    organization = "tailwind-traders"
    workspaces {
      name = "tailwind-traders-aws"
    }
  }
}

provider "aws" {}