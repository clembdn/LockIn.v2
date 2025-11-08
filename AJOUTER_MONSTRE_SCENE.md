# 🚀 Ajouter le Monstre à la Scène - Guide Express

## Méthode la plus simple (1 clic!)

### Option 1: Configuration Complète Automatique ⭐ RECOMMANDÉ

1. Dans Unity, allez dans le menu: **LockIn > Complete Setup: Monster + Spawner**
2. Cliquez "Oui" pour confirmer
3. Si vous avez déjà un prefab configuré, tout sera fait automatiquement!
4. Sinon, suivez les instructions à l'écran
5. **Appuyez sur Play!** ▶️

---

## Méthode manuelle (si la méthode auto ne fonctionne pas)

### Étape 1: Créer et configurer le prefab

1. **Ajouter le monstre à la scène:**
   - Menu: **GameObject > LockIn > Add Monster Here**
   - Le monstre apparaît à côté du joueur

2. **Configurer le monstre:**
   - Le monstre devrait être sélectionné automatiquement
   - Dans l'Inspector, cliquez **Add Component**
   - Recherchez et ajoutez **QuickMonsterSetup**
   - Cliquez sur le gros bouton **⚙ SETUP MONSTER COMPONENTS**
   
3. **Assigner l'Animator Controller:**
   - Dans le composant **Animator**, glissez:
     `Assets/MonsterMutant 7/MonsterMutant7 Animator Controller.controller`
   
4. **Créer le prefab:**
   - Dans **QuickMonsterSetup**, cliquez **💾 Create Prefab**
   - Le prefab est créé dans `Assets/Prefabs/`
   
5. **Supprimer le monstre de la scène:**
   - Sélectionnez le monstre et appuyez sur **Delete**

### Étape 2: Ajouter le Spawner

1. **Créer le Spawner:**
   - Clic droit dans Hierarchy > **Create Empty**
   - Nommez-le "MonsterSpawner"
   
2. **Configurer le Spawner:**
   - Avec MonsterSpawner sélectionné, **Add Component > MonsterSpawner**
   - Dans **Monster Prefab**, glissez le prefab créé
   - Cochez **Spawn On Start**
   
3. **Sauvegarder:**
   - **Ctrl+S** (Cmd+S sur Mac) pour sauvegarder la scène

### Étape 3: Tester!

1. Appuyez sur **Play** ▶️
2. Le monstre devrait apparaître et courir vers vous!

---

## Méthodes alternatives (via menus)

### Méthode A: Via le menu LockIn

**LockIn > Add Monster to Scene**
- Ouvre une fenêtre avec options
- Cliquez **Auto-Find Monster Prefab**
- Puis **Add Monster System to Current Scene**

### Méthode B: Ajouter monstre configuré directement

**LockIn > Add Configured Monster to Scene**
- Cherche automatiquement un prefab configuré
- L'ajoute directement à la scène (sans spawner)
- Pratique pour tester rapidement

### Méthode C: Quick Add pour SampleScene

**LockIn > Quick Add Monster to SampleScene**
- Ouvre automatiquement SampleScene
- Cherche le prefab
- Configure tout
- Parfait pour un setup rapide!

---

## Vérification rapide

### Checklist avant de tester:

- [ ] La scène SampleScene est ouverte
- [ ] Un objet "MonsterSpawner" existe dans la hiérarchie
- [ ] Le MonsterSpawner a un prefab assigné dans "Monster Prefab"
- [ ] "Spawn On Start" est coché
- [ ] Le joueur existe dans la scène (tag "Player")

### Si le monstre n'apparaît pas:

1. Ouvrez la **Console** (Ctrl+Shift+C / Cmd+Shift+C)
2. Regardez les messages d'erreur
3. Vérifiez que le prefab a bien le composant **MonsterAI**
4. Vérifiez que le joueur a le tag **Player**

---

## Raccourcis clavier utiles

| Raccourci | Action |
|-----------|--------|
| **Ctrl+S** | Sauvegarder la scène |
| **Ctrl+P** | Play / Stop |
| **Ctrl+Shift+C** | Ouvrir la Console |
| **F2** | Renommer l'objet sélectionné |
| **Delete** | Supprimer l'objet sélectionné |

---

## Commandes du menu LockIn

Tous les outils sont disponibles dans le menu **LockIn**:

1. **Complete Setup: Monster + Spawner** ⭐
   - Fait TOUT automatiquement
   
2. **Add Monster to Scene**
   - Fenêtre interactive pour ajouter le monstre
   
3. **Add Configured Monster to Scene**
   - Ajoute directement un monstre configuré
   
4. **Quick Add Monster to SampleScene**
   - Setup rapide pour SampleScene spécifiquement

---

## Dépannage express

### "Prefab non trouvé"
➜ Assurez-vous que le dossier `Assets/MonsterMutant 7/` existe

### "Joueur non trouvé"
➜ Ajoutez le tag "Player" à votre joueur:
   - Sélectionnez le joueur
   - En haut de l'Inspector: Tag → Player

### "Le monstre ne bouge pas"
➜ Vérifiez que:
   - L'Animator Controller est assigné
   - Le script MonsterAI est présent
   - Move Speed > 0

### "Aucun prefab configuré trouvé"
➜ Créez d'abord le prefab:
   1. GameObject > LockIn > Add Monster Here
   2. Ajoutez QuickMonsterSetup
   3. Configurez et créez le prefab

---

## Test rapide (30 secondes)

```
1. Menu: LockIn > Complete Setup: Monster + Spawner
2. Cliquez "Oui"
3. Si erreur: suivez les instructions
4. Sinon: Appuyez sur Play ▶️
5. Profit! 🎉
```

---

## Résumé visuel

```
📁 Assets/
  📁 MonsterMutant 7/     ← Doit exister
    📄 Prefab/
  📁 Prefabs/              ← Sera créé
    📦 ConfiguredMonsterMutant7.prefab  ← Votre prefab configuré

🎮 Hierarchy (Scène):
  🎯 Player               ← Doit avoir tag "Player"
  🌟 MonsterSpawner       ← À créer
  🌍 Terrain/Ground
  💡 Lights
```

---

## Pour aller plus loin

Une fois que le monstre fonctionne:

- Changez **Spawn Offset** dans MonsterSpawner pour changer la position
- Modifiez **Move Speed** dans MonsterAI pour changer la vitesse
- Ajoutez plusieurs spawners pour plusieurs monstres
- Consultez **FICHIERS_CREES.md** pour ajouter des attaques

---

**Vous avez des problèmes?**
Consultez **GUIDE_RAPIDE_MONSTRE.md** section "🐛 Problèmes courants"

Bon jeu! 🎮✨
