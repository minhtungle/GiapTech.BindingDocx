FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/GiapTech.BindingDocx.Api/GiapTech.BindingDocx.Api.csproj", "src/GiapTech.BindingDocx.Api/"]
COPY ["src/GiapTech.BindingDocx.Application/GiapTech.BindingDocx.Application.csproj", "src/GiapTech.BindingDocx.Application/"]
COPY ["src/GiapTech.BindingDocx.Domain/GiapTech.BindingDocx.Domain.csproj", "src/GiapTech.BindingDocx.Domain/"]
COPY ["src/GiapTech.BindingDocx.Infrastructure/GiapTech.BindingDocx.Infrastructure.csproj", "src/GiapTech.BindingDocx.Infrastructure/"]

RUN dotnet restore "src/GiapTech.BindingDocx.Api/GiapTech.BindingDocx.Api.csproj"

COPY . .

RUN dotnet build "src/GiapTech.BindingDocx.Api/GiapTech.BindingDocx.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/GiapTech.BindingDocx.Api/GiapTech.BindingDocx.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GiapTech.BindingDocx.Api.dll"]
