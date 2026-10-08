using FilePathHelper;
using UnityEditor;
using UnityEngine;

namespace EditorTools.CatalogueViewer
{
    public static class PrefabManager
    {
        private static readonly FilePath HologramMaterialPath = FilePath.FromRelative("Assets/Artwork/Materials/Outline/Outline.mat");
        private static readonly FilePath CatalogueRoot = FilePath.FromRelative("Assets/Artwork/Models/Catalogue");
        public static readonly FilePath PrefabRoot = CatalogueRoot.Append("GeneratedPrefabs");
        private const string BundleExtension = Bundles.AssetBundleBuilder.BundleExtension;

        public static void LabelCatalogueFolders()
        {
            bool changed = false;
            var folders = Utils.IO.GetAllFolders(CatalogueRoot.Absolute, true);
            using (var progressBar = new ProgressBar("Labelling Asset Bundles", folders.Count))
            {
                for (int i = 0; i < folders.Count; ++i)
                {
                    progressBar.UpdateIncrement(folders[i]);
                    changed = LabelFolder(folders[i]);
                }

                if (changed == true)
                {
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
        }

        public static bool LabelFolder(string folderPath)
        {
            string bundleName = string.Empty;
            var folder = FilePath.FromAbsolute(folderPath);
            if (Utils.IO.IsFolderEmpty(folderPath) == false)
            {
                var rootLess = folder.ChangeRoot(CatalogueRoot, "catalogue/prefabs/");
                bundleName = $"{rootLess.Absolute.Replace(" ", "_")}.{BundleExtension}".ToLower();
            }

            var importer = AssetImporter.GetAtPath(folder.Relative);

            if (importer == null)
            {
                Debug.LogError($"folderPath:{folderPath},   bundleName:{bundleName},    folder.Relative:{folder.Relative}");
            }
            else
            {
                if (importer.assetBundleName != bundleName)
                {
                    importer.assetBundleName = bundleName;
                    return true;
                }
            }
            return false;
        }

        public static GameObject CreatePrefab(CatalogueEntry entry, GameObject existingPrefab)
        {
            var go = new GameObject();

            var ChildNames = new[] { GlobalConsts.CollisionObj, GlobalConsts.RootModel }.ToList();
            ChildNames.ForEach(n => go.transform.CreateChild(n));

            if (existingPrefab != null)
            {
                existingPrefab = PrefabUtility.ReplacePrefab(go, existingPrefab);
            }
            else
            {
                var filePath = CatalogueRoot.Append($"{entry.Guid}.prefab");
                existingPrefab = PrefabUtility.CreatePrefab(filePath.Relative, go);
            }
            go.DestroyObjectImmediate();

            var prefabPath = existingPrefab.GetAssetPath();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath.Relative);
            EditorHelper.SelectAndPingObject(prefab);
            return prefab;
        }

        public static GameObject LoadPrefab(RenderItemData renderItem)
        {
            if (string.IsNullOrEmpty(renderItem.Guid) == false)
            {
                string path = AssetDatabase.GUIDToAssetPath(renderItem.Guid);
                if (string.IsNullOrEmpty(path) == false)
                {
                    return AssetDatabase.LoadAssetAtPath<GameObject>(path);
                }
            }
            return null;
        }

        public static GameObject GetSelectedModel() => Selection.activeGameObject;

        public static bool HasModelSelected() => GetSelectedModel().GetImporter<ModelImporter>() != null;

        public static RenderItemData CreateNewPrefabFromModel(CatalogueEntry entry, GameObject model, int attempts = 0)
        {
            // create prefab
            string name = model.name.Strip("_LOD0");
            var importer = model.GetImporter<ModelImporter>();
            FilePath folderPath = null;
            if (importer != null)
            {
                folderPath = importer.GetAssetPath().GetFolder();
            }
            else
            {
                folderPath = model.GetAssetPath().GetFolder();
            }
            var prefabPath = folderPath.Append($"{name}{GlobalConsts._PREFAB_PREFAB_EXTENTION}");
            GameObject prefab = null;
            if (model.name.EndsWith(GlobalConsts._PREFAB_EXTENTION) == false)
            { 
                var go = new GameObject() { name = (name + GlobalConsts._PREFAB_EXTENTION) };
                prefab = PrefabUtility.CreatePrefab(prefabPath.Relative, go);
                prefab.ClearLocals();
                prefab = CreatePrefab(entry, prefab);
                prefab.ClearLocals();
                go.DestroyObjectImmediate();
                model.ClearLocals();
            }
            else
            {
                prefab = model;
            }
            var renderItem = CreateRenderItem(prefab);

            if (string.IsNullOrEmpty(renderItem.Bundle) == true)
            {
                if (attempts < 1)
                {
                    EditorUtility.DisplayDialog("Error", "Imported model was not placed in a labelled folder.\nPlace your " +
                                "model under the Catalogue folder, press button  \"Label Catalogue Folders\" from Catalogue view, and try again.", "Ok");


                    prefab.DestroyObjectImmediate(true);
                    LabelCatalogueFolders();

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    return CreateNewPrefabFromModel(entry, model, attempts + 1);
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", "Imported model was not placed in a labelled folder.\nPlace your " +
                        "model under the Catalogue folder, press button  \"Label Catalogue Folders\" from Catalogue view, and try again.", "Ok");

                    ApplyModelToPrefab(entry, renderItem, model);
                    return renderItem;
                }
                return null;
            }
            else
            {
                ApplyModelToPrefab(entry, renderItem, model);
                return renderItem;
            }
        }

        public static RenderItemData CreateRenderItem(GameObject prefab)
        {
            return new RenderItemData
            {
                Guid = prefab.GetAssetGuid(),
                Bundle = prefab.GetAssetBundleName(),
                Prefab = prefab.name,
            };
        }

        public static void ApplySelectedModelToPrefab(CatalogueEntry entry, RenderItemData renderItem)
        {
            var model = Selection.activeGameObject;
            if (model != null)
            {
                ApplyModelToPrefab(entry, renderItem, model);
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "You have not selected a model", "Whoops!");
            }
        }

        public static void ApplyModelToPrefab(CatalogueEntry entry, RenderItemData renderItem, GameObject model)
        {
            var targetPath = model.GetAssetPath().Strip("_LOD0").RemoveExt();

            // check if prefab already contains model
            var prefab = LoadPrefab(renderItem);
            if (prefab.transform.Find($"{GlobalConsts.RootModel}/{model.name}") != null)
            {
                string message = $"Prefab already contains a model called {model.name}.\n\nWould you like to like to apply it anyway?";
                if (EditorUtility.DisplayDialog(model.name, message, "Yes", "No") == false)
                {
                    return;
                }
            }

            // instantiate prefab
            var clone = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            var transform = clone.transform;

            // instantiate model & apply to prefab
            var lodClone = Utils.Unity.Clone(model);
            lodClone.transform.SetParent(transform.Find(GlobalConsts.RootModel));
            ApplyMaterialToModel(targetPath, lodClone);

            // create collision & apply to prefab
            var placementCollider = GenerateCollider(model, targetPath, "_Collision.FBX", "_Collider.FBX");
            var placementColliderParent = transform.Find(GlobalConsts.CollisionObj);
            placementCollider.transform.SetParent(placementColliderParent);
            placementCollider.SetLayerRecursively(Layers.VrInteractionLayer);


            // not sure if its regwnerating them or not
            //////////var basicPysicsCollider = GenerateCollider(model, targetPath, "_Collision.FBX", "_Collider.FBX");
            //////////var rootModel = transform.Find(GlobalConsts.RootModel);

            //////////basicPysicsCollider.transform.SetParent(rootModel);
            //////////basicPysicsCollider.transform.SetSiblingIndex(0);
            //////////basicPysicsCollider.SetLayerRecursively(Layers.VrInteractionLayer);
            //////////basicPysicsCollider.name = GlobalConsts.BasicPhysicsCollider;

            // update prefab
            PrefabUtility.ReplacePrefab(clone, prefab);
            clone.DestroyObjectImmediate();

            // refresh database
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void ApplyMaterialToModel(FilePath targetPath, GameObject lodClone)
        {
            // apply material to model
            var materialPath = targetPath.AppendExt("_Mat.mat");
            var material = AssetDatabase.LoadAssetAtPath<Material>(materialPath.Relative);
            if (material == null)
            {
                string path = targetPath.Absolute;
                int index = path.LastIndexOf("-");
                if (index >= 0)
                {
                    materialPath = FilePath.FromAbsolute(path.Substring(0, index)).AppendExt("_Mat.mat");
                    material = AssetDatabase.LoadAssetAtPath<Material>(materialPath.Relative);
                }
            }
            if (material != null)
            {
                if (material.mainTexture == null)
                {
                    PbrMaterialMenu.PopulateMaterial(material);
                }
                var lodRenderers = lodClone.GetComponentsInChildren<MeshRenderer>().ToList();
                lodRenderers.ForEach(r => r.sharedMaterial = material);
            }
        }

        private static GameObject GenerateCollider(GameObject lodAsset, FilePath targetPath, params string[] extensions)
        {
            for (int i = 0; i < extensions.Length; ++i)
            {
                var path = targetPath.AppendExt(extensions[i]);
                var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path.Relative);
                if (asset != null)
                {
                    return GenerateColliderFromMesh(asset);
                }
            }

            // create bounds to fit all renderers in 
            var bounds = new Bounds();
            var meshFilters = lodAsset.GetComponentsInChildren<MeshFilter>().ToList();

            foreach(var item  in meshFilters)
            {
                if (item != null && item.sharedMesh != null && item.sharedMesh.bounds != null)
                {
                    bounds.Encapsulate(item.sharedMesh.bounds);
                }
            }


            var colliderGo = new GameObject(GlobalConsts.CollisionObj);
            var box = colliderGo.AddComponent<BoxCollider>();
            box.transform.position = bounds.center;
            box.size = bounds.size;
            box.isTrigger = true;
            return colliderGo;
        }

        private static GameObject GenerateColliderFromMesh(GameObject asset)
        {
            // create parent object for this fbx
            var colliderGo = new GameObject(asset.name);

            // get meshes in asset
            var meshFilters = asset.GetComponentsInChildren<MeshFilter>();

            // if there is one add a mesh collider to the root object
            if (meshFilters.Length == 1)
            {
                var meshFilter = asset.GetComponent<MeshFilter>();
                CreateCollisionMesh(meshFilter, colliderGo);
            }
            else
            {
                // add a child object for each mesh in the asset
                foreach (var meshFilter in meshFilters)
                {
                    var child = colliderGo.transform.CreateChild(meshFilter.name);
                    CreateCollisionMesh(meshFilter, child.gameObject);
                }
            }
            return colliderGo;
        }

        private static void CreateCollisionMesh(MeshFilter meshFilter, GameObject parentObject)
        {
            var collider = parentObject.AddComponent<MeshCollider>();
            collider.sharedMesh = meshFilter.sharedMesh;

            parentObject.transform.position = meshFilter.transform.position;
            parentObject.transform.rotation = meshFilter.transform.rotation;
            collider.convex = true;
            collider.isTrigger = true;
        }

        private static FilePath GetVariationPath(string guid, string variation)
        {
            return GetFolderPath(guid).Append($"{guid}_{variation}.prefab");
        }

        private static FilePath GetFolderPath(string guid)
        {
            return PrefabRoot.Append(guid);
        }
    }
}
