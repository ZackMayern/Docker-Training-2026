variable ami_id {
  description = "The ID of the AMI to use for the EC2 instance."
  type        = string
}

variable instance_type {
  description = "The type of the EC2 instance."
  type        = map(string)
  default     = {
    "instance1"    = "t2.micro"
  }
}

variable "vpc_id" {}

variable "private_subnet_ids" {
  type = list(string)
}