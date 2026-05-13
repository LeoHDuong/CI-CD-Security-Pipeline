variable "location" {
  description = "Azure region"
  type        = string
  default     = "eastus"
}

variable "environment" {
  description = "Deployment environment"
  type        = string
  default     = "staging"
}

variable "project_name" {
  description = "Project identifier used in resource names"
  type        = string
  default     = "myapp"
}

variable "aks_node_count" {
  description = "Number of AKS worker nodes"
  type        = number
  default     = 2
}

variable "aks_vm_size" {
  description = "VM size for AKS nodes"
  type        = string
  default     = "Standard_D2s_v3"
}

variable "kubernetes_version" {
  description = "Kubernetes version for AKS"
  type        = string
  default     = "1.29"
}
