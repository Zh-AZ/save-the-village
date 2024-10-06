using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HarvestTimer : MonoBehaviour
{
    [SerializeField] private float maxTime;

    private Image img;
    private float currentTime;
    public bool Tick;

    // Start is called before the first frame update
    void Start()
    {
        img = GetComponent<Image>();
        currentTime = maxTime;
    }

    // Update is called once per frame
    void Update()
    {
        Tick = false;
        currentTime -= Time.deltaTime;
        
        if (currentTime <= 0)
        {
            Tick = true;
            currentTime = maxTime;
        }

        img.fillAmount = currentTime / maxTime;
    }
}
