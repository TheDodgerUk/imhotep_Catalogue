using UnityEngine;

public static class Layers
{
    public static void Initialise()
    {
        // this is to Initialise all the readonly data below
        int temp = DefaultLayer;
    }

    public static readonly string UnTaggedTag = "Untagged";
    public static readonly string MainCameraTag = "MainCamera";

    public static readonly int DefaultLayer = LayerMask.NameToLayer("Default");
    public static readonly int ControllerLayer = LayerMask.NameToLayer("Controller");

    public static readonly int ControllerUILayer = LayerMask.NameToLayer("ControllerUI");

    public static readonly int LoadingLayer = LayerMask.NameToLayer("Loading");
    public static readonly int VrInteractionLayer = LayerMask.NameToLayer("VrInteraction");
    public static readonly int TeleportLayer = LayerMask.NameToLayer("Teleport");
    public static readonly int HiddenLayer = LayerMask.NameToLayer("HiddenLayer");



    public static readonly int AllMask = ~0;

    public static readonly int CameraRuntimeMask = (1 << DefaultLayer) | (1 << ControllerLayer) | (1 << ControllerUILayer) | (1 << VrInteractionLayer) | (1 << TeleportLayer);
    public static readonly int CameraLoadingMask = (1 << LoadingLayer);
    public static readonly int NavigationMask = (1 << DefaultLayer) | (1 << VrInteractionLayer) | (1 << TeleportLayer);
    public static readonly int UIMask = (1 << ControllerUILayer);

    public static bool IsTeleport(this Collider collider) => collider.IsLayer(TeleportLayer);

    public static bool IsEnvironment(this Collider collider) => (collider.IsLayer(DefaultLayer) || collider.IsLayer(VrInteractionLayer));
    private static bool IsLayer(this Collider collider, int layer)
    {
        if (collider != null)
        {
            return (collider.gameObject.layer == layer);
        }
        return false;
    }
}
