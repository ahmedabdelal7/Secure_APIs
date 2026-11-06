# HTTPS for Beginners — What It Is, Why It Matters, and How It Works

## Overview
HTTPS is **HTTP running over TLS**, protecting information while it travels between a client and a server. It provides confidentiality, integrity, and server authentication, making it the foundation on which API authentication and authorization depend.

## 1. What HTTPS Protects

Plain HTTP sends data without transport encryption, so attackers on the network (for example, on public Wi-Fi, through compromised routers, or via untrusted intermediaries) may read or modify it.

HTTPS protects data **in transit**, not data stored on either endpoint:

| Protection | What it means |
|---|---|
| **Confidentiality** | Traffic is encrypted so unauthorized parties cannot read its contents. |
| **Integrity** | Tampering during transmission is detected. |
| **Server authentication** | The client verifies it is communicating with the server identified by the certificate, helping prevent fake-server / MITM attacks. |

## 2. TLS Certificates & Trust

A TLS certificate is a server's digital identity. It binds a domain name to a public key and is digitally signed by a trusted **Certificate Authority (CA)**.

A certificate includes:
- Domain name
- Public key
- Issuer (CA)
- Validity period
- Digital signature

The server's **private key must remain on the server**.

Examples of CAs from the lesson: **Let's Encrypt** (free), DigiCert, GlobalSign, and Sectigo. Browsers and operating systems contain trusted CA roots, allowing them to validate certificate chains. For most APIs, a free certificate is sufficient.

## 3. Development vs. Production Certificates

| Environment | Certificate approach |
|---|---|
| **Development** | Local self-signed development certificate, trusted on the developer's machine. |
| **Production** | Certificate issued by a publicly trusted CA for the real domain; available through hosting providers, cloud platforms, or CAs. |

For .NET local development, create/trust the development certificate with:

```bash
dotnet dev-certs https --trust
```

A browser warning such as **“Your connection is not private”** can indicate a missing, expired, untrusted, or domain-mismatched certificate. This is a critical production issue; locally, the development certificate may simply need to be trusted.

## 4. How an HTTPS Connection Works

1. **Client connects** to the server.
2. **Server presents its certificate.**
3. **Client validates** the certificate: trusted CA chain, validity period, matching domain, and valid signature.
4. **TLS negotiates shared session keys** using secure cryptographic methods; the shared secret is not sent directly over the network.
5. **Encrypted communication begins:** HTTP requests, responses, headers, and data are protected using the negotiated session keys.

If certificate validation fails, the client blocks the connection or displays a warning.

## 5. What HTTPS Does NOT Do

HTTPS does **not**:
- Authenticate the application's users.
- Decide which actions a user may perform.
- Prevent abuse or business-logic attacks.

HTTPS secures the communication channel; application-level security is still required.

## 6. Quick-Reference Cheat Sheet

| Concept | Remember |
|---|---|
| **HTTPS** | HTTP over TLS. |
| **TLS** | Protects traffic in transit. |
| **Certificate** | Binds a domain identity to a public key. |
| **CA** | Trusted organization that issues/signs certificates. |
| **Private key** | Must remain secret on the server. |
| **Development certificate** | Local certificate trusted on the development machine. |
| **Production certificate** | Publicly trusted certificate for the real domain. |
| **HTTPS limitation** | Does not replace user authentication or authorization. |

**Security stack:**  
**HTTPS** → protects transport · **Authentication** → proves user identity · **Authorization** → controls what the user can access or do.

**Key Takeaway:** Without HTTPS, credentials, tokens, and sessions can be exposed in transit; HTTPS is the security foundation, not the complete security solution.
