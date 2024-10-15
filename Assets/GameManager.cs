using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private HarvestTimer harvestTimer;
    [SerializeField] private EatTimer eatTimer;

    [SerializeField] private Sprite pauseSprite;
    [SerializeField] private Sprite resumeSprite;

    [SerializeField] private Image raidTimerImg;
    [SerializeField] private Image peasantTimerImg;
    [SerializeField] private Image warriorTimerImg;
    [SerializeField] private Image harvestTimerImg;
    [SerializeField] private Image eatTimerImg;
    [SerializeField] private Image ResumeButtonImg;

    [SerializeField] private Button peasantButton;
    [SerializeField] private Button warriorButton;
    [SerializeField] private Button peasantToWarrior;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button changeSoundPlayButton;
    //[SerializeField] private Button changeSoundButton;

    [SerializeField] private Text resourcePeasantText;
    [SerializeField] private Text resourceWarriorText;
    [SerializeField] private Text resourceWheatText;
    [SerializeField] private Text enemyCount;

    [SerializeField] private Text survivedCountText;
    private int survivedCount;
    [SerializeField] private Text wheatCreditText;
    private int wheatCredit;
    [SerializeField] private GameObject warriorLeaveText;

    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject rulesScreen;

    private AudioSource backgroundMusic;
    [SerializeField] private AudioClip clip_1;
    [SerializeField] private AudioClip clip_2;
    [SerializeField] private AudioClip buttonSound;
    [SerializeField] private AudioClip buttonErrorSound;
    private AudioSource audioButtons;

    [SerializeField] private AudioClip harvestSound;
    [SerializeField] private AudioClip raidSound;
    [SerializeField] private AudioClip eatSound;
    [SerializeField] private AudioClip peasantCreateSound;
    [SerializeField] private AudioClip warriorCreateSound;

    [SerializeField] private int peasantCount;
    [SerializeField] private int warriorCount;
    [SerializeField] private int wheatCount;
    [SerializeField] private int wheatPerPeasant;
    [SerializeField] private int wheatToWarrior;
    [SerializeField] private int peasantCost;
    [SerializeField] private int warriorCost;
    [SerializeField] private float peasantCreateTime;
    [SerializeField] private float warriorCreateTime;
    [SerializeField] private float raidMaxTime;
    [SerializeField] private int raidIncrease;
    [SerializeField] private int nextRaid;

    private float peasantTime = -2;
    private float warriorTime = -2;
    private float raidTime;
    private float twoSecond;
    private bool isError;
    private Random random = new Random();
    
    // Start is called before the first frame update
    void Start()
    {
        UpdateText();
        raidTime = raidMaxTime;
        backgroundMusic = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        twoSecond += Time.deltaTime;
        if (twoSecond >= 2)
        {
            twoSecond = 0;
            resourceWheatText.color = ColorUtility.TryParseHtmlString("#E9B165", out Color colorWheat) ? colorWheat : Color.red;
            resourcePeasantText.color = ColorUtility.TryParseHtmlString("#98702E", out Color colorPeasant) ? colorPeasant : Color.red;

            if (wheatCredit != 0 && wheatCount != 0)
            {
                resourceWheatText.text = $"Пшеницы: {--wheatCount}";
                wheatCreditText.text = $"Воины хотят есть :\n{--wheatCredit} пшениц";
            }

            if (wheatCredit >= 10)
            {
                wheatCredit -= 10;
                warriorCount -= 1;
                warriorLeaveText.SetActive(true);
            }
            else
            {
                warriorLeaveText.SetActive(false);
            }
        }

        raidTime -= Time.deltaTime;
        raidTimerImg.fillAmount = raidTime / raidMaxTime;

        if (raidTime <= 0)
        {
            for (int i = 0; i < nextRaid; i++)
            {
                if (random.Next(2) == 1)
                {
                    --warriorCount;
                }
            }
            
            raidTime = raidMaxTime;
            //warriorCount -= nextRaid;
            nextRaid += raidIncrease;            
            ++survivedCount;

            if (survivedCount >= 10)
            {
                survivedCountText.text = "Последний рейд!";
            }
            else
            {
                survivedCountText.text = $"Рейд: {survivedCount}";
            }
            //GetButtonsSound(raidTimerImg.GetComponent<AudioSource>());

            audioButtons = raidTimerImg.GetComponent<AudioSource>();
            audioButtons.clip = raidSound;
            audioButtons.Play();
        }

        if (harvestTimer.Tick)
        {
            wheatCount += peasantCount * wheatPerPeasant;
            //GetButtonsSound(harvestTimerImg.GetComponent<AudioSource>());
            audioButtons = harvestTimerImg.GetComponent<AudioSource>();
            audioButtons.clip = harvestSound;
            audioButtons.Play();
        }

        if (eatTimer.Tick)
        {
            if ((wheatCount - (warriorCount * wheatToWarrior)) <= 0)
            {
                wheatCredit += Mathf.Abs(wheatCount - (warriorCount * wheatToWarrior));
                wheatCreditText.text = $"Воины хотят есть :\n{wheatCredit} пшениц";
                //wheatCount = 0;
            }
            else
            {
                wheatCount -= warriorCount * wheatToWarrior;
                //GetButtonsSound(eatTimerImg.GetComponent<AudioSource>());
                audioButtons = eatTimerImg.GetComponent<AudioSource>();
                audioButtons.clip = eatSound;
                audioButtons.Play();
            }                   
        }

        if (peasantTime > 0)
        {
            peasantTime -= Time.deltaTime;
            peasantTimerImg.fillAmount = peasantTime / peasantCreateTime;
        }
        else if (peasantTime > -1)
        {
            peasantTimerImg.fillAmount = 1;
            peasantButton.interactable = true;
            ++peasantCount;
            peasantTime = -2;
            //GetButtonsSound(peasantTimerImg.GetComponent<AudioSource>());
            audioButtons = peasantTimerImg.GetComponent<AudioSource>();
            audioButtons.clip = peasantCreateSound;
            audioButtons.Play();
        }

        if (warriorTime > 0)
        {
            warriorTime -= Time.deltaTime;
            warriorTimerImg.fillAmount = warriorTime / warriorCreateTime;
        }
        else if (warriorTime > -1)
        {
            warriorTimerImg.fillAmount = 1;
            warriorButton.interactable = true;
            ++warriorCount;
            warriorTime = -2;
            //GetButtonsSound(warriorTimerImg.GetComponent<AudioSource>());
            audioButtons = warriorTimerImg.GetComponent<AudioSource>();
            audioButtons.clip = warriorCreateSound;
            audioButtons.Play();
        }

        UpdateText();     

        if (survivedCount > 10)
        {
            Time.timeScale = 0;
            gameOverScreen.SetActive(true);
        }
        
        if (warriorCount < 0)
        {
            Time.timeScale = 0;
            resourceWarriorText.text = $"Оставшиеся варвары: {Mathf.Abs(warriorCount)}";
            gameOverScreen.SetActive(true);
        }
    }

    private void GetButtonsSound(AudioSource source, bool isError = false)
    {
        audioButtons = source;
        if (isError)
        {
            audioButtons.clip = buttonErrorSound;
        }
        else
        {
            audioButtons.clip = buttonSound;
        }
        audioButtons.Play();
    }

    public void CreatePeasant()
    {
        if (wheatCount - peasantCost <= 0)
        {
            resourceWheatText.color = Color.red;
            isError = true;
        }
        else
        {
            wheatCount -= peasantCost;
            peasantTime = peasantCreateTime;
            peasantButton.interactable = false;
            isError = false;
        }

        GetButtonsSound(peasantButton.GetComponent<AudioSource>(), isError);
    }

    public void CreateWarrior()
    {
        //GetButtonsSound(warriorButton.GetComponent<AudioSource>());

        if (wheatCount - warriorCost <= 0)
        {
            resourceWheatText.color = Color.red;
            isError= true;
            //wheatCount += warriorCost;
        }
        else
        {
            wheatCount -= warriorCost;
            warriorTime = warriorCreateTime;
            warriorButton.interactable = false;
            isError = false;
        }     

        GetButtonsSound(warriorButton.GetComponent<AudioSource>(), isError);
    }

    public void ConvertToWarrior()
    {
        //GetButtonsSound(peasantToWarrior.GetComponent<AudioSource>());

        if (peasantCount - 10 >= 0)
        {
            peasantCount -= 10;
            warriorCount += 1;
            isError = false;
        }
        else
        {
            resourcePeasantText.color = Color.red;
            isError= true;
        }

        GetButtonsSound (peasantToWarrior.GetComponent<AudioSource>(), isError);
    }

    public void Pause()
    {
        GetButtonsSound(pauseButton.GetComponent<AudioSource>());
        

        if (Time.timeScale == 0)
        {
            rulesScreen.SetActive(false);
            backgroundMusic.clip = clip_2;
            backgroundMusic.Play();
            ResumeButtonImg.sprite = pauseSprite;
            Time.timeScale = 1;
        }
        else
        {
            rulesScreen.SetActive(true);
            backgroundMusic.clip = clip_1;
            backgroundMusic.Play();
            ResumeButtonImg.sprite = resumeSprite;
            Time.timeScale = 0;
        }
    }

    public void ChangeSoundPlay()
    {
        GetButtonsSound(changeSoundPlayButton.GetComponent<AudioSource>());

        if (backgroundMusic.isPlaying)
        {
            backgroundMusic.Pause();
            changeSoundPlayButton.image.color = Color.gray;
        }
        else
        {
            backgroundMusic.Play();
            changeSoundPlayButton.image.color = Color.white;
        }
    }

    // No use
    //public void ChangeSound()
    //{
    //    //GetButtonsSound(changeSoundButton.GetComponent<AudioSource>());
    //    changeSoundButton.GetComponent<AudioSource>();
    //    backgroundMusic.clip = clip_1;
    //}

    private void UpdateText()
    {
        //resourcePeasantText.text = $"<color=#98702E>Крестиан: {peasantCount}</color>";
        //resourceWarriorText.text = $"<color=#434343>Воинов: {warriorCount}</color>";
        //resourceWheatText.text = $"<color=#E9B165>Пшеницы :{wheatCount} </color>";

        resourcePeasantText.text = $"Крестиан: {peasantCount}";
        resourceWarriorText.text = $"Воинов: {warriorCount}";
        resourceWheatText.text = $"Пшеницы: {wheatCount}";

        enemyCount.text = $"Количество врагов в следующем набеге: <color=\"red\">{nextRaid}</color>";

        if (wheatCredit > 0)
        {
            wheatCreditText.text = $"Воины хотят есть :\n{wheatCredit} пшениц";
        }
        else
        {
            wheatCreditText.text = $"";
        }
    }
}
