# ITAM CLI → API Project TODO

## 🎯 Goal

Build a simple IT Asset Management system using:

* .NET (CLI first)
* Dapper
* PostgreSQL

Focus:

* Learn deeply
* Keep architecture clean
* Enable future Web API + monetization

---

# ✅ Phase 0 — Repo Setup (Day 1)

* [ ] Create GitHub repo (`itam-core` or similar)
* [ ] Initialize .NET solution

  * [ ] `dotnet new sln`
* [ ] Create projects:

  * [ ] `Domain`
  * [ ] `Application`
  * [ ] `Infrastructure`
  * [ ] `Cli`
* [ ] Add projects to solution
* [ ] Setup basic folder structure

---

# 🧱 Phase 1 — Domain Modeling (Days 2–3)

Define core entities:

* [ ] Asset

  * Id, Name, Type, SerialNumber, PurchaseDate

* [ ] User

  * Id, Name, Email

* [ ] Assignment

  * AssetId, UserId, AssignedAt, ReturnedAt

* [ ] Location (optional)

* [ ] Vendor (optional)

* [ ] Create basic domain classes

* [ ] Add simple validation (required fields, etc.)

---

# 🗄️ Phase 2 — Database Setup (Days 3–5)

* [ ] Spin up PostgreSQL (Docker or local)

* [ ] Create database: `itam_dev`

* [ ] Create tables:

  * [ ] assets
  * [ ] users
  * [ ] assignments

* [ ] Add migrations tool (DbUp or FluentMigrator)

* [ ] Write initial migration script

---

# ⚙️ Phase 3 — Infrastructure (Dapper) (Days 5–7)

* [ ] Add Dapper + Npgsql packages

* [ ] Create DB connection factory

* [ ] Implement repositories:

  * [ ] AssetRepository
  * [ ] UserRepository
  * [ ] AssignmentRepository

* [ ] Write basic queries:

  * [ ] Insert asset
  * [ ] Get all assets
  * [ ] Assign asset
  * [ ] Get assigned/unassigned assets

---

# 🧠 Phase 4 — Application Layer (Days 7–10)

* [ ] Create services:

  * [ ] AssetService
  * [ ] AssignmentService

* [ ] Implement use cases:

  * [ ] AddAsset
  * [ ] AssignAsset
  * [ ] ListAssets
  * [ ] GetAuditReport

* [ ] Keep business logic OUT of CLI

---

# 🖥️ Phase 5 — CLI Interface (Days 10–12)

* [ ] Setup CLI entry point

* [ ] Implement commands:

  * [ ] `add-asset`
  * [ ] `list-assets`
  * [ ] `assign-asset`
  * [ ] `audit-report`

* [ ] Basic console output (keep simple)

---

# 📊 Phase 6 — Reporting (Days 12–14)

* [ ] Build:

  * [ ] Unassigned assets report
  * [ ] Assets by user
  * [ ] Assignment history

* [ ] Optimize at least one query (learn Dapper deeply)

---

# 🧪 Phase 7 — Polish & Learn (Ongoing)

* [ ] Add logging
* [ ] Add configuration (appsettings.json)
* [ ] Improve error handling
* [ ] Refactor messy code
* [ ] Document key decisions in README

---

# 🚀 Phase 8 — API Transition (Later)

(Only after CLI feels solid)

* [ ] Create `Api` project (ASP.NET Core)
* [ ] Add controllers:

  * [ ] AssetsController
  * [ ] AssignmentsController
* [ ] Reuse Application layer
* [ ] Add basic REST endpoints
* [ ] Add Swagger

---

# 💡 Stretch Goals (Optional)

* [ ] Authentication (JWT)
* [ ] Multi-tenant support
* [ ] Export to CSV
* [ ] Simple Vue frontend
* [ ] Dockerize app

---

# 🧠 Rules for This Project

* Keep it simple
* Favor clarity over cleverness
* No business logic in controllers/CLI
* Learn something every session
* Ship small, often

---

# 🏁 Definition of “Done” (MVP)

* Can add assets
* Can assign assets
* Can view asset list
* Can generate simple report

---

# 🔥 Reminder

This is not just a CLI tool.

You are building:
→ A system
→ A portfolio piece
→ A potential SaaS foundation

