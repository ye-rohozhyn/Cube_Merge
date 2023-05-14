using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int fpsLock = 60;
    [SerializeField] private VSync vSync;

    [Header("Game panels")]
    [SerializeField] private GameObject losePanel;

    [Header("Other")]
    [SerializeField] private GameObject startText;

    public bool ActivePanel { get; set; }

    private void Start()
    {
        Application.targetFrameRate = fpsLock;
        QualitySettings.vSyncCount = (int) vSync;
        Time.timeScale = 1f;
    }

    public void ShowLosePanel()
    {
        Time.timeScale = 0f;

        losePanel.SetActive(true);
    }

    public void ShowPanel(GameObject panelRoot)
    {
        if (ActivePanel) return;

        panelRoot.SetActive(true);
        ActivePanel = true;
    }

    public void ClosePanel(GameObject panelRoot)
    {
        panelRoot.SetActive(false);
        ActivePanel = false;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void HideStartText()
    {
        startText.SetActive(false);
    }
}

public enum VSync
{
    Off = 0,
    On = 1
}