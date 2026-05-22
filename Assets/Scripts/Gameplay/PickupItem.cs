using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public enum ItemType { Flashlight, Gun }

    public ItemType itemType = ItemType.Flashlight;
    // Prefab to give the player when picked up (e.g. a flashlight GameObject with the Flashlight script)
    public GameObject itemPrefab;
    // Optional: if set, the instantiated item will be parented to this transform on the player.
    // You can assign this at runtime by finding a child on the player (e.g. a hand anchor) or
    // set it in code before calling Pickup.
    public Transform attachPoint;
    // Local offset applied to the instantiated item so it sits in front of the camera/anchor.
    // Tweak this in the inspector to center the flashlight beam.
    public Vector3 attachLocalPosition = new Vector3(0f, -0.05f, 0.3f);
    public Vector3 attachLocalEulerAngles = Vector3.zero;
    // Optional overrides for guns so they don't use the flashlight offsets
    public Vector3 gunAttachLocalPosition = new Vector3(0.25f, -0.2f, 0.45f);
    public Vector3 gunAttachLocalEulerAngles = new Vector3(0f, 5f, 0f);
    // Optional: if set, update the game's current quest when this item is picked up.
    // Leave empty to do nothing.
    public string questToSet = "";
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
        Debug.Log($"Pickup called on '{gameObject.name}' for player '{player.name}'");
        // Instantiate whatever prefab is assigned (flashlight, gun, etc.)
        if (itemPrefab != null)
        {
            // Instantiate and parent to player so it moves with the player.
            var instance = GameObject.Instantiate(itemPrefab);

            // Determine parent: prefer provided attachPoint, then a child named "FlashlightAnchor" on the player,
            // then the player's camera if present, otherwise player root.
            Transform parent = attachPoint;
            if (parent == null)
            {
                // Choose anchor based on item type: guns prefer a WeaponAnchor, flashlights a FlashlightAnchor
                string anchorName = itemType == ItemType.Gun ? "WeaponAnchor" : "FlashlightAnchor";
                var anchor = player.transform.Find(anchorName);
                if (anchor != null)
                    parent = anchor;
            }
            // Cache the player's camera (if any) so we can both choose a parent and align rotation below.
            Camera cam = player.GetComponentInChildren<Camera>();
            if (parent == null)
            {
                if (cam != null)
                    parent = cam.transform;
            }
            if (parent == null)
                parent = player.transform;

            instance.transform.SetParent(parent, false);
            // Position the item relative to the parent. Use type-specific offsets for guns vs other items.
            if (itemType == ItemType.Gun)
            {
                instance.transform.localPosition = gunAttachLocalPosition;
                instance.transform.localEulerAngles = gunAttachLocalEulerAngles;
            }
            else
            {
                instance.transform.localPosition = attachLocalPosition;
                instance.transform.localEulerAngles = attachLocalEulerAngles;
            }

            // Make sure the instance is active and give it a clear name so it's easy to find in the Hierarchy
            instance.SetActive(true);
            instance.name = itemPrefab.name + "_Instance";

            Debug.Log($"Instantiated '{instance.name}' and parented to '{parent.name}'. Active: {instance.activeSelf}");
            Debug.Log($"Instance world position: {instance.transform.position}");
            // Log full hierarchy path for easier searching
            string path = instance.name;
            Transform t = instance.transform.parent;
            while (t != null)
            {
                path = t.name + "/" + path;
                t = t.parent;
            }
            Debug.Log($"Instance hierarchy path: {path}");
            Debug.Log($"Parent '{parent.name}' child count: {parent.childCount}");
        }

        // If a quest string is provided, update the GameManager's quest
        if (!string.IsNullOrEmpty(questToSet) && GameManager.Instance != null)
        {
            GameManager.Instance.SetQuest(questToSet);
            Debug.Log($"Set GameManager quest to '{questToSet}'");
        }

        // Destroy pickup object in the world
        Destroy(gameObject);
    }
}
