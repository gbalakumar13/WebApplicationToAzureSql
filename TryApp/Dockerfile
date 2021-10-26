FROM mcr.microsoft.com/dotnet/sdk:5.0 as firststage
WORKDIR /src
COPY *.csproj .
RUN dotnet restore
COPY . .
RUN dotnet build
RUN dotnet publish -o /out

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=firststage /out .
CMD dotnet TryApp.dll
