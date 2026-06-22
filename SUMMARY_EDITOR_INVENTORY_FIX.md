# SUMMARY_EDITOR_INVENTORY_FIX.md

## Problem
Der Editor-Play-Test startete nicht sauber. Zwei gelöschte Skripte wurden noch von Szenen referenziert, und das Inventar war sichtbar, voll mit 99er-Stacks und nicht schließbar.

## Lösung
1. Verwaiste Skript-Referenzen in `Inventory.tscn` und `Minotaur.tscn` entfernt.
2. Inventar-UI komplett neu verdrahtet:
   - `InventoryMenu` ist jetzt das aktive Skript der Autoload-Szene.
   - Panel startet unsichtbar und kann über `inventory`-Taste (Esc) sowie `ui_cancel` (Esc) getoggelt werden.
   - Statische Slot-Nodes entfernt; Slots werden dynamisch aus `InventoryDataResource.Slots` erzeugt.
   - `Player_Inventory.tres` enthält jetzt 28 leere Slots statt `null`.
   - Player meldet sich als Inventar-Besitzer und stellt (oder erzeugt) die `InventoryData`.
   - Leere Slots zeigen keine Textur und keine Mengenzahl an.

## Ergebnis
- Build: 0 Warnungen, 0 Fehler.
- Das Inventar sollte nun:
  - beim Start geschlossen sein,
  - leer sein,
  - mit der `inventory`-Taste geöffnet und geschlossen werden,
  - mit `ui_cancel` geschlossen werden.

## Nächster Schritt
Erneuter Play-Test im Godot-Editor. Falls weitere Bugs auftauchen, Stacktrace/Output kopieren und mir schicken.
