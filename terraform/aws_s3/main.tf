resource "aws_s3_bucket" "anubhav_instance_bucket-2026" {
    bucket = var.bucketname

    tags = {
        Name = var.bucketname
    }
}