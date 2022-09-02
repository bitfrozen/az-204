FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /build
COPY . ./
RUN dotnet publish \
  --configuration Release \
  --output /out

FROM mcr.microsoft.com/dotnet/runtime:6.0-alpine AS release
WORKDIR /app
COPY --from=build /out ./

ENTRYPOINT [ "dotnet", "/app/ipcheck.dll" ]