FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

ARG version
ENV APP_VERSION=$version
LABEL VERSION=$version

WORKDIR /app
COPY . .
HEALTHCHECK --interval=30s --timeout=5s --retries=1 CMD curl --silent --fail http://localhost:80/health || exit 1
CMD ["dotnet", "BioEngine.BRC.Jobs.dll"]
