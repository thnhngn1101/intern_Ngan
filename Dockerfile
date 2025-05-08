FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY BPMaster/*.csproj ./BPMaster/
RUN dotnet restore

# copy everything else and build app
COPY BPMaster/. ./BPMaster/
WORKDIR /source/BPMaster
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "BPMaster.dll"]


