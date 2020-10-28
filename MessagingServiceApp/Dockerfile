FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
ENV PATH $PATH:/root/.dotnet/tools
RUN dotnet tool install -g dotnet-ef --version 3.1.1
COPY . /app
WORKDIR /app
RUN ["dotnet", "restore"]
RUN ["dotnet", "build"]
EXPOSE 80/tcp
RUN chmod +x ./entrypoint.sh
CMD /bin/bash ./entrypoint.sh