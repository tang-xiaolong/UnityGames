using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour
{
	private AudioSource myAudio;
	public int cubeCount = 64;
	private float[] info;
	public GameObject cube;
	public CameraControl.MyCameraType type = CameraControl.MyCameraType.Circle;
    private CameraControl cameraControl;
	
	private List<Transform> cubes = new List<Transform>();
	private List<Material> materials = new List<Material>();
	private float radius = 30;
	private void Init()
	{
        cameraControl = FindObjectOfType<CameraControl>();
        cameraControl.type = type;
		myAudio = GetComponent<AudioSource>();
		info = new float[cubeCount];
	}
	/// <summary>
	/// 圆形
	/// </summary>
	private void Circle()
	{
		Vector3 pos = transform.position;
		float angle = 0f;
		float dat = 360 / cubeCount;
		for (angle = 0; angle < 360f; angle += dat)
		{
			float x = pos.x + radius * cubeCount / 64 * Mathf.Cos(angle * Mathf.PI / 180f);
			float z = pos.z + radius * cubeCount / 64 * Mathf.Sin(angle * Mathf.PI / 180f);
			GameObject go = Instantiate(cube);
			go.SetActive(true);
			cubes.Add(go.transform);
			materials.Add(go.GetComponent<Renderer>().material);
			go.transform.position = new Vector3(x, 0, z);
			go.transform.parent = transform;
		}
	}
	/// <summary>
	/// 直线型
	/// </summary>
	private void Line()
	{
		Vector3 pos = transform.position;
		for (int i = 0; i < cubeCount; i++)
		{
			GameObject go = Instantiate(cube);
			go.SetActive(true);
			cubes.Add(go.transform);
			materials.Add(go.GetComponent<Renderer>().material);
			go.transform.position = new Vector3(i - cubeCount + pos.x + i, -40, 30);
			go.transform.parent = transform;
		}
	}
	private void Awake()
	{
		Init();
		//Circle();
		if (type == CameraControl.MyCameraType.Circle)
		{
            Circle();
		}
		else
			Line();
	}
	private void Update()
	{
		Cube();
	}
	public void Cube()
	{
		myAudio.GetSpectrumData(info, 0, FFTWindow.Rectangular);
		for (int i = 0; i < cubeCount; i++)
		{
			//设置立方体的规模，设置立方体的高度scale
			//Debug.Log(info[i]);
			float myY = info[i] * (i + 1) * (i + 1);
			myY = Mathf.Clamp(myY,1,20);
			Debug.Log(myY);
			SetColor(i,myY);
			cubes[i].localScale = new Vector3(1, myY + 1, 1);
			cubes[i].position = new Vector3(cubes[i].position.x, myY / 2 + 0.5f, cubes[i].position.z);
		}
	}
	public void SetColor(int index,float myY)
	{
		if (myY > 9)
		{
			materials[index].SetColor("_EmissionColor", new Color(0 / 255f, 11 / 255f, 233 / 255f));
		}
		else if (myY > 7)
		{
			materials[index].SetColor("_EmissionColor", new Color(0 / 255f, 233 / 255f, 4 / 255f));
		}
		else if (myY > 5)
		{
			materials[index].SetColor("_EmissionColor", new Color(233 / 255f, 93 / 255f, 0 / 255f));
		}
		else if (myY > 3)
		{
			materials[index].SetColor("_EmissionColor", new Color(231 / 255f, 0 / 255f, 5 / 255f));
		}
		else
		{
			materials[index].SetColor("_EmissionColor", new Color(233 / 255f, 233 / 255f, 233 / 255f));
		}
	}
}
