# CHANGELOG_EDITOR_INVENTORY_FIX.md

## Branch
`fix/editor-missing-script-references`

## Auslöser
Play-Test im Godot-Editor zeigte nach dem Start sofort rote Fehler im Output:
- `Cannot open file 'res://UI/Inventory/inventory_menu.cs'`
- `Cannot open file 'res://Characters/Character Components/CharacterMovementComponent.cs'`

Im Anschluss war das Inventar:
- immer sichtbar / im Vordergrund
- voll mit 99er-Stacks
- nicht schließbar

## Ziel
1. Verwaiste Skript-Referenzen in `.tscn`-Dateien bereinigen.
2. Inventar-System so umbauen, dass es im Editor sauber startet, initial geschlossen ist, leer ist und sich über die `inventory`-Aktion sowie `ui_cancel` öffnen/schließen lässt.

## Änderungen

### Verwaiste Skript-Referenzen und UIDs
- `UI/Inventory/Inventory.tscn`
  - ExtResource `res://UI/Inventory/inventory_menu.cs` entfernt.
  - Root-Node `InventoryMenu` bekommt `InventoryMenu.cs` als Skript.
  - `load_steps` von 8 auf 7 korrigiert.
  - Szene startet jetzt unsichtbar (`visible = false`).
  - UIDs für `InventoryMenu.cs` und `InventoryUI.cs` auf die aktuellen `.uid`-Dateien korrigiert.
- `UI/Inventory/inventory_slot.tscn`
  - UID für `InventorySlotUI.cs` auf die aktuelle `.uid`-Datei korrigiert.
- `Characters/Enemies/Minotaur/Minotaur.tscn`
  - Verwaiste Node `CharacterMovementComponent` und zugehörige ExtResource entfernt.
  - `load_steps` entsprechend angepasst.

### Inventar-System
- `UI/Inventory/InventoryMenu.cs`
  - Entfernt: Signal-basierte Kopplung zu `InventoryUI`.
  - Neu: Direkte Steuerung von `_InventoryPanel.Visible`.
  - Auto-Lookup für `_InventoryUI`, `_InventoryPanel` und `_ItemDescriptionLabel` via `FindChild`/`GetNodeOrNull`.
  - `ProcessMode = Always`, damit Eingaben auch bei pausiertem Spiel verarbeitet werden.
  - `HideInventory()` beim `_Ready` aufrufen.
  - `ui_cancel` schließt das Inventar und markiert Eingabe als verarbeitet.
  - `UpdateItemDescription` / `UpdateItemName` (PascalCase).
  - Neue Property `InventoryData` für die aktiven Inventardaten.
- `UI/Inventory/Scripts/InventoryUI.cs`
  - `_Data` ist jetzt `public`, damit `Player` das Inventar nach der Registrierung zuweisen kann.
  - Referenz auf `InventoryMenu` über `Services.Get<InventoryMenu>()` statt über `Owner`.
  - `ClearInventory()` beim Start aufrufen.
- `UI/Inventory/InventorySlotUI.cs`
  - Benennung auf `_TextureRect` / `_QuantityLabel` konsistent.
  - Leere/ungültige Slots zeigen keine Textur und keine Menge an.
  - `ItemName` wird aus `ItemData.ItemName` gelesen (vorher nicht existente `Name`-Property).
  - `UpdateItemDescription` / `UpdateItemName` nutzt PascalCase-Methoden.
- `UI/Inventory/Inventory.tscn`
  - Root-Node umbenannt in `InventoryMenu` und mit `InventoryMenu.cs` verknüpft.
  - `Control`-Node startet unsichtbar.
  - 28 statische `InventorySlot`-Kinder aus `GridContainer` entfernt (werden dynamisch erzeugt).
  - Neues `ItemName`-Label hinzugefügt.
- `UI/Inventory/inventory_slot.tscn`
  - Default-Textur entfernt.
  - Default-Text "99" entfernt.
  - Leere Slots sehen jetzt tatsächlich leer aus.
- `UI/Inventory/Player_Inventory.tres`
  - `Slots = null` entfernt.
  - `Slots` jetzt als Array mit 28 `null`-Einträgen initialisiert (leeres Inventar).
- `Characters/Player/Player.cs`
  - `InventoryData` ist jetzt `[Export]`-Feld und public.
  - Player registriert sich bei `InventoryMenu.CurrentUser`.
  - Falls keine `InventoryData` zugewiesen ist, wird ein leeres 28-Slot-Inventar erzeugt.
  - `InventoryUI._Data` wird auf das Player-Inventar gesetzt.

## Build
`dotnet build 'TwoWorlds CSharp.csproj' -v quiet`
- 0 Warnungen
- 0 Fehler

## Verifizierte Dateien
- `UI/Inventory/Inventory.tscn`
- `UI/Inventory/InventoryMenu.cs`
- `UI/Inventory/Scripts/InventoryUI.cs`
- `UI/Inventory/InventorySlotUI.cs`
- `UI/Inventory/inventory_slot.tscn`
- `UI/Inventory/Player_Inventory.tres`
- `Characters/Player/Player.cs`
- `Characters/Enemies/Minotaur/Minotaur.tscn`
