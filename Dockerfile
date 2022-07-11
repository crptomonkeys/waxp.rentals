FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-env

WORKDIR /app
COPY ./ ./

WORKDIR /app/WaxRentals
RUN dotnet restore

WORKDIR /app/WaxRentals/WaxRentalsWeb
RUN dotnet publish -c Release -o out

WORKDIR /app/WaxRentals/WaxRentals.Processing
RUN dotnet publish -c Release -o out

WORKDIR /app/WaxRentals/WaxRentals.Service
RUN dotnet publish -c Release -o out

WORKDIR /app/WaxRentals/WaxRentals.Api
RUN dotnet publish -c Release -o out


FROM mcr.microsoft.com/dotnet/aspnet:6.0

WORKDIR /app/api
COPY --from=build-env /app/WaxRentals/WaxRentals.Api/out .
WORKDIR /app/service
COPY --from=build-env /app/WaxRentals/WaxRentals.Service/out .
WORKDIR /app/processors
COPY --from=build-env /app/WaxRentals/WaxRentals.Processing/out .
WORKDIR /app/web
COPY --from=build-env /app/WaxRentals/WaxRentalsWeb/out .

RUN apt-get update && apt-get install -y curl

ENTRYPOINT [ "dotnet" ]

#ENV ASPNETCORE_URLS=http://+:2022
#ENV SERVICE=http://service:2022
#ENV API=https://api.waxp.rentals
#CMD [ "WaxRentalsWeb.dll" ]

#ENV SERVICE=http://service:2022
#CMD [ "WaxRentals.Processing.dll" ]

#ENV ASPNETCORE_URLS=http://+:2022
#ENV DB_FILE=/run/secrets/wax.db
#CMD [ "WaxRentals.Service.dll" ]

#ENV ASPNETCORE_URLS=http://+:2022
#ENV SERVICE=http://service:2022
#CMD [ "WaxRentals.Service.dll" ]
