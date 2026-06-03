# T-200-ARCH Clean Architecture and Blazor Multi-Platform Migration Plan

**Status:** In Progress
**Priority:** P0
**Related Bug:** N/A
**Related Plans:** N/A

---

## 1. Problem Statement

The current FlashcardViewer project is structured as a single monolithic .NET MAUI application targeting `.net10.0`. Its UI is defined entirely in MAUI XAML (`.xaml` and `.xaml.cs` views paired with MVVM view models), which restricts deployment exclusively to platforms supported by MAUI native client runtimes (Windows, Android, MacCatalyst). 

There is currently no pathway to run FlashcardViewer inside web browsers as a static web application. Furthermore, the application lacks clear architectural boundaries, making it difficult to write decoupled tests or prevent UI/Framework concerns from leaking into domain entity and data storage rules.

## 2. Goals

1. **Decouple Core Business Rules**: Isolate domain entities and core application logic from UI and database frameworks.
2. **Enable Web Browser Support**: Structure the UI using Razor components (`.razor`) and CSS so it can be hosted in both Blazor WebAssembly (for the web) and Blazor Hybrid (for native desktop/mobile wrappers).
3. **Establish Data Access Abstractions**: Decouple the SQLite persistence engine so that native platforms run direct local filesystem I/O, while sandboxed web browsers fallback to IndexedDB or Local Storage.
4. **Programmatic Architecture Enforcement**: Introduce automated architecture tests that fail compilation if cross-boundary dependencies are introduced (e.g., Domain referencing Application, or Application referencing UI).
5. **No Code Loss**: Preserve all existing feature functionality (Flashcard management, sets, session configurations, theme support).

## 3. Non-Goals

1. Do not port the application to .NET 11 (remain pinned to the `.net10.0` environment).
2. Do not rewrite UI logic to use WinUI XAML, Uno Platform, or Avalonia.
3. Do not modify the existing SQLite data schema or raw asset contents.
4. Do not target iOS in this plan (keep platform support matrix identical to the pre-migration state: Windows, Android, MacCatalyst, and Web).

## 4. Architecture Decision

Decouple the application using a Clean Architecture design pattern with dependency-inversion boundaries. The target projects and dependencies are structured as follows:

```mermaid
graph TD
    %% UI Layer
    subgraph UI_Layer [UI Layer]
        MAUI[FlashcardViewer.Maui]
        WASM[FlashcardViewer.Wasm]
        RCL[FlashcardViewer.SharedUI]
    end

    %% Infrastructure Layer
    subgraph Infrastructure_Layer [Infrastructure Layer]
        INFRA_DB[FlashcardViewer.Infrastructure.Sqlite]
        INFRA_WEB[FlashcardViewer.Infrastructure.Web]
    end

    %% Core Layers
    subgraph Core_Layers [Core Layers]
        APP[FlashcardViewer.Application]
        DOMAIN[FlashcardViewer.Domain]
    end

    %% Test Layer
    subgraph Test_Layer [Verification Layer]
        ARCH_TESTS[FlashcardViewer.ArchitectureTests]
        UNIT_TESTS[FlashcardViewer.UnitTests]
    end

    %% Dependencies
    MAUI --> RCL
    WASM --> RCL
    RCL --> APP
    
    MAUI -.-> INFRA_DB
    WASM -.-> INFRA_WEB
    
    INFRA_DB --> APP
    INFRA_WEB --> APP
    
    APP --> DOMAIN
    
    ARCH_TESTS --> APP
    ARCH_TESTS --> DOMAIN
    ARCH_TESTS --> INFRA_DB
```

* **Core Domain and Application**: Contains entities, value objects, use cases, and interfaces. This project contains zero references to MAUI, Blazor, SQL, or web components.
* **Shared UI (Razor Class Library)**: Hosts all shared pages (`.razor`) and static assets (CSS, images). Views communicate with the `Application` layer via states or commands.
* **Platform Hosts**: `FlashcardViewer.Maui` and `FlashcardViewer.Wasm` act as thin execution shells. They register platform-specific infrastructure implementations (SQLite vs Web LocalStorage) in the dependency injection container and host the Shared UI root component.

## 5. Planned Slices

- [ ] **T-200a (P0): Core Domain and Application Boundary Setup**
  - **Goal:** Spin up core projects, migrate domain models, and define programmatic dependency verification rules.
  - **DoD:**
    1. Create `FlashcardViewer.Domain` (`net10.0`) class library.
    2. Create `FlashcardViewer.Application` (`net10.0`) class library referencing `Domain`.
    3. Create `FlashcardViewer.ArchitectureTests` (`net10.0`) xUnit test project using `NetArchTest.eNET`.
    4. Write architecture tests ensuring:
       - `Domain` has no external dependencies.
       - `Application` depends only on `Domain`.
       - Infrastructure layers depend on `Application` but not vice versa.
    5. Migrate entities (`Flashcard`, `FlashcardSet`, `SessionConfig`) from the current project to `Domain`.
    6. Implement abstract interface definitions (`IFlashcardRepository`, `IDatabaseInitializer`) in `Application`.
  - **Artifacts:** `FlashcardViewer.Domain/`, `FlashcardViewer.Application/`, `FlashcardViewer.ArchitectureTests/`, updated `FlashcardViewer.slnx`.

- [ ] **T-200b (P0): Infrastructure and Storage Decoupling**
  - **Goal:** Move database access code out of the application and set up platform storage layers.
  - **DoD:**
    1. Create `FlashcardViewer.Infrastructure.Sqlite` (`net10.0`) containing the SQLite database schema setup and operations.
    2. Create `FlashcardViewer.Infrastructure.Web` (`net10.0`) implementing repository interfaces using browser local storage/IndexedDB.
    3. Create `FlashcardViewer.UnitTests` xUnit project.
    4. Implement SQLite repository integration tests verifying correct database migrations and CRUD execution paths on Windows.
  - **Artifacts:** `FlashcardViewer.Infrastructure.Sqlite/`, `FlashcardViewer.Infrastructure.Web/`, `FlashcardViewer.UnitTests/`.

- [ ] **T-200c (P0): Shared UI Razor Component Porting**
  - **Goal:** Convert current MAUI XAML pages into responsive, platform-agnostic Razor Web components.
  - **DoD:**
    1. Create Razor Class Library `FlashcardViewer.SharedUI` (`net10.0`).
    2. Establish a responsive layout (`MainLayout.razor`) and design tokens (variables, typography, transitions) in `wwwroot/app.css`.
    3. Convert view pages to Razor Components:
       - `FlashcardSetListPage` $\rightarrow$ `FlashcardSetList.razor`
       - `FlashcardListPage` $\rightarrow$ `FlashcardList.razor`
       - `FlashcardSessionPage` $\rightarrow$ `FlashcardSession.razor`
       - `SessionConfigManagementPopup` $\rightarrow$ `SessionConfigPopup.razor`
    4. Implement component state and event handlers linked to `Application` services.
  - **Artifacts:** `FlashcardViewer.SharedUI/`.

- [ ] **T-200d (P0): Bootstrapping MAUI Hybrid and Blazor WebAssembly Hosts**
  - **Goal:** Configure host projects to bootstrap the Shared UI with platform-specific configurations.
  - **DoD:**
    1. Refactor the original project to act as the thin native host `FlashcardViewer.Maui` (`net10.0`).
    2. Configure `MainPage.xaml` to host a `<BlazorWebView>` targeting `wwwroot/index.html`.
    3. Register `Infrastructure.Sqlite` and shared components in `MauiProgram.cs`.
    4. Create `FlashcardViewer.Wasm` (`net10.0`) Blazor WebAssembly project.
    5. Register `Infrastructure.Web` and shared components in `Program.cs` of the Wasm host.
    6. Verify solution builds and restores cleanly for all targets.
  - **Artifacts:** `FlashcardViewer.Maui/` project refactoring, `FlashcardViewer.Wasm/` project creation, root `FlashcardViewer.slnx`.

- [ ] **T-200e (P1): Continuous Integration and Continuous Deployment Pipelines**
  - **Goal:** Automate code validation and compilation for multi-platform delivery.
  - **DoD:**
    1. Write `.github/workflows/ci.yml` to trigger on pull requests and branch updates (verifies restores, compiles projects, and runs unit/architecture tests).
    2. Write `.github/workflows/deploy.yml` configured to:
       - Package the Windows desktop release (MSIX).
       - Compile the Android APK.
       - Publish the Blazor WebAssembly app to GitHub Pages.
  - **Artifacts:** `.github/workflows/ci.yml`, `.github/workflows/deploy.yml`.

## 6. Verification

1. **Architecture Test**: Passing assembly scan verifying Domain, Application, and Infrastructure boundaries are preserved.
2. **Persistence Test**: SQLite repositories write/read test data correctly.
3. **Execution Verification (Desktop)**: Executing `dotnet run -f net10.0-windows10.0.26100.0` in `FlashcardViewer.Maui` starts the application, initializes the DB, and shows the UI.
4. **Execution Verification (Web)**: Executing `dotnet run` in `FlashcardViewer.Wasm` opens the client app in a local browser port and fully loads/edits card sets.

## 7. Fallout And Coordination

1. Porting ViewModels to Web-compatible state handlers must not alter the core execution rules of flashcard session calculations.
2. When creating the Shared UI project, CSS components should avoid TailwindCSS and stick to Vanilla CSS matching current styling elements to ensure maximum flexibility and direct transition control.
3. Central Package Management (CPM) configurations in the parent directory must be verified so they do not conflict with the isolated project files.

## 8. Open Questions

1. Should the IndexedDB storage layer synchronize card changes back to an external web service, or remain local-only?
2. Are there any custom SVG assets or fonts in the native platform folders that need to be migrated to the shared web assembly environment?
