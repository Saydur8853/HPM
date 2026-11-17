FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY HPM.Web/HPM.Web.csproj HPM.Web/
RUN dotnet restore HPM.Web/HPM.Web.csproj

COPY . .
RUN dotnet publish HPM.Web/HPM.Web.csproj --configuration Release --output /out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build --chown=app:app /out ./
RUN mkdir /app/keys && chown app:app /app/keys

USER app
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "HPM.Web.dll"]
