# Zusammenfassung: Architektur-Risiken

> Branch: fix/architecture-risks  
> Erstellt: 2026-06-21

---

## Überblick

Dieser Branch behebt die P0- und P1-Architekturrisiken aus `ARCHITECTURE_RECOMMENDATIONS_2026-06-21.md`. Ziel war es, Kopplungen zu lösen, Null-Ref-Risiken zu reduzieren und generische Schnittstellen für Player und NPC einzuführen.

**Build-Status:** `dotnet build` → **0 Fehler, 0 Warnungen**

---

## Durchgeführte Änderungen

1. **`ToolStateMachine` entkoppelt**
   - `Owner.Owner`-Zugriff entfernt.
   - Robuster `GetOwnerStateMachine()`-Helfer prüft direkten Owner und `Owner.Owner` via `GetNodeOrNull`.

2. **`AttackComponent` generisch gemacht**
   - Kein harter Cast auf `Player` mehr.
   - Richtung kommt vom `Character`-Parent.
   - Null-Checks und `?.Invoke()` für Events.

3. **`Player`-Equipment generisch**
   - `_CurrentHandItem : HandItem` statt hartem `Sword`.
   - Axe, Sword und zukünftige Items werden gleichermaßen akzeptiert.
   - `GameManager.RegisterPlayer(this)` zur sicheren Player-Registrierung.

4. **`CharacterStateMachine._PreviousState` korrigiert**
   - `_PreviousState` zeigt jetzt tatsächlich auf den vorherigen State.
   - Reihenfolge: `oldState speichern → Exit → CurrentState setzen → PreviousState = oldState → Enter`.

5. **`WalkState` generisch über `IMovementComponent`**
   - Neues Interface `IMovementComponent` mit `HandleMovement(Vector2)`.
   - `PlayerMovementComponent` und `CharacterMovementComponent` implementieren es.
   - `WalkState` sucht das erste Kind mit diesem Interface.

6. **`HitBoxComponent` entkoppelt**
   - `_Tool` ist jetzt `HandItem` statt `Sword`.
   - `_CharacterParent` über `Owner as Character`.
   - Null-Checks in allen kritischen Methoden.

7. **Null-Checks und sichere Invokes flächendeckend**
   - `GetNodeOrNull` statt `GetNode` an vielen Stellen.
   - `?.Invoke()` statt direktem `.Invoke()`.
   - Betroffen: `HealthComponent`, `HurtBoxComponent`, `HitBoxComponent`, `AttackComponent`, `Player`, `InputHandler`, `PlayerInteractionComponents`, `AnimationController`.

8. **State-Suche robuster**
   - `FindChild(state) as CharacterState` durch `Dictionary<string, CharacterState>` ersetzt.
   - States werden in `_Ready()` aus `GetChildren()` geladen.

9. **`GameManager` / Heilitem robust gemacht**
   - `GameManager` erzeugt keinen Phantom-Player mehr.
   - `RegisterPlayer` stellt aktuellen Player bereit.
   - `HealItemEffectResource` prüft Player und HealthComponent.
   - `GameManager/EventBus.cs` als Basis für lose Kopplung hinzugefügt.

10. **`NPCStateMachine` stabilisiert**
    - `_Process` → `_PhysicsProcess`.
    - Eigene Player-Referenz statt Basisklasse.
    - Null-Checks für Player und States.

11. **Build-Fehler behoben**
    - `Character.cs`: doppelte schließende Klammer entfernt.
    - `Sword.cs`: doppeltes `_Damage` entfernt.
    - `Player.cs`: `String` → `string`, korrektes `using`.

---

## Betroffene Dateien

### Geändert
- `Animation/AnimationController.cs`
- `Characters/Character Components/Attack Component/AttackComponent.cs`
- `Characters/Character Components/CharacterMovementComponent.cs`
- `Characters/Character Components/Health Component/HealthComponent.cs`
- `Characters/Character Components/Hit Box Component/HitBoxComponent.cs`
- `Characters/Character Components/Hurt Box Component/HurtBoxComponent.cs`
- `Characters/Character.cs`
- `Characters/Player/Player.cs`
- `Characters/Player/PlayerOnlyComponents/InputHandlerComponent/InputHandler.cs`
- `Characters/Player/PlayerOnlyComponents/PlayerInteractionsComponent/PlayerInteractionComponents.cs`
- `Characters/Player/PlayerOnlyComponents/PlayerMovementComponent/PlayerMovementComponent.cs`
- `GameManager.cs`
- `Items/ItemEffects/Heal [Item Effect Resource]/HealItemEffectResource.cs`
- `StateMachine/CharacterStateMachine.cs`
- `StateMachine/NPCStateMachine.cs`
- `StateMachine/ToolStateMachine.cs`
- `StateMachine/WalkState.cs`
- `Tools/HandItem.cs`
- `Tools/Sword/Sword.cs`

### Neu erstellt
- `Characters/Character Components/Movement/IMovementComponent.cs`
- `GameManager/EventBus.cs`
- `CHANGELOG_ARCHITECTURE_RISKS.md`
- `SUMMARY_ARCHITECTURE_RISKS.md`

---

## Bekannte Einschränkungen / offene Punkte

- **Szenen-Files (`.tscn`)** wurden nicht angepasst. Falls Node-Pfade oder Export-Werte im Godot-Editor nicht mehr passen, müssen diese dort korrigiert werden.
- **`UseToolState`** greift weiterhin direkt auf `_HitBoxComponent._CollisionShape2D.Disabled` zu (P2).
- **`FollowState`** ist weiterhin leer (P2); die Logik liegt im `FollowPlayerBehavior`.
- **`Nullable`-Checks** im `.csproj` sind nicht aktiviert.
- **`ToolStateMachine.GetOwnerStateMachine()`** ist robuster, aber immer noch von der Szenenhierarchie abhängig. Eine sauberere Lösung wäre ein `[Export]`-NodePath oder ein Signal.

---

## Empfohlene nächste Schritte

1. Projekt im Godot-Editor öffnen und Szenen (besonders `Player.tscn`, `Sword.tscn`, `Minotaur.tscn`) auf fehlende Node-Verknüpfungen prüfen.
2. `UseToolState` weiter entkoppeln (P2).
3. `FollowState` mit echter Logik füllen oder in `WalkState` + Behavior integrieren (P2).
4. `Nullable`-Checks im `.csproj` aktivieren (`<Nullable>enable</Nullable>`) und verbleibende Warnungen abarbeiten.
5. EventBus für weitere globale Events ausbauen (Item-Pickup, Player-Death, Quest-Trigger).

