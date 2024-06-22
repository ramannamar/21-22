using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusTimer : MonoBehaviour
{
    public BonusCheck bonusCheck;
    public GameObject bonusTimer;
    public Timer timerComponent;

    private void Start()
    {
        bonusCheck.BonusActiveChanged += OnBonusActiveChanged;
    }

    private void OnBonusActiveChanged()
    {
        if (bonusCheck.isActive)
        {
            StartCoroutine(TimerActive());
        }
        else
        {
            timerComponent.time = 10f;
            timerComponent.Max = 10f;
            bonusTimer.SetActive(false);
            StopCoroutine(TimerActive());
        }
    }

    private IEnumerator TimerActive()
    {
        bonusTimer.SetActive(true);
        yield return new WaitForSeconds(10);
        bonusTimer.SetActive(false);
        StopCoroutine(TimerActive());
    }
}