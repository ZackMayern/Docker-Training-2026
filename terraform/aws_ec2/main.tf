resource "aws_instance" "anubhav_instance-2026" {
    for_each      = var.instance_type
    ami           = var.ami_id
    instance_type = each.value

    tags = {
        Name = each.key
    }
}
