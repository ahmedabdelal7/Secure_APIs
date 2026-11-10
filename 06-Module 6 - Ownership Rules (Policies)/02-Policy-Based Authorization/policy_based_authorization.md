# Policy-Based Authorization & Advanced Rules

## Overview
Policy-based authorization in ASP.NET Core centralizes dynamic ownership logic into decoupled, reusable components, replacing brittle and repetitive inline controller checks. This lesson teaches how to define custom requirements and resource-based handlers, wire them into the dependency injection container, and invoke runtime evaluations via `IAuthorizationService`.

## Detailed Core Concepts

### 1. The Architectural Problem: Inline Logic vs. Centralized Policies
Hardcoding claims extraction and identity comparisons directly inside controller actions works for prototypes but degrades rapidly in production:
* **Maintenance & Duplication:** The same evaluation logic must be rewritten across every endpoint handling protected resources.
* **Inconsistency & Auditability:** Verifying whether every endpoint enforces identical rules requires scanning every controller line by line, making security audits error-prone.
* **Separation of Concerns:** Controllers should direct application flow, not contain low-level authorization rules. Policies encapsulate the rules so that changing business logic (e.g., adding a Guardian or Manager role) only requires updating a single handler.

### 2. Core Components of Resource-Based Policies
ASP.NET Core breaks custom authorization into three decoupled responsibilities:
1. **Requirement (`IAuthorizationRequirement`):** A marker class representing the rule's contract. It contains no logic.
2. **Handler (`AuthorizationHandler<TRequirement, TResource>`):** The engine executing the evaluation logic against the current `ClaimsPrincipal` and the supplied runtime resource.
3. **Policy Registration:** The configured rule named inside `Program.cs` that links requirements to the authorization middleware pipeline.

### 3. Implementation Workflow

#### Project Organization
Authorization code belongs in security infrastructure, isolated from HTTP controllers and domain entities:
```text
StudentApi/
├── Authorization/
│   ├── StudentOwnerOrAdminRequirement.cs
│   └── StudentOwnerOrAdminHandler.cs
├── Controllers/
│   └── StudentsController.cs
└── Program.cs
```

#### Step 1: Define the Requirement
The requirement marks the intent (*"Owner or Admin access"*).
```csharp
// Authorization/StudentOwnerOrAdminRequirement.cs
using Microsoft.AspNetCore.Authorization;

public class StudentOwnerOrAdminRequirement : IAuthorizationRequirement
{
}
```

#### Step 2: Implement the Resource-Based Handler
The handler evaluates two parameters: the user context and the target resource (in this case, an `int` representing `studentId`).
```csharp
// Authorization/StudentOwnerOrAdminHandler.cs
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

public class StudentOwnerOrAdminHandler 
    : AuthorizationHandler<StudentOwnerOrAdminRequirement, int>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        StudentOwnerOrAdminRequirement requirement,
        int studentId)
    {
        // 1. Unconditional bypass for administrative roles
        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // 2. Ownership comparison
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (int.TryParse(userId, out int authenticatedStudentId) && 
            authenticatedStudentId == studentId)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
```

#### Step 3 & 4: Service Registration (`Program.cs`)
Register the handler as a dependency injection singleton and declare the named policy:
```csharp
// Register the evaluation handler
builder.Services.AddSingleton<IAuthorizationHandler, StudentOwnerOrAdminHandler>();

// Register the named policy with its requirement
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("StudentOwnerOrAdmin", policy =>
        policy.Requirements.Add(new StudentOwnerOrAdminRequirement()));
});
```

#### Step 5: Invoke via `IAuthorizationService`
Because route parameters and database resources are dynamic, declarative attributes (e.g., `[Authorize(Policy = "...")]`) cannot inspect them. Instead, inject `IAuthorizationService` to evaluate the resource imperatively at runtime.
```csharp
[HttpGet("{id}", Name = "GetStudentById")]
public async Task<ActionResult<Student>> GetStudentById(
    int id,
    [FromServices] IAuthorizationService authorizationService)
{
    if (id < 1)
        return BadRequest("Invalid student id.");

    var student = StudentDataSimulation.StudentsList
        .FirstOrDefault(s => s.Id == id);

    if (student == null)
        return NotFound("Student not found.");

    // Evaluate policy against the runtime resource ID
    var authResult = await authorizationService.AuthorizeAsync(
        User, 
        id, 
        "StudentOwnerOrAdmin");

    if (!authResult.Succeeded)
        return Forbid(); // HTTP 403

    return Ok(student);
}
```

### 4. Why Not Use Ad-Hoc Helper Methods?
A standard alternative considered by junior engineers is writing a static helper method (e.g., `SecurityHelper.CheckOwnership(User, id)`). While functional, this creates distinct architectural drawbacks:
* **Visibility:** Helper methods exist outside ASP.NET Core’s authorization infrastructure, making them invisible to diagnostic tools, security filters, and unit testing mocks designed for `IAuthorizationService`.
* **Composability:** Custom handlers can be chained, stacked with fallback handlers, or combined with multiple requirements in a single policy definition. Helper methods require manual chaining and nested conditional logic.
* **Predictability:** Policies integrate directly into framework diagnostics and auditing pipelines, ensuring that authorization failures fail safely and consistently.