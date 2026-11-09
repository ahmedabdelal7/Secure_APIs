# Implementing Role-Based Authorization in Student API

## Overview
This lesson demonstrates the practical implementation of role-based authorization in ASP.NET Core by mapping controller actions to an explicit security boundary contract. It teaches developers how to lock down controllers by default, selectively open public endpoints, enforce administrative restrictions on sensitive operations, and identify endpoints requiring ownership-based policies.

## Detailed Core Concepts

### 1. Default-Deny Controller Architecture
Security best practice dictates centralizing access control at the highest possible scope rather than relying on per-action security.
* Applying `[Authorize]` at the controller level blocks unauthenticated (anonymous) requests to all actions by default.
* This defensive architecture eliminates accidental data exposure if a developer adds a new action without explicit authorization attributes.

```csharp
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    // All endpoints inherit [Authorize] unless explicitly overridden
}
```

### 2. Overriding Security for Public Endpoints
Aggregated metrics and non-sensitive data (such as passing lists or grade averages) do not disclose individual student identities and should be publicly consumable.
* When a controller is locked with `[Authorize]`, public actions must explicitly override it using `[AllowAnonymous]`.
* Being declarative ensures that future global authorization filters do not inadvertently break intended public functionality.

```csharp
[AllowAnonymous]
[HttpGet("Passed", Name = "GetPassedStudents")]
public ActionResult<IEnumerable<Student>> GetPassedStudents() { ... }

[AllowAnonymous]
[HttpGet("AverageGrade", Name = "GetAverageGrade")]
public ActionResult<double> GetAverageGrade() { ... }
```

### 3. Enforcing Administrative Endpoints
High-risk, mutating operations (data creation, modification, deletion) and bulk sensitive queries must be restricted to administrative roles.
* Declaring `[Authorize(Roles = "Admin")]` instructs ASP.NET Core middleware to automatically inspect the user's JWT claims.
* The application framework evaluates role membership before action execution; manual claims parsing inside controllers is unnecessary and discouraged.
* If a valid JWT contains the required role claim, execution proceeds; otherwise, the framework automatically terminates the pipeline with `403 Forbidden`.

```csharp
[Authorize(Roles = "Admin")]
[HttpGet("All", Name = "GetAllStudents")]
public ActionResult<IEnumerable<Student>> GetAllStudents() { ... }

[Authorize(Roles = "Admin")]
[HttpPost(Name = "AddStudent")]
public ActionResult<Student> AddStudent(Student newStudent) { ... }

[Authorize(Roles = "Admin")]
[HttpPut("{id}", Name = "UpdateStudent")]
public ActionResult<Student> UpdateStudent(int id, Student updatedStudent) { ... }

[Authorize(Roles = "Admin")]
[HttpDelete("{id}", Name = "DeleteStudent")]
public ActionResult DeleteStudent(int id) { ... }
```

### 4. Technical Diagnostics: 401 vs. 403
* **`401 Unauthorized`:** Authentication failure. The caller provided no JWT, an expired token, or a tampered signature. The API cannot establish caller identity.
* **`403 Forbidden`:** Authorization failure. The caller's identity is verified, but their role claims lack clearance for the target action. In API defense, a `403` indicates access controls are operating as designed.

### 5. The Boundary Limitation: Resource Ownership
Simple role checks cannot evaluate contextual relationships between the caller and the data.
* **The Problem:** In `GET /api/Students/{id}`, an `Admin` can view any student, but a `Student` should only access their own record.
* **Limitation:** `[Authorize(Roles = "Student")]` would allow any student to fetch any other student's profile via route parameter manipulation.
* **Resolution:** Securing this endpoint requires resource ownership validation via policy-based authorization (addressed in upcoming architecture tiers).

## Quick-Reference Cheat Sheet

### Endpoint Authorization Matrix
| HTTP Method & Route | Access Rule | Applied Attribute | Behavioral Outcome |
| :--- | :--- | :--- | :--- |
| `GET /api/Students/Passed` | Public | `[AllowAnonymous]` | Bypasses controller-level `[Authorize]`. |
| `GET /api/Students/AverageGrade` | Public | `[AllowAnonymous]` | Open to all callers without JWT verification. |
| `GET /api/Students/All` | Admin Only | `[Authorize(Roles = "Admin")]` | Returns `403` if JWT role $\neq$ `Admin`. |
| `POST /api/Students` | Admin Only | `[Authorize(Roles = "Admin")]` | Blocks non-admin creation requests. |
| `PUT /api/Students/{id}` | Admin Only | `[Authorize(Roles = "Admin")]` | Blocks non-admin modification requests. |
| `DELETE /api/Students/{id}` | Admin Only | `[Authorize(Roles = "Admin")]` | Prevents unauthorized record destruction. |
| `GET /api/Students/{id}` | Owner or Admin | *Deferred* | Requires policy-based ownership checking. |

### Framework Attributes & Status Codes
| Attribute / Code | Level | Responsibility | Trigger Condition |
| :--- | :--- | :--- | :--- |
| `[Authorize]` | Controller | Default lock | Missing or invalid JWT $\rightarrow$ `401 Unauthorized`. |
| `[AllowAnonymous]` | Action | Whitelist override | Permits unauthenticated requests through locked controller. |
| `[Authorize(Roles = "...")]` | Action | Privilege check | Valid JWT but missing target role $\rightarrow$ `403 Forbidden`. |
| **`401 Unauthorized`** | HTTP Status | Identity error | Token missing, invalid, or signature verification failed. |
| **`403 Forbidden`** | HTTP Status | Permission error | Identity verified, but caller lacks required role claims. |