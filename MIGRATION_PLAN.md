### Migration plan: move from text files to internal SQLite database (EF Core)

### Goal and scope
- Replace file-based storage (`*.txt` + JSON) with a local embedded database.
- Keep WinForms screens working during migration (no big-bang rewrite).
- Solve points 1–4 from the review: typed data model, serializer consistency, path/IO duplication, and update safety.

### Phase 1 — Foundation (low risk, no behavior changes)
1. Add persistence packages
- Add `Microsoft.EntityFrameworkCore`
- Add `Microsoft.EntityFrameworkCore.Sqlite`
- Add `Microsoft.EntityFrameworkCore.Design` (for migrations tooling)

2. Define folder and DB file location
- Keep current app data root (`widmTest` folder), but store `app.db` there.
- Use `Path.Combine(...)` only.

3. Create `AppDbContext`
- Add `DbSet<>` per domain: `Game`, `SettingEntry`, `Question`, `Answer`, `Test`, `TestQuestion`, `TestAnswer`, `Player`, `Opdracht`.
- Configure keys, indexes, required fields, lengths.

4. Add startup initialization
- On app startup, run `Database.Migrate()` (or `EnsureCreated()` initially).
- Keep a small one-time migration marker so legacy import runs once.

### Phase 2 — Data model design (typed, explicit)
1. Create typed entities (replace `List<List<string>>` shape)
- `Question`: `Id`, `Opdracht`, `Text`, `Alphabetical`.
- `Answer`: `Id`, `Opdracht`, `Vraag`, `Name`, `IsCorrect`, `ConnectedPlayersJson` (or join table later).
- `SettingEntry`: `Key`, `Value` (key is PK).
- Others analogous to current data classes.

2. Add constraints/indexes
- Example unique constraints:
  - `Games.Name`
  - `Tests.Name`
  - `Question (Opdracht, Text)`
  - `Answer (Opdracht, Vraag, Name)`
- These directly enforce current `AlreadyExists` checks.

3. Keep compatibility where useful
- For now, keep `ConnectedPlayers` as JSON string field if that simplifies transition.
- Normalize later to relation table once migration is stable.

### Phase 3 — Repository/service layer (anti-corruption boundary)
1. Introduce interfaces
- `IGameStore`, `IQuestionStore`, `IAnswerStore`, `ISettingsStore`, etc.
- This prevents UI/forms from depending on EF directly.

2. Implement EF stores
- Move logic from current classes (`games.cs`, `questions.cs`, `antwoorden.cs`, `settings.cs`) into EF-backed implementations.
- Preserve method contracts initially (`Add...`, `...AlreadyExists`, `Get...`).

3. Add transaction-safe updates
- For operations like “mark one answer correct and others incorrect”, use one transaction / single `SaveChanges`.

### Phase 4 — Legacy data import (one-time migration from txt)
1. Create importer service
- Reads current txt files if they exist.
- Parses JSON into temporary DTOs.
- Maps into entities and inserts/upserts.

2. Idempotency rules
- Import must be safe to rerun (upsert by unique keys).
- Log/skips malformed records instead of crashing whole import.

3. Mark completion
- Store `LegacyImportCompleted=true` in settings table.
- After successful import, app uses DB as source of truth.

### Phase 5 — Incremental class replacement (module-by-module)
1. Replace `Settings` first (smallest surface)
- Removes current `foreach-remove` risk and proves end-to-end flow.

2. Replace `Games` and `Tests`
- Simple lists with uniqueness constraints; low complexity.

3. Replace `Questions` and `Antwoorden`
- Core logic with composite uniqueness and correction flags.

4. Replace remaining classes (`TestVragen`, `TestAntwoorden`, `Spelers`, `Opdrachten`)
- Keep API contracts stable while internals move to DB.

### Phase 6 — API sync adaptation
1. Keep API payload contract initially
- Continue sending/receiving existing JSON schema to avoid server-side breaking changes.

2. Map payload ↔ entities
- In `Api` service, deserialize to DTOs, then upsert into DB.
- Add proper HTTP error handling (`EnsureSuccessStatusCode`, timeout, clear error messages).

3. Add conflict policy
- Decide: remote wins / local wins / timestamp-based.
- Apply consistently per table.

### Phase 7 — Cleanup and hardening
1. Remove legacy file write paths
- After full rollout + validation, stop writing to `games.txt`, `questions.txt`, etc.

2. Standardize JSON library
- Prefer `System.Text.Json` for API/DTO unless special Newtonsoft features are required.

3. Improve string handling and validation
- Use `StringComparison.OrdinalIgnoreCase` for checks.
- Add user-visible validation feedback in forms.

### Phase 8 — Testing strategy
1. Unit tests (logic)
- `AlreadyExists` behavior, setting upsert, “set correct answer” logic.

2. Integration tests (SQLite)
- CRUD for each entity, unique constraint behavior, transactional operations.

3. Migration/import tests
- Verify sample legacy txt data imports correctly and idempotently.

4. API sync tests
- Payload mapping tests for both outbound and inbound flows.

### Suggested rollout order (practical)
1. Add EF Core + DB context.
2. Implement `SettingEntry` + `Settings` migration.
3. Implement `Games` + `Tests`.
4. Implement `Questions` + `Answers`.
5. Integrate importer and run once on startup.
6. Migrate API sync to DB-backed services.
7. Remove old file persistence.

### Definition of done
- App works without any dependency on text files for runtime persistence.
- Existing user data is imported automatically and safely.
- Core create/read/update flows work in UI unchanged.
- API sync still works with current server contract.
- Basic unit + integration tests pass for migrated modules.