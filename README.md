# waxp.rentals

To stand up your own version of this site, you'll need a few things:

- [Docker-Compose File](README.md#docker-compose-file)
- [Secrets Files](README.md#secrets-files)
- [.env for Database](README.md#env-for-database)
- [Docker Networks](README.md#docker-networks)
- [tracking.csv (optional)](README.md#trackingcsv-optional)
- [Constants files](README.md#constants-files)

# Docker-Compose File

There are three containers involved in https://waxp.rentals: the website (web), the event processors (processors), and the database (data).  
The web and processors containers are based on the same image but with different CMDs.  
Some lines may differ slightly, but this is the basic concept:

```
version: '3.8'

services:

  web:
    build: 'https://github.com/cinderblockgames/waxp.rentals.git#main'
    image: 'your-private-registry/wax-rentals/web:#.#.#'
    command: [ "/app/web/WaxRentalsWeb.dll" ]
    secrets:
      - banano.seed
      - welcome.banano.seed
      - wax.key
      - wax.db
    environment:
      - ASPNETCORE_URLS=http://+:2022
    networks:
      - traefik
      - banano
      - wax-rentals
      - wax-rentals-internal
    deploy:
      mode: replicated
      replicas: 1
      labels:
        - 'traefik.enable=true'
        - 'traefik.docker.network=traefik'
        - 'traefik.http.routers.wax-rentals.rule=Host(`waxp.rentals`)'
        - 'traefik.http.routers.wax-rentals.entrypoints=web-secure'
        - 'traefik.http.routers.wax-rentals.tls'
        - 'traefik.http.services.wax-rentals.loadbalancer.server.port=2022'

  processors:
    image: 'your-private-registry/wax-rentals/web:#.#.#'
    command: [ "/app/processors/WaxRentals.Processing.dll" ]
    secrets:
      - banano.seed
      - welcome.banano.seed
      - wax.key
      - wax.db
    volumes:
      - '/run/rentwax/output:/run/output'
    networks:
      - banano
      - wax-rentals
      - wax-rentals-internal
    deploy:
      mode: replicated
      replicas: 1

  data:
    image: 'cinderblockgames/dacpac-aware-mssql:1.0.0'
    env_file: .env
    volumes:
      - '/run/rentwax/db:/var/opt/mssql'
      - '/run/rentwax/dacpac:/tmp/db'
    networks: [wax-rentals-internal]
    deploy:
      mode: replicated
      replicas: 1
      placement:
        constraints: [node.platform.arch == x86_64]

networks:
  traefik:
    external: true
  banano:
    external: true
  wax-rentals:
    external: true
  wax-rentals-internal:
    external: true

secrets:
  banano.seed:
    external: true
  welcome.banano.seed:
    external: true
  wax.key:
    external: true
  wax.db:
    external: true
```

# Secrets Files

You will need a handful of Docker Secrets to provide the private keys and connection strings necessary for use in the site.  These could obviously be combined, if you'd like.

## banano.seed

```
{
  "seed": "your seed here"
}
```

## welcome.banano.seed

```
{
  "seed": "your seed here"
}
```

## wax.key

```
{
  "private": "your ACTIVE private key here"
}
```

## wax.db

```
{
  "ConnectionString": "your connection string to the WaxRentals db here"
}
```

# .env for Database

The SA_PASSWORD should only be necessary on initial startup -- after that, the database has been set up with the SA password.  DATABASE_NAME continues to be necessary for dacpac processing.

```
DATABASE_NAME=WaxRentals
SA_PASSWORD=your SA password here
```

# Docker Networks

There are four docker networks in use, all overlay networks:

## traefik

This is the ingress network for traefik to reverse proxy the site.

## banano

This allows access to the banano node used to process transactions.

## wax-rentals

This allows access outside the docker network.

## wax-rentals-internal

This allows access for communications within the project -- specifically, connections to the database.

# tracking.csv (optional)

This file tracks transactions for tax purposes.  The headers follow:

```
Date,Event,Coins,Earned,Spent
```

# Constants files

These code files, contained in the `Config` folder in the projects within this solution, should be inspected for changes necessary to support your environment.  As just one example, the `WorkServer` in `WaxRentals.Banano.Config.Constants` should be updated to point to your work server.
