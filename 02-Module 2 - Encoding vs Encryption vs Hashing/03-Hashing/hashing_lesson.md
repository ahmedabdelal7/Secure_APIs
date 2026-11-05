# Hashing — Protecting Secrets That Must Never Be Revealed

## Overview
Hashing transforms input data into a fixed-length fingerprint that is designed to be one-way, so the original value is not recovered from the hash. It is useful when data must be **verified, compared, or checked for changes** without revealing the original—especially for password storage.

## 1. Core Properties

- **One-way:** A hash is not decrypted or reversed to recover the input.
- **Deterministic:** The same input produces the same hash when the same algorithm and parameters are used.
- **No secret key:** Basic hashing does not require an encryption key.
- **Fixed-length output:** The digest has a defined length for a given algorithm.

Examples mentioned in the lesson: **SHA-256, bcrypt, Argon2**.

> **Mental model — Fingerprint:** A fingerprint can help identify and compare, but it cannot recreate the person. A hash works as a data fingerprint.

## 2. Password Storage & Verification

**Registration**
1. User submits a password.
2. The API hashes it.
3. The hash is stored in the database.
4. The original password is discarded.

**Login**
1. User submits the password again.
2. The system verifies the entered password against the stored password hash.
3. If verification succeeds, login succeeds; otherwise, it fails.

**Key idea:** The system does not retrieve or compare the original stored password—it verifies a submitted password using the stored hash.

## 3. Hashing vs. Encryption vs. Encoding

| Mechanism | Purpose | Can the original be recovered? | Typical use |
|---|---|---|---|
| **Encoding** | Change data representation for compatibility | Yes; no secret key needed | Base64, URL encoding, UTF-8 |
| **Encryption** | Keep data confidential while allowing retrieval | Yes, with the key | Sensitive data, tokens, files |
| **Hashing** | Verify, compare, or detect changes without revealing input | No practical reversal | Password verification, integrity checks |

- Use **hashing** when the original value must never be retrieved.
- Use **encryption** when the original data must be read again.
- Encoding is formatting, not protection.

## 4. Real-World Uses of Hashing

| Use case | What hashing helps do |
|---|---|
| **Password storage** | Verify credentials without storing recoverable passwords. |
| **File integrity** | Compare a downloaded file's hash with a published hash to detect changes. |
| **Request signatures** | Help verify request integrity and authenticity using a secret-based signature. |
| **Message checksums** | Detect accidental corruption during transfer. |
| **Deduplication** | Identify data that may already be stored. |
| **Cache keys** | Create short, consistent identifiers for long requests or queries. |
| **Change detection** | Detect changes in configuration or other data. |
| **Blockchain / ledgers** | Link blocks using previous-block hashes, making tampering detectable. |
| **Privacy-preserving comparisons** | Compare fingerprints of data without directly sharing the original values. |

## 5. Quick-Reference Cheat Sheet

**Hashing is useful when you need to:**
- **Identify** data.
- **Compare** values.
- **Verify** integrity or credentials.
- **Detect** changes.

**Important practical note:** For password storage, use a password-hashing algorithm such as **bcrypt or Argon2**, not a fast general-purpose hash such as plain SHA-256. Password-hashing systems use salts and dedicated verification procedures; follow the library's verify function rather than treating every password hash as a simple unsalted `Hash(input)` comparison.

**Key Takeaway:** Hashing is for verification—not recovery. If the original data must be read again, use encryption instead.
