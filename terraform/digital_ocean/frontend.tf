resource "digitalocean_droplet" "web" {
  image    = var.image
  name     = "web-${var.name}-${var.region}-frontend"
  region   = var.region
  size     = var.droplet_size
  ssh_keys = [data.digitalocean_ssh_key.main.id]
  //vpc_uuid  = digitalocean_vpc.web.id
  tags      = ["${var.name}-webserver"]
  user_data = templatefile("frontend-cloudinit.tpl", { api_ip = "${digitalocean_droplet.api.ipv4_address}" })
  lifecycle {
    create_before_destroy = true
  }
  depends_on = [
    digitalocean_droplet.api
  ]
}

output "frontend-url" {
  value     = digitalocean_droplet.web.ipv4_address
  sensitive = true
}


/*resource "digitalocean_certificate" "web" {
  name    = "${var.name}-certificate"
  type    = "lets_encrypt"
  domains = ["${var.subdomain}.${data.digitalocean_domain.web.name}"]
  lifecycle {
    create_before_destroy = true
  }
}*/


resource "digitalocean_firewall" "web" {
  name        = "${var.name}-web-22-80-433"
  droplet_ids = digitalocean_droplet.web.*.id

  inbound_rule {
    protocol         = "tcp"
    port_range       = "22"
    source_addresses = ["0.0.0.0/0", "::/0"]
  }

  inbound_rule {
    protocol         = "tcp"
    port_range       = "80"
    source_addresses = ["0.0.0.0/0", "::/0"]
  }

  inbound_rule {
    protocol         = "tcp"
    port_range       = "443"
    source_addresses = ["0.0.0.0/0", "::/0"]
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
    protocol              = "tcp"
    port_range            = "80"
    destination_addresses = ["0.0.0.0/0", "::/0"]
  }

  outbound_rule {
    protocol              = "tcp"
    port_range            = "443"
    destination_addresses = ["0.0.0.0/0", "::/0"]
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
