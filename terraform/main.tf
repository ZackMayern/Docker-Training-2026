# resource "aws_instance" "anubhav_instance-2026" {
#   for_each      = var.instance_type
#   ami           = var.ami_id
#   instance_type = each.value

#   tags = {
#     Name = each.key
#   }
# }

# resource "aws_s3_bucket" "anubhav_instance_bucket-2026" {
#   bucket = "bucket-terraform-2840263405"

#   tags = {
#     Name = "Anubhav-Instance-Bucket"
#   }
# }

resource "aws_ecr_repository" "app_repo_2840263405" {
  name                 = "app_repo_2840263405"
  image_tag_mutability = "MUTABLE"

  image_scanning_configuration {
    scan_on_push = true
  }

  tags = {
    Name = "app_repo_2840263405"
  }
}

module "eks" {
  source  = "terraform-aws-modules/eks/aws"
  version = "~> 20.0"

  cluster_name    = "asahu-eks-cluster"
  cluster_version = "1.35"

  vpc_id     = module.vpc.vpc_id
  subnet_ids = module.vpc.private_subnets

  enable_irsa = true

  eks_managed_node_groups = {
    default = {
      desired_size = 2
      min_size     = 1
      max_size     = 3

      instance_types = ["t3.medium"]
    }
  }

  tags = {
    Environment = "dev"
  }
}

module "vpc" {
  source  = "terraform-aws-modules/vpc/aws"
  version = "~> 5.0"

  name = "my-eks-vpc"
  cidr = "10.0.0.0/16"

  azs             = ["ap-south-1a", "ap-south-1b", "ap-south-1c"]
  private_subnets = ["10.0.1.0/24", "10.0.2.0/24", "10.0.3.0/24"]
  public_subnets  = ["10.0.101.0/24", "10.0.102.0/24", "10.0.103.0/24"]

  enable_nat_gateway = true
  single_nat_gateway = true

  tags = {
    Environment = "dev"
  }
}