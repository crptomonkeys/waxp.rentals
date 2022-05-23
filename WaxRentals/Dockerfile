FROM mcr.microsoft.com/dotnet/sdk:5.0 as build-env
WORKDIR /app
COPY ./ ./
RUN dotnet restore
RUN dotnet publish -c Release -o out


FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build-env /app/out .

RUN apt-get update && apt-get install curl -y

ENTRYPOINT [ "dotnet" ]

#ENV ASPNETCORE_URLS=http://+:2022
#HEALTHCHECK CMD curl --fail http://localhost:2022/ || exit 1
#CMD [ "WaxRentalsWeb.dll" ]

#CMD [ "WaxRentals.Processing.dll" ]