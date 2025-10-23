data "aws_caller_identity" "current" {}

resource "aws_s3_bucket" "encryption_app_bucket" {
    bucket = var.DEV_MODE ? "encryption-app-bucket" : "encryption-app-bucket-${data.aws_caller_identity.current.account_id}"
}