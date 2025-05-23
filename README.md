# GoogleEmailApi.Core

A simple **ASP NET Core Web API** that sends emails via the **Gmail API** using OAuth 2.0 credentials stored in **MongoDB**.

---

## Features

- Sends email with **Subject**, **Body**, and optional **Attachments**
- Uses **Google OAuth 2.0** Refresh Token flow (no interactive re‑consent required at runtime)
- Stores `ClientId` / `ClientSecret` / `RefreshToken` in **MongoDB**
- Exposes Swagger UI for easy testing (`/swagger`)

---

## Prerequisites

- [.NET 7 SDK](https://dotnet.microsoft.com/download)
- [MongoDB Community Server](https://www.mongodb.com/try/download/community) (running locally or remote)
- A **Gmail** account with an **OAuth 2.0** Web application client set up in Google Cloud Console

---

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/<your-username>/GoogleEmailApi.Core.git
cd GoogleEmailApi.Core
```

### 2. Install dependencies

```bash
dotnet restore
```

### 3. Configure MongoDB connection

Edit `appsettings.json` and update under `MongoDb`:

```json
"MongoDb": {
  "ConnectionString": "mongodb://localhost:27017",
  "DatabaseName": "MailConfigDB"
}
```

### 4. Seed your Gmail credentials

Use MongoDB Compass or shell to insert into `MailConfigDB.Credentials`:

```js
use MailConfigDB

db.Credentials.insertOne({
  key:          "gmail",
  clientId:     "<YOUR_CLIENT_ID>",
  clientSecret: "<YOUR_CLIENT_SECRET>",
  refreshToken: "<YOUR_REFRESH_TOKEN>"
});
```

Replace `<YOUR_...>` with values from your Google Cloud OAuth client.

### 5. Run the API

```bash
dotnet run
```

The API will start on `https://localhost:5001` (by default).

---

## Swagger UI

Navigate to:

```
https://localhost:5001/swagger
```

Use the **POST /api/email/send** endpoint:

- **to** (string): recipient email address
- **subject** (string): email subject
- **body** (string): plain-text email body
- **attachments** (file[]): optional files

Test directly from the UI by clicking **Try it out**.

---

## EmailController Overview

The `EmailController` handles:

1. Fetching OAuth credentials from MongoDB via `CredentialService`.
2. Exchanging Refresh Token for Access Token.
3. Building a `MimeMessage` with any attachments.
4. Sending via `Users.Messages.Send` on the Gmail API.

---

## Folder Structure

```
GoogleEmailApi.Core/
├── Controllers/
│   └── EmailController.cs       # API logic
├── Models/
│   └── Credential.cs            # MongoDB document mapping
├── Services/
│   └── CredentialService.cs     # MongoDB access
├── appsettings.json             # Configuration
├── Program.cs                   # App startup & DI
├── README.md                    # Project overview
└── GoogleEmailApi.Core.csproj   # Project file
```

---

## License

This project is licensed under the MIT License. Feel free to use and modify.


