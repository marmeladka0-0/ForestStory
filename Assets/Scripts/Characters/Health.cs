using UnityEngine;
using Inventory.Model;
using System;

public class Health: MonoBehaviour 
{
    [SerializeField]
    public int MaxHeath = 5;
    [SerializeField]
    public PlayerHealthValueSO currHealth;
    [SerializeField]
    private bool isFainted = false;
    
    public void Start() 
    {    
        if (currHealth != null)
        {
            // currentHealth.HealthValue = maxHealth;
            
            // Test restoring health
            currHealth.HealthValue = 3;            
        }
    }
    
    public int AddHealth(int val) 
    {
        if (currHealth.HealthValue < MaxHeath) 
        {
            int increaseVal = Mathf.Min(currHealth.HealthValue + val, MaxHeath) - currHealth.HealthValue;
            currHealth.HealthValue += increaseVal;
            
            if (currHealth.HealthValue > 0 && isFainted) 
            {
                isFainted = false;
                Debug.Log("Character woke up!");
            }
            
            Debug.Log($"Characters health increased by {increaseVal} points!");
            return increaseVal;
        }
        return 0;
    }
    
    public int DecreaseHealth(int val) 
    {
        if (currHealth.HealthValue > 0) 
        {
            int decreaseVal = currHealth.HealthValue - Mathf.Max(currHealth.HealthValue - val, 0);
            currHealth.HealthValue -= decreaseVal; 
            
            if (currHealth.HealthValue == 0 && !isFainted) 
            {
                isFainted = true;
                Debug.Log("Character fainted! Medicine or immediate help is needed to wake him up!");
            }
            
            Debug.Log($"Characters health decreased by {decreaseVal} points!");
            return decreaseVal;
        }
        return 0;
    }   
}