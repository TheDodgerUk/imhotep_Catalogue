## Artwork Inputs
 

###The following file formats are currently on LFS:
 - .png
 - .fbx
 - .mp4
 - .psd
 - .unity
 - .mov

The following can be added to a folder:

- Models (FBX)
- Textures (PNG, TGA)
- Collision (FBX) `optional`
- Thumbnail (PNG)

----

#### Folder structure

```
<repo-root>/Catalogue/asset-name/asset-name.fbx
<repo-root>/Catalogue/asset-name/collision.fbx      # optional
<repo-root>/Catalogue/asset-name/texture-type.png
<repo-root>/Catalogue/asset-name/Thumbnail.png
```

----

#### Asset Name

If the model has LODs then these should be packaged up inside the .fbx file

See 'Example' folder for FBX example

----

#### LODs

(3 Levels of LOD)
Highest Quality = 0
Lowest Quality = 2

Triangle count should decrease dramatically between each LOD.

- LOD0 = Full Details
- LOD1 = Small details, bevels etc... removed
- LOD2 = Just outline shape needed. Lowest poly count possible without destroying shape

----

#### Texture Naming

`Name_TEXTURE_TYPE.EXTENSION`

e.g. Player_Albedo.png / Player_Normal.png

###### TEXTURE_TYPE

- Diffuse = Diffuse
- Albedo = Albedo
- Normal = Normal
- Ambient = Ambient
- Occlusion = Occlusion
- Emission = Emission
- Specular = MetallicSmoothness

Example:

```
<repo-root>/Catalogue/Turbosound_TLX212L/TLX212L.fbx
<repo-root>/Catalogue/Turbosound_TLX212L/TLX212L_Albedo.png
<repo-root>/Catalogue/Turbosound_TLX212L/TLX212L_Normal.png
<repo-root>/Catalogue/Turbosound_TLX212L/TLX212L_MetallicSmoothness.png
```

----
###To Build
 - Find the Vareient you want 
 - View Original
 - Generate Scene
###Bundle Builder
 - Build Bundle
 - Copy to