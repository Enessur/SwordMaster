using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindArmor : MonoBehaviour
{
     
    void Start()
    {
        GameObject[] house = GameObject.FindGameObjectsWithTag("Empty");
        int randomIndex = Random.Range(0, house.Length);
        house[randomIndex].tag = "Armor";
    }

    
}
