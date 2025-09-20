FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY ./ ./

RUN dotnet format --verify-no-changes
RUN dotnet restore
RUN dotnet test --no-restore --verbosity normal

RUN dotnet publish src/KanbanBoard.Api/KanbanBoard.Api.csproj -c Release -o /app/out /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/out .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER=true

EXPOSE 8080
ENTRYPOINT ["dotnet", "KanbanBoard.Api.dll"]
