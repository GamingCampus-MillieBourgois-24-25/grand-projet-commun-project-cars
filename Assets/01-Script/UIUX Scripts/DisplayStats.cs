using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayStats : MonoBehaviour
{
    [SerializeField] GameObject SpawnPoint;
    [SerializeField] GameObject[] Barres;
    [SerializeField] int Categorie;

    Transform firstChild;
    CarController.CarController Script;

    private void Start()
    {
        for (int i = 0; i < Barres.Length; i++)
        {
            Barres[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        firstChild = SpawnPoint.transform.GetChild(0);
        Script = firstChild.GetComponent<CarController.CarController>();
        if (firstChild != null && Script != null)
        {
            float Accel = Script.GetAccel();
            float Nitro = Script.GetNitro();
            float Drift = Script.GetDrift();
            float Speed = Script.GetMaxSpeed();
            switch (Categorie)
            {
                case 0:
                    for (int i = 0; i < Barres.Length; i++)
                    {
                        Accel -= 6;
                        if (Accel > 0)
                        {
                            Barres[i].SetActive(true);
                        }
                        else
                        {
                            Barres[i].SetActive(false);
                        }
                    }                        
                    break;
                case 1:
                    for (int i = 0; i < Barres.Length; i++)
                    {
                        Drift -= 0.5f;
                        if (Drift > 0)
                        {
                            Barres[i].SetActive(true);
                        }
                        else
                        {
                            Barres[i].SetActive(false);
                        }
                    }
                    break;
                case 2:
                    for (int i = 0; i < Barres.Length; i++)
                    {
                        Nitro -= 0.5f;
                        if (Nitro > 0)
                        {
                            Barres[i].SetActive(true);
                        }
                        else
                        {
                            Barres[i].SetActive(false);
                        }
                    }
                    break;
                case 3:
                    for (int i = 0; i < Barres.Length; i++)
                    {
                        Speed -= 25;
                        if (Speed > 0)
                        {
                            Barres[i].SetActive(true);
                        }
                        else
                        {
                            Barres[i].SetActive(false);
                        }
                    }
                    break;
            }
        }
    }
}
