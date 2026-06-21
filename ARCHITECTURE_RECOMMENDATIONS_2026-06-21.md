# Architektur- und State-Machine-Analyse 2026-06-21

**Branch:** `state-Machine`  
**Engine:** Godot 4.3, C# (.NET 6.0)  
**Projekt:** `twcs` — Top-Down 2D Action-RPG  
**Datum:** 2026-06-21  
**Build:** `dotnet build` → 0 Fehler, 0 Warnungen

---

## 1. Ziel dieses Dokuments

Dokumentation einer neuen, vertieften Analyse des aktuellen Stands von

1. Component System (Health, HurtBox, HitBox, Attack, Movement, Interaktion)
2. State Machine (`StateMachine/`)
3. verbleibende Architekturprobleme, Kopplungen, Null-Ref-Risiken und Konventionsinkonsistenzen

Vergleich mit Godot-Best-Practices (Node-Komposition, HSM, Resource-basierte Daten, EventBus) sowie konkrete, priorisierte Empfehlungen zur weiteren Stabilisierung und Refaktorisierung.

---

## 2. Executive Summary

Das jüngste State-Machine-Refactoring hat das Projekt in einen **build-fähigen, konsolidierten Zustand** gebracht:

- Zwei parallele State-Machine-Systeme wurden auf eine einheitliche `StateMachine/`-Hierarchie reduziert.
- State-Lifecycle ist korrekt (`Exit()` vor `Enter()`).
- `SetPhysicsProcess` wird nur noch auf dem State-Node selbst ausgeführt.
- Health-Events sind implementiert und werden von `HealthBarDisplay` konsumiert.

**Dennoch bleiben relevante Architekturrisiken übrig**, die in dieser Analyse identifiziert und priorisiert werden. Die meisten betreffen Kopplung, Null-Sicherheit, generische Schnittstellen und die Wiederverwendbarkeit von Player- und NPC-Code.

---

## 3. Stand des Component Systems

### 3.1 Vorhandene Components

| Component | Datei | Verantwortung | Bewertung |
|-----------|-------|---------------|-----------|
| `HealthComponent` | `Characters/Character Components/Health Component/HealthComponent.cs` | HP, MaxHP, I-Frames, Todesevent | Gut, aber Events noch mit `_`-Prefix |
| `HurtBoxComponent` | `Characters/Character Components/Hurt Box Component/HurtBoxComponent.cs` | Schaden empfangen, Effektiv-Waffen filtern | Generisch, aber Cooldown ungenutzt |
| `HitBoxComponent` | `Characters/Character Components/Hit Box Component/HitBoxComponent.cs` | Waffen-Hitbox aktivieren | Stark an `Sword` und `Character` gekoppelt |
| `AttackComponent` | `Characters/Character Components/Attack Component/AttackComponent.cs` | Hitbox-Timer, Animationstrigger | Castet hart auf `Player` |
| `CharacterMovementComponent` | `Characters/Character Components/CharacterMovementComponent.cs` | NPC-Patrouille | Nur horizontal, unvollständig |
| `PlayerMovementComponent` | `Characters/Player/PlayerOnlyComponents/PlayerMovementComponent/PlayerMovementComponent.cs` | Player-Bewegung | Einfach, hart an `Player` gekoppelt |
| `PlayerInteractionComponents` | `Characters/Player/PlayerOnlyComponents/PlayerInteractionsComponent/PlayerInteractionComponents.cs` | Interaktions-Area | Fragile Parent-Suche, fehlende Null-Checks |
| `CollectableComponent` | `CollectableComponent.cs` | Item-Pickup | Signal per String-Literal |
| `AnimationController` | `Animation/AnimationController.cs` | Animation + Effekte | Nur Minotaur nutzt ihn aktiv |

### 3.2 Kritische Kopplungen

#### `HitBoxComponent` → `Sword` / `Character`

```csharp
public Sword _Tool;
public Character _CharacterParent;
```

In `_Ready()` läuft der Node-Baum hoch, um `Sword` und `Character` zu finden. Das macht die Hitbox für andere Werkzeuge (z. B. `Axe`) oder zukünftige NPC-Waffen nicht wiederverwendbar. Eine generische Referenz auf `HandItem` und ein Interface/ein `[Export]` auf `Character` wären robuster.

**Empfohlene Änderung:**
- `_Tool` als `HandItem` typisieren.
- `_CharacterParent` über `[Export]` oder `Owner as Character` setzen.
- Collision-Shape-Positionierung in `HitBoxComponent` sollte auf eine generische "Wielder"-Referenz zugreifen, nicht direkt auf `_CharacterParent._CurrentLookingDirection`.

#### `AttackComponent` → `Player`

```csharp
_StartAttackAnimation.Invoke(GetParent<Player>()._CurrentLookingDirection);
```

NPCs haben keinen `Player`-Parent, können also `AttackComponent` nicht nutzen. Sobald NPCs angreifen sollen, bricht das System.

**Empfohlene Änderung:**
- `_StartAttackAnimation` auf `Character` (oder `Node2D`) umstellen.
- Richtung aus `Owner as Character` oder State-Machine holen.

#### `Player` → `Sword`

```csharp
_Tool = GetNode("Tool").GetChild(0) as Sword;
```

Nur `Sword` wird akzeptiert, obwohl `HandItem` bereits als Basisklasse existiert. Äquipieren der `Axe` funktioniert nicht.

**Empfohlene Änderung:**
- `_CurrentHandItem` als `HandItem` halten.
- Waffen-Switching über einen `EquipmentComponent`-Node kapseln.

### 3.3 Null-Ref-Risiken im Component System

| Stelle | Risiko | Bewertung |
|--------|--------|-----------|
| `HealthComponent._Ready()` | `GetNode<Timer>("InvulnerableTimer")` ohne Null-Check | Mittel |
| `HurtBoxComponent.OnAreaEntered()` | `_OnDamageRecived.Invoke(...)` ohne `?.Invoke` | Mittel |
| `HitBoxComponent._Process()` | `_CharacterParent` kann null sein → NRE | Hoch |
| `AttackComponent._Ready()` | `_HitBoxComponent` kann null sein, trotzdem `_HitBoxTimer.Timeout += _HitBoxComponent.DeactivateHitBox` | Hoch |
| `AttackComponent.OnAttackRequest()` | `_StartAttackAnimation.Invoke(...)` ohne `?.Invoke` | Hoch |
| `Player._Ready()` | Diverse `GetNode` ohne `GetNodeOrNull` | Hoch |
| `PlayerInteractionComponents._Ready()` | `InteractionArea.GetNode<CollisionShape2D>(...)` ohne Prüfung | Mittel |
| `AnimationController._Ready()` | `_HurtEffectTimer` via `FindChild` ohne Null-Check, Timeout direkt geconnectet | Hoch |

### 3.4 Bewertung gegen Godot-Best-Practices

| Bereich | Godot-Best-Practice | twcs aktuell | Bewertung |
|---------|---------------------|--------------|-----------|
| Node-Komposition | Fähigkeiten als Kind-Nodes | Grundsätzlich vorhanden | Gut |
| Generische Components | Wiederverwendbare Logik über Interfaces/Export | Teilweise, aber viele harte Casts | Verbesserbar |
| Resource-basierte Daten | `[GlobalClass]` Resources für Items/Stats | Sauber bei Items | Gut |
| Event-getriebene Kommunikation | Godot-Signals oder C#-Events lose gekoppelt | C#-Events, aber teilweise unsicher | Verbesserbar |
| Null-Sicherheit | `GetNodeOrNull<T>` + Prüfungen | Viele direkte `GetNode<T>` | Verbesserbar |

---

## 4. Stand der State Machine

### 4.1 Struktur

```
StateMachine/
├── CharacterStateMachine.cs      (Basis)
├── CharacterState.cs               (Basis-State mit Animationslogik)
├── IdleState.cs
├── WalkState.cs
├── UseToolState.cs
├── FollowState.cs                  (leere Vererbung von WalkState)
├── PlayerStateMachine.cs
├── NPCStateMachine.cs
└── ToolStateMachine.cs
```

### 4.2 Stärken

- **Eine einheitliche HSM:** Keine zwei konkurrierenden Systeme mehr.
- **Korrekter Lifecycle:** `Exit()` vor `Enter()` in `CharacterStateMachine.ChangeState()`.
- **Physik nur auf State-Node:** `SetPhysicsProcess(true/false)` in `CharacterState.Enter()`/`Exit()`.
- **Trennung Player/NPC/Tool:** Spezialisierte State Machines für unterschiedliche Akteure.
- **Gemeinsame Animationslogik:** `CharacterState.StartAnimation()` zentralisiert Richtungsanimationen.

### 4.3 Verbleibende Probleme

#### A. String-basierte State-Suche

```csharp
CharacterState newState = FindChild(state) as CharacterState;
```

`FindChild` ist anfällig für Umbenennungen und leere Strings. Es gibt keine Typprüfung zur Compile-Zeit.

**Empfohlene Änderung:**
- States in ein `Dictionary<string, CharacterState>` laden (`_Ready()` iteriert über `GetChildren()`).
- Oder `[Export] private Godot.Collections.Array<CharacterState> _States;` mit einem separaten `_StateName` pro State.

#### B. `_PreviousState` wird überschrieben, bevor `Exit()` aufgerufen wird

Aktuell:

```csharp
_PreviousState.Exit();
newState.Enter();
_CurrentState = newState;
_PreviousState = _CurrentState;   // == newState, nicht der alte!
```

Das macht `_PreviousState` praktisch wertlos; es zeigt nach einem Wechsel auf den neuen State.

**Empfohlene Änderung:**

```csharp
var oldState = _CurrentState;
oldState?.Exit();
_CurrentState = newState;
_PreviousState = oldState;
_CurrentState.Enter();
```

#### C. `CharacterStateMachine` kennt `Player`

```csharp
if (Owner is Player)
{
    _CurrentScenePlayer = Owner as Player;
}
else
{
    _CurrentScenePlayer = GetTree().GetNodesInGroup("Player")[0] as Player;
}
```

Das führt zu einer ungewollten Abhängigkeit: jede `CharacterStateMachine` muss den Player finden. NPCs brauchen den Player in der Basisklasse nicht.

**Empfohlene Änderung:**
- `_CurrentScenePlayer` in `NPCStateMachine` verschieben.
- In `CharacterStateMachine` nur einen optionalen `Character OwnerCharacter` bereitstellen.

#### D. `WalkState` ist für NPCs und Player unterschiedlich implementiert

```csharp
PlayerMovementComponent cmc = Owner.GetNode<PlayerMovementComponent>("MovementComponent");
cmc.HandleMovement(_StateMachine._CurrentDirection);
```

Dieser Code wird für NPCs in `TestStateMachine.tscn` und `Minotaur.tscn` aufgerufen, die aber `CharacterMovementComponent` oder `PlayerMovementComponent` verwenden. Es ist Zufall, wenn es läuft.

**Empfohlene Änderung:**
- Gemeinsames Interface `IMovementComponent` einführen:

```csharp
public interface IMovementComponent
{
    void HandleMovement(Vector2 direction);
}
```

- `PlayerMovementComponent` und `CharacterMovementComponent` implementieren es.
- `WalkState` nutzt `Owner.FindChild<IMovementComponent>()` oder `[Export]`.

#### E. `ToolStateMachine` greift über `Owner.Owner` auf Character-State-Machine zu

```csharp
_CurrentDirection = Owner.Owner.GetNode<CharacterStateMachine>("StateMachine")._CurrentDirection;
```

Das ist die vermuteteste Null-Ref-Quelle im gesamten System. Wenn der Wielder keine `StateMachine` hat, crasht das Tool.

**Empfohlene Änderung:**
- `[Export] private CharacterStateMachine _OwnerStateMachine;` im ToolStateMachine setzen.
- Oder Event-basiert: Character-State-Machine feuert `DirectionChanged`, ToolStateMachine lauscht.

#### F. `NPCStateMachine` arbeitet in `_Process`

```csharp
public override void _Process(double delta)
{
    if ((Owner as Character).GlobalPosition.DistanceTo(_CurrentScenePlayer.GlobalPosition) < 300)
    ...
}
```

State-Entscheidungen, die Bewegung/Physik betreffen, sollten in `_PhysicsProcess` laufen, damit sie synchron mit der Kollision sind.

**Empfohlene Änderung:**
- `_Process` → `_PhysicsProcess` umstellen.
- Sichtradius und Aggro-Logik in ein `EnemyDetectionArea`-Signal auslagern.

#### G. `FollowState` ist leer

```csharp
public partial class FollowState : WalkState { }
```

Das funktioniert nur, weil die Follow-Logik über das `_MovementBehavior` im Editor konfiguriert wird. Das ist akzeptabel, aber dokumentationsbedürftig. Ein Kommentar oder eine minimale `FollowState`-Implementierung wäre sinnvoll.

#### H. `UseToolState` greift direkt auf Hitbox zu

```csharp
_HitBoxComponent._CollisionShape2D.Disabled = false;
```

Das vermischt State-Logik mit Hitbox-Detailwissen. Besser:

```csharp
Owner.FindChild<AttackComponent>()?.OnAttackRequest();
```

#### I. Verwaiste Signal-Verbindung in `Player.tscn`

```
[connection signal="animation_finished" from="AnimationPlayer" to="." method="_FinishedAnimation"]
```

`_FinishedAnimation` existiert nicht in `Player.cs`. Das war ein Build-Blocker in der alten Analyse; derzeit scheint Godot es nicht mehr als Fehler zu melden, aber es ist toter Code in der Szene.

---

## 5. Globale Architektur

### 5.1 `GameManager` und Player-Referenz

```csharp
public static Player getPlayer()
{
    return _Player;
}
```

`GameManager` instanziiert einen Player in `_Ready()`, der nie der Szene hinzugefügt wird. `HealItemEffectResource` ruft `GameManager.getPlayer()` auf und heilt damit einen **falschen, nicht in der Welt existierenden Player**.

**Empfohlene Änderung:**
- `GameManager` sollte keine Player-Instanz halten, sondern nur PackedScenes oder globale Konfigurationen.
- Heilitems sollten ein `Use(Character target)`-Muster bekommen oder über einen EventBus (`PlayerSpawned`, `PlayerHealthChanged`) arbeiten.

### 5.2 `InputHandler` als Autoload

Der `InputHandler` ist ein globaler Autoload, der Events feuert. Das ist ein korrektes Pattern. Problematisch:

- `_OnUseInput.Invoke()` und `_OnInteractionInput.Invoke()` ohne `?.Invoke` → NRE, wenn niemand lauscht.
- `Interact`-Action in `project.godot` hat keine Events hinterlegt, ist also nicht nutzbar.

**Empfohlene Änderung:**
- Alle `.Invoke()` durch `?.Invoke()` ersetzen.
- `Interact`-Action mit Tastenbelegung versehen oder entfernen.

### 5.3 Fehlender EventBus

Globale Kommunikation erfolgt über direkte Singleton-Zugriffe (`GameManager.getPlayer()`). Das erschwert zukünftige Erweiterungen (z. B. Multiplayer, Save/Load, Quests).

**Empfohlene Änderung:**
- Einen `EventBus`-Autoload einführen:

```csharp
public partial class EventBus : Node
{
    public static EventBus Instance { get; private set; }

    [Signal] public delegate void PlayerSpawnedEventHandler(Player player);
    [Signal] public delegate void HealthChangedEventHandler(Node entity, int health);
    [Signal] public delegate void EntityDiedEventHandler(Node entity);

    public override void _Ready() => Instance = this;
}
```

Damit können `HealthBarDisplay`, `HealItemEffectResource`, Quests etc. lose gekoppelt werden.

---

## 6. Inventar und Items

### 6.1 Aktueller Stand

- `InventoryDataResource` hat 5 Slots (`Player_Inventory.tres`).
- `InventoryUI` instanziiert dynamisch pro Slot ein `InventorySlotUI` — hier ist die Anzahl konsistent, solange Resource und UI synchron gehalten werden.
- `addItem` stapelt unbegrenzt (`slot._Quantity += amount`), ohne Maximalmenge.
- `InventorySlotUI.ItemPressed` reduziert `_Quantity` ohne Limit-Prüfung und löscht leere Slots nicht.
- `ItemDataResource.Use()` gibt nur `true`/`false` zurück; der eigentliche Zielcharakter ist nicht bekannt.

### 6.2 Empfohlene Änderungen

1. **Stapel-Limit pro Item:** `ItemDataResource` erhält `[Export] int _MaxStackSize = 99`.
2. **Leere Slots entfernen:** `InventorySlotUI.ItemPressed` sollte bei `_Quantity <= 0` das SlotData auf `null` setzen und das UI neu aufbauen.
3. **Item-Ziel explizit machen:** `ItemDataResource.Use(Character user)` oder EventBus-basierte Effekte.
4. **Signal statt String-Literal:** `CollectableComponent.EmitSignal(SignalName.OnItemPickedUp)` statt `EmitSignal("OnItemPickedUp")`.

---

## 7. Konventionen und Code-Qualität

### 7.1 Namenskonventionen

Fast alle Felder (auch public/exportierte) beginnen mit `_`. Das widerspricht C#-Konventionen:

| Aktuell | Empfohlen |
|---------|-----------|
| `[Export] private int _MaxHealth;` | `[Export] public int MaxHealth { get; private set; }` |
| `public event Action<int> _HealthChanged;` | `public event Action<int> HealthChanged;` |
| `public Vector2 _CurrentDirection;` | `public Vector2 CurrentDirection;` |
| `public HealthComponent _HealthComponent;` | `public HealthComponent HealthComponent { get; private set; }` |

### 7.2 Null-Sicherheit

Viele `GetNode<T>` und Casts ohne Prüfung. Empfohlene Minimalregel:

> Jeder `GetNode<T>()` und jeder harte Cast bekommt entweder ein `GetNodeOrNull<T>()` + Null-Check oder ein `[Export]`-NodePath mit `node_paths` in der `.tscn`.

### 7.3 Nullable aktivieren

`TwoWorlds CSharp.csproj` hat `<Nullable>` nicht gesetzt. Aktivierung würde viele latente Null-Fehler zur Compile-Zeit sichtbar machen.

Empfohlen:

```xml
<Nullable>enable</Nullable>
```

Das kann zunächst viele Warnungen erzeugen; daher in einer eigenen Refactor-Sitzung schrittweise einführen.

### 7.4 Toter Code

- `Characters/NPCInputHandler.cs` ist komplett auskommentiert, aber noch als Node in `Minotaur.tscn` vorhanden.
- `Characters/Character.cs` enthält mehrere auskommentierte Zeilen (`_StateMachine`, `_HurtBoxComponent`).
- `Player.cs` enthält auskommentierte Signal-Verbindungen.
- `Player.tscn` hat eine Signal-Verbindung auf `_FinishedAnimation`, das nicht existiert.

---

## 8. Priorisierte Empfehlungen

### P0 — Kritisch: Laufzeitfehler verhindern

| # | Maßnahme | Dateien | Begründung |
|---|----------|---------|------------|
| 1 | Alle `.Invoke()` ohne `?.Invoke` korrigieren | `InputHandler.cs`, `HurtBoxComponent.cs`, `AttackComponent.cs` | NRE, wenn keine Listener registriert |
| 2 | `ToolStateMachine` vom `Owner.Owner`-Zugriff befreien | `ToolStateMachine.cs` | Hochriskante Null-Ref |
| 3 | `HitBoxComponent._Process()` gegen Null-Parent schützen | `HitBoxComponent.cs` | NRE pro Frame |
| 4 | `_PreviousState` korrekt setzen | `CharacterStateMachine.cs` | Historie/Korrektheit |
| 5 | `WalkState` generisch auf `IMovementComponent` umstellen | `WalkState.cs`, Movement-Components | NPCs/Player dürfen nicht unterschiedlich behandelt werden |

### P1 — Hoch: Architektur entkoppeln

| # | Maßnahme | Dateien | Begründung |
|---|----------|---------|------------|
| 6 | `HitBoxComponent` kennt `HandItem`, nicht `Sword` | `HitBoxComponent.cs`, `HandItem.cs`, `Sword.cs` | Waffenwiederverwendung |
| 7 | `AttackComponent` auf `Character` umstellen | `AttackComponent.cs` | NPC-Angriffe ermöglichen |
| 8 | Player-Equipment generisch halten (`HandItem` statt `Sword`) | `Player.cs` | Werkzeugwechsel ermöglichen |
| 9 | `GameManager` bereinigen; Heilitem zielorientiert machen | `GameManager.cs`, `HealItemEffectResource.cs` | Richtiger Player wird geheilt |
| 10 | EventBus-Autoload einführen | neues `EventBus.cs` | Lose Kopplung |
| 11 | `NPCStateMachine._Process` → `_PhysicsProcess` | `NPCStateMachine.cs` | Physik-Synchronität |

### P2 — Mittel: Wartbarkeit erhöhen

| # | Maßnahme | Dateien | Begründung |
|---|----------|---------|------------|
| 12 | States per Dictionary registrieren | `CharacterStateMachine.cs` | Robuster als `FindChild(state)` |
| 13 | `_CurrentScenePlayer` aus Basisklasse entfernen | `CharacterStateMachine.cs`, `NPCStateMachine.cs` | Klare Zuständigkeit |
| 14 | `AnimationController` statt direktem `AnimationPlayer`-Zugriff | `CharacterStateMachine.cs`, States | Konsistentere Animation-Steuerung |
| 15 | `UseToolState` nutzt `AttackComponent` | `UseToolState.cs` | Single-Responsibility |
| 16 | `FollowState` dokumentieren oder minimal implementieren | `FollowState.cs` | Verständlichkeit |
| 17 | `PlayerInteractionComponents` robust machen | `PlayerInteractionComponents.cs` | Null-Sicherheit |

### P3 — Niedrig: Qualität und Konventionen

| # | Maßnahme | Dateien | Begründung |
|---|----------|---------|------------|
| 18 | `_`-Prefix aus public/exportierten Membern entfernen | alle `.cs`-Dateien | C#-Konvention |
| 19 | `Nullable` aktivieren | `TwoWorlds CSharp.csproj` | Compile-Zeit-Sicherheit |
| 20 | `.editorconfig` + CSharpier einführen | Wurzelverzeichnis | Einheitliche Formatierung |
| 21 | Toter Code entfernen | `NPCInputHandler.cs`, `Character.cs`, `Player.cs` | Sauberkeit |
| 22 | `CollectableComponent` auf `SignalName` umstellen | `CollectableComponent.cs` | Typsicherheit |
| 23 | Inventar-Stapel-Limit und leere Slots | `InventoryDataResource.cs`, `InventorySlotUI.cs`, `ItemDataResource.cs` | Konsistenz |

---

## 9. Empfohlene zukünftige Architektur

```
Player / Enemy (CharacterBody2D)
├── Sprite2D / Texture
├── StateMachine (CharacterStateMachine)
│   ├── IdleState
│   ├── WalkState
│   ├── UseToolState
│   ├── HurtState          (neu)
│   └── DieState           (neu)
├── AnimationController    (wiederverwendbar)
├── HealthComponent
├── HurtBoxComponent
├── AttackComponent        (wiederverwendbar für Player + NPC)
├── MovementComponent      (PlayerMovementComponent / NPCMovementComponent → IMovementComponent)
├── InteractionComponent   (nur Player)
└── EquipmentComponent     (neu: hält aktuelles HandItem)

Tool / HandItem (Node2D)
├── HitBoxComponent
├── AnimationPlayer
├── ToolStateMachine
│   ├── IdleState
│   └── UseToolState
└── ToolDataResource       (neu: Schaden, Reichweite, Kategorie)
```

### Neue/angepasste Interfaces

```csharp
public interface IMovementComponent
{
    void HandleMovement(Vector2 direction);
}

public interface IDamageable
{
    void TakeDamage(int damage);
}

public interface IWeapon
{
    int Damage { get; }
    data_types.HandItemsTypes Category { get; }
    void Use(Vector2 direction);
}
```

---

## 10. Fazit

Das State-Machine-Refactoring auf dem Branch `state-Machine` hat das Projekt stabilisiert: der Build ist sauber, die State-Lifecycle-Reihenfolge ist korrekt, und Health/Healthbar funktionieren eventbasiert. Die größten verbleibenden Risiken liegen in **direkten Kopplungen** (`Sword`/`Player`, `Owner.Owner`, `GameManager.getPlayer()`), **fehlender Null-Sicherheit** und der **nicht-generischen Movement-Behandlung**.

Die Empfehlungen sind so priorisiert, dass P0 zuerst die Laufzeitstabilität sichert, P1 die Architektur entkoppelt und P2/P3 langfristige Wartbarkeit schaffen. Eine schrittweise Umsetzung in dieser Reihenfolge minimiert Regressionen und hält den Build jederzeit grün.

---

## 11. Referenzen

- `ARCHITECTURE_ANALYSIS_AND_PLAN.md` — älterer Refactor-Plan
- `SUMMARY_STATE_MACHINE.md` — Zusammenfassung des State-Machine-Fixes
- `CHANGELOG_STATE_MACHINE.md` — detailliertes Änderungsprotokoll
- Godot 4.3 C# Docs: https://docs.godotengine.org/en/4.3/tutorials/scripting/c_sharp/
- GDQuest / HeartBeast Godot-RPG-Tutorials (State Machine als Node-Baum)

---

*Dokument erstellt im Rahmen der vertieften Architektur-Analyse 2026-06-21.*
