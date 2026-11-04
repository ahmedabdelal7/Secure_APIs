# Defining Security Boundaries for Student Endpoints

## Overview
Security boundaries define **who may access each API endpoint and which actions they are allowed to perform**. They are designed before implementing authentication, JWT, and authorization so every endpoint has a clear access policy.

## 1. Endpoint Inventory

| Method | Endpoint | Purpose |
|---|---|---|
| GET | `/api/Students/All` | Retrieve all students |
| GET | `/api/Students/Passed` | Retrieve passed students |
| GET | `/api/Students/AverageGrade` | Retrieve average grade |
| GET | `/api/Students/{id}` | Retrieve one student |
| POST | `/api/Students` | Create a student |
| PUT | `/api/Students/{id}` | Update a student |
| DELETE | `/api/Students/{id}` | Delete a student |

## 2. Classify Endpoints by Risk

| Risk | Endpoints | Why it matters |
|---|---|---|
| **High** | POST, PUT, DELETE | Modify or destroy records; may corrupt student data or grades. |
| **Medium** | GET `/All`, GET `/{id}` | May expose identities, personal information, and grades. |
| **Low*** | GET `/Passed`, GET `/AverageGrade` | Aggregated results may be public if they reveal no sensitive information. |

> **Important:** Read-only endpoints are not automatically safe. Review the actual response data before making an endpoint public.

## 3. Define Access Policies

| Access Group | Allowed Actions | Rule |
|---|---|---|
| **Admin Only** | Create, update, and delete students | Requires an authenticated administrator. |
| **Authenticated Student** | View a student record | The requested student ID must match the authenticated user's identity. |
| **Public** | View passed-student results and average grade | No login required, provided the response contains no sensitive data. |

## 4. Quick-Reference Cheat Sheet

| Concept | Remember |
|---|---|
| **Security Boundary** | Who can access what? |
| **Authentication** | Who are you? |
| **Authorization** | What are you allowed to do? |
| **Ownership Rule** | A student can access only their own resource. |
| **Public Endpoint** | Accessible without login. |
| **Protected Endpoint** | Requires authentication and/or authorization. |

**Security Design Workflow:**  
List endpoints → Assess risk → Define access rules → Implement protection.

---

**Key Takeaway:** Design access rules first, then enforce them through authentication and authorization.
