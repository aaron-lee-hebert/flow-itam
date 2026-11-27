# Flow ITAM — GitHub Copilot Instructions

## Project overview

**Product name:** Flow ITAM

**Repository:** `flow-itam`

Build a small, dependable SaaS application for small and medium-sized
businesses that do not have a dedicated IT department.

**Working concept:** Asset accountability and basic cybersecurity
governance for non-technical business owners.

**Positioning:** "Know what you own. Know who is responsible. Know where
your risks are."

The product should help a business owner understand: - What equipment
the business owns - Who is responsible for each asset - Where assets are
located and their lifecycle status - Which basic security practices have
been reviewed or confirmed - Which exceptions, risks, and follow-up
actions need attention - What evidence and review history the business
can produce

This is an accountability, governance, and visibility product. It is
**not** an endpoint-management or security-enforcement platform.

## Target customer

Initially focus on businesses with roughly 10--100 employees that: -
Have no dedicated IT department - Track equipment in spreadsheets or
informal processes - Do not use Microsoft Intune, Jamf, or a comparable
MDM/endpoint-management platform - Need a practical way to establish
ownership, review basic security practices, and document exceptions -
May use Microsoft 365 or Google Workspace

Potential early customer segments include professional-services firms,
construction businesses, churches and nonprofits, small manufacturers,
and independent medical or dental practices. Do not hard-code
assumptions for any one industry.

## Product boundaries

### In scope

1.  **Asset accountability**
    -   Asset inventory and categories
    -   Asset identifiers, serial numbers, make/model, purchase date,
        warranty date, and lifecycle status
    -   Assignment to an employee and/or location
    -   Assignment and return history
    -   Asset notes and change history
    -   QR-code labels as a later enhancement
2.  **People and locations**
    -   Employee records (employees do not need application logins)
    -   Departments and locations
    -   Asset assignment and return workflows
3.  **Security hygiene and governance**
    -   A small library of plain-language control definitions
    -   Periodic control assessments
    -   Asset-level and organization-level assessments where appropriate
    -   Assessment statuses: `NotAssessed`, `Implemented`,
        `PartiallyImplemented`, `NotImplemented`, `NotApplicable`
    -   Evidence metadata and secure evidence-file references
    -   Risk findings, exceptions, owners, due dates, and remediation
        status
    -   Review history and audit events
4.  **Owner-focused reporting**
    -   A clear dashboard showing what needs attention
    -   Overdue reviews, missing confirmations, unsupported equipment
        (when entered or confirmed by a user), unassigned assets, and
        open findings
    -   Exportable asset accountability and security-governance
        summaries

### Explicitly out of scope

Do not build or imply that the product provides: - MDM, endpoint
management, EDR, antivirus, or remote device management - Automated
device discovery or network scanning - Vulnerability scanning or
penetration testing - Software deployment, patch deployment, or endpoint
enforcement - Active Directory, Entra ID, Intune, or Jamf
administration - Regulatory certification, compliance attestation, or a
guarantee of security - Enterprise IT asset-management replacement -
Microservices, Kubernetes, or a native mobile application in the MVP

Where a security condition is entered by a customer, label it as
**self-reported** or **user-confirmed**. Never present it as technically
verified unless the application actually obtains reliable verification
data.

Use NIST Cybersecurity Framework 2.0 as a conceptual reference for
organizing security practices, not as a certification claim. Keep
control language understandable to non-technical owners.

## Product principles

1.  **Clarity over complexity.** A business owner should understand a
    page without needing an IT background.
2.  **Actionable over decorative.** Explain what needs attention, why it
    matters, and what the next reasonable action is.
3.  **Honest security language.** Distinguish recorded, self-reported,
    reviewed, and technically verified information.
4.  **Secure by default.** Tenant isolation, authorization, safe file
    handling, and backups are foundational.
5.  **Small, shippable increments.** Prefer a modest working feature
    with tests over a broad unfinished abstraction.
6.  **Low operational burden.** Prefer a modular monolith and boring,
    well-supported infrastructure.
7.  **Customer trust.** Treat asset, employee, and security records as
    sensitive business information.

## Technology choices

Use this stack unless the project owner explicitly approves a change:

-   **Runtime/framework:** .NET 10 LTS
-   **Language:** C#
-   **Web:** ASP.NET Core MVC with Razor Views
-   **Database:** PostgreSQL
-   **ORM:** Entity Framework Core 10
-   **PostgreSQL provider:** Npgsql EF Core provider compatible with EF
    Core 10
-   **Authentication:** ASP.NET Core Identity
-   **UI:** Bootstrap and minimal vanilla JavaScript
-   **Hosting (initial):** Ubuntu LTS VPS
-   **Reverse proxy/TLS:** Nginx and a trusted TLS certificate provider
-   **Deployment:** Docker Compose initially; keep the application
    portable
-   **Source control/CI:** GitHub and GitHub Actions
-   **Evidence files:** Private S3-compatible object storage; store
    object keys and metadata in PostgreSQL
-   **Payments:** Defer Stripe integration until product validation
-   **Background work:** Use simple hosted services for basic scheduled
    work. Add a job framework only when actual requirements justify it.

Prefer a **modular monolith**. Do not introduce a SPA framework,
microservices, message broker, Kubernetes, or multiple databases without
a demonstrated need.

## Solution structure

A reasonable starting structure:

``` text
src/
  FlowItam.Web/
    Controllers/
    Views/
    ViewModels/
    Middleware/
    Authorization/
    wwwroot/
  FlowItam.Core/
    Organizations/
    Assets/
    People/
    Locations/
    Security/
    Governance/
    Risks/
    Common/
  FlowItam.Infrastructure/
    Identity/
    Persistence/
      Configurations/
      Migrations/
    Storage/
    Email/
    Services/

tests/
  FlowItam.UnitTests/
  FlowItam.IntegrationTests/

docs/
  PROJECT_SPECIFICATION.md
  ARCHITECTURE.md
  SECURITY.md
  OPERATIONS.md

docker/
.github/
  workflows/
```

This is a starting point, not a requirement to create every folder
immediately. Keep dependencies intentional: - `Core` should not depend
on `Web` or `Infrastructure`. - `Infrastructure` implements persistence
and external-service concerns. - `Web` coordinates requests,
authorization, and presentation. - Controllers should remain thin; put
business rules in application/domain services.

If an existing repository already has a sensible structure, inspect it
before proposing a restructure.

## Multi-tenancy and data isolation

The application is multi-tenant from the beginning.

-   An `Organization` is a tenant.
-   An authenticated application user is distinct from an `Employee`
    record. Employees do not need accounts.
-   Use `OrganizationMemberships` to associate users with organizations
    and roles.
-   A user may belong to more than one organization.
-   Use a shared PostgreSQL database and shared schema initially.
-   Every tenant-owned entity must include a non-null `OrganizationId`.
-   Resolve the active organization from the authenticated user's
    authorized memberships.
-   Never trust an `OrganizationId` supplied by a browser form, route,
    query string, or API request.
-   Scope every read, update, and delete to the active authorized
    organization.
-   On create, set the tenant ID server-side.
-   Validate that related records (for example, an asset assignment's
    employee and location) belong to the same organization.
-   Use composite foreign keys or equivalent database constraints where
    practical to prevent cross-tenant relationships.
-   Add integration tests that attempt cross-tenant reads, updates,
    deletes, and relationship creation.
-   Consider PostgreSQL row-level security later as defense in depth; it
    does not replace application authorization.

Tenant isolation is a release-blocking security requirement, not a
future enhancement.

## Initial domain model

Treat this as a starting model. Inspect the project specification and
existing code before changing it.

### Core entities

-   `Organization`
-   ASP.NET Core Identity user entities/tables
-   `OrganizationMembership`
-   `Employee`
-   `Department`
-   `Location`
-   `AssetCategory`
-   `Asset`
-   `AssetAssignment`
-   `AssetHistory`
-   `ControlDefinition`
-   `ControlAssessment`
-   `EvidenceRecord`
-   `RiskFinding`
-   `AuditEvent` (introduce when the audit requirements are defined)

Use UUID primary keys unless a specific integration or database concern
justifies another choice. Use UTC timestamps. Configure relationships
and indexes explicitly with EF Core configuration classes.

### Important modeling rules

-   Use the standard ASP.NET Core Identity schema and password-handling
    mechanisms. Never invent or store a custom plaintext/password-hash
    implementation.
-   `ControlDefinition` is global reference data unless there is a clear
    requirement for tenant customization.
-   `ControlAssessment` belongs to an organization and may optionally
    relate to an asset. Ensure any referenced asset belongs to that same
    organization.
-   `EvidenceRecord` must be tenant-scoped and linked to an appropriate
    assessment or finding. Store file metadata and a private
    object-storage key, not the file contents in the database.
-   Asset history should be append-oriented. Do not silently overwrite
    meaningful historical events.
-   Enforce at most one active assignment per asset. A PostgreSQL
    partial unique index on `(OrganizationId, AssetId)` where
    `ReturnedAt IS NULL` is one possible implementation.
-   Model asset lifecycle and assignment status explicitly; avoid
    relying on free-form strings for important state.
-   Define deletion behavior deliberately. Prefer archive/soft-delete
    for records whose history is important.

## Authorization roles

Start with a small organization-level role set:

-   `Owner`
-   `Administrator`
-   `Contributor`
-   `ReadOnly`

Define permissions centrally using ASP.NET Core authorization policies
or a similarly explicit approach. Do not scatter role-name string
comparisons throughout controllers and views.

At minimum: - Owners manage organization ownership and settings. -
Administrators manage members and business records. - Contributors
perform permitted day-to-day asset and assessment work. - Read-only
members can view permitted records and reports.

A user must never gain access to another tenant's data merely by
changing a URL or submitted identifier.

## Security and privacy requirements

Implement security deliberately and document the decisions.

-   Use ASP.NET Core Identity for authentication, password reset, and
    email confirmation.
-   Require authorization for every non-public application endpoint.
-   Apply tenant scoping server-side to every tenant-owned resource.
-   Use anti-forgery protection for state-changing MVC requests.
-   Validate all user input server-side.
-   Use secure, HTTP-only cookies and appropriate SameSite settings.
-   Enforce HTTPS in production.
-   Apply sensible authentication and request rate limits.
-   Store secrets outside source control; use environment-specific
    configuration or a secrets manager.
-   Never log passwords, tokens, secrets, or uploaded evidence contents.
-   Keep PostgreSQL inaccessible from the public internet.
-   Keep evidence storage private; use short-lived authorized download
    links or a controlled download endpoint.
-   Validate file size, extension, and content type. Do not trust the
    browser-provided content type alone.
-   Add audit events for important security- and accountability-related
    actions.
-   Keep dependencies patched and enable dependency/security alerts.
-   Back up PostgreSQL automatically, encrypt backups, store them
    off-site, define retention, and test restores.
-   Document incident response, data export, account closure, and
    data-retention expectations before commercial launch.

Do not claim the product is "secure," "compliant," or "certified" based
only on the presence of these controls. Describe implemented
capabilities precisely.

## User experience

Design for a non-technical owner or office manager.

The main navigation can start with:

-   Dashboard
-   Assets
    -   All Assets
    -   Assignments
    -   Categories
    -   Locations
-   People
    -   Employees
    -   Departments
-   Security & Governance
    -   Controls
    -   Assessments
    -   Evidence
    -   Risks & Exceptions
-   Reports
-   Organization
    -   Members
    -   Settings

Use plain-language labels and helpful empty states. Avoid security
jargon where ordinary language works. Every status should have a clear
meaning. Do not use color as the only way to communicate status.

The dashboard should prioritize a short list of actions rather than
overwhelm the user with charts. Examples: - "3 assets have no assigned
owner." - "2 security reviews are overdue." - "1 device is marked as
running an unsupported operating system." - "1 open issue is past its
target date."

Make it clear whether a status is based on user-entered information, a
completed review, or a technical integration.

## Development workflow for GitHub Copilot

Act as a careful senior engineer and pragmatic technical partner. The
project owner has prior .NET and software-development experience but is
building this product incrementally alongside a full-time job.

### Before changing code

1.  Inspect the repository, current branch, working tree, and existing
    conventions.
2.  Read `docs/PROJECT_SPECIFICATION.md` if it exists.
3.  Read relevant architecture and security documentation.
4.  Identify the smallest useful task and its acceptance criteria.
5.  State assumptions and any important design trade-offs before
    implementing a non-trivial change.
6.  Do not overwrite or discard existing user work.

### While implementing

-   Work in small, reviewable increments.
-   Prefer simple, idiomatic C# and standard ASP.NET Core patterns.
-   Use asynchronous I/O for database and network operations.
-   Use dependency injection rather than service-locator patterns.
-   Use DTOs/ViewModels for request and presentation boundaries; do not
    bind EF entities directly to untrusted forms.
-   Keep controllers thin.
-   Keep business rules out of Razor views.
-   Use explicit EF Core entity configurations for relationships,
    constraints, indexes, and delete behavior.
-   Add or update tests with each meaningful feature.
-   Use migrations for schema changes; explain the migration and any
    data implications.
-   Handle errors safely and provide useful validation feedback.
-   Avoid premature abstractions and unnecessary packages.
-   Do not add a dependency without explaining why it is needed.
-   Do not silently change framework versions, major architecture,
    tenancy strategy, or hosting assumptions.

### GitHub Copilot collaboration style

Use these instructions as the project-wide source of truth when working in
GitHub Copilot Chat, Agent mode, or when generating code completions.

- Explain the purpose of a change briefly before making it.
- For a multi-step task, begin with a concise plan and identify the files
  likely to change. Implement only the requested scope.
- Do not dump an entire application or generate many unrelated files at once.
- When the task is larger than one focused change, propose a sequence and
  implement only the first approved step unless the user explicitly asks
  you to proceed through the full sequence.
- Ask for clarification only when a missing decision materially affects
  correctness or creates significant rework.
- Treat repository code, tests, configuration, and documentation as the
  source of truth for existing implementation details. Inspect them before
  proposing changes.
- Do not assume a feature exists because it appears in this guide; verify
  the current repository state first.
- Do not invent credentials, secrets, customer data, production URLs, or
  service configuration.
- Never claim a test, build, migration, deployment, or security check
  succeeded unless it was actually run and its result observed.
- If a command cannot be run, say so and provide the exact command the
  project owner can run.
- Do not silently change framework versions, major architecture,
  tenancy strategy, or hosting assumptions.
- Do not add a dependency without explaining why it is needed.
- Keep generated code idiomatic for the project's .NET and C# versions.
- Prefer targeted edits over replacing entire files. Preserve unrelated
  user changes.
- Do not weaken authorization, tenant isolation, validation, or security
  controls to make a test pass.
- At the end of each task, summarize:
  1. What changed
  2. Files affected
  3. Tests/builds run and their results
  4. Any migration or configuration steps
  5. Known limitations and the recommended next small task

### Testing expectations

Use a practical test pyramid: - Unit tests for domain rules and
calculations. - Integration tests for EF Core mappings, persistence,
authorization, and tenant isolation. - A small number of end-to-end
tests for critical user workflows when the application is ready.

Critical test cases include: - A user from Organization A cannot read,
update, or delete Organization B's records. - A user cannot create a
relationship between records from different organizations. - An asset
cannot have two active assignments. - Returning an asset closes the
active assignment and preserves history. - Users cannot access functions
outside their role. - Unauthorized users cannot download evidence
files. - Invalid assessment states and invalid lifecycle transitions are
rejected.

## Suggested implementation phases

Do not attempt all phases at once. Keep each phase independently
reviewable.

### Phase 0 --- Repository and development foundation

-   Inspect the existing repository.
-   Create the solution and projects.
-   Configure .NET 10, nullable reference types, and analyzers sensibly.
-   Add PostgreSQL local development through Docker Compose.
-   Configure EF Core and Npgsql.
-   Add environment-based configuration and a safe local secrets
    approach.
-   Add a basic health endpoint.
-   Add GitHub Actions for restore, build, and tests.
-   Document local setup.

**Exit criteria:** a clean clone can be configured locally, the
application starts, it can connect to PostgreSQL, and CI builds and
tests successfully.

### Phase 1 --- Authentication and tenancy

-   Configure ASP.NET Core Identity.
-   Implement organizations and memberships.
-   Implement organization creation and active-organization selection.
-   Add role-based authorization policies.
-   Add tenant-scoped persistence patterns.
-   Add cross-tenant integration tests.

**Exit criteria:** users can authenticate and access only organizations
to which they belong. Do not begin broad business functionality until
tenant isolation is tested.

### Phase 2 --- Asset accountability

-   Implement employees, departments, locations, and asset categories.
-   Implement asset create, view, edit, archive, and search.
-   Implement assignments and returns.
-   Record asset history.
-   Add basic asset and assignment reports.

**Exit criteria:** a small business can import or enter its equipment,
assign it to people/locations, and see its history.

### Phase 3 --- Security and governance

-   Add the initial plain-language control library.
-   Implement organization-level and asset-level assessments.
-   Implement evidence metadata and private file storage.
-   Implement risks, exceptions, owners, and due dates.
-   Make self-reported versus verified information explicit.

**Exit criteria:** a business owner can record a review, attach
supporting evidence, and track an issue to resolution.

### Phase 4 --- Dashboard and reporting

-   Build an owner-focused action dashboard.
-   Add overdue-review and missing-information indicators.
-   Add exportable asset accountability and governance reports.
-   Ensure all summaries are tenant-scoped and explain their data
    sources.

**Exit criteria:** a non-technical owner can identify the most important
follow-up actions without interpreting a complex control framework.

### Phase 5 --- Production readiness

-   Containerize the application.
-   Configure Linux VPS deployment and Nginx/TLS.
-   Use private external object storage for evidence.
-   Configure encrypted off-site backups and perform a restore test.
-   Add structured logging, error tracking, uptime monitoring, and
    health checks.
-   Document deployment, rollback, migrations, recovery, and incident
    procedures.
-   Review tenant isolation and authorization before inviting real
    customers.

**Exit criteria:** the application can be deployed and recovered using
documented procedures.

### Phase 6 --- Commercialization

Only after validating the product with real businesses: - Add
subscription billing and plan entitlements. - Add onboarding and import
workflows. - Add transactional email. - Add customer-facing help and
support processes. - Publish appropriate terms, privacy, retention, and
cancellation policies. - Provide customer data export and
account-closure workflows. - Validate pricing and support burden.

Do not build billing, complex plan restrictions, or elaborate marketing
infrastructure before customer discovery demonstrates the need.

## Initial product validation

Do not spend months building in isolation. Validate the problem while
implementing the foundation.

Aim to: - Interview 5--10 owners or office managers in the target
market. - Ask how they currently track equipment, assignments, returns,
warranties, and security reviews. - Ask what happens when an employee
leaves or equipment is lost. - Ask what they need to show an owner,
insurer, auditor, customer, or other stakeholder. - Observe existing
spreadsheets and workflows with permission. - Recruit 3--5 design
partners before investing heavily in advanced governance features. -
Test whether customers will pay for the workflow, not merely say that it
sounds useful.

Do not collect sensitive customer records during discovery unless there
is a clear, authorized need and appropriate safeguards.

## First task for a new GitHub Copilot session

Start by inspecting the repository. Do not immediately generate the
entire application. Treat this file as persistent project guidance, but
verify all implementation details against the current repository.

1.  Report the current repository structure, branch, and working-tree
    state.
2.  Identify whether `docs/PROJECT_SPECIFICATION.md` exists and read it
    if present.
3.  Compare the existing repository with this guide and identify any
    conflicts or missing decisions.
4.  Recommend a minimal Phase 0 implementation plan, including project
    structure, required NuGet packages, local PostgreSQL setup, and CI
    checks.
5.  Identify the first small implementation task and its acceptance
    criteria.
6.  Wait for approval before making broad changes. Small, clearly
    requested inspection or setup tasks may proceed without separate approval.

The guiding principle is to build a trustworthy, useful product in small
increments---not an enterprise platform.


## Copilot instruction-file placement

Store this document at `.github/copilot-instructions.md` in the repository.
GitHub Copilot can use it as repository-wide guidance. Keep product
requirements, architectural constraints, security rules, and workflow
expectations here. Put detailed, feature-specific guidance in separate
instruction files only when the repository grows enough to justify them.
