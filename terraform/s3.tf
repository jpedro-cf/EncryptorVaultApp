data "aws_caller_identity" "current" {}

resource "aws_s3_bucket" "secure_vault_bucket" {
    bucket = var.AWS_ENDPOINT != null ? "secure-vault-bucket" : "secure-vault-bucket-${data.aws_caller_identity.current.account_id}"
}