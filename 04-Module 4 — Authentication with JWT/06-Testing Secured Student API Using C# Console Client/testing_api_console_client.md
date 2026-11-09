# Testing a Secured Student API Using a C# Console Client

## 1. Overview
A .NET Console application can act as a real API client by logging in, receiving a JWT, and calling protected Student endpoints. This lesson demonstrates end-to-end Bearer authentication from the client side without relying on Swagger or a browser.

## 2. Detailed Core Concepts

### Why use a Console client?
Swagger simplifies testing, while a Console client exposes the actual HTTP workflow:
- Create and send HTTP requests explicitly.
- Receive and read the JWT login response.
- Add the token to the `Authorization` header.
- Compare protected endpoint behavior with and without a token.

The same client-server pattern is used by desktop apps, mobile apps, and background services.

### Client workflow

1. **Create `HttpClient`:** Configure its base address to the API's HTTPS URL.
2. **Log in:** Send `POST /api/Auth/login` with an email and password using `PostAsJsonAsync`.
3. **Read the token:** Deserialize the response into a `TokenResponse` DTO.
4. **Call without a token:** Send `GET /api/Students/All`; expect `401 Unauthorized`.
5. **Call with a token:** Add `Authorization: Bearer <token>` and send the same request; expect success (typically `200 OK`).

### Sending the JWT

```csharp
using var request = new HttpRequestMessage(
    HttpMethod.Get, "api/Students/All");

request.Headers.Authorization =
    new AuthenticationHeaderValue("Bearer", token);

var response = await http.SendAsync(request);
```

`AuthenticationHeaderValue` builds the header in the required format:

```http
Authorization: Bearer <JWT>
```

### Responsibilities: client vs. API

| Console client | API |
|---|---|
| Sends login credentials over HTTPS | Verifies the credentials |
| Receives and stores the token for later requests | Issues and signs the JWT |
| Attaches the token to protected requests | Validates signature, issuer, audience, and expiration |
| Handles HTTP status codes and displays results | Extracts identity claims and enforces authentication |

The client does **not** need to decode the JWT, inspect its claims, or validate its signature. The API is responsible for security decisions.

### Understanding the expected responses

| Request | Expected result | Reason |
|---|---|---|
| Protected endpoint, no token | `401 Unauthorized` | No authenticated identity was supplied |
| Protected endpoint, valid token | `200 OK` (if the endpoint succeeds) | The API validates the token and authenticates the caller |
| Login with invalid credentials | `401 Unauthorized` | Credentials were not accepted |

A successful login proves that the credentials were accepted and a token was issued. The two protected-endpoint calls then demonstrate whether the API enforces JWT authentication correctly.

### Important HTTPS development-certificate warning

The sample uses `ServerCertificateCustomValidationCallback` and permits `SslPolicyErrors.RemoteCertificateChainErrors`. This relaxes certificate validation and is only a temporary local-development workaround; **never use this callback in production**. Prefer trusting the ASP.NET Core development certificate with `dotnet dev-certs https --trust`, then use normal certificate validation.

### Practical implementation notes
- Keep the API base URL aligned with the active HTTPS launch profile (for example, `https://localhost:7217/`).
- Use the exact login route and protected endpoint route defined by your API.
- Keep credentials and tokens out of source control and avoid printing complete tokens. The lesson's sample prints only a short token prefix; even that is unnecessary in real applications.
- Dispose of `HttpClient` appropriately; the sample creates one client for the short-lived console run.
- Handle unsuccessful responses before deserializing their bodies, and account for a possibly missing or empty response payload.

## 3. Quick-Reference Cheat Sheet

| Item | Meaning |
|---|---|
| `HttpClient` | Sends HTTP requests to the API |
| `PostAsJsonAsync()` | Sends the login request as JSON |
| `ReadFromJsonAsync<T>()` | Deserializes a JSON response into a DTO |
| `HttpRequestMessage` | Builds an individual request |
| `AuthenticationHeaderValue("Bearer", token)` | Adds the JWT Bearer header |
| `SendAsync()` | Sends the prepared request |
| `401 Unauthorized` | Request lacks valid authentication |
| `200 OK` | Request succeeded; endpoint authorization and business logic also permitted it |
| Client responsibility | Store/use token and send requests |
| API responsibility | Validate token and enforce security |

**Core rule:** Login obtains the JWT; the client sends it; the API validates and enforces it.
