import os
from pathlib import Path

DATA_SEEDER_DIR = Path(__file__).resolve().parent
IMAGES_DIR = DATA_SEEDER_DIR / "images"

API_BASE_URL = os.environ.get("API_BASE_URL", "http://reverse-proxy")
KEYCLOAK_TOKEN_URL = os.environ.get(
    "KEYCLOAK_TOKEN_URL",
    "http://keycloak:8080/realms/ecommerce-api/protocol/openid-connect/token",
)
KEYCLOAK_CLIENT_ID = os.environ.get("KEYCLOAK_CLIENT_ID", "ecommerce-api")
KEYCLOAK_CLIENT_SECRET = os.environ.get("KEYCLOAK_CLIENT_SECRET", "secret")

REQUEST_TIMEOUT_SEC = int(os.environ.get("REQUEST_TIMEOUT_SEC", "30"))

SEED_STORE_PREFIX = "Seed Store"

SEED_STORES: list[dict[str, str]] = [
    {
        "displayName": f"{SEED_STORE_PREFIX} 1",
        "address": "101 Seed Street, City 1",
    },
    {
        "displayName": f"{SEED_STORE_PREFIX} 2",
        "address": "102 Seed Street, City 2",
    },
    {
        "displayName": f"{SEED_STORE_PREFIX} 3",
        "address": "103 Seed Street, City 3",
    },
]

STORE_NAME1, STORE_NAME2, STORE_NAME3 = (store["displayName"] for store in SEED_STORES)

CATEGORY_NAMES = ["Shoes", "Clothes"]
MANUFACTURER_NAMES = ["Nike", "Adidas", "Levi's", "Uniqlo"]

S3_BUCKET = os.environ.get("S3_BUCKET", "ecommerce-resources")
S3_ENDPOINT_URL = os.environ.get("S3_ENDPOINT_URL", "http://localstack:4566")
S3_REGION = os.environ.get("AWS_DEFAULT_REGION", "us-east-1")
