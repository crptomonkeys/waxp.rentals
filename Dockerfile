FROM mcr.microsoft.com/dotnet/sdk:5.0 as build-env

WORKDIR /app
COPY ./ ./

WORKDIR /app/WaxRentals
RUN dotnet restore

WORKDIR /app/WaxRentals/WaxRentalsWeb
RUN dotnet publish -c Release -o out

WORKDIR /app/WaxRentals/WaxRentals.Processing
RUN dotnet publish -c Release -o out


FROM mcr.microsoft.com/dotnet/aspnet:5.0

WORKDIR /app/processors
COPY --from=build-env /app/WaxRentals/WaxRentals.Processing/out .
WORKDIR /app/web
COPY --from=build-env /app/WaxRentals/WaxRentalsWeb/out .

RUN apt-get update && apt-get install curl -y

ENTRYPOINT [ "dotnet" ]

#ENV ASPNETCORE_URLS=http://+:2022
#HEALTHCHECK CMD curl --fail http://localhost:2022/ || exit 1
#CMD [ "WaxRentalsWeb.dll" ]

#CMD [ "WaxRentals.Processing.dll" ]
