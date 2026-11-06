# Salting & Slow Hashing — Defending Against Real Attacks

## Overview
A stolen password database enables **offline cracking**: attackers guess passwords, hash each guess, and compare it with stored hashes without facing login rate limits or alerts. Unique salts and deliberately expensive password-hashing algorithms make precomputed attacks ineffective and each guess slower and more costly.

## 1. Why Hashing Alone Is Not Enough
- Attackers do not need to reverse a hash; they **guess passwords**, hash the guesses, and compare results.
- A stolen database allows this offline, potentially at very high speed.
- Fast general-purpose hashes such as **MD5** and **SHA-256** are unsuitable for password storage because they make guessing cheap.

## 2. Salting
A **salt** is a unique, random value added to a password before hashing.

| Without a salt | With a unique salt |
|---|---|
| Same password → same hash | Same password → different hashes |
| Enables reuse of precomputed rainbow tables | Defeats precomputed rainbow-table attacks |
| Reveals when users share a password | Makes each password hash unique |

- Salts are **not secret**; they are stored alongside the password hash.
- Their value comes from being random and unique per password.
- Knowing a salt does not let an attacker reuse one precomputed result across users; guesses must be computed separately for each salt.

> **Mental model:** A unique salt is like a different recipe for each user. An attacker must repeat the work for every account.

## 3. Slow Password Hashing
Slow hashing deliberately consumes measurable time and/or computing resources, increasing the cost of every password guess.
- Fast hashing helps attackers test more guesses.
- Slow hashing reduces the number of guesses an attacker can perform.
- Lesson illustration: at 1 ms per hash, 1,000 guesses take about 1 second; at 500 ms each, they take about 500 seconds.

**Password-hashing algorithms:** **Argon2**, **bcrypt**, and **PBKDF2**. They support salts and adjustable work factors; Argon2 is highlighted as a modern recommended choice.

**Do not use:** MD5, SHA-1, or plain SHA-256 for password storage. **Base64 is encoding, not hashing.**

## 4. Secure Password Storage Flow
1. User submits a password.
2. Generate a random, unique salt.
3. Pass the password and salt to a slow password-hashing algorithm.
4. Store the resulting hash and salt (or the algorithm's encoded hash record) in the database.
5. Discard the original password.

During verification, use the password-hashing algorithm's verification process with the stored salt and parameters to check the submitted password.

## 5. Quick-Reference Cheat Sheet

| Mechanism | Role |
|---|---|
| **Hashing** | One-way transformation; does not make fast guessing expensive by itself. |
| **Salting** | Unique random value per password; defeats reusable precomputed tables and identical hashes. |
| **Slow hashing** | Raises the cost of each guess. |
| **Argon2 / bcrypt / PBKDF2** | Password-hashing algorithms designed for this purpose. |
| **MD5 / SHA-1 / plain SHA-256** | Too fast for password storage. |

**Common questions**
- **Is a stored salt a weakness?** No. A salt does not need to be secret.
- **Why not encrypt passwords?** Encryption is reversible; a leaked key could expose all stored passwords.

**Key Takeaway:** Password defense combines **hashing + a unique random salt + a deliberately expensive password-hashing algorithm**. Attackers rely on speed; salting prevents reusable precomputation, while slow hashing makes every guess costly.
