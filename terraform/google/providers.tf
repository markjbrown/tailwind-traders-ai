terraform {
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 4.0.0"
    }
    google = {
      source  = "hashicorp/google"
      version = "~> 4.38.0"
    }
  }

  cloud {
    organization = "tailwind-traders"
    workspaces {
      name = "tailwind-traders-google"
    }
  }
}

provider "aws" {}

provider "google" {}

provider "google-beta" {}