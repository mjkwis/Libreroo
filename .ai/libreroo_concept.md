# 📚 Libreroo — Concept Document

## 🎯 Goal

Build a small fullstack application to demonstrate ability to work in .NET ecosystem while leveraging strong Java
backend experience.

---

## 🧾 Elevator Pitch

Libreroo is a lightweight library management system built with ASP.NET Core and React/Angular. It allows managing books,
authors, members, and borrowing operations. The project demonstrates backend architecture, REST API design, and business
logic implementation in .NET.

---

## 🧱 Core Domain

### Entities

- **Book**
    - Id
    - Title
    - AuthorId
    - AvailableCopies

- **Author**
    - Id
    - Name

- **Member**
    - Id
    - Name
    - Email

- **Loan**
    - Id
    - BookId
    - MemberId
    - BorrowDate
    - ReturnDate

---

## 🔁 Relationships

- Author → Books (1:N)
- Book → Loans (1:N)
- Member → Loans (1:N)

---

## ⚙️ Backend (ASP.NET Core)

### Main Features

#### Books

- GET /books
- POST /books
- PUT /books/{id}
- DELETE /books/{id}

#### Authors

- Full CRUD

#### Members

- Full CRUD

#### Loans

- POST /loans → borrow book
- POST /loans/{id}/return → return book
- GET /loans/active

---

## 🧠 Business Logic

- Cannot borrow a book if no copies are available
- Borrowing decreases available copies
- Returning increases available copies
- Cannot return an already returned book

---

## 🖥️ Frontend Scope

- Book list
- Add/edit book form
- Member list
- Borrow/return actions

Focus on functionality over UI complexity.

---

## 🧪 Technical Features

### Must-have

- ASP.NET Core Web API
- Entity Framework Core
- Database (PostgreSQL or SQL Server)
- Swagger
- DTOs
- Validation
- Service layer
- Global exception handling

### Optional

- Pagination
- Search
- Docker
- JWT Authentication

---

## 🏗️ Architecture

Suggested layered structure:

- Libreroo.Api
- Libreroo.Application
- Libreroo.Domain
- Libreroo.Infrastructure

---

## 🧠 What This Project Demonstrates

- Backend architecture understanding
- REST API design skills
- Relational data modeling
- Business logic implementation
- Ability to work in .NET ecosystem

---

## ⚡ Development Plan

- Day 1–2: ASP.NET Core basics + project setup
- Day 3–4: CRUD + EF Core
- Day 5: Database + migrations
- Day 6: Business logic + validation
- Day 7: Testing + polishing

---

## 💬 Interview Positioning

"I built Libreroo to practice ASP.NET Core and demonstrate how I can transfer my Java backend experience into the .NET
ecosystem, focusing on clean architecture and business logic rather than just CRUD."

---

## 🚫 Scope Control

### Do

- Keep project small and focused
- Prioritize backend quality
- Finish MVP quickly

### Avoid

- Overengineering
- Complex frontend
- Unnecessary features (e.g. payments, advanced UI)

---

## 📌 Summary

Libreroo is a focused demo project designed to showcase backend skills, architectural thinking, and the ability to
quickly adapt from Java to .NET.

