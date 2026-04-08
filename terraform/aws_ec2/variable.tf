variable "instance_type"{
    description = "Map of instance names to instance types. Each entry creates one EC2 instance."
    type        = map(string)
    default = {
        "instance1" = "t2.micro"
    }
}

variable "ami_id" {
    type = string
}