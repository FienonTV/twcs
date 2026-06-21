# Änderungsprotokoll: State-Machine-Fix

> Branch: fix/state-machine
> Start: 2026-06-21
> Ziel: Zwei parallele State-Machine-Systeme bereinigen, Lifecycle korrigieren, State-Machine stabilisieren

---

## Regeln für dieses Protokoll

- Jede Änderung bekommt eine fortlaufende Nummer.
- Zu jeder Änderung gehören: Datei(en), Beschreibung, Begründung.
- Geänderte Code-Stellen werden mit Datei und Zeilenbereich notiert (sofern sinnvoll).
- Rückfragen an den Nutzer werden hier dokumentiert.

---

## Protokoll

| Nr. | Datei(en) | Beschreibung | Begründung |
|-----|-----------|--------------|------------|
| 1 | `StateMachine/CharacterStateMachine.cs` (vorher `New State Machine/newStateMachine.cs`) | Umbenennung der Klasse `newStateMachine` → `CharacterStateMachine` und aller zugehörigen Feld-/Variablennamen. | Konsistente, aussagekräftige PascalCase-Namen ohne `new…`-Präfix; Vorgabe aus Aufgabenstellung. |
| 2 | `StateMachine/CharacterState.cs` (vorher `New State Machine/newState.cs`) | Umbenennung der Klasse `newState` → `CharacterState`. | Gleiche Begründung wie Nr. 1. |
| 3 | `StateMachine/IdleState.cs`, `StateMachine/WalkState.cs`, `StateMachine/UseToolState.cs`, `StateMachine/FollowState.cs` | Umbenennung der Klassen `newIdleState` → `IdleState`, `newWalkState` → `WalkState`, `newUseToolState` → `UseToolState`, `newFollowState` → `FollowState` sowie aller Vererbungs-/Typ-Bezüge. | Einheitliche, sprechende State-Namen. |
| 4 | `StateMachine/PlayerStateMachine.cs`, `StateMachine/NPCStateMachine.cs`, `StateMachine/ToolStateMachine.cs` | Basisklasse in `CharacterStateMachine` / `CharacterState` angepasst. | Spezialisierte State Machines müssen mit der neuen Hierarchie kompilieren. |
| 5 | `StateMachine/CharacterStateMachine.cs` Zeile 33–48 | Lifecycle korrigiert: `Exit()` des aktuellen States wird **vor** `Enter()` des neuen States aufgerufen. | Bisher lief `Enter()` vor `Exit()`, was Ressourcen-/Animation-Konflikte verursachen kann. |
| 6 | `StateMachine/IdleState.cs`, `StateMachine/WalkState.cs`, `StateMachine/UseToolState.cs` | Entfernung aller `Owner.SetPhysicsProcess(true/false)`-Aufrufe. | `SetPhysicsProcess` soll nur auf dem State-Node selbst wirken; dies erledigt bereits `CharacterState.Enter()`/`Exit()`. Doppelte Steuerung des Owners vermeiden. |
| 7 | `Characters/Character.cs` | Kommentierter Typ `newStateMachine` → `CharacterStateMachine`. | Verwaister Verweis im Base-Character bereinigt. |
| 8 | `Characters/Player/Player.tscn` | Skript-Pfade von `New State Machine/…` auf `StateMachine/…` umgestellt (`PlayerStateMachine.cs`, `IdleState.cs`, `WalkState.cs`). | .tscn muss zur neuen Ordner-/Klassen-Struktur passen. |
| 9 | `Tools/Sword/Sword.tscn` | Skript-Pfade auf `StateMachine/ToolStateMachine.cs`, `StateMachine/IdleState.cs`, `StateMachine/UseToolState.cs` angepasst. | Gleiche Begründung wie Nr. 8. |
| 10 | `Tools/Axe/axe.tscn` | Skript-Pfade auf `StateMachine/ToolStateMachine.cs`, `StateMachine/IdleState.cs`, `StateMachine/UseToolState.cs` angepasst. | Gleiche Begründung wie Nr. 8. |
| 11 | `TestStateMachine.tscn` | Skript-Pfade auf `StateMachine/NPCStateMachine.cs`, `StateMachine/IdleState.cs`, `StateMachine/WalkState.cs`, `StateMachine/FollowState.cs` angepasst. | Gleiche Begründung wie Nr. 8. |
| 12 | `Characters/Enemies/Minotaur/Minotaur.tscn` | Altes `State Machine/State_Machine.tscn`-Instanzsystem ersetzt durch neuen `NPCStateMachine`-Node mit Unter-Nodes `Idle`, `Walk` (+ `Patrol`), `Follow` (+ `Follow`). `load_steps` von 29 auf 28 korrigiert. | Minotaurus soll das einheitliche neue State-Machine-System nutzen. |
| 13 | `Characters/Enemies/Minotaur/Minotaur.tscn` | Fehlende `_on_area_entered`-Signal-Verbindung hergestellt: `Enemy Detection Area.area_entered` → `Enemy Detection Area._on_area_entered`. | Fehlende Verbindung ergänzt; zuvor war ein fehlerhafter Verweis auf `HurtBoxComponent` vorhanden. |
| 14 | `Characters/Enemies/Minotaur/Minotaur.tscn` | Nicht mehr benötigte `RadiusAttackBehavior`- und `SimpleAttackBehavior`-Nodes unter `NPCInputHandler` entfernt, da das State-Machine-System die State-Logik übernimmt. | Vermeidung redundanter/dysfunktionaler Nodes. |
| 15 | Ordner `StateMachine/` | `New State Machine/`-Ordner physikalisch in `StateMachine/` umbenannt; Dateien entsprechend Nr. 1–4. | Ordnername ohne Leerzeichen und ohne `new…`-Präfix; konsistent zu Aufgabenstellung. |
| 16 | `StateMachine/IdleState.cs`, `StateMachine/WalkState.cs` | Überflüssige leere `Exit()`-Overrides entfernt; `UseToolState.Exit()` aufgeräumt. | Redundanter Code vermeiden; Basis-Implementierung in `CharacterState.Exit()` genügt. |

| 17 | Ordner `State Machine/` | Altes State-Machine-System vollständig aus Git und Dateisystem entfernt (`git rm -rf "State Machine"`). | Keine parallelen Systeme mehr; Build-Blocker und toter Code beseitigt. |

| 18 | `Tools/Sword/Sword.cs` | Verwaiste Referenz auf alte `StateMachine`-Klasse entfernt (`private StateMachine _StateMachine;` und auskommentierter `GetNode`). | Build-Fehler CS0246 nach Löschung des alten Systems; `Sword` nutzt jetzt das ToolStateMachine-System im Szenenbaum. |

| 19 | `Items/log.tres` | `_ItemEffects` verweist jetzt auf die `.tres`-Resource (`HealItemEffect.tres`) statt auf das `.cs`-Skript (`HealItemEffectResource.cs`). | Behebt Laufzeitfehler `InvalidCastException: unable to cast object of type 'Godot.CSharpScript' to 'ItemEffectResource'`. |

## Build-Ergebnis

- **.NET SDK**: 8.0.422 installiert
- **Build**: erfolgreich (`0 Fehler, 6 Warnungen`)
- **Verbleibende Warnungen**:
  - `World/TileMap.cs`: `TileMap` ist veraltet, sollte durch `TileMapLayer` ersetzt werden (4x)
  - `HealthComponent.cs`: Events `_MaxHealthChanged` und `_HealthChanged` werden nie verwendet (2x)

