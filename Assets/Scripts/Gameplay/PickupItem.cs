using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public enum ItemType { Flashlight }

    public ItemType itemType = ItemType.Flashlight;
    // Prefab to give the player when picked up (e.g. a flashlight GameObject with the Flashlight script)
    public GameObject itemPrefab;
    // Optional: make the pickup rotate so it's visible
    public float rotationSpeed = 50f;

    void Update()
    {
        if (rotationSpeed != 0f)
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
    }

    // Called by PlayerInteraction when the player picks this up
    public void Pickup(GameObject player)
    {
        if (itemType == ItemType.Flashlight && itemPrefab != null)
        {
            // Instantiate and parent to player so it moves with the player
            var instance = GameObject.Instantiate(itemPrefab);
            instance.transform.SetParent(player.transform, false);
            // Position the flashlight at the player's position (adjust in-editor as needed)
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;
        }

        // Destroy pickup object in the world
        Destroy(gameObject);
    }
}
