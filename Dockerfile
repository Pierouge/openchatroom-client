FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY OpenChatRoom.sln ./
COPY ./OpenChatRoom/OpenChatRoom.csproj ./OpenChatRoom/

RUN dotnet restore
COPY . ./
RUN dotnet publish -c Release -o out

FROM nginx:stable-alpine
WORKDIR /app
EXPOSE 8080
ARG Environment=Production
COPY nginx.conf /etc/nginx/nginx.conf
RUN sed -i "s/replace_this_string/${Environment}/" /etc/nginx/nginx.conf
COPY --from=build /app/out/wwwroot /usr/share/nginx/html