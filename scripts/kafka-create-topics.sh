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

