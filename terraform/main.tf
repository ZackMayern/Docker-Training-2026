# resource "aws_instance" "anubhav_instance-2026" {
#   for_each      = var.instance_type
#   ami           = var.ami_id
#   instance_type = each.value

#   tags = {
#     Name = each.key
#   }
# }

# resource "aws_s3_bucket" "anubhav_instance_bucket-2026" {
#   bucket = "bucket-terraform-04202601"

#   tags = {
#     Name = "Anubhav-Instance-Bucket"
#   }
# }

resource "aws_ecr_repository" "app_repo_04202601" {
  name                 = "app_repo_04202601"
  image_tag_mutability = "MUTABLE"

  image_scanning_configuration {
    scan_on_push = true
  }

  tags = {
    Name = "app_repo_04202601"
  }
}

module "eks" {
  source  = "terraform-aws-modules/eks/aws"
  version = "~> 20.0"

  cluster_name    = "eks-cluster-04202601"
  cluster_version = "1.35"

  vpc_id     = module.vpc.vpc_id
  subnet_ids = module.vpc.private_subnets

  enable_irsa = true

  eks_managed_node_groups = {
    default = {
      desired_size = 2
      min_size     = 1
      max_size     = 3

      instance_types = ["t2.micro"]
    }
  }

  tags = {
    Environment = "dev"
  }
}

module "vpc" {
  source  = "terraform-aws-modules/vpc/aws"
  version = "~> 5.0"

  name = "eks-vpc-04202601"
  cidr = "10.0.0.0/16"

  azs             = ["ap-south-1a"]
  private_subnets = ["10.0.1.0/24"]
  public_subnets  = ["10.0.101.0/24"]

  enable_nat_gateway = true
  single_nat_gateway = true

  tags = {
    Environment = "dev"
  }
}