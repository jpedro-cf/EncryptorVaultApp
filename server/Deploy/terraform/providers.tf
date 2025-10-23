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

  s3_use_path_style        = var.DEV_MODE ? true : null
  skip_credentials_validation = var.DEV_MODE ? true : null
  skip_metadata_api_check     = var.DEV_MODE ? true : null
  skip_requesting_account_id  = var.DEV_MODE ? true : null

  endpoints {
    s3 = var.DEV_MODE ? "http://localhost:4566" : null
    sqs = var.DEV_MODE ? "http://localhost:4566" : null
    sts = var.DEV_MODE ? "http://localhost:4566" : null
  }
}
