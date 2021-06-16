using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class Player : MonoBehaviour{
	private const string MaxSdkKey = "h9-Q1J1W-7_dN5-qKCUFVtSBgLdC2F9oIzRAyAeXLqaTqCTB8xvgmmAoTF87A483nh_0iNuXBZIAa9QOaF8Qn8";
    private const string InterstitialAdUnitId = "2ce7c9e92bb9298e";
	public static bool ShowAds = true;
	private int interstitialRetryAttempt;
	public GameObject GameOverMenuUI;
    public float jumpForce;
	public float StationaryForce;
    public Rigidbody2D rb;
    public SpriteRenderer sr;
    public string currentColor;
    public Color colorCyan;
	public Color colorYellow;
	public Color colorMagenta;
	public Color colorPink;
	public Text ScoreText;
	public Text ScoreTextString;
	public Text HighScore;
	public Text HighScoreTextString;
	public int Score;
	int timer = 0;
	private float deathTime;
	private double deathDuration = 2;
	public GameObject deathExplosion;
	public GameObject SlideUpExplosion;
	public ParticleSystem deathParticle;
	public ParticleSystem slideUpParticle;
    public static bool GameIsOver = false;

	void Start (){
		SetRandomColor();
		HighScore.text = ("High Score: ")+PlayerPrefs.GetInt("HighScore",0).ToString();
		HighScoreTextString.text = ("Best: ")+PlayerPrefs.GetInt("HighScore",0).ToString();
		GameIsOver = false;
		ShowAds = true;

        MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration =>
        {
            // AppLovin SDK is initialized, configure and start loading ads.
            Debug.Log("MAX SDK Initialized");
            InitializeInterstitialAds();
        };

        MaxSdk.SetSdkKey(MaxSdkKey);
        MaxSdk.InitializeSdk();

	}

    void Update(){

	if (deathTime != 0)
	{
		if (Time.time - deathTime > deathDuration)
			{
				if (ShowAds)
				{
					ShowInterstitial();
					ShowAds = false;
				}
				GameIsOver = true;
				GameOverMenuUI.SetActive(true);
			}
		return;
	}

	if (Input.GetMouseButtonDown(0))
	{

        if (GameIsOver)
            {
            	
            }
        else
            {
				rb.velocity = Vector2.up * StationaryForce;
				AddScore();
            }
	}

	if (Input.touchCount > 0)
    {

        if (GameIsOver)
            {
            	
            }
        else
            {
				timer += 1;
				rb.velocity = Vector2.up * StationaryForce;
				AddScore();
            }
    }

    else if (timer > 0)
    {
        if (GameIsOver)
            {
            	
            }
        else
        {
			timer = 0;
			rb.velocity = Vector2.up * jumpForce;
        }
    }
}

void OnTriggerEnter2D (Collider2D col){
	
		if (col.tag == "ColorChanger")
		{
			SetRandomColor();
			FindObjectOfType<AudioManager>().Play("Color Change");
			Destroy(col.gameObject);
			return;
		}

		if (col.tag == "Ground")
		{
			return;
		}

		if (col.tag != currentColor)
		{
			Instantiate (deathExplosion, transform.position, deathExplosion.transform.rotation);
			sr.enabled = false;
			Destroy(rb);
			deathTime = Time.time;
			FindObjectOfType<AudioManager>().Play("Game Over");
		}

		if (col.tag == currentColor)
		{
			FindObjectOfType<AudioManager>().Play("Slide Up");
			Instantiate (SlideUpExplosion, transform.position, SlideUpExplosion.transform.rotation);
		}

	}

void SetRandomColor (){
		var main = deathParticle.main;
		int index = UnityEngine.Random.Range(0, 4);
		switch (index)
		{
			case 0:
				currentColor = "Cyan";
				sr.color = colorCyan;
				main.startColor = colorCyan;
				slideUpParticle.startColor = colorCyan;
				break;
			case 1:
				currentColor = "Yellow";
				sr.color = colorYellow;
				main.startColor = colorYellow;
				slideUpParticle.startColor = colorYellow;
				break;
			case 2:
				currentColor = "Magenta";
				sr.color = colorMagenta;
				main.startColor = colorMagenta;
				slideUpParticle.startColor = colorMagenta;
				break;
			case 3:
				currentColor = "Pink";
				sr.color = colorPink;
				main.startColor = colorPink;
				slideUpParticle.startColor = colorPink;
				break;
		}
	}

void AddScore(){
	Score++;
	int score = Score;
	ScoreText.text = ("Score: ")+score.ToString();
	ScoreTextString.text = ("Score: ")+score.ToString();


    if (score > PlayerPrefs.GetInt("HighScore",0))
    {
        PlayerPrefs.SetInt("HighScore", score);
        HighScore.text = ("High Score: ")+score.ToString();
		HighScoreTextString.text = ("Best: ")+score.ToString();
    }
  }

public void PlayAgain()
	{
		SceneManager.LoadScene("MainScene");
	}

private void InitializeInterstitialAds()
{
    // Attach callbacks
    MaxSdkCallbacks.OnInterstitialLoadedEvent += OnInterstitialLoadedEvent;
    MaxSdkCallbacks.OnInterstitialLoadFailedEvent += OnInterstitialFailedEvent;
    MaxSdkCallbacks.OnInterstitialAdFailedToDisplayEvent += InterstitialFailedToDisplayEvent;
    MaxSdkCallbacks.OnInterstitialHiddenEvent += OnInterstitialDismissedEvent;

    // Load the first interstitial
    LoadInterstitial();
}

void LoadInterstitial()
{
    MaxSdk.LoadInterstitial(InterstitialAdUnitId);
}

    void ShowInterstitial()
    {
        if (MaxSdk.IsInterstitialReady(InterstitialAdUnitId))
        {
            MaxSdk.ShowInterstitial(InterstitialAdUnitId);
        }
        else
        {
            Debug.Log("Ad not ready");
        }
    }

    private void OnInterstitialLoadedEvent(string adUnitId)
    {
        // Interstitial ad is ready to be shown. MaxSdk.IsInterstitialReady(interstitialAdUnitId) will now return 'true'
        Debug.Log("Interstitial loaded");
        // Reset retry attempt
        interstitialRetryAttempt = 0;
    }

    private void OnInterstitialFailedEvent(string adUnitId, int errorCode)
    {
        // Interstitial ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
        interstitialRetryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, interstitialRetryAttempt));
        Debug.Log("Interstitial failed to load with error code: " + errorCode);
        Invoke("LoadInterstitial", (float) retryDelay);
    }

    private void InterstitialFailedToDisplayEvent(string adUnitId, int errorCode)
    {
        // Interstitial ad failed to display. We recommend loading the next ad
        Debug.Log("Interstitial failed to display with error code: " + errorCode);
        LoadInterstitial();
    }

    private void OnInterstitialDismissedEvent(string adUnitId)
    {
        // Interstitial ad is hidden. Pre-load the next ad
        Debug.Log("Interstitial dismissed");
        LoadInterstitial();
    }
}