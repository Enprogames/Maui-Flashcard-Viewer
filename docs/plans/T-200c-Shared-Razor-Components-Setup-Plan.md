# T-200c Shared Razor Components Setup Plan

**Status:** In Progress
**Priority:** P0
**Related Bug:** N/A
**Related Plans:** `T-200-ARCH-Clean-Architecture-Blazor-Hybrid-Plan.md`

---

## 1. Problem Statement
The user interface is currently written using native .NET MAUI XAML (`ContentPage` and `ContentView`). To support running the application on both native platforms (Windows, macOS, iOS, Android) and the Web (Blazor WebAssembly), we need to migrate the entire UI layer into a reusable **Razor Class Library (RCL)**.

We will rebuild the views as platform-agnostic Razor Components (`.razor`) styled with Vanilla CSS. These components will bind to application-level state/services instead of platform-specific view models.

## 2. Goals
1. Create `FlashcardViewer.SharedUI` inside `src/` to serve as the shared UI container.
2. Establish a responsive layout (`MainLayout.razor`) and styling system (`app.css`) with premium variables (light/dark mode colors) and smooth animations (e.g. card flips).
3. Port the following XAML views to Razor Components:
   * `FlashcardSetListPage` $\rightarrow$ `FlashcardSetList.razor`
   * `FlashcardListPage` $\rightarrow$ `FlashcardList.razor`
   * `FlashcardSessionPage` $\rightarrow$ `FlashcardSession.razor`
   * `SessionConfigManagementPopup` $\rightarrow$ `SessionConfigPopup.razor`
4. Implement clean CSS transitions for the interactive card-flipping page instead of relying on manual XAML animations.
5. Create TUnit verification tests to assert state transitions and UI helper behaviors.

## 3. Non-Goals
1. Do not touch native platform bootstrapping or shell files (left for Slice 4).
2. Do not introduce TailwindCSS (maintain Vanilla CSS for animations and layout).

## 4. Plan Checklist

### Slice T-200c1: Shared UI Structure, CSS Tokens & Layout Setup
- [ ] **T-200c-1.1**: Create Razor Class Library `FlashcardViewer.SharedUI` (`net10.0`) under `/src`:
  * Command: `dotnet new razorclasslib -n FlashcardViewer.SharedUI -f net10.0 -o src/FlashcardViewer.SharedUI`
- [ ] **T-200c-1.2**: Add project references:
  * Reference `FlashcardViewer.Application` inside `FlashcardViewer.SharedUI`.
- [ ] **T-200c-1.3**: Configure `FlashcardViewer.slnx` to include `src/FlashcardViewer.SharedUI/FlashcardViewer.SharedUI.csproj`.
- [ ] **T-200c-1.4**: Establish the global layout `SharedUI/MainLayout.razor` and global styling sheet `SharedUI/wwwroot/app.css` containing premium dark/light mode CSS tokens and animation keyframes.

### Slice T-200c2: Set List View Porting
- [ ] **T-200c-2.1**: Create `FlashcardSetList.razor` porting the layout from `FlashcardSetListPage.xaml`.
- [ ] **T-200c-2.2**: Integrate state bindings connecting the repository interfaces to the set list page (handling add, delete, and bulk selection).
- [ ] **T-200c-2.3**: Write TUnit verification tests validating set selection calculations.

### Slice T-200c3: Card List View Porting
- [ ] **T-200c-3.1**: Create `FlashcardList.razor` porting layout from `FlashcardListPage.xaml`.
- [ ] **T-200c-3.2**: Hook up form fields to update card questions and answers dynamically.
- [ ] **T-200c-3.3**: Write TUnit verification tests checking list state logic.

### Slice T-200c4: Flashcard Session View Porting
- [ ] **T-200c-4.1**: Create `FlashcardSession.razor` porting layout from `FlashcardSessionPage.xaml`.
- [ ] **T-200c-4.2**: Implement state-driven card progression rules (handling next/previous card actions, shuffle logic, and text-to-speech triggers).
- [ ] **T-200c-4.3**: Write HTML/CSS card structures implementing a 3D-transform flip transition (`transform: rotateY(180deg)`).
- [ ] **T-200c-4.4**: Write TUnit verification tests checking session progression indices.

### Slice T-200c5: Session Configuration Popup Porting
- [ ] **T-200c-5.1**: Create `SessionConfigPopup.razor` porting settings overlays from `SessionConfigManagementPopup.xaml`.
- [ ] **T-200c-5.2**: Bind switches to autoplay, read aloud, shuffle, and display direction preferences.
- [ ] **T-200c-5.3**: Write TUnit verification tests checking config state validation.

---

## 5. Verification

1. **Format Compliance**: Ensure all C# and Razor files end with a single newline, and all `.csproj` files end with zero newlines (validated by `powershell -File tests/format-check.ps1`).
2. **Compilation Verification**: Build `src/FlashcardViewer.SharedUI` using `dotnet build` to confirm zero compilation errors.
3. **Execution of tests**: Run `dotnet test` (via console test project) to verify all unit/layout tests pass.
