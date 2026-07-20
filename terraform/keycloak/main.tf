variable "keycloak_admin_username" {
    type = string
    description = "Keycloak admin username"
}

variable "keycloak_admin_password" {
    type = string
    description = "Keycloak admin password"
}

variable "keycloak_url" {
    type = string
    description = "Keycloak URL"
}

variable "ecommerce_api_client_secret" {
    type = string
    description = "Client secret of the ecommerce-api secret"
}

variable "frontend_root_url" {
  type        = string
  description = "Root URL of the Angular frontend (used as Keycloak client root/base URL)"
  default     = "http://localhost:4200"
}

variable "frontend_valid_redirect_uris" {
  type        = list(string)
  description = "Allowed OAuth redirect URIs for the public frontend client"
  default = [
    "http://localhost:4200/*",
  ]
}

variable "frontend_web_origins" {
  type        = list(string)
  description = "Allowed CORS origins for the public frontend client"
  default = [
    "http://localhost:4200",
  ]
}

variable "frontend_valid_post_logout_redirect_uris" {
  type        = list(string)
  description = "Allowed post-logout redirect URIs for the public frontend client"
  default = [
    "http://localhost:4200/*",
  ]
}

terraform {
  required_providers {
    keycloak = {
      source  = "mrparkers/keycloak"
      version = ">= 4.4.0"
    }
  }
}

provider "keycloak" {
    client_id     = "admin-cli"
    username = var.keycloak_admin_username
    password = var.keycloak_admin_password
    url           = var.keycloak_url
}

resource "keycloak_realm" "ecommerce_api" {
  realm                = "ecommerce-api"
  enabled              = true
  display_name         = "Ecommerce"
  login_theme          = "ecommerce"
  registration_allowed = true
}

resource "keycloak_openid_client" "ecommerce-api" {
  realm_id            = keycloak_realm.ecommerce_api.id
  client_id           = "ecommerce-api"
  client_secret = var.ecommerce_api_client_secret

  name                = "Ecommerce API"
  enabled             = true

  access_type         = "CONFIDENTIAL"
  service_accounts_enabled = true
}

resource "keycloak_openid_client" "frontend" {
  realm_id  = keycloak_realm.ecommerce_api.id
  client_id = "frontend"

  name    = "Frontend"
  enabled = true

  access_type               = "PUBLIC"
  standard_flow_enabled     = true
  full_scope_allowed        = true
  use_refresh_tokens        = true
  pkce_code_challenge_method = "S256"

  root_url = var.frontend_root_url
  base_url = var.frontend_root_url

  valid_redirect_uris             = var.frontend_valid_redirect_uris
  web_origins                     = var.frontend_web_origins
  valid_post_logout_redirect_uris = var.frontend_valid_post_logout_redirect_uris
}

// TODO: use audience: frontend
resource "keycloak_openid_client_default_scopes" "frontend_client_default_scopes" {
  realm_id  = keycloak_realm.ecommerce_api.id
  client_id = keycloak_openid_client.frontend.id

  default_scopes = [
    "openid",
    "profile",
    "email",
    "basic",
    keycloak_openid_client_scope.audience_client_scope.name,
    keycloak_openid_client_scope.realm_roles_client_scope.name,
  ]
}

resource "keycloak_role" "ecommerce_admin_role" {
  realm_id    = keycloak_realm.ecommerce_api.id
  name        = "ecommerce-admin"
}

resource "keycloak_user" "admin_user" {
  realm_id       = keycloak_realm.ecommerce_api.id
  username       = "adminuser"
  email          = "adminuser@example.com"
  first_name     = "Admin"
  last_name      = "User"
  email_verified = true
  enabled        = true

  initial_password {
    value     = "adminuser"
    temporary = false
  }
}

resource "keycloak_user_roles" "admin_user_roles" {
  realm_id = keycloak_realm.ecommerce_api.id
  user_id  = keycloak_user.admin_user.id
  role_ids = [keycloak_role.ecommerce_admin_role.id]
}

resource "keycloak_openid_hardcoded_role_protocol_mapper" "hardcoded_admin_role" {
  realm_id  = keycloak_realm.ecommerce_api.id
  client_id = keycloak_openid_client.ecommerce-api.id
  name      = "hardcoded-admin-role"
  
  role_id = keycloak_role.ecommerce_admin_role.id
}

# Protocol mapper to include realm roles in the token
resource "keycloak_openid_user_realm_role_protocol_mapper" "realm_roles_mapper" {
  realm_id  = keycloak_realm.ecommerce_api.id
  client_id = keycloak_openid_client.ecommerce-api.id
  name      = "realm-roles-mapper"
  
  claim_name       = "roles"
  multivalued      = true
  claim_value_type = "String"
}

resource "keycloak_openid_client_scope" "audience_client_scope" {
  realm_id = keycloak_realm.ecommerce_api.id
  name     = "audience-client-scope"
}

resource "keycloak_openid_client_scope" "realm_roles_client_scope" {
  realm_id = keycloak_realm.ecommerce_api.id
  name     = "realm-roles-client-scope"
}

resource "keycloak_openid_user_realm_role_protocol_mapper" "realm_roles_scope_mapper" {
  realm_id        = keycloak_realm.ecommerce_api.id
  client_scope_id = keycloak_openid_client_scope.realm_roles_client_scope.id
  name            = "realm-roles-mapper"

  claim_name       = "roles"
  multivalued      = true
  claim_value_type = "String"
}

resource "keycloak_openid_audience_protocol_mapper" "audience_mapper" {
  realm_id        = keycloak_realm.ecommerce_api.id
  client_scope_id = keycloak_openid_client_scope.audience_client_scope.id
  name            = "audience-mapper"
  included_client_audience = keycloak_openid_client.ecommerce-api.client_id
}

resource "keycloak_openid_client_default_scopes" "ecommerce_api_client_default_scopes" {
  realm_id  = "ecommerce-api"
  client_id = keycloak_openid_client.ecommerce-api.id

  default_scopes = [
    keycloak_openid_client_scope.audience_client_scope.name,
  ]
}
