output "ecr_repository_url" {
  value = aws_ecr_repository.app_repo_04202601.repository_url
}

output "eks_cluster_name" {
  value = module.eks.cluster_name
}

output "vpc_id" {
  value = module.vpc.vpc_id
}