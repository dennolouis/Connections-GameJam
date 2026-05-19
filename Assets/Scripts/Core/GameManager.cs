using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Quest UI")]
    public TextMeshProUGUI questText;

    private string currentQuest;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetQuest(string newQuest)
    {
        currentQuest = newQuest;
        UpdateQuestUI();
    }

    private void UpdateQuestUI()
    {
        if (questText != null)
        {
            questText.text = "Quest: " + currentQuest;
        }
    }
}