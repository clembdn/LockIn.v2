# 🎯 Guide Visuel - Configuration du Monstre

## 📋 Vue d'ensemble rapide

```
┌─────────────────────────────────────────────────────────┐
│                  SYSTÈME DE MONSTRE                      │
│                                                          │
│  Assets/MonsterMutant 7/  ────────────────┐             │
│  (Fourni avec le projet)                  │             │
│  • Prefabs originaux                      ▼             │
│  • Animations                      [Configuration]      │
│  • Materials                              │             │
│  • Animator Controller                    │             │
│                                           ▼             │
│  Assets/Prefabs/                   Prefab Configuré    │
│  (Créé automatiquement)                   │             │
│  • ConfiguredMonsterMutant7.prefab        │             │
│                                           ▼             │
│  Scène (SampleScene)              MonsterSpawner        │
│  • MonsterSpawner ──────────────▶ Spawn au démarrage   │
│  • Player                                 │             │
│                                           ▼             │
│                                    Monstre en jeu!      │
└─────────────────────────────────────────────────────────┘
```

## 🔄 Flux de travail recommandé

### Option A: Automatique (1 minute) ⭐ RECOMMANDÉ

```
1. Menu Unity
   └─ LockIn
      └─ Complete Setup: Monster + Spawner
         └─ Cliquer "Oui"
            └─ ✓ Tout est fait!
               └─ Appuyer sur Play ▶️
```

### Option B: Manuelle (5 minutes)

```
1. Ajouter le monstre
   GameObject → LockIn → Add Monster Here
   └─ Monstre apparaît dans la scène
   
2. Configurer
   Sélectionner le monstre
   └─ Add Component → QuickMonsterSetup
      └─ Cliquer "SETUP MONSTER COMPONENTS"
         └─ Assigner Animator Controller
            └─ Cliquer "Create Prefab"
            
3. Nettoyer
   Supprimer le monstre de la scène (Delete)
   
4. Spawner
   Create Empty → Nommer "MonsterSpawner"
   └─ Add Component → MonsterSpawner
      └─ Assigner le prefab créé
         └─ Cocher "Spawn On Start"
         
5. Tester
   Appuyer sur Play ▶️
```

## 🗂️ Structure des fichiers

```
LockIn/
│
├── Assets/
│   ├── Scripts/                      ← Scripts du jeu
│   │   ├── MonsterAI.cs             ← Cerveau du monstre
│   │   ├── MonsterSpawner.cs        ← Fait spawner le monstre
│   │   ├── QuickMonsterSetup.cs     ← ⭐ Config auto
│   │   ├── MonsterAnimatorSetup.cs  ← Setup Animator
│   │   ├── MonsterSystemTester.cs   ← Tests & debug
│   │   │
│   │   └── Editor/                   ← Outils Unity Editor
│   │       ├── AddMonsterToScene.cs
│   │       ├── DirectMonsterAdder.cs
│   │       └── MonsterSystemWelcome.cs
│   │
│   ├── MonsterMutant 7/              ← Assets du monstre
│   │   ├── Prefab/                   ← Prefabs originaux
│   │   ├── Animations/               ← 35+ animations
│   │   ├── Materials/                ← 4 skins
│   │   └── MonsterMutant7 Animator Controller.controller
│   │
│   ├── Prefabs/                      ← Créé automatiquement
│   │   └── ConfiguredMonsterMutant7.prefab  ← Votre prefab prêt
│   │
│   └── Scenes/
│       └── SampleScene.unity         ← Votre scène de jeu
│
├── AJOUTER_MONSTRE_SCENE.md         ← 📘 Guide express
├── GUIDE_RAPIDE_MONSTRE.md          ← 📗 Guide 5 minutes
├── MONSTER_SETUP.md                 ← 📙 Doc complète
├── FICHIERS_CREES.md                ← 📄 Vue d'ensemble
└── README.md                         ← Ce que vous lisez
```

## 🎮 Dans Unity Editor

### Hiérarchie de la scène (Hierarchy)

```
SampleScene
├── 🎯 Player                   (Tag: "Player")
│   ├── Camera
│   └── Collider
│
├── 🌟 MonsterSpawner          (Créé par vous)
│   └── MonsterSpawner (Script)
│       ├── Monster Prefab: ConfiguredMonsterMutant7
│       ├── Spawn Offset: (5, 0, 0)
│       └── ☑ Spawn On Start
│
├── 🌍 Plane/Terrain
├── 💡 Directional Light
└── 📷 Global Volume
```

### Structure du prefab du monstre

```
ConfiguredMonsterMutant7 (Prefab)
├── 🎨 Mesh Renderer
├── 🎭 Animator
│   └── Controller: MonsterMutant7 Animator Controller
├── 📦 Capsule Collider
│   ├── Height: 2
│   ├── Radius: 0.5
│   └── Center: (0, 1, 0)
├── 💪 Rigidbody
│   ├── Mass: 80
│   ├── Use Gravity: ☑
│   └── Freeze Rotation: ☑
├── 🧠 MonsterAI (Script)
│   ├── Player: (Auto-détecté)
│   ├── Animator: (Auto-assigné)
│   ├── Move Speed: 3.5
│   └── Stopping Distance: 2.0
└── 🛠️ NavMeshAgent (Optionnel)
    ├── Speed: 3.5
    └── Stopping Distance: 2.0
```

## 📊 Paramètres principaux

### MonsterAI

| Paramètre | Type | Défaut | Description |
|-----------|------|--------|-------------|
| `player` | Transform | Auto | Le joueur à poursuivre |
| `animator` | Animator | Auto | Animator du monstre |
| `moveSpeed` | float | 3.5 | Vitesse de déplacement |
| `stoppingDistance` | float | 2.0 | Distance d'arrêt |
| `speedParameterName` | string | "Speed" | Paramètre Animator |

### MonsterSpawner

| Paramètre | Type | Défaut | Description |
|-----------|------|--------|-------------|
| `monsterPrefab` | GameObject | null | Prefab à spawner |
| `spawnOffset` | Vector3 | (5,0,0) | Position relative au joueur |
| `player` | Transform | Auto | Référence au joueur |
| `spawnOnStart` | bool | true | Spawn au démarrage |

## 🎯 Points de contrôle (Checklist)

### Avant le premier test

- [ ] Unity est ouvert
- [ ] La scène SampleScene est ouverte
- [ ] Le dossier `Assets/MonsterMutant 7/` existe
- [ ] Un objet Player existe avec le tag "Player"

### Après configuration automatique

- [ ] Un prefab existe dans `Assets/Prefabs/`
- [ ] Le prefab a le composant `MonsterAI`
- [ ] L'Animator Controller est assigné
- [ ] Un `MonsterSpawner` existe dans la scène
- [ ] Le prefab est assigné au Spawner
- [ ] "Spawn On Start" est coché

### Test réussi

- [ ] Play lancé sans erreur
- [ ] Le monstre apparaît dans la scène
- [ ] Le monstre se déplace vers le joueur
- [ ] L'animation de course joue
- [ ] Le monstre s'arrête près du joueur

## 🔍 Inspection visuelle

### Dans l'Inspector du MonsterSpawner

```
┌─────────────────────────────────────┐
│ MonsterSpawner (Script)             │
├─────────────────────────────────────┤
│ Monster Prefab                      │
│ ┌─────────────────────────────────┐ │
│ │ ConfiguredMonsterMutant7        │ │
│ └─────────────────────────────────┘ │
│                                     │
│ Spawn Offset                        │
│ X: 5    Y: 0    Z: 0               │
│                                     │
│ Player                              │
│ ┌─────────────────────────────────┐ │
│ │ Player (Transform)               │ │
│ └─────────────────────────────────┘ │
│                                     │
│ ☑ Spawn On Start                   │
└─────────────────────────────────────┘
```

### Dans l'Inspector du monstre (runtime)

```
┌─────────────────────────────────────┐
│ MonsterAI (Script)                  │
├─────────────────────────────────────┤
│ References                          │
│ Player: Player (Transform)          │
│ Animator: Animator                  │
│                                     │
│ Movement Parameters                 │
│ Move Speed: 3.5                     │
│ Stopping Distance: 2                │
│                                     │
│ Animation Parameters                │
│ Speed Parameter Name: "Speed"       │
│ Run Trigger Name: "Run"             │
└─────────────────────────────────────┘
```

## 🎨 Animator Controller

### Vue de l'Animator Window

```
┌──────────────────────────────────────────┐
│  Animator: MonsterMutant7                │
├──────────────────────────────────────────┤
│  Parameters:                             │
│  • Speed (Float)        [0.0]            │
│  • IsRunning (Bool)     [false]          │
│                                          │
│  Layers:                                 │
│  └─ Base Layer                           │
│     ├─ Idle ──────┐                      │
│     │             │ Speed > 0.1          │
│     │             └──▶ Run               │
│     │                 │                  │
│     └─────────────────┘ Speed < 0.1     │
└──────────────────────────────────────────┘
```

## 🚀 Commandes rapides

### Menus Unity

```
LockIn
├─ Complete Setup: Monster + Spawner  ⭐ Tout automatique
├─ Add Monster to Scene               📝 Fenêtre interactive
├─ Add Configured Monster to Scene    ➕ Ajout direct
├─ Quick Add Monster to SampleScene   ⚡ Setup rapide
└─ Help
   ├─ Quick Start Guide               📘 Guide rapide
   ├─ View All Documentation          📚 Tous les docs
   ├─ Troubleshooting                 🔧 Dépannage
   └─ About Monster System            ℹ️ Infos

GameObject → LockIn
└─ Add Monster Here                   ➕ Ajouter à la scène
```

## 💡 Astuces visuelles

### Gizmos dans l'éditeur

Quand le monstre est sélectionné en mode Scene:
- 🟡 **Sphère jaune** = Stopping Distance
- 🔴 **Ligne rouge** = Direction vers le joueur

### Console Unity

Messages importants à surveiller:
- ✅ `✓` = Succès
- ⚠️ `⚠` = Avertissement (non critique)
- ❌ `✗` = Erreur (à corriger)

### Couleurs dans la hiérarchie

- 🔵 **Bleu** = Prefab
- ⚪ **Blanc** = GameObject normal
- 🔴 **Rouge** = Prefab modifié (override)

## 📍 Positions par défaut

### Spawn Offset expliqué

```
        Joueur (0, 0, 0)
           ↓
    ───────●───────  Vision du joueur →
           │
           │ Y (haut)
           │
─────────────────── Z (avant/arrière)
           │
     X ←───┼───→ X (gauche/droite)
```

Exemples:
- `(5, 0, 0)` = 5m à droite du joueur
- `(-3, 0, 0)` = 3m à gauche du joueur
- `(0, 0, 10)` = 10m devant le joueur
- `(0, 0, -5)` = 5m derrière le joueur
- `(3, 1, 5)` = 3m droite, 1m haut, 5m devant

## 🎬 Séquence de démarrage

```
1. Play ▶️
   ↓
2. Scene Load
   ↓
3. MonsterSpawner.Start()
   ↓ Spawn On Start = true
4. Trouver le joueur
   ↓
5. Calculer position de spawn
   ↓ position_joueur + spawn_offset
6. Instantiate(monsterPrefab)
   ↓
7. MonsterAI.Start()
   ↓
8. Trouver le joueur
   ↓
9. Configurer NavMeshAgent
   ↓
10. Update Loop ↺
    └─ Poursuivre le joueur
    └─ Mettre à jour animations
```

---

Maintenant vous avez une vue complète du système! 🎮✨

Pour commencer: **LockIn > Complete Setup: Monster + Spawner**
