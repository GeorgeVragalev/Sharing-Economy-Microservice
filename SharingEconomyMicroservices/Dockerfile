FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app

EXPOSE 80
EXPOSE 443

# Copy csproj and restore as distinct layers
COPY ./InventoryAPI/InventoryAPI.csproj ./InventoryAPI/
COPY ./InventoryBLL/InventoryBLL.csproj ./InventoryBLL/
COPY ./InventoryDAL/InventoryDAL.csproj ./InventoryDAL/

RUN dotnet restore ./InventoryAPI/InventoryAPI.csproj

# Copy everything else and build
COPY ./InventoryAPI/ ./InventoryAPI/
COPY ./InventoryBLL/ ./InventoryBLL/
COPY ./InventoryDAL/ ./InventoryDAL/

# Build and publish
WORKDIR /app/InventoryAPI
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build-env /app/InventoryAPI/out .
ENTRYPOINT ["dotnet", "InventoryAPI.dll"]