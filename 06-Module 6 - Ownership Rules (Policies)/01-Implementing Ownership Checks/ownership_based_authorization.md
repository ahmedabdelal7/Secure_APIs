# Ownership-Based Authorization in Student API

## Overview
This lesson addresses horizontal privilege escalation by introducing runtime ownership checks to guarantee that authenticated users only access resources they directly own. It teaches developers how to extract cryptographic claims from the JWT, evaluate dynamic data boundaries within controller actions, and preserve administrative overrides while issuing correct HTTP status responses.

## Detailed Core Concepts

### 1. The Architectural Gap: Roles vs. Ownership
While role-based access control (RBAC) establishes broad capabilities, it fails to evaluate contextual data boundaries:
* **Roles (Static & Global):** Answer *"What can this user do in general?"* (e.g., an Admin manages resources; a Student queries resources).
* **Ownership (Dynamic & Contextual):** Answers *"Does this specific record belong to this caller?"*
* **The Vulnerability:** Applying `[Authorize(Roles = "Student")]` allows any authenticated student to supply arbitrary route parameters (`GET /api/Students/1`, `/2`, `/999`) and retrieve peers' private data.

### 2. Threat Profile: Horizontal Privilege Escalation
* **Definition:** An exploit where an attacker accesses unauthorized data belonging to peers at the same privilege tier without escalating to an administrative role.
* **Mechanism:** An attacker logs in as Student A (ID: 3) and modifies the client request URI to target Student B (ID: 4).
* **Impact:** Systematic data harvesting, privacy violations, and record enumeration. This remains one of the most widespread API vulnerabilities when applications rely solely on identity tokens.

### 3. Static Attributes vs. Dynamic Runtime Evaluation
Role checks are declarative and static; they evaluate token claims before reaching action logic. Ownership validation is inherently dynamic:
* It depends on contextual request variables (route parameters, query strings).
* It requires comparisons against database records or runtime state.
* Because attributes cannot evaluate dynamic runtime data, ownership logic must execute within the action body (or via resource-based authorization policies).

### 4. Identity Extraction & Claims Resolution
Once JWT authentication middleware validates an incoming token, claims are exposed via the controller's `User` property (`ClaimsPrincipal`):
* **`ClaimTypes.NameIdentifier`:** Stores the unique identity (Student ID) embedded during token issuance. Because the token signature is verified, this value is tamper-proof and trusted.
* **Extraction:** `var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);`
* **Role Check:** `bool isAdmin = User.IsInRole("Admin");` or `User.FindFirstValue(ClaimTypes.Role) == "Admin";`

### 5. Implementation Pattern: `GET /api/Students/{id}`
The endpoint must enforce: **Admins access any record; students access only their own record.**

```csharp
[HttpGet("{id}", Name = "GetStudentById")]
public ActionResult<Student> GetStudentById(int id)
{
    // 1. Validate route input
    if (id < 1)
        return BadRequest("Invalid student id.");

    // 2. Fetch target resource
    var student = StudentDataSimulation.StudentsList
        .FirstOrDefault(s => s.Id == id);

    // 3. Prevent information disclosure on non-existent records
    if (student == null)
        return NotFound("Student not found.");

    // 4. Extract authenticated claims from validated JWT
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    var userRole = User.FindFirstValue(ClaimTypes.Role);
    int authenticatedStudentId = int.Parse(userId);
    bool isAdmin = userRole == "Admin";

    // 5. Enforce ownership boundary
    if (!isAdmin && authenticatedStudentId != id)
        return Forbid(); // HTTP 403

    // 6. Return authorized resource
    return Ok(student);
}
```

### 6. Diagnostic Accuracy: `401 Unauthorized` vs. `403 Forbidden`
Returning the proper HTTP status code is essential for client handling and security audits:
* **`401 Unauthorized` (Authentication Failure):** Returned when no valid JWT is supplied. The caller's identity is unverified.
* **`403 Forbidden` (Authorization Failure via `Forbid()`):** Returned when the JWT is valid and the user's identity is verified, but access to the requested entity is refused. Ownership violations must always return `403`.

### 7. Security Roadmap Positioning
1. **Secure Transport:** HTTPS + CORS
2. **Authentication:** JWT (Identity verification)
3. **Authorization:** Roles (Coarse-grained permissions)
4. **Ownership-Based Authorization:** Runtime data boundaries (*Current Layer*)
5. **Policy-Based Rules:** Decoupled resource-based authorization handlers
6. **Production Hardening:** Audit logging, rate limiting, and tampering protection

## Quick-Reference Cheat Sheet

### Role-Based vs. Ownership-Based Authorization

| Dimension | Role-Based Authorization | Ownership-Based Authorization |
| :--- | :--- | :--- |
| **Core Question** | *"What operations can you perform?"* | *"Is this specific data yours?"* |
| **Evaluation Timing** | Static (Pipeline / Pre-action) | Dynamic (Action execution / Runtime) |
| **Data Dependencies** | JWT claims only | JWT claims + Route/Body + Data Store |
| **Implementation** | Declarative (`[Authorize(Roles = "...")]`) | Imperative code or Resource Handlers |
| **Threat Mitigated** | Vertical Privilege Escalation | Horizontal Privilege Escalation |

### Pipeline Decision & Status Code Matrix

| Execution Step | Condition / Check | Action / Result | HTTP Status Code |
| :--- | :--- | :--- | :--- |
| **Input Validation** | Route parameter `id < 1` | Immediate termination | `400 Bad Request` |
| **Resource Retrieval** | Target record does not exist | Terminate execution | `404 Not Found` |
| **Identity Verification** | Missing / Invalid JWT | Terminated by Middleware | `401 Unauthorized` |
| **Ownership Evaluation** | `!isAdmin && authId != routeId` | Access denied via `Forbid()` | `403 Forbidden` |
| **Authorized Access** | `isAdmin \|\| authId == routeId` | Return data payload | `200 OK` |

### Key ASP.NET Core Claims APIs

| Method / Property | Purpose | Return Type |
| :--- | :--- | :--- |
| `User.FindFirstValue(ClaimTypes.NameIdentifier)` | Extracts authenticated user ID from token | `string?` |
| `User.FindFirstValue(ClaimTypes.Role)` | Extracts user role from token | `string?` |
| `User.IsInRole("Admin")` | Evaluates if user belongs to the specified role | `bool` |
| `Forbid()` | Halts pipeline and returns authorization failure | `ForbidResult` (`403`) |