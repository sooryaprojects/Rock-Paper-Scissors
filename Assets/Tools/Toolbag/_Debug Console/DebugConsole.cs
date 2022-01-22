using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;
using System.Reflection;

public class DebugConsole : Singleton<DebugConsole>
{
    [SerializeField] KeyCode toggleKey;
    [SerializeField] KeyCode commandKey;
    [SerializeField] KeyCode processCommandKey;
    [SerializeField] KeyCode previousCommandKey;
    [SerializeField] KeyCode completeCommandKey;

    public GUISkin mySkin;
    int previousPointer;
    string commandPrefix;
    List<string> commandParams = new List<string>();

    List<string> commandNames = new List<string>();
    List<System.Action> commandList = new List<System.Action>();
    List<string> commandDesciptions = new List<string>();
    List<string> commandSyntaxes = new List<string>();

    List<string> found = new List<string>();
    List<string> previousCommands = new List<string>();

    GUIStyle style;
    bool showLogs = true;
    bool showErrors = true;
    bool showWarnings = false;
    bool showTrace = false;
    public bool hideAfterExec = true;

    string fileName;
    int startNumber = 1;
    string commandText = "";
    string predictionText = "";


    void Update()
    {
        CalculateFrameRate();
        ProcessInput();
        ProcessShake();
    }

    void CalculateFrameRate()
    {
        m_FpsAccumulator++;
        if (Time.realtimeSinceStartup > m_FpsNextPeriod)
        {
            m_CurrentFps = (int)(m_FpsAccumulator / fpsMeasurePeriod);
            m_FpsAccumulator = 0;
            m_FpsNextPeriod += fpsMeasurePeriod;
            // m_Text.text = string.Format(display, m_CurrentFps);
        }
    }

    void ProcessShake()
    {
        Vector3 acceleration = Input.acceleration;
        lowPassValue = Vector3.Lerp(lowPassValue, acceleration, lowPassFilterFactor);
        Vector3 deltaAcceleration = acceleration - lowPassValue;

        if (deltaAcceleration.sqrMagnitude >= shakeDetectionThreshold && canShake)
        {
            // Perform your "shaking actions" here. If necessary, add suitable
            // guards in the if check above to avoid redundant handling during
            // the same shake (e.g. a minimum refractory period).
            ToggleDebugMode();

            canShake = false;
            Helper.Execute(this, () => canShake = true, 1);
        }
    }

    void ProcessInput()
    {
        if (Input.anyKeyDown)
        {
            if (commandText.Length > 0)
            {
                found = commandNames.FindAll(w => w.StartsWith(commandText));
                if (found.Count > 0)
                {
                    predictionText = found[0];
                }
                else
                {
                    predictionText = "";
                }
            }
            else
            {
                predictionText = "";
            }
        }

        if (Input.GetKeyDown(toggleKey))
        {
            ToggleDebugMode();
        }

        if (Input.GetKeyDown(commandKey))
        {
            commandText = "";
        }

        if (Input.GetKeyDown(previousCommandKey))
        {
            moveToEnd = true;
            if (previousCommands.Count > 0)
            {
                commandText = previousCommands[previousPointer];
            }
            if (previousPointer > 0)
            {
                previousPointer--;
            }
        }

        if (Input.GetKeyDown(completeCommandKey))
        {
            if (predictionText != "")
            {
                moveToEnd = true;
                commandText = predictionText;

                //got to tap twice because OnGUI probably has already run for this frame???
                //flag this?
            }
        }
        if (Input.GetKeyDown(processCommandKey))
        {
            RunCommand();
        }
    }

    public void RunCommand()
    {
        Execute();
        if (hideAfterExec)
        {
            ToggleDebugMode();
        }
    }

    void Execute()
    {
        string[] inputCommands = commandText.Split('-');
        for (int j = 0; j < inputCommands.Length; j++)
        {
            string[] substrings = inputCommands[j].Split(' ');
            commandPrefix = substrings[0];
            commandParams.Clear();
            for (int i = 1; i < substrings.Length; i++)
            {
                commandParams.Add(substrings[i]);
            }
            if (commandNames.Contains(commandPrefix))
            {
                Debug.Log(">...Running: " + commandPrefix);
                commandList[commandNames.IndexOf(commandPrefix)].Invoke();
                previousCommands.Add(commandPrefix);
                if (previousCommands.Count > 10)
                {
                    previousCommands.RemoveAt(0);
                }
                previousPointer = previousCommands.Count - 1;
                if (previousPointer < 0)
                {
                    previousPointer = 0;
                }
            }
            else
            {
                Debug.LogError("> " + commandPrefix + " command not found.");
            }
        }
        commandText = "";
    }

    void ToggleDebugMode()
    {
        show = !show;
        previousPointer = previousCommands.Count - 1;
    }

    struct Log
    {
        public string logMessage;
        public string logStackTrace;
        public LogType logType;

        public Log(string _logMessage, string _logStackTrace, LogType _logType)
        {
            logMessage = _logMessage;
            logStackTrace = _logStackTrace;
            logType = _logType;
        }
    }

    List<Log> logs = new List<Log>();
    Vector2 scrollPosition;
    bool show;
    bool collapse;


    //Colored logs, my dude!
    static readonly Dictionary<LogType, Color> logTypeColors = new Dictionary<LogType, Color>()
        {
            { LogType.Assert, Color.white },
            { LogType.Error, Color.red },
            { LogType.Exception, Color.red },
            { LogType.Log, Color.white },
            { LogType.Warning, Color.yellow },
        };

    Rect windowRect = new Rect(0, 0, UnityEngine.Screen.width, UnityEngine.Screen.height);
    GUIContent clearLabel = new GUIContent("Clear", "Clear the contents of the console.");
    GUIContent collapseLabel = new GUIContent("Collapse", "Hide repeated messages.");
    GUIContent logButton = new GUIContent("Hide logs");
    GUIContent warningButton = new GUIContent("Hide warnings");
    GUIContent errorButton = new GUIContent("Hide errors");
    GUIContent traceButton = new GUIContent("Show trace");
    GUIContent copyButton = new GUIContent("Copy");
    GUIContent screenShotButton = new GUIContent("ScrnSht");
    GUIContent runButton = new GUIContent("  Run  ");

    const float fpsMeasurePeriod = 0.5f;
    private int m_FpsAccumulator = 0;
    private float m_FpsNextPeriod = 0;
    private int m_CurrentFps;
    const string display = "{0} FPS";
    private Text m_Text;
    void Start()
    {
        m_FpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;

        GUIScaler.Initialize();

        AddMarkedMethods();

        lowPassFilterFactor = accelerometerUpdateInterval / lowPassKernelWidthInSeconds;
        shakeDetectionThreshold *= shakeDetectionThreshold;
        lowPassValue = Input.acceleration;
        Input.eatKeyPressOnTextFieldFocus = false;
    }

    // [EasyButtons.Button]
    // public void AllClassMethods()
    // {
    //     Assembly[] referencedAssemblies = System.AppDomain.CurrentDomain.GetAssemblies();
    //     for (int i = 0; i < referencedAssemblies.Length; ++i)
    //     {
    //         System.Type type = referencedAssemblies[i].GetType("Initializer");

    //         if (type != null)
    //         {    // I want all the declared methods from the specific class.
    //             System.Reflection.MethodInfo[] methodInfo = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

    //         }
    //     }

    // }

    public void AddForeignMethods(System.Type type)
    {
        foreach (var methodInfo in type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
        {
            var attrib = methodInfo.GetCustomAttributes(typeof(DebugCommand), true);
            if (attrib.Length > 0)
            {
                Debug.Log(attrib.Length + " " + methodInfo.Name);
                commandNames.Add(methodInfo.Name);

                commandList.Add((System.Action)Delegate.CreateDelegate(typeof(System.Action), type, methodInfo));
                var debugAttrib = (DebugCommand)Attribute.GetCustomAttribute(methodInfo, typeof(DebugCommand));
                commandDesciptions.Add(debugAttrib.desc);
                commandSyntaxes.Add(debugAttrib.syntax);
            }
        }
    }
    void AddMarkedMethods()
    {
        foreach (var methodInfo in this.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
        {
            var attrib = methodInfo.GetCustomAttributes(typeof(DebugCommand), true);
            if (attrib.Length > 0)
            {
                commandNames.Add(methodInfo.Name);
                commandList.Add((System.Action)Delegate.CreateDelegate(typeof(System.Action), this, methodInfo));
                var debugAttrib = (DebugCommand)Attribute.GetCustomAttribute(methodInfo, typeof(DebugCommand));
                commandDesciptions.Add(debugAttrib.desc);
                commandSyntaxes.Add(debugAttrib.syntax);
            }
        }
    }



    public void OnDebugBtnClicked()
    {
        show = !show;
    }

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
        fileName = Application.productName.ToString() + " | ver " + Application.version.ToString();
    }

    void OnDisable()
    {

        Application.logMessageReceived -= HandleLog;
    }

    void OnGUI()
    {
        // GUIScaler.Begin();

        if (!show)
        {
            return;
        }

        windowRect = GUILayout.Window(123456, windowRect, ConsoleWindow, "", mySkin.box);
        // GUIScaler.End();
    }

    /// <summary>
    /// A window that displayss the recorded logs.
    /// </summary>
    /// <param name="windowID">Window ID.</param>

    Matrix4x4 m;
    public float matMux = 2.21f;
    void ConsoleWindow(int windowID)
    {
        //setup new, and ultra sexy GUI skin!👌👌👌

        // float horizRatio = UnityEngine.Screen.width / 1920.0f;
        // float vertRatio = UnityEngine.Screen.height / 1080.0f;

        // GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(horizRatio, vertRatio, 1));
        // m = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(UnityEngine.Screen.width / 1920f * matMux, UnityEngine.Screen.height / 1080f * matMux, 1.0f));
        // GUI.matrix = m

        GUI.skin = mySkin;
        style = new GUIStyle();
        style.fontSize = 25;
        style.wordWrap = true;
        style.normal.textColor = Color.white;

        DrawCLI();
        DrawScrollingConsole();
        DrawMetaData();
        DrawButtons();
        // DrawMethodButtons();
    }

    /// <summary>
    /// Records a log from the log callback.
    /// </summary>
    /// <param name="message">Message.</param>
    /// <param name="stackTrace">Trace of where the message came from.</param>
    /// <param name="type">Type of message (error, exception, warning, assert).</param>
    void HandleLog(string message, string stackTrace, LogType type)
    {
        if (type == LogType.Log)
        {
            if (showLogs)
            {
                Log log;
                log.logMessage = message;
                log.logStackTrace = stackTrace;
                log.logType = type;
                logs.Add(log);

                // {
                //     logMessage = message,
                //     logStackTrace = stackTrace,
                //     logType = type,
                // });
            }
        }
        else if (type == LogType.Warning)
        {
            if (showWarnings)
            {
                Log log;
                log.logMessage = message;
                log.logStackTrace = stackTrace;
                log.logType = type;
                logs.Add(log);
                // logs.Add(new Log()
                // {
                //     logMessage = message,
                //     logStackTrace = stackTrace,
                //     logType = type,
                // });
            }
        }

        else if (type == LogType.Error)
        {
            if (showErrors)
            {
                Log log;
                log.logMessage = message;
                log.logStackTrace = stackTrace;
                log.logType = type;
                logs.Add(log);
                // logs.Add(new Log()
                // {
                //     logMessage = message,
                //     logStackTrace = stackTrace,
                //     logType = type,
                // });
            }
        }
        else
        {
            Log log;
            log.logMessage = message;
            log.logStackTrace = stackTrace;
            log.logType = type;
            logs.Add(log);
            // logs.Add(new Log()
            // {
            //     logMessage = message,
            //     logStackTrace = stackTrace,
            //     logType = type,
            // });
        }

        canScroll = true;
    }

    void DrawMetaData()
    {

        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        GUILayout.Label(string.Format(display, m_CurrentFps), style);
        GUILayout.Label(Application.productName.ToString() + " | ver " + Application.version.ToString(), style);
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();


    }

    private void DrawButtons()
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button(clearLabel, GUILayout.MinWidth(10)))
        {
            logs.Clear();


        }

        if (GUILayout.Button(logButton, GUILayout.MinWidth(10)))
        {
            if (showLogs)
            {
                showLogs = false;
                logButton.text = "Show logs";
            }
            else
            {
                showLogs = true;
                logButton.text = "Hide logs";

            }
        }


        if (GUILayout.Button(warningButton, GUILayout.MinWidth(10)))
        {
            if (showWarnings)
            {
                showWarnings = false;
                warningButton.text = "Show warnings";
            }
            else
            {
                showWarnings = true;
                warningButton.text = "Hide warnings";

            }
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();



        if (GUILayout.Button(errorButton, GUILayout.MinWidth(10)))
        {
            if (showErrors)
            {
                showErrors = false;
                errorButton.text = "Show errors";
            }
            else
            {
                showErrors = true;
                errorButton.text = "Hide errors";

            }
        }

        if (GUILayout.Button(traceButton, GUILayout.MinWidth(10)))
        {
            if (showTrace)
            {
                showTrace = false;
                traceButton.text = "Show trace";
            }
            else
            {
                showTrace = true;
                traceButton.text = "Hide trace";

            }
        }

        if (GUILayout.Button(copyButton, GUILayout.MinWidth(10)))
        {
            // Iterate through the recorded logs.
            GUIUtility.systemCopyBuffer = "";
            for (int i = 0; i < logs.Count; i++)
            {
                if (showTrace)
                {
                    GUIUtility.systemCopyBuffer += logs[i].logMessage + logs[i].logStackTrace + "\n";

                }
                else
                {
                    GUIUtility.systemCopyBuffer += logs[i].logMessage + "\n";

                }
            }
        }

        if (GUILayout.Button(runButton, GUILayout.MinWidth(10)))
        {
            RunCommand();
        }

        //Doesn't fucking work, lmao!
        // if (GUILayout.Button(screenShotButton))
        // {
        //     int number = startNumber;

        //     while (System.IO.File.Exists(fileName + " | "+number+".png"))
        //     {
        //         number++;

        //     }
        //     Debug.Log(fileName);

        //     startNumber = number + 1;

        //     ScreenCapture.CaptureScreenshot(fileName + " | "+number+".png");
        //     Debug.Log("Captured screenshot as: "+fileName + " | "+number+".png");
        //     Debug.Log("Data path: "+Application.persistentDataPath);

        // }
        GUILayout.EndHorizontal();

    }

    void DrawMethodButtons()
    {
        int columns = 5;
        int rows = commandSyntaxes.Count / columns; //2 

        for (int i = 0; i < commandSyntaxes.Count; i++)
        {
            GUILayout.BeginHorizontal();

            for (int j = i; j < rows; j++)
            {
                GUILayout.BeginVertical();
                if (GUILayout.Button(clearLabel, GUILayout.MinWidth(10)))
                {
                    logs.Clear();
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();

        }

    }

    bool canScroll;
    void DrawScrollingConsole()
    {

        GUILayout.BeginHorizontal();
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);


        // Iterate through the recorded logs.
        for (int i = 0; i < logs.Count; i++)
        {
            var log = logs[i];

            // Combine identical messages if collapse option is chosen.
            if (collapse)
            {
                var messageSameAsPrevious = i > 0 && log.logMessage == logs[i - 1].logMessage;

                if (messageSameAsPrevious)
                {
                    continue;
                }
            }

            // GUI.contentColor = logTypeColors[log.logType];
            if (!showTrace)
            {
                GUILayout.Label(log.logMessage, style);
            }
            else
            {
                GUILayout.Label(log.logMessage + log.logStackTrace, style);

            }

        }
        GUILayout.EndScrollView();

        if (canScroll)
        {
            canScroll = false;
            scrollPosition.y = Mathf.Infinity;
        }
        GUI.contentColor = Color.white;

        GUILayout.EndHorizontal();



    }

    TextEditor commandInputField;
    bool moveToEnd;
    void DrawCLI()
    {
        GUI.contentColor = Color.white;
        GUI.SetNextControlName("commandTextField");
        commandText = GUILayout.TextField(commandText, mySkin.textField, GUILayout.MaxHeight(60), GUILayout.MinHeight(40)).ToLower();
        GUI.FocusControl("commandTextField");
        GUI.Label(GUILayoutUtility.GetLastRect(), predictionText, mySkin.GetStyle("consoleLabel"));

        if (moveToEnd)
        {
            moveToEnd = false;
            commandInputField = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
            commandInputField.MoveTextEnd();
        }
    }

    float accelerometerUpdateInterval = 1.0f / 60.0f;
    // The greater the value of LowPassKernelWidthInSeconds, the slower the
    // filtered value will converge towards current input sample (and vice versa).
    float lowPassKernelWidthInSeconds = 1.0f;
    // This next parameter is initialized to 2.0 per Apple's recommendation,
    // or at least according to Brady! ;)
    float shakeDetectionThreshold = 2.0f;

    float lowPassFilterFactor;
    bool canShake = true;
    Vector3 lowPassValue;

    #region Actual Commands 

    [DebugCommand]
    public void help()
    {
        Debug.Log("Help:\n");
        Debug.Log("@ = required parameter");
        Debug.Log("# = optional parameter");
        Debug.Log("\n");

        for (int i = 0; i < commandList.Count; i++)
        {
            if (commandSyntaxes[i].Length > 0 && commandDesciptions[i].Length > 0)
                Debug.Log(commandSyntaxes[i] + "\n" + commandDesciptions[i] + "\n");
        }
        Debug.Log("\n");

    }

    [DebugCommand("fast @<float0> #<float1>", "Increase timescale by <float0> amount. Reset timescale after <float1> delay")]
    public void fast()
    {
        float mux = float.Parse(commandParams[0]);
        float prevTimeScale = Time.timeScale;
        Time.timeScale = Time.timeScale * mux;
        Debug.Log("Timescale: " + Time.timeScale);
        if (commandParams.Count > 1)
        {
            float delay = float.Parse(commandParams[1]);
            Helper.Execute(this, () => Time.timeScale = prevTimeScale, delay * Time.timeScale);
            Helper.Execute(this, () => Debug.Log("Timescale: " + Time.timeScale), delay * Time.timeScale);
            Debug.Log("DELAY: " + delay);

            Debug.Log("Resetting timescale to " + prevTimeScale + " seconds in: " + delay + " seconds");
        }
    }



    [DebugCommand("slow @<float0>", "Decrease timescale by float amount")]
    public void slow()
    {
        float mux = float.Parse(commandParams[0]);
        float prevTimeScale = Time.timeScale;
        Time.timeScale = Time.timeScale * mux;
        Debug.Log("Timescale: " + Time.timeScale);
        if (commandParams.Count > 1)
        {
            float delay = float.Parse(commandParams[1]);
            Helper.Execute(this, () => Time.timeScale = prevTimeScale, delay * Time.timeScale);
            Helper.Execute(this, () => Debug.Log("Timescale: " + Time.timeScale), delay * Time.timeScale);

            Debug.Log("Resetting timescale to " + prevTimeScale + " seconds in: " + delay + " seconds");
        }
    }

    [DebugCommand("speed @<float0>", "Set timescale to float amount")]
    public void speed()
    {
        float param = float.Parse(commandParams[0]);
        Time.timeScale = param;
        Debug.Log("Timescale: " + Time.timeScale);
    }

    [DebugCommand("draw", "Toggle gizmos")]
    public void draw()
    {
        DrawX.shouldDraw = !DrawX.shouldDraw;
    }


    [DebugCommand("reddit", "Opens reddit!")]
    public void reddit()
    {
        Application.OpenURL("http://www.reddit.com");
    }

    [DebugCommand("vfx", "shows particles!")]
    public void vfx()
    {
        //TapGo.EffectsManager.Instance.EnableParticles();
    }

    [DebugCommand("red", "spawn red circles")]
    public void red()
    {
        //TapGo.Data.Instance.cheatData.Red();
    }

    [DebugCommand("normal", "spawn normal circles")]
    public void normal()
    {
        //TapGo.Data.Instance.cheatData.Normal();

    }

    [DebugCommand("dnormal", "spawn double normal circles")]
    public void dnormal()
    {
        //TapGo.Data.Instance.cheatData.DoubleNormal();

    }

    [DebugCommand("gold", "spawn gold circles")]
    public void gold()
    {
        //TapGo.Data.Instance.cheatData.Gold();

    }

    [DebugCommand("bomb", "spawn bomb circles")]
    public void bomb()
    {
        //TapGo.Data.Instance.cheatData.Bomb();

    }

    [DebugCommand("ai", "let ai play")]
    public void ai()
    {
        //TapGo.Data.Instance.cheatData.AIPlay();

    }
    [DebugCommand("s", "set score to 210")]
    public void s()
    {
        //TapGo.Data.Instance.sessionData.SetScore(210);

    }

    #endregion
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class DebugCommand : Attribute
{
    public string desc;
    public string syntax;

    public DebugCommand()
    {
        this.desc = "";
        this.syntax = "";

    }

    public DebugCommand(string syntax, string description)
    {
        this.desc = description;
        this.syntax = syntax;
    }
}



