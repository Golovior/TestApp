# TestApp sync API contract

This documents the wire format the .NET client (`TestApp/Api.cs`) sends to and expects back from `POST {baseUrl}SyncTestAppData` (currently `http://widmtimer.fvandenberg.nl/api/SyncTestAppData`). It **replaces** the previous text-tuple / double-JSON-encoded format entirely — there is no version negotiation, so client and server must be updated together.

Serialized with `System.Text.Json.JsonSerializer.Serialize(...)` using default options: property names are **exact PascalCase C# property names** (no camelCase conversion), `Guid` serializes as the standard hyphenated lowercase string form, `bool` as JSON `true`/`false`, `DateTime`/`DateTime?` as ISO-8601 round-trip strings (produced by `DateTime.Now`, so — be aware — **not** UTC, no explicit offset).

## Envelope shape (identical for request and response)

```json
{
  "Games": [ { "Id": "3fa85f64-5717-4562-b3fc-2c963f66afa6", "Name": "Spel1", "UpdatedAtUtc": 1751900000 } ],
  "Settings": [ { "Key": "ActiveGame", "Value": "Spel1", "UpdatedAtUtc": 1751900000 } ],
  "Opdrachten": [ { "Id": "...", "Name": "Opdracht1", "UpdatedAtUtc": 1751900000 } ],
  "Questions": [ { "Id": "...", "OpdrachtId": "...", "Text": "Vraag1", "Alphabetical": "0", "UpdatedAtUtc": 1751900000 } ],
  "Answers": [ { "Id": "...", "QuestionId": "...", "Name": "AntwoordA", "IsCorrect": true, "ConnectedPlayersJson": "[\"Speler1\"]", "UpdatedAtUtc": 1751900000 } ],
  "Tests": [ { "Id": "...", "Name": "Test1", "GameId": "...", "UpdatedAtUtc": 1751900000 } ],
  "TestQuestions": [ { "Id": "...", "TestId": "...", "QuestionId": "...", "Order": "1", "UpdatedAtUtc": 1751900000 } ],
  "Players": [ { "Id": "...", "Name": "Speler1", "UpdatedAtUtc": 1751900000 } ],
  "GameSpelers": [ { "Id": "...", "GameId": "...", "SpelerId": "...", "Status": "1", "UpdatedAtUtc": 1751900000 } ],
  "TestAfnamen": [ { "Id": "...", "TestId": "...", "SpelerId": "...", "Starttijd": "2026-07-08T14:23:01.1234567", "Eindtijd": null, "Jokers": null, "UpdatedAtUtc": 1751900000 } ],
  "TestAnswers": [ { "Id": "...", "TestAfnameId": "...", "TestQuestionId": "...", "AnswerId": "...", "UpdatedAtUtc": 1751900000 } ]
}
```

Every table's `Id` (or `Key`, for `Settings`) is the row's real primary key from the SQLite schema (`AppEntities.cs`/`AppDbContext.cs`). Foreign keys (`OpdrachtId`, `QuestionId`, `GameId`, `TestId`, `SpelerId`, `TestAfnameId`, `TestQuestionId`, `AnswerId`) are the real referenced row's `Id` — no more name-based lookups.

`GameId` on `Tests` and `Eindtijd`/`Jokers` on `TestAfnamen` are nullable — expect JSON `null`, not an omitted field or empty string.

`ConnectedPlayersJson` on `Answers` is unchanged from before: a JSON-encoded string containing a list of player **names** (not ids) — this one field is intentionally out of scope for the id-based redesign.

## Semantics

- **`UpdatedAtUtc`** is Unix **seconds** (not milliseconds), maintained by `RecordSyncHelper` and touched on every mutating write. Missing/unset defaults to `0`.
- **Last-write-wins per row**: the client only applies an incoming row if `incoming.UpdatedAtUtc >= <its own last-known UpdatedAtUtc for that row>` (or applies unconditionally if it's never seen that Id before). The server should implement the same rule when deciding whether to accept a row from the client's request — don't blindly overwrite newer server data with an older client value.
- **No deletions**: this is a pure upsert protocol in both directions — there's no tombstone/delete propagation. A row removed from one side won't be removed from the other via sync.
- **Apply in dependency order** (both sides must respect this, since foreign keys are enforced): `Settings`, `Opdrachten`, `Games`, `Players` first (no dependencies) → `Questions` (needs Opdracht), `Tests` (needs Game, nullable) → `Answers` (needs Question), `TestQuestions` (needs Test+Question), `GameSpelers` (needs Game+Player) → `TestAfnamen` (needs Test+Player) → `TestAnswers` (needs TestAfname+TestQuestion+Answer).
- **`Settings` export excludes internal bookkeeping**: the client stores its own per-record sync timestamps as rows in this same table, keyed like `SyncRecord:<table>:<base64>`. These are filtered out of `GetForSync()`/never appear in the outgoing `Settings` array — the server should never receive or need to handle a `Key` with that prefix.
- The client throws instead of applying anything if the raw response body's leading text starts with the literal string `"Error"` (case-insensitive) — this predates the redesign and still applies.
