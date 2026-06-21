# SUMMARY: Runtime P0 Fixes

Branch: `fix/runtime-p0-issues`
Status: Abgeschlossen
Build: 0 Warnungen, 0 Fehler

---

## Ziel
Kritische Laufzeitfehler und Architekturrisiken (P0) aus der Analyse vom 2026-06-21 beheben.

---

## Durchgeführte Änderungen

### 1. Repository-Sauberkeit
- `.gitignore` um `*.uid` ergänzt (Godot 4.4 Metadaten).
- Alle `.uid`-Dateien und `TwoWorlds CSharp.csproj.old` entfernt.
- Unstaged Änderungen vom vorherigen Branch zurückgesetzt.

### 2. `UseToolState` robust gemacht
- `StateMachine/UseToolState.cs` nutzt jetzt `HitBoxComponent.ActivateHitBox()` / `DeactivateHitBox()`.
- Kein direkter Zugriff mehr auf `_CollisionShape2D.Disabled`.
- Null-Checks für `_AnimationPlayer` und `_HitBoxComponent` ergänzt.

### 3. Heilitems zielorientiert
- `ItemEffectResource.Use()` → `Use(Character user)`.
- `ItemDataResource.Use()` → `Use(Character user)`.
- `HealItemEffectResource` sucht die `HealthComponent` am übergebenen `Character` und heilt diesen.
- `InventorySlotUI.ItemPressed` übergibt den aktuellen Player an das Item.

### 4. Verwaiste Signal-Verbindungen entfernt
- `Characters/Player/Player.tscn`: `_FinishedAnimation`
- `UI/Inventory/Inventory.tscn`: `_on_inventory_inventory_active`
- `Characters/Enemies/Minotaur/Minotaur.tscn`: `_on_area_entered`

### 5. Inventar konsistent gemacht
- `ItemDataResource` erhält `_MaxStackSize = 99`.
- `InventoryDataResource.addItem` beachtet Stapel-Limit, füllt auf und erstellt neue Slots.
- `InventorySlotUI.ItemPressed` leert leere Slots sauber (`_ItemData = null`, Textur/Anzahl zurückgesetzt).

---

## Build

```
Der Buildvorgang wurde erfolgreich ausgeführt.
    0 Warnung(en)
    0 Fehler
```

---

## Offene Punkte (P1/P2/P3)

Folgende Punkte aus der Analyse wurden in diesem Branch absichtlich **nicht** angefasst, da sie keinen direkten Laufzeitabsturz verursachen:

- `WalkState` trennt noch Player/NPC über `Owner.GetType()`.
- `ToolStateMachine` sucht Owner-StateMachine über Baum-Suche (`Owner.Owner`).
- `HitBoxComponent`/`AttackComponent` sind noch an `Player`/`Sword` gekoppelt.
- `GameManager` hält weiterhin globale Player-Referenz.
- `NPCStateMachine` läuft in `_Process` statt `_PhysicsProcess`.
- Toter Code (`NPCInputHandler.cs`, auskommentierte Zeilen) ist noch vorhanden.
- Namenskonventionen (`_`-Prefix) und `Nullable` wären noch zu modernisieren.

Diese Punkte sollten in separaten Branches angegangen werden.

---

## Verwandte Dokumente

- `CHANGELOG_RUNTIME_P0.md` — detailliertes Änderungsprotokoll
- `ARCHITECTURE_RECOMMENDATIONS_2026-06-21.md` — Ausgangs-Architekturanalyse

