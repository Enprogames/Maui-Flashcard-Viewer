# Coding Agent Instructions & Architecture Guide

Welcome, Agent. You are pair programming on the **FlashcardViewer** solution. This file contains the mandatory architectural rules, project structures, formatting constraints, and verification procedures you must follow when editing code or creating new files.

---

## 1. Solution Architecture & Directories

We are migrating this project to a **Clean Architecture** structure that separates domain logic from hosting wrappers (Windows, Android, MacCatalyst, and Blazor WebAssembly).

```
Project Root
├── src/                         # All source code projects
│   ├── FlashcardViewer.Domain/        # Pure domain entities (no framework dependencies)
│   ├── FlashcardViewer.Application/   # Application use cases, interfaces, and state
│   ├── FlashcardViewer.SharedUI/      # Shared Razor components (*.razor) and Vanilla CSS styling
│   ├── FlashcardViewer.Maui/          # .NET MAUI wrapper host (Android, Windows, macOS)
│   └── FlashcardViewer.Wasm/          # Blazor WebAssembly static client host
├── tests/                       # All testing and verification projects
│   ├── FlashcardViewer.UnitTests/     # Unit tests verifying domain and application logic
│   ├── FlashcardViewer.ArchitectureTests/ # Programmatic architectural dependency boundary checks
│   └── format-check.ps1               # Automated formatting validation script
├── docs/
│   └── plans/                   # Migration plans (e.g. T-200-ARCH-Clean-Architecture-Blazor-Hybrid-Plan.md)
└── FlashcardViewer.slnx          # Solution file in XML format
```

### Dependency Rules
* `Domain` must not reference any project or third-party database/UI frameworks.
* `Application` depends only on `Domain`. Interfaces are declared here; implementation details are injected.
* Infrastructure layers (`Infrastructure.Sqlite`, `Infrastructure.Web`) depend on `Application`.
* The `SharedUI` Razor class library depends on `Application`.
* Host projects (`Maui`, `Wasm`) reference `SharedUI` and register their respective storage infrastructures at startup via Dependency Injection.
* **Arch Tests Validation**: Programmatic boundary validation is enforced inside `tests/FlashcardViewer.ArchitectureTests` using `NetArchTest.Rules`. Any compilation that breaches these boundaries will fail verification.

---

## 2. Testing Standards

* **Framework**: All test suites must use **TUnit** as the primary testing framework. Do not use xUnit, NUnit, or MSTest templates.
* **Coverage Mandate**: Every task slice involving code additions or changes must include corresponding unit, integration, or component tests.
* **No Stray Files**: Never commit template defaults like `Class1.cs` or `UnitTest1.cs`. Delete them immediately during project creation.

---

## 3. Formatting & Encoding Constraints

You must ensure that all files you modify or create comply with the following formatting criteria (enforced by the root `.editorconfig`):

| File Type | Ending Newline Requirement | File Encoding |
| :--- | :--- | :--- |
| **C# Source Code (`*.cs`)** | Ends with exactly **one** single newline character | UTF-8 (No BOM) |
| **XML Configurations (`*.csproj`, `*.xaml`)** | Ends with **zero** trailing newlines | UTF-8 (No BOM) |
| **Solution XML (`*.slnx`)** | Ends with **zero** trailing newlines | UTF-8 (No BOM) |
| **Markdown (`*.md`)** | Standard markdown trailing newlines | UTF-8 (No BOM) |

### Format Checking Script
Before finishing your turn or submitting code, you must execute the format checking script to verify compliance:
* **To check formatting status**:
  ```powershell
  powershell -File tests/format-check.ps1
  ```
* **To automatically fix formatting (newline trims & BOM strips)**:
  ```powershell
  powershell -File tests/format-check.ps1 -Fix
  ```

---

## 4. Coding Standards (Modern C# Syntax)

You must use modern C# language features supported by .NET 10. When writing or refactoring code:
* **File-scoped Namespaces**: Use file-scoped namespaces (`namespace FlashcardViewer.Domain;`) rather than block-scoped braces.
* **Primary Constructors**: Use primary constructors for class/struct designs where appropriate.
* **Collection Expressions**: Use collection expressions (`int[] numbers = [1, 2, 3];` or `List<string> list = [];`) rather than verbose initializers.
* **Pattern Matching**: Utilize modern pattern matching matching semantics (such as switch expressions and property patterns) to maintain readable block operations.
* **Expression-bodied Members**: Use expression-bodied definitions for short, single-line methods, properties, and constructors.

---

## 5. Verification Checklists

Before finalizing your work, ensure you satisfy these verification checks:
1. `dotnet build src/FlashcardViewer.Maui/FlashcardViewer.Maui.csproj -f net10.0-windows10.0.26100.0` builds with zero errors.
2. `dotnet run --project tests/FlashcardViewer.ArchitectureTests` runs and passes successfully.
3. `dotnet run --project tests/FlashcardViewer.UnitTests` runs and passes successfully.
4. `powershell -File tests/format-check.ps1` returns exit code 0.
