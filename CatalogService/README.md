# Catalog Service

Its a service responsible for all the catalog related operations. It uses node.js, express.js, mongo db and Redis cache. Uses Auth0 for authentication/autorization. This api contains route for catalog and image operations. It also implements Tracing and logging using open telemetry and Grafana.
This catalog service will send message to kafka whenever there is change in product price and which will be consumed by Kafka Consumer(Check KafkaConsumer Repo) written in Dot.net 6 and then makes Http call to Basket Service(Check Basker Service Api) to update the price.
This is unfinished project which would need lot of work!!

## How to set up

1. Install "Node" runtime into your machine.
2. Please get Azure cloud access and even without that it should be fine but, one of the end point used for storing images would not work.
3. Download the code
4. Do npm install.
5. Update environment file with azure blob storage connection string(Again applicable in case of storing image.).
6. Run docker compose up which will download docker image and run the container.
7. Then you can do npm run dev which should start server or even docker will start the server if you just want to test and don't want to debug.
8. It uses Mongo Db with replica mode using docker to support Transaction.
9. It uses Kafka for inter service communication.
10. It uses redis as cache.

## Pending items

1. Auth0 integration for authentication and authorization.
2. Few for business use case as well.

## environment key's

| Variable                        | Description                    |
| ------------------------------- | ------------------------------ |
| AZURE_STORAGE_ACCOUNT_NAME      | Blob storage account           |
| AZURE_STORAGE_CONNECTION_STRING | blob storage connection string |
