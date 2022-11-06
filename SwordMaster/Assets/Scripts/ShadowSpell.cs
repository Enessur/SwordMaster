using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowSpell : MonoBehaviour
{
    [SerializeField] private GameObject shadowRb;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = transform.position + new Vector3(0.3f, 1.7f, 0f);
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