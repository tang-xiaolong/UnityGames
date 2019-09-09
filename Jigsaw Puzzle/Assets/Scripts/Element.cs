using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Element : MonoBehaviour
{
    private Sprite sprite;
    private int id;
    private int pos;
    void Start()
    {
        
    }

    public void SetSprite(Sprite sprite)
    {
        this.sprite = sprite;
    }
    public void SetId(int id)
    {
        this.id = id;
    }
    public void SetPos(int pos)
    {
        this.pos = pos;
    }
}
