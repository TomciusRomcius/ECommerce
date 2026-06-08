#!/usr/bin/env python3
import logging
import mimetypes
import sys
from pathlib import Path

import boto3
from botocore.exceptions import ClientError
from mypy_boto3_s3.client import S3Client

import config
from client import ApiClient

logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s %(levelname)s %(message)s",
)
logger = logging.getLogger(__name__)

def _index_by_name(items: list[dict], id_field: str, name_key: str = "name") -> dict[str, int]:
    return {
        item[name_key]: item[id_field]
        for item in items
        if item.get(name_key) and item.get(id_field) is not None
    }

# returns dict(name, id)
def _load_categories(client: ApiClient) -> dict[str, int]:
    data = client.get_json("/productservice/categories", params={"pageNumber": 1})
    if isinstance(data, list):
        return _index_by_name(data, "categoryId")
    return {}

# returns dict(name, id)
def _load_manufacturers(client: ApiClient) -> dict[str, int]:
    data = client.get_json("/productservice/manufacturer", params={"pageNumber": 1})
    print(data);
    if isinstance(data, list):
        return _index_by_name(data, "manufacturerId")
    return {}

# returns dict(name, id)
def _load_products(client: ApiClient) -> dict[str, int]:
    data = client.get_json(
        "/productservice/product",
        params={"pageNumber": 1, "pageSize": 100},
    )
    if isinstance(data, dict) and isinstance(data.get("data"), list):
        return _index_by_name(data["data"], "productId")
    return {}

# returns dict(name, id)
def _load_store_locations(client: ApiClient) -> dict[str, int]:
    data = client.get_json("/storeservice/storelocation", params={"pageNumber": 1})
    if not isinstance(data, list):
        return {}
    return {
        loc["displayName"]: loc["storeLocationId"]
        for loc in data
        if loc.get("displayName") and loc.get("storeLocationId") is not None
    }

# returns set(productId)
def _load_product_links(client: ApiClient, store_location_id: int) -> set[int]:
    product_ids: set[int] = set()
    page_number = 1

    while True:
        data = client.get_json(
            "/storeservice/availableproducts",
            params={"storeLocationId": store_location_id, "pageNumber": page_number},
        )
        if not isinstance(data, dict):
            break

        items = data.get("data")
        if not isinstance(items, list):
            break

        for link in items:
            if link.get("productId") is not None:
                product_ids.add(link["productId"])

        if not data.get("hasNextPage"):
            break
        page_number += 1

    return product_ids

# Returns (id, error)
def find_or_create_category(
    client: ApiClient, existing: dict[str, int], name: str
) -> tuple[int | None, bool]:
    if name in existing:
        logger.info("Category %s already exists (id=%s)", name, existing[name])
        return existing[name], False

    result = client.post_json("/productservice/categories", {"name": name})
    if result and "categoryId" in result:
        category_id = result["categoryId"]
        existing[name] = category_id
        logger.info("Created category %s (id=%s)", name, category_id)
        return category_id, False

    logger.error("Failed to create category %s", name)
    return None, True

# Returns (id, error)
def find_or_create_manufacturer(
    client: ApiClient, existing: dict[str, int], name: str
) -> tuple[int | None, bool]:
    if name in existing:
        logger.info("Manufacturer %s already exists (id=%s)", name, existing[name])
        return existing[name], False

    result = client.post_json("/productservice/manufacturer", {"name": name})
    if result and "manufacturerId" in result:
        manufacturer_id = result["manufacturerId"]
        existing[name] = manufacturer_id
        logger.info("Created manufacturer %s (id=%s)", name, manufacturer_id)
        return manufacturer_id, False

    logger.error("Failed to create manufacturer %s", name)
    return None, True

# Returns (id, error)
def _product_image_keys(product_id: int, image_count: int) -> list[str]:
    return [f"{product_id}_{order}" for order in range(image_count)]


def find_or_create_product(
    client: ApiClient,
    existing: dict[str, int],
    name: str,
    description: str,
    price: float,
    manufacturer_id: int,
    category_id: int,
    image_count: int,
) -> tuple[int | None, bool]:
    if name in existing:
        logger.info("Product %s already exists (id=%s)", name, existing[name])
        return existing[name], False

    result = client.post_json(
        "/productservice/product",
        {
            "name": name,
            "description": description,
            "price": price,
            "manufacturerId": manufacturer_id,
            "categoryId": category_id,
            "imageKeys": [],
            "imageCount": image_count,
        },
    )
    if not result or "productId" not in result:
        logger.error("Failed to create product %s", name)
        return None, True

    product_id = result["productId"]
    existing[name] = product_id
    image_keys = _product_image_keys(product_id, image_count)
    logger.info(
        "Created product %s (id=%s) with image keys %s",
        name,
        product_id,
        image_keys,
    )
    return product_id, False

# Returns (id, error)
def find_or_create_store_location(
    client: ApiClient, existing: dict[str, int], display_name: str, address: str
) -> tuple[int | None, bool]:
    if display_name in existing:
        logger.info(
            "Store location %s already exists (id=%s)",
            display_name,
            existing[display_name],
        )
        return existing[display_name], False

    result = client.post_json(
        "/storeservice/storelocation",
        {"displayName": display_name, "address": address},
    )
    if result and "storeLocationId" in result:
        store_location_id = result["storeLocationId"]
        existing[display_name] = store_location_id
        logger.info(
            "Created store location %s (id=%s)", display_name, store_location_id
        )
        return store_location_id, False

    logger.error("Failed to create store location %s", display_name)
    return None, True

# Returns true if fails
def link_product_to_store(
    client: ApiClient,
    existing_links: set[int],
    store_location_id: int,
    product_id: int,
    stock: int,
) -> bool:
    if product_id in existing_links:
        logger.info(
            "Product %s already linked to store location %s",
            product_id,
            store_location_id,
        )
        return False

    result = client.post_json(
        "/storeservice/availableproducts",
        {
            "storeLocationId": store_location_id,
            "productId": product_id,
            "stock": stock,
        },
    )
    if result is not None:
        existing_links.add(product_id)
        logger.info(
            "Linked product %s to store location %s", product_id, store_location_id
        )
        return False

    logger.error(
        "Failed to link product %s to store location %s",
        product_id,
        store_location_id,
    )
    return True

SEED_PRODUCTS: list[dict] = [
    {
        "name": "Nike Air Zoom Pegasus 41",
        "description": "Men's road running shoe with responsive Zoom Air cushioning and breathable mesh upper.",
        "price": 129.99,
        "manufacturer": "Nike",
        "category": "Shoes",
        "stores": {config.STORE_NAME1: 12, config.STORE_NAME2: 8, config.STORE_NAME3: 15},
        "images": ["images/nike-air-zoom-pegasus-42_0.jpg"],
    },
    {
        "name": "Adidas Ultraboost 5",
        "description": "Women's road running shoe with Boost midsole and Primeknit upper for all-day comfort.",
        "price": 189.99,
        "manufacturer": "Adidas",
        "category": "Shoes",
        "stores": {config.STORE_NAME1: 5, config.STORE_NAME2: 10, config.STORE_NAME3: 7},
        "images": ["images/adidas-ultraboost-5_0.jpg"],
    },
    {
        "name": "Uniqlo Supima Cotton Crew Neck T-Shirt",
        "description": "Men's short-sleeve crew neck tee made from soft Supima cotton with a relaxed fit.",
        "price": 19.90,
        "manufacturer": "Uniqlo",
        "category": "Clothes",
        "stores": {config.STORE_NAME1: 40, config.STORE_NAME2: 25},
        "images": ["images/uniqlo-supima-cotton-crew-neck-t-shirt_0.jpg"],
    },
    {
        "name": "Uniqlo 3D Knit Cotton Boat Neck Top",
        "description": "Women's boat neck top in lightweight 3D knit cotton with a clean, minimal silhouette.",
        "price": 29.90,
        "manufacturer": "Uniqlo",
        "category": "Clothes",
        "stores": {config.STORE_NAME2: 30, config.STORE_NAME3: 20},
        "images": ["images/uniqlo-3d-knit-cotton-boat-neck-top_0.jpg"],
    },
    {
        "name": "Levi's 501 Original Fit Jeans",
        "description": "Men's straight-leg jeans with the classic button fly and durable non-stretch denim.",
        "price": 89.50,
        "manufacturer": "Levi's",
        "category": "Clothes",
        "stores": {config.STORE_NAME1: 18, config.STORE_NAME3: 14},
        "images": ["images/levi's-501-original-fit-jeans_0.jpg"],
    },
    {
        "name": "Levi's 724 High-Rise Slim Jeans",
        "description": "Women's high-rise slim jeans with stretch denim for a flattering, everyday fit.",
        "price": 98.00,
        "manufacturer": "Levi's",
        "category": "Clothes",
        "stores": {config.STORE_NAME2: 16, config.STORE_NAME3: 22},
        "images": ["images/levi's-724-high-rise-slim-jeans_0.jpg"],
    },
]


def _resolve_image_path(image_path: str) -> Path:
    path = Path(image_path)
    if not path.is_absolute():
        path = config.DATA_SEEDER_DIR / path
    return path


def _create_s3_client() -> S3Client:
    return boto3.client(
        "s3",
        endpoint_url=config.S3_ENDPOINT_URL,
        aws_access_key_id="test",
        aws_secret_access_key="test",
        region_name=config.S3_REGION,
    )

# returns (name -> id map, isError)
def seed_categories(client: ApiClient) -> tuple[dict[str, int], bool]:
    existing = _load_categories(client)
    failed = False
    for name in config.CATEGORY_NAMES:
        category_id, item_failed = find_or_create_category(client, existing, name)
        if item_failed or category_id is None:
            failed = True
    return existing, failed

# returns (name -> id map, isError)
def seed_manufacturers(client: ApiClient) -> tuple[dict[str, int], bool]:
    existing = _load_manufacturers(client)
    failed = False
    for name in config.MANUFACTURER_NAMES:
        manufacturer_id, item_failed = find_or_create_manufacturer(
            client, existing, name
        )
        if item_failed or manufacturer_id is None:
            failed = True
    return existing, failed

# returns (product name -> id map, isError)
def seed_products(
    client: ApiClient,
    categories: dict[str, int],
    manufacturers: dict[str, int],
) -> tuple[dict[str, int], bool]:
    existing = _load_products(client)
    failed = False

    for product in SEED_PRODUCTS:
        category_name = str(product["category"])
        manufacturer_name = str(product["manufacturer"])
        category_id = categories.get(category_name)
        manufacturer_id = manufacturers.get(manufacturer_name)

        if category_id is None:
            logger.error("Unknown category %s for product %s", category_name, product["name"])
            failed = True
            continue
        if manufacturer_id is None:
            logger.error(
                "Unknown manufacturer %s for product %s",
                manufacturer_name,
                product["name"],
            )
            failed = True
            continue

        image_paths = product.get("images", [])
        product_id, item_failed = find_or_create_product(
            client,
            existing,
            str(product["name"]),
            str(product["description"]),
            float(product["price"]),
            manufacturer_id,
            category_id,
            len(image_paths),
        )
        if item_failed or product_id is None:
            failed = True

    return existing, failed

# returns (store name -> id map, isError)
def seed_store_locations(client: ApiClient) -> tuple[dict[str, int], bool]:
    existing = _load_store_locations(client)
    failed = False
    for store in config.SEED_STORES:
        store_location_id, item_failed = find_or_create_store_location(
            client, existing, store["displayName"], store["address"]
        )
        if item_failed or store_location_id is None:
            failed = True
    return existing, failed


# returns isError bool
def link_seed_products_to_stores(
    client: ApiClient,
    store_locations: dict[str, int],
    products: dict[str, int],
) -> bool:
    failed = False
    links_by_store: dict[int, set[int]] = {}

    for product in SEED_PRODUCTS:
        product_id = products.get(str(product["name"]))
        if product_id is None:
            logger.error("Product %s was not seeded; skipping store links", product["name"])
            failed = True
            continue

        for store_name, stock in product.get("stores", {}).items():
            store_location_id = store_locations.get(store_name)
            if store_location_id is None:
                logger.error("Unknown store %s for product %s", store_name, product["name"])
                failed = True
                continue

            if store_location_id not in links_by_store:
                links_by_store[store_location_id] = _load_product_links(
                    client, store_location_id
                )

            if link_product_to_store(
                client,
                links_by_store[store_location_id],
                store_location_id,
                product_id,
                stock,
            ):
                failed = True

    return failed

def s3_key_exists(s3: S3Client, key: str) -> bool:
    try:
        s3.head_object(Bucket=config.S3_BUCKET, Key=key)
        return True
    except ClientError as e:
        error_code = e.response.get("Error", {}).get("Code", "")
        if error_code in ("404", "NoSuchKey", "NotFound"):
            return False
        raise


def populate_product_images(products: dict[str, int]) -> bool:
    s3 = _create_s3_client()
    failed = False

    for product in SEED_PRODUCTS:
        product_name = str(product["name"])
        product_id = products.get(product_name)
        if product_id is None:
            logger.error("Product %s was not seeded; skipping S3 upload", product_name)
            failed = True
            continue

        for order, image_path in enumerate(product.get("images", [])):
            path = _resolve_image_path(str(image_path))
            if not path.is_file():
                logger.error("Image file not found for %s: %s", product_name, path)
                failed = True
                continue

            key = f"{product_id}_{order}"
            if s3_key_exists(s3, key):
                logger.info("S3 object already exists: %s", key)
                continue

            content_type = mimetypes.guess_type(path.name)[0] or "application/octet-stream"
            with path.open("rb") as image_file:
                s3.put_object(
                    Bucket=config.S3_BUCKET,
                    Key=key,
                    Body=image_file,
                    ContentType=content_type,
                )
            logger.info("Uploaded %s to s3://%s/%s", path.name, config.S3_BUCKET, key)

    return failed


def main() -> int:
    client = ApiClient()

    if not client.get_admin_token():
        return 1

    failed = False

    categories, cat_failed = seed_categories(client)

    manufacturers, mfr_failed = seed_manufacturers(client)
    failed |= cat_failed | mfr_failed

    products, prod_failed = seed_products(client, categories, manufacturers)
    failed |= prod_failed

    if populate_product_images(products):
        failed = True

    store_locations, store_failed = seed_store_locations(client)
    failed |= store_failed

    if link_seed_products_to_stores(client, store_locations, products):
        failed = True

    total_links = sum(len(product.get("stores", {})) for product in SEED_PRODUCTS)
    total_images = sum(len(product.get("images", [])) for product in SEED_PRODUCTS)
    if failed:
        logger.error("Seeding finished with failures")
        return 1

    logger.info(
        "Seeding complete: %d categories, %d manufacturers, %d products, "
        "%d store locations, %d product-store links, %d images",
        len(categories),
        len(manufacturers),
        len(products),
        len(config.SEED_STORES),
        total_links,
        total_images,
    )
    return 0


if __name__ == "__main__":
    sys.exit(main())
