using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public Camera playerCamera;
    public float interactRange = 3f;

    void Reset()
    {
        // Try to auto-assign main camera
        if (playerCamera == null && Camera.main != null)
            playerCamera = Camera.main;
    }

    void Update()
    {
        if (playerCamera == null)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryPickup();
        }
    }

    void TryPickup()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
        {
            var pickup = hit.collider.GetComponentInParent<PickupItem>();
            if (pickup != null)
            {
                pickup.Pickup(gameObject);
            }
        }
    }
}
