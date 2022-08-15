using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    public static bool useFog;

    [Header("Environment Influences")]
    [SerializeField] private Camera cam = default;
    [SerializeField] private Light sun = default;

    // Start is called before the first frame update
    void Start()
    {
        if (useFog) FogOn();
    }

    public void FogOn()
    {
        useFog = true;

        RenderSettings.fog = true;
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;

        cam.backgroundColor = RenderSettings.fogColor;
        cam.clearFlags = CameraClearFlags.SolidColor;

        sun.shadowStrength = 0.0f;
    }

    public void FogOff()
    {
        useFog = false;

        RenderSettings.fog = false;
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;

        cam.clearFlags = CameraClearFlags.Skybox;

        sun.shadowStrength = 1.0f;
    }
}
