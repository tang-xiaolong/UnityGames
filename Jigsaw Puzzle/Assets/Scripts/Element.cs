using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Button;

public class Element : MonoBehaviour
{
    [SerializeField]
    private int id;
    private Vector2 pos;
    public Vector2 firstPos { set; private get; }
    public bool IsInRightPos(Vector2 position)
    {
        return position == firstPos;
    }
    
    public void SetId(int id)
    {
        this.id = id;
    }
    public int GetId()
    {
        return id;
    }
    public void SetPos(Vector2 pos)
    {

        this.pos = pos;
    }
    public Vector2 GetPos()
    {
        return pos;
    }
    public void OnClick()
    {
        Debug.Log(id);
    }
}