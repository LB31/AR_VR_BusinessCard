using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vuforia;

public class TardisHandler : MonoBehaviour
{

    public GameObject TardisInside;
    public GameObject ImageTarget;

    public GameObject BackgroundPlane;

    private AudioSource audioData;
    private bool AudioPlaying;

    private float SpawnTime = 10;
    private float CurrentTime = 0;
    private float CurrentAlpha = 0;
    private float SpawnStep;
    private float CurrentStep;

    private bool SpawnDirection = false;
    private float DirectionPeriod = 1f;
    private float NextPeriod;
    private float t;
    private bool InGettingBigger;
    

    private void Start() {
        audioData = GetComponent<AudioSource>();
        

        SpawnStep = 255 / (SpawnTime / 2);

        SetMaterialTransparent();

        

    }


    private void Update() {
        if (!AudioPlaying) {
            audioData.Play();
            AudioPlaying = true;
        }
        
        if(CurrentTime < SpawnTime) { // Ensures that the TARDIS spawns for 'SpawnTime' seconds
            if (CurrentTime > NextPeriod) { // Ensures that the direction changes every 'DirectionPeriod' seconds
                SpawnDirection = !SpawnDirection;
                NextPeriod = CurrentTime + DirectionPeriod;
                if (InGettingBigger) {
                    CurrentStep += SpawnStep;
                    InGettingBigger = false;
                } else {
                    InGettingBigger = true;
                }
                t = 0;
            }

            if (!SpawnDirection) {
                if (SpawnStep + CurrentStep <= 255) {
                    CurrentAlpha = Mathf.Lerp(CurrentStep - SpawnStep, SpawnStep + CurrentStep, t);
                    //print("Ich interpoliere zwischen " + (CurrentStep - SpawnStep) + " und " + (SpawnStep + CurrentStep));
                }
            } else {
                if (SpawnStep + CurrentStep <= 255) {
                    CurrentAlpha = Mathf.Lerp(SpawnStep + CurrentStep, CurrentStep, t);
                    //print("Ich interpoliere zwischen " + (CurrentStep + SpawnStep) + " und " + (CurrentStep));
                }
            }

            if (SpawnStep + CurrentStep <= 255) {
                SetMaterialTransparent();
            } else {
                SetMaterialOpaque();
            }

            t += Time.deltaTime;
            CurrentTime += Time.deltaTime;

        } 
    }


    public void EnterTardis() {
        VuforiaBehaviour.Instance.enabled = false;
        SceneManager.LoadScene("Tardis_Inside");
    }

    private void OnTriggerEnter(Collider other) {
        if(other.GetComponent<Camera>() != null) {
            EnterTardis();
        }
    }

    // Srouce: https://www.patreon.com/posts/c-script-to-hide-7678022
    private void SetMaterialTransparent() {

        foreach (Transform part in transform) {

            foreach (Material material in part.GetComponent<Renderer>().materials) {
                material.SetFloat("_Mode", 2);

                Color32 col = material.GetColor("_Color");
                col.a = (byte)CurrentAlpha;
                material.SetColor("_Color", col);

                
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
            }
        }
    }


    private void SetMaterialOpaque() {
        foreach (Transform part in transform) {
            foreach (Material material in part.GetComponent<Renderer>().materials) {

                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = -1;

            }
        }

    }




}