# JWT Fundamentals: Concept, Structure, and Trust

## 1. Overview

A JWT (JSON Web Token) is a digital identity badge issued by an API after a successful login and sent by the client with subsequent requests. It supports stateless authentication by carrying claims in a token whose signature the API validates to verify the token has not been modified.

## 2. Detailed Core Concepts

### What Is a JWT?

**JWT** stands for **JSON Web Token**. Think of it as a digital ID card:
- The API issues it after login.
- The client carries it and sends it with requests.
- It identifies the caller and can expire.

**University analogy:** You show your ID at login, receive a badge, and present that badge at each door. The badge represents your JWT.

### Why APIs Use JWT

JWTs are commonly used because they are:
- **Stateless:** The server does not need to store a session for each user.
- **Self-contained:** Identity information is carried inside the token.
- **Fast:** The lesson highlights avoiding a database lookup for every request.
- **Widely supported:** Commonly used with REST APIs.

### What Information Does a JWT Carry?

A JWT payload can contain claims (pieces of information about the user). For the Student API, examples include:
- Student ID
- Role (Student / Admin)
- Token expiration time

A JWT should not contain:
- Passwords
- Sensitive personal data

### JWT Format: Three Parts

A JWT has three parts separated by dots:

```text
Header.Payload.Signature
```

Each part has a specific responsibility.

#### 1. Header — How the Token Is Built

The header describes the token type and signing algorithm.

Example:

```json
{
  "typ": "JWT",
  "alg": "HS256"
}
```

- `typ` identifies the token type as JWT.
- `alg` specifies the signing algorithm.
- The header does **not** contain user data.

#### 2. Payload — Who the Student Is

The payload contains **claims**, which are pieces of information about the user.

Example:

```json
{
  "studentId": 5,
  "role": "Admin",
  "exp": 1700000000
}
```

**Important:** The payload is encoded, not encrypted. Anyone who obtains the token can read its payload, but changing the contents causes signature validation to fail.

Therefore, never put passwords or sensitive personal data in the payload.

#### 3. Signature — Why the Token Can Be Trusted

The signature is created using the header, payload, and a secret key known only to the server (as described in this lesson). It allows the API to verify that the token was created by the trusted issuer and that its contents have not been modified.

If someone changes the payload—for example, changing the role from `Student` to `Admin`—the signature no longer matches, so validation fails and the API rejects the token.

### JWT Trust Flow

When a request arrives with a JWT, the API:

1. Reads the token.
2. Validates the signature.
3. Checks the expiration time.
4. Extracts the claims.
5. Trusts the identity only if validation succeeds.

If any validation step fails, the request is rejected.

### How JWT Is Used in the Student API

1. Student logs in.
2. API verifies the credentials and returns a JWT.
3. Client sends the JWT with subsequent requests.
4. API validates the token.
5. API identifies the caller and reads claims such as the student ID and role.

The token may accompany existing endpoints, for example:
- `POST /api/Students`
- `GET /api/Students/{id}`
- `DELETE /api/Students/{id}`

### What JWT Does Not Do

JWT alone does not:
- Decide permissions or what actions the caller may perform.
- Protect the API against every form of abuse.
- Replace HTTPS.

JWT helps establish **who the request is coming from**. Authorization determines what that identity is allowed to do.

## 3. Quick-Reference Cheat Sheet

| Concept | Remember |
|---|---|
| JWT | JSON Web Token; a digital identity badge |
| Issued | After successful login |
| Sent | With subsequent API requests |
| Main purpose | Stateless, token-based authentication |
| Structure | `Header.Payload.Signature` |
| Header | Token type and signing algorithm |
| Payload | Claims such as student ID, role, and expiration |
| Payload protection | Encoded, **not encrypted**; do not put secrets in it |
| Signature | Verifies token integrity and trusted issuance |
| Validation flow | Read → verify signature → check expiry → extract claims |
| Validation failure | Reject the token/request |
| Does not decide | Permissions or authorization |
| Does not replace | HTTPS |
