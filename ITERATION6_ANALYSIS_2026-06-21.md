# Iterations-Analyse #6: Finale Laufzeit-Risiken und API-Konsistenz

Erstellt: 2026-06-21
Repository: https://github.com/FienonTV/twcs
Branch: fix/iteration-6-node-inspection
Build: 0 Warnungen, 0 Fehler

---

## 1. Zusammenfassung

Iteration 6 ist die letzte Runde vor dem Abbruch des Workflows. Fokus: verbleibende Laufzeit-Risiken (NRE), fehlende Export-Attribute, API-Konsistenz.

---

## 2. Statistiken

| Metrik | Anzahl |
|---|---|
| C#-Dateien | 57 |
| `GD.Print` | 2 (nur Logger) |
| `async void` | 0 |
| `FindChild` (Fallback) | 11 |
| `GetNodeOrNull` (Fallback) | 9 |
| Singleton-Pfad-Lookups | 1 (absichtlich in Services) |
| Öffentliche `_`-Felder | 0 |
| Klassen mit Kleinbuchstaben | 0 |

---

## 3. P1 — Verbleibende Laufzeit-Risiken

1. `SmallTree.cs`: `HurtBoxComponent` via `FindChild`, sollte `[Export]` sein.
2. `Interaction_Area.cs`: `InteractLabel` als öffentliches Feld, kein Export.
3. `Chest.cs`: `InteractionLabel` als öffentliches Feld, kein Export.
4. `HitBoxComponent.cs`: Public Felder `IsActive`, `Tool`, `OwnerCharacter`, `CollisionShape` — teilweise internal verwendet, teilweise external.

## 4. P2 — API-Konsistenz

1. `HandItem.cs`: `Damage` und `HitBoxComponent` als public Felder.
2. `ItemDataResource.cs`: Public Felder statt Properties — bei Resources akzeptabel, aber `ItemName` statt `Name` vermeidet Konflikt.
3. `Character.cs`: `CurrentLookingDirection` public Feld, NavigationAgent2D public Export.

## 5. Fazit

Dies ist die letzte Iteration. Nach den Fixes wird geprüft, ob noch neue Fehler auftauchen. Wenn nicht, endet der Workflow.
