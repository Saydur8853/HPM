# PrimeHPM: Ubuntu VPS + Docker production deployment

এই runbook অনুযায়ী একটি নতুন Ubuntu VPS-এ Git থেকে source clone করে PrimeHPM deploy করা যাবে। Recommended architecture:

```text
Internet
   |
   | 80/443
   v
Caddy (automatic HTTPS)
   |
   | Docker private network :8080
   v
HPM.Web (.NET 8 Blazor Server)
   |
   | Docker private network :1433
   v
SQL Server 2022
```

SQL Server এবং application port সরাসরি Internet-এ publish করা হবে না। Publicly open থাকবে শুধু SSH, HTTP এবং HTTPS।

এই example-এ `MSSQL_PID=Express` ব্যবহার করা হয়েছে। SQL Server Express-এর database-size/resource সীমা production load-এর জন্য যথেষ্ট কিনা আগে যাচাই করুন। সীমা ছাড়ালে বৈধ SQL Server Standard লাইসেন্স/সঠিক `MSSQL_PID` অথবা managed SQL Server ব্যবহার করতে হবে; Developer edition production-এর জন্য licensed নয়।

## 1. Production-এর আগে বাধ্যতামূলক checks

- VPS: Ubuntu 22.04/24.04 LTS, x86-64, কমপক্ষে 2 vCPU, 4 GB RAM এবং পর্যাপ্ত SSD storage। Hospital data বেশি হলে 8 GB+ RAM ব্যবহার করা ভালো।
- একটি domain/subdomain, যেমন `hpm.example.com`, VPS-এর public IP-তে A/AAAA record দিয়ে point করতে হবে।
- VPS provider-এর firewall/security group-এ TCP `22`, `80`, `443` allow করতে হবে। TCP `1433` public করা যাবে না।
- Production database-এর verified backup VPS-এর বাইরে আরেকটি secure location-এ রাখতে হবে।
- `HPM.Web/Program.cs`-এ Blazor/SignalR `DetailedErrors` বর্তমানে সব environment-এ enabled। Public go-live-এর আগে এগুলো শুধু Development-এ enabled করার code change করতে হবে। নইলে internal exception detail client-এর কাছে প্রকাশ পেতে পারে।
- Repository-এর `appsettings.json` local Windows SQL named-pipe connection ব্যবহার করে। Docker-এ সেটি ব্যবহার করা হবে না; Compose environment variable দিয়ে connection string override করা হবে।
- `RUN_HPM_WEB.cmd` development-only। Ubuntu production server-এ এটি ব্যবহার করা যাবে না।

## 2. Ubuntu প্রস্তুত করা

Server update এবং basic packages install করুন:

```bash
sudo apt update
sudo apt upgrade -y
sudo apt install -y ca-certificates curl git openssl ufw
```

Firewall configure করুন। SSH allow করার আগেই UFW enable করবেন না:

```bash
sudo ufw allow OpenSSH
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp
sudo ufw enable
sudo ufw status verbose
```

Docker Engine এবং Compose plugin Docker-এর official Ubuntu repository থেকে install করুন। নিচের commands standard official-repository setup ব্যবহার করে:

```bash
sudo install -m 0755 -d /etc/apt/keyrings
sudo curl -fsSL https://download.docker.com/linux/ubuntu/gpg \
  -o /etc/apt/keyrings/docker.asc
sudo chmod a+r /etc/apt/keyrings/docker.asc

. /etc/os-release
echo "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.asc] https://download.docker.com/linux/ubuntu ${VERSION_CODENAME} stable" \
  | sudo tee /etc/apt/sources.list.d/docker.list > /dev/null

sudo apt update
sudo apt install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin
sudo systemctl enable --now docker
sudo docker version
sudo docker compose version
```

Optional: নিজের user-কে `docker` group-এ দিলে প্রতি command-এ `sudo` লাগবে না। এটি root-equivalent permission, তাই শুধু trusted deployment user-এর জন্য করুন:

```bash
sudo usermod -aG docker "$USER"
newgrp docker
```

## 3. Repository clone করা

Application-এর জন্য একটি fixed directory ব্যবহার করুন:

```bash
sudo mkdir -p /opt/primehpm
sudo chown "$USER":"$USER" /opt/primehpm
cd /opt/primehpm

git clone <YOUR_GIT_REPOSITORY_URL> app
cd app
git checkout main
git pull --ff-only origin main
git status
git log -1 --oneline
```

Deploy করার commit hash লিখে রাখুন:

```bash
git rev-parse HEAD | tee /opt/primehpm/DEPLOYED_COMMIT
```

Private repository হলে deploy key বা read-only access token ব্যবহার করুন। Token কখনও repository URL, shell history বা committed file-এ রাখবেন না।

## 4. Docker files তৈরি করা

Repository root-এ নিচের `Dockerfile` তৈরি করুন:

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

ENV PATH="${PATH}:/root/.dotnet/tools"
RUN dotnet tool install --global dotnet-ef --version 8.0.13

COPY HPM.Web.sln ./
COPY HPM.Web/HPM.Web.csproj HPM.Web/
RUN dotnet restore HPM.Web/HPM.Web.csproj

COPY . .
RUN dotnet publish HPM.Web/HPM.Web.csproj \
    --configuration Release \
    --output /out \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build --chown=app:app /out ./
USER app

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "HPM.Web.dll"]
```

Repository root-এ `.dockerignore` তৈরি করুন:

```text
.git
.gitignore
.vs
.vscode
**/bin
**/obj
**/logs
publish
backups
.env
.env.*
```

Repository root-এ `compose.production.yml` তৈরি করুন:

```yaml
name: primehpm

services:
  db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: primehpm-db
    restart: unless-stopped
    environment:
      ACCEPT_EULA: "Y"
      MSSQL_PID: "Express"
      MSSQL_SA_PASSWORD: "${MSSQL_SA_PASSWORD}"
    volumes:
      - sql_data:/var/opt/mssql
    networks:
      - backend
    healthcheck:
      test:
        - CMD-SHELL
        - /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$${MSSQL_SA_PASSWORD}" -C -Q "SELECT 1" || exit 1
      interval: 10s
      timeout: 5s
      retries: 30
      start_period: 30s
    logging:
      options:
        max-size: "20m"
        max-file: "5"

  migrate:
    build:
      context: .
      target: build
    environment:
      ASPNETCORE_ENVIRONMENT: "Production"
      ConnectionStrings__HpmDatabase: "Server=db,1433;Database=HPM_Web;User ID=HpmApp;Password=${HPM_DB_PASSWORD};Encrypt=True;TrustServerCertificate=True;Connect Timeout=60;Max Pool Size=128;Pooling=true"
    depends_on:
      db:
        condition: service_healthy
    networks:
      - backend
    entrypoint: ["/root/.dotnet/tools/dotnet-ef"]
    command:
      - database
      - update
      - --project
      - HPM.Web/HPM.Web.csproj
      - --startup-project
      - HPM.Web/HPM.Web.csproj
      - --configuration
      - Release
      - --no-build
    restart: "no"

  app:
    build:
      context: .
      target: runtime
    container_name: primehpm-app
    restart: unless-stopped
    environment:
      ASPNETCORE_ENVIRONMENT: "Production"
      ASPNETCORE_FORWARDEDHEADERS_ENABLED: "true"
      ConnectionStrings__HpmDatabase: "Server=db,1433;Database=HPM_Web;User ID=HpmApp;Password=${HPM_DB_PASSWORD};Encrypt=True;TrustServerCertificate=True;Connect Timeout=60;Max Pool Size=128;Pooling=true"
      AllowedHosts: "${HPM_DOMAIN}"
    depends_on:
      db:
        condition: service_healthy
    expose:
      - "8080"
    networks:
      - frontend
      - backend
    logging:
      options:
        max-size: "20m"
        max-file: "10"

  caddy:
    image: caddy:2-alpine
    container_name: primehpm-caddy
    restart: unless-stopped
    environment:
      HPM_DOMAIN: "${HPM_DOMAIN}"
    ports:
      - "80:80"
      - "443:443"
      - "443:443/udp"
    volumes:
      - ./deployment/Caddyfile:/etc/caddy/Caddyfile:ro
      - caddy_data:/data
      - caddy_config:/config
    depends_on:
      - app
    networks:
      - frontend
    logging:
      options:
        max-size: "20m"
        max-file: "5"

networks:
  frontend:
  backend:
    internal: true

volumes:
  sql_data:
  caddy_data:
  caddy_config:
```

`deployment/Caddyfile` তৈরি করুন:

```caddyfile
{$HPM_DOMAIN} {
    encode zstd gzip
    reverse_proxy app:8080
}
```

Caddy WebSocket/Blazor Server traffic proxy করবে এবং domain-এর DNS সঠিক থাকলে Let's Encrypt/ZeroSSL থেকে certificate নিয়ে HTTPS renew করবে।

## 5. Secrets তৈরি করা

Repository root-এ `.env.production` তৈরি করুন। Password-এ semicolon বা quote না রেখে নিচের মতো strong generated value ব্যবহার করলে connection string parsing সহজ থাকে:

```bash
cd /opt/primehpm/app
SA_SECRET="Aa!$(openssl rand -hex 24)"
APP_SECRET="Bb!$(openssl rand -hex 24)"

umask 077
cat > .env.production <<EOF
HPM_DOMAIN='hpm.example.com'
MSSQL_SA_PASSWORD='${SA_SECRET}'
HPM_DB_PASSWORD='${APP_SECRET}'
EOF

chmod 600 .env.production
unset SA_SECRET APP_SECRET
```

`hpm.example.com` নিজের domain দিয়ে বদলাতে হবে। তারপর নিশ্চিত করুন secret file tracked নয়:

```bash
printf '\n.env\n.env.*\n' >> .git/info/exclude
git status --short
```

`.env.production` কখনও `git add`/commit করবেন না। Password একটি password manager-এ সংরক্ষণ করুন।

পরের commands ছোট করার জন্য:

```bash
export COMPOSE_FILE=compose.production.yml
export COMPOSE_ENV_FILES=.env.production
```

নতুন SSH session-এ এই দুইটি `export` আবার দিতে হবে, অথবা প্রতিটি command-এ `docker compose --env-file .env.production -f compose.production.yml ...` ব্যবহার করতে হবে।

## 6. SQL Server start এবং application login তৈরি করা

প্রথমে শুধু database container start করুন:

```bash
docker compose up -d db
docker compose ps
docker compose logs -f --tail=100 db
```

Log-এ SQL Server ready হওয়ার পর `Ctrl+C` চাপলে শুধু log-follow বন্ধ হবে; container চলতে থাকবে।

SQL shell খুলুন:

```bash
set -a
. ./.env.production
set +a
docker compose exec db /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C
```

তারপর নিচের SQL চালান। `<HPM_DB_PASSWORD>`-এর জায়গায় `.env.production`-এর exact `HPM_DB_PASSWORD` দিন:

```sql
IF DB_ID(N'HPM_Web') IS NULL
    CREATE DATABASE [HPM_Web];
GO

IF SUSER_ID(N'HpmApp') IS NULL
    CREATE LOGIN [HpmApp]
    WITH PASSWORD = N'<HPM_DB_PASSWORD>', CHECK_POLICY = ON;
GO

USE [HPM_Web];
GO

IF USER_ID(N'HpmApp') IS NULL
    CREATE USER [HpmApp] FOR LOGIN [HpmApp];
GO

ALTER ROLE [db_owner] ADD MEMBER [HpmApp];
GO
```

`QUIT` লিখে sqlcmd থেকে বের হন এবং shell variable মুছে দিন:

```bash
unset MSSQL_SA_PASSWORD HPM_DB_PASSWORD HPM_DOMAIN
```

বর্তমান application startup-এ EF migration ও কিছু guarded schema updates চলে, তাই `HpmApp`-কে আপাতত `db_owner` দেওয়া হয়েছে। পরবর্তীতে schema migration-কে আলাদা privileged identity-তে সরিয়ে application runtime permission `db_datareader`/`db_datawriter`-এ নামানো উচিত।

## 7. Existing database নাকি fresh database

### Existing local `HPM_Web` production-এ নিতে হলে

সবচেয়ে নিরাপদ পথ:

1. Local SQL Server থেকে `HPM_Web`-এর `COPY_ONLY, COMPRESSION, CHECKSUM` সহ `.bak` নিন।
2. Backup verify করুন (`RESTORE VERIFYONLY`)।
3. Secure copy/SFTP দিয়ে VPS-এ পাঠান।
4. Empty container database-এ data entry করার আগে backup restore করুন।
5. Logical data/log filename `RESTORE FILELISTONLY` দিয়ে দেখে `RESTORE DATABASE ... WITH MOVE` চালান।
6. Restore শেষ হলে `HpmApp` user/login mapping এবং role আবার verify করুন।
7. তারপর Section 8-এর migration চালান।

Production data থাকলে শুধু empty migration database deploy করে পুরোনো data বাদ দেবেন না। Backup restore test না করে cut-over করবেন না।

### একদম fresh installation হলে

Section 6-এ তৈরি empty `HPM_Web` database রেখে সরাসরি Section 8 চালান। Migration reference/master rows তৈরি করবে। প্রথমবার browser থেকে one-time hospital setup এবং Superadmin account তৈরি হবে।

## 8. Build, test এবং migration

Image build করুন:

```bash
docker compose build --pull app migrate
```

একবার automated test চালানো recommended:

```bash
docker run --rm \
  -v "$PWD:/src" \
  -w /src \
  mcr.microsoft.com/dotnet/sdk:8.0 \
  dotnet test HPM.Web.Tests/HPM.Web.Tests.csproj --configuration Release
```

Production DB backup নিয়ে migration চালান:

```bash
docker compose run --rm migrate
```

Command exit code `0` না হলে app start করবেন না। Error ঠিক করে আবার migration চালান। Application startup-ও migration attempt করে, কিন্তু production deployment-এ explicit migration command-এর সফলতা যাচাই করা বাধ্যতামূলক।

Migration history verify করুন:

```bash
set -a
. ./.env.production
set +a
docker compose exec db /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U HpmApp -P "$HPM_DB_PASSWORD" -C \
  -d HPM_Web -Q 'SELECT MigrationId FROM dbo.__EFMigrationsHistory ORDER BY MigrationId;'
unset MSSQL_SA_PASSWORD HPM_DB_PASSWORD HPM_DOMAIN
```

## 9. Application start করা

```bash
docker compose up -d app caddy
docker compose ps
docker compose logs --tail=200 app
docker compose logs --tail=100 caddy
```

Health endpoint test করুন:

```bash
curl -fsS "https://$(grep '^HPM_DOMAIN=' .env.production | cut -d= -f2-)/health"
```

Expected response-এর `status` হবে `Healthy`, `database` হবে `true`, এবং HTTP status হবে `200`। এছাড়া browser-এ domain খুলে certificate valid কিনা দেখুন।

Fresh database হলে `https://your-domain/setup` খুলে hospital name এবং প্রথম Superadmin account তৈরি করুন। Setup page existing configured database বা existing users থাকলে আবার setup করতে দেবে না। Strong unique password ব্যবহার করুন।

শেষে এই UAT checks করুন:

- Login/logout এবং password change
- Patient registration ও search
- Doctor appointment/ticket
- Diagnostic/OT/IPD cash memo print
- Pharmacy purchase, stock, POS এবং requisition
- Accounts posting/reconciliation
- Report date filter ও print
- একাধিক browser/client দিয়ে Blazor session stability
- Server/container restart-এর পর data persistence

## 10. Database backup

প্রতিদিন backup নিন এবং VPS-এর বাইরেও encrypted copy রাখুন। একটি manual backup উদাহরণ:

```bash
cd /opt/primehpm/app
set -a
. ./.env.production
set +a

STAMP="$(date +%Y%m%d-%H%M%S)"
BACKUP_NAME="HPM_Web-${STAMP}.bak"

docker compose exec -T db /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -b \
  -Q "BACKUP DATABASE [HPM_Web] TO DISK=N'/var/opt/mssql/data/${BACKUP_NAME}' WITH COPY_ONLY, COMPRESSION, CHECKSUM; RESTORE VERIFYONLY FROM DISK=N'/var/opt/mssql/data/${BACKUP_NAME}' WITH CHECKSUM;"

mkdir -p /opt/primehpm/backups
docker cp "primehpm-db:/var/opt/mssql/data/${BACKUP_NAME}" "/opt/primehpm/backups/${BACKUP_NAME}"
docker compose exec -T db rm -f "/var/opt/mssql/data/${BACKUP_NAME}"
chmod 600 "/opt/primehpm/backups/${BACKUP_NAME}"
unset MSSQL_SA_PASSWORD HPM_DB_PASSWORD HPM_DOMAIN
```

এই command-কে root-owned script + systemd timer/cron-এ schedule করা যায়। Backup retention, off-site upload, restore drill এবং alerting আলাদাভাবে configure করতে হবে। Backup আছে বললেই হবে না—নিয়মিত test restore করতে হবে।

## 11. নতুন code deploy/update করা

Deployment window-এর আগে বর্তমান commit এবং DB backup রাখুন:

```bash
cd /opt/primehpm/app
git rev-parse HEAD
git status --short
```

Working tree clean না হলে deploy বন্ধ করে পরিবর্তন review করুন। তারপর:

```bash
git fetch origin
git checkout main
git pull --ff-only origin main
git log -1 --oneline

docker compose build --pull app migrate
docker compose run --rm migrate
docker compose up -d --no-deps app
docker compose up -d caddy

docker compose ps
docker compose logs --tail=200 app
curl -fsS "https://$(grep '^HPM_DOMAIN=' .env.production | cut -d= -f2-)/health"
git rev-parse HEAD | tee /opt/primehpm/DEPLOYED_COMMIT
```

Migration-এর আগে Section 10 অনুযায়ী verified database backup নিতে হবে। `docker compose down -v` চালাবেন না—`-v` দিলে SQL data volume delete হয়ে data loss হবে।

## 12. Rollback

শুধু application code rollback করতে হলে:

```bash
cd /opt/primehpm/app
git log --oneline -10
git checkout <PREVIOUS_GOOD_COMMIT>
docker compose build app
docker compose up -d --no-deps app
```

তারপর `/health`, logs এবং UAT verify করুন। পরের স্বাভাবিক deploy-এর আগে আবার `git checkout main` করতে হবে।

নতুন migration যদি destructive বা old binary-এর সঙ্গে incompatible হয়, শুধু পুরোনো image/code চালানো নিরাপদ নয়। Maintenance window-এ app stop করে deployment-পূর্ব verified DB backup restore করতে হবে:

```bash
docker compose stop app
# DBA-reviewed database restore এখানে হবে
docker compose up -d app
```

Rollback decision এবং database restore সম্ভব হলে DBA/technical owner approve করবে।

## 13. Monitoring এবং troubleshooting

Useful commands:

```bash
docker compose ps
docker compose logs -f --tail=200 app
docker compose logs -f --tail=200 db
docker compose logs -f --tail=200 caddy
docker stats
df -h
free -h
```

Common checks:

- `502 Bad Gateway`: app container running কিনা এবং app log দেখুন।
- `/health` returns `503`: SQL container, credentials, database name এবং migration log দেখুন।
- HTTPS certificate issue: DNS A/AAAA record, ports 80/443 এবং Caddy log দেখুন।
- SQL login failure: `.env.production` password এবং `HpmApp` login/user mapping verify করুন।
- Migration fails: app start না করে error resolve করুন; প্রয়োজনে backup restore করুন।
- Blazor disconnect: proxy/network stability এবং browser console দেখুন; Caddy WebSocket automatically proxy করে।
- Disk growing: Docker logs, SQL data এবং backup retention inspect করুন।

OS/Docker security updates নিয়মিত apply করুন। SQL এবং application image update করার আগে staging/backup-restore test করুন; `latest` image blindভাবে production-এ restart করবেন না। Stable deployment চাইলে tested image digest pin করুন।

## Final go-live checklist

- [ ] Domain VPS IP-তে resolve করে
- [ ] VPS/provider firewall-এ শুধু প্রয়োজনীয় ports open
- [ ] Repository clean এবং intended commit deploy হয়েছে
- [ ] `.env.production` permission `600`, Git-এ tracked নয়
- [ ] Detailed errors production-এ disabled
- [ ] SQL port host/public-এ published নয়
- [ ] App dedicated `HpmApp` login ব্যবহার করে; app SA ব্যবহার করে না
- [ ] Verified DB backup এবং off-site copy আছে
- [ ] EF migration সফল এবং history verified
- [ ] `/health` HTTP 200 এবং database healthy
- [ ] HTTPS certificate valid ও auto-renew operational
- [ ] First Superadmin strong password দিয়ে তৈরি
- [ ] Critical workflow UAT complete
- [ ] Daily backup, retention, restore test এবং monitoring configured
- [ ] Rollback commit ও deployment-পূর্ব DB backup চিহ্নিত
