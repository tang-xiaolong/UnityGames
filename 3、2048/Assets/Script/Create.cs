using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Create : MonoBehaviour {
	static public int N = 4;
	static public int M = 13;
	int[,] Map = new int[N,N];
	private bool isWin = false;
	private bool isLose = false;
	public GameObject[,] images = new GameObject[N,N];//保存所有的精灵
	private Sprite[] sprites;//这是资源
	public GameObject num;//加载预设

    public Transform bg;
    public GameObject resPanel;
    public Button reStart;
    public Button quit;
    public Text resultInfo;

    private void ReStart()
    {
        SceneManager.LoadScene("main");
    }
    private void QuitGame()
    {
        Application.Quit();
    }
	public void Init()
	{
        //hide result panel
        if(resPanel != null)
            resPanel.SetActive(false);
        if(reStart != null)
            reStart.onClick.AddListener(ReStart);
        if(quit != null)
            quit.onClick.AddListener(QuitGame);

		sprites = Resources.LoadAll<Sprite>("numbers");
		Debug.Log(sprites.Length);
		int n = 2;
		for (int i = 0; i < N; ++i)
			for (int j = 0; j < N; ++j)
				Map[i, j] = 0;
		for (int i = 0; i < n; ++i)
		{
			int i1 = Random.Range(0,N-1);
			int i2 = Random.Range(0, N - 1);
			//随机一组下标，让地图那个位置变成1
			Map[i1,i2] = 1; //这个1代表2的一次，最终展现的是2 
		}
		for(int i = 0;i < N;++i)//
		{
			for (int j = 0; j < N; j++)
			{
				GameObject go = Instantiate(num,bg);
                //go.transform.position = new Vector3(0.5f * j, -0.5f * i, 0)+new Vector3(-0.5f*(N/2), 0.5f * (N / 2), 0);
                (go.transform as RectTransform).anchoredPosition = new Vector3(128f * j, -128 * i, 0) + new Vector3(3.0f*j,-3.0f*i,0) + new Vector3(18,-12,0);
				//go.transform.localPosition = new Vector3(128f * j, -128 * i, 0)+new Vector3(-128f*(N/2), 128 * (N / 2), 0);
				go.name = i.ToString() + "  " + j.ToString();
				images[i, j] = go;
			}
		}
		Show();
	}
	void Generator(int dir)
	{
		//按下右，就按列扫，从左往右随机选一个数生成
		//按下上，就按行扫，从下网上随机选一个数生成	
		int flag = 0;
		int i1 = -1, i2 = -1;
		if (dir == 1 || dir == 2)
		{
			int begin, end, mul;
			if(dir == 1)
			{
				begin = 0;
				end = N;
				mul = 1;
			}
			else
			{
				begin = N - 1;
				end = -1;
				mul = -1;
			}
			for (int i = 0; i < N; ++i)
				for (int j = begin; j != end; j += mul)
				{
					//保证选一个数，然后其他的随机
					if (Map[i,j] == 0)//选到了一个数
					{
						if (flag == 0)//之前没选到过
						{
							i1 = i;
							i2 = j;
							flag = 1;
						}
						else
						{
							if (Random.Range(0,2) == 1)
							{
								i1 = i;
								i2 = j;
							}
						}
					}
				}
		}
		else if (dir == 3 || dir == 4)
		{
			int begin, end, mul;
			if (dir == 3)
			{
				begin = 0;
				end = N;
				mul = 1;
			}
			else
			{
				begin = N - 1;
				end = -1;
				mul = -1;
			}
			for (int j = begin; j != end; j += mul)
				for (int i = 0; i < N; ++i)
				{
					//保证选一个数，然后其他的随机
					if (Map[i,j] == 0)//选到了一个数
					{
						if (flag == 0)//之前没选到过
						{
							i1 = i;
							i2 = j;
							flag = 1;
						}
						else
						{
							if (Random.Range(0,2) == 1)
							{
								i1 = i;
								i2 = j;
							}
						}
					}
				}
		}
		if (i1 == i2 && i2 == -1)
			return;
		//Debug.Log("选到的是：" + i1 + "  " + i2);
		Map[i1,i2] = 1;
	}

	int Up(ref int isChange)
	{
		//往上，所以是从上往下扫描
		//一列一列的算 

		for (int j = 0; j < N; ++j)
		{
			int k = 0;
			for (int i = 1; i < N; ++i)
			{
				if (Map[i,j] != 0)//当前处理的这一位是有数字的 
				{
					//只要是非0，且没有合并，则相加 
					//这里保证Map[i,j]是有数字的 
					if (Map[i,j] == Map[k,j])
					{
						//与那个数字进行交互。合并操作 
						Map[k,j] += 1;
						Map[i,j] = 0;
						if (Map[k,j] == 12)
							return 5;
						isChange = 1;
					}
					else if (Map[k,j] == 0)//我们是说k是第一个非零的，但初始的时候我们为了好运算，是没做判断的
					{
						//把当前位置上的数移过去   特判的移动操作 
						Map[k,j] = Map[i,j];
						Map[i,j] = 0;
						isChange = 1;
					}
					else if (k + 1 != i)//移动操作 
					{
						Map[k + 1,j] = Map[i,j];//2 0 0 4  2 4 0 0     
						Map[i,j] = 0;//4 2 0 0
						++k;
						isChange = 1;
					}

					else//没有合并，且不是特判的 
						++k;
				}
			}
		}
		return 1;
	}
	int Down(ref int isChange)
	{
		for (int j = 0; j < N; ++j)
		{
			int k = N - 1;
			for (int i = N - 2; i >= 0; --i)
			{
				if (Map[i,j] != 0)//当前处理的这一位是有数字的 
				{
					//只要是非0，且没有合并，则相加 
					//这里保证Map[i,j]是有数字的 
					if (Map[i,j] == Map[k,j])
					{
						//与那个数字进行交互。合并操作 
						Map[k,j] += 1;
						Map[i,j] = 0;
						if (Map[k,j] == 12)
							return 5;
						isChange = 1;
					}
					else if (Map[k,j] == 0)//我们是说k是第一个非零的，但初始的时候我们为了好运算，是没做判断的
					{
						//把当前位置上的数移过去   特判的移动操作 
						Map[k,j] = Map[i,j];
						Map[i,j] = 0;
						isChange = 1;
					}
					else if (k - 1 != i)//移动操作 
					{
						Map[k - 1,j] = Map[i,j];//2 0 0 4  2 4 0 0     
						Map[i,j] = 0;//4 2 0 0
						--k;
						isChange = 1;
					}
					else//没有合并，且不是特判的 
						--k;
				}
			}
		}
		return 2;
	}
	int Left(ref int isChange)
	{
		//合并？什么情况下合并？什么情况下移动？
		//一行一行处理
		//从左往右处理
		//这个有数字，我们才需要处理，没数字不用管  2 0 4 8    2 4 0 8    2 4 8 0
		for (int i = 0; i < N; ++i)
		{
			int k = 0;
			for (int j = 1; j < N; ++j)
			{
				if (Map[i,j] != 0)//当前处理的这一位是有数字的 
				{
					//这里保证Map[i,j]是有数字的 
					if (Map[i,j] == Map[i,k])
					{
						//与那个数字进行交互。合并操作 
						Map[i,k] += 1;
						Map[i,j] = 0;
						if (Map[i,k] == 12)
							return 5;
						isChange = 1;
					}
					else if (Map[i,k] == 0)//我们是说k是第一个非零的，但初始的时候我们为了好运算，是没做判断的
					{
						//把当前位置上的数移过去   特判的移动操作 
						Map[i,k] = Map[i,j];
						Map[i,j] = 0;
						isChange = 1;
					}
					else if (k + 1 != j)//移动操作 
					{
						Map[i,k + 1] = Map[i,j];//2 0 0 4  2 4 0 0     
						Map[i,j] = 0;//4 2 0 0
						++k;
						isChange = 1;
					}
					else
						++k;
				}
			}
		}
		return 3;
	}
	int Right(ref int isChange)
	{
		//从右至左 
		for (int i = 0; i < N; ++i)
		{
			int k = N - 1;
			for (int j = N - 2; j >= 0; --j)
			{
				if (Map[i,j] != 0)//当前处理的这一位是有数字的 
				{
					//这里保证Map[i,j]是有数字的 
					if (Map[i,j] == Map[i,k])
					{
						//与那个数字进行交互。合并操作 
						Map[i,k] += 1;
						Map[i,j] = 0;
						if (Map[i,k] == 12)
							return 5;
						isChange = 1;
					}
					else if (Map[i,k] == 0)//我们是说k是第一个非零的，但初始的时候我们为了好运算，是没做判断的
					{
						Map[i,k] = Map[i,j];
						Map[i,j] = 0;
						isChange = 1;
					}
					else if (k - 1 != j)//移动操作 
					{
						Map[i,k - 1] = Map[i,j];
						Map[i,j] = 0;
						--k;
						isChange = 1;
					}
					else
						--k;
				}
			}
		}
		return 4;
	}
	public void Show()
	{
		//在地图上显示图片
		for(int i = 0;i < N;++i)
		{
			//Debug.Log(i+"  : "+Map[i,0]+"  "+Map[i, 1] + "  " + Map[i, 2] + "  " + Map[i, 3] + "  " + Map[i, 4]);
			for(int j = 0;j < N;++j)
			{
				//images[i, j].GetComponent<SpriteRenderer>().sprite = sprites[Map[i,j]];
				images[i, j].GetComponent<Image>().sprite = sprites[Map[i,j]];
			}
		}
	}
	public void Run()
	{
		int isChange = 0;//检测是否发生了变化 
		int flag = 0;
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			flag = Up(ref isChange);
		}
		else if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			flag = Down(ref isChange);
		}
		else if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			flag = Left(ref isChange);
		}
		else if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			flag = Right(ref isChange);
		}
		if (flag != 5 && isChange != 0)
			Generator(flag);
		if (flag == 5)
		{
			isWin = true;
		}
		else if (IsLose() == 1)
		{
			isLose = true;
		}
        if(isChange != 0)
            Show();
	}
	int IsLose()
	{
		//判断是否还具备可操作空间
		for (int i = 0; i < N; ++i)
			for (int j = 0; j < N - 1; ++j)
			{
				if (Map[i,j] == Map[i,j + 1])
					return 0;
				else if (Map[i,j] * Map[i,j + 1] == 0)
					return 0;
			}
		for (int j = 0; j < N; ++j)
			for (int i = 0; i < N - 1; ++i)
			{
				if (Map[i,j] == Map[i + 1,j])
					return 0;
				else if (Map[i,j] * Map[i + 1,j] == 0)
					return 0;
			}
		return 1;
	}
	void Start () {
		Init();
        Show();
    }
	
	// Update is called once per frame
	void Update () {
		
		if (isWin)//如果胜利了，显示胜利界面
		{
            Win();
			return;
		}
		else if(isLose)//失败了
		{
            Lose();
			return;
		}
		else
		{
			Run();
		}
	}
    private void Win()
    {
        if (resPanel != null)
            resPanel.SetActive(true);
        if (resultInfo != null)
            resultInfo.text = "恭喜你获胜了！";
    }

    private void Lose()
    {
        if (resPanel != null)
            resPanel.SetActive(true);
        if (resultInfo != null)
            resultInfo.text = "很遗憾你输了~重新战斗吧！";
    }
	
}
