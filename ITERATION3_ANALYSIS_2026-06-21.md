# Iterations-Analyse #3: Konventionen, Safety und Service-Lokalisierung

Erstellt: 2026-06-21
Repository: https://github.com/FienonTV/twcs
Branch: fix/iteration-3-cleanup
Build: 0 Warnungen, 0 Fehler

---

## 1. Zusammenfassung

Nach zwei erfolgreichen Cleanup-Runden ist der Build weiterhin stabil (0 Warnungen, 0 Fehler).
Die gröbsten Laufzeitfehler sind behoben. Die verbleibenden Probleme konzentrieren sich auf:
- Verletzung von C#-Namenskonventionen (öffentliche Member mit `_`)
- Klassen mit kleinem Anfangsbuchstaben
- Verbleibte fragil Knoten-Suche (`FindChild`, `Owner`-Zugriffe)
- Singleton-Lookups ohne zentrale Abstraktion
- Fehlende Null-Prüfungen an einigen Stellen
- Tote Code-Dateien
- Mischung von öffentlichen Feldern und Properties

Diese Iteration adressiert die niedrig hängenden Früchte, ohne die Architektur neu zu erfinden.

---

## 2. Statistiken

| Metrik | Anzahl | Trend |
|---|---|---|
| C#-Dateien | 59 | ↔ |
| `GD.Print` | 2 | ↓ (nur noch Logger selbst) |
| `GD.PrintErr` | 0 | ↓ |
| `GetNodeOrNull` | 26 | ↔ |
| `FindChild` | 18 | ↔ |
| `async void` | 0 | ↓ |
| Singleton-Pfad-Lookups | 8 | ↔ |
| Öffentliche Felder mit `_`-Präfix | 22 | ↑ |
| Klassen mit Kleinbuchstaben | 2 | ↓ (vorher 10, meist Kommentare) |

---

## 3. P1 — Konventionen und API-Qualität

### 3.1 Öffentliche Member mit `_`-Präfix
**Beispiele:**
- `Item.cs: public ItemDataResource _ItemData;`
- `Chest.cs: public string _InteractionLabel`
- `HandItem.cs: public HitBoxComponent _HitBoxComponent; public int _Damage`
- `ItemDataResource.cs: public String _Description; public Texture2D _Texture; public int _MaxStackSize`
- `AnimationController.cs: public AnimationPlayer _AnimationPlayer; public AnimationPlayer _EffectPlayer`
- `inventory_menu.cs: public bool _IsOpen`
- `SlotDataResource.cs: public ItemDataResource _ItemData; public int _Quantity`
- `InventoryDataResource.cs: public SlotDataResource[] _Slots`
- `Player.cs: public InventoryDataResource _INVENTORY_DATA`
- `CharacterMovementComponent.cs: public Character _Character; public Vector2 _StartPosition; public Vector2 _CurrentPosition`
- `WalkState.cs: public BaseMovementBehavior _MovementBehavior`
- `CharacterStateMachine.cs: public Vector2 _CurrentDirection`
- `WorldInitialization.cs: public Vector2 _PlayerSpawnPosition`

**Problem:** Verstößt gegen C#-Konventionen (öffentliche Member sollten PascalCase ohne `_` sein).

### 3.2 Klassennamen mit Kleinbuchstaben
- `data_types.cs: public partial class data_types`
- `inventory_menu.cs: public partial class inventory_menu`

**Problem:** Verstößt gegen C#-Konventionen und erschwert Lesbarkeit/Verwendung.

### 3.3 Signale/Events mit `_`-Präfix
- `HealthComponent._HealthChanged`
- `HealthComponent._MaxHealthChanged`
- `HealthComponent._HealthEmpty`
- `HurtBoxComponent.OnDamageReceived` (bereits korrigiert)
- `HitBoxComponent.OnHitboxActivated` (bereits korrigiert)
- `AttackComponent.StartAttackAnimation` (bereits korrigiert)

**Problem:** Events/Signals sollten kein `_`-Präfix haben.

---

## 4. P1 — Sicherheit und Robustheit

### 4.1 `Owner`-Zugriffe ohne Null-Check
**Dateien:**
- `WalkState.cs: Owner.FindChild(...)`
- `CharacterStateMachine.cs: Owner.GetNodeOrNull(...)`
- `UseToolState.cs: Owner.GetNodeOrNull(...)`

**Problem:** Wenn der State nicht korrekt in den Character eingebunden ist, kommt es zu `NullReferenceException`.

### 4.2 `FindChild` bleibt fragil
**Beispiele:**
- `Item.cs: FindChild("CollectableComponent", true)`
- `Sword.cs: FindChild("HitBoxComponent", recursive: true)`
- `HealItemEffectResource.cs: user.FindChild("HealthComponent", recursive: true)`
- `AnimationController.cs: FindChild("AnimationPlayer")`
- `HealthBarDisplay.cs: _CharacterParent.FindChild("HealthComponent", recursive: true)`
- `Character.cs: FindChild("HealthComponent", true)`
- `Player.cs: FindChild("HealthComponent", true)`, `FindChild("MovementComponent")`
- `PlayerInteractionComponents.cs: FindChild("InteractLabel", recursive: true)`
- `HurtBoxComponent.cs: FindChild("CooldownTimer")`
- `AttackComponent.cs: FindChild("HitBoxTimer")`
- `HitBoxComponent.cs: FindChild("CollisionShape2D", recursive: true)`
- `WalkState.cs: Owner.FindChild("MovementComponent", recursive: true)`
- `SmallTree.cs: FindChild("HurtboxComponent", recursive: true)`, `FindChild("HealthComponent", recursive: true)`

**Problem:** Änderungen im Szenenbaum brechen diese Zugriffe leicht. Export-Referenzen oder Interface-basierte Suche wären robuster.

### 4.3 Tote Dateien / ungenutzte Klassen
- `Characters/Character Components/CharacterMovementComponent.cs`
- Möglicherweise `FollowPlayerBehavior` wenn nicht in Szene verwendet?

**Problem:** Toter Code erhöht Wartungskosten und Verwirrung.

---

## 5. P2 — Architekturverbesserungen

### 5.1 Kein zentraler Service-Locator
- `GetNodeOrNull<GameManager>("/root/GameManager")`
- `GetNodeOrNull<InputHandler>("/root/InputHandler")`
- `GetNodeOrNull<inventory_menu>("/root/InventoryMenu")`

**Problem:** Pfad-Strings verteilt im Code; Umbenennen eines Autoloads bricht mehrere Stellen.

### 5.2 `Player.cs` lädt Inventory synchron in `_Ready`
```csharp
public InventoryDataResource _INVENTORY_DATA = ResourceLoader.Load<InventoryDataResource>(ResourcePaths.InventoryTres) as InventoryDataResource;
```
**Problem:** Klasse crasht beim Laden, wenn Resource fehlt. Besser: Lazy-Property mit Null-Check.

### 5.3 `inventory_menu` hat gemischte Verantwortlichkeiten
- Verwaltet Sichtbarkeit
- Hört auf InputHandler
- Liest UI-Labels
- Speichert `CurrentUser`

**Problem:** UI-Controller und Daten-Handler sind vermischt.

---

## 6. P3 — Kleinere Verbesserungen

- HealthComponent-Signale konsequent ohne `_` benennen.
- Alle `public` Felder in PascalCase ohne `_` umwandeln.
- `data_types` → `DataTypes`
- `inventory_menu` → `InventoryMenu`
- Export-Attribute für häufig gesuchte Components hinzufügen.

---

## 7. Fazit und nächste Schritte

Die nächste Iteration sollte sich auf folgende Punkte konzentrieren:
1. Öffentliche Felder von `_` auf PascalCase umstellen.
2. HealthComponent-Signale ohne `_` benennen.
3. `data_types` → `DataTypes` umbenennen.
4. `inventory_menu` → `InventoryMenu` umbenennen.
5. Tote Dateien prüfen/entfernen.
6. Weitere `FindChild`-Aufrufe durch Export-Referenzen ersetzen.

Empfohlener Branch-Name: `fix/iteration-3-cleanup`
