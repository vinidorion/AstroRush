# AstroRush
Jeu de course similaire à [Wipeout](https://en.wikipedia.org/wiki/Wipeout_(video_game_series)), développé avec Unity.

### Création de piste de course
- Il est recommandé d'utiliser Blender pour faciliter le placement des waypoints.
- Une fois le modèle fait dans Blender, on peut simplement l'importer en format FBX.
- Doit avoir un [mesh collider](https://docs.unity3d.com/ScriptReference/MeshCollider.html) component, [NON convex](https://docs.unity3d.com/ScriptReference/MeshCollider-convex.html) et sur la layer `track`.

### Items
- De nouveaux items (préfab) peuvent être ajoutés dans le dossier `AstroRush\Assets\_MyAssets\Prefabs\Resources\PUs`.
- La baseclass des scripts pour les items est [PU](https://github.com/vinidorion/AstroRush/blob/main/Assets/_MyAssets/Scripts/PowerUp/testpoly/PU.cs).
- La nouvelle classe doit se trouver dans le namespace `poly`, pour éviter des conflits avec les anciens items qui n'utilisaient pas le polymorphisme.

### Musiques
Les musiques peuvent être ajoutées directement dans le dossier `AstroRush\Assets\_MyAssets\Sounds\Resources\Music`. Ils joueront aléatoirement dans le jeu.

### Système de sauvegarde
Les scores sont tous sauvegardés à cet emplacement de fichier.
```bash
  Application.persistentDataPath + "/save.json"
```
- Ce qui devrait être `C:\Users\<user>\AppData\LocalLow\<company name>\AstroRush\save.json` sur windows.
- Voir [Application.persistentDataPath](https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html) pour plus d'informations.
À gauche : l'affichage des scores dans le jeu.\
À droite : le score enregistré en JSON.
![App Screenshot](https://i.gyazo.com/ec174adcbe9597581790f80e6e81e48a.png)