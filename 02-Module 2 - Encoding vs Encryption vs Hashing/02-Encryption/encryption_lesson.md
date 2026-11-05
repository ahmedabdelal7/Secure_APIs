# Encryption — Protecting Data That Must Be Read Again

## Overview
Encryption protects confidential data by transforming readable information into ciphertext using a cryptographic key, allowing authorized parties to recover it later. Use it when data must remain private but still be retrievable; unlike passwords, it is reversible with the correct key.

## 1. Core Concept

- **Purpose:** Confidentiality for data that must be read again.
- **Process:** Plaintext + key + encryption algorithm → ciphertext; decryption with the correct key restores the plaintext.
- **Key dependency:** Without the required key, the encrypted data should be computationally infeasible to read; losing the key can make the data unrecoverable.
- **Mental model:** A locked safe—the algorithm is public, but the key must remain secret.

> **Security principle:** Security depends on the secrecy and management of the key, not on hiding the encryption algorithm.

## 2. Main Types of Encryption

| Type | How it works | Typical use |
|---|---|---|
| **Symmetric (AES)** | The same secret key encrypts and decrypts. Fast and suitable for large amounts of data. | Database fields and stored data |
| **Asymmetric (RSA)** | Uses a public/private key pair; in the basic model, the public key encrypts and the private key decrypts. | Establishing or exchanging keys |

**HTTPS/TLS uses both:** asymmetric cryptography helps authenticate the server and establish shared session keys; symmetric encryption then efficiently protects the connection data.

## 3. When to Use It — and When Not To

**Use encryption when the original value must be recovered:**
- Data in transit (HTTPS/TLS).
- Sensitive database fields.
- Configuration secrets that an application must retrieve.

**Do not use encryption for password storage.**
- Passwords should be stored as salted, slow password hashes, not recoverable ciphertext.
- Encryption is also not a substitute for authentication or authorization.

## 4. Key Management & Common Mistakes

- Never hard-code encryption keys in source code or commit them to repositories.
- Avoid weak or reused keys; use secure key generation and storage.
- Restrict access to keys and plan rotation, backup, and recovery.
- Remember: if an attacker obtains the key, encrypted data may be exposed.

**Strong encryption with poor key management is weak security.**

## 5. Quick-Reference Cheat Sheet

| Need | Use |
|---|---|
| Prepare data for a format or protocol | **Encoding** |
| Keep data confidential and recover it later | **Encryption** |
| Verify passwords without storing recoverable passwords | **Password hashing** |
| Protect data while it travels over a network | **HTTPS/TLS** |

**Key Takeaway:** Encoding prepares data, encryption keeps retrievable data confidential, and hashing is used for password verification. Encryption is one security layer—combine it with sound key management and access control.
