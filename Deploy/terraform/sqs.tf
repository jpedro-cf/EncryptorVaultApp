data "aws_iam_policy_document" "queue_policy" {
  statement {
    effect = "Allow"
    
    principals {
      type        = "*"
      identifiers = ["*"]
    }

    actions   = ["sqs:SendMessage"]
    resources = ["arn:aws:sqs:*:*:${var.SQS_QUEUE_NAME}"]

    condition {
      test     = "ArnEquals"
      variable = "aws:SourceArn"
      values   = [aws_s3_bucket.encryption_app_bucket.arn]
    }
  }
}

resource "aws_sqs_queue" "file_uploaded_queue" {
  name   = var.SQS_QUEUE_NAME
  visibility_timeout_seconds = 60
  policy = data.aws_iam_policy_document.queue_policy.json
}