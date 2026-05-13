resource "azurerm_container_registry" "main" {
  name                = "acr${var.project_name}${var.environment}"
  resource_group_name = var.resource_group_name
  location            = var.location
  sku                 = "Standard"

  # Disable admin account — AKS uses managed identity instead
  admin_enabled = false

  tags = {
    environment = var.environment
    managed_by  = "terraform"
  }
}
