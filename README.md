# TS3 to TS6 Database Converter

This project is a web-based tool for converting TeamSpeak 3 SQLite databases to the format compatible with TeamSpeak 6 (Server Release 5).

You can use this hosted version [ts3to6](https://ts3to6.janus.ovh)

## 🚀 Deployment with Docker

The easiest way to run the application is using Docker.

### Prerequisites
- [Docker](https://docs.docker.com/get-docker/) installed.
- [Docker Compose](https://docs.docker.com/compose/install/) (usually included with Docker Desktop).

### 1. Using Docker Compose (Recommended)

Create a `docker-compose.yml` file with the following content:

```yaml
services:
  ts3to6:
    image: ghcr.io/anthodingo/ts3to6:latest
    container_name: ts3to6
    restart: unless-stopped
    ports:
      - "8080:8080"
    environment:
      # Duration (in minutes) before converted files are deleted from cache (default: 60)
      - RETENTION_MINUTES=60
```

Then, start the container:
```bash
docker compose up -d
```

The interface will be accessible at `http://localhost:8080`.

### 2. Using Docker Run

If you prefer not to use Compose:

```bash
docker run -d \
  --name ts3to6 \
  -p 8080:8080 \
  -e RETENTION_MINUTES=60 \
  ghcr.io/anthodingo/ts3to6:latest
```

## ⚙️ Configuration

The application can be configured via environment variables:

| Variable | Description | Default Value |
| :--- | :--- | :--- |
| `RETENTION_MINUTES` | Time before converted files are removed from the cache (in minutes). | `60` |
| `ASPNETCORE_URLS` | The URL the application listens on inside the container. | `http://+:8080` |

## 🛠️ Development (Local Build)

If you wish to build the image yourself locally:

```bash
docker build -t ts3to6:local ./ts3to6
```

## 📄 License

This project is licensed under the [MIT](LICENSE) License.
