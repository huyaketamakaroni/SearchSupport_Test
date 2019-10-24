//display関係すべてコメントアウト
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class UnityTargetRotation: MonoBehaviour
{
    //Findで探しきれないため、手動でいれる
    [Header("オブジェクト")]
    [SerializeField]
    protected GameObject[] m_TargetObj = null;

    public GameObject arrowObj;



    [Header("音")]
    //音用flag
    public AudioSource audio1;
	public AudioSource audio2;
	public AudioSource audio3;
	public AudioSource audioOK;
	public AudioSource audioNG;
    public AudioSource targetAudio;
    public AudioClip targetClip;


    private static bool gameOverFlag = false;

	//配列の最大数を決定するための変数
	private int TARGET_MAX = 3;
	private int OBJECT_MAX = 48;
	
        
    //Displayを制御するための変数
	public GameObject display;
	public static GameObject DebugLog;
	bool is_debug_mode = true;
	Color BoxInvisibleColor = Color.black;

    /***テキスト用オブジェクト****/
    public Text StatusbarText;
    public TextMesh TaskText;

    //クリックイベントの数を数える変数
    public static int phase;
	//画角内に数字があるかを判定
	public static bool viewPortFlag;


	//オブジェクトを格納するための配列
	public static GameObject[] BoxAnnotations;
    

	//棚の番号配置
	public static int[] BoxIds = new int[] {
        1, 2, 3, 4, 5, 6, 7, 8, 9 ,10,11,12,
        13,14,15,16,17,18,19,20,21,22,23,24,
        25,26,27,28,29,30,31,32,33,34,35,36,
        37,38,39,50,41,42,43,44,45,46,47,48
    };

    //探索順序のパターン
	public static int[] patternA = new int[] { 4,17,48 };
	public static int[] patternB = new int[] { 1, 2, 3 };
    public static int[] patternC = new int[] { 36, 19, 12 };
    
    //実際に探索させる順序の配列
    public static int[] AnnotationIds = null;


	//ターゲットをTextに表示させるためのカウンタ変数
	public static int CurrentAnnotationId = 0;
	public double duration = 0.0;
	public float accumuDeltaTime = 0.0f;
	public Texture targetTexture;


    void Start()
	{
        //シーン内のテキストオブジェクトのコンポーネントを代入
		TaskText.text = null;
        //各変数を初期化
        VarInit();
		SetDebugMode(true);
        //SetDebugMode(false);


    }

	void VarInit()
	{
        BoxAnnotations = new GameObject[OBJECT_MAX];
        for (int i = 0; i < OBJECT_MAX; i++)
        {
            //GameObjectを格納
            BoxAnnotations[i] = m_TargetObj[i];
        }
        DebugLog = GameObject.Find("DebugLog");

        //変数の初期化
        phase =0;
		CurrentAnnotationId=0;
		viewPortFlag=false;

        SockertSend.SetNum(0);
		SockertSend.SetMode(0);
		SockertSend.SetTrial(0);
		SockertSend.SetNumFlag(false);
		SockertSend.SetDuration(0.0);
		SockertSend.SetAnswer("00");
		SockertSend.SetDurationFlag(false);
		SockertSend.SetDirectionFlag(false);
		SockertSend.SetDirection("no");
		SockertSend.SyncDisplay(this);
	}

    int select_mode = 0;

    void Update()
	{
		float fps = 1f / Time.deltaTime;

        if(select_mode == 0)
        {
             //情報提示パターンの選択
            TaskText.text = "提示パターン選択";

            if (Input.GetKeyDown("1"))
            {
                //中心視
                select_mode = 1;
                arrowObj.SetActive(true);
                DebugLog.SetActive(false);
                StatusbarText.enabled = false;
            }
            else if (Input.GetKeyDown("2"))
            {
                //音像定位
                select_mode = 2;
                arrowObj.SetActive(false);
                DebugLog.SetActive(false);
                StatusbarText.enabled = false;

            }
            else if (Input.GetKeyDown("3"))
            {
                //周辺視のみ
                select_mode = 3;
                arrowObj.SetActive(false);
                DebugLog.SetActive(false);
                StatusbarText.enabled = false;

            }
            else if (Input.GetKeyDown("4"))
            {
                //Debugモード
                select_mode = 4;
                arrowObj.SetActive(true);
                DebugLog.SetActive(true);
                StatusbarText.enabled = true;


            }

            if (select_mode != 0)
                audio2.Play();
        }



        if (select_mode != 0 && AnnotationIds == null)
        {
            TaskText.text = "探索パターンを選択";

            if (Input.GetKeyDown("a"))
                AnnotationIds = patternA;
            else if (Input.GetKeyDown("b"))
                AnnotationIds = patternB;
            else if (Input.GetKeyDown("c"))
                AnnotationIds = patternC;
        }

        if (select_mode != 0 && AnnotationIds != null)
            Task();


        if (Input.GetKeyDown(KeyCode.Return))
        {
            SockertSend.SetEnter(true);
            SetDebugMode(is_debug_mode);
            phase = 0;
            duration = 0.0;
            CurrentAnnotationId = 0;
            select_mode = 0;
            gameOverFlag = false;
            AnnotationIds = null;
            DebugLog.SetActive(true);
            Debug.Log("Key: Enter");
        }


    }

    void Task()
    {

        if (phase == 0)
        {
            TaskText.text = "PRESS SPACE KEY";
        }

        if (gameOverFlag)
        {
            TaskText.text = "Thank You";
            return;
        }

        RayCastTest.CheckRaycast();
        string dir = PereferalTest.GetDirection();
        SockertSend.SetDirection(dir);
        SockertSend.SetNum(AnnotationIds[CurrentAnnotationId]);
        SockertSend.SetDurationFlag(false);
        SockertSend.SetMode(PlayModeSelecter.GetMode());
        SockertSend.SetPhase(phase);
        SockertSend.SetDebug(is_debug_mode);
        SockertSend.SetEnter(false);
        SockertSend.SetTrial(CurrentAnnotationId);


        if (Input.GetKeyDown(KeyCode.Space))
        {

            ProceesPhase();
            Debug.Log("Key: Space");
        }

        



   

        //フラグを全部見てから、最後にSocketを送る
        SockertSend.SyncDisplay(this);

        if (accumuDeltaTime > 1.0f / 30)
        {
            SockertSend.SyncDisplay(this);
            accumuDeltaTime = 0.0f;
        }
        accumuDeltaTime += Time.deltaTime;


        //Status Bar


        try
        {
            string objectName = RayCastTest.GetSelectedGameObject().name;
            StatusbarText.text = "mode: " + PlayModeSelecter.GetMode() + ", "
                + "phase: " + phase + ", "
                + AnnotationIds[CurrentAnnotationId] + "-" + objectName + ", dir: " + dir;
        }
        catch (System.NullReferenceException)
        {
            StatusbarText.text = "mode: " + PlayModeSelecter.GetMode() + ", "
                + "phase: " + phase + ", "
                + "-" + "null" + ", dir: " + dir; ;
        }
    }


	//スペースを押したときの動作
	void ProceesPhase()
	{
		GameObject box = GameObject.Find("Cube"+AnnotationIds[CurrentAnnotationId]);


		//待機状態→探索フェーズ
		if (phase==0)
		{
			//時間計測スタート

			duration=Time.time;
			SockertSend.SetNumFlag(true);
			int mode = PlayModeSelecter.GetMode();
			if (mode == 2 || mode == 4 || mode == 0) {
                
                if (AnnotationIds[CurrentAnnotationId] % 3 == 1)
                {
                    TaskText.text = "「赤い箱」を探せ";
                    BoxAnnotations[AnnotationIds[CurrentAnnotationId] - 1].GetComponent<Renderer>().material.color = Color.red;
                }
                if (AnnotationIds[CurrentAnnotationId] % 3 == 2)
                {
                    TaskText.text = "「緑の箱」を探せ";
                    BoxAnnotations[AnnotationIds[CurrentAnnotationId] - 1].GetComponent<Renderer>().material.color = Color.green;

                }
                if (AnnotationIds[CurrentAnnotationId] % 3 == 0)
                {
                    TaskText.text = "「青い箱」を探せ";
                    BoxAnnotations[AnnotationIds[CurrentAnnotationId] - 1].GetComponent<Renderer>().material.color = Color.blue;
                }

                TargetTracker.targetObject = BoxAnnotations[AnnotationIds[CurrentAnnotationId] - 1];
            }

			if (mode==3||mode==4||mode==0)
			{
				SockertSend.SetDirectionFlag(true);
			}
			else
			{
				SockertSend.SetDirectionFlag(false);
			}

            targetAudio = BoxAnnotations[AnnotationIds[CurrentAnnotationId] - 1].GetComponent<AudioSource>();

            if (select_mode == 2 || select_mode == 4)
            {                
                targetAudio.clip = targetClip;                
            }

            targetAudio.Play();
            audio1.Play();
            

            Debug.Log("-----------------");
			Debug.Log("phase 1");
			phase = 1;
		}

		//探索フェーズ→回答フェーズ
		else if (phase==1)
		{

            audio2.Play();

			SockertSend.SetDirectionFlag(false);

			SockertSend.SetNumFlag(false);
			duration=Time.time-duration;
			SockertSend.SetDuration(duration);
			Debug.Log("-----------------");
			Debug.Log("phase 3: "+duration);


			//phase3
			//正解／不正解にかかわらずいずれかのBoxを見ている場合は次にすすむ
			bool is_looking_at_box = false;

            if (RayCastTest.GetSelectedGameObject() == BoxAnnotations[AnnotationIds[CurrentAnnotationId] - 1])
            {
                is_looking_at_box = true;
            }


			//正誤送信
			if (is_looking_at_box)
			{
				string name = RayCastTest.GetSelectedGameObject().name.ToString().Substring(4);
				//SockertSend.SetMode(PlayModeSelecter.GetMode());
				SockertSend.SetAnswer(name);
				SockertSend.SetDurationFlag(true);
			}
			//Boxを見ていない場合
			else
			{
				audioNG.Play();
				return;
			}

            //正解の場合次へ
            TargetTracker.targetObject = display;
            targetAudio.clip = null;
            targetAudio.Play();
            audioOK.Play();
			box.GetComponent<Renderer>().material.color=BoxInvisibleColor;


			//ディスプレイを見たとき
			//数字を表示，時間を記録しない
			CurrentAnnotationId++;



			//修正前　最初に戻る
			//修正後　Endと表示
			if (CurrentAnnotationId==TARGET_MAX)
			{
				SockertSend.SetNumFlag(true);
				SockertSend.SetNum("End");
				gameOverFlag=true;
				Debug.Log("-----------------");
				Debug.Log("-----------------");
				Debug.Log ("----END----");
				Debug.Log("-----------------");
				Debug.Log("-----------------");
				CurrentAnnotationId--;

			}

			phase=0;
		}

	}

	public static GameObject GetTempTargetArray()
	{
		try
		{
			return GameObject.Find("Cube"+AnnotationIds[CurrentAnnotationId]);
		}
		catch
		{
			return GameObject.Find("Cube"+1);
		}
	}


    //Debugモード
    void SetDebugMode(bool is_debug_mode)
    {
        //画面上から流れるデバッグメッセージ
        if (is_debug_mode)
        {
            for (int i = 0; i < OBJECT_MAX; i++)
            {
                if (BoxAnnotations[i].GetComponent<Renderer>().material.color == BoxInvisibleColor)
                    BoxAnnotations[i].GetComponent<Renderer>().material.color = Color.white;
                else
                    BoxAnnotations[i].GetComponent<Renderer>().material.color = Color.white;
            }

            BoxInvisibleColor = Color.white;
        }
        else
        {
            for (int i = 0; i < OBJECT_MAX; i++)
            {
                if (BoxAnnotations[i].GetComponent<Renderer>().material.color == BoxInvisibleColor)
                    BoxAnnotations[i].GetComponent<Renderer>().material.color = Color.white;
                else
                    BoxAnnotations[i].GetComponent<Renderer>().material.color = Color.white;
            }
            BoxInvisibleColor = Color.white;
        }


        //画面下の状態表示
        if (is_debug_mode)
        {
            StatusbarText.enabled = true;
        }
        else
        {
            StatusbarText.enabled = false;
        }

    }

}

