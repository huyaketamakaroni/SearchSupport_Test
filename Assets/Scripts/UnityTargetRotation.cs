//display関係すべてコメントアウト
using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using System.IO;

#if UNITY_UWP
using Windows.Storage;
using System.Threading.Tasks;
#endif


public class UnityTargetRotation : MonoBehaviour
{
    //Findで探しきれないため、手動でいれる
    [Header("オブジェクト")]
    [SerializeField]
    protected GameObject[] m_TargetObj = null;

    public GameObject arrowObj;

    public Material InvMaterial;

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
    private int TARGET_MAX = 10;
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
        38, 18, 17, 45, 3, 41, 32, 20, 16, 35, 2, 36,//前
        39, 24, 10, 14, 23, 44, 42, 22, 25, 47, 30, 43,//右
        15 ,7, 37, 4, 11, 46, 31, 13, 48, 5, 19, 21,//後
        6, 27, 26, 34, 29, 8, 40, 9, 28, 12, 1, 33//左
    };

    //探索順序のパターン
    public static int[] patternA = new int[] { 39, 46, 22, 33, 9, 25, 3, 4, 21, 31 };
    public static int[] patternB = new int[] { 11, 24, 35, 44, 31, 6, 2, 40, 26, 30 };
    public static int[] patternC = new int[] { 2, 6, 33, 48, 17, 29, 44, 1, 8, 42 };//29
    public static int[] patternD = new int[] { 7, 41, 36 };
    public static int[] patternE = new int[] { 12, 28, 19 };
    public static int[] patternF = new int[] { 32, 18, 39 };

    //実際に探索させる順序の配列
    public static int[] AnnotationIds = null;

    //時間添付
    public double[] MeasureTime = new double[10];

    //ターゲットをTextに表示させるためのカウンタ変数
    public static int CurrentAnnotationId = 0;
    public double duration = 0.0;
    public float accumuDeltaTime = 0.0f;
    public Material InvMaterial2;

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
        phase = 0;
        CurrentAnnotationId = 0;
        viewPortFlag = false;

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

    void LookBoxColor()
    {
        for (int i = 0; i < OBJECT_MAX; i++)
        {
            m_TargetObj[i].GetComponent<Renderer>().material.color = Color.white;
        }
    }

    void InvBoxColor()
    {
        for (int i = 0; i < OBJECT_MAX; i++)
        {
            m_TargetObj[i].GetComponent<Renderer>().material.color = Color.black;
        }
    }

    int select_mode = 0;

    void Update()
    {
        float fps = 1f / Time.deltaTime;

        //Debug.Log(DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss"));

        if (select_mode == 0)
        {
            if (Input.GetKeyDown("r") && Input.GetKey("x"))
            {
                transform.Rotate(new Vector3(0.5f, 0, 0));
            }

            if (Input.GetKeyDown("l") && Input.GetKey("x"))
            {
                transform.Rotate(new Vector3(-0.5f, 0, 0));
            }
            if (Input.GetKeyDown("r") && Input.GetKey("y"))
            {
                transform.Rotate(new Vector3(0, 0.5f, 0));
            }
            if (Input.GetKeyDown("l") && Input.GetKey("y"))
            {
                transform.Rotate(new Vector3(0, -0.5f, 0));
            }

            if (Input.GetKeyDown("r") && Input.GetKey("z"))
            {
                transform.Rotate(new Vector3(0, 0, 0.5f));
            }
            if (Input.GetKeyDown("l") && Input.GetKey("z"))
            {
                transform.Rotate(new Vector3(0, 0, -0.5f));
            }

            //情報提示パターンの選択
            TaskText.text = "提示パターン選択";

            if (Input.GetKeyDown("1"))
            {
                //中心視
                select_mode = 1;
                arrowObj.SetActive(true);
                DebugLog.SetActive(false);
                StatusbarText.enabled = false;
                InvBoxColor();
            }
            else if (Input.GetKeyDown("2"))
            {
                //音像定位
                select_mode = 2;
                arrowObj.SetActive(false);
                DebugLog.SetActive(false);
                StatusbarText.enabled = false;
                InvBoxColor();
            }
            else if (Input.GetKeyDown("3"))
            {
                //周辺視のみ
                select_mode = 3;
                arrowObj.SetActive(false);
                DebugLog.SetActive(false);
                StatusbarText.enabled = false;
                InvBoxColor();
            }
            else if (Input.GetKeyDown("4"))
            {
                //Debugモード
                select_mode = 4;
                arrowObj.SetActive(true);
                DebugLog.SetActive(true);
                StatusbarText.enabled = true;
                LookBoxColor();
            }

            if (select_mode != 0)
                audio2.Play();
        }



        if (select_mode != 0 && AnnotationIds == null)
        {
            TaskText.text = "探索パターンを選択";

            if (Input.GetKeyDown("a"))
            {
                AnnotationIds = patternA;
                TARGET_MAX = 10;
            }
            else if (Input.GetKeyDown("b"))
            {
                AnnotationIds = patternB;
                TARGET_MAX = 10;
            }
            else if (Input.GetKeyDown("c"))
            {
                AnnotationIds = patternC;
                TARGET_MAX = 10;
            }
            else if (Input.GetKeyDown("d"))
            {
                AnnotationIds = patternD;
                TARGET_MAX = 3;
            }
            else if (Input.GetKeyDown("e"))
            {
                AnnotationIds = patternE;
                TARGET_MAX = 3;
            }
            else if (Input.GetKeyDown("f"))
            {
                AnnotationIds = patternF;
                TARGET_MAX = 3;
            }
        }

        if (select_mode != 0 && AnnotationIds != null)
            UserTask();


        if (Input.GetKeyDown(KeyCode.Return))
        {
            SockertSend.SetEnter(true);
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
            SetDebugMode(is_debug_mode);
            phase = 0;
            duration = 0.0;
            CurrentAnnotationId = 0;
            select_mode = 0;
            gameOverFlag = false;
            AnnotationIds = null;
            DebugLog.SetActive(true);
            MeasureTime = new double[10];
            TargetTracker.targetObject = display;
            Debug.Log("Key: Enter");

            
        }





    }



    void UserTask()
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

        /*
        if (targetAudio != null)
        {
            if (TargetTracker.screenArea.Contains(TargetTracker.targetPoint))
                targetAudio.volume = 1f;
            else
                targetAudio.volume = 0.3f;
        }
        */

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
        GameObject box = GameObject.Find("Cube" + AnnotationIds[CurrentAnnotationId]);


        //待機状態→探索フェーズ
        if (phase == 0 && TargetTracker.screenArea.Contains(TargetTracker.targetPoint))
        {
            //時間計測スタート

            duration = Time.time;
            SockertSend.SetNumFlag(true);
            int mode = PlayModeSelecter.GetMode();
            if (mode == 2 || mode == 4 || mode == 0)
            {
                TaskText.text = BoxIds[AnnotationIds[CurrentAnnotationId] - 1] + "を探せ";
                TargetTracker.targetObject = BoxAnnotations[AnnotationIds[CurrentAnnotationId] - 1];
            }

            if (mode == 3 || mode == 4 || mode == 0)
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
        else if (phase == 1)
        {
            audio2.Play();

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
            //box.GetComponent<Renderer>().material.color=BoxInvisibleColor;


            SockertSend.SetDirectionFlag(false);

            SockertSend.SetNumFlag(false);
            duration = Time.time - duration;
            SockertSend.SetDuration(duration);
            Debug.Log("-----------------");
            Debug.Log("phase 2: " + duration);

            MeasureTime[CurrentAnnotationId] = duration;

            //ディスプレイを見たとき
            //数字を表示，時間を記録しない
            CurrentAnnotationId++;



            //修正前　最初に戻る
            //修正後　Endと表示
            if (CurrentAnnotationId == TARGET_MAX)
            {
                csvWrite();
                SockertSend.SetNumFlag(true);
                SockertSend.SetNum("End");
                gameOverFlag = true;
                Debug.Log("-----------------");
                Debug.Log("-----------------");
                Debug.Log("----END----");
                Debug.Log("-----------------");
                Debug.Log("-----------------");
                CurrentAnnotationId--;

            }

            phase = 0;
        }

    }

    public static GameObject GetTempTargetArray()
    {
        if (phase == 2)
        {
            try
            {
                return TargetTracker.targetObject;

            }
            catch
            {
                return TargetTracker.targetObject;

            }
        }
        else
        {
            return TargetTracker.targetObject;
        }

    }


    //Debugモード
    void SetDebugMode(bool is_debug_mode)
    {

    }

    void csvWrite()
    {
#if UNITY_UWP
        Task.Run(async ()=>
        {
            // ローカルフォルダー
            // 「User Files\LocalAppData\<アプリ名>\LocalState」 以下にできる
            {
                var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(
                    "TimeData", CreationCollisionOption.OpenIfExists
                );
                var file = await folder.CreateFileAsync(
                    DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv", CreationCollisionOption.ReplaceExisting
                );                         

                using (var stream = await file.OpenStreamForWriteAsync())
                {
                    for(int cnt_out=0; cnt_out<10; cnt_out++){;
                        var bytes = System.Text.Encoding.UTF8.GetBytes(
                            @select_mode + "," + MeasureTime[cnt_out] +"\n"
                        );
                        await stream.WriteAsync(bytes, 0, bytes.Length);
                        if (cnt_out == 9) {
                            await stream.WriteAsync(
                                System.Text.Encoding.UTF8.GetBytes(@"\n"),
                                0,
                                System.Text.Encoding.UTF8.GetBytes(@"\n").Length);
                        }
                    }
                }
            }
        });
#endif
    }

}

