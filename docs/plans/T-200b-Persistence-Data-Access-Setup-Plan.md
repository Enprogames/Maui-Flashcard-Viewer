# T-200b Persistence and Data Access Setup Plan

**Status:** Completed
**Priority:** P0
**Related Bug:** N/A
**Related Plans:** `T-200-ARCH-Clean-Architecture-Blazor-Hybrid-Plan.md`

---

## 1. Problem Statement
Currently, database persistence is implemented inside the client host app (`FlashcardViewer/Services/LocalFlashcardDataStore.cs`) using `sqlite-net-pcl`. Since web browsers run inside a sandboxed environment, we cannot use direct local file SQLite database connections on the web (Blazor WebAssembly). 

We need to decouple the persistence logic. The repository interfaces from `FlashcardViewer.Application` must be implemented in two isolated infrastructure projects:
1. `FlashcardViewer.Infrastructure.Sqlite` for native mobile/desktop platforms (using SQLite file database access).
2. `FlashcardViewer.Infrastructure.Web` for the web client (using browser `localStorage` or `IndexedDB`).

We also need to implement comprehensive integration and unit tests using **TUnit** to verify both storage implementations.

## 2. Goals
1. Create `FlashcardViewer.Infrastructure.Sqlite` inside `src/` to house SQLite operations.
2. Create `FlashcardViewer.Infrastructure.Web` inside `src/` to house browser storage operations.
3. Write TUnit integration tests validating SQLite transactions against in-memory connections.
4. Write TUnit unit tests with mocked browser state APIs validating web storage configurations.
5. De-couple SQLite dependencies from the `FlashcardViewer.Domain` library (porting SQLite-net attributes to infrastructure-specific representations or ensuring they are decoupled).

## 3. Non-Goals
1. Do not touch UI views or Razor Class Library components (Slice 3).
2. Do not change existing SQLite tables or column configurations.

## 4. Plan Checklist

### Slice T-200b1: SQLite Infrastructure & Persistence Tests

- [x] **T-200b-1.1**: Create class library `FlashcardViewer.Infrastructure.Sqlite` (`net10.0`) under `/src`:
  * Command: `dotnet new classlib -n FlashcardViewer.Infrastructure.Sqlite -f net10.0 -o src/FlashcardViewer.Infrastructure.Sqlite`
- [x] **T-200b-1.2**: Add dependencies to `FlashcardViewer.Infrastructure.Sqlite`:
  * Add project reference to `FlashcardViewer.Application`.
  * Add NuGet package `sqlite-net-pcl` (version `1.9.172`).
- [x] **T-200b-1.3**: Move SQLite specific implementations out of the native client program and implement `IFlashcardRepository` and `IDatabaseInitializer` in `FlashcardViewer.Infrastructure.Sqlite`:
  * Create `SqliteFlashcardRepository.cs` implementing `IFlashcardRepository`.
  * Create `SqliteDatabaseInitializer.cs` implementing `IDatabaseInitializer`.
- [x] **T-200b-1.4**: Set up TUnit integration tests in `tests/FlashcardViewer.UnitTests` checking SQLite CRUD transactions:
  * Initialize an in-memory SQLite connection (`:memory:`).
  * Verify `AddFlashcardSetAsync`, `GetFlashcardsForSetAsync`, and card mutations execute correctly.
- [x] **T-200b-1.5**: Link project references: `tests/FlashcardViewer.UnitTests` $\rightarrow$ `src/FlashcardViewer.Infrastructure.Sqlite`.

### Slice T-200b2: Web Infrastructure & Web Test Coverage

- [x] **T-200b-2.1**: Create class library `FlashcardViewer.Infrastructure.Web` (`net10.0`) under `/src`:
  * Command: `dotnet new classlib -n FlashcardViewer.Infrastructure.Web -f net10.0 -o src/FlashcardViewer.Infrastructure.Web`
- [x] **T-200b-2.2**: Add project reference `FlashcardViewer.Infrastructure.Web` $\rightarrow$ `FlashcardViewer.Application`.
- [x] **T-200b-2.3**: Implement `IFlashcardRepository` in `FlashcardViewer.Infrastructure.Web` using browser LocalStorage:
  * Create `WebFlashcardRepository.cs` utilizing a serialization layer to store items.
- [x] **T-200b-2.4**: Implement TUnit unit tests in `tests/FlashcardViewer.UnitTests` validating browser persistence fallback boundaries (e.g. serialize/deserialize correctness, empty storage handling).
- [x] **T-200b-2.5**: Link project references: `tests/FlashcardViewer.UnitTests` $\rightarrow$ `src/FlashcardViewer.Infrastructure.Web`.
- [x] **T-200b-2.6**: Update `FlashcardViewer.slnx` to include `src/FlashcardViewer.Infrastructure.Sqlite` and `src/FlashcardViewer.Infrastructure.Web` projects.

---

## 5. Verification

1. **Format Compliance**: Ensure all C# files end with a single newline, and all `.csproj` and `.slnx` files end with zero newlines (by running `powershell -File tests/format-check.ps1`).
2. **Clean compilation**: `dotnet build src/FlashcardViewer.Infrastructure.Sqlite` and `src/FlashcardViewer.Infrastructure.Web` compile without warning.
3. **Execution of tests**: Run `dotnet run --project tests/FlashcardViewer.UnitTests` and confirm all persistence/unit tests pass.
