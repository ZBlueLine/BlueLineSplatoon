using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public PaintGun weapon;

    private void Awake()
    {
    }

    void Start()
    {
        
    }

    void Update()
    {
        weapon.Paint();
    }
}
