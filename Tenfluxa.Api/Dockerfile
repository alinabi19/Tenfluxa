# Base runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080


# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Tenfluxa.Api/Tenfluxa.Api.csproj", "Tenfluxa.Api/"]
COPY ["Tenfluxa.Application/Tenfluxa.Application.csproj", "Tenfluxa.Application/"]
COPY ["Tenfluxa.Domain/Tenfluxa.Domain.csproj", "Tenfluxa.Domain/"]
COPY ["Tenfluxa.Infrastructure/Tenfluxa.Infrastructure.csproj", "Tenfluxa.Infrastructure/"]

RUN dotnet restore "Tenfluxa.Api/Tenfluxa.Api.csproj"

COPY . .
WORKDIR "/src/Tenfluxa.Api"
RUN dotnet publish -c Release -o /app/publish

# Final image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Tenfluxa.Api.dll"]