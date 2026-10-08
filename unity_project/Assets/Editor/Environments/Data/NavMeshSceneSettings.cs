using UnityEditor.AI;

public class NavMeshSceneSettings : EnvironmentSceneSettings
{
    public float AgentRadius { set { m_AgentRadius.Value = value; } }
    public float AgentHeight { set { m_AgentHeight.Value = value; } }
    public float MaxSlope { set { m_AgentSlope.Value = value; } }
    public float StepHeight { set { m_AgentClimb.Value = value; } }

    public float DropHeight { set { m_DropHeight.Value = value; } }
    public float JumpDistance { set { m_JumpDistance.Value = value; } }

    public bool ManualVoxelSize { set { m_ManualVoxelSize.Value = value ? 1 : 0; } }
    public float VoxelSize { set { m_VoxelSize.Value = value; } }
    public float MinRegionArea { set { m_MinRegionArea.Value = value; } }
    public bool ComputeHeightMesh { set { m_HeightMesh.Value = value ? 1 : 0; } }

    private SerializableProperty<float> m_AgentRadius;
    private SerializableProperty<float> m_AgentHeight;
    private SerializableProperty<float> m_AgentSlope;
    private SerializableProperty<float> m_AgentClimb;

    private SerializableProperty<float> m_DropHeight;
    private SerializableProperty<float> m_JumpDistance;

    private SerializableProperty<int> m_ManualVoxelSize;
    private SerializableProperty<float> m_VoxelSize;
    private SerializableProperty<float> m_MinRegionArea;
    private SerializableProperty<int> m_HeightMesh;

    public NavMeshSceneSettings() : base(NavMeshBuilder.navMeshSettingsObject)
    {
        var settings = m_SerializedObject.FindProperty("m_BuildSettings");

        m_AgentRadius = GetProperty<float>(settings, "agentRadius");
        m_AgentHeight = GetProperty<float>(settings, "agentHeight");
        m_AgentSlope = GetProperty<float>(settings, "agentSlope");
        m_AgentClimb = GetProperty<float>(settings, "agentClimb");

        m_DropHeight = GetProperty<float>(settings, "ledgeDropHeight");
        m_JumpDistance = GetProperty<float>(settings, "maxJumpAcrossDistance");

        m_ManualVoxelSize = GetProperty<int>(settings, "manualCellSize");
        m_VoxelSize = GetProperty<float>(settings, "cellSize");
        m_MinRegionArea = GetProperty<float>(settings, "minRegionArea");
        m_HeightMesh = GetProperty<int>(settings, "accuratePlacement");

        if (m_ManualVoxelSize.Value == 0)
        {
            VoxelSize = m_AgentRadius.Value / 3f;
        }
    }

    public static bool Apply()
    {
        using (var settings = new NavMeshSceneSettings
        {
            AgentRadius = 0.12f,
            AgentHeight = 2f,
            MaxSlope = 50f,
            StepHeight = 0.085f,

            DropHeight = 0f,
            JumpDistance = 0f,

            ManualVoxelSize = false,
            MinRegionArea = 0,
            ComputeHeightMesh = false,
        })
        {
            return settings.ApplySerializedObjects();
        }
    }
}
