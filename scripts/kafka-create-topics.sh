#!/bin/bash

until kafka-topics.sh --bootstrap-server kafka:9092 --list > /dev/null 2>&1; do
    echo "Waiting for kafka to start..."
    sleep 5
done

echo "Creating kafka topics"

topics=(
    charge_succeeded
    product_created
    product_updated
    product_deleted
    product_added_to_cart
    product_cart_quantity_modified
    product_removed_from_cart
    user_cart_cleared
    product_added_to_store
    product_stock_updated
    product_removed_from_store
    store_created
    store_updated
    store_deleted
    manufacturer_created
    manufacturer_updated
    manufacturer_deleted
    category_created
    category_updated
    category_deleted
    product_image_created
    product_image_updated
    product_image_deleted
    payment_failed
    user_account_deleted
)

for topic in "${topics[@]}"; do
    kafka-topics.sh \
        --bootstrap-server kafka:9092 \
        --topic "$topic" \
        --partitions 6 \
        --replication-factor 1 \
        --create > /dev/null 2>&1 || true
    echo "Created topic: $topic"
done
