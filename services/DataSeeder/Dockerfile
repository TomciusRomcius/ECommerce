FROM python:3.12-slim

WORKDIR /app

COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

COPY config.py client.py seed.py ./
COPY images ./images/

CMD ["python", "seed.py"]
