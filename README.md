
# 📘 Book API (.NET 8 + MongoDB)

Detta är backenddelen av BookApp – en RESTful API byggd med **.NET 8 C#**, kopplad till **MongoDB** och skyddad med **JWT-autentisering** (via cookies).

Frontend-repo: [https://github.com/huenguyensw/BookApi](https://github.com/huenguyensw/BookApi)

---

## 🔧 Funktioner

- ✅ CRUD för böcker (Skapa, Läs, Uppdatera, Radera)
- 🔐 JWT-baserad autentisering (lagras i cookies)
- 📦 MongoDB som databas
- 🧪 Swagger UI för test av endpoints
- 📂 Strukturerad med Services, Models och Kontrollers

---

## 🛠️ Teknisk stack

- .NET 8 Web API  
- MongoDB (Atlas)
- JWT (via `System.IdentityModel.Tokens.Jwt`)
- Swagger (för test & dokumentation)

---

## 🚀 Kom igång

### 1. Klona repot

```bash
git clone https://github.com/huenguyensw/book-api.git
cd book-api
```

### 2. Konfigurera miljövariabler

Skapa en `.env`-fil i rotmappen och fyll i:

```env
MYAPP_MONGODB_CONNECTION=mongodb+srv://<username>:<password>@cluster0.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0
MYAPP_JWT_SECRET_KEY=din-superhemliga-nyckel
MYAPP_PORT=10000
MYAPP_FRONTEND_URLS=http://localhost:4200,https://book-app-delta-hazel.vercel.app
```

> 🔒 Publicera aldrig din riktiga `.env`-fil. Använd en `.env.example` i publika projekt.

### 3. Kör applikationen

```bash
dotnet run
```

API:t är då tillgängligt på `http://localhost:10000`.

---

## 🧪 Testa API:t

När servern är igång, gå till:

```
http://localhost:10000/swagger
```

Där kan du testa:

- POST `/api/Auth/login` – logga in och få JWT-token i cookie
- POST `/api/Auth/register` – skapa ny användare
- GET/POST/PUT/DELETE `/api/Books` – bokhantering (kräver inloggning)

---

## 🔐 Autentisering

- När användaren loggar in returneras en **JWT-token** som sparas som **HTTPOnly-cookie**.
- Alla CRUD-endpoints för böcker skyddas med `[Authorize]`.
- Token valideras vid varje förfrågan.

---

## 🌐 Deployment

- API:t är live på Render:  
  [https://bookapi-8cvo.onrender.com](https://bookapi-8cvo.onrender.com)

---

## 📁 Projektstruktur

```text
📦 BookAPI
├── Controllers/
│   ├── AuthController.cs
│   └── BooksController.cs
├── Models/
├── Services/
├── Program.cs
└── .env
```

---

## ✅ Kommande förbättringar

- Rate limiting & loggning
- Bättre felhantering (problem details)
- Roll-baserad åtkomstkontroll
- Enhetstester

---

## 📬 Kontakt

För frågor eller feedback:  
📧 huenguyensw@gmail.com  
🔗 [LinkedIn](https://www.linkedin.com/in/huenguyensw)

