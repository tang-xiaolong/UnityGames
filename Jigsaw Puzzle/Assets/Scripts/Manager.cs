using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{

    //1、读取图片
    //2、输入切割大小
    //3、得到图片碎片，添加脚本，并删除最后一个的显示
    //4、打乱图片
    private Texture imageSource;
    private List<Element> pictures = new List<Element>();
    private int size = 2;
    public Material material;
    public GameObject go;
    private Element emptyElement;
    [SerializeField]
    private int currentRightPosCount = 0;
    private bool canMove = true;
    

    public void Start()
    {
        StartGame();
    }
    public void StartGame()
    {
        currentRightPosCount = 0;
        OpenFile();
        SliceImage(size);
        RandomImage();
    }
    public bool CheckWin()
    {
        return currentRightPosCount == size * size;
    }
    //[MenuItem("Tools/OPen File", false, 1)]
    /// <summary>
    /// 打开文件
    /// </summary>
    public void OpenFile()
    {
        string inputFile = EditorUtility.OpenFilePanel("Choose Image", Application.dataPath, "jpg");
        if (!string.IsNullOrEmpty(inputFile))
        {
            Debug.Log(inputFile);
            string fileName;
            CopyToResources(inputFile,out fileName);
            AssetDatabase.Refresh();
            ReadImage(fileName);
        }
        GC.Collect();
    }
    /// <summary>
    /// 复制资源到Resources文件夹
    /// </summary>
    /// <param name="path"></param>
    /// <param name="fileName"></param>
    public static void CopyToResources(string path,out string fileName)
    {
        string[] res = path.Split('/');
        fileName = res[res.Length - 1];
        string resPath = Application.dataPath + "/Resources/"+fileName;
        if(!path.Equals(resPath))
            File.Copy(path, resPath, true);
    }
    /// <summary>
    /// 读取图片
    /// </summary>
    /// <param name="path"></param>
    public void ReadImage(string path)
    {
        Debug.Log(path);
        string fileName = path.Split('.')[0];
        imageSource = Resources.Load<Texture>(fileName);
        
    }
    public void GetCount()
    {

    }
    public void SliceImage(int count)
    {
        //去较小的一个值，然后切割成三份
        // n * n
        int minSize = Mathf.Min(imageSource.width, imageSource.height);
        float delta = 1.0f / size;
        float offset = 0.05f;
        //取较小的一个，并均分为n块
        for (int j = 0; j < size; j++)
        {
            for (int i = 0; i < size; i++)
            {
                GameObject temp = Instantiate(go);
                temp.name = i.ToString() + "  " + j.ToString();
                //Material m = Instantiate(material);
                Material m = new Material(material);
                temp.transform.position = new Vector3(i + offset * i,j + offset * j,0);
                temp.GetComponent<MeshRenderer>().material = m;
                Element element = temp.GetComponent<Element>();

                element.SetId(j * size + i);
                element.SetPos(new Vector2(j,i));
                element.firstPos = new Vector2(j,i);
                //element.gameObject.GetComponent<Button>().onClick.AddListener(element.OnClick);
                pictures.Add(element);

                m.SetTexture("_MainTex", imageSource);//设置图片
                m.SetTextureScale("_MainTex", new Vector2(delta, delta));//设置大小和偏移
                m.SetTextureOffset("_MainTex", new Vector2(delta * i, delta * j));
                //Debug.Log(m.GetTextureOffset("_MainTex")+"  "+ new Vector2(delta * i, delta * j));
            }
        }
        pictures[size - 1].GetComponent<MeshFilter>().mesh = null;
        emptyElement = pictures[size - 1];
    }
    public void RandomImage()
    {
        //随机交换两张图片的位置，交换多次即可
        System.Random random = new System.Random();
        for (int i = 0; i < pictures.Count; i++)
        {
            int index = random.Next(0,pictures.Count);
            //交换位置
            Vector3 pos = pictures[i].transform.position;
            pictures[i].transform.position = pictures[index].transform.position;
            pictures[index].transform.position = pos;
        }
        //统计当前有多少个是在正确的位置上
        foreach (var item in pictures)
        {
            if (item.IsInRightPos(item.GetPos()))
                currentRightPosCount += 1;
        }
        //for (int i = 0; i < size; i++)
        //{
        //    for (int j = 0; j < size; j++)
        //    {
        //        pictures[i * size + j].SetPos(new Vector2(i,j));
        //        if (pictures[i * size + j].GetId() == (i * size + j))
        //            currentRightPosCount += 1;
        //    }
        //}
    }
    public void SwapElement(Element a,Element b)
    {
        //交换位置  
        StartCoroutine(StartMove(a, b));
    }
    IEnumerator StartMove(Element a,Element b)
    {
        Vector3 pos = a.transform.position;
        canMove = false;
        while(Vector3.Distance(a.transform.position,b.transform.position) > 0.1f)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            a.transform.position = Vector3.Lerp(a.transform.position, b.transform.position,0.1f);//move
        }
        a.transform.position = b.transform.position;
        b.transform.position = pos;
        canMove = true;
    }
    public void Update()
    {
        if(canMove && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray,out hit))
            {
                //判断当前的与空的位置
                int id = hit.collider.gameObject.GetComponent<Element>().GetId();
                Element element = hit.collider.gameObject.GetComponent<Element>();
                CheckClick(element);
                Debug.Log(id);
            }
        }
    }
    public void CheckClick(Element element)
    {
        //如果这个位置在
        Vector2 pos1 = element.GetPos();
        Vector2 pos2 = emptyElement.GetPos();
        if(Vector2.Distance(pos1,pos2) == 1)
        {
            Vector2 a = element.GetPos(),b = emptyElement.GetPos();
            if(element.IsInRightPos(a))
                currentRightPosCount -= 1;
            else if (element.IsInRightPos(b))//移到了正确位置
                currentRightPosCount += 1;
            if (emptyElement.IsInRightPos(b))//原本在正确位置
                currentRightPosCount -= 1;
            else if (emptyElement.IsInRightPos(a))//移到了正确位置
                currentRightPosCount += 1;
            SwapElement(element, emptyElement);//交换
            if (CheckWin())
                Debug.Log("You are win!");
        }
    }
}
