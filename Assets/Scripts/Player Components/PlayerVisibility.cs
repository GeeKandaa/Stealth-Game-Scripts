using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerVisibility : MonoBehaviour
{
    public float Visibility { get; private set; }

    // Start is called before the first frame update
    void FixedUpdate()
    {
        //LightProbes.Tetrahedralize();
        LightProbes.GetInterpolatedProbe(transform.position, null, out SphericalHarmonicsL2 sh);

        Vector3[] LightDirections = new[]
        {
            new Vector3(1.0f, 0.0f, 0.0f),
            new Vector3(0.0f, 1.0f, 0.0f),
            new Vector3(0.0f, 0.0f, 1.0f),
            new Vector3(-1.0f, 0.0f, 0.0f),
            new Vector3(0.0f, -1.0f, 0.0f),
            new Vector3(0.0f, 0.0f, -1.0f)
        };
        Color[] LightResults = new Color[6];
        sh.Evaluate(LightDirections, LightResults);
        // Perceived light is approximated by r*0.375+g*0.5+b*0.125
        // min=0 ~ There is no maximum, so we define our own range for min max light.
        double PerceivedLightLevel = 0;
        for (int i = 0; i < 6; i++)
        {
            Color LR = LightResults[i];
            PerceivedLightLevel += LR.r * 0.375 + LR.g * 0.5 + LR.b * 0.125;
        }
        // We use min = 0, max=2 (literal values can still fall outside this range so we normalise then clamp them to 0~1).
        float ClampedLightLevel = Mathf.Clamp01((float)PerceivedLightLevel / 2);

        Visibility = ClampedLightLevel;
    }

}
