# Änderungsprotokoll: Architektur-Risiken

> Branch: fix/architecture-risks
> Start: 2026-06-21
> Ziel: P0- und P1-Architekturrisiken aus ARCHITECTURE_RECOMMENDATIONS_2026-06-21.md beheben

---

## Änderungen

| Nr. | Datei(en) | Änderung | Begründung |
|-----|-----------|----------|------------|
| 1 | `StateMachine/ToolStateMachine.cs` | `Owner.Owner`-Zugriff entfernt. `GetOwnerStateMachine()`-Helfer mit `GetNodeOrNull` eingeführt, der direkten Owner und `Owner.Owner` prüft. | Null-Ref-Risiko bei verschachtelten Szenen; robuster Zugriff auf Owner-StateMachine. |
| 2 | `Characters/Character Components/Attack Component/AttackComponent.cs` | Harten Cast auf `Player` entfernt. Richtung wird jetzt vom `Character`-Parent geholt. `?.Invoke()` und Null-Checks für `HitBoxComponent`/`Timer` ergänzt. | NPCs sollen später angreifen können; keine Null-Refs bei fehlenden Nodes. |
| 3 | `Characters/Player/Player.cs` | `_Tool` durch generisches `_CurrentHandItem : HandItem` ersetzt. `GetNodeOrNull` für InputHandler, InteractionComponents, AttackComponent, MovementComponent. `GameManager.RegisterPlayer(this)` hinzugefügt. | Axe und andere HandItems werden akzeptiert; robustere Initialisierung; keine Phantom-Player-Referenz. |
| 4 | `StateMachine/CharacterStateMachine.cs` | `_PreviousState`-Bug korrigiert: `oldState` zwischengespeichert, dann `Exit()`, `_CurrentState = newState`, `_PreviousState = oldState`, `Enter()`. | `_PreviousState` zeigte bisher auf den neuen State und war wertlos. |
| 5 | `StateMachine/WalkState.cs`, `Characters/Character Components/Movement/IMovementComponent.cs`, `Characters/Character Components/CharacterMovementComponent.cs`, `Characters/Player/PlayerOnlyComponents/PlayerMovementComponent/PlayerMovementComponent.cs` | Neues `IMovementComponent`-Interface eingeführt. `PlayerMovementComponent` und `CharacterMovementComponent` implementieren es. `WalkState` nutzt `FindChild` + Interface statt hartem `PlayerMovementComponent`. | Player und NPC können denselben WalkState nutzen; generische Bewegungskomponente. |
| 6 | `Characters/Character Components/Hit Box Component/HitBoxComponent.cs` | `_Tool` ist jetzt `HandItem` statt `Sword`. `_CharacterParent` über `Owner as Character`. Null-Checks in `_Process`, `ActivateHitBox`, `DeactivateHitBox`, `ChangeCurrentHitboxPosition`. Parent-Suche bricht nach Fund ab. | Wiederverwendbar für Axe und zukünftige Waffen; weniger Null-Ref-Risiken. |
| 7 | `Characters/Character Components/Health Component/HealthComponent.cs`, `Characters/Character Components/Hurt Box Component/HurtBoxComponent.cs`, `Characters/Player/PlayerOnlyComponents/InputHandlerComponent/InputHandler.cs`, `Characters/Player/PlayerOnlyComponents/PlayerInteractionsComponent/PlayerInteractionComponents.cs`, `Animation/AnimationController.cs` | `GetNodeOrNull` statt `GetNode` eingeführt, wo keine Garantie besteht. `.Invoke()` durch `?.Invoke()` ersetzt. Fehlende Null-Checks ergänzt. | Weniger Laufzeitabstürze bei fehlenden Nodes; sichere Event-Aufrufe. |
| 8 | `StateMachine/CharacterStateMachine.cs` | String-basierte State-Suche `FindChild(state) as CharacterState` durch `Dictionary<string, CharacterState>` ersetzt, in `_Ready` via `GetChildren()` befüllt. | Compile-zeitnahe State-Referenz, weniger Fehleranfälligkeit bei Umbenennungen. |
| 9 | `GameManager.cs`, `GameManager/EventBus.cs`, `Items/ItemEffects/Heal [Item Effect Resource]/HealItemEffectResource.cs` | `GameManager` instanziiert keinen Phantom-Player mehr, bietet `RegisterPlayer`. `HealItemEffectResource` prüft Player und HealthComponent mit Null-Checks. EventBus-Autoload als Basis für lose Kopplung hinzugefügt. | Heilitems heilen den tatsächlichen Player; keine verwaisten Player-Instanzen; Vorbereitung für EventBus-Muster. |
| 10 | `StateMachine/NPCStateMachine.cs` | `_Process` → `_PhysicsProcess` umgestellt. Eigene Player-Referenz statt Basisklasse. Null-Checks für Player und States. | State-Wechsel innerhalb der Physik-Schleife; robustere NPC-Logik. |
| 11 | `Characters/Character.cs` | `_Ready()` mit `base._Ready()` und Null-Check für `HealthComponent` aufgeräumt; doppelte schließende Klammer entfernt. | Sauberere Basisklasse; Build-Fehler behoben. |
| 12 | `Tools/HandItem.cs`, `Tools/Sword/Sword.cs` | `_Damage` in `HandItem`-Basisklasse belassen; doppeltes `_Damage` in `Sword` entfernt. | Kein CS0108-Warnung mehr; einheitlicher Schadenswert. |
| 13 | `Characters/Player/Player.cs` | `String` durch `string` ersetzt und `using System.Collections.Generic` durch `using System` ausgetauscht. | Build-Fehler CS0246/GD0102 behoben. |

---

## Build-Ergebnis

- **.NET SDK**: 8.0.422 installiert
- **Build**: erfolgreich (`0 Fehler, 0 Warnungen`)

---

## Offene Punkte / Hinweise

- Szenen-Files (`.tscn`) wurden nicht angepasst; ggf. müssen Node-Pfade/Export-Werte in Godot noch geprüft werden.
- `UseToolState` greift weiterhin direkt auf `_HitBoxComponent._CollisionShape2D.Disabled` zu (P2).
- `FollowState` ist weiterhin leer (P2); die Follow-Logik liegt im `FollowPlayerBehavior`.
- `Nullable`-Checks im `.csproj` sind nicht aktiviert.
