# SUMMARY_AXE_TREES_ZLAYER.md

## Problem
1. Die Axt traf Bäume nicht und konnte diese nicht abbauen.
2. Der Charakter wurde immer vor den Bäumen gerendert, unabhängig von seiner Position.

## Lösung
1. **Axt-Schlag:**
   - `HurtBoxComponent` prüft jetzt korrekt, ob die eintretende Hitbox zu einer Axt gehört (`hitBoxComponent.Tool.HandItemCategory`).
   - Der fehlerhafte Check auf `hitBoxComponent.Owner is HandItem` wurde entfernt.
   - Cooldown-Mechanismus aktiviert, damit ein Baum pro Schwung nur einmal Schaden nimmt.
   - Die Baum-Szenen verwenden jetzt `EffectiveItems = [1]` statt `_EffectiveItems = [1]`.

2. **Z-Layer / Y-Sort:**
   - Player bekommt `ZIndex = 0`, `ZAsRelative = true`, `YSortEnabled = true`.
   - Bäume bekommen `z_index = 1` und `y_sort_enabled = true`.
   - Damit sortiert Godot Charakter und Bäume korrekt nach ihrer Y-Position.

## Ergebnis
- Build: 0 Warnungen, 0 Fehler.
- Axt sollte Bäume jetzt treffen und Schaden machen.
- Charakter sollte je nach Y-Position vor oder hinter Bäumen gerendert werden.

## Nächster Schritt
Erneuter Play-Test:
- Vor einen Baum stellen, mit der Axt schlagen → Baum sollte wackeln/Schaden nehmen.
- Hinter einen Baum laufen → Charakter sollte hinter dem Baum erscheinen.
- Falls weiterhin Probleme auftreten, Output/Stacktrace kopieren und schicken.
