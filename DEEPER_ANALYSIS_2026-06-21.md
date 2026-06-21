# Erweiterte Analyse #2: Architektur, Laufzeitfehler und Code-Smells

Erstellt: 2026-06-21
Repository: https://github.com/FienonTV/twcs
Branch: fix/deeper-analysis-cleanup
Build: 0 Warnungen, 0 Fehler

---

## 1. Zusammenfassung des aktuellen Zustands

Nach den letzten drei Fix-Runden (P0-Runtime, Unified Spawn/Pathfinding, Services/Logger) ist der Build stabil:
- **C#-Dateien:** 59
- **Build-Fehler:** 0
- **Build-Warnungen:** 0
- **`async void`:** 0
- **`Task`:** 0

Die gröbsten Architekturrisiken sind behoben:
- GameManager ist nicht mehr statisch.
- Pathfinding wurde vereinfacht.
- Player-Spawn läuft über `WorldInitialization`.
- Ein Logger und zentrale Resource-Pfade existieren.

Trotzdem bleiben **Laufzeitfehler, Design-Smells und Architektur-Lücken**, die beim tatsächlichen Spielen zuschlagen können. Diese Analyse listet sie priorisiert auf.

---

## 2. Statistiken

| Metrik | Anzahl | Bewertung |
|---|---|---|
| C#-Dateien | 59 | okay |
| `GD.Print` | 8 | 6 sollten in Logger/Debug umgezogen werden |
| `GD.PrintErr` | 5 | sollten `Logger.Error` sein |
| `GetNodeOrNull` | 21 | viele notwendig, aber 8 Singleton-Lookups |
| `FindChild` | 18 | teilweise fragile Knoten-Suche |
| `GetNode` (hard) | 3 | kann Laufzeit-Exceptions werfen |
| Hardcodierte `res://` | 5 | alle in `ResourcePaths.cs` — akzeptabel |
| Singleton-Autoload-Lookups | 8 | mittelhohe Kopplung |

---

## 3. P0 — Laufzeitfehler (kritisch)

### 3.1 `HealthBarDisplay._Ready()` wirft, wenn kein `HealthComponent` existiert
**Datei:** `UI/Healthbar/HealthBarDisplay.cs:31`
```csharp
_HealthComponent = _CharacterParent.FindChild("HealthComponent", recursive: true) as HealthComponent;
_Healthbar.MaxValue = _HealthComponent.GetMaxHealth();  // NullReferenceException
```
Wenn ein Charakter kein `HealthComponent` hat, stürzt die HealthBar ab.

### 3.2 `BaseMovementBehavior._Ready()` wirft, wenn kein Player in der Szene ist
**Datei:** `Characters/Behaviors/BaseMovementBehavior.cs:15`
```csharp
_CurrentScenePlayer = GetTree().GetNodesInGroup("Player")[0] as Player;
```
Falls die Gruppe leer ist (z.B. Test-Szene ohne Player), kommt es zu einem `IndexOutOfRangeException`.

### 3.3 `PlayerStateMachine` und `ToolStateMachine` holen `InputHandler` über Autoload-Pfad
**Dateien:**
- `StateMachine/PlayerStateMachine.cs:11`
- `StateMachine/ToolStateMachine.cs:14`
```csharp
_InputHandler = GetNodeOrNull<InputHandler>("/root/InputHandler");
```
Das funktioniert nur, wenn der Autoload-Name exakt `InputHandler` ist. Umbenennen oder Fehlkonfiguration = stiller Fehler.

### 3.4 `PlayerMovementComponent` hängt hart am `Player`-Typ
**Datei:** `Characters/Player/PlayerOnlyComponents/PlayerMovementComponent/PlayerMovementComponent.cs:14`
```csharp
_Player = GetParent() as Player;
```
NPCs können diese Komponente nicht wiederverwenden. `WalkState` ist generisch, aber die Movement-Komponente nicht.

---

## 4. P1 — Architekturrisiken (hoch)

### 4.1 Doppeltes Animation-System
**Vorhanden:**
- `AnimationController` ( separates Skript)
- `AnimationPlayer` direkt in `CharacterStateMachine`

**Problem:** Zwei parallele Wege, Animationen zu steuern. Das führt leicht zu Race Conditions oder überschriebenen Animationen.

**Empfohlene Lösung:**
- `CharacterStateMachine` sollte ausschließlich `AnimationController` nutzen.
- `AnimationController` kapselt `AnimationPlayer` und `EffectPlayer`.

### 4.2 `Character` mischt öffentliche Felder und interne Felder
**Datei:** `Characters/Character.cs`
```csharp
[Export] public NavigationAgent2D navigationAgent2D;   // öffentlich
public HealthComponent HealthComponent;               // öffentlich
protected CharacterStateMachine StateMachine;         // intern
protected AnimationController _AnimationController;     // intern
public Vector2 CurrentLookingDirection = Vector2.Down;  // öffentliches Feld
```
**Problem:** Keine klare Trennung zwischen API und Interna. Exportierte Felder haben `_`-Präfix verloren, was gegen C#-Konventionen verstößt.

### 4.3 `PlayerInteractionComponents` nutzt `FindParent("Player")`
**Datei:** `Characters/Player/PlayerOnlyComponents/PlayerInteractionsComponent/PlayerInteractionComponents.cs:20`
```csharp
_CharacterParent = FindParent("Player") as Player;
```
**Problem:** Fragil, wenn der Player-Node anders benannt wird. Besser: `[Export] Character _CharacterParent` oder über Parent-Hierarchie iterieren.

### 4.4 `InventoryUI` nutzt statisches `PackedScene`
**Datei:** `UI/Inventory/Scripts/InventoryUI.cs`
```csharp
static readonly PackedScene InventorySlot = ResourceLoader.Load<PackedScene>(ResourcePaths.InventorySlotScene);
```
**Problem:** Wird beim ersten Klassen-Zugriff geladen. Bei Fehlern im Pfad kommt es zu einem sofortigen Crash ohne Kontext.

### 4.5 `InventoryUI.UpdateInventory()` dupliziert Slots nicht, sondern appended
**Datei:** `UI/Inventory/Scripts/InventoryUI.cs`
```csharp
foreach (SlotDataResource s in _Data._Slots)
{
    InventorySlotUI slot = InventorySlot.Instantiate() as InventorySlotUI;
    AddChild(slot);
    slot.SlotData = s;
}
```
**Problem:** Bei jedem Öffnen werden neue Slots hinzugefügt, ohne vorherige zu löschen. Nach mehrfachem Öffnen sind Slots mehrfach vorhanden.

### 4.6 `AttackComponent` sucht `HitBoxComponent` beim Parent
**Datei:** `Characters/Character Components/Attack Component/AttackComponent.cs:32`
```csharp
_HitBoxComponent = GetParent().FindChild("HitBoxComponent", recursive: true) as HitBoxComponent;
```
**Problem:** Sucht im gesamten Character-Baum. Besser: `[Export] HitBoxComponent _HitBoxComponent` oder über Tool-Node.

### 4.7 `HitBoxComponent` hat Referenz auf `_Tool` aber nicht auf `_OwnerCharacter`
**Datei:** `Characters/Character Components/Hit Box Component/HitBoxComponent.cs`
**Problem:** Die Hitbox weiß nicht, wer sie schwingt. Für Knockback, Schadensquelle oder Statistik braucht man den Besitzer.

### 4.8 `AnimationController` wird nicht mehr verwendet, obwohl er existiert
**Datei:** `Animation/AnimationController.cs`
**Problem:** `CharacterStateMachine.ResolveAnimationPlayer()` sucht direkt `AnimationPlayer`. Der `AnimationController` ist tot Code. Entweder reaktivieren oder entfernen.

---

## 5. P2 — Code-Smells (mittel)

### 5.1 Verbleibende `GD.Print` und `GD.PrintErr`
- `Logger.cs:23,31` — akzeptabel (Logger selbst)
- `InventoryDataResource.cs:53,57`
- `HealthBarDisplay.cs:92`
- `PlayerInteractionComponents.cs:88`
- `BaseMovementBehavior.cs:18,26`
- `PlayerInteractionsComponent.cs:23,29,37`
- `PlayerMovementComponent.cs:17,25`

### 5.2 `PlayerInteractionComponents` hat deutschen Kommentar
**Datei:** `UI/Healthbar/HealthBarDisplay.cs:91-92`
```csharp
// Charakter gefunden, zurÃ¼ckgeben
GD.Print("Parent gefunden");
```
**Problem:** Vermischung von Sprachen im Code.

### 5.3 `inventory_menu` ist eine Klasse mit kleinem `i`
**Problem:** Verstößt gegen C#-Namenskonventionen. Sollte `InventoryMenu` heißen. Godot erlaubt das, aber es ist ein Smell.

### 5.4 `HealthComponent` und `HurtBoxComponent` Signals haben `_`-Präfix
Beispiele:
- `_HealthChanged`
- `_OnDamageRecived` (auch noch Rechtschreibfehler: "Recived")

**Problem:** Signals sollten keine `_`-Präfixe haben.

### 5.5 `WorldInitialization` hat hardcodierte Spawn-Position
```csharp
public Vector2 _PlayerSpawnPosition = new Vector2(150, 150);
```
**Problem:** Für jede Szene muss der Export überschrieben werden. Akzeptabel, aber dokumentiert werden.

### 5.6 `FollowPlayerBehavior` holt Player in `_Ready()`
```csharp
_GameManager = GetNodeOrNull<GameManager>("/root/GameManager");
```
**Problem:** Falls Player später spawnt, ist `_Target` dauerhaft null. Besser: Lazy in `GetNextDirection()`.

---

## 6. P3 — Verbesserungspotenzial (niedrig)

### 6.1 Kein zentrales Service-Locator-Interface
- `GetNodeOrNull` für Autoloads überall verstreut.
- Besser: `Services.Get<T>()` oder Dependency Injection über `[Export]`.

### 6.2 Kein Save/Load-System
### 6.3 Keine Quest- oder Dialog-Datenbank
### 6.4 Kein Equip-Wechsel zur Laufzeit
### 6.5 Keine zentrale Konstanten-Datei für Input-Actions
### 6.6 Keine Tests (Unit, Integration, Scene-Tests)

---

## 7. Architektur-Vergleich mit Godot-Best-Practice

| Bereich | Aktuell | Best Practice | Gap |
|---|---|---|---|
| Autoloads | 4 (GameManager, InputHandler, InventoryMenu, EventBus) | 2-3 Core-Services | leicht überfrachtet |
| State-Machine | State-Pattern mit Registry | Hierarchical/Switch | akzeptabel |
| Input | InputHandler emittiert Events | Actions + Signals | gut |
| Animation | Controller + Player parallel | Controller kapselt Player | mittel |
| Bewegung | WalkState generisch, aber PlayerMovementComponent nur für Player | Generische IMovementComponent | mittel |
| Interaktion | IInteractable eingeführt | IInteractable + Area2D | gut |
| Ressourcen | ResourcePaths zentral | ResourcePaths / Addressables | gut |
| Logging | Logger eingeführt | Logger mit Leveln | gut |

---

## 8. Handlungsempfehlung

### Sofort (P0)
1. `HealthBarDisplay` null-sicher machen.
2. `BaseMovementBehavior` gegen leere Player-Gruppe absichern.
3. `PlayerStateMachine`/`ToolStateMachine` InputHandler-Lookup robuster machen.

### Kurzfristig (P1)
4. AnimationController reaktivieren oder entfernen.
5. `PlayerMovementComponent` generisch für Character machen.
6. `PlayerInteractionComponents` ohne `FindParent("Player")`.
7. `InventoryUI.UpdateInventory()` vorher leeren.
8. `AttackComponent` und `HitBoxComponent` über Export-Referenzen verbinden.

### Mittelfristig (P2/P3)
9. Verbleibende `GD.Print`/`GD.PrintErr` bereinigen.
10. Signale und Klassen nach C#-Konventionen benennen.
11. Service-Locator oder DI einführen.
12. Save/Load, Quests, Equip-System bauen.

---

## 9. Fazit

Das Projekt ist deutlich stabiler als noch vor einigen Stunden. Die größten Crash-Quellen sind behoben oder klar identifiziert. Die nächste Priorität liegt darin, die verbleibenden Laufzeit-NullPointer und die parallele Animation-Infrastruktur zu bereinigen, bevor neue Features wie Quests oder Save/Load hinzukommen.

Empfohlener nächster Branch-Name: `fix/runtime-nulls-animation`
