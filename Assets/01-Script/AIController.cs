using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

namespace CarController
{
    public class AIController : MonoBehaviour
    {
        #region Variables

        [Header("Car Settings")]
        [SerializeField] private CarController carController;
        [SerializeField] private float maxSpeed = 15f;
        [SerializeField] private float steeringResponse = 5f;
        [SerializeField] private float baseLookAheadDistance = 5f;
        [SerializeField] private float speedLookAheadFactor = 0.2f;
        [SerializeField] private float reverseTime = 2f;
        
        [Header("Multi-Car Settings")]
        [SerializeField] private float carAvoidanceDistance = 5.0f;
        [SerializeField] private float carAvoidanceStrength = 2.0f;
        [SerializeField] private float initialBoostDuration = 1.0f;
        private float startTimer;
        private bool hasStarted = false;
        
        [Header("Multi-Car Initialization")]
        [SerializeField] private float randomStartDelayMin = 0.1f;
        [SerializeField] private float randomStartDelayMax = 0.5f;
        [SerializeField] private float initialCarSpacing = 2.0f;
        private int uniqueCarID;
        private static int totalCarCount = 0;
        private bool isInitialized = false;

        [Header("Cornering Settings")]
        [SerializeField] private float corneringSpeedFactor = 0.6f;
        [SerializeField] private float cornerDetectionDistance = 10f;
        [SerializeField] private float cornerAngleThreshold = 45f;
        [SerializeField] private float corneringSmoothness = 0.5f; // Higher = more gradual slowdown

        [Header("Recovery Settings")]
        [SerializeField] private float pathDeviationThreshold = 5f;
        [SerializeField] private float recoverySteeringMultiplier = 1.5f;
        [SerializeField] private float stuckTime = 2f;
        [SerializeField] private float recoveryDuration = 3f;

        [Header("Checkpoint System")]
        [SerializeField] private int numberOfCheckpoints = 20;
        [SerializeField] private float checkpointReachedDistance = 5f;
        [SerializeField] private bool showCheckpoints = true;
        [SerializeField] private int lookaheadCheckpoints = 2; // Look ahead this many checkpoints for smoother cornering

        [Header("Path Settings")]
        [SerializeField] private SplineContainer splinePath;
        [SerializeField] private bool loopPath = true;
        
        [Header("Path Variation")]
        [SerializeField] private float lateralOffsetAmount = 1.5f; // Maximum side-to-side deviation
        [SerializeField] private float offsetVariationFrequency = 0.05f; // How quickly offset changes along track
        private int aiSeed; // Unique seed for this AI instance
        private float currentPathOffset = 0f; // Current lateral offset value
        [SerializeField] private bool useRacingLine = true;
        [SerializeField] private float cornerEntryOffset = 1.5f; // Positive = outside of turn
        [SerializeField] private float cornerApexOffset = -0.8f; // Negative = inside of turn
        [SerializeField] private float cornerExitOffset = 1.2f;  // Positive = outside of turn

        [Header("Input Control")]
        private float currentSteeringInput = 0f;
        private float currentAccelerationInput = 0f;
        private float currentBrakeInput = 0f;
        private bool waitingForFirstUpdate = true;
        bool inputsApplied;
        
        [Header("Aggressive Cornering")]
        [SerializeField] private float corneringMinSpeed = 5f; // Minimum speed in corners
        [SerializeField] private bool debugAIState = true;

        private float currentSplinePosition = 0f;
        private float splineLength;
        private float currentSpeed = 0f;
        private bool isApproachingCorner = false;
        private float brakeInput = 0f;
        private bool isRecovering = false;
        private float recoveryTimer = 0f;
        private Vector3 lastPosition;
        private float stuckTimer = 0f;
        private bool isReversing = false;
        private float reverseTimer = 0f;
        private float cornerSpeedModifier = 1f;
        private bool isPostRecovery = false;
        private float postRecoveryTimer = 0f;
        private float postRecoveryDuration = 2.5f;

        // Checkpoint system
        private List<Vector3> checkpoints = new List<Vector3>();
        private int currentCheckpointIndex = 0;
        private int nextCheckpointIndex = 1;
        private int targetCheckpointIndex = 1;
        private Vector3 targetPoint;
        private Vector3 previousTargetPoint;
        private float checkpointChangeTime;

        [Header("Debug Settings")]
        [SerializeField] private bool showDebugPath = true;
        [SerializeField] private int debugPathSegments = 100;

        #endregion

        #region Unity Methods

        void Start()
        {
            // Ensure unique ID for each car
            uniqueCarID = GetInstanceID(); // Use instance ID directly
            aiSeed = uniqueCarID;
    
            // Each car will get a different delay based on instance ID
            float startDelay = (uniqueCarID % 100) * 0.02f; 
            startTimer = -startDelay;
    
            // Ensure car controller reference
            if (carController == null)
                carController = GetComponent<CarController>();
    
            // Add input override component immediately
            var inputOverride = GetComponent<CarControllerInputOverride>();
            if (inputOverride == null)
                gameObject.AddComponent<CarControllerInputOverride>();
        
            // Initialize path immediately rather than with coroutine
            if (splinePath != null)
            {
                splineLength = splinePath.CalculateLength();
                GenerateCheckpoints();
                InitializeStartingPoint();
            }
    
            // Set initial values
            lastPosition = transform.position;
            previousTargetPoint = transform.position + transform.forward * 10f;
            isInitialized = true;
    
            if (debugAIState)
                Debug.Log($"Car {uniqueCarID} initialized with {checkpoints.Count} checkpoints");
        }

        private void Update()
        {
            // Skip if not initialized or missing dependencies
            if (!isInitialized || splinePath == null || carController == null)
                return;

            // Handle race start with the staggered delay
            if (!hasStarted)
            {
                startTimer += Time.deltaTime;
                if (startTimer < 0.2f) // Initial wait period
                {
                    return;
                }
                else if (startTimer < initialBoostDuration + 0.2f) // Initial boost phase
                {
                    SetInputs(0f, 1f, 0f);
                    return;
                }
                else
                {
                    hasStarted = true;
                }
            }
            
            if (splinePath == null || carController == null)
                return;

            // Calculate current speed
            currentSpeed = carController.carVelocity.magnitude;

            // Check if stuck
            CheckIfStuck();

            if (isReversing)
            {
                HandleReversing();
                return;
            }

            // Manage recovery state
            ManageRecoveryState();
            // Update checkpoint progression
            UpdateCheckpointProgress();

            // Check for upcoming corners
            isApproachingCorner = DetectCornerAhead();
            UpdateCornerSpeed();

            // Determine target point
            DetermineTargetPoint();

            // Calculate steering
            Vector3 directionToTarget = targetPoint - transform.position;
            Vector3 localDirection = transform.InverseTransformDirection(directionToTarget);
            float steeringInput = Mathf.Clamp(localDirection.x / steeringResponse, -1f, 1f);

            // Apply more aggressive steering during recovery
            if (isRecovering)
                steeringInput *= recoverySteeringMultiplier;

            // Calculate forward vector for alignment
            Vector3 forwardOnPath;
            if (checkpoints.Count > 0)
            {
                // Use checkpoint direction for alignment
                forwardOnPath = (targetPoint - transform.position).normalized;
            }
            else
            {
                // Fallback to spline tangent
                forwardOnPath = GetTangentOnSpline(currentSplinePosition);
            }

            // Calculate alignment and target speed
            float alignment = Vector3.Dot(transform.forward, forwardOnPath);
    
            // Ensure minimum speed factor for hairpin turns (prevents complete stops)
            float minCornerSpeedFactor = 0.3f; // Adjust this value based on your max speed
            float adjustedCornerSpeedModifier = Mathf.Max(cornerSpeedModifier, minCornerSpeedFactor);
    
            float targetSpeed = maxSpeed * adjustedCornerSpeedModifier;
            float speedDifference = targetSpeed - currentSpeed;

            // Determine acceleration/braking
            float accelerationInput = 0f;

            // Force strong acceleration during post-recovery
            if (isPostRecovery)
            {
                accelerationInput = 1.0f;
                brakeInput = 0f;
            }
            // Maintain minimum forward momentum in tight turns
            else if (isApproachingCorner && currentSpeed < maxSpeed * minCornerSpeedFactor)
            {
                accelerationInput = 0.5f;
                brakeInput = 0f;
            }
            // Normal driving
            else if (speedDifference > 0 && alignment > 0.5f)
            {
                accelerationInput = Mathf.Lerp(0.5f, 1.0f, alignment);
            }
            else if (speedDifference < -2f || alignment < 0.3f)
            {
                brakeInput = Mathf.Lerp(0.3f, 0.7f, 1.0f - alignment);
            }
            else
            {
                brakeInput = 0f;
            }

            // Apply inputs to car controller
            SetInputs(steeringInput, accelerationInput, brakeInput);

            // Save previous target for smoothing
            previousTargetPoint = targetPoint;
        }
        
        private void FixedUpdate()
        {
            // This ensures inputs are applied in the physics update
            if (carController != null)
            {
                // Apply our custom inputs directly through a special field
                // We'll add this field to CarController
                CarControllerInputOverride inputOverride = carController.gameObject.GetComponent<CarControllerInputOverride>();
                if (inputOverride == null)
                {
                    inputOverride = carController.gameObject.AddComponent<CarControllerInputOverride>();
                }
        
                if (inputOverride != null)
                {
                    inputOverride.steeringInput = currentSteeringInput;
                    inputOverride.accelerationInput = currentAccelerationInput;
                    inputOverride.brakeInput = currentBrakeInput;
                    inputOverride.overrideInputs = true;
                    inputsApplied = true;
                }
            }
        }

        #endregion

        #region Checkpoint System
        
        private void InitializeStartingPoint()
        {
            currentCheckpointIndex = FindClosestCheckpointIndex();
    
            // Stagger cars along the track
            if (uniqueCarID > 0)
            {
                // Offset by 1-5 checkpoints based on car ID
                int offset = uniqueCarID % 5 + 1;
                currentCheckpointIndex = (currentCheckpointIndex + offset) % checkpoints.Count;
            }
    
            nextCheckpointIndex = GetNextCheckpointIndex(currentCheckpointIndex);
            targetCheckpointIndex = CalculateTargetCheckpoint();
        }
        
        private IEnumerator InitializeWithDelay()
        {
            yield return null; // Wait one frame
    
            if (splinePath != null)
            {
                splineLength = splinePath.CalculateLength();
                GenerateCheckpoints();
        
                // Verify the checkpoints were generated correctly
                if (checkpoints.Count == 0)
                {
                    Debug.LogError($"Car {uniqueCarID}: Failed to generate checkpoints!");
                    yield break;
                }
        
                // Find best starting point with some space from other cars
                FindOptimalStartingCheckpoint();
            }
    
            lastPosition = transform.position;
            previousTargetPoint = transform.position + transform.forward * 10;
    
            // Set initial random offset
            currentPathOffset = Random.Range(-lateralOffsetAmount, lateralOffsetAmount);
    
            // Make sure we're not inside another car
            CheckAndFixCollisions();
    
            isInitialized = true;
            hasStarted = false; // Will be set to true after the startup sequence
    
            Debug.Log($"Car {uniqueCarID} initialized successfully with {checkpoints.Count} checkpoints.");
        }
        
        private void FindOptimalStartingCheckpoint()
        {
            // First find the closest checkpoint as a starting point
            currentCheckpointIndex = FindClosestCheckpointIndex();
    
            // Look for a checkpoint that's not too close to another car's checkpoint
            Collider[] nearbyCars = Physics.OverlapSphere(transform.position, initialCarSpacing * 3);
    
            foreach (var carCollider in nearbyCars)
            {
                AIController otherCar = carCollider.GetComponent<AIController>();
                if (otherCar != null && otherCar != this && otherCar.isActiveAndEnabled)
                {
                    // Try to find a checkpoint that's further ahead
                    for (int offset = 1; offset < checkpoints.Count/4; offset++)
                    {
                        int candidateIndex = (currentCheckpointIndex + offset) % checkpoints.Count;
                        if (Vector3.Distance(checkpoints[candidateIndex], transform.position) < 
                            Vector3.Distance(checkpoints[candidateIndex], otherCar.transform.position))
                        {
                            currentCheckpointIndex = candidateIndex;
                            break;
                        }
                    }
                }
            }
    
            nextCheckpointIndex = GetNextCheckpointIndex(currentCheckpointIndex);
            targetCheckpointIndex = CalculateTargetCheckpoint();
        }
        
        private void CheckAndFixCollisions()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 1.5f);
            foreach (var collider in colliders)
            {
                if (collider.gameObject != gameObject && collider.GetComponent<CarController>() != null)
                {
                    // We're too close to another car, move slightly to the side
                    Vector3 awayDirection = (transform.position - collider.transform.position).normalized;
                    transform.position += awayDirection * 1.5f;
                    transform.position = new Vector3(transform.position.x, 
                        transform.position.y + 0.1f, 
                        transform.position.z);
                }
            }
        }

        private void GenerateCheckpoints()
        {
            checkpoints.Clear();

            for (int i = 0; i < numberOfCheckpoints; i++)
            {
                float t = (float)i / numberOfCheckpoints;
                if (loopPath)
                    t = Mathf.Repeat(t, 1f);
                else
                    t = Mathf.Clamp01(t);

                Vector3 checkpointPosition = splinePath.EvaluatePosition(t);
                checkpoints.Add(checkpointPosition);
            }

            // Add one more checkpoint at the end if not looping
            if (!loopPath)
                checkpoints.Add(splinePath.EvaluatePosition(1f));

            // Find closest checkpoint to start with
            currentCheckpointIndex = FindClosestCheckpointIndex();
            nextCheckpointIndex = GetNextCheckpointIndex(currentCheckpointIndex);
            targetCheckpointIndex = nextCheckpointIndex;
        }

        private void UpdateCheckpointProgress()
        {
            if (checkpoints.Count == 0)
                return;

            float distanceToNextCheckpoint = Vector3.Distance(transform.position, checkpoints[nextCheckpointIndex]);

            // More lenient checkpoint detection
            if (distanceToNextCheckpoint < checkpointReachedDistance * 1.5f)
            {
                ProgressToNextCheckpoint();
            }
            // Check if we've passed the checkpoint
            else
            {
                Vector3 checkpointToVehicle = transform.position - checkpoints[nextCheckpointIndex];
                Vector3 checkpointToNext = (nextCheckpointIndex + 1 < checkpoints.Count || loopPath) ?
                    checkpoints[GetNextCheckpointIndex(nextCheckpointIndex)] - checkpoints[nextCheckpointIndex] :
                    checkpoints[nextCheckpointIndex] - checkpoints[currentCheckpointIndex];

                // More lenient check for passing checkpoints
                float dotProduct = Vector3.Dot(checkpointToVehicle.normalized, checkpointToNext.normalized);
                if (dotProduct > 0.5f && distanceToNextCheckpoint < checkpointReachedDistance * 4)
                {
                    ProgressToNextCheckpoint();
                }
            }
        }
        
        private void ProgressToNextCheckpoint()
        {
            currentCheckpointIndex = nextCheckpointIndex;
            nextCheckpointIndex = GetNextCheckpointIndex(currentCheckpointIndex);
            targetCheckpointIndex = CalculateTargetCheckpoint();
            checkpointChangeTime = Time.time;
    
            // Reset recovery/stuck timers when making progress
            stuckTimer = -1f;
            if (isPostRecovery)
                isPostRecovery = false;
        }

        private int GetNextCheckpointIndex(int currentIndex)
        {
            if (!loopPath && currentIndex >= checkpoints.Count - 1)
                return currentIndex;
            return (currentIndex + 1) % checkpoints.Count;
        }

        private int CalculateTargetCheckpoint()
        {
            // Look ahead several checkpoints to smooth corners
            int targetIndex = nextCheckpointIndex;
            for (int i = 0; i < lookaheadCheckpoints; i++)
            {
                int nextIndex = GetNextCheckpointIndex(targetIndex);
                if (nextIndex == targetIndex) break; // End of non-looping path
                targetIndex = nextIndex;
            }
            return targetIndex;
        }

        private int FindClosestCheckpointIndex()
        {
            float closestDistance = float.MaxValue;
            int closestIndex = 0;

            for (int i = 0; i < checkpoints.Count; i++)
            {
                float distance = Vector3.Distance(transform.position, checkpoints[i]);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestIndex = i;
                }
            }

            return closestIndex;
        }

        private bool IsCheckpointAhead(int candidateIndex, int referenceIndex)
        {
            if (loopPath)
            {
                // Consider trajectory and distance
                Vector3 forwardVector = transform.forward;
                Vector3 toCandidate = (checkpoints[candidateIndex] - transform.position).normalized;
                float dot = Vector3.Dot(forwardVector, toCandidate);
                
                // If angle < 90 degrees, checkpoint is somewhat ahead
                return dot > 0;
            }
            else
            {
                // For non-looped paths, simply check if the index is greater
                return candidateIndex > referenceIndex;
            }
        }

        private void DetermineTargetPoint()
        {
            // Calculate target point based on current state
            Vector3 newTargetPoint;
        
            if (checkpoints.Count > 0)
            {
                // Use checkpoint system with offset
                newTargetPoint = checkpoints[targetCheckpointIndex];
        
                // Only apply offset during normal driving (not recovery)
                if (!isRecovering && !isReversing && !isPostRecovery)
                {
                    // Calculate path direction
                    Vector3 pathDirection;
                    Vector3 currentToNext;
        
                    if (targetCheckpointIndex != currentCheckpointIndex)
                    {
                        pathDirection = (checkpoints[targetCheckpointIndex] - checkpoints[currentCheckpointIndex]).normalized;
                        currentToNext = pathDirection;
                    }
                    else if (nextCheckpointIndex != currentCheckpointIndex)
                    {
                        pathDirection = (checkpoints[nextCheckpointIndex] - checkpoints[currentCheckpointIndex]).normalized;
                        currentToNext = pathDirection;
                    }
                    else
                    {
                        pathDirection = transform.forward;
                        currentToNext = pathDirection;
                    }
        
                    // Get perpendicular vector to path (cross with up for side-to-side)
                    Vector3 perpendicular = Vector3.Cross(Vector3.up, pathDirection).normalized;
        
                    // Determine if we should use racing line or random offset
                    if (useRacingLine && isApproachingCorner)
                    {
                        // Detect corner direction (left or right)
                        int lookAheadIndex = GetNextCheckpointIndex(targetCheckpointIndex);
                        Vector3 nextSegment = (checkpoints[lookAheadIndex] - checkpoints[targetCheckpointIndex]).normalized;
        
                        // Positive = right turn, Negative = left turn
                        float turnDirection = Vector3.SignedAngle(currentToNext, nextSegment, Vector3.up);
        
                        // Calculate progress through current segment
                        float segmentLength = Vector3.Distance(checkpoints[currentCheckpointIndex], checkpoints[nextCheckpointIndex]);
                        float distanceFromCurrent = Vector3.Distance(transform.position, checkpoints[currentCheckpointIndex]);
                        float segmentProgress = segmentLength > 0 ? distanceFromCurrent / segmentLength : 0f;
        
                        // Apply racing line offset based on corner direction and progress
                        float racingLineOffset;
        
                        if (segmentProgress < 0.3f) // Corner entry
                            racingLineOffset = cornerEntryOffset;
                        else if (segmentProgress < 0.7f) // Corner apex
                            racingLineOffset = cornerApexOffset;
                        else // Corner exit
                            racingLineOffset = cornerExitOffset;
        
                        // Apply the offset in the correct direction (positive offset goes to the outside of turn)
                        currentPathOffset = Mathf.Lerp(currentPathOffset, racingLineOffset * Mathf.Sign(turnDirection), 0.1f);
                    }
                    else
                    {
                        // Random variation for straights or if racing line is disabled
                        float checkpointDistance = Vector3.Distance(transform.position, checkpoints[currentCheckpointIndex]);
                        float noiseInput = aiSeed * 0.01f + checkpointDistance * offsetVariationFrequency;
                        float targetOffset = lateralOffsetAmount * (Mathf.PerlinNoise(noiseInput, aiSeed * 0.01f) * 2 - 1);
        
                        // Limit offset in corners to avoid bad lines
                        if (isApproachingCorner) {
                            targetOffset *= 0.4f; // Reduce offset in corners when using random mode
                        }
        
                        currentPathOffset = Mathf.Lerp(currentPathOffset, targetOffset, 0.02f);
                    }
        
                    // Apply offset to target point
                    newTargetPoint += perpendicular * currentPathOffset;
                }
            }
            else
            {
                // Use direct spline following
                float lookAheadDist = baseLookAheadDistance + (currentSpeed * speedLookAheadFactor);
                newTargetPoint = GetPointOnSpline(currentSplinePosition + lookAheadDist);
        
                // Apply offset to spline following too
                if (!isRecovering && !isReversing && !isPostRecovery)
                {
                    Vector3 tangent = GetTangentOnSpline(currentSplinePosition);
                    Vector3 perpendicular = Vector3.Cross(Vector3.up, tangent).normalized;
        
                    float noiseInput = aiSeed * 0.01f + currentSplinePosition * offsetVariationFrequency;
                    currentPathOffset = Mathf.Lerp(currentPathOffset,
                                              lateralOffsetAmount * Mathf.PerlinNoise(noiseInput, aiSeed * 0.01f) * 2 - lateralOffsetAmount,
                                              0.02f);
        
                    newTargetPoint += perpendicular * currentPathOffset;
                }
            }
        
            // Add car avoidance at the end before applying smoothing:
            if (!isRecovering && !isReversing && hasStarted)
            {
                Vector3 avoidanceVector = AvoidOtherCars();
                newTargetPoint += avoidanceVector;
            }
    
            // Smooth target point changes to prevent sudden direction changes
            float smoothFactor = isRecovering ? 0.8f : 0.5f;
            targetPoint = Vector3.Lerp(previousTargetPoint, newTargetPoint, smoothFactor);
        }
        
        private Vector3 AvoidOtherCars()
        {
            Vector3 avoidanceVector = Vector3.zero;
            Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, carAvoidanceDistance);
    
            foreach (var obj in nearbyObjects)
            {
                // Ignore self and non-car objects
                if (obj.gameObject == gameObject || !obj.GetComponent<CarController>())
                    continue;
            
                Vector3 dirToOtherCar = transform.position - obj.transform.position;
                float distance = dirToOtherCar.magnitude;
        
                // Apply stronger avoidance at closer distances
                float avoidanceWeight = 1.0f - Mathf.Clamp01(distance / carAvoidanceDistance);
                avoidanceVector += dirToOtherCar.normalized * avoidanceWeight * carAvoidanceStrength;
            }
    
            return avoidanceVector;
        }

        #endregion

        #region Recovery Methods

        private void ManageRecoveryState()
        {
            bool isCurrentlyOffTrack = IsOffTrack();

            // Start recovery if not already recovering
            if (isCurrentlyOffTrack && !isRecovering && !isPostRecovery)
            {
                isRecovering = true;
                recoveryTimer = 0f;
                stuckTimer = -3f; // Reset stuck timer with plenty of grace time

                // Find a better checkpoint when starting recovery
                if (checkpoints.Count > 0)
                {
                    int nearestIndex = FindClosestVisibleCheckpointIndex();
                    if (nearestIndex != currentCheckpointIndex)
                    {
                        currentCheckpointIndex = nearestIndex;
                        nextCheckpointIndex = GetNextCheckpointIndex(currentCheckpointIndex);
                        targetCheckpointIndex = nextCheckpointIndex;
                    }
                }
            }
            // Continue recovery for a minimum duration to prevent flickering
            else if (isRecovering)
            {
                recoveryTimer += Time.deltaTime;
                if (recoveryTimer > recoveryDuration && !isCurrentlyOffTrack)
                {
                    isRecovering = false;
            
                    // Enter post-recovery state to ensure car gets moving
                    isPostRecovery = true;
                    postRecoveryTimer = 0f;
            
                    // Set target to a checkpoint that's clearly ahead
                    targetCheckpointIndex = GetNextCheckpointIndex(nextCheckpointIndex);
                }
            }
            // Handle post-recovery state
            else if (isPostRecovery)
            {
                postRecoveryTimer += Time.deltaTime;
                if (postRecoveryTimer > postRecoveryDuration || currentSpeed > maxSpeed * 0.5f)
                {
                    isPostRecovery = false;
                }
            }
        }


        private int FindClosestVisibleCheckpointIndex()
        {
            float closestDistance = float.MaxValue;
            int bestIndex = currentCheckpointIndex;

            for (int i = 0; i < checkpoints.Count; i++)
            {
                // Skip checkpoints that aren't ahead of us
                if (!IsCheckpointAhead(i, currentCheckpointIndex))
                    continue;
            
                Vector3 checkpointDir = (checkpoints[i] - transform.position);
                float distance = checkpointDir.magnitude;

                // Only consider checkpoints within reasonable distance
                if (distance < pathDeviationThreshold * 5 && distance < closestDistance)
                {
                    closestDistance = distance;
                    bestIndex = i;
                }
            }

            return bestIndex;
        }

        private bool IsOffTrack()
        {
            if (checkpoints.Count > 0)
            {
                // Distance-based check to next checkpoint
                float distance = Vector3.Distance(transform.position, checkpoints[nextCheckpointIndex]);
                return distance > pathDeviationThreshold * 2;
            }
            else
            {
                // Distance from spline
                Vector3 closestPoint = GetPointOnSpline(FindClosestPointOnSpline());
                return Vector3.Distance(transform.position, closestPoint) > pathDeviationThreshold;
            }
        }

        private float FindClosestPointOnSpline()
        {
            float closestDistance = float.MaxValue;
            float closestPoint = 0f;
            int samples = 50;

            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)samples * splineLength;
                Vector3 pointOnSpline = GetPointOnSpline(t);
                float distance = Vector3.Distance(transform.position, pointOnSpline);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPoint = t;
                }
            }

            return closestPoint;
        }

        private void CheckIfStuck()
        {
            // Skip stuck check during recovery or post-recovery
            if (isRecovering || isPostRecovery)
            {
                stuckTimer = -2f;
                return;
            }

            // Check if we're stuck
            if (currentSpeed < 1f && !isReversing)
            {
                stuckTimer += Time.deltaTime;
                if (stuckTimer > stuckTime)
                {
                    StartReversing();
                }
            }
            else
            {
                stuckTimer = Mathf.Max(stuckTimer - Time.deltaTime, -2f);
            }
        }

        private void StartReversing()
        {
            isReversing = true;
            reverseTimer = 0f;
        }

        private void HandleReversing()
        {
            reverseTimer += Time.deltaTime;

            if (reverseTimer < reverseTime)
            {
                // Create a wiggling reverse movement
                float steeringInput = Mathf.Sin(reverseTimer * 3f) * 0.8f;
                SetInputs(steeringInput, -0.7f, 0f);
            }
            else
            {
                isReversing = false;
                stuckTimer = 0f;
                
                // Reset to closest checkpoint
                if (checkpoints.Count > 0)
                {
                    currentCheckpointIndex = FindClosestVisibleCheckpointIndex();
                    nextCheckpointIndex = GetNextCheckpointIndex(currentCheckpointIndex);
                    targetCheckpointIndex = nextCheckpointIndex;
                }
            }
        }

        #endregion

        #region Input Methods

        private void SetInputs(float steering, float acceleration, float brake)
        {
            // Store values directly
            var inputOverride = GetComponent<CarControllerInputOverride>();
            if (inputOverride == null)
            {
                inputOverride = gameObject.AddComponent<CarControllerInputOverride>();
            }
    
            // Apply inputs directly
            inputOverride.steeringInput = steering;
            inputOverride.accelerationInput = acceleration;
            inputOverride.brakeInput = brake;
            inputOverride.overrideInputs = true;
        }


        #endregion

        #region Path Following Methods

        private Vector3 GetPointOnSpline(float distance)
        {
            float normalizedDistance = Mathf.Repeat(distance / splineLength, 1f);
            return splinePath.EvaluatePosition(normalizedDistance);
        }

        private Vector3 GetTangentOnSpline(float distance)
        {
            float normalizedDistance = Mathf.Repeat(distance / splineLength, 1f);
            Vector3 tangent = splinePath.EvaluateTangent(normalizedDistance);
            return tangent.magnitude > 0 ? tangent / tangent.magnitude : Vector3.forward;
        }

        private bool DetectCornerAhead()
        {
            if (checkpoints.Count > 0)
            {
                // Get our current checkpoint index and position
                Vector3 currentPosition = transform.position;

                // Find checkpoints within the corner detection distance
                int checkpointIndex = nextCheckpointIndex;
                float distanceSum = Vector3.Distance(currentPosition, checkpoints[checkpointIndex]);
                Vector3 currentDirection = (checkpoints[checkpointIndex] - currentPosition).normalized;

                // Look ahead by cornerDetectionDistance
                while (distanceSum < cornerDetectionDistance &&
                       checkpointIndex != ((targetCheckpointIndex + 1) % checkpoints.Count))
                {
                    int nextIndex = (checkpointIndex + 1) % checkpoints.Count;
                    if (!loopPath && nextIndex < checkpointIndex) break;

                    Vector3 nextDirection = (checkpoints[nextIndex] - checkpoints[checkpointIndex]).normalized;
                    float angle = Vector3.Angle(currentDirection, nextDirection);

                    // If sharp angle detected within corner detection distance
                    if (angle > cornerAngleThreshold)
                        return true;

                    // Add distance to next checkpoint
                    distanceSum += Vector3.Distance(checkpoints[checkpointIndex], checkpoints[nextIndex]);
                    currentDirection = nextDirection;
                    checkpointIndex = nextIndex;
                }
            }
            else if (splinePath != null)
            {
                // Use the cornerDetectionDistance for spline-based detection
                float checkDistance = cornerDetectionDistance;
                Vector3 currentTangent = GetTangentOnSpline(currentSplinePosition);
                Vector3 futureTangent = GetTangentOnSpline(currentSplinePosition + checkDistance);

                float angle = Vector3.Angle(currentTangent, futureTangent);
                return angle > cornerAngleThreshold;
            }

            return false;
        }

        private void UpdateCornerSpeed()
        {
            // Calculate a smooth speed adjustment for corners that maintains minimum speed
            if (isApproachingCorner)
            {
                // Use a higher minimum cornering speed
                cornerSpeedModifier = Mathf.Lerp(
                    cornerSpeedModifier, 
                    Mathf.Max(corneringSpeedFactor, corneringMinSpeed / maxSpeed),
                    Time.deltaTime / corneringSmoothness
                );
            }
            else
            {
                cornerSpeedModifier = Mathf.Lerp(
                    cornerSpeedModifier, 
                    1.0f,
                    Time.deltaTime / corneringSmoothness
                );
            }
    
            // Debug corner speed
            if (debugAIState && isApproachingCorner)
                Debug.Log($"Car {uniqueCarID} cornering at {cornerSpeedModifier * maxSpeed} speed");
        }

        #endregion

        #region Debug Methods

        private void OnDrawGizmos()
        {
            if (splinePath == null)
                return;

            // Draw the spline
            if (showDebugPath)
            {
                Gizmos.color = Color.green;
                for (int i = 0; i < debugPathSegments; i++)
                {
                    float t = (float)i / debugPathSegments;
                    float nextT = (float)(i + 1) / debugPathSegments;

                    Vector3 current = splinePath.EvaluatePosition(t);
                    Vector3 next = splinePath.EvaluatePosition(nextT);

                    Gizmos.DrawLine(current, next);
                }
            }

            // Draw checkpoints
            if (showCheckpoints && checkpoints.Count > 0)
            {
                for (int i = 0; i < checkpoints.Count; i++)
                {
                    if (i == currentCheckpointIndex)
                        Gizmos.color = Color.cyan;
                    else if (i == nextCheckpointIndex)
                        Gizmos.color = Color.yellow;
                    else if (i == targetCheckpointIndex)
                        Gizmos.color = Color.magenta;
                    else
                        Gizmos.color = Color.white;

                    Gizmos.DrawSphere(checkpoints[i], 0.7f);

                    if (i < checkpoints.Count - 1)
                        Gizmos.DrawLine(checkpoints[i], checkpoints[i + 1]);
                }

                // Draw line from last to first if looping
                if (loopPath && checkpoints.Count > 1)
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawLine(checkpoints[checkpoints.Count - 1], checkpoints[0]);
                }
            }

            // Draw target point and recovery info
            if (Application.isPlaying)
            {
                // Draw actual target point
                Gizmos.color = isRecovering ? Color.red : Color.green;
                Gizmos.DrawSphere(targetPoint, 0.5f);
                Gizmos.DrawLine(transform.position, targetPoint);
            }
        }

        #endregion
    }
}