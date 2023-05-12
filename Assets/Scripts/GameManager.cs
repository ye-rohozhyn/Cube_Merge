using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float timeStopDuration = 1f;
    [SerializeField] private float timeStopSmoothness = 0.1f;

    private void Start()
    {
        Time.timeScale = 1;
    }

    public void ShowLosePanel()
    {
        Time.timeScale = 0f;

        print("Lose");
    }

    public void Restart()
    {
        //restart scene
    }
}