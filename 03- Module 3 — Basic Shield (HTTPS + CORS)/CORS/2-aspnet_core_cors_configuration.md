# ASP.NET Core CORS Configuration

## 1. Overview

ASP.NET Core CORS configuration defines which browser origins may access the API and applies those rules through middleware. Correct policy definition, middleware order, and environment-specific origins help frontend applications work without unnecessarily exposing the API.

## 2. Detailed Core Concepts

### The Two Parts of CORS Configuration

CORS requires both:
1. **Define a policy** in services.
2. **Apply the policy** in the request pipeline.

If either part is missing, CORS will not work as intended.

### Step 1: Define a Named CORS Policy

In `Program.cs`, register the policy **before** `builder.Build()`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("StudentApiCorsPolicy", policy =>
    {
        policy
            .WithOrigins(
                "https://localhost:7217",
                "http://localhost:5215"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
```

- `AddCors()` registers CORS services.
- `AddPolicy()` creates a reusable named policy.
- `WithOrigins()` allows only the listed origins.
- `AllowAnyHeader()` permits any request header.
- `AllowAnyMethod()` permits any HTTP method.

### Step 2: Apply CORS Middleware in the Correct Order

The lesson's pipeline order is:

```csharp
app.UseHttpsRedirection();

app.UseCors("StudentApiCorsPolicy");

app.UseAuthorization();

app.MapControllers();
```

**Important:** CORS middleware must run before endpoint execution. Do not place `UseCors()` after `MapControllers()`.

### Step 3: Understand Preflight Requests (OPTIONS)

For certain cross-origin requests, the browser sends a preflight `OPTIONS` request before the actual request. Examples include:
- Methods such as `PUT`, `DELETE`, and `PATCH`.
- Custom request headers, such as `Authorization`.
- JSON requests.

Example:

```http
OPTIONS /api/Students/5
```

The browser uses the preflight to ask whether the intended cross-origin request is permitted. ASP.NET Core handles preflight when CORS is configured correctly. If the preflight check fails, the browser does not send the corresponding actual request.

**Mental model:**
- Preflight (`OPTIONS`) = “Is this request allowed?”
- Actual request = sent only after a successful preflight when one is required.

### Step 4: Configure Origins for Each Environment

**Development — local frontend origins:**

```text
https://localhost:7217
http://localhost:5215
```

**Production — real frontend origins:**

```text
https://studentportal.com
https://admin.studentportal.com
```

Be explicit about trusted origins. The lesson advises against wildcard origins in production.

### Common CORS Configuration Mistakes

| Mistake | Why it is a problem |
|---|---|
| `AllowAnyOrigin()` with `AllowCredentials()` | Invalid combination; credentialed CORS requires explicit origins |
| `AllowAnyOrigin()` in production | Allows any website origin to read eligible API responses in a browser |
| Treating CORS as API security | CORS does not replace authentication or authorization and does not block `curl`, Postman, or mobile clients |
| Forgetting the port | Different ports are different origins; e.g. `localhost:3000` and `localhost:5173` |
| Applying CORS after `MapControllers()` | Middleware runs too late to apply the policy to mapped endpoints |

### Step 5: Test CORS Correctly

Do not use the browser address bar, Postman, or `curl` to verify browser CORS behavior.

Test from a **browser-based frontend**:
- Request from an allowed origin → browser allows access.
- Request from an untrusted origin → browser blocks JavaScript access.

Postman and `curl` working is expected because they do not enforce browser CORS rules.

### Scope of This Lesson

- No authentication or JWT is added.
- No endpoints are changed.
- Browser access is controlled through a CORS policy.
- CORS reduces which browser origins can access responses; it is not a replacement for API authentication and authorization.

## 3. Quick-Reference Cheat Sheet

| Item | Remember |
|---|---|
| Policy registration | `builder.Services.AddCors(...)` |
| Registration location | Before `builder.Build()` |
| Apply policy | `app.UseCors("StudentApiCorsPolicy");` |
| Middleware order | HTTPS redirection → CORS → authorization → controller mapping |
| Allowed origins | Exact scheme + host + port |
| Preflight method | `OPTIONS` |
| Preflight purpose | Check permission before certain actual requests |
| Development | Explicit localhost origins |
| Production | Explicit trusted production origins; avoid wildcards |
| Credentialed CORS | Do not combine `AllowAnyOrigin()` with `AllowCredentials()` |
| Correct test | Browser frontend from allowed and untrusted origins |
| Not a security replacement | Authentication and authorization are still required |
