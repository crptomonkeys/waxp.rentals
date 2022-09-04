# waxp.rentals

To stand up your own version of this site, you'll need a few things:

- [Docker-Compose File](README.md#docker-compose-file)
- [Secrets Files](README.md#secrets-files)
- [.env for Database](README.md#env-for-database)
- [Docker Networks](README.md#docker-networks)
- [tracking.csv (optional)](README.md#trackingcsv-optional)
- [site-message](README.md#site-message)
- [Constants files](README.md#constants-files)

# Docker-Compose File

There are seven containers involved in https://waxp.rentals: the public api (api), the web front end (web), the event processors (processors), the central service (service), the private management api (manage), the database (data), and the ad system (banad).  
The api, web, processors, service, and manage containers are based on the same image but with different CMDs.  
Some lines may differ slightly, but this is the basic concept:

```
version: '3.8'

services:

  api:
    build: 'https://github.com/cinderblockgames/waxp.rentals.git#main'
    image: 'your-private-registry/wax-rentals:#.#.#'
    command: [ "/app/api/WaxRentals.Api.dll" ]
    healthcheck:
      test: curl --fail http://localhost:2022/index.html || exit 1
      interval: 60s
      timeout: 5s
      retries: 2
      start_period: 5s
    environment:
      - ASPNETCORE_URLS=http://+:2022
      - SERVICE=http://service:2022
    networks:
      - traefik
      - wax-rentals-internal
    deploy:
      mode: replicated
      replicas: 2
      labels:
        - 'traefik.enable=true'
        - 'traefik.docker.network=traefik'
        - 'traefik.http.routers.wax-rentals-api.rule=Host(`api.waxp.rentals`)'
        - 'traefik.http.routers.wax-rentals-api.entrypoints=web-secure'
        - 'traefik.http.routers.wax-rentals-api.tls'
        - 'traefik.http.services.wax-rentals-api.loadbalancer.server.port=2022'

  web:
    image: 'your-private-registry/wax-rentals:#.#.#'
    command: [ "/app/web/WaxRentalsWeb.dll" ]
    environment:
      - ASPNETCORE_URLS=http://+:2022
      - SERVICE=http://service:2022
      - API=https://api.waxp.rentals
    volumes:
      - '/run/rentwax/files:/run/files'
      - '/run/rentwax/DataProtection-Keys:/root/.aspnet/DataProtection-Keys'
    networks:
      - traefik
      - wax-rentals
      - wax-rentals-internal
    deploy:
      mode: replicated
      # do not increase past 1
      replicas: 1
      labels:
        - 'traefik.enable=true'
        - 'traefik.docker.network=traefik'
        - 'traefik.http.routers.wax-rentals.rule=Host(`waxp.rentals`)'
        - 'traefik.http.routers.wax-rentals.entrypoints=web-secure'
        - 'traefik.http.routers.wax-rentals.tls'
        - 'traefik.http.services.wax-rentals.loadbalancer.server.port=2022'

  processors:
    image: 'your-private-registry/wax-rentals:#.#.#'
    command: [ "/app/processors/WaxRentals.Processing.dll" ]
    environment:
      - SERVICE=http://service:2022
    networks:
      - wax-rentals-internal
    deploy:
      mode: replicated
      # do not increase past 1
      replicas: 1

  service:
    image: 'your-private-registry/wax-rentals:#.#.#'
    command: [ "/app/service/WaxRentals.Service.dll" ]
    secrets:
      - ads.banano.seed
      - banano.seed
      - welcome.banano.seed
      - wax.key
      - wax.db
      - telegram.waxp.rentals
    environment:
      - ASPNETCORE_URLS=http://+:2022
      - DB_FILE=/run/secrets/wax.db
      - WAX_PRIMARY=rentwaxp4ban
      - WAX_TRANSACT=rentwax4ban1+rentwax4ban2+rentwax4ban3+rentwax4ban4
      - WAX_KEY_FILE=/run/secrets/wax.key
      - BANANO_SEED_FILE=/run/secrets/banano.seed
      - BANANO_SEED_FILE_WELCOME=/run/secrets/welcome.banano.seed
      - BANANO_SEED_FILE_ADS=/run/secrets/ads.banano.seed
    volumes:
      - '/run/rentwax/output:/run/output'
    networks:
      - banano
      - wax-rentals
      - wax-rentals-internal
    deploy:
      mode: replicated
      # do not increase past 1
      replicas: 1

  manage:
    image: 'your-private-registry/wax-rentals:#.#.#'
    command: [ "/app/manage/WaxRentals.Manage.dll" ]
    healthcheck:
      test: curl --fail http://localhost:2022/index.html || exit 1
      interval: 60s
      timeout: 5s
      retries: 2
      start_period: 5s
    environment:
      - ASPNETCORE_URLS=http://+:2022
      - SERVICE=http://service:2022
    networks:
      - traefik
      - wax-rentals-internal
    deploy:
      mode: replicated
      replicas: 1
      labels:
        - 'traefik.enable=true'
        - 'traefik.docker.network=traefik'
        - 'traefik.http.routers.wax-rentals-manage.rule=Host(`manage.waxp.rentals`)'
        - 'traefik.http.routers.wax-rentals-manage.entrypoints=web-secure'
        - 'traefik.http.routers.wax-rentals-manage.tls'
        - 'traefik.http.services.wax-rentals-manage.loadbalancer.server.port=2022'

  data:
    image: 'cinderblockgames/dacpac-aware-mssql:1.0.0'
    env_file: .env
    volumes:
      - '/run/rentwax/db:/var/opt/mssql'
      - '/run/rentwax/dacpac:/tmp/db'
    networks: [wax-rentals-internal]
    deploy:
      mode: replicated
      # do not increase past 1
      replicas: 1
      placement:
        constraints: [node.platform.arch == x86_64]

  banad:
    image: 'thearistotle/ban-ad:1.0.2'
    secrets:
      - wax-email.pwd
    environment:
      - SITE_ID=waxp.rentals
      - EMAIL_ADDRESS=[redacted]
      - EMAIL_PASSWORD_FILE=/run/secrets/wax-email.pwd
      - EMAIL_DISPLAY_NAME=waxp.rentals
      - BANANO_NODE=https://api.banano.kga.earth/node/proxy
      - AD_APPROVER_EMAIL=[redacted]
      - BANANO_PAYMENT_ADDRESS=[redacted]
    volumes:
      - '/run/rentwax/banad/ad-slots:/run/ad-slots'
      - '/run/rentwax/banad/ads:/run/ads'
      - '/run/rentwax/banad/banano-tracking:/run/banano-tracking'
      - '/run/rentwax/banad/default.png:/run/default.png'
    networks:
      - traefik
      # The ad system needs internet access for emails and payment tracking.
      - wax-rentals
    deploy:
      mode: replicated
      replicas: 1
      labels:
        - 'traefik.enable=true'
        - 'traefik.docker.network=traefik'
        - 'traefik.http.routers.wax-rentals-banad.rule=Host(`waxp.rentals`) && PathPrefix(`/ads`)'
        - 'traefik.http.middlewares.wax-rentals-banad.stripprefix.prefixes=/ads'
        - 'traefik.http.routers.wax-rentals-banad.middlewares=wax-rentals-banad'
        - 'traefik.http.routers.wax-rentals-banad.entrypoints=web-secure'
        - 'traefik.http.routers.wax-rentals-banad.tls'
        - 'traefik.http.services.wax-rentals-banad.loadbalancer.server.port=2022'

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
  ads.banano.seed:
    external: true
  banano.seed:
    external: true
  welcome.banano.seed:
    external: true
  wax-email.pwd:
    external: true
  wax.key:
    external: true
  wax.db:
    external: true
  telegram.waxp.rentals:
    external: true
```

# Secrets Files

You will need a handful of Docker Secrets to provide the private keys and connection strings necessary for use in the site.  These could obviously be combined, if you'd like.

## ads.banano.seed

```
{
  "seed": "your seed here"
}
```

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

## wax-email.pwd

```
your email password here
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

## telegram.waxp.rentals

```
{
  "token": "bot token here",
  "targetChat": target chat id here
}
```

# .env for Database

The `SA_PASSWORD` should only be necessary on initial startup -- after that, the database has been set up with the SA password.  `DATABASE_NAME` continues to be necessary for dacpac processing.

```
DATABASE_NAME=WaxRentals
SA_PASSWORD=your SA password here
```

# Docker Networks

There are four docker networks in use, all overlay networks:

## traefik

This is the ingress network for traefik to reverse proxy the api and web front end.

## banano

This allows access to the banano node used to process transactions.

## wax-rentals

This allows access outside the docker network.

## wax-rentals-internal

This allows access for communications within the project -- specifically, connections to the service and to the database.

# tracking.csv (optional)

This file lives in `/run/output` and tracks transactions for tax purposes.  The headers follow:

```
Date,Event,Coins,Earned,Spent
```

# site-message

This file lives in `/run/files` in the web front end container and provides the ability to set a site message to be displayed at the top of the site for announcements.

# Constants files

These code files, contained in the `Config` folder in the projects within this solution, should be inspected for changes necessary to support your environment.  As just one example, the `WorkServer` in `WaxRentals.Banano.Config.Constants` should be updated to point to your work server.
