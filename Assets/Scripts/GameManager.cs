using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int fpsLock = 60;
    [SerializeField] private Status vSync;
    [SerializeField] private Status sounds;

    [Header("Sounds")]
    [SerializeField] private AudioSource audioSource;

    [Header("Score")]
    [SerializeField] private Animator scoreAnimator;
    [SerializeField] private TMP_Text bestScoreText;
    [SerializeField] private TMP_Text scoreText;

    [Header("Other")]
    [SerializeField] private Toggle soundToggle;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject startText;

    public bool ActivePanel { get; set; }

    private int _bestScore = 0;
    private int _gameScore = 0;
    private AudioSource[] _soundSources;

    private void Start()
    {
        Time.timeScale = 1f;

        _soundSources = FindObjectsOfType<AudioSource>();

        ApplySettings();
    }

    private void ApplySettings()
    {
        Application.targetFrameRate = fpsLock;
        QualitySettings.vSyncCount = (int)vSync;

        sounds = (Status)PlayerPrefs.GetInt("Sounds", 1);
        soundToggle.isOn = sounds == Status.On;
        foreach (AudioSource sound in _soundSources)
        {
            sound.volume = (int)sounds;
        }

        _bestScore = PlayerPrefs.GetInt("Score", 0);
        bestScoreText.text = _bestScore.ToString();
    }

    public void SoundToggleChanged()
    {
        sounds = (Status)(soundToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("Sounds", (int)sounds);
        foreach (AudioSource sound in _soundSources)
        {
            sound.volume = (int)sounds;
        }
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void ShowLosePanel()
    {
        Time.timeScale = 0f;

        if (_bestScore < _gameScore)
        {
            SaveScore();
        }

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

    public void StartPlay()
    {
        startText.SetActive(false);
        scoreAnimator.SetTrigger("StartPlay");
    }

    public void AddScore(int score)
    {
        _gameScore += score;
        scoreText.text = _gameScore.ToString();
    }

    public void SaveScore()
    {
        PlayerPrefs.SetInt("Score", _gameScore);
        PlayerPrefs.Save();
    }
}

public enum Status
{
    Off = 0,
    On = 1
}