using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private Transform target;  // La voiture
    public float rotationSpeed = 5f; // Vitesse de rotation
    private float currentRotationX = 0f; // Pour suivre l'angle actuel de rotation autour de la voiture

    void Update()
    {
        // Détection du mouvement de la souris ou du doigt
        float horizontal = Input.GetAxis("Mouse X");  // Souris sur PC
        float vertical = Input.GetAxis("Mouse Y");    // Souris sur PC

        // Pour mobile (touches)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            horizontal = touch.deltaPosition.x * 0.1f;  // Sensibilité du glissement horizontal
            // vertical = touch.deltaPosition.y * 0.1f;  // ON SUPPRIME ce contrôle
        }

        // Rotation horizontale
        transform.RotateAround(target.position, Vector3.up, horizontal * rotationSpeed);

        // On garde l'angle actuel de la rotation verticale (pas de mouvement haut/bas)
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0);
    }
}
