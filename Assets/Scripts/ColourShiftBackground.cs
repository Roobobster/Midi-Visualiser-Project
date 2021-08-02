using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColourShiftBackground : MonoBehaviour
{
    [SerializeField]
    private float colourShiftSpeed;


    // Update is called once per frame
    void Update()
    {
        SetNewColour();
    }

    private void SetNewColour() 
    {
        Material backgroundMaterial = GetComponent<Image>().material;
        Color currentColour = backgroundMaterial.GetColor("_GlowColour");
        float H, S, V;
        Color.RGBToHSV(currentColour, out H, out S, out V);

        H += (1f / 360f) * Time.deltaTime * colourShiftSpeed;
        if (H > 1)
        {
            H = 0;
        }
        Debug.Log("H: " + H);
   
        Color nextColour = Color.HSVToRGB(H, S, V);
        GetComponent<Image>().material.SetColor("_GlowColour", nextColour);
    }
}
