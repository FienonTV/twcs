# Analyse: Zustand, Architektur und Verbesserungspotenzial des Projekts "TwoWorlds CSharp"

Erstellt: 2026-06-21
Repository: https://github.com/FienonTV/twcs
Branch: fix/runtime-p0-issues
Build: 0 Warnungen, 0 Fehler

---

## 1. Zusammenfassung des Projekts

### 1.1 Name, Engine, Zielplattform
- **Name:** TwoWorlds CSharp
- **Engine:** Godot 4.4 Mono
- **Zielplattform:** 2D-Top-Down-PC-Spiel
- **Auflösung:** 640x360 Viewport, skaliert auf 1280x720
- **Programmiersprache:** C# (.NET 8.0.422)

### 1.2 Genre und Spielziel (abgeleitet aus dem Code)
Es handelt sich um ein **Top-Down-Action-RPG / Survival-Crafting-Spiel** in 2D-Pixeloptik mit folgenden Kernmechaniken:

1. **Bewegung und Kampf**
   - Spieler bewegt sich mit WASD in 4 Richtungen.
   - Werkzeuge (Schwert, Axt) werden per Linksklick benutzt.
   - Gegner (z.B. Minotaurus) patrouillieren oder verfolgen den Spieler.

2. **Interaktion**
   - Interaktionsbereiche (Chest, NPC, Trigger) können Text ausgeben.
   - Aktuell einfache "print_text"-Interaktion implementiert.

3. **Inventar**
   - 5-Slot-Inventar mit stapelbaren Items (Stack-Limit 99).
   - Heilitems wirken auf den Spieler.

4. **Ressourcenabbau**
   - Bäume können mit Werkzeugen geschlagen werden und droppen Logs.
   - Logs sind einsammelbar.

5. **Navigation**
   - Gegner nutzen Godot-NavigationServer2D mit NavigationPolygon-Regionen.

### 1.3 Projektstruktur

```
TwoWorlds CSharp/
├── Animation/                 # AnimationController
├── Characters/                # Player, Enemy, Components, Behaviors, StateMachines
│   ├── Behaviors/            # Patrol, Follow, Attack conditions/executions
│   ├── Character Components/ # Health, HitBox, HurtBox, Attack, Movement
│   ├── Enemies/Minotaur/    # Minotaur-Szene
│   ├── Player/               # Player-Szene, InputHandler, Interactions, Movement
│   └── (NPCInputHandler entfernt)
├── GameManager/              # GameManager, EventBus
├── Items/                    # ItemDataResource, ItemEffects, Collectables
├── Scenes/                   # Test-Szenen, Trees, Logs, WorldInitialization
├── StateMachine/             # PlayerStateMachine, ToolStateMachine, States
├── Tools/                    # HandItem, Sword, Axe
├── UI/                       # Inventory, Healthbar
├── World/                    # World-Szene, TileSet, TileMap-Logik
└── Dokumentation/            # CHANGELOG_*.md, SUMMARY_*.md, ANALYSIS_*.md
```

---

## 2. Gesamtzustand

### 2.1 Stärken
- **Build sauber:** 0 Warnungen, 0 Fehler nach dem letzten Refactoring.
- **Component-basierte Architektur:** Health, HitBox, HurtBox, Attack, Movement sind in wiederverwendbare Komponenten aufgeteilt.
- **State-Machine-Grundgerüst vorhanden:** Player und Tools haben separate StateMachines.
- **EventBus als Autoload:** Zentrale Kommunikation für PlayerSpawned, HealthChanged, EntityDied.
- **Generische Bewegung:** IMovementComponent / IMovementBehavior erlauben unterschiedliche Bewegungsstile.
- **Dokumentation:** Changelog- und Summary-Dateien werden gepflegt.

### 2.2 Schwächen
- **Viele harte Kopplungen:** StateMachines kennen direkt andere StateMachines und Nodes über Pfade.
- **Unklare Verantwortlichkeiten:** World und WorldInitialization instanziieren beide Player; GameManager hatte doppelte Rolle.
- **Szenen enthalten Redundanzen:** Test-Szenen, verwaiste Nodes, doppelte Player-Spawns.
- **Input- und Interaktionslogik gemischt:** InputHandler kennt InventoryMenu direkt.
- **Lokale Datenstrukturen statt zentraler Konfiguration:** TileMap-Logik, Item-Daten, Spawn-Positionen sind in Szenen hart codiert.
- **Kein klares Save/Load-System:** Keine Serialisierung für Spielstand sichtbar.
- **Kein Test- oder CI-Setup:** Keine Unit-Tests, kein automatisches Formatting/Linting.

---

## 3. Architekturanalyse im Detail

### 3.1 Szene-Hierarchie und Spielstart

**Startpunkt:** `project.godot` → `run/main_scene = "res://World/World.tscn"`

**World.tscn** enthält:
- Eine `World`-Node mit A*-Grid-Logik für Pathfinding.
- Einen `Y Sorted`-Container mit:
  - Player-Instanz (direkt in der Szene).
  - InteractionArea für Test-Prints.
- Einen `NPCs`-Container mit:
  - Minotaur-Instanz.
  - NavigationRegion2D.
  - "New State Machine Test Character" (TestStateMachine.tscn).
- TileMapLayers für Boden/Wände.

**Problem:** Der Player ist **zweimal** vorhanden:
1. Als Node in `World.tscn`.
2. Als PackedScene in `GameManager._PlayerScene`, die von `WorldInitialization.cs` instanziiert wird.

Das ist verwirrend und kann zu zwei Player-Instanzen führen, wenn `WorldInitialization` läuft.

### 3.2 Spieler-Charakter (Player)

**Player.tscn** besteht aus:
- `Player` (CharacterBody2D)
  - `AnimationPlayer`
  - `Sprite2D` / `AnimationController`
  - `StateMachine` (PlayerStateMachine)
    - IdleState
    - WalkState
  - `HealthComponent`
  - `HurtBoxComponent`
  - `AttackComponent`
  - `MovementComponent` (PlayerMovementComponent)
  - `Interaction Components` (PlayerInteractionComponents)
  - `Tool` (Node2D)
    - Schwert (HandItem)
    - Axt (HandItem)
    - ToolStateMachine
      - IdleState
      - UseToolState
  - `HitBoxComponent` (global am Player?)

**Beobachtung:** Der Player hat zwei HitBox-Mechanismen:
1. Eine `HitBoxComponent` direkt am Player.
2. Eine `HitBoxComponent` als Child des Tools.

Das ist doppelt gemoppelt. Das Tool sollte die einzige Quelle für Angriffs-Hitboxen sein.

### 3.3 Gegner (Minotaur)

**Minotaur.tscn** besteht aus:
- `Minotaur` (Enemy → Character)
  - `StateMachine` (NPCStateMachine)
    - IdleState
    - WalkState
    - FollowState
  - `AnimationPlayer`
  - `HurtBoxComponent`
  - `HealthComponent`
  - `HealthbarPosition`
  - `MovementComponent`
  - `PointToPointPatrolBehavior`
  - `FollowPlayerBehavior`
  - `Enemy Detection Area`

**Probleme:**
- `NPCStateMachine` sucht den Player über `GetTree().GetNodesInGroup("Player")`, was robust ist.
- Aber: `FollowPlayerBehavior` sucht ebenfalls den Player über die Gruppe. Doppelte Logik.
- `WalkState` muss wissen, ob es Follow oder Patrol ist, da die Richtung von `_MovementBehavior` kommt. Das ist aber jetzt generisch gelöst.

### 3.4 State Machine Architektur

**Hierarchie:**
```
Player
├── StateMachine (PlayerStateMachine)
│   ├── Idle
│   └── Walk
└── Tool
    └── ToolStateMachine
        ├── Idle
        └── UseTool
```

**Zuständigkeiten:**
- `PlayerStateMachine` reagiert auf Input und setzt `_CurrentDirection`.
- `WalkState` bewegt den Charakter über `IMovementComponent`.
- `ToolStateMachine` reagiert auf `_OnUseInput` und startet die Tool-Animation.
- `UseToolState` aktiviert die Hitbox des Tools.

**Kritik:**
- Guter Fortschritt: StateMachines sind jetzt generischer.
- Aber: `ToolStateMachine` muss extern über Export wissen, wer sein Owner ist. Das ist okay, aber könnte noch automatisch aus der Hierarchie abgeleitet werden.

### 3.5 Kampf-System

**Fluss:**
1. InputHandler feuert `_OnUseInput`.
2. ToolStateMachine wechselt in `UseToolState`.
3. `UseToolState` ruft `_HitBoxComponent.ActivateHitBox()` auf.
4. HitBoxComponent wird aktiv, bewegt sich vor den Character.
5. Wenn HitBox eine HurtBox berührt:
   - `HurtBoxComponent.OnAreaEntered` prüft, ob das HandItem in `_EffectiveItems` enthalten ist.
   - Falls ja, `_OnDamageRecived?.Invoke(damage)`.
6. `HealthComponent.ChangeHealth` verarbeitet Schaden, Invulnerability, Tod.

**Probleme:**
- `HurtBoxComponent` filtert nach `_HandItemCategory`. Das ist gut für "nur Axt kann Bäume fällen", aber es mischen sich Gameplay-Mechaniken in der Komponente.
- Besser: Eine `IDamageable`-Schnittstelle am HurtBox-Owner, die selbst entscheidet, ob Schaden zulässig ist.

### 3.6 Inventar und Items

**Fluss:**
1. `Item` auf der Map hat `CollectableComponent`.
2. Player sammelt ein Item auf (noch nicht vollständig sichtbar).
3. ItemDataResource wird in InventoryDataResource gepackt.
4. InventoryUI zeigt Slots an.
5. Klick auf Slot ruft `ItemDataResource.Use(Character)` auf.

**Probleme:**
- Nur ein Item-Effekt (Heal) implementiert.
- Kein Crafting, kein Equip-System, kein Drop-System aus dem Inventar.
- Inventar ist statisch mit 5 Slots — nicht erweiterbar.

### 3.7 Interaktionssystem

**Aktueller Stand:**
- `Interaction_Area` hat `InteractType` (z.B. "print_text") und `InteractValue`.
- `PlayerInteractionComponents` sammelt alle überlappenden `Interaction_Area`s und zeigt ein Label.
- Druck auf "interact" (E) führt `executeInteraction()` aus.

**Probleme:**
- Switch-Case auf Strings ist nicht erweiterbar.
- Keine generische `IInteractable`-Schnittstelle.
- Interaktion mit Chests, NPCs, Türen ist nicht wirklich implementiert (Chest.tscn existiert, aber keine Öffnungslogik).

### 3.8 World / TileMap / Navigation

**Aktueller Stand:**
- `World.cs` baut ein AStarGrid2D aus TileMapLayer-Daten.
- `NavigationRegion2D` mit NavigationPolygon für NPCs.
- Zwei verschiedene TileSets (`TileSet.tres` und `game_tile_set.tres`).

**Probleme:**
- Doppelte TileSet- und TileMap-Systeme (World vs. Test_Scene_Objects_Trees).
- `World.cs` erstellt A*-Grid, aber NPCs nutzen Godot NavigationServer2D. Zwei Pathfinding-Systeme parallel.
- A*-Grid wird nicht genutzt, weil NPCs über `NavigationAgent2D` laufen.

---

## 4. Vergleich mit Best Practices, DikuMUD und anderen Projekten

### 4.1 Godot C# Best Practices

**Was das Projekt richtig macht:**
- Components nutzen Node-Hierarchie.
- State-Pattern für Player/Tools.
- EventBus-Autoload für lose Kopplung.

**Was verbessert werden kann:**

| Best Practice | Projektstatus | Empfehlung |
|--------------|---------------|------------|
| C#-Namenskonventionen (kein `_` für publics) | Nicht eingehalten | public Felder ohne `_`, private mit `_` |
| Null-Checks mit `GetNodeOrNull` | Teilweise | Durchgängig einführen |
| Keine `async void` außer Signal-Handlern | Beseitigt | Beibehalten |
| Single Responsibility Principle | Mischung | Input, Movement, Inventory trennen |
| Service Locator statt globale statische Klassen | GameManager statisch | Services über Dependency Injection/Autoloads |
| Unit-Tests | Keine | Einführen für reine Logik |

### 4.2 DikuMUD-Vergleich

DikuMUD ist ein klassisches Text-MUD mit strikter Trennung:

| DikuMUD-Konzept | Projekt-Äquivalent | Bewertung |
|-----------------|---------------------|-----------|
| `mob` (Mobile/NPC) | `Enemy` / `Character` | Gut |
| `obj` (Objekt) | `Item` / `HandItem` | Gut |
| `room` | `World`-Szenen | Fragmentiert |
| `zone` | Fehlend | Sollte eingeführt werden |
| `command interpreter` | `InputHandler` | Zu direkt gekoppelt |
| `affect` / `spell` | `ItemEffectResource` | Nur ein Effekt |
| `fight` / `combat` | HitBox/HurtBox | Grundlage vorhanden, aber einfach |
| `reset` / `repop` | Kein System | Für Spawn/Respawn nötig |

**Empfehlung aus DikuMUD-Perspektive:**
- Führe ein zentrales **Entity/World-Template-System** ein.
- Trenne statische Daten (ItemDefinition, NPCDefinition) von laufenden Instanzen.
- Implementiere ein **Zone/Area-System**, das Entities verwaltet.

---

## 5. Schwachstellen und konkrete Verbesserungsvorschläge

### 5.1 P0 — Kritisch (Bereits größtenteils behoben)
- [x] UseToolState direkter CollisionShape-Zugriff
- [x] Heilitems heilen falschen Player
- [x] Verwaiste Signal-Verbindungen
- [x] Inventory Stapel-Limit / leere Slots

### 5.2 P1 — Hoch

#### 5.2.1 Doppelter Player-Spawn
**Problem:** Player ist in `World.tscn` und `WorldInitialization.cs`.
**Empfehlung:**
- Entferne Player aus `World.tscn`.
- `WorldInitialization` sollte der einzige Spawn-Punkt sein.
- Oder besser: Ein dedizierter `PlayerSpawnSystem`-Autoload.

#### 5.2.2 Zwei Pathfinding-Systeme
**Problem:** A*-Grid + NavigationServer2D.
**Empfehlung:**
- Entscheide dich für eines. NavigationServer2D ist für NPCs geeigneter.
- A*-Grid nur für Spieler-Maus-Ziele oder ganz entfernen.

#### 5.2.3 HitBoxComponent am Player
**Problem:** Doppelte HitBox.
**Empfehlung:**
- Entferne `HitBoxComponent` am Player-Root.
- Jede Waffe/Tools bringt ihre eigene HitBox mit.

#### 5.2.4 InputHandler kennt InventoryMenu
**Problem:** UI-Logik im InputHandler.
**Empfehlung:**
- InputHandler feuert nur `ToggleInventoryRequested`.
- Ein `UIManager`-Autoload reagiert darauf.

#### 5.2.5 HurtBoxComponent filtert nach Item-Kategorien
**Problem:** Gameplay-Regeln in Komponente.
**Empfehlung:**
- `HurtBoxComponent` feuert einfach `DamageReceived` mit Item-Daten.
- Der Owner (Tree, Enemy, Player) entscheidet über Effektivität.

### 5.3 P2 — Mittel

#### 5.3.1 Keine zentrale Entity-Factory
**Problem:** Jede Szene instanziiert eigene Entities.
**Empfehlung:**
- `EntityFactory` oder `SpawnManager` als Autoload.
- Lädt PackedScenes aus einem `EntityDatabase`.

#### 5.3.2 Kein Save/Load-System
**Problem:** Spielstand nicht speicherbar.
**Empfehlung:**
- `ISaveable`-Schnittstelle.
- `SaveManager` serialisiert Player-Position, Inventory, Quest-State.

#### 5.3.3 Kein Quest/Dialog-System
**Problem:** Interaktionen sind nur "print_text".
**Empfohlung:**
- `IInteractable`-Schnittstelle.
- Dialog-Ressourcen mit Texten und Antwortmöglichkeiten.

#### 5.3.4 Keine zentrale Audio-Verwaltung
**Problem:** Keine AudioManager-Sichtbarkeit.
**Empfehlung:**
- `AudioManager`-Autoload mit Sound-Pools.

### 5.4 P3 — Niedrig / Kosmetisch

#### 5.4.1 Namenskonventionen
- Public Felder mit `_`-Prefix korrigieren.
- Events ohne `_`-Prefix (z.B. `HealthChanged` statt `_HealthChanged`).

#### 5.4.2 Ordnerstruktur
- `Scenes/Testing/` aufsplitten in echte Szenen und Test-Szenen.
- `Characters/Player/Enemy.cs` ist irreführend benannt (Enemy im Player-Ordner).

#### 5.4.3 Magic Numbers
- Patrouillenradius, Entfernungs-Checks (300f), Stack-Limit (99) als Konstanten/Export.

---

## 6. Architektur-Empfehlung für die nächste Phase

### 6.1 Kurzfristig (sofort umsetzbar)
1. Player-Spawn eindeutig machen.
2. Redundante HitBoxComponent am Player entfernen.
3. InputHandler von InventoryMenu entkoppeln.
4. `Interaction_Area` auf `IInteractable`-Interface umstellen.
5. A*-Grid entfernen oder auf Spieler-Maus-Navigation begrenzen.

### 6.2 Mittelfristig
1. `EntityFactory` / `SpawnManager` einführen.
2. `SaveManager` mit `ISaveable`.
3. `QuestManager` und Dialog-System.
4. `AudioManager`.
5. Tool/Weapon-System erweitern: Haltbarkeit, Schadensarten, kritische Treffer.

### 6.3 Langfristig
1. Prozedurale oder zonenbasierte Welt (anstatt einer einzelnen World.tscn).
2. Netzwerk-Multiplayer (Godot MultiplayerAPI vorbereiten).
3. Mod-Support über Ressourcen-Packs.
4. Unit-Tests und CI-Pipeline.

---

## 7. Fazit

Das Projekt "TwoWorlds CSharp" ist ein vielversprechendes 2D-Top-Down-RPG mit einer bereits guten component-basierten Grundstruktur. Nach dem letzten Refactoring sind die kritischen Laufzeitfehler beseitigt und die Architektur ist deutlich generischer geworden.

Die größten verbleibenden Risiken sind:
1. Doppelte Player-Instanziierung.
2. Parallel existierende Pathfinding-Systeme.
3. UI-Logik im InputHandler.
4. Fehlende zentrale Entity-/Save-/Quest-Systeme.
5. Unklare Trennung zwischen statischen Daten und laufenden Instanzen.

Mit den vorgeschlagenen Verbesserungen entsteht eine solide Basis für ein erweiterbares Action-RPG.
