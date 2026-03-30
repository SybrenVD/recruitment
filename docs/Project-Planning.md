# Recruitment Application – Architecture Overview

This solution is a full-stack recruitment platform built on **.NET 10**.  
It uses a layered / clean-architecture approach so that each concern lives in its own project and dependencies always flow inward (toward the domain).

---

## Layer Summary

| Project | Role |
|---|---|
| `Recruitment.Entities` | Domain models |
| `Recruitment.Interfaces` | Service & repository contracts |
| `Recruitment.Data` | Data access (EF Core + SQL Server) |
| `Recruitment.Services` | Business logic & algorithms |
| `Recruitment.Api` | REST API (ASP.NET Core) |
| `Recruitement.Ui.Blazor` | Blazor WebAssembly front-end |
| `Recruitment.Dto` | Data-transfer objects *(placeholder)* |
| `Recruitment.Responses` | API response models *(placeholder)* |
| `Recuitment.Requests` | API request models *(placeholder)* |

---

## Layer Details

### 1. `Recruitment.Entities` – Domain Models
The innermost layer. Contains all EF Core entity classes that map directly to database tables:

- **Candidate / Recruiter** – user profiles for job-seekers and employers.
- **Job** – a job listing, owned by a recruiter.
- **Skill** – the master catalog of available skills.
- **JobSkill** – a join table that links a skill to a job, recording the *required level* and a *weight* (importance).
- **CandidateSkill** – a join table that records a candidate's *proficiency level* for a skill.
- **JobMatch** – stores the computed match score and whether each party swiped "like".
- **CVAnalysis** – the result of an AI-powered analysis of a candidate's uploaded CV.
- **InterviewQuestion** – suggested interview questions generated per match.

No dependencies on other projects in this solution.

---

### 2. `Recruitment.Interfaces` – Service & Repository Contracts
Defines the *what*, not the *how*. Every service and the repository pattern are expressed as interfaces here:

- `IRepository<T>` / `IUnitOfWork` – generic data-access abstractions with transaction support.
- `ICandidateService`, `IJobService`, `ISkillService`, `IRecruiterService` – CRUD contracts for the main entities.
- `IMatchingService` – contract for the skill-based matching algorithm and swipe workflow.
- `ICVAnalysisService` – contract for CV parsing and AI-powered analysis.
- `IFileStorageService` / `IPdfReaderService` / `IAIService` – infrastructure abstractions for file I/O, PDF text extraction, and the AI backend.

Depends only on `Recruitment.Entities`.

---

### 3. `Recruitment.Data` – Data Access Layer
Implements the persistence side of the contracts defined above:

- **`RecruitmentDbContext`** – EF Core `DbContext` with eight `DbSet<T>` properties, fluent entity configurations, cascade-delete rules, and index definitions.
- **`DbContextFactory`** – creates context instances for use in tooling (e.g. migrations).
- **`DataSeeder`** – populates the database with an initial set of 18 skills when the database is empty.

Uses SQL Server via the EF Core 10.0.4 provider.  
Depends on `Recruitment.Entities`.

---

### 4. `Recruitment.Services` – Business Logic
Implements all interfaces from `Recruitment.Interfaces`. This is where the real work happens:

- **`Repository<T>` / `UnitOfWork`** – concrete EF Core implementations of the generic repository and unit-of-work with transaction support.
- **`MatchingService`** *(core algorithm)* – calculates a weighted skill-match score between a candidate and a job, surfaces top suggestions for both sides, and orchestrates the Tinder-style swipe workflow (candidate swipe → recruiter swipe → mutual match).
- **`CandidateService` / `JobService` / `SkillService` / `RecruiterService`** – CRUD operations plus domain-specific logic such as creating a job together with its required skills.
- **`CVAnalysisService`** – reads a candidate's uploaded PDF CV (via `PdfReaderService`), sends the text to the AI service, and stores the structured result (summary, strengths, weaknesses, experience level).
- **`FileStorageService`** – saves and retrieves CV files from the local file system using a configurable base path.
- **`PdfReaderService`** – extracts plain text from PDF files using the PdfPig library.
- **`AIService`** – calls the configured AI backend and returns a JSON analysis payload.

Depends on `Recruitment.Interfaces` and `Recruitment.Data`.

---

### 5. `Recruitment.Api` – REST API
The HTTP entry point for all clients. Built with ASP.NET Core on .NET 10.

**Controllers:**
- **`AuthController`** – registers and authenticates candidates and recruiters; issues JWT tokens.
- **`CandidatesController`** – full CRUD for candidates plus CV upload/download and analysis endpoints.
- **`JobsController`** – full CRUD for jobs, skill attachment/removal, and filtering by keyword, location, and experience level.
- **`MatchingController`** – exposes the matching algorithm: trigger CV analysis, retrieve job/candidate suggestions, process swipes, and retrieve mutual matches.
- **`SkillsController`** – CRUD for the skill catalog.
- **`RecruitersController`** – full CRUD for recruiters.

**Cross-cutting concerns wired up in `Program.cs`:**
- JWT Bearer authentication and authorization.
- CORS policy allowing the Blazor front-end on localhost.
- JSON serialization options (reference-cycle handling, null suppression).
- Dependency injection registration for all services.

Depends on `Recruitment.Interfaces`, `Recruitment.Services`, `Recruitment.Data`, and `Recruitment.Entities`.

---

### 6. `Recruitement.Ui.Blazor` – Front-End (Blazor WebAssembly)
A single-page application that runs entirely in the browser.

**Pages:**
- **Home** – landing page.
- **Login / Register** – authentication forms for both user roles.
- **Profile** – view and edit personal information; upload a CV.
- **Swipe** – candidate browses job cards and swipes left/right (Tinder-style).
- **SwipeCandidates** – recruiter browses candidate cards and swipes left/right.
- **PostJob / MyJobs** – recruiter creates new job listings and manages existing ones.
- **Matches** – both parties view their mutual matches.

**Services:**
- **`AuthService`** – stores the JWT token in `localStorage`, decodes it to extract the user's role and ID, and exposes login/logout helpers.
- **`RecruitmentApiService`** – thin HTTP client wrapper that attaches the Bearer token to every request and maps responses to local model classes.

**Auth infrastructure:**
- `JwtAuthenticationStateProvider` – bridges the JWT token with Blazor's `AuthenticationStateProvider` so that `[Authorize]` and `<AuthorizeView>` work correctly.
- `JwtAuthorizationMessageHandler` – `DelegatingHandler` that injects the token into outgoing HTTP requests automatically.

Role-based UI: navigation items and pages are shown or hidden depending on whether the logged-in user is a *Candidate* or a *Recruiter*.

Depends on `Recruitment.Entities` (shared models) and communicates with `Recruitment.Api` over HTTP.

---

### 7. `Recruitment.Dto` – Data Transfer Objects *(placeholder)*
Intended to hold request and response DTO classes that decouple the API contract from the internal entity models. Currently contains empty `Requests/` and `Responses/` sub-folders. Depends on `Recruitment.Entities`.

---

### 8. `Recruitment.Responses` – API Response Models *(placeholder)*
Reserved for strongly-typed response objects to be returned by API endpoints. Not yet implemented.

---

### 9. `Recuitment.Requests` – API Request Models *(placeholder)*
Reserved for strongly-typed request body models for API endpoints. Not yet implemented.

---

## Dependency Graph

```
Recruitement.Ui.Blazor  ──────────────────────────────────────┐
  (Blazor WASM)                                               │ HTTP
                                                              ▼
Recruitment.Api  ──► Recruitment.Services  ──► Recruitment.Data
  (Controllers)         (Business Logic)         (EF Core)
       │                      │                      │
       └──────────────────────┴──────────────────────┘
                              │
                     Recruitment.Interfaces
                       (Service Contracts)
                              │
                     Recruitment.Entities
                       (Domain Models)
```

All dependencies point inward toward `Recruitment.Entities`, keeping the domain model free of infrastructure concerns.

---

## Key Matching Algorithm

The weighted skill-match score is calculated in `MatchingService.CalculateMatchAsync()`:

```
For each skill required by the job:
    candidate_score = MAX(0, (5 - |candidateLevel - requiredLevel|) * 100 / 5)
    weighted_score  = candidate_score * skill.Weight

finalScore = (sum of weighted_scores / max possible weighted score) * 100
```

Skills the candidate does not possess are recorded as *skill gaps* and surfaced alongside the score.

---

## Technology Stack

| Concern | Technology | Version |
|---|---|---|
| Runtime | .NET | 10.0 |
| Back-end framework | ASP.NET Core | 10.0 |
| Front-end framework | Blazor WebAssembly | 10.0 |
| ORM | Entity Framework Core | 10.0.4 |
| Database | SQL Server | – |
| Authentication | JWT Bearer | 10.0.5 |
| PDF parsing | PdfPig | 0.1.9 |
