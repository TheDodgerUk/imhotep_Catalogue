using EditorTools.Views;
using FilePathHelper;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorTools.CatalogueViewer
{
    public class ProfileDataView
    {
        public static IDataView Create(object profileData)
        {
            var profileType = profileData.GetType();
            var dataType = profileType.GetGenericBaseType().GetGenericArguments()[0];

            string fieldName = nameof(ProfileData<object>.Overridden);
            var field = profileType.GetField(fieldName);
            var data = field.GetValue(profileData);

            var viewType = typeof(DataView<>).GetConcreteChildren(dataType);
            var dataView = System.Activator.CreateInstance(viewType.First(), data);

            var viewerType = typeof(ProfileDataViewer<,>).MakeGenericType(profileType, dataType);
            return System.Activator.CreateInstance(viewerType, profileData, dataView) as IDataView;
        }
    }

    public class ProfileDataViewer<TProfileData, TData> : ProfileDataView<TProfileData, TData> where TProfileData : ProfileData<TData> where TData : class, new()
    {
        private DataView<TData> m_DataView;

        public ProfileDataViewer(TProfileData profileData, DataView<TData> dataView) : base(profileData)
        {
            m_DataView = dataView;
        }

        protected override void OnDataGUI(TData gizmoData)
        {
            m_DataView.Update(gizmoData);
            m_DataView.OnGUI();
        }
    }

    public abstract class ProfileDataView<TProfileData, TData> : IDataView where TProfileData : ProfileData<TData> where TData : class, new()
    {
        private const string NoneProfile = "(None)";

        private TProfileData m_ProfileData;

        public ProfileDataView(TProfileData profileData)
        {
            m_ProfileData = profileData;
        }

        public void OnGUI()
        {
            string profileName = m_ProfileData.ProfileName;
            if (string.IsNullOrEmpty(profileName) == true)
            {
                profileName = NoneProfile;
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                // draw profile name
                profileName = EditorHelper.Popup("Profile", m_ProfileData.ProfileName, GetAllProfileNames());

                var createRect = Utils.Gui.GetButtonRect("Create", 72);
                createRect.y -= 2;

                if (GUI.Button(createRect, "Create") == true)
                {
                    // get proposed name of new profile
                    InputTextWindow.Create("Profile Name", "default", createRect, input =>
                    {
                        input = input.ToLower();

                        // create profile
                        var profile = CreateProfile(input);
                        if (profile != null)
                        {
                            profileName = input;
                            m_ProfileData.ProfileName = input;
                            m_ProfileData.Overridden = null;
                        }
                    });
                }
            }

            // reset profile name
            if (profileName == NoneProfile)
            {
                profileName = null;
            }

            // if profile name has changed
            if (profileName != m_ProfileData.ProfileName)
            {
                if (string.IsNullOrEmpty(profileName) == true)
                {
                    // create custom profile based on current profile name
                    var profile = LoadProfile(m_ProfileData.ProfileName);
                    if (profile == null)
                    {
                        profile = new TData();
                    }

                    // use override data rather than profile name
                    m_ProfileData.ProfileName = null;
                    m_ProfileData.Overridden = profile;
                }
                else
                {
                    // use profile name rather than overridden data
                    m_ProfileData.ProfileName = profileName;
                    m_ProfileData.Overridden = null;
                }
            }

            EditorGUILayout.Space();

            // if there is a profile name, load the profile for viewing
            if (string.IsNullOrEmpty(m_ProfileData.ProfileName) == false)
            {
                var profile = LoadProfile(m_ProfileData.ProfileName);
                if (profile != null)
                {
                    using (new GuiEnabledScope(false))
                    {
                        OnDataGUI(profile);
                    }
                }
            }

            // if there is override data, draw the overridden data
            if (m_ProfileData.Overridden != null)
            {
                OnDataGUI(m_ProfileData.Overridden);
            }
        }

        protected abstract void OnDataGUI(TData data);

        private TData CreateProfile(string profileName)
        {
            if (string.IsNullOrEmpty(profileName) == true)
            {
                Debug.Log("Profile Name cannot be empty\n");
                return null;
            }

            var path = FilePath.FromAbsolute(GetPath(profileName));
            if (Utils.IO.FileExists($"{path.Absolute}.json") == true)
            {
                Debug.LogError($"File already exists. Cannot create profile.\n\"{path.Absolute}\"");
                return null;
            }

            var profile = m_ProfileData.Overridden ?? new TData();
            Json.JsonNet.WriteToFile(profile, path.Absolute, true);

            AssetDatabase.Refresh();
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(path.Relative);

            return profile;
        }

        private static List<string> GetAllProfileNames()
        {
            string folder = Catalogue.GetProfileFolder<TData>();
            if (Utils.IO.FolderExists(folder) == true)
            {
                var files = Utils.IO.GetFiles(folder, "*.json");
                var fileNames = files.Extract(Utils.IO.GetFileName);
                fileNames.Insert(0, NoneProfile);
                return fileNames;
            }
            else
            {
                return new List<string> { NoneProfile };
            }
        }

        private static string GetPath(string name) => Catalogue.GetProfilePath<TData>(name);

        private static TData LoadProfile(string profileName)
        {
            var filePath = GetPath(profileName);
            if (Utils.IO.FileExists($"{filePath}.json") == true)
            {
                return Catalogue.LoadProfile<TData>(profileName);
            }
            return null;
        }
    }
}