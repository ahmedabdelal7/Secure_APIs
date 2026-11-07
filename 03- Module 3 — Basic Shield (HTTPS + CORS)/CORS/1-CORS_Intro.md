# CORS for Beginners

## 1. Overview

CORS (Cross-Origin Resource Sharing) is a browser-enforced rule that controls whether JavaScript from one origin can access a response from another origin. It differs from HTTPS: HTTPS encrypts data in transit, while CORS governs browser-based cross-origin access.

## 2. Detailed Core Concepts

### What Is an Origin?

An **origin** consists of three parts:
- **Protocol:** `http` or `https`
- **Domain:** e.g. `example.com`
- **Port:** e.g. `3000` or `8080`

If any part changes, the origin is different.

Examples: `https://localhost:3000`, `https://studentportal.com`, `https://studentportal.com:8080`.

### Why CORS Exists

CORS helps protect users when browser-based JavaScript on one website tries to access resources from another origin.

Example:
1. A user is logged in to `bank.com`.
2. The user visits an untrusted website.
3. That website attempts to call `https://bank.com/api/account`.
4. The browser checks whether the calling origin is allowed to access the response.

If the origin is not allowed, the browser blocks JavaScript from reading the response. The API may still receive and process the request; CORS does not necessarily prevent the request from reaching the server.

### Who Enforces CORS?

- **API/server:** Sends CORS response headers declaring allowed origins, methods, and headers.
- **Browser:** Checks those headers and allows or blocks JavaScript access to the response.

CORS is not a general network firewall.

### When CORS Applies

CORS is relevant when all three conditions are true:
1. The request comes from a browser.
2. JavaScript makes the request.
3. The target has a different origin.

**Typical browser clients:** React, Angular, and Vue applications.

**Not governed by browser CORS enforcement:** Postman, `curl`, mobile applications, and backend services.

`curl` sends HTTP requests directly to an API without a browser JavaScript environment, so it does not apply browser CORS rules.

### Same-Origin vs. Cross-Origin

| Scenario | Example | CORS |
|---|---|---|
| Same-origin | Frontend and API both use `https://app.com` | Cross-origin CORS check is not needed |
| Cross-origin | Frontend uses `https://app.com`; API uses `https://api.com` | Browser checks the API's CORS response |

### High-Level CORS Flow

1. Browser sends a cross-origin request.
2. API responds with CORS headers.
3. Browser checks the response against the origin and requested access.
4. Browser allows JavaScript to access the response or blocks that access.

**Key distinction:** The server declares the policy; the browser enforces it.

### Common CORS Headers

| Header | Purpose |
|---|---|
| `Access-Control-Allow-Origin` | Indicates which origin may access the response |
| `Access-Control-Allow-Methods` | Indicates which HTTP methods are allowed |
| `Access-Control-Allow-Headers` | Indicates which request headers are allowed |

### What CORS Does Not Do

CORS is **not**:
- Authentication
- Authorization
- Rate limiting
- General attack prevention
- A replacement for API security

It does not stop requests from Postman, `curl`, mobile apps, or backend services. Therefore, a successful Postman request does not mean CORS is broken.

### Why Backend Developers Must Understand CORS

- Frontend applications depend on correct CORS configuration.
- Incorrect configuration can prevent a frontend from accessing the API.
- Overly permissive configuration can expose browser users to unwanted cross-origin access.
- Treating CORS as complete API security creates a false sense of protection.

## 3. Quick-Reference Cheat Sheet

| Concept | Remember |
|---|---|
| CORS | Cross-Origin Resource Sharing |
| Main purpose | Controls browser JavaScript access to cross-origin responses |
| Origin | Protocol + domain + port |
| Different origin | Any one of those parts changes |
| Enforcement | Browser |
| Policy declaration | API response headers |
| CORS applies | Browser + JavaScript + different origin |
| CORS does not block | Postman, `curl`, mobile apps, backend services |
| HTTPS | Encrypts data in transit |
| CORS | Governs browser-based cross-origin access |
| Not a substitute for | Authentication, authorization, rate limiting, or API security |
