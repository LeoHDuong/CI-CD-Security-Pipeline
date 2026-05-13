terraform {
  required_version = ">= 1.5.0"

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.100"
    }
  }

  # Remote state backend - enabled in CI/CD pipeline via service principal auth
  # Comment this block in locally once `az login` is configured, or leave commented for local dev
  # backend "azurerm" {
  #     resource_group_name  = "rg-tfstate"
  #     storage_account_name = "tfstatemyapp"
  #     container_name       = "tfstate"
  #     key                  = "cicd-pipeline.tfstate"
  # }
}

provider "azurerm" {
  features {}
}