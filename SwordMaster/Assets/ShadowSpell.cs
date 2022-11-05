using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowSpell : MonoBehaviour
{
    
    [SerializeField] private GameObject shadowRb;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }
    
    
}
