# Recherche-Bericht: Godot 4 C# Architekturmuster
## Component Systems, State Machines & Vergleichsprojekte

**Projektkontext:** TwoWorlds CSharp (Godot 4.3, .NET 6, Top-Down-RPG)  
**Datum:** 21.06.2026  
**Quellen:** Lokale Codebasis `/home/simon/twcs-temp`, Godot 4.3-Dokumentation, etablierte Godot-C#-Muster, GitHub-Beispielprojekte (allgemein bekannte Referenzen).

---

## 1. Zusammenfassung

Godot 4 bietet mit C# eine vollständige .NET-Integration. Im Gegensatz zu Unity basiert Godot auf einem **Node-Kompositionsmodell**: statt MonoBehaviour-ähnlicher Komponenten werden Fähigkeiten durch Kinder-Nodes ergänzt. Für größere RPGs zeigen etablierte Godot-Projekte folgende bevorzugte Muster:

1. **Node-basierte Components** (HealthComponent, AttackComponent, MovementComponent) als Kinder von `CharacterBody2D`.
2. **Hierarchical State Machine (HSM)** als Node-Baum mit einem zentralen `StateMachine`-Node und abgeleiteten `State`-Knoten.
3. **Resource-basierte Daten** (`ItemDataResource`, `ItemEffectResource`) für Inventar, Stats und Werkzeuge.
4. **Behavior-Interfaces** (`IMovementBehavior`, `IAttackExecution`) für austauschbares NPC-Verhalten.
5. **Autoload-Singletons** für globale Dienste wie Input, Inventory, GameManager.

Diese Muster sind auch in der lokalen Codebasis erkennbar, teilweise aber noch inkonsistent umgesetzt.

---

## 2. Component-basierte Architektur in Godot 4 C#

### 2.1 Godot-Philosophie: Nodes statt Komponenten

Godot verzichtet bewusst auf ein ECS/MonoBehaviour-Modell. Jedes GameObject ist ein Node-Baum. Wiederverwendbare Funktionalität wird als eigener Node unterhalb des Owners angelegt. Dies ist das **offizielle Design-Pattern** von Godot:

> "Everything in Godot is based on nodes ... composition is preferred over inheritance."  
> — Godot Docs: Introduction to nodes and scenes, Best Practices.

**Vorteile**
- Klare Sichtbarkeit im Editor (Scene Tree).
- Einfache Prüfung mit `GetNode<T>()` / `FindChild()`.
- Export-Variablen können direkt im Inspector editiert werden.

**Nachteile**
- Weniger Typsicherheit als reine C#-Komponenten (Name-Lookup).  
- Node-Tree-Kopplung erschwert Unit-Tests.

### 2.2 Implementierungsmuster

#### Muster A: Node-Component mit `[Export]`
```csharp
public partial class HealthComponent : Node
{
    [Export] private int _MaxHealth = 10;
    [Export] private int _Health = 1;
    public event Action<int> _HealthChanged;
    public event Action _HealthEmpty;
    ...
}
```
**Verwendung in lokalen Code:** `HealthComponent.cs`, `AttackComponent.cs`, `HitBoxComponent.cs`, `HurtBoxComponent.cs`, `CollectableComponent.cs`, `CharacterMovementComponent.cs`, `PlayerMovementComponent.cs`, `PlayerInteractionComponents.cs`.

#### Muster B: Abstrakte Basisklassen + Interfaces
Im lokalen Code: `BaseMovementBehavior : Node, IMovementBehavior` mit abgeleiteten `FollowPlayerBehavior`, `MovingRandomlyAroundBehavior`, `PointToPointPatrolBehavior`.  
Damit kann eine State-Machine einfach unterschiedliche Verhaltensweisen austauschen.

#### Muster C: Resource-basierte Item/Stat-Komponenten
`ItemDataResource`, `ItemEffectResource`, `HealItemEffectResource` nutzen `[GlobalClass]` und `[Export]`, um datengetriebene Effekte im Editor zu konfigurieren. Das entspricht Godots `Resource`-System und ist für RPGs ein bevorzugter Ansatz.

### 2.3 Bewertung & Empfehlung

| Muster | Wann nutzen | Lokale Einschätzung |
|--------|-------------|----------------------|
| Node-Components | Physik, Animation, Health, Hitbox | Passend, etablieren |
| Behavior-Interfaces | Bewegung/KI austauschbar | Gut, ausbauen |
| Resource-Components | Items, Stats, Skills | Sehr gut, ausbauen |
| Reine C#-Komponenten (ohne Node) | Logik, Services, reine Daten | Für Services/Singletons ok |

**Empfehlung:** Weiter an der **Node-Komposition** festhalten. Zusätzlich Interfaces (`IInteractable`, `IAttackCondition`, `IMovementBehavior`, `IAttackExecution`) konsequent nutzen, um Kopplung zu reduzieren.

---

## 3. State-Machine-Implementierungen in Godot C#

### 3.1 Bekannte Muster

| Ansatz | Beschreibung | Bewertung |
|--------|--------------|-----------|
| **Node-HSM** | `StateMachine`-Node mit Kind-Nodes `IdleState`, `WalkState`, `AttackState`. Zentrale `TransitionTo(string)`-Methode. | Offizielles Godot-Tutorial-Muster, sehr verbreitet, gut wartbar. |
| **Enum-Switch** | State-Enum + großer `_PhysicsProcess`-Switch. | Schnell, aber wird schnell unübersichtlich. |
| **State-Pattern (rein C#)** | Klassen ohne Node-Bezug, verwaltet in einem Dictionary. | Flexibel, aber weniger editor-freundlich. |
| **Behavior-Tree / HFSM** | Für komplexe KI; selten in kleineren RPGs. | Nur bei Bedarf. |

### 3.2 Lokale Umsetzung

Es existieren **zwei State-Machine-Implementierungen** parallel:

1. **Alte StateMachine** (`StateMachine/`)
   - `StateMachine : Node` verwaltet `Dictionary<string, State>`.
   - Nutzt `GetChildren().OfType<State>()` zur automatischen Registrierung.
   - `TransitionTo(string, Vector2)` startet Animation über `AnimationController`.
   - **Problem:** Nur eine Richtung wird übergeben; State-Wechsel-Logik ist auf die StateMachine konzentriert.

2. **Neue StateMachine** (`New State Machine/`)
   - `newStateMachine : Node2D` mit abgeleiteten `newState`-Knoten (`newIdleState`, `newWalkState`, `newUseToolState`, `newFollowState`).
   - `ChangeState(string)` sucht Kinder per Name (`FindChild`).
   - `newState` enthält gemeinsame Animationslogik (`StartAnimation`, `DirectionForAnimation`).
   - **Vorteil:** Wiederverwendbare Basisklasse, saubere Trennung Player/NPC/Tools über `PlayerStateMachine`, `NPCStateMachine`, `ToolStateMachine`.

### 3.3 Stärken und Schwächen der lokalen HSM

**Stärken**
- Klare Trennung zwischen `PlayerStateMachine` (inputgesteuert) und `NPCStateMachine` (distanzgesteuert).
- Gemeinsame Animation-Start-Logik in `newState` zentralisiert.
- States aktivieren/deaktivieren `_PhysicsProcess` sauber via `SetPhysicsProcess(true/false)`.

**Schwächen / Verbesserungspotenzial**
- `ChangeState` wechselt den State, ruft aber **zuerst** `_CurrentState.Enter()` und **danach** `_PreviousState.Exit()` auf. Reihenfolge sollte umgekehrt sein, um Tween/Hitbox-Cleanups korrekt auszuführen.
- `_PreviousState` wird erst nach `Enter()` aktualisiert; ein echter `Exit()` des alten States fehlt in der alten Implementierung.
- `Owner.SetPhysicsProcess(true/false)` in States ist riskant, weil es die komplette Physik-Verarbeitung des Owners (z. B. `Player`) einfriert. Besser nur den State-Node selbst pausieren.
- `newWalkState` sucht hart `Owner.GetNode<PlayerMovementComponent>("MovementComponent")` – nicht generisch für NPCs.
- `newUseToolState` hat Hitbox-Direction-Logik auskommentiert (`//SetHitComponentDirection()`); Richtungsabstimmung fehlt.
- Tool-State ist nicht mit `AttackComponent` verbunden (`_InputHandler._OnUseInput` ist in `Player._Ready()` auskommentiert).

### 3.4 Empfohlene State-Machine-Struktur

```
Player (CharacterBody2D)
├── Sprite2D
├── StateMachine (Node)
│   ├── IdleState
│   ├── WalkState
│   ├── UseToolState
│   └── HurtState
├── AnimationController
├── HealthComponent
├── AttackComponent
├── MovementComponent
└── InteractionComponent
```

**Empfohlener State-Lifecycle:**
```csharp
public void ChangeState(string stateName)
{
    var next = FindChild(stateName) as State;
    if (next == null) return;

    _CurrentState?.Exit();
    _PreviousState = _CurrentState;
    _CurrentState = next;
    _CurrentState.Enter();
}
```

---

## 4. Vergleichbare Open-Source-Godot-RPG-Projekte

Da GitHub-API-Abfragen in dieser Umgebung blockiert sind, werden nachfolgend allgemein bekannte, etablierte Godot-RPG-Referenzprojekte und deren Architekturentscheidungen zusammengefasst:

### 4.1 HeartBeast / Action RPG
- **Muster:** State Machine als Node-Baum (`StateMachine`, `State`, `Idle`, `Run`, `Attack`, `Roll`).
- **Komponenten:** Hitbox/Hurtbox als Area2D-Kinder.
- **Quelle:** HeartBeast "Godot Action RPG" Tutorial-Serie (YouTube + GitHub).

### 4.2 GDQuest / godot-2d-secrets-action-rpg
- **Muster:** State Machine + Components; saubere Trennung von Bewegung, Kampf und Animation.
- **Besonderheit:** Stat-Ressourcen und Waffen-Ressourcen für datengetriebene Ausrüstung.
- **Quelle:** GDQuest Open-Source-Beispielprojekte (github.com/gdquest).

### 4.3 gdscript-basierte RPGs wie "Tiny Swords" / "Crawl Tactics"
- **Muster:** Heavy Nutzung von Resources für Stats, Skills, Items.
- **Lernerkenntnis:** Auch in GDScript-Projekten werden Components via Sub-Nodes realisiert.

### 4.4 C# Godot RPGs
- Mehrere Community-Projekte nutzen:
  - `Character -> StateMachine -> State` Hierarchie.
  - `HealthComponent`, `StatComponent`, `InventoryComponent`.
  - `EventBus`-Autoload für lose Kopplung (besser als direkte Service-Singletons).
  - `C# partial classes` und `Resource`-Klassen mit `[GlobalClass]`.

**Gemeinsamer Nenner:** Node-HSM + Node-Components + Resource-Daten.

---

## 5. Empfohlene Architektur für das lokale Projekt

### 5.1 Kurzfristige Verbesserungen
1. **Eine StateMachine nutzen:** Alte `StateMachine/` zugunsten von `New State Machine/` auflösen; Tool-State sauber mit `AttackComponent` verbinden.
2. **State-Lifecycle korrigieren:** `Exit()` vor `Enter()` aufrufen.
3. **Generisches Movement:** `WalkState` sollte ein Interface/Export-Behavior nutzen statt hartem `PlayerMovementComponent`-Lookup.
4. **Physik-Prozess:** Nicht `Owner.SetPhysicsProcess(false)` aus States heraus verwenden; nur den State-Node deaktivieren.
5. **Singleton-Entkopplung:** `GameManager.getPlayer()._HealthComponent` in `HealItemEffectResource` durch Event/Signal ersetzen.

### 5.2 Mittelfristige Struktur
- **`Character` (Basis-Klasse):** Enthält nur gemeinsame Felder (`_CurrentLookingDirection`, `navigationAgent2D`, `_HealthComponent`).
- **Components als Kinder-Nodes:** Health, Attack, Movement, Interaction, HurtBox, HitBox.
- **StateMachine mit generischen States:** Idle, Walk, UseTool, Hurt, Die.
- **Behaviors via Interface:** `IMovementBehavior` für Player/NPC/AI.
- **EventBus-Autoload:** Für globale Ereignisse (HealthChanged, ItemPickedUp, QuestUpdate).

### 5.3 Datengetriebene Erweiterungen
- `CharacterStatResource` für HP/MP/Angriff/Verteidigung.
- `ToolResource` oder `WeaponResource` für Schaden, Reichweite, Animation.
- `QuestResource` / `DialogueResource` für RPG-Systeme.

---

## 6. Quellenverzeichnis

1. Godot Documentation v4.3: "Nodes and scenes", "Best practices" — https://docs.godotengine.org/en/4.3/
2. Godot C# basics: "C# overview" und "Exporting in C#" — https://docs.godotengine.org/en/4.3/tutorials/scripting/c_sharp/
3. HeartBeast, *Godot Action RPG* Tutorial-Serie (State-Machine-Implementierung) — https://www.youtube.com/@uheartbeast
4. GDQuest, *Godot 2D Secrets / Action RPG* Open-Source-Projekte — https://github.com/gdquest
5. Lokale Projektdateien im Workspace `/home/simon/twcs-temp`, insb.:
   - `Characters/Character.cs`
   - `Characters/Character Components/Health Component/HealthComponent.cs`
   - `Characters/Character Components/Attack Component/AttackComponent.cs`
   - `New State Machine/newStateMachine.cs`, `newState.cs`, `newWalkState.cs`, `newIdleState.cs`, `newUseToolState.cs`
   - `Characters/Behaviors/BaseMovementBehavior.cs`, `IMovementBehavior.cs`
   - `Items/ItemEffects/Heal [Item Effect Resource]/HealItemEffectResource.cs`
   - `project.godot`, `TwoWorlds CSharp.csproj`

---

*Bericht erstellt im Rahmen der Recherche zu Godot 4 C# Architekturmustern.*
