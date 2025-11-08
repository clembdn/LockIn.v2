# Configuration du MonsterMutant7

Ce guide explique comment configurer le monstre MonsterMutant7 pour qu'il spawne et coure vers le joueur.

## Scripts créés

1. **MonsterAI.cs** - Contrôle le comportement du monstre (poursuite du joueur)
2. **MonsterSpawner.cs** - Gère l'apparition du monstre dans le jeu

## Configuration étape par étape

### Étape 1: Préparer le prefab du monstre

1. Ouvrez le dossier `Assets/MonsterMutant 7/Prefab/`
2. Glissez l'un des prefabs (par exemple `Base mesh MonsterMutant7 skin1.prefab`) dans votre scène pour le configurer
3. Sélectionnez le monstre dans la hiérarchie

### Étape 2: Ajouter les composants au monstre

1. **Ajouter le script MonsterAI:**
   - Avec le monstre sélectionné, cliquez sur "Add Component"
   - Recherchez "MonsterAI" et ajoutez-le
   
2. **Configurer l'Animator:**
   - Le script devrait détecter automatiquement l'Animator
   - Si ce n'est pas le cas, assignez l'Animator Controller manuellement
   - Ouvrez `Assets/MonsterMutant 7/MonsterMutant7 Animator Controller.controller`
   - Vérifiez les paramètres disponibles (Speed, Run, etc.)

3. **Ajouter un NavMeshAgent (optionnel):**
   - Si vous avez un NavMesh dans votre scène, le script l'utilisera automatiquement
   - Sinon, il utilisera un mouvement simple vers le joueur

4. **Ajouter un Collider (si absent):**
   - Add Component > Capsule Collider
   - Ajustez la taille pour correspondre au modèle

5. **Ajouter un Rigidbody (recommandé):**
   - Add Component > Rigidbody
   - Cochez "Use Gravity"
   - Cochez "Is Kinematic" si vous utilisez NavMesh

### Étape 3: Créer le prefab configuré

1. Une fois le monstre configuré, glissez-le du Hierarchy vers le dossier `Assets/Prefabs/` (créez le dossier si nécessaire)
2. Supprimez le monstre de la scène
3. Vous avez maintenant un prefab prêt à être spawné!

### Étape 4: Configurer le Spawner

1. **Créer un GameObject Spawner:**
   - Dans la hiérarchie, cliquez droit > Create Empty
   - Nommez-le "MonsterSpawner"

2. **Ajouter le script MonsterSpawner:**
   - Sélectionnez MonsterSpawner
   - Add Component > MonsterSpawner

3. **Configurer le Spawner:**
   - **Monster Prefab:** Glissez votre prefab de monstre configuré
   - **Spawn Offset:** Ajustez la position (par défaut: 5m à droite du joueur)
   - **Player:** Laissez vide (sera trouvé automatiquement)
   - **Spawn On Start:** Cochez pour spawner au démarrage

### Étape 5: Configurer le joueur (si nécessaire)

Si le script ne trouve pas le joueur automatiquement:

1. Sélectionnez votre joueur dans la hiérarchie
2. Dans l'Inspector, en haut, assignez le tag "Player"
3. Ou assignez manuellement le joueur dans les scripts

### Étape 6: Configurer l'Animator Controller

Pour que les animations fonctionnent correctement:

1. Ouvrez `MonsterMutant7 Animator Controller` (double-clic)
2. Vérifiez/créez les paramètres suivants dans l'onglet Parameters:
   - **Speed** (Float) - Contrôle la vitesse d'animation
   - **Run** (Trigger, optionnel) - Déclenche l'animation de course

3. Configurez les transitions:
   - Idle → Run (quand Speed > 0.1)
   - Run → Idle (quand Speed < 0.1)

4. Animations recommandées:
   - **Idle:** `MutantMonster2@idle1` ou similaire
   - **Run:** `MutantMonster2@run1`, `run2`, ou `run3`

### Étape 7: Tester

1. Sauvegardez votre scène
2. Appuyez sur Play
3. Le monstre devrait apparaître à côté du joueur et courir vers lui
4. Il s'arrêtera à la distance définie (2m par défaut)

## Paramètres du MonsterAI

- **Move Speed:** Vitesse de déplacement (défaut: 3.5)
- **Stopping Distance:** Distance d'arrêt par rapport au joueur (défaut: 2m)
- **Speed Parameter Name:** Nom du paramètre dans l'Animator (défaut: "Speed")
- **Run Trigger Name:** Nom du trigger de course (défaut: "Run")

## Paramètres du MonsterSpawner

- **Spawn Offset:** Position relative au joueur (x, y, z)
  - X: gauche(-)/droite(+)
  - Y: bas(-)/haut(+)
  - Z: arrière(-)/avant(+)
- **Spawn On Start:** Active le spawn automatique au démarrage

## Troubleshooting

### Le monstre ne bouge pas
- Vérifiez que le joueur est bien détecté (check la console)
- Assurez-vous que le monstre a un Rigidbody ou NavMeshAgent
- Vérifiez que moveSpeed > 0

### Les animations ne fonctionnent pas
- Vérifiez que l'Animator Controller est bien assigné
- Ouvrez l'Animator et vérifiez les paramètres (Speed, Run, etc.)
- Assurez-vous que les animations sont bien liées dans l'Animator

### Le monstre ne spawne pas
- Vérifiez que le prefab est assigné au MonsterSpawner
- Vérifiez la console pour les messages d'erreur
- Assurez-vous que "Spawn On Start" est coché

### Le monstre traverse le sol
- Ajoutez un Collider au monstre
- Vérifiez que le terrain a un Collider
- Si vous utilisez NavMesh, assurez-vous que le terrain est "baked"

## NavMesh (Optionnel - pour une meilleure navigation)

Pour une navigation plus intelligente:

1. Sélectionnez votre terrain/sol
2. Window > AI > Navigation
3. Onglet "Bake" > Cliquez sur "Bake"
4. Le monstre utilisera automatiquement le NavMesh s'il est présent

## Prochaines étapes

Pour ajouter les attaques plus tard:
- Ajoutez une distance d'attaque dans MonsterAI
- Créez des méthodes pour les différentes attaques
- Utilisez les animations d'attaque disponibles (attack1, attack2, etc.)
- Ajoutez des dégâts au joueur

## Animations disponibles

Le dossier `MonsterMutant 7/Animations/` contient:
- **Idle:** idle1, idle2, idle3, idle4
- **Marche:** walk2, walk3, walk4, walkback
- **Course:** run1, run2, run3
- **Attaques:** attack1-5 (avec variations spike)
- **Dégâts:** gethit1-4
- **Mort:** death1-4
- **Autres:** jump, rage, strafeleft, straferight
