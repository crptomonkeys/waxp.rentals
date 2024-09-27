FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-env

WORKDIR /app
COPY ./ ./

WORKDIR /app/WaxRentals
RUN dotnet restore

WORKDIR /app/WaxRentals/WaxRentalsWeb
RUN dotnet publish -c Web -o out

WORKDIR /app/WaxRentals/WaxRentals.Processing
RUN dotnet publish -c Processors -o out

WORKDIR /app/WaxRentals/WaxRentals.Service
RUN dotnet publish -c Service -o out

WORKDIR /app/WaxRentals/WaxRentals.Api
RUN dotnet publish -c API -o out

WORKDIR /app/WaxRentals/WaxRentals.Manage
RUN dotnet publish -c Manage -o out


FROM mcr.microsoft.com/dotnet/aspnet:6.0

WORKDIR /app/api
COPY --from=build-env /app/WaxRentals/WaxRentals.Api/out .
WORKDIR /app/service
COPY --from=build-env /app/WaxRentals/WaxRentals.Service/out .
WORKDIR /app/processors
COPY --from=build-env /app/WaxRentals/WaxRentals.Processing/out .
WORKDIR /app/manage
COPY --from=build-env /app/WaxRentals/WaxRentals.Manage/out .
WORKDIR /app/web
COPY --from=build-env /app/WaxRentals/WaxRentalsWeb/out .

RUN apt-get update && apt-get install -y curl

ENTRYPOINT [ "dotnet" ]

#ENV ASPNETCORE_URLS=http://+:2022
#ENV SERVICE=http://service:2022
#ENV API=https://api.waxp.rentals
#CMD [ "WaxRentalsWeb.dll" ]

#ENV ASPNETCORE_URLS=http://+:2022
#ENV SERVICE=http://service:2022
#CMD [ "/app/processors/WaxRentals.Processing.dll" ]

#ENV ASPNETCORE_URLS=http://+:2022
#ENV DB_FILE=/run/secrets/wax.db
#CMD [ "/app/service/WaxRentals.Service.dll" ]

#ENV ASPNETCORE_URLS=http://+:2022
#ENV SERVICE=http://service:2022
#CMD [ "/app/api/WaxRentals.Api.dll" ]

#ENV ASPNETCORE_URLS=http://+:2022
#ENV SERVICE=http://service:2022
#CMD [ "/app/manage/WaxRentals.Manage.dll" ]
