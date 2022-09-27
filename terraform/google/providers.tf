terraform {
  required_providers {
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

provider "google" {}