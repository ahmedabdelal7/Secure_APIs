# JWT Structure: Header, Payload & Signature

## 1. Overview

A JWT consists of three parts—Header, Payload, and Signature—joined by dots, and each part has a specific responsibility. The Header describes the token type and signing algorithm, the Payload carries user claims, and the Signature helps the API verify the token's integrity and trusted origin.

## 2. Detailed Core Concepts

### JWT Format

A JWT has this format:

```text
Header.Payload.Signature
```

The three parts are separated by dots.

### 1. Header — How the Token Is Built

The **Header** describes how the token is created. It usually contains:
- The token type (`JWT`).
- The signing algorithm (e.g. `HS256`).

Example:

```json
{
  "typ": "JWT",
  "alg": "HS256"
}
```

Meaning:
- `typ`: This is a JWT.
- `alg`: This is the algorithm used to sign it.

The Header does **not** contain user data.

### 2. Payload — Who the Student Is

The **Payload** contains **claims**—pieces of information about the user.

Typical claims in the Student API:
- Student ID.
- Role (Student / Admin).
- Token expiration time.

Example:

```json
{
  "studentId": 5,
  "role": "Admin",
  "exp": 1700000000
}
```

The Payload represents the identity information carried by the token.

**Important: The Payload is encoded, not encrypted.**
- Anyone who obtains the token can read its payload.
- Do not store passwords or sensitive personal data in it.
- The signature helps detect modifications; it does not make the payload secret.

### 3. Signature — Why the Token Can Be Trusted

The **Signature** helps prove that:
- The token was created by the trusted API/issuer.
- The token contents have not been modified.

As described in the lesson, it is created using:
- The Header.
- The Payload.
- A secret key known only to the server.

If the token is modified, signature validation fails and the API rejects it.

**Why it matters:** Without a valid signature, someone could attempt to change a claim such as `role` from `Student` to `Admin` or impersonate another student. The signature protects against this kind of token forgery.

### JWT Trust Flow

When a request arrives with a JWT:

1. API reads the token.
2. API checks the signature.
3. API checks expiration.
4. API extracts the claims.
5. API trusts the identity only if validation succeeds.

If any validation step fails, the request is rejected.

## 3. Quick-Reference Cheat Sheet

| Part | Purpose | Typical contents |
|---|---|---|
| Header | Describes how the token is built | Token type, signing algorithm |
| Payload | Carries user claims | Student ID, role, expiration |
| Signature | Allows integrity and issuer validation | Computed using header, payload, and secret key |

| Key rule | Remember |
|---|---|
| JWT format | `Header.Payload.Signature` |
| Header | No user data |
| Payload | Encoded, **not encrypted** |
| Sensitive data | Never put passwords or sensitive personal data in the payload |
| Signature | Detects token modification and helps prevent forgery |
| Validation | Read → check signature → check expiration → extract claims |
| Failed validation | Reject the request |
