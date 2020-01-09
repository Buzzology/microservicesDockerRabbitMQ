# The base image we're going to build
FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build

# Declare working directory for above build
WORKDIR /app

# Copy csproj and restore as distinct layers (restore pulls down dependencies)
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS runtime

WORKDIR /app

COPY --from=build /app/out .

EXPOSE 30005

ENV ASPNETCORE_URLS http://*:30005

# When the container launches it will launch the file declared here
ENTRYPOINT ["dotnet", "DataLoader.dll", "--server.urls", "http://*:30005"]