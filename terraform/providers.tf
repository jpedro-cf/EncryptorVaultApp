terraform {
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "6.0.0"
    }
  }
}

provider "aws" {
  region     = var.AWS_REGION
  access_key = var.AWS_ACCESS
  secret_key = var.AWS_SECRET

  s3_use_path_style        = var.AWS_ENDPOINT != null ? true : null
  skip_credentials_validation = var.AWS_ENDPOINT != null ? true : null
  skip_metadata_api_check     = var.AWS_ENDPOINT != null ? true : null
  skip_requesting_account_id  = var.AWS_ENDPOINT != null ? true : null

  endpoints {
    s3 = var.AWS_ENDPOINT != null ? var.AWS_ENDPOINT : null
    sts = var.AWS_ENDPOINT != null ? var.AWS_ENDPOINT : null
  }
}
