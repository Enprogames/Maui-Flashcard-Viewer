# T-200a Core Domain and Application Setup Plan

**Status:** Completed
**Priority:** P0
**Related Bug:** N/A
**Related Plans:** `T-200-ARCH-Clean-Architecture-Blazor-Hybrid-Plan.md`

---

## 1. Goal

Migrate core domain models (`Flashcard`, `FlashcardSet`, `SessionConfig`) into an isolated `FlashcardViewer.Domain` project and set up interfaces, abstraction behaviors, and dependency enforcement rules in a `FlashcardViewer.Application` project. Create a `FlashcardViewer.ArchitectureTests` suite using the **TUnit** testing library to ensure architectural boundaries are not violated.

All new code projects will live under `/src`, and all test projects will live under `/tests`.

## 2. Non-Goals

1. Do not implement SQLite persistence or LocalStorage storage layers (left for Slice 2).
2. Do not port or refactor the XAML views into Razor Components (left for Slice 3).
3. Do not modify existing logic rules inside models.

## 3. Plan Checklist

### Phase 1: Directory & Project Setup

- [x] **T-200a-1.1**: Create the core directory folders `src/` and `tests/` in the project root.
- [x] **T-200a-1.2**: Create the class library project `FlashcardViewer.Domain` targeting `net10.0` under the `/src` folder:
  * Command: `dotnet new classlib -n FlashcardViewer.Domain -f net10.0 -o src/FlashcardViewer.Domain`
- [x] **T-200a-1.3**: Create the class library project `FlashcardViewer.Application` targeting `net10.0` under the `/src` folder:
  * Command: `dotnet new classlib -n FlashcardViewer.Application -f net10.0 -o src/FlashcardViewer.Application`
- [x] **T-200a-1.4**: Establish project reference: `FlashcardViewer.Application` $\rightarrow$ `FlashcardViewer.Domain`.
- [x] **T-200a-1.5**: Create the test projects under `/tests`:
  * Create `FlashcardViewer.ArchitectureTests` targeting `net10.0`.
  * Create `FlashcardViewer.UnitTests` targeting `net10.0`.
- [x] **T-200a-1.6**: Add **TUnit** reference to both test projects:
  * `dotnet add tests/FlashcardViewer.ArchitectureTests package TUnit`
  * `dotnet add tests/FlashcardViewer.UnitTests package TUnit`
- [x] **T-200a-1.7**: Add the newly created projects to [FlashcardViewer.slnx](file:///C:/Users/e_pos/Dropbox/VSCodeProjects/.NET/WebApplications/FlashcardViewer/FlashcardViewer.slnx).

### Phase 2: Domain Migration & Validation Tests

- [x] **T-200a-2.1**: Move the following models from the current monolithic project into `FlashcardViewer.Domain`:
  * `Models/Flashcard.cs`
  * `Models/FlashcardSet.cs`
  * `Models/SessionConfig.cs` *(Note: SessionConfig was already defined directly inside ViewModels, no separate model file needed migration)*.
- [x] **T-200a-2.2**: Update namespaces in these moved files to `FlashcardViewer.Domain`.
- [x] **T-200a-2.3**: Write TUnit unit tests in `tests/FlashcardViewer.UnitTests` validating:
  * Proper properties and logic on `Flashcard` and `FlashcardSet`.
  * `SessionConfig` settings constraints and validation boundaries.

### Phase 3: Application Abstractions

- [x] **T-200a-3.1**: Add a dependency-inversion interface layer in `FlashcardViewer.Application`:
  * Create `Interfaces/IFlashcardRepository.cs` representing data store interactions.
  * Create `Interfaces/IDatabaseInitializer.cs` representing initialization setups.
- [x] **T-200a-3.2**: Port any shared app-level helper logic (e.g. storage mode calculation checks, formatting helpers) to `FlashcardViewer.Application`.

### Phase 4: Architectural Enforcement

- [x] **T-200a-4.1**: Add `NetArchTest.eNET` package to `FlashcardViewer.ArchitectureTests`:
  * Command: `dotnet add tests/FlashcardViewer.ArchitectureTests package NetArchTest.eNET` *(Note: Installed NetArchTest.Rules 1.3.2 as correct package name)*.
- [x] **T-200a-4.2**: Implement TUnit architecture validation rules in `tests/FlashcardViewer.ArchitectureTests`:
  * **Test 1**: Verify `Domain` has no dependencies on other assemblies inside the solution or external UI/DB components.
  * **Test 2**: Verify `Application` depends only on `Domain`.

---

## 4. Verification

1. **Clean compilation**: `dotnet build FlashcardViewer.slnx` builds successfully.
2. **Execution of tests**: `dotnet test FlashcardViewer.slnx` runs and passes all TUnit tests (unit tests and architecture tests).
3. **No regressions on native wrapper**: Ensure current MAUI project (which still references original files temporarily) builds successfully.
