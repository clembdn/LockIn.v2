# LockedIn

Un jeu 3D Unity avec système de monstre.

## Installation

```bash
sudo apt install git-lfs
git lfs install
git pull
```

## 🎮 Système de Monstre MonsterMutant7

Le projet inclut un système complet pour faire apparaître et contrôler le monstre MonsterMutant7.

### 🚀 Démarrage rapide (30 secondes)

1. Ouvrez le projet dans Unity
2. Menu: **LockIn > Complete Setup: Monster + Spawner**
3. Cliquez "Oui"
4. Appuyez sur **Play** ▶️

Le monstre devrait apparaître et courir vers vous!

### 📚 Documentation

- **[AJOUTER_MONSTRE_SCENE.md](AJOUTER_MONSTRE_SCENE.md)** - Guide express pour ajouter le monstre
- **[ANIMATION_SETUP.md](ANIMATION_SETUP.md)** - Configuration des animations et paramètres ⭐ NOUVEAU
- **[GUIDE_RAPIDE_MONSTRE.md](GUIDE_RAPIDE_MONSTRE.md)** - Configuration en 5 minutes
- **[MONSTER_SETUP.md](MONSTER_SETUP.md)** - Documentation détaillée
- **[FICHIERS_CREES.md](FICHIERS_CREES.md)** - Vue d'ensemble du système
- **[GUIDE_VISUEL.md](GUIDE_VISUEL.md)** - Diagrammes et schémas

### 🛠️ Outils Unity disponibles

Dans le menu **LockIn**:
- **Complete Setup: Monster + Spawner** - Configuration automatique complète ⭐
- **Setup Monster Animator** - Configure les animations et transitions ⭐ NOUVEAU
- **Add Monster to Scene** - Fenêtre interactive
- **Add Configured Monster to Scene** - Ajout direct d'un monstre configuré
- **Quick Add Monster to SampleScene** - Setup rapide pour SampleScene
- **Help/** - Guides et dépannage

Dans le menu **GameObject > LockIn**:
- **Add Monster Here** - Ajouter un monstre à la position actuelle

### ✨ Fonctionnalités

- ✅ IA de poursuite du joueur
- ✅ Animations fluides (idle, marche, course) avec transitions automatiques
- ✅ Système de spawn automatique
- ✅ Configuration en un clic
- ✅ Outils de test et debug
- ✅ Paramètres d'animation configurables (Speed, IsWalking, IsRunning)

### 📦 Scripts créés

**Gameplay:**
- `MonsterAI.cs` - Logique de l'IA du monstre
- `MonsterSpawner.cs` - Gestion du spawn
- `MonsterSystemTester.cs` - Tests et debug

**Configuration:**
- `QuickMonsterSetup.cs` - Configuration automatique
- `MonsterAnimatorSetup.cs` - Setup de l'Animator

**Éditeur Unity:**
- `AddMonsterToScene.cs` - Outil d'ajout interactif
- `DirectMonsterAdder.cs` - Ajout direct via menus
- `MonsterSystemWelcome.cs` - Message de bienvenue et aide
- `SetupMonsterAnimator.cs` - Configuration automatique de l'Animator ⭐ NOUVEAU

### 🎬 Configuration des animations

Les paramètres suivants ont été ajoutés à l'Animator Controller:
- **Speed** (Float) - Vitesse actuelle du monstre
- **IsWalking** (Bool) - Le monstre marche
- **IsRunning** (Bool) - Le monstre court

Pour configurer automatiquement les états et transitions:
1. Menu Unity: **LockIn > Setup Monster Animator**
2. Cliquez "Auto-Find Animator Controller"
3. Cliquez "SETUP ANIMATOR STATES & TRANSITIONS"

Voir **[ANIMATION_SETUP.md](ANIMATION_SETUP.md)** pour plus de détails.

