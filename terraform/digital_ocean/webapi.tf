resource "digitalocean_droplet" "api" {
  image    = var.image
  name     = "api-${var.name}-${var.region}-backend"
  region   = var.region
  size     = var.droplet_size
  ssh_keys = [data.digitalocean_ssh_key.main.id]
  //vpc_uuid  = digitalocean_vpc.web.id
  tags = ["${var.name}-apiserver"]
  user_data = templatefile("backend-cloudinit.tpl", {
    host     = "${digitalocean_database_cluster.postgres-cluster.host}",
    port     = "${digitalocean_database_cluster.postgres-cluster.port}",
    database = "${digitalocean_database_cluster.postgres-cluster.database}",
    user     = "${digitalocean_database_cluster.postgres-cluster.user}",
    password = "${digitalocean_database_cluster.postgres-cluster.password}"
    },
  )

  lifecycle {
    create_before_destroy = true
  }
  depends_on = [
    digitalocean_database_cluster.postgres-cluster
  ]
}

resource "digitalocean_firewall" "api" {
  name        = "${var.name}-api-80-433-from-web"
  droplet_ids = digitalocean_droplet.api.*.id

  inbound_rule {
    protocol         = "tcp"
    port_range       = "22"
    source_addresses = ["0.0.0.0/0", "::/0"]
  }

  inbound_rule {
    protocol    = "tcp"
    port_range  = "80"
    source_tags = ["${var.name}-webserver"]
  }

  inbound_rule {
    protocol    = "tcp"
    port_range  = "443"
    source_tags = ["${var.name}-webserver"]
  }

  inbound_rule {
    protocol         = "icmp"
    source_addresses = ["0.0.0.0/0", "::/0"]
  }

  outbound_rule {
    protocol              = "tcp"
    port_range            = "22"
    destination_addresses = ["0.0.0.0/0", "::/0"]
  }

  outbound_rule {
    protocol         = "tcp"
    port_range       = "80"
    destination_tags = ["${var.name}-webserver"]
  }

  outbound_rule {
    protocol         = "tcp"
    port_range       = "443"
    destination_tags = ["${var.name}-webserver"]
  }

  outbound_rule {
    protocol              = "tcp"
    port_range            = "53"
    destination_addresses = ["0.0.0.0/0", "::/0"]
  }

  outbound_rule {
    protocol              = "udp"
    port_range            = "53"
    destination_addresses = ["0.0.0.0/0", "::/0"]
  }

  outbound_rule {
    protocol              = "icmp"
    destination_addresses = ["0.0.0.0/0", "::/0"]
  }
}

output "backend-url" {
  value     = digitalocean_droplet.api.ipv4_address
  sensitive = true
}
