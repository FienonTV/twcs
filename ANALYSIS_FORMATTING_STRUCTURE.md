# Formatierungs- und Projektstruktur-Analyse: twcs (Godot 4.3 C#)

> Erstellt am 21.06.2026. Analyse beschränkt auf `/home/simon/twcs-temp`.

---

## 1. Zusammenfassung der Gesamteinschätzung

Das Repository ist ein frühes Godot 4.3 / C#-Projekt mit erkennbarem technischen Versuch, Klassen in logische Ordner zu trennen (Characters, Tools, UI, World, Items, State Machines). **Die größten strukturellen Probleme sind:**

1. **Inkonsistente Namenskonventionen** (PascalCase/CamelCase/_-Prefix, Dateinamen, Szenen vs. Skripte).
2. **Verwaiste/duplizierte State-Machine-Systeme** (`State Machine/` vs. `New State Machine/`).
3. **Tabulator-basierter und Leerzeilen-Chaos**, das Formatierungs-Tool CSharpier ist installiert, aber offenbar nicht durchgängig angewendet.
4. **Zu viele Kommentar-/Trennblock-Muster** statt sinnvoller, kurzer XML-Dokumentation.
5. **Public Fields statt Properties**, was Godot-Exports und Datenkapselung vermischt.
6. **Verstreute Assets** und Inkonsistenz bei Datei- / Szenenbenennung.

---

## 2. Konkrete Befunde mit Dateinennung

### 2.1 Namenskonventionen (PascalCase / CamelCase / _-Prefix)

| Befund | Beispiel(e) | Empfohlene Konvention |
|--------|-------------|----------------------|
| **Felder fast alle mit `_` Prefix** – auch public/exportierte Felder. | `Player.cs`: `_INVENTORY_DATA`, `_Name`, `_HealthComponent`; `ItemDataResource.cs`: `_Name`, `_Description`, `_Texture`; `HealthComponent.cs`: `_MaxHealth`, `_Health`. | `_` nur für `private` (nicht-exportierte) Felder. Exportierte/public Felder sollten PascalCase ohne Unterstrich sein: `MaxHealth`, `Health`, `InventoryData`. |
| **Konstanten oder static readonly Fields in CamelCase/unterstrichen.** | `InventoryUI.cs`: `static readonly PackedScene INVENTORY_SLOT`. | `static readonly` sollten PascalCase sein: `InventorySlot`. |
| **Events mit `_On`-Prefix** | `InputHandler.cs`: `_OnMoveInput`, `_OnUseInput`, `_NoMovement`; `CharacterMovementComponent.cs`: `_OnMovingPerformed`. | Events PascalCase ohne führenden Unterstrich: `MoveInput`, `UseInput`, `NoMovement`, `MovingPerformed`. |
| **Mischung aus PascalCase und camelCase in Methoden** | `EnemyDetectionArea.cs`: `checkForEnemies()` (camelCase), `InventoryDataResource.cs`: `addItem()`; `GameManager.cs`: `getPlayer()`. | Methoden immer PascalCase: `CheckForEnemies`, `AddItem`, `GetPlayer`. |
| **Parameter in PascalCase** | `ItemDataResource.Use()`, `BaseAttackExecution.ExecuteAttack(Vector2 currentPosition, Vector2 targetPosition)`. | Parameter camelCase: `currentPosition` ist korrekt; oft aber inkonsistent. |
| **Lokale Variablen in PascalCase** | `World.cs`: `TileMapSize`, `TileSize`, `LayerSize`, `MapRect`; `SmallTree.cs`: `log_instance`. | Lokale Variablen immer camelCase: `tileMapSize`, `layerSize`, `mapRect`, `logInstance`. |

**Besonders kritische Dateien:**

- `Characters/Player/Player.cs` (Zeile 11, 18, 28): `_Name` public, `_INVENTORY_DATA` public Konstantenschreibweise.
- `UI/Inventory/inventory_menu.cs`: Klasse selbst `inventory_menu` lower_snake_case.
- `Tools/data_types.cs`: Klasse `data_types` lower_snake_case, Enum `HandItemsTypes` enthält Tippfehler `MeeleeWeapon`.
- `Interaction_Area.cs`: Properties `InteractLabel`, `InteractType`, `InteractValue` PascalCase, aber Getter `getInteractLabel()` camelCase.

### 2.2 Klassen- und Dateinamen

| Befund | Datei / Klasse | Empfehlung |
|--------|----------------|------------|
| Dateiname lower_snake_case mit Klasse PascalCase | `inventory_menu.cs` → Klasse `inventory_menu` | Beides PascalCase: `InventoryMenu.cs` ↔ `InventoryMenu`. |
| Leerzeichen im Ordnernamen | `New State Machine/`, `Character Components/`, `Heal [Item Effect Resource]/` | Keine Leerzeichen in C#-Ordnern; nutze PascalCase: `StateMachines/`, `CharacterComponents/`, `ItemEffects/Heal/`. |
| Datei/Ordner gemischt Englisch/Deutsch | `newStateMachine.cs`, `newState.cs`, `newIdleState.cs` mit deutschen Kommentaren (`// Nur Gerade laufen`) | Einheitlich Englisch; aussagekräftige Namen statt `new…`, z.B. `HierarchicalStateMachine/States/IdleState.cs`. |
| Duplikat-Klassen- / Ordnernamen | `State Machine/` und `New State Machine/` existieren parallel. | Altes System löschen oder umbenennen. Empfohlen: `StateMachines/Core/` und `StateMachines/States/`. |
| Szenennamen inkonsistent | `Health_Bar_Display.tscn` vs. `HealthBarDisplay.cs`; `Inventory.tscn` vs. `inventory_menu.cs`; `State_Machine.tscn` vs. `StateMachine.cs`. | Szenennamen = Klassennamen in PascalCase: `HealthBarDisplay.tscn`, `InventoryMenu.tscn`, `StateMachine.tscn`. |
| Leer-/Placeholder-Dateien | `Characters/Behaviors/AttackInRadius.cs` ist komplett leer; `MovingRandomlyAroundBehavior.cs` ist leer; `UMLDiagram.drawio` ist 0 Bytes. | Entfernen oder mit Inhalt füllen. |

### 2.3 Einrückung und Formatierung

- **Tabulatoren vs. Leerzeichen gemischt.**
  - `Item.cs`, `InventoryDataResource.cs`, `SmallTree.cs`, `data_types.cs`, `HandItem.cs`, `newStateMachine.cs` verwenden Tabs (`\t`).
  - Andere Dateien verwenden 4 Leerzeichen.
  - **Empfehlung:** CSharpier auf das gesamte Projekt anwenden und `.editorconfig` mit `indent_style = space`, `indent_size = 4`, `charset = utf-8`, `trim_trailing_whitespace = true` hinterlegen.

- **Übermäßige Leerzeilen.**
  - `Characters/Player/Enemy.cs`: 23 Leerzeilen bei 53 Zeilen.
  - `HealthBarDisplay.cs`: 18 Leerzeilen.
  - `HitBoxComponent.cs`: 25 Leerzeilen.
  - `PlayerInteractionComponents.cs`: 29 Leerzeilen.
  - **Empfehlung:** Maximal 1 Leerzeile zwischen Methoden, keine Leerzeilen innerhalb von Methodenblöcken, außer zur logischen Trennung.

- **Lange Codezeilen.**
  - `newState.cs` Zeile 31: `NavigationServer2D.MapGetRandomPoint(...)` sehr lang.
  - `Player.cs` Zeile 28: lange `ResourceLoader.Load<...>()`-Zeile.
  - **Empfehlung:** CSharpier Standard-Zeilenumbruch (~100 Zeichen) oder `.editorconfig` mit `max_line_length = 120`.

- **Unnötige Blockkommentare/Trennlinien.**
  - Fast jede Klasse hat `/****************************** EVENTS & SIGNALS ******************************/`-Header. Das ist laut, schwer pflegbar und verhindert regionale Faltung in modernen Editoren.
  - **Empfehlung:** Entfernen und stattdessen `#region Events`, `#region Fields`, `#region Methods` verwenden – oder ganz darauf verzichten.

### 2.4 C#-Stil und Sprachfeatures

- **Public Felder statt Properties.**
  - `ItemDataResource.cs`: `public String _Description`, `public Texture2D _Texture`.
  - `SlotDataResource.cs`: `public ItemDataResource _ItemData`, `public int _Quantity`.
  - **Empfehlung:** Für logische Werte Auto-Properties verwenden: `public string Description { get; set; }`. `[Export]` funktioniert auch auf Properties, wenn `init` oder `set` vorhanden ist.

- **Null-Checks inkonsistent / unsicher.**
  - `Player.cs` Zeile 41: `_HitBoxComponent = (HitBoxComponent)FindChild(...)` → potenzielle `NullReferenceException`.
  - `ToolStateMachine.cs` Zeile 20: `Owner.Owner.GetNode<newStateMachine>("StateMachine")._CurrentDirection` → `Owner.Owner` kann null sein.
  - **Empfehlung:** `GetNodeOrNull<T>()` + null-Check verwenden, statt `GetNode<T>()` ohne Prüfung.

- **String statt string.**
  - `Player.cs`, `ItemDataResource.cs`, `Interaction_Area.cs` etc. verwenden `String` anstelle des Keywords `string`.
  - **Empfehlung:** Keywords `string`, `int`, `float`, `bool` statt Klassennamen verwenden.

- **Verwaiste/duplizierte Variablen.**
  - `Character.cs` deklariert `_StateMachine`, `_AnimationController`, `_HurtBoxComponent`, die auskommentiert oder ungenutzt sind.
  - `Player.cs` hat `_CurrentHandItem` und `_Tool`, aber `_CurrentHandItem` wird nicht initialisiert.
  - **Empfehlung:** Unbenutzte Felder entfernen, um Warnungen/Verschmutzung zu vermeiden.

- **Kommentare teilweise auf Deutsch, teilweise auf Englisch.**
  - `newState.cs`: `// Runde die Richtung auf den nächsten ganzzahligen Vektor`.
  - `StateMachine.cs`: `// Solange ein Parent existiert`.
  - **Empfehlung:** Einheitlich Englisch, da Codebase größtenteils Englisch ist.

- **Async-Methoden ohne Rückgabewert/kein Fehlerhandling.**
  - `Enemy.cs` `initialize()` als `public async void`.
  - `SmallTree.cs` `ReciveDamage(int damage)` als `public async void`.
  - **Empfehlung:** `async void` nur bei Event-Handlern; ansonsten `async Task`. Außerdem `ReciveDamage` → `ReceiveDamage` (Typo).

### 2.5 Godot-Best-Practices

- **Signal-Definitionen inkonsistent.**
  - `InventorySlotUI.cs` nutzt Godot-Signale: `FocusEntered += ItemFocused` (modern, empfohlen).
  - `CollectableComponent.cs` ruft `EmitSignal("OnItemPickedUp")` mit String-Literal – typosicher wäre `EmitSignal(SignalName.OnItemPickedUp)`.
  - **Empfehlung:** Immer `SignalName.XY` oder Nameof-ähnliche Typisierung verwenden.

- **Node-Pfade hartkodiert.**
  - `Player.cs` Zeile 42: `GetNode("/root/InputHandler")`.
  - `PlayerStateMachine.cs` Zeile 11: `GetNode("/root/InputHandler")`.
  - `InputHandler.cs` Zeile 17: `GetNode<inventory_menu>("/root/InventoryMenu")`.
  - **Empfehlung:** Durch `[Export] NodePath` oder Autoload-Typreferenzen ersetzen. Alternativ ein zentrales Service-Locator-Pattern.

- **Input-Action-Namen inkonsistent.**
  - `project.godot` definiert sowohl `interact` (klein) als auch `Interact` (PascalCase), `MoveLeft`, `MoveDown`, `MoveRight`, `MoveUp`, `UseEquippedItem`, `inventory`. InputMap hat `interact` und `Interact` doppelt, wobei `Interact` leer ist.
  - **Empfehlung:** Einheitliche Schreibweise, z.B. `move_left`, `move_right`, `interact`, `use_equipped_item`, `inventory` (Godot-Idiom) oder PascalCase `Interact`, `MoveLeft`. Nicht beides.

- **Autoload-Namen vs. Klassennamen.**
  - `project.godot`: `InputHandler`, `InventoryMenu`, `GameManager` als Autoloads. `GameManager.cs` ist ein Node, der `_PlayerScene` lädt. `InventoryMenu` ist eine Scene (`Inventory.tscn`), obwohl die Klasse `inventory_menu` heißt.
  - **Empfehlung:** Autoload-Name = Klassenname = Dateiname. `InventoryMenu.tscn` + `InventoryMenu.cs`.

- **Unused old TileMap node.**
  - `World/TileMap.cs` ist eine leere Wrapper-Klasse, die `Godot.TileMap` erweitert. Godot 4.3 nutzt `TileMapLayer`, und `World.cs` arbeitet bereits mit `TileMapLayer[]`.
  - **Empfehlung:** `World/TileMap.cs` und `World/TileMap.tscn` (falls vorhanden) entfernen.

- **Kamera im Player statt in World / dedizierter CameraRig.**
  - `Player.tscn` enthält `Camera2D`. Für Mehrspieler oder getrennte Tests wäre eine eigene `Camera2D`-Szene sinnvoller.
  - **Empfehlung:** Optional `CameraRig.tscn` erstellen, die den Player verfolgt.

### 2.6 Projektstruktur und Dateiorganisation

- **Gute Ansätze:**
  - Logische Trennung in `Characters/`, `Tools/`, `UI/`, `World/`, `Items/`.
  - Components-Muster bei `HealthComponent`, `HitBoxComponent`, `HurtBoxComponent` erkennbar.

- **Verbesserungsbedarf:**
  - `Characters/Player/Enemy.cs` liegt im `Player/`-Ordner, obwohl es sich um einen generischen NPC/Enemy handelt. Besser: `Characters/Enemies/Enemy.cs` oder `Characters/NPCs/Enemy.cs`.
  - `Characters/NPCInputHandler.cs` liegt direkt unter `Characters/`; besser zu `Characters/NPCs/` oder `Characters/Common/`.
  - `Item.cs` liegt im Root, obwohl `Items/` existiert. Besser `Items/Item.cs`.
  - `Chest.cs`, `IInteractable.cs`, `Interaction_Area.cs`, `CollectableComponent.cs` liegen im Root. Besser `Items/Interactables/` oder `Interactions/`.
  - `GameManager.cs` im Root – okay, aber `Scripts/` oder `Core/` könnte Sauberkeit erhöhen.
  - `TileSets/game_tile_set.tres` vs. `World/TileSet.tres` – Duplikat? Zumindest inkonsistente Platzierung.
  - `Scenes/Testing/` enthält Test-Szenen, aber viele Test-Assets liegen im Root (`test_szene.tscn`, `TestStateMachine.tscn`, `Interaction_Area.tscn`, `Chest.tscn`).
  - **Empfehlung:**
    - Alle Szenen nach Domäne sortieren: `Scenes/Characters/`, `Scenes/Items/`, `Scenes/UI/`, `Scenes/World/`, `Scenes/Testing/`.
    - Oder konsequent „Skript + Szene nebeneinander“ beibehalten und Root-Testdateien auflösen.

### 2.7 .csproj / Editor-Konfiguration

- **`.csproj`:**
  - `TargetFramework` ist `net6.0`; Godot 4.3 SDK empfiehlt `net8.0` für neuere Features und Performance.
  - Keine `Nullable`, `ImplicitUsings`, `TreatWarningsAsErrors` Einstellungen.
  - **Empfohlene Ergänzung:**
    ```xml
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <TreatWarningsAsErrors>false</TreatWarningsAsErrors>
    <WarningsAsErrors>Nullable</WarningsAsErrors>
    ```

- **`.editorconfig` fehlt.**
  - CSharpier ist in `.config/dotnet-tools.json` installiert, aber ohne `.editorconfig` fehlen zentrale Regeln.
  - **Empfohlene `.editorconfig`:**
    ```ini
    root = true
    [*]
    indent_style = space
    indent_size = 4
    end_of_line = lf
    charset = utf-8
    trim_trailing_whitespace = true
    insert_final_newline = true

    [*.cs]
    dotnet_sort_system_directives_first = true
    dotnet_separate_import_directive_groups = false
    csharp_new_line_before_open_brace = all
    csharp_new_line_before_else = true
    csharp_new_line_before_catch = true
    csharp_new_line_before_finally = true
    ```

- **`.vscode/settings.json`:**
  - Enthält nur better-comments-Konfiguration. Keine `omnisharp.enableRoslynAnalyzers`, kein `csharpier.enableLogging`, kein Format-On-Save.
  - **Empfehlung:** Hinzufügen:
    ```json
    "editor.formatOnSave": true,
    "[csharp]": { "editor.defaultFormatter": "csharpier.csharpier-vscode" }
    ```

- **`.gitignore` ist sehr knapp.**
  - Fehlt: `.vs/`, `bin/`, `obj/`, `.idea/`, `*.user`, `*.csproj.user`, `*.sln.cache`, `.config/dotnet-tools.json.lock`.
  - **Empfehlung:** Standard-Godot4-C#-Gitignore ergänzen.

---

## 3. Dringliche, konkrete Verbesserungsmaßnahmen

1. **Einen Lint/Format-Schritt etablieren.**
   - `dotnet tool restore`
   - `dotnet csharpier .`
   - In CI/GitHub Actions einbauen.

2. **Namenskonventionen dokumentieren und global anwenden.**
   - PascalCase für Klassen, Methoden, Properties, Events, Dateien, Szenen.
   - camelCase für Parameter und lokale Variablen.
   - `_camelCase` nur für private Felder.
   - Keine `_` für public/exportierte Felder.

3. **State-Machine-Duplikat auflösen.**
   - Entscheiden zwischen `State Machine/` (älter) und `New State Machine/` (aktueller).
   - Altes System löschen, aktuelles umbenennen in `StateMachines/`.

4. **Public Fields zu Properties migrieren.**
   - Besonders `ItemDataResource`, `SlotDataResource`, `InventoryDataResource`.

5. **Null-Sicherheit erhöhen.**
   - `Nullable` einschalten.
   - Alle `GetNode<T>()` durch `GetNodeOrNull<T>()` + Check ersetzen.

6. **InputMap bereinigen.**
   - `interact`/`Interact` deduplizieren.
   - Einheitliche Schreibweise festlegen.

7. **Verwaiste/ leere Dateien löschen.**
   - `AttackInRadius.cs`, `MovingRandomlyAroundBehavior.cs`, `UMLDiagram.drawio`, `World/TileMap.cs` (wenn ungenutzt).

8. **Szenen-Dateinamen an Skripten angleichen.**
   - `inventory_menu.cs` → `InventoryMenu.cs` + `InventoryMenu.tscn`.
   - `Health_Bar_Display.tscn` → `HealthBarDisplay.tscn`.
   - `State_Machine.tscn` → `StateMachine.tscn`.

9. **Assets neu organisieren.**
   - `Item.cs` → `Items/Item.cs`.
   - `Chest.cs` + `Chest.tscn` → `Items/Chest/` oder `Scenes/Interactables/Chest/`.
   - Root-Testdateien in `Scenes/Testing/` verschieben.

10. **Deutsche Kommentare übersetzen und TODOs auflösen.**
    - `newState.cs`, `StateMachine.cs`, `PointToPointPatrolBehavior.cs`, `NPCInputHandler.cs` enthalten deutsche Kommentare/TODOs.

---

## 4. Statistische Übersicht (C#-Codebasis)

- **C#-Dateien:** ~58
- **Verwendung von `_` Prefix:** ~571 Vorkommen (geschätzt viele public/exportierte Felder)
- **Tab-Zeilen in .cs:** 173 (6 Dateien betroffen)
- **Leerzeilen:** ~542
- **GD.Print-Debug-Ausgaben:** ~62
- **TODO/HACK/BUG/FIXME-Vorkommen:** 4 Dateien (`PointToPointPatrolBehavior.cs`, `NPCInputHandler.cs`, `Item.cs`, `newState.cs`)

---

## 5. Empfohlene Dateiumbennungen / Verschiebungen (Beispiel)

| Alt | Neu |
|-----|-----|
| `UI/Inventory/inventory_menu.cs` | `UI/Inventory/InventoryMenu.cs` |
| `Tools/data_types.cs` | `Tools/DataTypes.cs` |
| `New State Machine/newStateMachine.cs` | `StateMachines/Core/HierarchicalStateMachine.cs` |
| `New State Machine/newState.cs` | `StateMachines/Core/State.cs` |
| `Characters/Player/Enemy.cs` | `Characters/NPCs/Enemy.cs` |
| `Item.cs` | `Items/WorldItem.cs` |
| `Chest.cs` + `Chest.tscn` | `Scenes/Interactables/Chest/Chest.cs` + `Chest.tscn` |
| `Interaction_Area.cs` | `Interactions/InteractionArea.cs` |
| `Health_Bar_Display.tscn` | `UI/Healthbar/HealthBarDisplay.tscn` |
| `State_Machine.tscn` | `StateMachines/Core/StateMachine.tscn` |

---

## 6. Fazit

Das Projekt hat eine funktionale Codebasis, leidet aber unter **Inkonsistenzen in Formatierung, Namenskonventionen und Struktur**, die langfristig die Wartbarkeit und Teamarbeit erschweren. Die größten Hebel sind:

1. **CSharpier + .editorconfig global anwenden.**
2. **Eine einheitliche Namenskonvention dokumentieren und durchziehen.**
3. **Duplikate und verwaiste Systeme bereinigen.**
4. **Public Felder → Properties + Nullable aktivieren.**

Mit diesen vier Maßnahmen erreicht das Projekt einen deutlich professionelleren und wartbareren Zustand.
