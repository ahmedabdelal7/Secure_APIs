# Authentication Fundamentals

## 1. Overview

Authentication answers **“Who are you?”** by verifying the identity of the caller making an API request. This lesson introduces the mental model for adding authentication to the Student API; it explains the purpose and flow but does not implement JWT code yet.

## 2. Detailed Core Concepts

### What Authentication Does

Authentication verifies identity. It does **not** decide:
- What the caller can do.
- Whether the caller is allowed to perform a specific action.

Those decisions belong to **authorization**, which comes later.

**Analogy — University entrance:** Gate security checks your ID to confirm who you are; it does not decide which offices you may enter.

### Why the Student API Needs Authentication

The existing endpoints include:
- `POST /api/Students`
- `PUT /api/Students/{id}`
- `DELETE /api/Students/{id}`

Without authentication, anyone can call these endpoints and the API trusts every request. Authentication lets the API identify the caller before authorization determines what that caller may do.

### Common API Authentication Approaches

APIs commonly use:
- **Username and password → token**
- **API keys**
- **OAuth** (e.g. Google or Microsoft)

This course uses **username + password → JWT token** because the lesson describes it as beginner-friendly, stateless, and widely used for REST APIs.

### Stateless Authentication

The Student API is stateless:
- The server does not remember users between requests.
- Each request must prove the caller's identity again.

JWT supports this model by carrying identity information in a token that the client sends with requests.

### Planned Authentication Flow

The API will add new authentication endpoints without changing the existing Student endpoints:
- `POST /api/Auth/Register`
- `POST /api/Auth/Login`

Flow:
1. The client submits login credentials.
2. The API verifies the credentials.
3. The API returns a token.
4. The client sends that token with subsequent requests.

### What Authentication Does Not Do

Authentication alone does not:
- Decide whether the caller is an admin.
- Decide which student the caller may access.
- Enforce business rules or permissions.

These responsibilities belong to **authorization**.

## 3. Quick-Reference Cheat Sheet

| Concept | Remember |
|---|---|
| Authentication | **Who are you?** — verifies identity |
| Authorization | **What are you allowed to do?** — decides permissions |
| Existing API issue | Student endpoints currently trust requests without identifying callers |
| Course approach | Username + password → JWT |
| Stateless API | Server does not remember users between requests |
| JWT role in this lesson | Carries identity information and is sent with requests |
| Register endpoint | `POST /api/Auth/Register` |
| Login endpoint | `POST /api/Auth/Login` |
| Login flow | Credentials → verification → token → token sent with requests |
| Authentication limitation | Does not grant permissions or enforce business rules |
