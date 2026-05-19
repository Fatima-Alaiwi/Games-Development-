using System.Collections;
using UnityEngine;

public class TruckCutsceneManager : MonoBehaviour
{
    [Header("Cameras")]
    [Tooltip("The cinematic camera set up to watch the drone heist scene unfold.")]
    public Camera cutsceneCamera;

    [Header("Actors & Assets")]
    [Tooltip("The Drone GameObject parent that will move along the waypoints.")]
    public Transform drone;
    [Tooltip("The visual Power Cell model currently resting on the truck bed.")]
    public GameObject powerCellOnTruck;
    [Tooltip("Reference to the truck instance component in the scene.")]
    public SciFiTruckController truckController;

    [Header("Player Teleportation Target")]
    [Tooltip("An empty 3D GameObject indicating where the player should stand to look on safely.")]
    public Transform playerCutsceneStandingPoint;

    [Header("Drone Path Flight Waypoints")]
    [Tooltip("Spawn/Entry coordinate for the incoming drone.")]
    public Transform pointA; 
    [Tooltip("The exact alignment point over the truck bed to secure the cargo.")]
    public Transform pointB; 
    [Tooltip("The exit trajectory point out of the operational area.")]
    public Transform pointC; 

    [Header("Drone Speed Adjustments")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 180f;

    private bool _isCutscenePlaying = false;

    public void StartCutscene()
    {
        if (_isCutscenePlaying) return;
        StartCoroutine(PlayCutsceneSequence());
    }

    private IEnumerator PlayCutsceneSequence()
    {
        _isCutscenePlaying = true;

        if (truckController != null)
        {
            // 1. Lock down interaction parameters instantly so player can't re-enter mid-scene
            truckController.isInteractable = false;
            
            // 2. Clear driving states, turn off vehicle camera, and extract active player references
            truckController.ExitVehicleForCutscene(out MonoBehaviour playerScript, out Camera playerCamera);

            // 3. Teleport player character to your empty spot so they can view the event natively
            if (playerScript != null && playerCutsceneStandingPoint != null)
            {
                playerScript.transform.position = playerCutsceneStandingPoint.position;
                playerScript.transform.rotation = playerCutsceneStandingPoint.rotation;

                // 4. Safely reactivate player locomotion loops immediately
                var field = playerScript.GetType().GetField("canMove");
                if (field != null)
                {
                    field.SetValue(playerScript, true);
                }
            }

            // 5. Shift perspective from player's view over to the cinematic framing layout
            if (cutsceneCamera != null)
            {
                if (playerCamera != null) playerCamera.gameObject.SetActive(false);
                cutsceneCamera.gameObject.SetActive(true);
            }
        }

        // Initialize drone orientation state at Point A
        if (drone != null && pointA != null)
        {
            drone.position = pointA.position;
            drone.rotation = pointA.rotation;
        }

        yield return new WaitForSeconds(0.5f); // Brief buffer frame for visual transition matching

        // STEP I: Fly from high/away space down to the flat truck bed alignment
        if (pointB != null)
        {
            yield return StartCoroutine(MoveAndRotateDrone(pointB.position, pointB.rotation));
        }

        // STEP II: Hide the cell visual attached to the truck to simulate the drone detaching it
        if (powerCellOnTruck != null)
        {
            powerCellOnTruck.SetActive(false); 
        }
        
        yield return new WaitForSeconds(0.6f); // Short structural hold for kinematic weight feedback

        // STEP III: Execute a crisp 180-degree pivot orientation update facing out towards Point C
        if (pointC != null && drone != null)
        {
            Quaternion targetTurnRotation = Quaternion.LookRotation(pointC.position - drone.position);
            yield return StartCoroutine(RotateDroneOnly(targetTurnRotation));
        }

        // STEP IV: Accelerate away out of the sky vector track bounds
        if (pointC != null)
        {
            yield return StartCoroutine(MoveAndRotateDrone(pointC.position, pointC.rotation));
        }

        EndCutscene();
    }

    private IEnumerator MoveAndRotateDrone(Vector3 targetPosition, Quaternion targetRotation)
    {
        while (Vector3.Distance(drone.position, targetPosition) > 0.05f)
        {
            drone.position = Vector3.MoveTowards(drone.position, targetPosition, moveSpeed * Time.deltaTime);
            drone.rotation = Quaternion.RotateTowards(drone.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }
        drone.position = targetPosition;
    }

    private IEnumerator RotateDroneOnly(Quaternion targetRotation)
    {
        while (Quaternion.Angle(drone.rotation, targetRotation) > 0.1f)
        {
            drone.rotation = Quaternion.RotateTowards(drone.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }
        drone.rotation = targetRotation;
    }

    private void EndCutscene()
    {
        _isCutscenePlaying = false;
        
        // Return view field targeting straight back to player camera setup 
        if (cutsceneCamera != null && cutsceneCamera.gameObject.activeSelf)
        {
            cutsceneCamera.gameObject.SetActive(false);
            if (truckController != null && truckController.playerCamera != null)
            {
                truckController.playerCamera.gameObject.SetActive(true);
            }
        }

        // Hide or clear out the drone reference so it doesn't linger static in the sky matrix
        if (drone != null)
        {
            drone.gameObject.SetActive(false);
        }

        Debug.Log("Sequence Complete: Power cell stolen. Control entirely restored to default loops.");
    }
}