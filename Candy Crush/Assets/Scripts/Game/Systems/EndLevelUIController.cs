using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndLevelUIController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject failPanel;

    [Header("Victory UI")]
    [SerializeField] private Image vStar1;
    [SerializeField] private Image vStar2;
    [SerializeField] private Image vStar3;
    [SerializeField] private TextMeshProUGUI vScoreText;
    [SerializeField] private TextMeshProUGUI vThresholdsText;
    [SerializeField] private Button vContinueButton;

    [Header("Fail UI")]
    [SerializeField] private Image fStar1;
    [SerializeField] private Image fStar2;
    [SerializeField] private Image fStar3;
    [SerializeField] private TextMeshProUGUI fScoreText;
    [SerializeField] private TextMeshProUGUI fThresholdsText;
    [SerializeField] private Button fRetryButton;
    [SerializeField] private Button fMenuButton;

    // optional: dim grey for inactive stars
    [SerializeField] private Color inactiveStarColor = new Color(1f, 1f, 1f, 0.25f);

    public void ShowVictory(int score, int s1, int s2, int s3, System.Action onContinue)
    {
        gameObject.SetActive(true);
        if (failPanel) failPanel.SetActive(false);
        if (victoryPanel) victoryPanel.SetActive(true);

        SetupStars(score, s1, s2, s3, vStar1, vStar2, vStar3);
        if (vScoreText) vScoreText.text = "Score: " + score;
        if (vThresholdsText) vThresholdsText.text = $"1: {s1}      2: {s2}      3: {s3}";

        if (vContinueButton)
        {
            vContinueButton.onClick.RemoveAllListeners();
            vContinueButton.onClick.AddListener(() => onContinue?.Invoke());
        }
    }

    public void ShowFail(int score, int s1, int s2, int s3, System.Action onRetry, System.Action onMenu)
    {
        gameObject.SetActive(true);
        if (victoryPanel) victoryPanel.SetActive(false);
        if (failPanel) failPanel.SetActive(true);

        SetupStars(score, s1, s2, s3, fStar1, fStar2, fStar3);
        if (fScoreText) fScoreText.text = "Score: " + score;
        if (fThresholdsText) fThresholdsText.text = $"1: {s1}      2: {s2}      3: {s3}";

        if (fRetryButton)
        {
            fRetryButton.onClick.RemoveAllListeners();
            fRetryButton.onClick.AddListener(() => onRetry?.Invoke());
        }
        if (fMenuButton)
        {
            fMenuButton.onClick.RemoveAllListeners();
            fMenuButton.onClick.AddListener(() => onMenu?.Invoke());
        }
    }

    private void SetupStars(int score, int s1, int s2, int s3, Image star1, Image star2, Image star3)
    {
        int stars = (score >= s3) ? 3 : (score >= s2) ? 2 : (score >= s1) ? 1 : 0;

        SetStar(star1, stars >= 1);
        SetStar(star2, stars >= 2);
        SetStar(star3, stars >= 3);
    }

    private void SetStar(Image img, bool on)
    {
        if (!img) return;
        img.color = on ? Color.white : inactiveStarColor;
        img.enabled = true; // ensure visible
    }
}

