# Takes in service name(eg. OrderService)
addJwtEnvs() {
    echo "Jwt__ClientId=$jwtClientId" >> "../services/$1/.env"
    echo "Jwt__SecretClientId=$jwtClientSecret" >> "../services/$1/.env"
    echo "Jwt__Issuer=$jwtIssuer" >> "../services/$1/.env"
    echo "Jwt__Authority=$jwtAuthority" >> "../services/$1/.env"
    echo "Jwt__Audience=$jwtAudience" >> "../services/$1/.env"
}

productServiceUrl="http://product-service:8080"
paymentServiceUrl="http://payment-service:8080"
userServiceUrl="http://user-service:8080"
storeServiceUrl="http://store-service:8080"
orderServiceUrl="http://order-service:8080"

# Takes in service name(eg. OrderService)
addMicroserviceUrls() {
  echo "MicroserviceNetworkConfig__PaymentServiceUrl=$paymentServiceUrl" >> ../services/$1/.env
  echo "MicroserviceNetworkConfig__ProductServiceUrl=$productServiceUrl" >> ../services/$1/.env
  echo "MicroserviceNetworkConfig__UserServiceUrl=$userServiceUrl" >> ../services/$1/.env
  echo "MicroserviceNetworkConfig__OrderServiceUrl=$orderServiceUrl" >> ../services/$1/.env
  echo "MicroserviceNetworkConfig__StoreServiceUrl=$storeServiceUrl" >> ../services/$1/.env
}

read -p "Enter localstack auth token (or leave empty for no S3 support): " localstackToken
echo "LOCALSTACK_AUTH_TOKEN=$localstackToken" > ../.env
echo "LOCALSTACK_VOLUME_DIR=./localstack_data" >> ../.env
read -p "Enter keycloak ecommerce-api client secret: " jwtClientSecret

jwtClientId=ecommerce-api
jwtClientSecret=secret
jwtAuthority="http://keycloak:8080/realms/ecommerce-api"
jwtAudience=ecommerce-api

# BFF
echo "ASPNETCORE_URLS=http://+:8080" > ../services/BFF/.env
echo "Database__Host=read-db" >> ../services/BFF/.env
echo "Database__Port=5432" >> ../services/BFF/.env
echo "Database__Database=read_db" >> ../services/BFF/.env
echo "Database__Username=postgres" >> ../services/BFF/.env
echo "Database__Password=postgres" >> ../services/BFF/.env
echo "Kafka__Servers=kafka:9092" >> ../services/BFF/.env
addJwtEnvs "BFF"
addMicroserviceUrls "BFF"
echo "KeycloakAuth__TokenEndpoint=http://keycloak:8080/auth/realms/ecommerce-api/protocol/openid-connect/token" >> ../services/BFF/.env
echo "KeycloakAuth__ClientId=frontend" >> ../services/BFF/.env
echo "KeycloakAuth__RedirectUri=http://localhost:4200/auth/callback" >> ../services/BFF/.env
echo "S3__AwsAccessKeyId=test" >> ../services/BFF/.env
echo "S3__AwsSecretAccessKey=test" >> ../services/BFF/.env
echo "S3__Region=us-east-1" >> ../services/BFF/.env
echo "S3__ServiceUrl=http://localstack:4566" >> ../services/BFF/.env

# Payment service
echo "Database__Host=write-db" > ../services/PaymentService/.env 
echo "Database__Port=5432" >> ../services/PaymentService/.env 
echo "Database__Database=payment_db" >> ../services/PaymentService/.env 
echo "Database__Username=postgres" >> ../services/PaymentService/.env 
echo "Database__Password=postgres" >> ../services/PaymentService/.env 
echo "Kafka__Servers=kafka" >> ../services/PaymentService/.env 

read -p "Enter Stripe secret API key(or leave empty): " stripeApiKey
read -p "Enter Stripe webhook secret(or leave empty): " stripeWebhookSecret

echo "Stripe__ApiKey=$stripeApiKey" >> ../services/PaymentService/.env
echo "Stripe__WebhookSecret=$stripeWebhookSecret" >> ../services/PaymentService/.env
echo "Stripe__CheckoutSuccessUrl=http://localhost:4200/checkout-success" >> ../services/PaymentService/.env
echo "Stripe__CheckoutCancelUrl=http://localhost:4200/checkout-cancelled" >> ../services/PaymentService/.env

addJwtEnvs "PaymentService"
addMicroserviceUrls "PaymentService"

# Product service
echo "Database__Host=write-db" > ../services/ProductService/.env 
echo "Database__Port=5432" >> ../services/ProductService/.env 
echo "Database__Database=product_db" >> ../services/ProductService/.env 
echo "Database__Username=postgres" >> ../services/ProductService/.env 
echo "Database__Password=postgres" >> ../services/ProductService/.env 
echo "Kafka__Servers=kafka" >> ../services/ProductService/.env 

addJwtEnvs "ProductService"
addMicroserviceUrls "ProductService"

# Store service
echo "Database__Host=write-db" > ../services/StoreService/.env 
echo "Database__Port=5432" >> ../services/StoreService/.env 
echo "Database__Database=store_db" >> ../services/StoreService/.env 
echo "Database__Username=postgres" >> ../services/StoreService/.env 
echo "Database__Password=postgres" >> ../services/StoreService/.env 
echo "Kafka__Servers=kafka" >> ../services/StoreService/.env 

addJwtEnvs "StoreService"
addMicroserviceUrls "StoreService"

# User service
echo "Database__Host=write-db" > ../services/UserService/.env 
echo "Database__Port=5432" >> ../services/UserService/.env 
echo "Database__Database=user_db" >> ../services/UserService/.env 
echo "Database__Username=postgres" >> ../services/UserService/.env 
echo "Database__Password=postgres" >> ../services/UserService/.env 
echo "Kafka__Servers=kafka" >> ../services/UserService/.env 

addJwtEnvs "UserService"
addMicroserviceUrls "UserService"

# Order service
echo "Database__Host=write-db" > ../services/OrderService/.env 
echo "Database__Port=5432" >> ../services/OrderService/.env 
echo "Database__Database=order_db" >> ../services/OrderService/.env 
echo "Database__Username=postgres" >> ../services/OrderService/.env 
echo "Database__Password=postgres" >> ../services/OrderService/.env 
echo "Kafka__Servers=kafka" >> ../services/OrderService/.env

addJwtEnvs "OrderService"
addMicroserviceUrls "OrderService"

# Api worker
echo "Database__Host=read-db" > ../services/ApiWorker/.env
echo "Database__Port=5432" >> ../services/ApiWorker/.env
echo "Database__Database=read_db" >> ../services/ApiWorker/.env
echo "Database__Username=postgres" >> ../services/ApiWorker/.env
echo "Database__Password=postgres" >> ../services/ApiWorker/.env
echo "Kafka__Servers=kafka:9092" >> ../services/ApiWorker/.env
addMicroserviceUrls "ApiWorker"

# Frontend (written to .env but the service reads from system environment variables)

read -p "Enter Stripe publishable API key(or leave empty): " stripePubKey
echo "STRIPE_PUB_KEY=$stripePubKey" > ../services/frontend/.env