using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsActive : MonoBehaviour
{
    [SerializeField] private bool PageIsActive;

    public void SetBool(bool value) { PageIsActive = value; }
    public bool GetBool() { return PageIsActive; }
}
