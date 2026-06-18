#!/bin/bash

until kafka-topics.sh --bootstrap-server kafka:9092 --list > /dev/null 2>&1; do
    echo "Waiting for kafka to start..."
    sleep 5
done

echo "Creating kafka topics"

# Create kafka topics
kafka-topics.sh \
    --bootstrap-server kafka:9092 \
    --topic charge-succeeded \
    --partitions 6 \
    --replication-factor 1 \
    --create > /dev/null 2>&1 || true

kafka-topics.sh \
    --bootstrap-server kafka:9092 \
    --topic product-added-to-store \
    --partitions 6 \
    --replication-factor 1 \
    --create > /dev/null 2>&1 || true

kafka-topics.sh \
    --bootstrap-server kafka:9092 \
    --topic product-stock-updated \
    --partitions 6 \
    --replication-factor 1 \
    --create > /dev/null 2>&1 || true

kafka-topics.sh \
    --bootstrap-server kafka:9092 \
    --topic product-removed-from-store \
    --partitions 6 \
    --replication-factor 1 \
    --create > /dev/null 2>&1 || true

kafka-topics.sh \
    --bootstrap-server kafka:9092 \
    --topic product-created \
    --partitions 6 \
    --replication-factor 1 \
    --create > /dev/null 2>&1 || true

kafka-topics.sh \
    --bootstrap-server kafka:9092 \
    --topic product-updated \
    --partitions 6 \
    --replication-factor 1 \
    --create > /dev/null 2>&1 || true

kafka-topics.sh \
    --bootstrap-server kafka:9092 \
    --topic product-deleted \
    --partitions 6 \
    --replication-factor 1 \
    --create > /dev/null 2>&1 || true

kafka-topics.sh \
    --bootstrap-server kafka:9092 \
    --topic store-created \
    --partitions 6 \
    --replication-factor 1 \
    --create > /dev/null 2>&1 || true

kafka-topics.sh \
    --bootstrap-server kafka:9092 \
    --topic store-updated \
    --partitions 6 \
    --replication-factor 1 \
    --create > /dev/null 2>&1 || true

kafka-topics.sh \
    --bootstrap-server kafka:9092 \
    --topic store-deleted \
    --partitions 6 \
    --replication-factor 1 \
    --create > /dev/null 2>&1 || true

kafka-topics.sh \
    --bootstrap-server kafka:9092 \
    --topic manufacturer-created \
    --partitions 6 \
    --replication-factor 1 \
    --create > /dev/null 2>&1 || true

kafka-topics.sh \
    --bootstrap-server kafka:9092 \
    --topic manufacturer-updated \
    --partitions 6 \
    --replication-factor 1 \
    --create > /dev/null 2>&1 || true

kafka-topics.sh \
    --bootstrap-server kafka:9092 \
    --topic manufacturer-deleted \
    --partitions 6 \
    --replication-factor 1 \
    --create > /dev/null 2>&1 || true

kafka-topics.sh \
    --bootstrap-server kafka:9092 \
    --topic category-created \
    --partitions 6 \
    --replication-factor 1 \
    --create > /dev/null 2>&1 || true

kafka-topics.sh \
    --bootstrap-server kafka:9092 \
    --topic category-updated \
    --partitions 6 \
    --replication-factor 1 \
    --create > /dev/null 2>&1 || true

kafka-topics.sh \
    --bootstrap-server kafka:9092 \
    --topic category-deleted \
    --partitions 6 \
    --replication-factor 1 \
    --create > /dev/null 2>&1 || true

kafka-topics.sh \
    --bootstrap-server kafka:9092 \
    --topic product-image-created \
    --partitions 6 \
    --replication-factor 1 \
    --create > /dev/null 2>&1 || true

kafka-topics.sh \
    --bootstrap-server kafka:9092 \
    --topic product-image-updated \
    --partitions 6 \
    --replication-factor 1 \
    --create > /dev/null 2>&1 || true

kafka-topics.sh \
    --bootstrap-server kafka:9092 \
    --topic product-image-deleted \
    --partitions 6 \
    --replication-factor 1 \
    --create > /dev/null 2>&1 || true
