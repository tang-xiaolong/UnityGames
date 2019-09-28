using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
	public enum MyCameraType
	{
		Circle,
		Line
	}

	public Transform center;
	private float switchTime = 3;
	private float rotateSpeed = 40f;
	private float high = 50f;
	private Vector3 pos;
	public MyCameraType type = MyCameraType.Circle;
    private Control control;
    private int count;

    void Start()
    {
        control = FindObjectOfType<Control>();
        count = control.cubeCount;
        //transform.position = 
        if (type == MyCameraType.Circle)
		{
			StartCoroutine(Rotate());
		}
		else
		{
			LineInit();
		}
		pos = center.position;
		//pos.y = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
		if (type == MyCameraType.Circle)
		{
			transform.RotateAround(pos, Vector3.up, rotateSpeed * Time.deltaTime);
            transform.LookAt(center);
		}
		//transform.Rotate(pos, rotateSpeed * Time.deltaTime,Space.Self);
		//transform.Rotate((transform.position - center.position).normalized, rotateSpeed * Time.deltaTime);
		//transform.Rotate(center.position,rotateSpeed*Time.deltaTime);
    }
	public void LineInit()
	{
		transform.position = new Vector3(0, 0, -count);
		transform.LookAt(center);
	}

	IEnumerator Rotate()
	{
        //设置摄像机的位置
        
		//transform.position = new Vector3(62f, 42f, -5f)+new Vector3(0,0,0);
		transform.position = new Vector3(count, count, -5f);
		transform.rotation = new Quaternion(28, -81, 0, 0);
		while(true)
		{
			yield return new WaitForSeconds(switchTime);
			//改变摄像机的旋转方向
			if (Random.Range(0, 2) == 1)
			{
				rotateSpeed *= -1;
				switchTime = Random.Range(3, 7);
			}
		}
	}
}
