# Encoding — Why We Encode Data (and Why It Is Not Security)

## Overview
Encoding converts data from one representation to another so systems can store, transmit, and interpret it reliably. It is **not a security mechanism**: it uses no secret key and anyone can reverse it.

## 1. What Encoding Does

- **Purpose:** Format data for storage, transmission, or display.
- **Characteristics:** Reversible, requires no secret key, and does not hide meaning.
- **Examples:** Base64, URL encoding, and UTF-8.

Example: `Password123` → `UGFzc3dvcmQxMjM=` → anyone can decode it back.

> **Mental model:** Encoding is like packaging a TV for delivery: it helps it travel safely, but it does not lock the box or prevent someone from opening it.

## 2. Why We Use Encoding

| Problem | Encoding's role | Example |
|---|---|---|
| **Binary data in text-based formats** | Represents bytes as text that can be carried by text-based systems. | Base64 for binary data |
| **Special characters in URLs** | Escapes characters that have structural meaning so they are parsed as data. | `John & Admin` → `John%20%26%20Admin` |
| **Different character representations** | Provides consistent interpretation of text across systems. | UTF-8 supports Arabic, English, and emoji |
| **Strict protocol fields** | Formats values to fit protocol rules. | Base64 can represent `username:password` in HTTP Basic authentication |
| **Structured data such as JSON** | Proper formatting/escaping keeps data valid and parseable. | JSON string escaping for quotes and control characters |

**Important distinctions**
- Base64 is a binary-to-text representation, not encryption.
- URL encoding handles URL-reserved characters; it does not validate or secure input.
- UTF-8 is a character encoding that maps text to bytes; it is not a way to conceal text.
- HTTP Basic authentication uses Base64 formatting, so credentials still require HTTPS/TLS in transit.

## 3. What Encoding Does NOT Do

Encoding:
- Does **not** protect or encrypt data.
- Does **not** provide confidentiality.
- Does **not** hide passwords, secrets, or sensitive information.
- Can be reversed by anyone who has the encoded value.

**Never use Base64 or another encoding to protect passwords or confidential data.**

| Need | Correct mechanism |
|---|---|
| Keep data confidential and recover it later | **Encryption** |
| Store password verifiers without storing recoverable passwords | **Password hashing** using a password-specific hash (with salt and suitable work factor) |
| Make data compatible with a protocol or format | **Encoding** |

## 4. Quick-Reference Cheat Sheet

| Term | Remember |
|---|---|
| **Encoding** | Changes representation for compatibility and transport. |
| **Reversible** | Original data can be recovered without a secret key. |
| **Base64** | Converts bytes to text; anyone can decode it. |
| **URL Encoding** | Escapes reserved/special URL characters. |
| **UTF-8** | Encodes Unicode text as bytes across languages and platforms. |
| **Encryption** | Protects confidentiality; requires a key. |
| **Hashing** | One-way transformation; used for password verification. |

**Big Picture:** Encoding supports reliable data handling and may be used before encryption or hashing when a format requires it, but it never replaces those security mechanisms.

**Key Takeaway:** We encode data because raw bytes or special characters may not fit a protocol's rules; encoding makes data compatible for transport—not secret or secure.
