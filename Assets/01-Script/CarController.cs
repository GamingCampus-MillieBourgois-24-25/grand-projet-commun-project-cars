using Cinemachine;
using UnityEngine;

namespace CarController
{
    public class CarController : MonoBehaviour
    {
        #region Variables
        public enum groundCheck { rayCast, sphereCaste };
        public  enum MovementMode { Velocity, AngularVelocity };
        [Header("General Settings")]
        [SerializeField] private  MovementMode movementMode;
        [SerializeField] private  groundCheck GroundCheck;
        [SerializeField] private  LayerMask drivableSurface;
        [SerializeField] private  CinemachineVirtualCamera virtualCamera;

        [Header("Vehicle Settings")]
        [Tooltip("turn more while drifting (while holding space) only if kart Like is true")]
        [SerializeField] private  float driftMultiplier = 1.5f;
        [SerializeField] private  float MaxSpeed, accelaration, turn, gravity = 7f, downforce = 5f;
        [Tooltip("if true : can turn vehicle in air")]
        [SerializeField] private  bool AirControl = false;
        [Tooltip("if true : vehicle will drift instead of brake while holding space")]
        [SerializeField] private  bool kartLike = false;

        [SerializeField] private  Rigidbody rb, carBody;

        [HideInInspector]
        public RaycastHit hit;
        [Header("Friction Settings")]
        [SerializeField] private  AnimationCurve frictionCurve;
        [SerializeField] private  AnimationCurve turnCurve;
        [SerializeField] private  PhysicMaterial frictionMaterial;
        
        [Header("Visuals")]
        [SerializeField] private Transform BodyMesh;
        [SerializeField] private  Transform[] FrontWheels = new Transform[2];
        [SerializeField] private  Transform[] RearWheels = new Transform[2];
        [HideInInspector]
        public Vector3 carVelocity;
        
        [Range(0, 10)]
        [SerializeField] private  float BodyTilt;
        [Header("Audio settings")]
        [SerializeField] private  AudioSource engineSound;
        [Range(0, 1)]
        [SerializeField] private  float minPitch;
        [Range(1, 3)]
        [SerializeField] private  float MaxPitch;
        [SerializeField] private  AudioSource SkidSound;
        
        [Header("Boost Settings")]
        [SerializeField] private float boostMaxSpeedMultiplier = 1.5f;     // Multiplicateur de vitesse max pendant le boost
        [SerializeField] private float boostFalloffSpeed = 4.0f;           // Vitesse de retour à la normale (plus grand = plus rapide)
        [SerializeField] private float maxBoostAmount = 100f;              // Quantité maximale de boost
        [SerializeField] private float boostConsumptionRate = 25f;         // Consommation du boost par seconde
        [SerializeField] private float boostRechargeRate = 10f;            // Régénération du boost par seconde
        [SerializeField] private bool boostAutoRecharge = true;            // Si le boost se recharge automatiquement
        [SerializeField] private ParticleSystem boostParticles;            // Effets visuels du boost
        [SerializeField] private AudioClip boostSound;                     // Son du boost
        [SerializeField] private AudioClip boostEmptySound;                // Son quand le boost est vide

        private float currentBoostAmount;                       // Quantité actuelle de boost
        private float originalMaxSpeed;                         // Vitesse max originale
        private float currentBoostMultiplier = 1.0f;            // Multiplicateur de vitesse actuel
        private bool isBoostActive = false;                     // Si le boost est actuellement activé
        private AudioSource boostAudioSource;                   // Source audio pour le boost
        
        [HideInInspector]
        public float skidWidth;
        
        private IA_Steering steeringAction;
        private float radius, steeringInput, accelerationInput, brakeInput, boostInput;
        private Vector3 origin;
        
        #endregion
        
        #region Unity Methods

        private void Awake()
        {
            steeringAction = new IA_Steering();
        }
        
        private void OnEnable()
        {
            steeringAction.Enable();
        }
        
        private void OnDisable()
        {
            steeringAction.Disable();
        }
        
        private void Start()
        {
            radius = rb.GetComponent<SphereCollider>().radius;
            if (movementMode == MovementMode.AngularVelocity)
            {
                Physics.defaultMaxAngularSpeed = 100;
            }
            
            // Initialize the boost amount
            originalMaxSpeed = MaxSpeed;
            currentBoostAmount = maxBoostAmount;
        }

        private void Update()
        {
            Visuals();
            AudioManager();
            ProcessInputs();
        }
        
        void FixedUpdate()
        {
            carVelocity = carBody.transform.InverseTransformDirection(carBody.velocity);

            if (Mathf.Abs(carVelocity.x) > 0)
            {
                //changes friction according to sideways speed of car
                frictionMaterial.dynamicFriction = frictionCurve.Evaluate(Mathf.Abs(carVelocity.x / 100));
            }


            if (grounded())
            {
                
                //turnlogic
                float sign = Mathf.Sign(carVelocity.z);
                float TurnMultiplyer = turnCurve.Evaluate(carVelocity.magnitude / MaxSpeed);
                if (kartLike && brakeInput > 0.1f) { TurnMultiplyer *= driftMultiplier; } //turn more if drifting


                if (accelerationInput > 0.1f || carVelocity.z > 1)
                {
                    carBody.AddTorque(Vector3.up * steeringInput * sign * turn * 100 * TurnMultiplyer);
                }
                else if (accelerationInput < -0.1f || carVelocity.z < -1)
                {
                    carBody.AddTorque(Vector3.up * steeringInput * sign * turn * 100 * TurnMultiplyer);
                }
                
                // mormal brakelogic
                if (!kartLike)
                {
                    if (brakeInput > 0.1f)
                    {
                        rb.constraints = RigidbodyConstraints.FreezeRotationX;
                    }
                    else
                    {
                        rb.constraints = RigidbodyConstraints.None;
                    }
                }
                
                // Boost logic
                ManageBoost();

                //accelaration logic

                if (movementMode == MovementMode.AngularVelocity)
                {
                    if (Mathf.Abs(accelerationInput) > 0.1f && brakeInput < 0.1f && !kartLike)
                    {
                        rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, carBody.transform.right * accelerationInput * MaxSpeed / radius, accelaration * Time.deltaTime);
                    }
                    else if (Mathf.Abs(accelerationInput) > 0.1f && kartLike)
                    {
                        rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, carBody.transform.right * accelerationInput * MaxSpeed / radius, accelaration * Time.deltaTime);
                    }
                }
                else if (movementMode == MovementMode.Velocity)
                {
                    if (Mathf.Abs(accelerationInput) > 0.1f && brakeInput < 0.1f && !kartLike)
                    {
                        rb.velocity = Vector3.Lerp(rb.velocity, carBody.transform.forward * accelerationInput * MaxSpeed, accelaration / 10 * Time.deltaTime);
                    }
                    else if (Mathf.Abs(accelerationInput) > 0.1f && kartLike)
                    {
                        rb.velocity = Vector3.Lerp(rb.velocity, carBody.transform.forward * accelerationInput * MaxSpeed, accelaration / 10 * Time.deltaTime);
                    }
                }

                // down froce
                rb.AddForce(-transform.up * downforce * rb.mass);

                //body tilt
                carBody.MoveRotation(Quaternion.Slerp(carBody.rotation, Quaternion.FromToRotation(carBody.transform.up, hit.normal) * carBody.transform.rotation, 0.12f));
            }
            else
            {
                if (AirControl)
                {
                    //turnlogic
                    float TurnMultiplyer = turnCurve.Evaluate(carVelocity.magnitude / MaxSpeed);

                    carBody.AddTorque(Vector3.up * steeringInput * turn * 100 * TurnMultiplyer);
                }

                carBody.MoveRotation(Quaternion.Slerp(carBody.rotation, Quaternion.FromToRotation(carBody.transform.up, Vector3.up) * carBody.transform.rotation, 0.02f));
                rb.velocity = Vector3.Lerp(rb.velocity, rb.velocity + Vector3.down * gravity, Time.deltaTime * gravity);
            }

        }

        #endregion
        
        #region Input
        private void ProcessInputs()
        {
            // Check if we have an input override component and it's active
            CarControllerInputOverride inputOverride = GetComponent<CarControllerInputOverride>();
            if (inputOverride != null && inputOverride.overrideInputs)
            {
                // Utiliser les inputs de l'IA
                steeringInput = inputOverride.steeringInput;
                accelerationInput = inputOverride.accelerationInput;
                brakeInput = inputOverride.brakeInput;
                boostInput = inputOverride.boostInput;
            }
            else
            {
                // Utiliser le système d'input du joueur
                Vector2 input = steeringAction.Steering.Steering.ReadValue<Vector2>();
                steeringInput = input.x;
                accelerationInput = input.y;
                brakeInput = steeringAction.Steering.Brake.ReadValue<float>();
        
                // Lire l'input de boost
                boostInput = steeringAction.Steering.Boost.ReadValue<float>();
            }
    
            // Activer ou désactiver le boost selon l'input
            if (boostInput > 0.5f)
            {
                ActivateBoost();
            }
            else if (isBoostActive)
            {
                DeactivateBoost();
            }


            if (isBoostActive && boostInput > 0.5f)
            {
                virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(virtualCamera.m_Lens.FieldOfView, 120, Time.deltaTime * 5);
            }
            else
            {
                virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(virtualCamera.m_Lens.FieldOfView, 80, Time.deltaTime * 5);
            }
            
            
        }
        
        #endregion

        #region Audio And Visuals
        public void AudioManager()
        {
            engineSound.pitch = Mathf.Lerp(minPitch, MaxPitch, Mathf.Abs(carVelocity.z) / MaxSpeed);
            if (Mathf.Abs(carVelocity.x) > 10 && grounded())
            {
                SkidSound.mute = false;
            }
            else
            {
                SkidSound.mute = true;
            }
        }
        
        public void Visuals()
        {
            //tires
            foreach (Transform FW in FrontWheels)
            {
                FW.localRotation = Quaternion.Slerp(FW.localRotation, Quaternion.Euler(FW.localRotation.eulerAngles.x,
                                   30 * steeringInput, FW.localRotation.eulerAngles.z), 0.7f * Time.deltaTime / Time.fixedDeltaTime);
                FW.GetChild(0).localRotation = rb.transform.localRotation;
            }
            RearWheels[0].localRotation = rb.transform.localRotation;
            RearWheels[1].localRotation = rb.transform.localRotation;

            //Body
            if (carVelocity.z > 1)
            {
                BodyMesh.localRotation = Quaternion.Slerp(BodyMesh.localRotation, Quaternion.Euler(Mathf.Lerp(0, -5, carVelocity.z / MaxSpeed),
                                   BodyMesh.localRotation.eulerAngles.y, BodyTilt * steeringInput), 0.4f * Time.deltaTime / Time.fixedDeltaTime);
            }
            else
            {
                BodyMesh.localRotation = Quaternion.Slerp(BodyMesh.localRotation, Quaternion.Euler(0, 0, 0), 0.4f * Time.deltaTime / Time.fixedDeltaTime);
            }


            if (kartLike)
            {
                if (brakeInput > 0.1f)
                {
                    BodyMesh.parent.localRotation = Quaternion.Slerp(BodyMesh.parent.localRotation,
                    Quaternion.Euler(0, 45 * steeringInput * Mathf.Sign(carVelocity.z), 0),
                    0.1f * Time.deltaTime / Time.fixedDeltaTime);
                }
                else
                {
                    BodyMesh.parent.localRotation = Quaternion.Slerp(BodyMesh.parent.localRotation,
                    Quaternion.Euler(0, 0, 0),
                    0.1f * Time.deltaTime / Time.fixedDeltaTime);
                }

            }

        }
        
        #endregion
        
        #region Boost
        
        // Activer le boost
        private void ActivateBoost()
        {
            if (currentBoostAmount <= 0) 
            {
                // Jouer le son de boost vide si disponible
                if (boostEmptySound != null && boostAudioSource != null && !boostAudioSource.isPlaying)
                    boostAudioSource.PlayOneShot(boostEmptySound);
                return;
            }
        
            if (!isBoostActive)
            {
                // Activer le boost pour la première fois
                isBoostActive = true;
                
                // Démarrer les effets visuels
                if (boostParticles != null && !boostParticles.isPlaying)
                    boostParticles.Play();
                
                // Jouer le son du boost
                if (boostSound != null)
                {
                    if (boostAudioSource == null)
                    {
                        boostAudioSource = gameObject.AddComponent<AudioSource>();
                        boostAudioSource.playOnAwake = false;
                        boostAudioSource.spatialBlend = engineSound.spatialBlend;
                        boostAudioSource.volume = 0.7f;
                    }
                    boostAudioSource.clip = boostSound;
                    boostAudioSource.loop = true;
                    boostAudioSource.Play();
                }
            }
        
            // Appliquer immédiatement la vitesse max du boost
            currentBoostMultiplier = boostMaxSpeedMultiplier;
            MaxSpeed = originalMaxSpeed * currentBoostMultiplier;
            
            
        }
        
        // Désactiver le boost
        private void DeactivateBoost()
        {
            isBoostActive = false;
            
            // Arrêter les effets
            if (boostParticles != null)
                boostParticles.Stop();
                
            if (boostAudioSource != null && boostAudioSource.isPlaying)
                boostAudioSource.Stop();
            
        }
        
        // Obtenir le pourcentage de boost restant (0-1)
        public float GetBoostPercentage()
        {
            return currentBoostAmount / maxBoostAmount;
        }
        
        public float GetBoostAmount()
        {
            return currentBoostAmount;
        }
        
        public float GetMaxBoostAmount()
        {
            return maxBoostAmount;
        }
        
        // Gérer le boost dans FixedUpdate
        private void ManageBoost()
        {
            Debug.Log(currentBoostAmount);
            // Si le boost est actif et il reste du carburant
            if (isBoostActive)
            {
                // Consommer le boost
                currentBoostAmount -= boostConsumptionRate * Time.fixedDeltaTime;
                
                // Vérifier si le boost est épuisé
                if (currentBoostAmount <= 0)
                {
                    currentBoostAmount = 0;
                    DeactivateBoost();
                }
                
            }
            // Si le boost n'est pas actif mais le multiplicateur est encore supérieur à 1
            else if (currentBoostMultiplier > 1.0f)
            {
                // Retour progressif à la vitesse normale
                currentBoostMultiplier = Mathf.Lerp(currentBoostMultiplier, 1.0f, Time.fixedDeltaTime * boostFalloffSpeed);
                MaxSpeed = originalMaxSpeed * currentBoostMultiplier;
                
                // Réinitialiser quand on est presque à la normale
                if (currentBoostMultiplier < 1.01f)
                {
                    currentBoostMultiplier = 1.0f;
                    MaxSpeed = originalMaxSpeed;
                }
            }
            
            // Recharger le boost quand il n'est pas utilisé
            if (!isBoostActive && boostAutoRecharge && currentBoostAmount < maxBoostAmount)
            {
                currentBoostAmount += boostRechargeRate * Time.fixedDeltaTime;
                currentBoostAmount = Mathf.Min(currentBoostAmount, maxBoostAmount);
            }
        }
        
        public void ReloadBoost()
        {
            if (!isBoostActive)
            {
                // Recharge le boost en fonction de la vitesse laterale en utilisant la vitesse sur l'axe x normalized
                float lateralSpeed = Mathf.Abs(carVelocity.x);
                // Clamp la vitesse latérale pour éviter une recharge trop rapide
                lateralSpeed = Mathf.Clamp(lateralSpeed, 0, 4);
                currentBoostAmount += lateralSpeed * Time.fixedDeltaTime;
                currentBoostAmount = Mathf.Min(currentBoostAmount, maxBoostAmount);
            }
        }
        
        #endregion

        #region Car Status Check
        
        public bool grounded() //checks for if vehicle is grounded or not
        {
            origin = rb.position + rb.GetComponent<SphereCollider>().radius * Vector3.up;
            var direction = -transform.up;
            var maxdistance = rb.GetComponent<SphereCollider>().radius + 0.2f;

            if (GroundCheck == groundCheck.rayCast)
            {
                if (Physics.Raycast(rb.position, Vector3.down, out hit, maxdistance, drivableSurface))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            else if (GroundCheck == groundCheck.sphereCaste)
            {
                if (Physics.SphereCast(origin, radius + 0.1f, direction, out hit, maxdistance, drivableSurface))
                {
                    return true;

                }
                else
                {
                    return false;
                }
            }
            else { return false; }
        }
        
        #endregion

        #region Debug
        private void OnDrawGizmos()
        {
            //debug gizmos
            radius = rb.GetComponent<SphereCollider>().radius;
            float width = 0.02f;
            if (!Application.isPlaying)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(rb.transform.position + ((radius + width) * Vector3.down), new Vector3(2 * radius, 2 * width, 4 * radius));
                if (GetComponent<BoxCollider>())
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireCube(transform.position, GetComponent<BoxCollider>().size);
                }

            }

        }
        
        #endregion

    }
}
