terraform {
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 3.75.2"
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