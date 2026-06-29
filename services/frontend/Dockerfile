FROM node:22 AS build
WORKDIR /app
COPY package.json package.json
COPY package-lock.json package-lock.json
RUN npm ci
COPY . .
RUN npx ng build

FROM build AS prod
WORKDIR /app
CMD [ "node", "./dist/ecommerce/server/server.mjs" ]
