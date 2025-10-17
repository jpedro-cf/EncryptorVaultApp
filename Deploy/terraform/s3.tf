data "aws_caller_identity" "current" {}

resource "aws_s3_bucket" "encryption_app_bucket" {
    bucket = var.DEV_MODE ? "encryption-app-bucket" : "encryption-app-bucket-${data.aws_caller_identity.current.account_id}"
}

resource "aws_s3_bucket_notification" "bucket_notification" {
  bucket = aws_s3_bucket.encryption_app_bucket.id

  queue {
    queue_arn     = aws_sqs_queue.file_uploaded_queue.arn
    events        = ["s3:ObjectCreated:*"]
  }
}