# Guide Rapide - MonsterMutant7

## 🚀 Configuration en 5 minutes

### Méthode 1: Configuration automatique (Recommandée)

1. **Glissez le monstre dans la scène:**
   - Allez dans `Assets/MonsterMutant 7/Prefab/`
   - Glissez `Base mesh MonsterMutant7 skin1.prefab` dans votre scène

2. **Ajoutez QuickMonsterSetup:**
   - Sélectionnez le monstre dans la hiérarchie
   - Add Component → Quick Monster Setup
   - Dans l'Inspector, cliquez sur "⚙ SETUP MONSTER COMPONENTS"

3. **Configurez l'Animator:**
   - Toujours avec le monstre sélectionné
   - Dans le composant Animator, assignez le controller:
     - Glissez `Assets/MonsterMutant 7/MonsterMutant7 Animator Controller.controller` dans le champ "Controller"

4. **Créez le prefab:**
   - Cliquez sur "✓ Validate Configuration" pour vérifier
   - Cliquez sur "💾 Create Prefab"
   - Supprimez le monstre de la scène

5. **Ajoutez le spawner:**
   - Créez un Empty GameObject (clic droit dans Hierarchy → Create Empty)
   - Nommez-le "MonsterSpawner"
   - Add Component → Monster Spawner
   - Dans "Monster Prefab", glissez le prefab créé à l'étape 4

6. **Testez:**
   - Appuyez sur Play ▶
   - Le monstre devrait apparaître et courir vers vous!

---

### Méthode 2: Configuration manuelle

Si vous préférez tout faire manuellement:

1. **Préparez le monstre:**
   - Glissez un prefab de monstre dans la scène
   - Ajoutez ces composants:
     - MonsterAI
     - Capsule Collider (height: 2, radius: 0.5, center: 0,1,0)
     - Rigidbody (mass: 80, freeze rotation)
     - NavMeshAgent (optionnel)

2. **Configurez l'Animator:**
   - Assignez le MonsterMutant7 Animator Controller
   - Ouvrez Window → Animation → Animator
   - Créez les paramètres:
     - Speed (Float)
     - IsRunning (Bool) - optionnel

3. **Créez des états d'animation:**
   - État "Idle" avec idle1/idle2/idle3
   - État "Run" avec run1/run2/run3
   - Transition: Idle → Run (condition: Speed > 0.1)
   - Transition: Run → Idle (condition: Speed < 0.1)

4. **Créez le prefab:**
   - Glissez le monstre configuré dans Assets/Prefabs/

5. **Configurez le spawner:**
   - Comme dans la Méthode 1, étape 5

---

## 🎮 Utilisation

### Spawner le monstre automatiquement
Le MonsterSpawner spawn automatiquement au démarrage si "Spawn On Start" est coché.

### Spawner manuellement via code
```csharp
MonsterSpawner spawner = FindObjectOfType<MonsterSpawner>();
spawner.SpawnMonster();
```

### Modifier la position de spawn
Dans le MonsterSpawner:
- **Spawn Offset (5, 0, 0)** = 5 mètres à droite du joueur
- **Spawn Offset (-5, 0, 5)** = 5m à gauche, 5m devant
- **Spawn Offset (0, 0, -10)** = 10m derrière le joueur

---

## ⚙️ Paramètres

### MonsterAI
| Paramètre | Description | Valeur par défaut |
|-----------|-------------|-------------------|
| Move Speed | Vitesse de déplacement | 3.5 |
| Stopping Distance | Distance d'arrêt | 2.0 |
| Speed Parameter Name | Paramètre Animator pour vitesse | "Speed" |
| Run Trigger Name | Trigger pour animation course | "Run" |

### MonsterSpawner
| Paramètre | Description | Valeur par défaut |
|-----------|-------------|-------------------|
| Monster Prefab | Prefab du monstre | (à assigner) |
| Spawn Offset | Position relative au joueur | (5, 0, 0) |
| Spawn On Start | Spawn automatique | Coché |

---

## 🐛 Problèmes courants

### Le monstre ne se déplace pas
- ✅ Vérifiez que Move Speed > 0
- ✅ Vérifiez que le joueur est détecté (console Unity)
- ✅ Assurez-vous que le monstre a un Rigidbody

### Les animations ne jouent pas
- ✅ Vérifiez que l'Animator Controller est assigné
- ✅ Ouvrez l'Animator et vérifiez les paramètres "Speed"
- ✅ Vérifiez les transitions entre états

### Le monstre traverse le sol
- ✅ Ajoutez un Collider au terrain
- ✅ Vérifiez que le monstre a un Capsule Collider
- ✅ Assurez-vous que Use Gravity est coché sur le Rigidbody

### Le monstre spawne au mauvais endroit
- ✅ Ajustez le Spawn Offset dans le MonsterSpawner
- ✅ Vérifiez que le joueur a le tag "Player"

### "Joueur non trouvé"
- ✅ Ajoutez le tag "Player" à votre joueur:
  - Sélectionnez le joueur dans Hierarchy
  - En haut de l'Inspector: Tag → Player

---

## 📋 Checklist complète

Avant de lancer le jeu, vérifiez:

- [ ] Le prefab du monstre a le script MonsterAI
- [ ] Le prefab a un Animator avec le Controller assigné
- [ ] L'Animator a le paramètre "Speed" (Float)
- [ ] Le prefab a un Capsule Collider
- [ ] Le prefab a un Rigidbody
- [ ] Le MonsterSpawner est dans la scène
- [ ] Le MonsterSpawner a le prefab du monstre assigné
- [ ] Le joueur a le tag "Player" OU le component FirstPersonMovement
- [ ] Spawn On Start est coché (si vous voulez un spawn auto)

---

## 🎯 Prochaines étapes

Pour ajouter plus de fonctionnalités:

1. **Attaques**: Modifiez MonsterAI.cs pour ajouter une logique d'attaque
2. **Vie**: Ajoutez un système de points de vie
3. **Plusieurs monstres**: Modifiez MonsterSpawner pour spawner plusieurs instances
4. **Patrouille**: Ajoutez des waypoints avant de poursuivre le joueur
5. **Sons**: Ajoutez des AudioSource pour les bruits de pas, grognements, etc.

---

## 📞 Debug

Pour activer les messages de debug dans la console:

1. Ouvrez MonsterAI.cs
2. Ajoutez au début de la classe:
```csharp
public bool debugMode = true;
```
3. Ajoutez dans Update():
```csharp
if (debugMode)
{
    Debug.Log($"Distance au joueur: {Vector3.Distance(transform.position, player.position)}");
}
```

Bon jeu! 🎮
