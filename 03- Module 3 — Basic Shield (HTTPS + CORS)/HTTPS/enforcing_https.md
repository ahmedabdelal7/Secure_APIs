# Enforcing HTTPS

## 1. Overview

This lesson applies HTTPS to the existing Student API without changing
endpoints, controllers, or adding authentication. The goal is to ensure
the API runs over HTTPS and that HTTP requests are redirected
appropriately.

## 2. Detailed Core Concepts

### Why HTTPS Comes Before JWT

Future authentication features will send login credentials, JWT access
tokens, and refresh tokens. Enforcing HTTPS first protects this
sensitive data in transit and helps prevent token theft and user
impersonation.

### HTTPS Redirection Middleware

Add or verify this line in `Program.cs`:

``` csharp
app.UseHttpsRedirection();
```

-   Redirects HTTP requests to HTTPS when HTTPS is available.
-   Place it before controller mapping.
-   **It does not create or enable HTTPS by itself.**

### Configure and Select an HTTPS Launch Profile

HTTPS must be configured, running, and selected as the startup profile.

Open `Properties/launchSettings.json` and inspect the `profiles`
section. It may contain `http`, `https`, and `IIS Express` profiles.

Example HTTPS profile:

``` json
"https": {
  "commandName": "Project",
  "launchBrowser": true,
  "launchUrl": "swagger",
  "applicationUrl": "https://localhost:7217;http://localhost:5215"
}
```

The HTTPS profile starts the application with HTTPS configured.

A typical HTTP-only profile may look like:

``` json
"http": {
  "applicationUrl": "http://localhost:5215"
}
```

Running this profile starts the API on HTTP only; HTTPS is not started
by that profile. This is a startup-profile choice, not necessarily a
security bug.

**In Visual Studio:** select `https` in the run-profile dropdown next to
the Run button, then start the project. Swagger should open at:

`https://localhost:7217/swagger`

### Trust the Development HTTPS Certificate

Development HTTPS requires a trusted certificate. Run these commands in
the Visual Studio terminal (or another command prompt):

``` bash
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```

These commands are generally needed once per machine, not once per
project. If HTTPS already works, there is no need to repeat them.

### Verify Development and Production Behavior

**Development (Visual Studio):** - Launch profiles are separate startup
configurations. - The `http` profile stays on HTTP; the `https` profile
runs HTTPS. - Redirection does not switch the application from one
launch profile to another. - Verify development HTTPS by running the
`https` profile and confirming that Swagger opens with an `https://`
URL.

**Production:** - The lesson describes a single hosting environment
where HTTP and HTTPS are handled together. - Verify that requests made
over HTTP are redirected to HTTPS.

### What This Security Step Changes

-   No authentication is added.
-   No endpoints or controllers are changed.
-   Security is added transparently at the transport layer.
-   HTTPS establishes the foundation for later authentication and
    token-based security.

## 3. Quick-Reference Cheat Sheet

  -----------------------------------------------------------------------
  Item                                Remember
  ----------------------------------- -----------------------------------
  Middleware                          `app.UseHttpsRedirection();`

  Middleware placement                Before controller mapping

  Middleware limitation               Redirects only when HTTPS is
                                      available; does not enable HTTPS

  HTTPS configuration                 `Properties/launchSettings.json`

  Visual Studio profile               Select `https`, not `http`

  Development URL                     `https://localhost:7217/swagger`
                                      (example)

  Development certificate             `dotnet dev-certs https --trust`

  Certificate setup                   Usually once per machine

  Development verification            Confirm the API and Swagger run on
                                      HTTPS

  Production verification             Confirm HTTP requests redirect to
                                      HTTPS

  Scope                               Transport security only; no
                                      authentication or endpoint changes
  -----------------------------------------------------------------------
