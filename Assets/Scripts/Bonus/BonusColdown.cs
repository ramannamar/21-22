using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BonusColdown : MonoBehaviour
{
    public GameObject bonus;
    public GameObject player;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(Test());         
        }

        IEnumerator Test()
        {
            bonus.SetActive(false);
           
            yield return new WaitForSeconds(10);

            bonus.SetActive(true);
        }
    }
    
}




