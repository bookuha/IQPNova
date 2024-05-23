resource "digitalocean_database_cluster" "postgres-cluster" {
  name       = "${var.name}-database-cluster"
  engine     = "pg"
  version    = "15"
  size       = var.database_size
  region     = var.region
  node_count = var.db_count

}
resource "digitalocean_database_firewall" "postgres-cluster-firewall" {
  cluster_id = digitalocean_database_cluster.postgres-cluster.id
  rule {
    type  = "tag"
    value = "${var.name}-apiserver"
  }
}

output "postgres-cluster-connection-string" {
  value     = digitalocean_database_cluster.postgres-cluster.uri
  sensitive = true
}
