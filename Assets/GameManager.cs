using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private HarvestTimer harvestTimer;
    [SerializeField] private EatTimer eatTimer;

    [SerializeField] private Image raidTimerImg;
    [SerializeField] private Image peasantTimerImg;
    [SerializeField] private Image warriorTimerImg;
    
    [SerializeField] private Button peasantButton;
    [SerializeField] private Button warriorButton;

    [SerializeField] private Text resourcePeasantText;
    [SerializeField] private Text resourceWarriorText;
    [SerializeField] private Text resourceWheatText;

    [SerializeField] private GameObject gameOverScreen;

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

    // Start is called before the first frame update
    void Start()
    {
        UpdateText();
        raidTime = raidMaxTime;
    }

    // Update is called once per frame
    void Update()
    {
        raidTime -= Time.deltaTime;
        raidTimerImg.fillAmount = raidTime / raidMaxTime;

        if (raidTime <= 0)
        {
            raidTime = raidMaxTime;
            warriorCount -= nextRaid;
            nextRaid += raidIncrease;
        }

        if (harvestTimer.Tick)
        {
            wheatCount += peasantCount * wheatPerPeasant;
        }

        if (eatTimer.Tick)
        {
            wheatCount -= warriorCount * wheatToWarrior;
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
    
        if (warriorCount < 0)
        {
            Time.timeScale = 0;
            gameOverScreen.SetActive(true);
        }
    }

    public void CreatePeasant()
    {
        wheatCount -= peasantCost;
        peasantTime = peasantCreateTime;
        peasantButton.interactable = false;
    }

    public void CreateWarrior()
    {
        wheatCount -= warriorCost;
        warriorTime = warriorCreateTime;
        warriorButton.interactable = false;
    }

    private void UpdateText()
    {
        resourcePeasantText.text = $"Крестиан: {peasantCount}";
        resourceWarriorText.text = $"Воинов: {warriorCount}";
        resourceWheatText.text = $"Пшеницы :{wheatCount}";
    }
}
