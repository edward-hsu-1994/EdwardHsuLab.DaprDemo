#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["EdwardHsuLab.DaprDemo/EdwardHsuLab.DaprDemo.csproj", "EdwardHsuLab.DaprDemo/"]
RUN dotnet restore "EdwardHsuLab.DaprDemo/EdwardHsuLab.DaprDemo.csproj"
COPY . .
WORKDIR "/src/EdwardHsuLab.DaprDemo"
RUN dotnet build "EdwardHsuLab.DaprDemo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EdwardHsuLab.DaprDemo.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EdwardHsuLab.DaprDemo.dll"]