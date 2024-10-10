using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private HarvestTimer harvestTimer;
    [SerializeField] private EatTimer eatTimer;

    [SerializeField] private Image raidTimerImg;
    [SerializeField] private Image peasantTimerImg;
    [SerializeField] private Image warriorTimerImg;
    
    [SerializeField] private Button peasantButton;
    [SerializeField] private Button warriorButton;
    [SerializeField] private Button peasantToWarrior;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button changeSoundPlayButton;
    [SerializeField] private Button changeSoundButton;

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

    private new AudioSource audio;
    [SerializeField] private AudioClip clip_1;
    [SerializeField] private AudioClip clip_2;
    [SerializeField] private AudioClip buttonSound;
    private AudioSource audioButtons;

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
    private Random random = new Random();
    

    // Start is called before the first frame update
    void Start()
    {
        UpdateText();
        raidTime = raidMaxTime;
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        twoSecond += Time.deltaTime;
        if (twoSecond >= 2)
        {
            twoSecond = 0;
            resourceWheatText.color = Color.black;
            resourcePeasantText.color = Color.black;

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
                survivedCountText.text = "Последний цикл";
            }
            else
            {
                survivedCountText.text = $"Циклов: {survivedCount}";
            }
        }

        if (harvestTimer.Tick)
        {
            wheatCount += peasantCount * wheatPerPeasant;
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

    private void GetButtonsSound(AudioSource source)
    {
        //audioButtons = peasantButton.GetComponent<AudioSource>();
        audioButtons = source;
        audioButtons.clip = buttonSound;
        audioButtons.Play();
    }

    public void CreatePeasant()
    {
        //wheatCount -= peasantCost;
        GetButtonsSound(peasantButton.GetComponent<AudioSource>());

        if (wheatCount - peasantCost <= 0)
        {
            resourceWheatText.color = Color.red;
        }
        else
        {
            wheatCount -= peasantCost;
            peasantTime = peasantCreateTime;
            peasantButton.interactable = false;
        }
    }

    public void CreateWarrior()
    {
        GetButtonsSound(warriorButton.GetComponent<AudioSource>());

        if (wheatCount - warriorCost <= 0)
        {
            resourceWheatText.color = Color.red;
            //wheatCount += warriorCost;
        }
        else
        {
            wheatCount -= warriorCost;
            warriorTime = warriorCreateTime;
            warriorButton.interactable = false;
        }     
    }

    public void ConvertToWarrior()
    {
        GetButtonsSound(peasantToWarrior.GetComponent<AudioSource>());

        if (peasantCount - 10 >= 0)
        {
            peasantCount -= 10;
            warriorCount += 1;
        }
        else
        {
            resourcePeasantText.color = Color.red;
        }
    }

    public void Pause()
    {
        GetButtonsSound(pauseButton.GetComponent<AudioSource>());

        if (Time.timeScale == 0)
        {
            rulesScreen.SetActive(false);
            audio.clip = clip_2;
            audio.Play();
            Time.timeScale = 1;
        }
        else
        {
            rulesScreen.SetActive(true);
            audio.clip = clip_1;
            audio.Play();
            Time.timeScale = 0;
        }
    }

    public void ChangeSoundPlay()
    {
        GetButtonsSound(changeSoundPlayButton.GetComponent<AudioSource>());

        if (audio.isPlaying)
        {
            audio.Pause();
        }
        else
        {
            audio.Play();
        }
    }

    public void ChangeSound()
    {
        GetButtonsSound(changeSoundButton.GetComponent<AudioSource>());
        audio.clip = clip_1;
    }

    private void UpdateText()
    {
        resourcePeasantText.text = $"Крестиан: {peasantCount}";
        resourceWarriorText.text = $"Воинов: {warriorCount}";
        resourceWheatText.text = $"Пшеницы :{wheatCount}";
        enemyCount.text = $"Количество врагов в следующем набеге {nextRaid}";

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
