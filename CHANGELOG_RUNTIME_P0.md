# CHANGELOG: Runtime P0 Fixes

Branch: `fix/runtime-p0-issues`
Ziel: Kritische Laufzeitfehler und Architekturrisiken (P0) aus der Analyse vom 2026-06-21 beheben.

---

## 1. Setup
- Branch `fix/runtime-p0-issues` aus `feature/merged-state-machine-fixes` angelegt.
- Vorhandene unversionierte `.uid`-Dateien, `.csproj.old` und unstaged `.tscn`/`.csproj`-Änderungen identifiziert.

---

## 2. Unversionierte Artefakte aufräumen
- `.uid`-Dateien sind Godot-4.4-Metadaten und gehören nicht in das Repo, solange das Projekt auf Godot 4.3 ausgerichtet ist.
- `TwoWorlds CSharp.csproj.old` ist ein Backup und wird entfernt.
- Unstaged Änderungen an `project.godot`, `.csproj`, `.tscn` stammen vom vorherigen Branch und wurden zurückgesetzt, damit dieser Branch nur die P0-Fixes enthält.
- `.gitignore` um `*.uid` ergänzt.

## 3. P0 Fix: UseToolState direkter CollisionShape-Zugriff
- Datei: `StateMachine/UseToolState.cs`
- Problem: State-Logik griff direkt auf `_HitBoxComponent._CollisionShape2D.Disabled` zu, brach bei fehlender Hitbox oder unsauberer Szene-Struktur.
- Änderung:
  - `GetNodeOrNull` statt `GetNode` für `_HitBoxComponent`.
  - `DeactivateHitBox()` beim `_Ready()` aufrufen.
  - `ActivateHitBox()` / `DeactivateHitBox()` in `Enter()` / `Exit()` verwenden.
  - Null-Check für `_AnimationPlayer` ergänzt.
  - Methode `SetCollisionShapeDisabled` entfernt.
- Begründung: Hitbox-Aktivierung ist Aufgabe der Komponente, nicht des States. Vermeidet Null-Ref und duplizierte Logik.

## 4. P0 Fix: Heilitems heilen den falschen Player
- Dateien:
  - `Items/ItemEffects/ItemEffectResource.cs`
  - `Items/ItemEffects/Heal [Item Effect Resource]/HealItemEffectResource.cs`
  - `Items/Scripts/ItemDataResource.cs`
  - `UI/Inventory/InventorySlotUI.cs`
- Problem: `HealItemEffectResource` nutzte `GameManager.getPlayer()`, der einen nicht-szene-existenten Player liefern kann. Außerdem war nicht klar, wer das Item nutzt.
- Änderung:
  - `ItemEffectResource.Use()` → `ItemEffectResource.Use(Character user)`.
  - `HealItemEffectResource` sucht jetzt beim übergebenen `Character` nach `HealthComponent` und heilt diesen.
  - `ItemDataResource.Use()` → `ItemDataResource.Use(Character user)` gibt den User an alle Effekte weiter.
  - `InventorySlotUI.ItemPressed` holt den aktuellen Player über `GameManager.getPlayer()` und übergibt ihn an `ItemDataResource.Use(user)`.
  - Leere Slots werden bereinigt (`_ItemData = null`, `_Quantity = 0`, Textur geleert).
- Begründung: Items wirken jetzt auf den tatsächlichen Nutzer. Keine stille Heilung eines falschen Players mehr.

## 5. P0 Fix: Verwaiste Signal-Verbindungen in Szenen
- Dateien:
  - `Characters/Player/Player.tscn`
  - `UI/Inventory/Inventory.tscn`
  - `Characters/Enemies/Minotaur/Minotaur.tscn`
- Problem: Drei Szenen hatten Signal-Verbindungen auf Methoden, die im jeweiligen Skript nicht existieren. Das erzeugt Laufzeitfehler oder stille Fehlschläge.
- Entfernte Verbindungen:
  - `Player.tscn`: `AnimationPlayer.animation_finished` → `_FinishedAnimation` (existiert nicht in `Player.cs`)
  - `Inventory.tscn`: `InventoryActive` → `_on_inventory_inventory_active` (existiert nicht)
  - `Minotaur.tscn`: `Enemy Detection Area.area_entered` → `_on_area_entered` (existiert nicht)
- Begründung: Tote Verbindungen bereinigen, damit Szenen sauber laden und keine versteckten Fehler auftreten.

## 6. P0 Fix: Inventory Stapel-Limit und leere Slots
- Dateien:
  - `Items/Scripts/ItemDataResource.cs`
  - `UI/Inventory/Scripts/InventoryDataResource.cs`
  - `UI/Inventory/InventorySlotUI.cs`
- Problem:
  - `InventoryDataResource.addItem` stapelt unbegrenzt in einen Slot.
  - `InventorySlotUI.ItemPressed` reduziert die Menge, leert aber leere Slots nicht sauber.
- Änderung:
  - `ItemDataResource` erhält `[Export] public int _MaxStackSize = 99`.
  - `InventoryDataResource.addItem` beachtet `_MaxStackSize`, füllt vorhandene Stacks auf und erstellt bei Bedarf neue Slots.
  - `InventoryDataResource.addItem` gibt `false` zurück, wenn nicht alles hineinpasst, aber speichert den Teil, der passt.
  - `InventorySlotUI.ItemPressed` setzt bei `_Quantity <= 0` das SlotData auf null und leert die Textur/Anzahl.
- Begründung: Inventar bleibt konsistent, unendliche Stapel werden verhindert, leere Slots werden sauber zurückgesetzt.

## 7. Build-Status
- `dotnet build` erfolgreich.
- 0 Warnungen, 0 Fehler.

