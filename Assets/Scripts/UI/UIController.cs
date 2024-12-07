using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class UIController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI targetsKilledText;
    [SerializeField] private TextMeshProUGUI projectileTypeText;

    private int score;
    private int targetsKilled;
    private string currentProjectile = "CannonBall";

    public int Score => score;
    public int TargetsKilled => targetsKilled;

    private void Start()
    {
        UpdateScore(0);
        UpdateTargetsKilled(0);
        UpdateProjectileType("CannonBall");
    }

    private void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            ResetLevel();
        }
    }

    private void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void UpdateScore(int newScore)
    {
        score = newScore;
        scoreText.text = $"Score: {score}";
    }

    public void UpdateTargetsKilled(int newCount)
    {
        targetsKilled = newCount;
        targetsKilledText.text = $"Targets Destroyed: {targetsKilled}";
    }

    public void UpdateProjectileType(string projectileName)
    {
        currentProjectile = projectileName;
        projectileTypeText.text = $"Projectile: {currentProjectile}";
    }
}