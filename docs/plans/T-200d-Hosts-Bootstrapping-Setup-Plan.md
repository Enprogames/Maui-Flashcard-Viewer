# T-200d Hosts Bootstrapping & Platform Integration Plan

**Status:** Proposed
**Priority:** P0
**Related Bug:** N/A
**Related Plans:** `T-200-ARCH-Clean-Architecture-Blazor-Hybrid-Plan.md`

---

## 1. Problem Statement
With the UI pages successfully ported to Razor Components in `FlashcardViewer.SharedUI`, we need to bootstrap our hosting environments:
1. **Desktop/Mobile (Blazor Hybrid)**: Migrate the legacy native MAUI project to use `BlazorWebView` and run the Blazor-based UI inside a native container.
2. **Web (Blazor WebAssembly)**: Create a static SPA client project that runs entirely in-browser.

Both hosts will reference `FlashcardViewer.SharedUI` for the UI, but will inject their respective storage layers at startup (SQLite for MAUI, LocalStorage/Web for WASM).

---

## 2. Goals
1. Relocate and refactor the MAUI project to `/src/FlashcardViewer.Maui`:
   * Set up `<BlazorWebView>` in `MainPage.xaml`.
   * Configure `MauiProgram.cs` to register SQLite infrastructure services and Blazor Hybrid dependencies.
   * Remove old views, viewmodels, and speech readers from the MAUI project.
2. Create the Blazor WebAssembly project `/src/FlashcardViewer.Wasm`:
   * Set up static host hosting `index.html` and booting the WASM client.
   * Register Web storage repositories and standard HttpClient/Blazor dependencies.
3. Add `App.razor` (routing shell) in `FlashcardViewer.SharedUI` to enable routing reuse.
4. Update solution configuration files to build both host frameworks cleanly.
5. Verify build, restore, and launch success on Windows and Web hosts.

---

## 3. Non-Goals
1. Do not add any new application screens or styling (rely on `SharedUI`).
2. Do not modify domain model properties or persistence interfaces.

---

## 4. Plan Checklist

### Slice T-200d1: Shared Router Setup
- [ ] **T-200d-1.1**: Create `App.razor` inside `src/FlashcardViewer.SharedUI` to serve as the shared Blazor Router.
- [ ] **T-200d-1.2**: Register `SessionStateService` inside the host projects' Dependency Injection containers.

### Slice T-200d2: MAUI Blazor Hybrid Host Bootstrapping
- [ ] **T-200d-2.1**: Move the current `FlashcardViewer/` folder to `src/FlashcardViewer.Maui/`.
- [ ] **T-200d-2.2**: Update `src/FlashcardViewer.Maui/FlashcardViewer.Maui.csproj`:
  * Rename project namespace, adjust project/assembly names.
  * Target frameworks: `net10.0-maccatalyst`, `net10.0-android36.0`, and `net10.0-windows10.0.26100.0`.
  * Add package reference: `Microsoft.AspNetCore.Components.WebView.Maui` version `10.0.70`.
  * Reference project `FlashcardViewer.SharedUI` and `FlashcardViewer.Infrastructure.Sqlite`.
- [ ] **T-200d-2.3**: Replace `MainPage.xaml` with a clean layout containing `<BlazorWebView>` pointing to `wwwroot/index.html`.
- [ ] **T-200d-2.4**: Create `wwwroot/index.html` inside the MAUI project containing the host viewport, stylesheet links, and loading states.
- [ ] **T-200d-2.5**: Update `MauiProgram.cs`:
  * Add `.AddMauiBlazorWebView()` and `.AddBlazorWebViewDeveloperTools()`.
  * Register `SqliteFlashcardRepository` as the `IFlashcardRepository` implementation.
  * Register `SqliteDatabaseInitializer` as `IDatabaseInitializer` and execute initialization.
- [ ] **T-200d-2.6**: Delete legacy views (`Views/`), viewmodels (`ViewModels/`), models (`Models/`), and old services from `FlashcardViewer.Maui` to keep the host project slim.

### Slice T-200d3: Blazor WebAssembly Host Bootstrapping
- [ ] **T-200d-3.1**: Create `src/FlashcardViewer.Wasm` Blazor WebAssembly project:
  * Command: `dotnet new blazorwasm -n FlashcardViewer.Wasm -f net10.0 -o src/FlashcardViewer.Wasm`
  * Add project references: `FlashcardViewer.SharedUI` and `FlashcardViewer.Infrastructure.Web`.
- [ ] **T-200d-3.2**: Configure `Program.cs` in `FlashcardViewer.Wasm`:
  * Register `WebFlashcardRepository` as the `IFlashcardRepository` implementation.
  * Configure HttpClient and register standard services.
- [ ] **T-200d-3.3**: Create index template `wwwroot/index.html` inside the WASM project linking to `_content/FlashcardViewer.SharedUI/app.css`.

### Slice T-200d4: Solution Alignment
- [ ] **T-200d-4.1**: Update `FlashcardViewer.slnx`:
  * Remove `FlashcardViewer/FlashcardViewer.csproj`.
  * Add `src/FlashcardViewer.Maui/FlashcardViewer.Maui.csproj` and `src/FlashcardViewer.Wasm/FlashcardViewer.Wasm.csproj`.
- [ ] **T-200d-4.2**: Verify clean formatting using `powershell -File tests/format-check.ps1 -Fix`.

---

## 5. Verification

1. **Compilation Checks**:
   * Build MAUI host: `dotnet build src/FlashcardViewer.Maui/FlashcardViewer.Maui.csproj -f net10.0-windows10.0.26100.0`
   * Build WASM host: `dotnet build src/FlashcardViewer.Wasm/FlashcardViewer.Wasm.csproj`
2. **Unit & Architecture Tests**: Run `dotnet test FlashcardViewer.slnx` to ensure all tests continue to pass.
3. **Execution Checks**:
   * Run MAUI windows host: `dotnet run --project src/FlashcardViewer.Maui/FlashcardViewer.Maui.csproj -f net10.0-windows10.0.26100.0`
   * Run WASM web host: `dotnet run --project src/FlashcardViewer.Wasm/FlashcardViewer.Wasm.csproj`
