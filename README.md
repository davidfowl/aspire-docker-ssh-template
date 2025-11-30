# Aspire Docker + SSH Template

A starter template for deploying [Aspire](https://aspire.dev) applications with Docker over SSH.

## Prerequisites

- [Aspire CLI](https://aspire.dev/get-started/install-cli/)
- Docker

## Quick Start

1. Use this template
2. Add your services to the AppHost
3. Deploy:
   ```bash
   aspire deploy
   ```

Or generate a GitHub Actions workflow:
```bash
aspire do gh-action-dcenv
```

## What's Included

- Astro frontend
- AppHost configured for Docker Compose + SSH deploy
- Works with any Linux server running Docker

## How It Works

The AppHost configures a Docker Compose environment with SSH deploy support. YARP is used as a reverse proxy to serve the frontend and route requests.

### Direct Deploy

Run `aspire deploy` to deploy directly from your machine. This will:

1. Build container images for your services
2. Push them to a container registry
3. SSH into your target server
4. Deploy via Docker Compose

### CI/CD with GitHub Actions

Run `aspire do gh-action-dcenv` to set up automated deployments. This will:

- Generate a GitHub Actions workflow
- Create a GitHub environment with required secrets and variables
- Configure parameters (SSH host, registry, etc.)

Once set up, every push triggers an automatic deployment.

## HTTPS

To enable HTTPS, set `EnableHttps` to `true` in `apphost.settings.json`:

```json
{
    "EnableHttps": true
}
```

When enabled, the template uses [Certbot](https://certbot.eff.org/) to automatically obtain and configure Let's Encrypt certificates. You'll need to provide two parameters during deployment:

- `domain` — Your domain name (e.g., `example.com`)
- `letsencrypt-email` — Email for Let's Encrypt notifications

Certbot runs as a container, obtains the certificate, then YARP starts with HTTPS configured. Certificates are stored in a shared Docker volume and auto-renewed.

## Learn More

- [aspire.dev](https://aspire.dev)
- [GitHub](https://github.com/dotnet/aspire)
