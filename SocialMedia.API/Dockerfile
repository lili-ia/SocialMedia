FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SocialMedia.Presentation/SocialMedia.Presentation.csproj", "SocialMedia.Presentation/"]
COPY ["SocialMedia.Application/SocialMedia.Application.csproj", "SocialMedia.Application/"]
COPY ["SocialMedia.Domain/SocialMedia.Domain.csproj", "SocialMedia.Domain/"]
COPY ["SocialMedia.Infrastructure/SocialMedia.Infrastructure.csproj", "SocialMedia.Infrastructure/"]
RUN dotnet restore "SocialMedia.Presentation/SocialMedia.Presentation.csproj"
COPY . .
WORKDIR "/src/SocialMedia.Presentation"
RUN dotnet build "SocialMedia.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SocialMedia.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SocialMedia.Presentation.dll"]
