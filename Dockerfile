# ---------- Build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-env
WORKDIR /app

# Copy only the project files – enables layer caching for restores
COPY OpenChatRoom/*.csproj ./OpenChatRoom/
RUN dotnet restore ./OpenChatRoom/OpenChatRoom.csproj

# Copy the rest of the source code
COPY . ./

# Publish the Blazor WebAssembly app
RUN dotnet publish ./OpenChatRoom/OpenChatRoom.csproj \
    -c Release \
    -o /app/publish   # final output folder

# ---------- Runtime stage ----------
FROM nginx:alpine
WORKDIR /var/www/web

# Serve the static files produced by the publish step
COPY --from=build-env /app/publish/wwwroot ./

# Optional custom Nginx configuration -> In compose instead so user can customize it
# COPY nginx.conf /etc/nginx/nginx.conf

# Copy SSL Certs for HTTPS -> Do that in compose instead
# COPY certs/localhost.crt /etc/nginx/certs/localhost.crt
# COPY certs/localhost.key /etc/nginx/certs/localhost.key

EXPOSE 80 443
