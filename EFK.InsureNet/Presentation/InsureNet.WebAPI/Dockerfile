## See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.
#
## Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
## For more information, please see https://aka.ms/containercompat
#
## This stage is used when running from VS in fast mode (Default for Debug configuration)
#FROM mcr.microsoft.com/dotnet/aspnet:9.0-nanoserver-1809 AS base
#WORKDIR /app
#EXPOSE 8080
#EXPOSE 8081
#
#
## This stage is used to build the service project
#FROM mcr.microsoft.com/dotnet/sdk:9.0-nanoserver-1809 AS build
#ARG BUILD_CONFIGURATION=Release
#WORKDIR /src
#COPY ["EFK.InsureNet/Presentation/InsureNet.WebAPI/InsureNet.WebAPI.csproj", "EFK.InsureNet/Presentation/InsureNet.WebAPI/"]
#RUN dotnet restore "./EFK.InsureNet/Presentation/InsureNet.WebAPI/InsureNet.WebAPI.csproj"
#COPY . .
#WORKDIR "/src/EFK.InsureNet/Presentation/InsureNet.WebAPI"
#RUN dotnet build "./InsureNet.WebAPI.csproj" -c %BUILD_CONFIGURATION% -o /app/build
#
## This stage is used to publish the service project to be copied to the final stage
#FROM build AS publish
#ARG BUILD_CONFIGURATION=Release
#RUN dotnet publish "./InsureNet.WebAPI.csproj" -c %BUILD_CONFIGURATION% -o /app/publish /p:UseAppHost=false
#
## This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "InsureNet.WebAPI.dll"]
# 1. Base image olarak Linux tabanl� .NET Runtime kullan (Windows yerine Linux)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# 2. SDK i�eren image kullanarak proje build edilecek
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# 3. Proje dosyalar�n� kopyala ve restore et
COPY EFK.InsureNet/Presentation/InsureNet.WebAPI/InsureNet.WebAPI.csproj EFK.InsureNet/Presentation/InsureNet.WebAPI/
RUN dotnet restore "EFK.InsureNet/Presentation/InsureNet.WebAPI/InsureNet.WebAPI.csproj"

# 4. Geri kalan dosyalar� kopyala ve build et
COPY . .
WORKDIR "/src/EFK.InsureNet/Presentation/InsureNet.WebAPI"
RUN dotnet build "./InsureNet.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# 5. Publish a�amas�
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./InsureNet.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# 6. Final stage - runtime ortam�
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InsureNet.WebAPI.dll"]
