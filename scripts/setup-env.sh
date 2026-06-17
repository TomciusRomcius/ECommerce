# Takes in service name(eg. OrderService)
addJwtEnvs() {
    echo "Jwt__ClientId=$jwtClientId" >> "../$1/.env"
    echo "Jwt__SecretClientId=$jwtClientSecret" >> "../$1/.env"
    echo "Jwt__Issuer=$jwtIssuer" >> "../$1/.env"
    echo "Jwt__Authority=$jwtAuthority" >> "../$1/.env"
    echo "Jwt__Audience=$jwtAudience" >> "../$1/.env"
}

productServiceUrl="http://product-service:8080"
paymentServiceUrl="http://payment-service:8080"
userServiceUrl="http://user-service:8080"
storeServiceUrl="http://store-service:8080"
orderServiceUrl="http://order-service:8080"

# Takes in service name(eg. OrderService)
addMicroserviceUrls() {
  echo "MicroserviceNetworkConfig__PaymentServiceUrl=$paymentServiceUrl" >> ../$1/.env
  echo "MicroserviceNetworkConfig__ProductServiceUrl=$productServiceUrl" >> ../$1/.env
  echo "MicroserviceNetworkConfig__UserServiceUrl=$userServiceUrl" >> ../$1/.env
  echo "MicroserviceNetworkConfig__OrderServiceUrl=$orderServiceUrl" >> ../$1/.env
  echo "MicroserviceNetworkConfig__StoreServiceUrl=$storeServiceUrl" >> ../$1/.env
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
echo "ASPNETCORE_URLS=http://+:8080" > ../BFF/.env
echo "Database__Host=read-db" >> ../BFF/.env
echo "Database__Port=5432" >> ../BFF/.env
echo "Database__Database=read_db" >> ../BFF/.env
echo "Database__Username=postgres" >> ../BFF/.env
echo "Database__Password=postgres" >> ../BFF/.env
echo "Kafka__Servers=kafka:9092" >> ../BFF/.env
addJwtEnvs "BFF"
addMicroserviceUrls "BFF"
echo "KeycloakAuth__TokenEndpoint=http://keycloak:8080/auth/realms/ecommerce-api/protocol/openid-connect/token" >> ../BFF/.env
echo "KeycloakAuth__ClientId=frontend" >> ../BFF/.env
echo "KeycloakAuth__RedirectUri=http://localhost:4200/auth/callback" >> ../BFF/.env
echo "S3__AwsAccessKeyId=test" >> ../BFF/.env
echo "S3__AwsSecretAccessKey=test" >> ../BFF/.env
echo "S3__Region=us-east-1" >> ../BFF/.env
echo "S3__ServiceUrl=http://localstack:4566" >> ../BFF/.env

# Payment service
echo "Database__Host=write-db" > ../PaymentService/.env 
echo "Database__Port=5432" >> ../PaymentService/.env 
echo "Database__Database=payment_db" >> ../PaymentService/.env 
echo "Database__Username=postgres" >> ../PaymentService/.env 
echo "Database__Password=postgres" >> ../PaymentService/.env 
echo "Kafka__Servers=kafka" >> ../PaymentService/.env 

read -p "Enter Stripe secret API key(or leave empty): " stripeApiKey
read -p "Enter Stripe webhook secret(or leave empty): " stripeWebhookSecret

echo "Stripe__ApiKey=$stripeApiKey" >> ../PaymentService/.env
echo "Stripe__WebhookSecret=$stripeWebhookSecret" >> ../PaymentService/.env
echo "Stripe__CheckoutSuccessUrl=http://localhost:4200/checkout?payment=success" >> ../PaymentService/.env
echo "Stripe__CheckoutCancelUrl=http://localhost:4200/checkout?payment=cancelled" >> ../PaymentService/.env

addJwtEnvs "PaymentService"
addMicroserviceUrls "PaymentService"

# Product service
echo "Database__Host=write-db" > ../ProductService/.env 
echo "Database__Port=5432" >> ../ProductService/.env 
echo "Database__Database=product_db" >> ../ProductService/.env 
echo "Database__Username=postgres" >> ../ProductService/.env 
echo "Database__Password=postgres" >> ../ProductService/.env 
echo "Kafka__Servers=kafka" >> ../ProductService/.env 

addJwtEnvs "ProductService"
addMicroserviceUrls "ProductService"

# Store service
echo "Database__Host=write-db" > ../StoreService/.env 
echo "Database__Port=5432" >> ../StoreService/.env 
echo "Database__Database=store_db" >> ../StoreService/.env 
echo "Database__Username=postgres" >> ../StoreService/.env 
echo "Database__Password=postgres" >> ../StoreService/.env 
echo "Kafka__Servers=kafka" >> ../StoreService/.env 

addJwtEnvs "StoreService"
addMicroserviceUrls "StoreService"

# User service
echo "Database__Host=write-db" > ../UserService/.env 
echo "Database__Port=5432" >> ../UserService/.env 
echo "Database__Database=user_db" >> ../UserService/.env 
echo "Database__Username=postgres" >> ../UserService/.env 
echo "Database__Password=postgres" >> ../UserService/.env 
echo "Kafka__Servers=kafka" >> ../UserService/.env 

addJwtEnvs "UserService"
addMicroserviceUrls "UserService"

# Order service
echo "Database__Host=write-db" > ../OrderService/.env 
echo "Database__Port=5432" >> ../OrderService/.env 
echo "Database__Database=order_db" >> ../OrderService/.env 
echo "Database__Username=postgres" >> ../OrderService/.env 
echo "Database__Password=postgres" >> ../OrderService/.env 
echo "Kafka__Servers=kafka" >> ../OrderService/.env

addJwtEnvs "OrderService"
addMicroserviceUrls "OrderService"

# Api worker
echo "Database__Host=api-read-db" > ../ApiWorker/.env
echo "Database__Port=5432" >> ../ApiWorker/.env
echo "Database__Database=read_db" >> ../ApiWorker/.env
echo "Database__Username=postgres" >> ../ApiWorker/.env
echo "Database__Password=postgres" >> ../ApiWorker/.env
echo "Kafka__Servers=kafka:9092" >> ../ApiWorker/.env
addMicroserviceUrls "ApiWorker"

# Frontend (written to .env but the service reads from system environment variables)

read -p "Enter Stripe publishable API key(or leave empty): " stripePubKey
echo "STRIPE_PUB_KEY=$stripePubKey" > ../frontend/.env