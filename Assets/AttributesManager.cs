using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AttributesManager : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthBar;
    public int damage;
    private bool canDealDamage = true;
    public float damageCooldown = 1f;

    public void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            OnDie();
        }
    }

    private void OnDie()
    {
        if (CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }

        else
        {
            SceneManager.LoadScene(0);
        }
    }
    

    public void DealDamage(GameObject target)
    {
        if (canDealDamage)
        {
            var atm = target.GetComponent<AttributesManager>();
            if (atm != null)
            {
                atm.TakeDamage(damage);
                StartCoroutine(DamageDelayCooldown());
            }
        }
    }
    
    private IEnumerator DamageDelayCooldown()
    {
        canDealDamage = false;
        yield return new WaitForSeconds(damageCooldown);
        canDealDamage = true;
    }
}
