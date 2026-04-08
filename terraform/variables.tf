variable "ami_id" {
  description = "The ID of the AMI to use for the EC2 instance."
  type        = string
}

variable "instance_type" {
  description = "Map of instance names to instance types. Each entry creates one EC2 instance."
  type        = map(string)
  default = {
    "instance1" = "t2.micro"
  }
}

variable "bucket" {
  description = "The name of the S3 bucket to create."
  type        = string
  default = "bucket-terraform-04202601"
}

variable "create_s3" {
  description = "Set to true to create S3 bucket, false to skip."
  type        = bool
  default     = true
}