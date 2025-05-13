using UnityEngine;

public class HologramRot : MonoBehaviour
{
    [Header("Rotation")]
    [SerializeField] private bool useRandomSpeed = false;
    [SerializeField] private float rotationSpeed = 30f; // Degrés par seconde
    [SerializeField] private Vector3 rotationAxis = Vector3.up; // Axe de rotation (Y par défaut)
    [SerializeField] private bool invertRotation = false; // Inverser le sens
    [SerializeField] private bool useRandomDirection = false; // Sens de rotation aléatoire
    [SerializeField] private bool chaoticRotation = false; // Rotation chaotique avec changement d'axes

    [Header("Paramètres aléatoires")]
    [SerializeField] private float minSpeed = 15f;
    [SerializeField] private float maxSpeed = 60f;
    [SerializeField] private float speedChangeInterval = 2f; // Intervalle pour changer la vitesse
    [SerializeField] private float directionChangeInterval = 1.5f; // Intervalle pour changer la direction
    [SerializeField] private float axisChangeInterval = 3f; // Intervalle pour changer l'axe (mode chaotique)
    
    [Header("Paramètres de transition")]
    [SerializeField] private float speedTransitionRate = 1.5f; // Vitesse de transition pour la vitesse
    [SerializeField] private float directionTransitionRate = 2.0f; // Vitesse de transition pour la direction

    private float currentRotationSpeed;
    private float targetRotationSpeed;
    private float nextSpeedChangeTime;
    private float nextDirectionChangeTime;
    private float nextAxisChangeTime;
    
    private float currentDirectionFloat = 1f; // Version float de la direction pour le Lerp
    private float targetDirectionFloat = 1f; // Direction cible (-1f ou 1f)
    private Vector3 currentRotationAxis;
    private Vector3 targetRotationAxis;

    private void Start()
    {
        // Initialiser la vitesse de rotation
        targetRotationSpeed = useRandomSpeed ? Random.Range(minSpeed, maxSpeed) : rotationSpeed;
        currentRotationSpeed = targetRotationSpeed;

        // Initialiser la direction
        targetDirectionFloat = invertRotation ? -1f : 1f;
        currentDirectionFloat = targetDirectionFloat;
        if (useRandomDirection)
        {
            targetDirectionFloat = (Random.Range(0, 2) * 2 - 1); // -1 ou 1
            currentDirectionFloat = targetDirectionFloat; // Au début, on commence directement avec la cible
        }

        // Initialiser l'axe de rotation
        currentRotationAxis = rotationAxis.normalized;
        targetRotationAxis = currentRotationAxis;

        // Initialiser les temps de changement
        nextSpeedChangeTime = Time.time + speedChangeInterval;
        nextDirectionChangeTime = Time.time + directionChangeInterval;
        nextAxisChangeTime = Time.time + axisChangeInterval;
    }

    private void Update()
    {
        // Gérer les changements de vitesse
        if (useRandomSpeed && Time.time >= nextSpeedChangeTime)
        {
            SetRandomSpeed();
            nextSpeedChangeTime = Time.time + speedChangeInterval;
        }

        // Gérer les changements de direction indépendamment
        if (useRandomDirection && Time.time >= nextDirectionChangeTime)
        {
            SetRandomDirection();
            nextDirectionChangeTime = Time.time + directionChangeInterval;
        }

        // Gérer les changements d'axe pour la rotation chaotique
        if (chaoticRotation && Time.time >= nextAxisChangeTime)
        {
            SetRandomAxis();
            nextAxisChangeTime = Time.time + axisChangeInterval;
        }

        // Transition douce pour la vitesse
        currentRotationSpeed = Mathf.Lerp(currentRotationSpeed, targetRotationSpeed, 
            Time.deltaTime * speedTransitionRate);

        // Transition douce pour la direction
        currentDirectionFloat = Mathf.Lerp(currentDirectionFloat, targetDirectionFloat, 
            Time.deltaTime * directionTransitionRate);

        // Interpoler vers le nouvel axe de rotation pour une transition douce
        if (chaoticRotation)
        {
            currentRotationAxis = Vector3.Slerp(currentRotationAxis, targetRotationAxis, 
                Time.deltaTime * 2f);
        }

        // Appliquer la rotation avec la direction et la vitesse interpolées
        transform.Rotate(currentRotationAxis * (currentRotationSpeed * currentDirectionFloat * Time.deltaTime));
    }

    private void SetRandomSpeed()
    {
        targetRotationSpeed = Random.Range(minSpeed, maxSpeed);
    }

    private void SetRandomDirection()
    {
        targetDirectionFloat = (Random.Range(0, 2) * 2 - 1); // -1 ou 1
    }

    private void SetRandomAxis()
    {
        // Créer un axe de rotation complètement aléatoire
        targetRotationAxis = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        ).normalized;
    }
}