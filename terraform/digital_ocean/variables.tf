variable "do_token" {
  type        = string
  description = "Digital Ocean personal access token"
  default     = "<token_string>"
}

variable "name" {
  type        = string
  description = "Infrastructure"
  default     = "iqp"
}

variable "region" {
  type    = string
  default = "ams2"
}

variable "droplet_count" {
  type    = number
  default = 1
}

variable "image" {
  type        = string
  description = "OS to install on the servers"
  default     = "ubuntu-24-04-x64"
}

variable "droplet_size" {
  type    = string
  default = "s-1vcpu-1gb"
}

variable "ssh_key" {
  type = string
}

variable "db_count" {
  type    = number
  default = 1
}

variable "database_size" {
  type    = string
  default = "db-s-1vcpu-1gb"
}
