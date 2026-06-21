# Architektur-Analyse und Refactor-Plan: twcs

> Erstellt: 2026-06-21
> Repo: https://github.com/FienonTV/twcs
> Lokaler Pfad: /home/simon/twcs-temp
> Engine: Godot 4.3, C# (.NET 6.0)
> Genre: Top-Down 2D Action-RPG

---

## 1. Zusammenfassung des aktuellen Stands

Das Projekt `twcs` ist ein Godot-4.3-C#-Top-Down-Action-RPG mit folgenden Kernsystemen:

- **Welt/Level:** `World` mit TileMap, A*-Navigation, Spieler-Spawn
- **Charaktere:** `Character` → `Player` / `Enemy`, komponentenbasiert
- **Komponenten:** Health, HurtBox, HitBox, Attack, Movement, Interaction, Collectable
- **State Machines:** zwei parallele Systeme (alt + neu)
- **Items/Inventar:** Resource-basierte Items, Stack-Inventar, UI
- **Input:** globaler `InputHandler`-Autoload
- **Tools/Waffen:** `HandItem` → `Sword`, `Axe`

### Stärken

- Ressourcen-basierte Items sind sauber und erweiterbar.
- `HealthComponent` + `HurtBoxComponent` sind generisch und werden auch für Bäume genutzt.
- Node-Komposition wird grundsätzlich korrekt verwendet.
- Trennung von Player- und NPC-StateMachine ist richtig gedacht.

### Schwächen (überblick)

- Zwei konkurrierende State-Machine-Systeme parallel.
- Gebrochene Verknüpfungen: Angriff, Schaden, Tod, Animationen.
- Nicht-generische Bewegung: Player und NPC nutzen unterschiedliche Movement-Komponenten.
- `GameManager` hält eine globale Player-Instanz, die nicht in der Szene lebt.
- Mangelnde Null-Sicherheit, inkonsistente Namenskonventionen, auskommentierter Code.

---

## 2. Architektur-Vergleich mit Godot-Best-Practices

| Bereich | Godot-Best-Practice | twcs aktuell | Bewertung |
|---------|---------------------|--------------|-----------|
| Node-Komposition | Fähigkeiten als Kind-Nodes | Grundsätzlich vorhanden | Gut |
| State Machine | HSM als Node-Baum, `Exit()` vor `Enter()` | Zwei Systeme parallel, Reihenfolge falsch | Kritisch |
| Movement | Generisches Interface für Player & NPC | Zwei unterschiedliche Components | Schlecht |
| Tool/Weapon | Resource-basiert oder generisches `HandItem` | `Sword` hart in `HitBoxComponent` | Verbesserbar |
| Globale Services | Autoloads oder EventBus | `GameManager.getPlayer()` direkt | Schlecht |
| Item/Inventory | `[GlobalClass]` Resources | Sauber umgesetzt | Gut |
| Health/Damage | Events + generische Components | Events nicht konsistent gefeuert | Verbesserbar |

Referenzquellen / vergleichbare Muster:
- GDQuest Godot-RPG-Tutorials (State Machine als Node-Baum, Components)
- HeartBeast Action-RPG-Tutorial (Health/HitBox/HurtBox-Pattern)
- Godot-Dokumentation: Node-Komposition bevorzugt, `NavigationAgent2D` für KI, `Resources` für Daten
- Community-Pattern: `IMovementBehavior`-ähnliche Verhaltensschnittstellen, EventBus für globale Events

---

## 3. Detaillierte Probleme nach Bereich

### 3.1 State Machine

#### Altes System (`State Machine/`)

- `StateMachine : Node` mit `Dictionary<string, State>`.
- `State`-Basisklasse mit `Enter(Vector2, AnimationController)`.
- Konkrete States: `IdleState`, `MoveState`, `AttackState`, `UseToolState`.
- Im Player nicht mehr aktiv, aber im Minotaurus (`State_Machine.tscn`) noch angebunden.

#### Neues System (`New State Machine/`)

- `newStateMachine : Node2D` mit `ChangeState(string)`.
- `newState`-Basisklasse mit gemeinsamer Animationslogik.
- Konkrete States: `newIdleState`, `newWalkState`, `newUseToolState`, `newFollowState`.
- Spezialisierte State Machines: `PlayerStateMachine`, `NPCStateMachine`, `ToolStateMachine`.

#### Kritische Fehler

1. **Falsche Exit/Enter-Reihenfolge**
   ```csharp
   // newStateMachine.cs (aktuell falsch)
   _CurrentState.Enter();
   _PreviousState.Exit();
   ```
   Richtig:
   ```csharp
   _PreviousState.Exit();
   _CurrentState.Enter();
   ```

2. **States frieren den gesamten Owner ein**
   ```csharp
   // newWalkState.cs
   Owner.SetPhysicsProcess(false);
   ```
   Das stoppt Health, Interaktionen und andere Komponenten. Besser:
   ```csharp
   SetPhysicsProcess(false); // nur der State-Node selbst
   ```

3. **String-basierte Zustandssuche**
   ```csharp
   // newStateMachine.cs
   _CurrentState = FindChild(state) as newState;
   ```
   Fragil bei Umbenennungen. Besser: Dictionary oder `[Export]`-Array.

4. **ToolStateMachine greift fragil auf Owner.Owner zu**
   ```csharp
   // ToolStateMachine.cs
   _CurrentDirection = Owner.Owner.GetNode<newStateMachine>("StateMachine")._CurrentDirection;
   ```

5. **newWalkState erwartet hart `PlayerMovementComponent`**
   ```csharp
   // newWalkState.cs
   PlayerMovementComponent cmc = Owner.GetNode<PlayerMovementComponent>("MovementComponent");
   ```
   Funktioniert bei NPCs nicht, die `CharacterMovementComponent` haben.

6. **newUseToolState manipuliert Hitbox direkt**
   ```csharp
   // newUseToolState.cs
   _HitBoxComponent._CollisionShape2D.Disabled = false;
   ```
   Besser: `AttackComponent.OnAttackRequest()` aufrufen.

7. **NPCStateMachine arbeitet außerhalb der Physics-Schleife**
   - Distanzprüfung passiert in `_Process` statt `_PhysicsProcess`.

### 3.2 Component System

#### Vorhandene Components

| Component | Verantwortung | Problem |
|-----------|---------------|---------|
| `HealthComponent` | HP, MaxHP, I-Frames, Todesevent | `_HealthChanged` und `_HealthEmpty` werden nicht konsistent gefeuert |
| `HitBoxComponent` | Waffen-Hitbox aktivieren/deaktivieren | Kennt konkret `Sword` und `Character` über Baumparent-Suche |
| `HurtBoxComponent` | Schaden empfangen, Effektiv-Waffen filtern | `_CanGetDamage`-Cooldown wird nie verwendet |
| `AttackComponent` | Werkzeug-Animation triggern, Hitbox-Timer | Castet direkt auf `Player` |
| `CharacterMovementComponent` | NPC-Patrouille mit `NavigationAgent2D` | Nur horizontal, hart gekoppelt an `navigationAgent2D` |
| `PlayerMovementComponent` | Spieler-Bewegung | Sehr einfach, hart an Player gekoppelt |
| `PlayerInteractionComponents` | Interaktions-Area vor dem Spieler | Nutzt `FindParent("Player")`, viele Null-Checks fehlen |
| `CollectableComponent` | Item-Pickup | Signal-Emitt mit String-Literal |

#### Kritische Kopplungen

- `Player` hängt hart an `_PlayerMovementComponent`, `_AttackComponent`, `_Tool` (als `Sword` gecastet).
- `Enemy.cs` referenziert `PlayerMovementComponent` statt generischer Bewegung.
- `AttackComponent.OnAttackRequest` castet direkt auf `Player`.
- `HitBoxComponent` sucht `Sword` und `Character` über Parent-Walk statt über Export/Interface.

### 3.3 Globale Architektur

#### GameManager

```csharp
// GameManager.cs
public static PackedScene _PlayerScene;
private static Player _Player;
```

Probleme:
- `_Player` wird in `_Ready()` instanziiert, aber nie `AddChild()` hinzugefügt.
- `World` und `WorldInitialization` instanziieren eigene Player.
- `HealItemEffectResource` ruft `GameManager.getPlayer()` auf — heilt den falschen Player.

#### InputHandler

- Globaler Autoload, sendet Events.
- `_OnUseInput` und `_OnInteractionInput` werden unsicher mit `.Invoke()` aufgerufen.
- `Interact`-Input-Action hat keine Events in der InputMap hinterlegt.

#### Inventar

- Resource hat 5 Slots (`Player_Inventory.tres`), UI instanziiert 30 Slots.
- `addItem` stapelt unbegrenzt ohne Maximalmenge.
- `ItemPressed` reduziert `_Quantity` ohne Limit-Prüfung.

---

## 4. Build-Blocker und Laufzeitfehler

### Build-Blocker

1. **Duplicate class `PointToPointPatrolBehavior`**
   - `MovingRandomlyAroundBehavior.cs`
   - `PointToPointPatrolBehavior.cs`

2. **Nicht existierende Methode `_on_area_entered`**
   - Referenziert in `Minotaur.tscn`, existiert in `HurtBoxComponent.cs` als `OnAreaEntered`.

3. **Nicht existierende Methode `_FinishedAnimation`**
   - Referenziert in `Player.tscn`, fehlt in `Player.cs`.

4. **Nicht existierende Methode `_on_inventory_inventory_active`**
   - Referenziert in `Inventory.tscn`, fehlt in `InventoryUI.cs`.

### Laufzeitfehler (hohe Wahrscheinlichkeit)

- Angriff funktioniert nicht (Zeile in `Player.cs` auskommentiert).
- Schaden/Tod funktioniert nicht (Events in `Character.cs` auskommentiert).
- Heilitems haben keine Wirkung (falscher Player).
- Gegner crashen im Walk-State (falsche Movement-Komponente).
- NullRef in `HitBoxComponent._Process` wenn kein Character-Parent.
- NullRef in `newUseToolState` wenn kein CollisionShape.
- IndexOutOfRange in `newStateMachine` wenn kein Spieler in Gruppe.

---

## 5. Entscheidung: Refactor oder Neuanfang?

### Entscheidung: Refactor

Begründung:
- Szenen, Animationen, Assets und Ressourcen sind vorhanden.
- Grundlegendes Node-Component-Muster ist korrekt.
- Ressourcen-System und Health/HurtBox sind bereits gut.
- Ein Neuanfang würde den Großteil der Arbeit neu machen, ohne proportionalen Mehrwert.
- Die Probleme liegen in der **konkreten Ausführung**, nicht im gewählten Pattern.

### Was beibehalten werden sollte

- Node-Komposition als Architektur-Pattern.
- Resource-basierte Items (`ItemDataResource`, `ItemEffectResource`).
- Generische `HealthComponent` + `HurtBoxComponent`.
- Hierarchical State Machine als Node-Baum.
- Trennung Player/NPC StateMachine.

### Was geändert werden muss

- Nur **eine** State Machine statt zwei.
- Generische Bewegungsschnittstelle für Player und NPC.
- Event-getriebene Kommunikation statt Singleton-Zugriffe.
- Korrekter State-Lifecycle (`Exit` vor `Enter`).
- Saubere Null-Checks und einheitliche Konventionen.

---

## 6. Refactor-Plan

### Phase 1: Grundlagen sicherstellen

Ziel: Das Projekt baut und läuft stabil, bevor die Architektur umgebaut wird.

| # | Aufgabe | Dateien |
|---|---------|---------|
| 1.1 | Doppelte Klasse `PointToPointPatrolBehavior` beheben | `MovingRandomlyAroundBehavior.cs`, `PointToPointPatrolBehavior.cs` |
| 1.2 | Fehlende Signal-Handler anlegen oder Verbindungen korrigieren | `Player.tscn`, `Minotaur.tscn`, `Inventory.tscn` |
| 1.3 | Auskommentierte Verknüpfungen in `Character.cs` wiederherstellen | `Character.cs` |
| 1.4 | Angriffssystem aktivieren und sicher machen | `Player.cs`, `AttackComponent.cs` |
| 1.5 | Health-Events konsistent feuern | `HealthComponent.cs` |

### Phase 2: State-Machine-Konsolidierung

Ziel: Nur noch eine State Machine, korrekter Lifecycle.

| # | Aufgabe | Dateien |
|---|---------|---------|
| 2.1 | Altes `State Machine/`-System entfernen | `State Machine/` Ordner, `Minotaur.tscn` |
| 2.2 | `New State Machine/` umbenennen in `StateMachine/` | alle Dateien im Ordner |
| 2.3 | Klassen umbenennen: `newStateMachine` → `CharacterStateMachine`, `newState` → `CharacterState` | entsprechende `.cs`-Dateien |
| 2.4 | `Exit()` vor `Enter()` aufrufen | `CharacterStateMachine.cs` |
| 2.5 | `SetPhysicsProcess` nur auf State-Node anwenden | `WalkState.cs`, `IdleState.cs` |
| 2.6 | Zustände per Dictionary oder `[Export]`-Array auflösen | `CharacterStateMachine.cs` |
| 2.7 | ToolStateMachine vom Owner.Owner-Kopplung befreien | `ToolStateMachine.cs` |

### Phase 3: Generisches Movement

Ziel: Player und NPC nutzen dieselbe Bewegungs-Schnittstelle.

| # | Aufgabe | Dateien |
|---|---------|---------|
| 3.1 | Interface `IMovementComponent` einführen | neues `IMovementComponent.cs` |
| 3.2 | `PlayerMovementComponent` und `CharacterMovementComponent` implementieren | `PlayerMovementComponent.cs`, `CharacterMovementComponent.cs` |
| 3.3 | `WalkState` generisch auf `IMovementComponent` umstellen | `WalkState.cs` |
| 3.4 | `Enemy.cs` auf generische Bewegung umstellen | `Enemy.cs` |

### Phase 4: Tool/Attack-System entkoppeln

Ziel: Jede Waffe funktioniert über `HandItem`, nicht nur `Sword`.

| # | Aufgabe | Dateien |
|---|---------|---------|
| 4.1 | `HitBoxComponent` kennt `HandItem` statt `Sword` | `HitBoxComponent.cs`, `HandItem.cs` |
| 4.2 | `AttackComponent` auf generisches `Character` umstellen | `AttackComponent.cs` |
| 4.3 | `UseToolState` nutzt `AttackComponent.OnAttackRequest()` | `UseToolState.cs` |
| 4.4 | `Player._Tool` als `HandItem` casten | `Player.cs` |

### Phase 5: Globale Architektur bereinigen

Ziel: Keine harten Singleton-Zugriffe mehr.

| # | Aufgabe | Dateien |
|---|---------|---------|
| 5.1 | Player-Spawn aus `GameManager` entfernen, in `World`/`LevelBootstrap` zentralisieren | `GameManager.cs`, `World.cs`, `WorldInitialization.cs` |
| 5.2 | `HealItemEffectResource` zielorientiert machen | `HealItemEffectResource.cs` |
| 5.3 | EventBus-Autoload für globale Events einführen | neues `EventBus.cs` |
| 5.4 | `HealthBarDisplay` auf `_HealthChanged`-Event umstellen | `HealthBarDisplay.cs`, `HealthComponent.cs` |

### Phase 6: Inventar und Items stabilisieren

Ziel: Inventar funktioniert konsistent.

| # | Aufgabe | Dateien |
|---|---------|---------|
| 6.1 | Slot-Anzahl in Resource und UI synchronisieren | `Player_Inventory.tres`, `Inventory.tscn` |
| 6.2 | Stapel-Limit einführen | `InventoryDataResource.cs` |
| 6.3 | Leere Slots entfernen, wenn Menge 0 erreicht | `InventorySlotUI.cs`, `InventoryDataResource.cs` |
| 6.4 | `CollectableComponent` auf `SignalName` umstellen | `CollectableComponent.cs` |

### Phase 7: Code-Qualität und Konventionen

Ziel: Langfristige Wartbarkeit.

| # | Aufgabe | Dateien |
|---|---------|---------|
| 7.1 | `.editorconfig` einführen | `.editorconfig` |
| 7.2 | CSharpier / "Format on Save" aktivieren | `.vscode/settings.json` |
| 7.3 | `Nullable` im `.csproj` aktivieren | `TwoWorlds CSharp.csproj` |
| 7.4 | Einheitliche Namenskonventionen durchziehen | alle `.cs`-Dateien |
| 7.5 | Public Felder zu Auto-Properties migrieren | Resource-Klassen |
| 7.6 | Tote Dateien und auskommentierten Code entfernen | diverse |

---

## 7. Kurzfristige Maßnahmen (erste 1–2 Sitzungen)

1. Build-Blocker beheben.
2. Schadenspipeline wiederherstellen.
3. Angriffssystem aktivieren.
4. Altes State-Machine-System entfernen.
5. `Exit()`/`Enter()`-Reihenfolge korrigieren.

Diese fünf Schritte machen das Projekt lauf- und spielbar. Die tieferen Refactor-Phasen können danach Schritt für Schritt erfolgen.

---

## 8. Offene Fragen / Entscheidungen

- Soll das Spiel Singleplayer bleiben oder Multiplayer-Fähigkeit angestrebt werden? (Beeinflusst `GameManager`- und Player-Referenz-Design.)
- Soll der Minotaurus ein aktiver Gegner bleiben oder ist er ein Test-Asset?
- Soll es ein zentrales EventBus-System geben oder eher direkte Node-Signal-Verbindungen?
- Welche Stapel-Limit sollen Items haben? (z. B. 99 pro Slot?)
- Soll die Projektstruktur nach Domänen (Player, Enemy, UI, World, Items) oder nach Typ (Scripts, Scenes, Resources) organisiert werden?

---

## 9. Verwandte Dateien

- `ANALYSIS_FORMATTING_STRUCTURE.md` — Formatierungs- und Strukturanalyse
- `RESEARCH_Godot4_CSharp_Architektur.md` — Recherche zu Godot-4-C#-Patterns

---

*Diese Datei dient als zentrale Planungsgrundlage für die weitere Arbeit am twcs-Repo.*
