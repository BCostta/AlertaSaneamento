# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia o csproj e restaura as dependências
COPY AlertaSaneamento/AlertaSaneamento.csproj AlertaSaneamento/
RUN dotnet restore AlertaSaneamento/AlertaSaneamento.csproj

# Copia o restante do código e publica
COPY AlertaSaneamento/ AlertaSaneamento/
WORKDIR /src/AlertaSaneamento
RUN dotnet publish AlertaSaneamento.csproj -c Release -o /app/publish

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# O Render injeta a variável PORT automaticamente
EXPOSE 10000

ENTRYPOINT ["dotnet", "AlertaSaneamento.dll"]
