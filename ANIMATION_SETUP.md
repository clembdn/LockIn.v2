# 🎬 Configuration des Animations du Monstre

## ✅ Paramètres ajoutés à l'Animator Controller

J'ai ajouté les paramètres suivants au **MonsterMutant7 Animator Controller**:

### Paramètres

| Nom | Type | Description | Valeur par défaut |
|-----|------|-------------|-------------------|
| **Speed** | Float | Vitesse actuelle du monstre | 0 |
| **IsWalking** | Bool | Le monstre marche | false |
| **IsRunning** | Bool | Le monstre court | false |

## 🎯 Configuration automatique des états et transitions

### Méthode rapide (1 clic)

1. **Ouvrez Unity**
2. **Menu: `LockIn > Setup Monster Animator`**
3. **Cliquez "Auto-Find Animator Controller"**
4. **Cliquez "SETUP ANIMATOR STATES & TRANSITIONS"**
5. **C'est fait!** ✓

Cette méthode va automatiquement:
- ✅ Créer les états Idle, Walk et Run
- ✅ Assigner les animations idle1, walk2 et run1
- ✅ Configurer toutes les transitions
- ✅ Utiliser les paramètres Speed, IsWalking, IsRunning

## 📊 États d'animation créés

```
┌──────────────────────────────────────────┐
│  Animator: MonsterMutant7                │
├──────────────────────────────────────────┤
│                                          │
│  ┌──────┐                                │
│  │ Idle │ (État par défaut)              │
│  └──┬───┘                                │
│     │                                    │
│     ├─────▶ Walk (IsWalking = true)     │
│     │                                    │
│     └─────▶ Run (IsRunning = true)      │
│                                          │
│  ┌──────┐                                │
│  │ Walk │                                │
│  └──┬───┘                                │
│     │                                    │
│     ├─────▶ Idle (IsWalking = false)    │
│     │                                    │
│     └─────▶ Run (IsRunning = true)      │
│                                          │
│  ┌─────┐                                 │
│  │ Run │                                 │
│  └──┬──┘                                 │
│     │                                    │
│     ├─────▶ Idle (IsRunning = false)    │
│     │                                    │
│     └─────▶ Walk (IsWalking = true)     │
│                                          │
└──────────────────────────────────────────┘
```

## 🎮 Logique de comportement

Le script **MonsterAI** contrôle automatiquement les animations:

### Distance et vitesse

```
Distance au joueur > 10m  →  IsRunning = true   →  Animation Run
Distance au joueur ≤ 10m  →  IsWalking = true   →  Animation Walk
Distance au joueur ≤ 2m   →  Speed = 0          →  Animation Idle
```

### Vitesses configurables

| Paramètre | Valeur par défaut | Description |
|-----------|-------------------|-------------|
| `walkSpeed` | 2.0 | Vitesse de marche (m/s) |
| `runSpeed` | 3.5 | Vitesse de course (m/s) |
| `runDistance` | 10.0 | Distance pour commencer à courir (m) |
| `stoppingDistance` | 2.0 | Distance d'arrêt (m) |

## 🔧 Configuration manuelle (si nécessaire)

Si vous préférez configurer manuellement:

### 1. Ouvrir l'Animator Window

- Sélectionnez le monstre dans la scène
- Window > Animation > Animator

### 2. Créer les états

**État Idle:**
- Clic droit > Create State > Empty
- Nom: "Idle"
- Motion: Glissez `MutantMonster2@idle1`
- Cochez "Set as Layer Default State"

**État Walk:**
- Clic droit > Create State > Empty
- Nom: "Walk"
- Motion: Glissez `MutantMonster2@walk2`

**État Run:**
- Clic droit > Create State > Empty
- Nom: "Run"
- Motion: Glissez `MutantMonster2@run1`

### 3. Créer les transitions

**De Idle vers Walk:**
- Clic droit sur Idle > Make Transition > Walk
- Dans l'Inspector de la transition:
  - Décochez "Has Exit Time"
  - Transition Duration: 0.2
  - Conditions: IsWalking = true

**De Idle vers Run:**
- Clic droit sur Idle > Make Transition > Run
- Décochez "Has Exit Time"
- Transition Duration: 0.2
- Conditions: IsRunning = true

**De Walk vers Idle:**
- Walk > Make Transition > Idle
- Décochez "Has Exit Time"
- Conditions: IsWalking = false AND IsRunning = false

**De Walk vers Run:**
- Walk > Make Transition > Run
- Décochez "Has Exit Time"
- Conditions: IsRunning = true

**De Run vers Idle:**
- Run > Make Transition > Idle
- Décochez "Has Exit Time"
- Conditions: IsRunning = false AND IsWalking = false

**De Run vers Walk:**
- Run > Make Transition > Walk
- Décochez "Has Exit Time"
- Conditions: IsRunning = false AND IsWalking = true

## 🎬 Animations disponibles

Dans `Assets/MonsterMutant 7/Animations/`:

### Idle (au repos)
- `MutantMonster2@idle1` ⭐ (utilisé)
- `MutantMonster2@idle2`
- `MutantMonster2@idle3`
- `MutantMonster2@idle4`

### Walk (marche)
- `MutantMonster2@walk2` ⭐ (utilisé)
- `MutantMonster2@walk3`
- `MutantMonster2@walk4`
- `MutantMonster2@walkback`

### Run (course)
- `MutantMonster2@run1` ⭐ (utilisé)
- `MutantMonster2@run2`
- `MutantMonster2@run3`

### Autres (pour plus tard)
- **Attaques:** attack1, attack2, attack3, attack4, attack5
- **Dégâts:** gethit1, gethit2, gethit3, gethit4
- **Mort:** death1, death2, death3, death4
- **Autres:** jump, rage, strafeleft, straferight

## 🧪 Test

### 1. Vérifier la configuration

Menu: `LockIn > Help > Quick Start Guide`

Ou sélectionnez le monstre et vérifiez dans l'Inspector:
- ✓ Animator Controller assigné
- ✓ MonsterAI configuré avec les bonnes vitesses

### 2. Lancer le jeu

1. Appuyez sur Play ▶️
2. Le monstre devrait:
   - Idle au départ
   - Courir vers vous si vous êtes loin (>10m)
   - Marcher si vous êtes proche (<10m)
   - S'arrêter à 2m

### 3. Debug

Activez le debug dans MonsterAI:
- Sélectionnez le monstre
- Dans MonsterAI (Script)
- Cochez "Show Debug Info"
- La console affichera les états en temps réel

## 🎨 Personnalisation

### Changer les animations utilisées

Ouvrez l'Animator Window et changez les Motion dans chaque état:
- **Idle**: Changez idle1 pour idle2, idle3 ou idle4
- **Walk**: Changez walk2 pour walk3 ou walk4
- **Run**: Changez run1 pour run2 ou run3

### Ajuster les vitesses

Dans le script MonsterAI:
```csharp
walkSpeed = 2f;     // Plus lent = plus menaçant
runSpeed = 5f;      // Plus rapide = plus effrayant
runDistance = 15f;  // Court de plus loin
stoppingDistance = 1.5f;  // S'approche plus
```

### Transitions plus fluides

Dans l'Animator, ajustez la "Transition Duration":
- 0.1 = Transition rapide
- 0.3 = Transition fluide
- 0.5 = Transition très fluide

## 📋 Checklist finale

Avant de tester:

- [ ] Animator Controller a les paramètres Speed, IsWalking, IsRunning
- [ ] États Idle, Walk, Run créés
- [ ] Transitions configurées entre tous les états
- [ ] MonsterAI configuré sur le prefab
- [ ] Animations assignées dans les états
- [ ] Prefab du monstre créé
- [ ] MonsterSpawner dans la scène
- [ ] Play et tester!

## 🐛 Troubleshooting

### Le monstre ne change pas d'animation

✓ Vérifiez que l'Animator Controller est assigné
✓ Ouvrez l'Animator window en mode Play pour voir les transitions
✓ Activez "Show Debug Info" dans MonsterAI
✓ Vérifiez les paramètres dans l'onglet Parameters de l'Animator

### Le monstre court tout le temps

✓ Vérifiez que runDistance > walkSpeed
✓ Ajustez runDistance à une valeur plus grande (ex: 15)

### Les transitions sont saccadées

✓ Augmentez "Transition Duration" (essayez 0.3)
✓ Vérifiez que les animations sont en loop

### Les paramètres n'apparaissent pas

✓ Relancez Unity pour recharger l'Animator Controller
✓ Ou utilisez `LockIn > Setup Monster Animator`

---

## 🎉 Résumé

**Paramètres ajoutés:**
- ✅ Speed (Float)
- ✅ IsWalking (Bool)
- ✅ IsRunning (Bool)

**États configurés:**
- ✅ Idle (idle1)
- ✅ Walk (walk2)
- ✅ Run (run1)

**Transitions:**
- ✅ Toutes les transitions entre états configurées
- ✅ Basées sur IsWalking et IsRunning

**Script MonsterAI:**
- ✅ Contrôle automatique des paramètres
- ✅ Gestion distance/vitesse
- ✅ Debug disponible

**Tout est prêt à tester!** 🎮✨

Pour démarrer: **Play ▶️**
