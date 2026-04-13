# Flow ITAM

**Flow ITAM** is a development project focused on building a simple, scalable IT Asset Management (ITAM) system from the ground up using modern .NET technologies.

This project is intentionally designed as a **learning-first, architecture-focused system**, with the long-term potential to evolve into a production-ready API and SaaS offering.

---

## 🎯 Purpose

The goal of Flow ITAM is to:

* Deepen understanding of **backend system design**
* Practice **clean architecture principles**
* Gain hands-on experience with **data access patterns (Dapper)**
* Build a system that can evolve from a **CLI tool → Web API → full application**
* Explore real-world concerns like:

  * Data modeling
  * Performance
  * Maintainability
  * Scalability

This is not just a coding exercise—it is an intentional effort to grow toward **high-impact, system-level engineering**.

---

## 🧱 Core Features (MVP)

* Track assets (laptops, monitors, etc.)
* Assign assets to users
* View asset inventory
* Generate simple audit reports

Future iterations may include:

* REST API endpoints
* Authentication & authorization
* Multi-tenant support
* Frontend interface

---

## 🏗️ Architecture

This project follows a layered approach inspired by Clean Architecture:

* **Domain**
  Core business entities and rules

* **Application**
  Use cases and orchestration logic

* **Infrastructure**
  Data access (PostgreSQL via Dapper)

* **CLI**
  Initial interface for interacting with the system

* *(Planned)* **API**
  ASP.NET Core Web API layer

The goal is to keep business logic independent from delivery mechanisms, allowing the system to evolve without major rewrites.

---

## ⚙️ Technology Stack

* **.NET (C#)**
  Core application framework

* **Dapper**
  Lightweight ORM for efficient, explicit data access

* **PostgreSQL**
  Relational database for structured data and reporting

* **CLI (Command-Line Interface)**
  Initial interface for rapid development and testing

* *(Planned)* **ASP.NET Core Web API**
  For external access and integration

---

## 🧠 Development Approach

This project emphasizes:

### 1. Iterative Development

Small, incremental changes over time rather than large rewrites.

### 2. Learning by Building

Each feature is an opportunity to explore:

* Better design decisions
* Tradeoffs in data access
* Real-world constraints

### 3. Simplicity First

Avoid overengineering. Focus on:

* Clear code
* Understandable structure
* Maintainable patterns

### 4. Separation of Concerns

* Business logic is isolated from infrastructure and UI
* The CLI is treated as a temporary interface layer

### 5. Performance Awareness

Using Dapper to better understand:

* SQL queries
* Execution performance
* Data access patterns

---

## 🚀 Project Roadmap

### Phase 1: CLI Foundation

* Core domain + database
* Basic asset tracking and assignment
* Simple reporting

### Phase 2: API Layer

* Introduce ASP.NET Core Web API
* Expose core functionality via REST endpoints

### Phase 3: Expansion

* Authentication & authorization
* Multi-tenant capabilities
* Enhanced reporting

### Phase 4: Frontend (Optional)

* JavaScript-based UI for broader usability

---

## 📚 Why This Project Exists

Flow ITAM exists to:

* Bridge the gap between **writing code** and **designing systems**
* Build practical experience with tools and patterns used in real-world applications
* Create a foundation for potential future products

---

