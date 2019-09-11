using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Button;

public class Element : MonoBehaviour
{
    [SerializeField]
    private int id;
    private int pos;
    void Start()
    {
        
    }
    
    public void SetId(int id)
    {
        this.id = id;
    }
    public void SetPos(int pos)
    {
        this.pos = pos;
    }
    public void OnClick()
    {
        Debug.Log(id);
    }
}
