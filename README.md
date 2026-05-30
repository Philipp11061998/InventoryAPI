# 📦 Inventory Management API
Eine moderne Portfolio-API zur Verwaltung von Produkten, Lagern, Bestandsbewegungen und aggregierten Inventardaten.

Dieses Projekt zeigt praxisnahe Backend-Entwicklung mit .NET 8, Entity Framework Core, SQL Server, Docker, Swagger und automatisierten Tests.

[![.NET CI](https://github.com/Philipp11061998/InventoryAPI/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Philipp11061998/InventoryAPI/actions/workflows/dotnet.yml)

---

## 🧭 Übersicht

Die Inventory Management API modelliert realistisches Lager- und Bestandsmanagement. Statt den Bestand als festen Wert zu speichern, wird er aus historischen Bewegungen berechnet.

Das macht das System transparenter, auditfähiger und näher an echten Business-Prozessen.

### Aktuelle Kernfunktionen
- Produktverwaltung (Create / Read / Update / Soft Delete)
- Lagerverwaltung (Create / Read / Update / Soft Delete)
- Bestandsbewegungen (Inbound / Outbound)
- Aggregierte Bestandsabfragen
- Tokenbasierte Authentifizierung mit JWT
- Rollenbasierte Policies (`AdminOnly`, `User`)
- Swagger UI mit Bearer-Token-Unterstützung
- Zentrale Exception-Middleware

---

## 🛠 Tech Stack
- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server (Docker)
- Swagger / OpenAPI
- xUnit + SQLite InMemory für Tests
- C#

---

## 🏗 Architektur
Das Projekt ist serviceorientiert aufgebaut:
- Controller → HTTP Layer
- Services → Geschäftslogik
- Data → EF Core DbContext
- DTOs → API-Anfragen und -Antworten
- Models → Datenbank- und Domain-Objekte

### Datenfluss
Controller → Service → DbContext → SQL Server

### Warum DTOs?
DTOs schützen interne Strukturen, halten API-Antworten stabil und trennen Datenbankmodell von API-Vertrag.

---

## 🔑 Domänenlogik
### Produkte
- Anlegen
- Aktualisieren
- Abfragen
- Soft Delete (statt physischem Löschen)

### Lager
- Anlegen
- Aktualisieren
- Abfragen
- Soft Delete

### Bewegungen
- Inbound erhöht Bestand
- Outbound reduziert Bestand
- Bestand wird nicht direkt gespeichert, sondern aus Bewegungen berechnet

### Bestandsaggregation
- Gruppierung nach Produkt und Lager
- Summierung aller Bewegungen
- Echtzeit-Ermittlung des Bestands

---

## 🔐 Authentifizierung & Autorisierung
### Login
- Endpoint: `POST /api/auth/login`
- Payload: `username`, `password`
- Rückgabe: JWT als String

### JWT
- Signiert mit einem Secret aus `appsettings.json`
- Enthält Claims: `sub`, `name`, `role`, `jti`, `iat`
- Gültig für 1 Stunde

### Authorization
- Rollen werden als Claim gespeichert
- Policies:
  - `AdminOnly`
  - `User` (Admin + User)
- Geschützte Controller nutzen `[Authorize]`

### Swagger-Flow
1. `/api/auth/login` aufrufen
2. Token kopieren
3. `Authorize` in Swagger öffnen
4. `Bearer <token>` eingeben

---

## ⚙️ Fehlerbehandlung
- Zentrale Exception-Middleware
- Einheitliche JSON-Fehlerantworten
- Business-Logik bleibt in Services
- Controller bleiben schlank

---

## 🧪 Tests
- xUnit als Testframework
- SQLite InMemory für isolierte Tests
- Fokus auf Service-Logik

### Abgedeckte Bereiche
- Lagerverwaltung
- Produktverwaltung
- Bewegungslogik
- Bestandsregeln

---

## 🗄 Datenbank
- SQL Server im Docker-Container
- Initialisierung über `database/init.sql`
- ConnectionString konfigurierbar in `appsettings.json`

---

## 🚀 Lokales Setup
### Voraussetzungen
- .NET 8 SDK
- Docker

### Start
1. `docker compose up --build`
2. Swagger öffnen: `http://localhost:8080/swagger`

---

## 🔄 Typischer Workflow
1. `POST /api/auth/login`
2. Bearer-Token in Swagger einfügen
3. Produkt anlegen
4. Lager anlegen
5. Inbound-Bewegung anlegen
6. Outbound-Bewegung anlegen
7. Inventory-Übersicht abrufen

---

## 🎯 Projektziel
Dieses Projekt demonstriert praxisrelevantes Backend-Know-how:
- REST API Design
- Tokenbasierte Authentifizierung
- Rollenbasierte Autorisierung
- Serviceorientierte Architektur
- Relationale Datenmodellierung
- Docker-basierte Entwicklung
- Automatisierte Tests

---

## 🛣 Roadmap
### Bereits umgesetzt
- Service-Schicht-Architektur ✅
- EF Core + SQL Server ✅
- Swagger ✅
- Zentrale Fehlerbehandlung ✅
- JWT-Login ✅
- Rollenbasierte Policies ✅
- Automatisierte Tests ✅

### Nächste Schritte
- User-Registrierung / User-Management
- Logging / Observability
- Integrationstests für Endpunkte
- Transfer-Logik zwischen Lagern
- Low-Stock Alerts / Monitoring

---

## 👨‍💻 Portfolio-Kontext
Dieses Projekt ist Teil meines Backend-Portfolios und zeigt, wie ich aus einer einfachen Datenbankanbindung ein durchdachtes API-System mit Auth und Geschäftslogik aufgebaut habe.

---

## ✍️ Autor
Backend Portfolio Projekt von Philipp Joeris