# PrimeHPM: Production Performance & Route Latency Optimization Guide

লাইভ সার্ভারে এক ইউআরএল রাউট থেকে অন্য রাউটে যাওয়ার সময় রেসপন্স আসতে দেরি হওয়া (Slow Route Transition/Lag) দূর করার জন্য এই গাইডটি প্রস্তুত করা হয়েছে।

---

## ১. কোড লেভেলে যা যা ফিক্স করা হয়েছে (Code Changes Applied)

1. **Blazor Server Prerendering নিষ্ক্রিয় করা (`App.razor`):**
   - আগে বাই-ডিফল্ট `@rendermode="InteractiveServer"` থাকায় প্রতিবার পেজে ঢোকার সময় ব্যাকগ্রাউন্ডে সার্ভার-সাইড প্রি-রেন্ডারিং (SSR) চালু হচ্ছিল। ফলে পেজের ডজনখানেক ডাটাবেজ কোয়েরি শেষ না হওয়া পর্যন্ত ব্রাউজারে রেসপন্স আসত না।
   - এখন `new InteractiveServerRenderMode(prerender: false)` সেট করা হয়েছে এবং পেজ লেভেলের ডুপ্লিকেট রেন্ডারমোড সরিয়ে ফেলা হয়েছে। এতে যেকোনো রাউটে ক্লিক করলে মিলিসেকেন্ডের মধ্যে পেজের শেল/ইউআই লোড হবে এবং ডাটা ব্যাকগ্রাউন্ডে অ্যাসিঙ্কভাবে আসবে।

2. **ডাটাবেজ কমান্ড টাইমআউট ও রিট্রাই টিউনিং (`Program.cs`):**
   - আগে `CommandTimeout(180)` ও `EnableRetryOnFailure(5)` ছিল। কোনো কোয়েরি আটকে গেলে বা নেটওয়ার্ক সামান্য স্লো হলে পুরো সার্ভার প্রায় ৩ থেকে ১৫ মিনিট পর্যন্ত ফ্রিজ হয়ে থাকত।
   - এটি কমিয়ে `CommandTimeout(30)` ও `EnableRetryOnFailure(3)` করা হয়েছে, যাতে কোনো কোয়েরি অকারণে কানেকশন পুলে আটকে না থাকে।

---

## ২. সার্ভার সাইডে যা যা করতে হবে (Mandatory Server-Side Configurations)

সার্ভারের ধীরগতির মূল কারণ লগে দেখা গেছে: **SQL Server-এর "Post-Login phase" হ্যান্ডশেকে ২৫-৩০ সেকেন্ড সময় নষ্ট হচ্ছিল** এবং **কানেকশন পুল খালি হওয়ার অপেক্ষায় ছিল**। নিচে উল্লেখিত পদক্ষেপগুলো সার্ভারে অবশ্যই প্রয়োগ করুন:

### ধাপ ১: Connection String অপ্টিমাইজ করা

আপনার সার্ভারের environment variable (`.env.production`) বা `appsettings.Production.json`-এ Connection String চেক করুন।

**ভুল বা সমস্যাযুক্ত কনফিগারেশন:**
```text
# ❌ Named instance বা পোর্ট ছাড়া হোস্টনেম দিলে UDP 1434 ব্রাউজার কল করতে গিয়ে ৩০ সেকেন্ড আটকে থাকে
Server=localhost\SQLEXPRESS;...
Server=localhost;...
Server=db:1433;...  (SQL Client-এ কোলন নয়, কমা লাগে)
Connect Timeout=60; (খুব দীর্ঘ টাইমআউট)
```

**সঠিক এবং দ্রুততম কনফিগারেশন:**
* **ডকার পরিবেশের জন্য:**
  ```env
  ConnectionStrings__HpmDatabase="Server=db,1433;Database=HPM_Web;User ID=HpmApp;Password=YOUR_STRONG_PASSWORD;Encrypt=True;TrustServerCertificate=True;Connect Timeout=15;Max Pool Size=128;Pooling=true;"
  ```
* **লোকাল/সরাসরি হোস্টেড লিনাক্স বা উইন্ডোজের জন্য:**
  ```env
  ConnectionStrings__HpmDatabase="Server=127.0.0.1,1433;Database=HPM_Web;User ID=HpmApp;Password=YOUR_STRONG_PASSWORD;Encrypt=True;TrustServerCertificate=True;Connect Timeout=15;Max Pool Size=128;Pooling=true;"
  ```

> ⚠️ **বিশেষ লক্ষণীয়:** 
> 1. অবশ্যই `127.0.0.1` বা নির্দিষ্ট আইপি দিন (যাতে IPv6 `::1` ফলব্যাক করতে ৩০ সেকেন্ড দেরি না হয়)।
> 2. পোর্টের জন্য কোলন নয়, কমা ব্যবহার করুন: `,1433`।
> 3. `Connect Timeout=15` দিন, যাতে কোনো সমস্যা হলে দ্রুত ফেইল করে রিট্রাই করে।

---

### ধাপ ২: SQL Server-এ READ_COMMITTED_SNAPSHOT (RCSI) অন করা (অত্যন্ত গুরুত্বপূর্ণ)

SQL Server-এ ডিফল্টভাবে রিড ও রাইট অপারেশনের মধ্যে টেবিল লেভেলে লক (Lock contention) তৈরি হয়। একটি ইউজার কোনো ভাউচার বা টিকেট সেভ করার সময় অন্য পেজ সামারি ক্যালকুলেট করতে গেলে ৩০-৬০ সেকেন্ড আটকে থাকে।

ডাটাবেজে **Row Versioning (RCSI)** অন করলে রিডার এবং রাইটার একে অপরকে কখনোই ব্লক করবে না:

সার্ভারের SQL Server-এ গিয়ে নিচের কোয়েরিটি একবার রান করুন:
```sql
USE master;
ALTER DATABASE [HPM_Web] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
ALTER DATABASE [HPM_Web] SET READ_COMMITTED_SNAPSHOT ON;
ALTER DATABASE [HPM_Web] SET MULTI_USER;
GO
```

যাচাই করতে রান করুন:
```sql
SELECT name, is_read_committed_snapshot_on FROM sys.databases WHERE name = 'HPM_Web';
-- মান 1 (True) দেখালে সফল।
```

---

### ধাপ ৩: Reverse Proxy (Caddy / Nginx) বাফারিং ও WebSocket কনফিগারেশন

Blazor Server নির্বিঘ্নে চলার জন্য রিভার্স প্রক্সিতে রিয়েল-টাইম বাফারিং বন্ধ থাকতে হবে।

#### ক) Caddy ব্যবহার করলে (`Caddyfile`):
`deployment/Caddyfile`-এ `flush_interval -1` নিশ্চিত করুন:
```caddyfile
{$HPM_DOMAIN} {
    encode zstd gzip

    reverse_proxy app:8080 {
        flush_interval -1
    }
}
```

#### খ) Nginx ব্যবহার করলে:
Nginx সাইট কনফিগারেশনে WebSocket এবং বাফারিং অফ রাখুন:
```nginx
location / {
    proxy_pass http://127.0.0.1:8080;
    proxy_http_version 1.1;
    proxy_set_header Upgrade $http_upgrade;
    proxy_set_header Connection "upgrade";
    proxy_set_header Host $host;
    proxy_cache_bypass $http_upgrade;
    
    # Blazor SignalR-এর জন্য বাধ্যতামূলক
    proxy_buffering off;
    proxy_read_timeout 300s;
}
```

---

### ধাপ ৪: Cloudflare থাকলে সেটিংস ঠিক করা

যদি ডোমেইনটি Cloudflare-এর মাধ্যমে প্রক্সি করা থাকে (Orange Cloud On):
1. **Cloudflare Dashboard** -> **Network** -> **WebSockets** নিশ্চিত করুন **ON** আছে।
2. **Speed** -> **Optimization** -> **Rocket Loader** অবশ্যই **OFF** করে দিন (রকেট লোডার ব্লেজরের `blazor.web.js` স্ক্রিপ্টের এক্সিকিউশন ডিলে করায়)।
3. **Speed** -> **Optimization** -> **Auto Minify** (HTML, JS, CSS) আনচেক/বন্ধ রাখুন।

---

### ধাপ ৫: লাইভ সার্ভারে ডিপ্লয় ও ভেরিফিকেশন

কোড সার্ভারে পুল করার পর অ্যাপ কনটেইনার বিল্ড ও রিস্টার্ট করুন:
```bash
cd /opt/primehpm/app
git pull origin main

# Docker Compose রিস্টার্ট
docker compose -f compose.production.yml down
docker compose -f compose.production.yml up -d --build
```

#### দ্রুত যাচাই করার নিয়ম:
1. ব্রাউজারে সাইট ওপেন করুন এবং `F12` প্রেস করে **Network** ট্যাবে যান।
2. ফিল্টারে **WS** নির্বাচন করুন এবং পেজ রিফ্রেশ করুন।
3. `_blazor` ফাইলটির স্ট্যাটাস কোড **`101 Switching Protocols`** দেখতে পাবেন।
4. এখন মেনুর বিভিন্ন রাউটে (যেমন `/appointments`, `/patients`, `/tickets`) ক্লিক করুন — সঙ্গে সঙ্গে ট্রানজিশন হয়ে যাবে, কোনো দীর্ঘ অপেক্ষা থাকবে না।
