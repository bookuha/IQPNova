FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/IQP.Web/IQP.Web.csproj", "src/IQP.Web/"]
COPY ["src/IQP.Infrastructure/IQP.Infrastructure.csproj", "src/IQP.Infrastructure/"]
COPY ["src/IQP.Application/IQP.Application.csproj", "src/IQP.Application/"]
COPY ["src/IQP.Domain/IQP.Domain.csproj", "src/IQP.Domain/"]
COPY ["src/IQP.Infrastructure.CodeRunner/IQP.Infrastructure.CodeRunner.csproj", "src/IQP.Infrastructure.CodeRunner/"]

RUN dotnet restore "src/IQP.Web/IQP.Web.csproj"
COPY . .
WORKDIR "/src/src/IQP.Web"
RUN dotnet build "IQP.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "IQP.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IQP.Web.dll"]
