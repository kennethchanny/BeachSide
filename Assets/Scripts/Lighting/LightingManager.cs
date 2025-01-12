using UnityEngine;

[ExecuteAlways]
public class LightingManager : MonoBehaviour
{
    //Scene References
    [SerializeField] private Light DirectionalLight;
    [SerializeField] private LightingPreset Preset;
    //Variables
    [SerializeField, Range(0, 24)] private float TimeOfDay;
    [SerializeField] private float TimeScale = 1;
    [SerializeField] private float sunRotY = 50f;
    [SerializeField] private bool isDayNightCycleActive = true;


    private Material skyBoxMaterial;

    private void Start()
    {
        skyBoxMaterial = new Material(RenderSettings.skybox);
        RenderSettings.skybox = skyBoxMaterial;
    }
    private void Update()
    {
        if (Preset == null)
            return;

        //if game running and system is active
        if (Application.isPlaying && isDayNightCycleActive)
        {
           
            //(Replace with a reference to the game time)
            TimeOfDay += Time.deltaTime * TimeScale;
            TimeOfDay %= 24; //Modulus to ensure always between 0-24
            UpdateLighting(TimeOfDay / 24f);

            // Update the skybox material
            skyBoxMaterial.SetFloat("_TimeOfDay", TimeOfDay / 24f);
            

        }
        else
        {
            UpdateLighting(TimeOfDay / 24f);
        }
    }


    private void UpdateLighting(float timePercent)
    {
        //Set ambient and fog
        RenderSettings.ambientLight = Preset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = Preset.FogColor.Evaluate(timePercent);

        //If the directional light is set then rotate and set it's color, I actually rarely use the rotation because it casts tall shadows unless you clamp the value
        if (DirectionalLight != null)
        {
            DirectionalLight.color = Preset.DirectionalColor.Evaluate(timePercent);

            DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, sunRotY, 0));
        }

       
    }



    //Try to find a directional light to use if we haven't set one
    private void OnValidate()
    {
        if (DirectionalLight != null)
            return;

        //Search for lighting tab sun
        if (RenderSettings.sun != null)
        {
            DirectionalLight = RenderSettings.sun;
        }
        //Search scene for light that fits criteria (directional)
        else
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach (Light light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    DirectionalLight = light;
                    return;
                }
            }
        }
    }

}
