using EditorTools.Environments;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BuildSystem
{
    public class BuildLightingJob
    {
        [MenuItem("Window/Custom/Build Lighting")]
        public static void GoThoughGenerateScenesLighingEct()
        {
            // load environment data
            Debug.Log("Initialising environments");
            var m_EnvironmentData = new EnvironmentDataManager();
            m_EnvironmentData.LoadFromFile();

            Debug.Log("Cleaning up old folders");
            EnvironmentSceneManager.CleanUnusedEnvironments(m_EnvironmentData.Environments);

            var repeatingLog = new RepeatingLogger();

            // add environment variants to queue
            foreach (var environment in m_EnvironmentData.Environments)
            {
                foreach (var variant in environment.Variants)
                {
                    //Skip this variant in this build?
                    if(false == variant.IncludeInBuild)
                    {
                        continue;
                    }

                    ExternalLogger.Indent++;
                    {
                        Debug.Log($"- {environment.Name} - {variant.Name}");

                        ExternalLogger.Indent += 2;
                        {
                            //Debug.Log("Applying Scene Settings to Original Scenes");
                            //EnvironmentSceneManager.ApplySceneSettings(variant.OriginalScenes);

                            //Debug.Log("Generating Scenes");
                            //EnvironmentSceneGenerator.GenerateScenes(environment, variant);

                            //Debug.Log("Applying Scene Settings to Generated Scenes");
                            //EnvironmentSceneManager.ApplySceneSettings(variant.GeneratedScenes, args.Quality);

                            //Debug.Log("Saving Scenes");
                            //EnvironmentSceneManager.SaveOpenScenes();

                            //repeatingLog.Start("Building Lighting");
                            //EnvironmentSceneManager.BakeLighting(EnvironmentVariantDataView.GetOnlyOriginalGeneratedScenes(variant));
                            //repeatingLog.Stop();

                            //Debug.Log("Saving Lighting");
                            //EnvironmentSceneManager.SaveOpenScenes();

                            //repeatingLog.Start("Building NavMesh");
                            //EnvironmentSceneManager.BakeNavMesh(EnvironmentVariantDataView.GetOnlyOriginalGeneratedScenes(variant));
                            //repeatingLog.Stop();


                            //Debug.Log("Saving NavMesh");
                            //EnvironmentSceneManager.SaveOpenScenes();

                            //Debug.LogError("This is a bit of a con way to do this, this is a message to test this HACK")
                            //EnvironmentSceneGenerator.GenerateDuplicateScenes(variant, variant.OptionalScenes, false);

                            repeatingLog.Start("Generating Scenes");

                            //TODO removed for now EnvironmentSceneManager.SaveGenericLightingData(variant);

                            EnvironmentSceneGenerator.GenerateScenes(environment, variant);
                            EnvironmentSceneGenerator.GenerateDuplicateScenes(variant, variant.OptionalScenes, false);
                            repeatingLog.Stop();

                            repeatingLog.Start("Baking Lighting");
                            EnvironmentSceneManager.BakeLighting(EnvironmentVariantDataView.GetOnlyOriginalAndAllowedGeneratedScenes(variant));
                            repeatingLog.Stop();

                            repeatingLog.Start("Baking NavMesh and Occlusion");
                            EnvironmentSceneManager.BakeNavMesh(EnvironmentVariantDataView.GetOnlyOriginalAndAllowedGeneratedScenes(variant));
                            EnvironmentSceneManager.BakeOcclusion(EnvironmentVariantDataView.GetOnlyOriginalAndAllowedGeneratedScenes(variant));
                            repeatingLog.Stop();
                        }
                        ExternalLogger.Indent -= 2;
                    }
                    ExternalLogger.Indent--;
                }
            }

            Debug.Log($"Light build complete. Saving Environment Data.");
            m_EnvironmentData.SaveToFile();
        }
    }
}
