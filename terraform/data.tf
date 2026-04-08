data "aws_ami" "linux" {
    most_recent = true
    filter {
        name   = "name"
        values = ["linux*"]
    }
}