using UnityEngine;

public class QuestTrigger : MonoBehaviour
{
    public string questMessage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.SetQuest(questMessage);
        }
    }
}