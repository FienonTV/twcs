# CHANGELOG: Services, Logger & Convention Cleanup

Branch: `fix/cleanup-services-logger`
Ziel: Verbleibende P1/P2/P3-Schwachstellen aus der erweiterten Analyse vom 2026-06-21 lösen.

---

## 1. GameManager nicht mehr statisch
- Datei: `GameManager.cs`
- `GameManager` ist jetzt ein Autoload-Node, der via `/root/GameManager` gefunden wird.
- `PlayerScene` ist eine öffentliche Property.
- `Player` und `HasPlayer` sind Instanz-Member.
- `RegisterPlayer` ist eine Instanz-Methode.
- Aufrufer angepasst:
  - `WorldInitialization.cs`
  - `FollowPlayerBehavior.cs`
  - `Player.cs`

## 2. ResourcePaths eingeführt
- Datei: `ResourcePaths.cs`
- Zentrale Konstanten für hartcodierte Pfade:
  - `PlayerScene`
  - `InventoryTres`
  - `InventorySlotScene`
  - `InventoryMenuAutoload`
  - `LogScene`
- Verwendet in:
  - `GameManager.cs`
  - `Player.cs`
  - `SmallTree.cs`
  - `InventorySlotUI.cs`

## 3. Logger eingeführt
- Datei: `Logger.cs`
- Ebenen: Debug, Info, Warning, Error.
- `Debug` wird in Release-Builds übersprungen.
- Viele `GD.Print`/`GD.PrintErr` durch Logger-Aufrufe ersetzt.

## 4. InventorySlotUI entkoppelt
- Datei: `InventorySlotUI.cs`
- Nutzt nicht mehr `GameManager.getPlayer()`.
- `SlotDataResource.User` enthält den Nutzer (z.B. Player).
- `InventoryUI.cs` setzt `SlotData.User` beim Öffnen auf `inventory_menu.CurrentUser`.
- `inventory_menu.cs` hat `CurrentUser { get; set; }`.

## 5. async void in SmallTree entfernt
- Datei: `SmallTree.cs`
- `ReceiveDamage` ist jetzt `void`.
- Shake-Reset läuft über `Tween` statt `await ToSignal(...)`.

## 6. State-Registry auf StateKey umgestellt
- Datei: `StateMachine/CharacterState.cs`
- Neue virtuelle Property `StateKey => Name`.
- `CharacterStateMachine.BuildStateRegistry` verwendet `state.StateKey` statt `child.Name`.
- States können ihre Keys überschreiben, falls Node-Namen in Szenen nicht exakt passen.

## 7. Konventionen und Field-Zugriffe bereinigt
- `Character._CurrentLookingDirection` -> `Character.CurrentLookingDirection`
- `Character._HealthComponent` -> `Character.HealthComponent`
- `CharacterStateMachine._AnimationPlayer` -> `CharacterStateMachine.AnimationPlayer` (Property)
- `CharacterState._StateMachine` -> `CharacterState.StateMachine`
- Reduziert harte Kopplung zwischen States und StateMachine-Interna.

## 8. Aufräumarbeiten
- Lokale `.uid`-Dateien entfernt.
- `TwoWorlds CSharp.csproj.old` entfernt.
- Alte Protokoll-/Analyse-Dateien aus früheren Branches entfernt.
- Neue Protokolle: `CHANGELOG_CLEANUP_SERVICES_LOGGER.md`, `SUMMARY_CLEANUP_SERVICES_LOGGER.md`.

## Build-Status
- `dotnet build 'TwoWorlds CSharp.csproj'`
- 0 Warnungen, 0 Fehler.
