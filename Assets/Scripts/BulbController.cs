using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulbController : ElectricalElementController
{
    public float power = 1.0f;

    private List<Renderer> filaments = new List<Renderer>();
    private Color emissionColor;
    private Light bulbLight;

    new void Start()
    {
        base.Start();
        filaments.Add(transform.Find("Scene").Find("filament0").GetComponent<Renderer>());
        filaments.Add(transform.Find("Scene").Find("filament1").GetComponent<Renderer>());
        filaments.Add(transform.Find("Scene").Find("filament2").GetComponent<Renderer>());
        emissionColor = filaments[0].material.GetColor("_EmissionColor");
        bulbLight = Instantiate(lightPrefab).GetComponent<Light>();
        bulbLight.transform.parent = this.transform;
        bulbLight.intensity = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPowered)
        {
            foreach (Renderer filament in filaments)
            {
                filament.material.SetColor("_EmissionColor", emissionColor * power);
                bulbLight.intensity = power / 4;
            }
        }
        else
        {
            foreach (Renderer filament in filaments)
            {
                filament.material.SetColor("_EmissionColor", emissionColor * -10.0f);
                bulbLight.intensity = 0;
            }
        }
    }
}
