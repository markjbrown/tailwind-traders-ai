terraform {
  required_providers {
    azurerm = {
      source = "hashicorp/azurerm"
      version = "~>3.21.1"
    }

    azuread = {
      source = "hashicorp/azuread"
      version = "~>2.28.1"
    }
  }

  cloud {
    organization = "tailwind-traders"
    workspaces {
      name = "tailwind-traders-azure"
    }
  }
}

provider "azurerm" {
  features {}
}

provider "azuread" {}
