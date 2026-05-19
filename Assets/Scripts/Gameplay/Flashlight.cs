using UnityEngine;

[RequireComponent(typeof(Light))]
public class Flashlight : MonoBehaviour
{
    public KeyCode toggleKey = KeyCode.F;
    Light flashlightLight;

    void Awake()
    {
        flashlightLight = GetComponent<Light>();
        if (flashlightLight == null)
        {
            // Add a default spot light if none exists
            flashlightLight = gameObject.AddComponent<Light>();
            flashlightLight.type = LightType.Spot;
            flashlightLight.spotAngle = 45f;
            flashlightLight.range = 10f;
            flashlightLight.intensity = 1.5f;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            flashlightLight.enabled = !flashlightLight.enabled;
        }
    }
}
