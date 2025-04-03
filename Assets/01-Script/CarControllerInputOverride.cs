// Create this as a new C# script - CarControllerInputOverride.cs
using UnityEngine;

namespace CarController
{
    public class CarControllerInputOverride : MonoBehaviour
    {
        [HideInInspector] public float steeringInput = 0f;
        [HideInInspector] public float accelerationInput = 0f;
        [HideInInspector] public float brakeInput = 0f;
        [HideInInspector] public bool overrideInputs = false;
    }
}