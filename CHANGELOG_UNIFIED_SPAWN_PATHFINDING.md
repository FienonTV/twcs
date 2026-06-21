# CHANGELOG: Unified Spawn, Pathfinding & Interaction Fixes

Branch: `fix/unified-spawn-pathfinding`
Ziel: Player-Spawn über alle Szenen hinweg wie in `Test_Scene_Objects_Trees` vereinheitlichen, Pathfinding-Systeme reparieren, InputHandler vom Inventar entkoppeln, redundante HitBoxComponent am Player entfernen und ein generisches IInteractable-Interface einführen.

---

## 1. Setup
- Branch `fix/unified-spawn-pathfinding` von `fix/runtime-p0-issues` abgeleitet.
- Alte Protokolldateien (CHANGELOG_*, SUMMARY_*, ARCHITECTURE_*, ANALYSIS_*, GAME_ANALYSIS_*) wurden aus diesem Branch entfernt.
- Neue Protokolldateien angelegt.

## 2. Fix: Einheitlicher Player-Spawn
### Problem
Der Player war auf zwei Arten im Spiel vorhanden:
1. Als vorgefertigter Node innerhalb von `World.tscn`.
2. Über `WorldInitialization.cs`, das `GameManager._PlayerScene` instanziiert.
In `Test_Scene_Objects_Trees.tscn` war es dagegen korrekt: die Szene enthält keinen Player, sondern `WorldInitialization` spawnt ihn.

### Lösung
- Player-Node aus `World.tscn` entfernt.
- `World.tscn` bekommt stattdessen einen `WorldInitialization`-Node, der den Player aus `GameManager._PlayerScene` spawnt.
- `WorldInitialization.cs` angepasst:
  - Player wird sauber registriert und positioniert.
  - `GameManager.RegisterPlayer(_PlayerInWorld)` wird aufgerufen.
- `GameManager._PlayerScene` ist weiterhin der einzige PackedScene-Bezug.

### Dateien
- `World/World.tscn`
- `Scenes/Testing/WorldInitialization.cs`
- `GameManager.cs`

## 3. Fix: Pathfinding-Systeme vereinheitlichen
### Problem
Es existierten zwei Systeme:
1. `World.cs` baut ein AStarGrid2D aus TileMap-Daten.
2. NPCs nutzen Godot `NavigationAgent2D` + `NavigationServer2D` über `NavigationRegion2D`.
Das A*-Grid wurde überhaupt nicht genutzt, verkomplizierte aber `World.cs` und duplizierte Logik.

### Lösung
- AStarGrid2D-Logik aus `World.cs` entfernt.
- `World.cs` reduziert auf Hilfsmethoden:
  - `FindAllMapLayers()`
  - `IsPointWalkable()` (nutzt weiterhin TileMap-Layer, bleibt für zukünftige Verwendung verfügbar)
- NavigationServer2D bleibt das einzige NPC-Pathfinding.
- `PointToPointPatrolBehavior` und `FollowPlayerBehavior` Null-Checks ergänzt.
- `NavigationAgent2D` Konfiguration in Minotaur.tscn und PlayerMovementComponent geprüft.

### Dateien
- `World/World.cs`
- `Characters/Behaviors/PointToPointPatrolBehavior.cs`
- `Characters/Behaviors/FollowPlayerBehavior.cs`
- `Characters/Enemies/Minotaur/Minotaur.tscn`
- `Characters/Player/PlayerOnlyComponents/PlayerMovementComponent/PlayerMovementComponent.cs`

## 4. Fix: InputHandler vom InventoryMenu entkoppeln
### Problem
`InputHandler` hatte direkte Kenntnis von `InventoryMenu` und öffnete/schloss es selbst. Das verletzt Single Responsibility und macht Tests schwierig.

### Lösung
- `InputHandler` feuert jetzt `ToggleInventoryRequested`-Event.
- `inventory_menu` (Autoload) abonniert dieses Event und toggelt sich selbst.
- `InputHandler` kennt `InventoryMenu` nicht mehr.

### Dateien
- `Characters/Player/PlayerOnlyComponents/InputHandlerComponent/InputHandler.cs`
- `UI/Inventory/inventory_menu.cs`

## 5. Fix: Redundante HitBoxComponent am Player entfernen
### Problem
Der Player hatte eine eigene `HitBoxComponent` direkt als Child, obwohl jede Waffe/Tools ihre eigene HitBox mitbringt.

### Lösung
- `HitBoxComponent`-Node aus `Player.tscn` entfernt.
- `Player.cs` entfernt `_HitBoxComponent`-Lookup.
- Angriffe funktionieren ausschließlich über das Tool.

### Dateien
- `Characters/Player/Player.tscn`
- `Characters/Player/Player.cs`

## 6. Fix: IInteractable-Interface
### Problem
Interaktionen wurden über String-Switch (`print_text`) in `PlayerInteractionComponents` behandelt. Nicht erweiterbar.

### Lösung
- `IInteractable`-Schnittstelle erstellt:
  - `string GetInteractionLabel()`
  - `void Interact(Character user)`
- `Interaction_Area` implementiert `IInteractable` und bleibt rückwärtskompatibel.
- `PlayerInteractionComponents` arbeitet mit `IInteractable` statt String-Typen.
- Chest kann später einfach `IInteractable` implementieren.

### Dateien
- `IInteractable.cs` (neu)
- `Interaction_Area.cs` (angepasst)
- `Characters/Player/PlayerOnlyComponents/PlayerInteractionsComponent/PlayerInteractionComponents.cs`

## 7. Build-Ergebnis
- Letzter Build: 0 Warnungen, 0 Fehler.
