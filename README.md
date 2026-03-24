# 📦 Inventory Management API

Eine moderne REST API zur Verwaltung von Produkten, Lagern, Bestandsbewegungen und aggregierten Lagerbeständen.

Dieses Projekt wurde als Backend-Portfolio-Projekt entwickelt, um praxisnahe API-Entwicklung, Business-Logik, Datenbankintegration und eine saubere Projektstruktur mit .NET 8, Entity Framework Core, SQL Server und Docker zu demonstrieren.

---

## 🧭 Überblick

Die Inventory Management API bildet einen realistischen Lager- und Bestandsverwaltungsprozess ab.

Anstatt einen statischen Lagerbestand pro Produkt zu speichern, wird der aktuelle Bestand aus historischen Bestandsbewegungen berechnet. Dadurch ist das System transparenter, flexibler und näher an realen Business-Szenarien.

### Aktuell unterstützt das Projekt:
- Produktverwaltung
- Lagerverwaltung
- Bestandsbewegungen (Inbound / Outbound)
- Aggregierte Bestandsabfragen
- Soft Delete für Produkte und Lager
- Validierung zentraler Business-Regeln (z. B. kein negativer Bestand)

---

## 🛠 Tech Stack

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server (Docker)
- Swagger / OpenAPI
- xUnit + SQLite (InMemory) für Tests
- C#

---

## 🏗 Architektur

Das Projekt folgt einer klaren, servicebasierten Struktur:

- Controller → HTTP Layer (Requests / Responses)
- Services → Business-Logik
- Data → EF Core DbContext
- DTOs → Request- und Response-Modelle
- Models → interne Domain- und Datenbankrepräsentation

### Datenfluss

Controller → Service → DbContext → SQL Server

### Response-DTOs

Response-DTOs werden bewusst genutzt, um:
- interne Datenstrukturen zu kapseln
- API-Antworten stabil zu halten
- Änderungen an der Datenbank vom Client zu entkoppeln

---

## 🔑 Kernkonzepte

### Produkte
Produkte können erstellt, aktualisiert, abgefragt und per Soft Delete deaktiviert werden.

### Lager (Warehouses)
Repräsentieren physische Lagerorte.

### Bestandsbewegungen
Bestände werden nicht direkt gespeichert, sondern über Bewegungen modelliert:

- Inbound → erhöht Bestand
- Outbound → reduziert Bestand

### Bestandsaggregation
Der aktuelle Bestand wird berechnet durch:
- Gruppierung nach Produkt und Lager
- Summierung aller Bewegungen (mit Vorzeichen)

---

## ⚙️ Fehlerbehandlung & Logging

- Zentrale Exception Middleware
- Einheitliche JSON-Fehlerantworten
- Controller enthalten keine try/catch-Blöcke
- Business-Fehler werden auf passende HTTP-Statuscodes gemappt
- Strukturierte Logs für Fehlerfälle

---

## 🧪 Tests

Das Projekt enthält automatisierte Tests für zentrale Business-Logik:

- xUnit als Test-Framework
- SQLite InMemory für isolierte Tests ohne echte Datenbank

### Abgedeckte Szenarien:
- Outbound ohne Bestand → Fehler
- Inbound → erfolgreich
- Inbound + Outbound → korrekter Restbestand

---

## 🧪 Qualitätssicherung

Die Business-Logik wird durch automatisierte Tests abgesichert.

Dabei werden sowohl Fehlerfälle (z. B. Outbound ohne Bestand) als auch erfolgreiche Szenarien validiert.

Die Tests laufen unabhängig von der produktiven Datenbank mithilfe von SQLite InMemory.

---

## 📏 Business-Regeln

- Produkte müssen existieren, bevor Bewegungen erstellt werden
- Lager müssen existieren
- Inaktive Produkte/Lager dürfen nicht verwendet werden
- Amount muss größer als 0 sein
- Outbound wird verhindert, wenn Bestand nicht ausreicht
- Soft Delete statt physischem Löschen

---

## 🧠 Design-Entscheidung: Bewegungsbasierter Bestand

Der Bestand wird bewusst nicht direkt gespeichert, sondern aus Bewegungen berechnet.

### Vorteile:
- vollständige Nachvollziehbarkeit (Audit)
- bessere Debugbarkeit
- realitätsnahe Modellierung
- einfache Erweiterbarkeit (Transfers, Reports, Alerts)

---

## 🗄 Datenbank

- SQL Server läuft in Docker
- Initialisierung über SQL-Skript

Automatisch:
- Erstellung der Datenbank
- Erstellung der Tabellen
- Seed-Daten

---

## 🚀 Lokales Setup

### Voraussetzungen
- .NET 8 SDK
- Docker

### Start

1. SQL Server Container starten
2. Init-Skript ausführen lassen
3. API starten

Swagger erreichbar unter:

http://localhost:8080/swagger

---

## 🔄 Beispiel-Workflow

1. Produkt erstellen  
2. Lager erstellen  
3. Bestand hinzufügen (Inbound)  
4. Bestand reduzieren (Outbound)  
5. Inventory abfragen  

---

## 🎯 Projektziele

Dieses Projekt demonstriert praxisrelevante Backend-Skills:

- REST API Design
- Service-basierte Architektur
- Asynchrone Datenbankzugriffe
- Business-Logik-Validierung
- Relationale Datenmodellierung
- LINQ & Aggregationen
- Docker-basierte Entwicklung
- Testgetriebene Absicherung von Logik

---

## 🛣 Roadmap / Geplante Erweiterungen

- Response DTOs ✅  
- Zentrale Exception Middleware ✅  
- Automatisierte Tests ✅  
- Authentifizierung  
- Logging-Ausbau  
- Erweiterte Business-Features  
- Transfers zwischen Lagern  
- Low-Stock Monitoring  

---

## 🔐 Authentifizierung (geplant)

Langfristig ist die Integration moderner Authentifizierungsmechanismen geplant (z. B. tokenbasiert oder cloudbasiert).

Der Fokus liegt aktuell bewusst auf:
- stabiler Business-Logik
- sauberem Backend-Design

---

## 👨‍💻 Portfolio-Kontext

Dieses Projekt ist Teil meines Backend-Portfolios und dient zur Vertiefung von:

- .NET API Entwicklung
- Backend-Architektur
- Datenmodellierung
- Service-orientiertem Design
- realitätsnaher Business-Logik

---

## ✍️ Autor

Backend Portfolio Projekt von Philipp Joeris