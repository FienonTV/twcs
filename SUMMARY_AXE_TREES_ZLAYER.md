# SUMMARY_AXE_TREES_ZLAYER.md

## Problem
1. Die Axt traf Bäume nicht und konnte diese nicht abbauen.
2. Der Charakter wurde immer vor den Bäumen gerendert, unabhängig von seiner Position.

## Ursache
Die Bäume in der Test-Szene waren **Atlas-Tiles** in einem `TileMapLayer`. Atlas-Tiles haben kein `HurtBoxComponent`, kein `HealthComponent` und werden nicht wie echte Node-Szenen nach Y-Position sortiert. Deshalb wirkten die vorherigen Reparaturen an `small_tree.tscn` / `large_tree.tscn` in dieser Szene nicht.

## Lösung
1. **Axt-Schlag:**
   - `HurtBoxComponent` prüft jetzt korrekt, ob die eintretende Hitbox zu einer Axt gehört (`hitBoxComponent.Tool.HandItemCategory`).
   - Der fehlerhafte Check auf `hitBoxComponent.Owner is HandItem` wurde entfernt.
   - Cooldown-Mechanismus aktiviert, damit ein Baum pro Schwung nur einmal Schaden nimmt.
   - `UseToolState` sucht jetzt auch im Parent des StateMachine-Owners nach der `HitBoxComponent`.
   - Die Baum-Szenen verwenden jetzt `EffectiveItems = [1]` statt `_EffectiveItems = [1]`.
   - Neuer `TreeTileReplacer`: Ersetzt Atlas-Bäume in der Test-Szene automatisch durch echte `small_tree.tscn` / `large_tree.tscn`-Instanzen.

2. **Z-Layer / Y-Sort:**
   - Player bekommt `ZIndex = 0`, `ZAsRelative = true`, `YSortEnabled = true`.
   - Bäume bekommen `z_index = 1` und `y_sort_enabled = true`.
   - Root-Node und `GameTileMap` der Test-Szene bekommen `y_sort_enabled = true`.
   - Damit sortiert Godot Charakter und Bäume korrekt nach ihrer Y-Position.

## Ergebnis
- Build: 0 Warnungen, 0 Fehler.
- Axt sollte Bäume jetzt treffen und Schaden machen.
- Charakter sollte je nach Y-Position vor oder hinter Bäumen gerendert werden.

## Nächster Schritt
Erneuter Play-Test in `Test_Scene_Objects_Trees.tscn`:
- Vor einen Baum stellen, mit der Axt schlagen → Baum sollte wackeln/Schaden nehmen.
- Hinter einen Baum laufen → Charakter sollte hinter dem Baum erscheinen.
- Falls weiterhin Probleme auftreten, Output/Stacktrace kopieren und schicken.
