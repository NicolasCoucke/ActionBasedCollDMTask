using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Collective DM touch 
public class GameManager : MonoBehaviour
{
    Camera Cam1;
    Camera Cam2;

    GameObject Cursor1;
    GameObject Cursor2;

    GameObject Cursor3;
    GameObject Cursor4;

    Vector2 worldposition1;
    Vector2 worldposition2;

    Vector2 worldposition3;
    Vector2 worldposition4;

    [SerializeField]
    GameObject StartScreen;
    int CountDownValue;

    [SerializeField]
    Text StartCounter;

    string path;

    bool IsStarted;
    bool PreStarted;



    [SerializeField]
    GameObject FinishScreen;

    [SerializeField]
    Text FinishFeedback;

    public Text TrialText;

    public Material Red;
    public Material Green;

    //public GameObject StimulusSprite;

    private Sprite mySprite;
    private SpriteRenderer sr;


    float Circle_radius_1 = 1.5f;
    float Circle_radius_2 = 1.5f;

    float Score = 0f;
   // public GameObject ScoreText;



    bool On_Spill_1;
    bool On_Spill_2;

    bool On_Spill_3;
    bool On_Spill_4;


    int trial = 1;

    bool isFinished;
    int Location = 0;
    int Difficulty = 0;

    float TimeCounter = 0f;

    public List<GameObject> TargetList;

    public Text Individual_countdowntext;

    public GameObject IndividualCanvas;

    public GameObject EndPauseButton;

    // Start is called before the first frame update
    void Start()
    {


        isFinished = false;
        Cam1 = GameObject.Find("Camera 1").GetComponent<Camera>();

        Cursor1 = GameObject.Find("Cursor 1");
        Cursor2 = GameObject.Find("Cursor 2");

        Cursor3 = GameObject.Find("Cursor 3");
        Cursor4 = GameObject.Find("Cursor 4");

        Cursor1.SetActive(false);
        Cursor2.SetActive(false);

        Cursor3.SetActive(false);
        Cursor4.SetActive(false);


        CountDownValue = 5;
        StartCounter.text = CountDownValue.ToString();
        InvokeRepeating("StartCountDown", 1f, 1f);
        InvokeRepeating("CheckHits", 0f, 0.1f);

 

    }

    void StartCountDown()
    {
        StartScreen.SetActive(true);
        CountDownValue -= 1;
        StartCounter.text = CountDownValue.ToString();
        if (CountDownValue == 0)
        {
          //  if (SendTriggers == true)
          //  {
           //     triggersender.SendTrigger(StaticVariables.ConditionNumber);
           // }
            PreStarted = true;
            StartScreen.SetActive(false);
            //  InvokeRepeating("StoreData", 0f, 0.01f);
            StartCoroutine("ShowStimulus");
            CancelInvoke("StartCountDown");
            
        }

    }


    IEnumerator ShowStimulus()
    {
        string stimulus_path = @"C:\Users\Administrator\Desktop\CollDM_LEVELDATA";
        DirectoryInfo dir = new DirectoryInfo(stimulus_path);
        FileInfo[] info = dir.GetFiles();

        trial = StaticVariables.LevelNumber;
        TrialText.text = "Trial " + trial.ToString();
        foreach (FileInfo item in info)
        {
            string filename = Path.GetFileName(item.ToString());
            string[] splitArray = filename.Split(char.Parse("_"));
            if (int.Parse(splitArray[1]) == trial)
            {
                stimulus_path = stimulus_path + "\\" + filename;
                Debug.Log(trial + filename);
                Texture2D tex = LoadPNG(stimulus_path);
                sr = gameObject.AddComponent<SpriteRenderer>() as SpriteRenderer;
                sr.color = new Color(0.9f, 0.9f, 0.9f, 1.0f);
                transform.position = new Vector3(-3f, -2f, 0.0f);
                mySprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0f, 0f), 100.0f);
                sr.sprite = mySprite;
                Location = int.Parse(splitArray[2]);
                Difficulty = int.Parse(splitArray[3].Split(char.Parse("."))[0]);
            }

         

        }


        if (StaticVariables.Definedpath == false)
        {
            path = @"C:\Users\Administrator\Desktop\GAMEDATA_CollDM";
            string DataString = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
            path = path + "\\" + DataString + ".csv";
            StaticVariables.Path = path;
            StaticVariables.Definedpath = true;
        }
        {
            path = StaticVariables.Path;
        }

        yield return new WaitForSeconds(0.5f);

        sr.enabled = false;
       


        //IndividualPhase
        IndividualCanvas.SetActive(true);
        KeyPadManager.keyPadManager.StartListenForAnswers();
        for (int t = 10; t > 0; t--)
        {

            if (!KeyPadManager.keyPadManager.AnswersDone)
            {
                yield return new WaitForSeconds(1f);
                Individual_countdowntext.text = t.ToString();
            }


        }

        IndividualCanvas.SetActive(false);
        

        InvokeRepeating("StoreData", 0f, 0.01f);
        using (StreamWriter file = new StreamWriter(@path, true))
        {

            file.WriteLine("Trial " + StaticVariables.LevelNumber.ToString() + "," + Location.ToString() + "," + Difficulty.ToString());
            file.WriteLine(KeyPadManager.keyPadManager.ResponseString);


        }
        Debug.Log(KeyPadManager.keyPadManager.ResponseString);

        Cursor1.SetActive(true);
        Cursor2.SetActive(true);
        if(StaticVariables.NumPlayers == 3)
        {
            Cursor3.SetActive(true);
        }
        if (StaticVariables.NumPlayers == 4)
        {
            Cursor3.SetActive(true);
            Cursor4.SetActive(true);
        }

    }

 

    
    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == StaticVariables.NumPlayers)
        {
            if(PreStarted == true)
            {
                IsStarted = true;

            }
        }

        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch1 = Input.GetTouch(i);
            Vector2 touchposition1 = touch1.position;

            if (Vector2.Distance(Cam1.ScreenToWorldPoint(touch1.position), Cursor1.transform.position) < 0.6)
            {
                worldposition1 = Cam1.ScreenToWorldPoint(touchposition1);
                if (IsStarted == true)
                {
                    //Debug.Log("1touchDistance");
                    Cursor1.transform.position = worldposition1;
                    continue;

                }
            }


            if (Vector2.Distance(Cam1.ScreenToWorldPoint(touch1.position), Cursor2.transform.position) < 0.6)
            {
                worldposition2 = Cam1.ScreenToWorldPoint(touchposition1);
                if (IsStarted == true)
                {
                    //Debug.Log("1touchDistance");
                    Cursor2.transform.position = worldposition2;
                    continue;

                }

            }

            if (Vector2.Distance(Cam1.ScreenToWorldPoint(touch1.position), Cursor3.transform.position) < 0.6)
            {
                worldposition3 = Cam1.ScreenToWorldPoint(touchposition1);
                if (IsStarted == true)
                {
                    //Debug.Log("1touchDistance");
                    Cursor3.transform.position = worldposition3;
                    continue;

                }

            }

            if (Vector2.Distance(Cam1.ScreenToWorldPoint(touch1.position), Cursor4.transform.position) < 0.6)
            {
                worldposition4 = Cam1.ScreenToWorldPoint(touchposition1);
                if (IsStarted == true)
                {
                    //Debug.Log("1touchDistance");
                    Cursor4.transform.position = worldposition4;
                    continue;

                }

            }
        }

        TimeCounter += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("LobbyScene");
        }

    }






    void CheckHits()
    {
        if(isFinished == false)
        {
            for (int i = 0; i < TargetList.Count; i++)
            {
                GameObject Circle = TargetList[i];
                int oncircle = 0;
                Vector2 CirclePos = Circle.transform.position;
                float distance1 = Vector2.Distance(worldposition1, CirclePos);
                float distance2 = Vector2.Distance(worldposition2, CirclePos);

                float distance3 = Vector2.Distance(worldposition3, CirclePos);
                float distance4 = Vector2.Distance(worldposition4, CirclePos);

                if (distance1 < 1.5)
                {
                    oncircle += 1;
                }


                if (distance2 < 1.5)
                {
                    oncircle += 1;
                }

                if(StaticVariables.NumPlayers == 3)
                {
                    if(Vector2.Distance(worldposition3, CirclePos) < 1.5)
                    {
                        oncircle += 1;
                    }
                }
                if (StaticVariables.NumPlayers == 4)
                {
                    if (Vector2.Distance(worldposition3, CirclePos) < 1.5)
                    {
                        oncircle += 1;
                    }
                    if (Vector2.Distance(worldposition4, CirclePos) < 1.5)
                    {
                        oncircle += 1;
                    }
                }




                if (oncircle == StaticVariables.NumPlayers)
                {
                    if (Location == i+1)
                    {
                        Circle.GetComponent<SpriteRenderer>().color = Color.green;
                        FinishScreen.active = true;
                        FinishFeedback.text = "CORRECT";
                        Finished("CORRECT");

                    }
                    else
                    {
                        Circle.GetComponent<SpriteRenderer>().color = Color.red;
                        FinishScreen.active = true;
                        FinishFeedback.text = "WRONG";
                        Finished("WRONG");
                    }
                    isFinished = true;
                    
                    // Give Feedback and start next trial
                    // Maybe make trial a little easier (python code)

                }
                else
                {
                    //Circle.GetComponent<SpriteRenderer>().color = Color.red;
                    On_Spill_1 = false;
                }
            }
        }
       
       

    }

    void Finished(string isCorrect)
    {
     
        FinishScreen.SetActive(true);
        string FinishTime = TimeCounter.ToString();
        // CompletionTime.text = FinishTime;
        IEnumerator coroutine = AfterFinished();
        StartCoroutine(coroutine);
        CancelInvoke("StoreData");
        using (StreamWriter file = new StreamWriter(@path, true))
        {
            file.WriteLine("Finished " + isCorrect + " " + FinishTime.ToString());
        }
       
    }

    IEnumerator AfterFinished()
    {
        yield return new WaitForSeconds(5f);
        NextLevel();
    }


    void NextLevel()
    {

        if(StaticVariables.LevelNumber < 64)
        {
            if (StaticVariables.LevelNumber == 16 || StaticVariables.LevelNumber == 32 || StaticVariables.LevelNumber == 48)
            {
                EndPauseButton.SetActive(true);



            }
            else
            {
                StaticVariables.LevelNumber += 1;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }

      
        }
        else
        {
            SceneManager.LoadScene("LobbyScene");
        }


    }

   public void PressButtonForNextTrial()
    {
        EndPauseButton.SetActive(false);
        StaticVariables.LevelNumber += 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
    }


    void StoreData()
    {
        using (StreamWriter file = new StreamWriter(@path, true))
        {
            if (StaticVariables.NumPlayers == 2)
            {
                file.WriteLine(TimeCounter.ToString("F4") + "," + worldposition1.ToString("F4") + "," + worldposition2.ToString("F4"));

            }
            if (StaticVariables.NumPlayers == 3)
            {
                file.WriteLine(TimeCounter.ToString("F4") + "," + worldposition1.ToString("F4") + "," + worldposition2.ToString("F4") + "," + worldposition3.ToString("F4") );

            }
            if (StaticVariables.NumPlayers == 4)
            {
                file.WriteLine(TimeCounter.ToString("F4") + "," + worldposition1.ToString("F4") + "," + worldposition2.ToString("F4") + "," + worldposition3.ToString("F4") + "," + worldposition4.ToString("F4"));

            }


        }
    }



    public Texture2D LoadPNG(string filePath)
    {

        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(5, 5);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        return tex;
    }

}
