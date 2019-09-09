//----------------------------------------------
//  F2DEngine: time: 2016.3  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace F2DEngine
{
    public class MyLog : MonoBehaviour
    {
        //日志系统
        public class LogFile: Save_Object
        {
            public int version = 0;//版本号
            public string time = "";//时间
            public List<LogInfo> firstError = new List<LogInfo>();//最新报错
            public List<LogInfo> mAllLog = new List<LogInfo>();//所有日志

        }

        public class LogVersion : Save_Object
        {
            public int version = 0;//最新版本号
        }

        public enum MyLogType
        {
            LT_Normal,//正常
            LT_Hide,//隐藏
        }
        public static MyLog instance;
        //控件适配
        private int mBtHight = 30;
        private int mBtRealHight = 25;
        private int mBtWidget = 100;

        private Rect windowRect = new Rect(20, 20, 150, 100);
        private Rect windowGMRect = new Rect(20, 20, 150, 100);
        private string mInputeText = "gm";
        private string mLastGMText = "";
        private string mGMShowText = "";
        public class LogInfo 
        {
            public string logName;
            public string logStack;
            public LogType logType;
        }

        List<LogInfo> AllLog = new List<LogInfo>();
        List<LogInfo> WarningLog = new List<LogInfo>();
        List<LogInfo> ErrorLog = new List<LogInfo>();

        private class LogUnit
        {
            public string keyName;
            public List<LogInfo> mLogInfos;
            public Rect mRect;
            public Color color;
        }


        List<LogUnit> mLogBufs = new List<LogUnit>();
        private MyLogType mLogType = MyLogType.LT_Normal;
        private LogFile mLogFile = null;
        private LogVersion mLogVersion = null;
        private LogFile mIsShowLogVersion = null;
        //日志
        private LogUnit mSelectLogUnity = null;
        private LogInfo mDetailInfo = null;
        private float mhSbarValue = 0;
        private float mhSbarSize = 0;
        private Vector2 mScrollPosition;
        private List<LogInfo> mPauseLogInfo = null;
        private bool mIsScale = true;
        private Vector2 mMousePos;
        private bool mIsLock = false;
        
        //帧频
        private System.DateTime mDateTime;
        private float mFrames = 1;

        private string mGuiFPSTex = "";
        private string mGuiCPUTex = "";
        private string mGuiMoTex = "";
        private System.TimeSpan mPrevCpuTime = System.TimeSpan.Zero;
        private float mButtonDp = 1.0f;

        private void RegLog(string keyName, List<LogInfo> log, Rect rt,Color c)
        {
            LogUnit lu = new LogUnit();
            lu.keyName = keyName;
            lu.mLogInfos = log;
            lu.mRect = rt;
            lu.color = c;
            mLogBufs.Add(lu);
        }
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                mDateTime = System.DateTime.Now;
               
                //测试
                if (Screen.height > Screen.width)
                {
                    //竖屏
                    mBtHight = Screen.width / 13;
                    mBtRealHight = mBtHight * 4 / 5;
                    mBtWidget = Screen.height / 8;
                    mButtonDp = 0.5f;
                }
                else
                {
                    //横屏
                    mBtHight = Screen.height / 13;
                    mBtRealHight = mBtHight * 4 / 5;
                    mBtWidget = Screen.width / 8;
                    mButtonDp = 1;
                }

                windowRect = new Rect(5, 5, mBtWidget+10, (mBtRealHight+20)*3);
                windowGMRect = new Rect(Screen.width - mBtWidget - 15, 15 + mBtHight * 3, mBtWidget + 10, (mBtRealHight) * 5.0f);
                RegLog("所有日志", AllLog, new Rect(10, 10, mBtWidget, mBtRealHight), Color.white);
                RegLog("警告日志", WarningLog, new Rect(10, 10 + mBtHight, mBtWidget, mBtRealHight), Color.yellow);
                RegLog("错误日志", ErrorLog, new Rect(10, 10 + mBtHight * 2, mBtWidget, mBtRealHight), Color.red);
                Application.logMessageReceived += _OnDebugLogCallbackHandler;
                GM.GMPlay();
            }
            
        }

        public Color GetColor(LogType type)
        {
            if (type == LogType.Warning)
                return Color.yellow;
            else if (type == LogType.Error || type == LogType.Exception)
                return Color.red;
            else
            {
                return Color.white;
            }
        }


        private void _OnDebugLogCallbackHandler(string name, string stack, LogType type)
        {
            LogInfo logInfo = new LogInfo() { logName = name, logType = type, logStack = stack };
            AllLog.Add(logInfo);
            if (type == LogType.Warning)
            {
                WarningLog.Add(logInfo);
            }
            else if(type == LogType.Error||type == LogType.Exception)
            {
                ErrorLog.Add(logInfo);
                if(mLogType == MyLogType.LT_Hide)
                {
                    mLogType = MyLogType.LT_Normal;
                    mIsScale = false;
                    mDetailInfo = logInfo;                 
                }

                //错误日志记录
                if(mLogVersion == null)
                {
                    mLogVersion = new LogVersion();
                    mLogVersion.ReadFile();
                    mLogVersion.version++;
                    mLogVersion.SaveFile();

                    mLogFile = new LogFile();
                    mLogFile.ReadFile(_GetLogPath(mLogVersion.version));
                    mLogFile.version = mLogVersion.version;
                    mLogFile.firstError.Clear();
                    mLogFile.time = System.DateTime.Now.ToString("MM月dd日-HH:mm:ss");
                    mLogFile.mAllLog = null;
                }

                if(mLogFile.firstError.Count <10)
                {
                    mLogFile.firstError.Add(logInfo);
                    mLogFile.mAllLog = AllLog;
                    mLogFile.SaveFile();
                }
            }
        }


        private string _GetLogPath(int ver)
        {
            return "Log/" + ver.ToString();
        }

        public static MyLog SetLog(MyLogType type = MyLogType.LT_Normal)
        {
            if (instance == null)
            {
                GameObject log = new GameObject();
                log.name = "MyLog";
                MyLog ml = log.AddComponent<MyLog>();
                ml.mLogType = type;
                DontDestroyOnLoad(log);
                return ml;
            }
            else
            {
                instance.mLogType = type;
            }
            return instance;
        }


        private void ShowFps()
        {
            ++mFrames;
            double timeDp = (System.DateTime.Now - mDateTime).TotalSeconds;
            if (timeDp > 0.5f)
            {
                System.Diagnostics.Process pro = System.Diagnostics.Process.GetCurrentProcess();
                int values = (int)((pro.TotalProcessorTime - mPrevCpuTime).TotalMilliseconds / (timeDp * 1000) / System.Environment.ProcessorCount * 100);
                values = Mathf.Clamp(values,0,100);
                mGuiFPSTex = "FPS: " + (mFrames / timeDp).ToString("f2");
                mGuiMoTex =  "内存: " + (pro.WorkingSet64 / (float)(1024 * 1024)).ToString("f2")+"M";       
                mGuiCPUTex = "CPU: " + values.ToString("f2")+"%";
                mPrevCpuTime = pro.TotalProcessorTime;
                mDateTime = System.DateTime.Now;
                mFrames = 0;
            }  
        }
      
        private void ScrollViewEvent()
        {
            //滑动事件
            if (Input.GetMouseButtonDown(0))
            {
                mIsLock = true;
                mMousePos = Input.mousePosition;
            }
            else if (Input.GetMouseButton(0))
            {
                if (mIsLock)
                {
                    if (mSelectLogUnity != null || mDetailInfo != null)
                    {
                        if (Input.mousePosition.x > mBtWidget + 20 && Input.mousePosition.y > 10 + mBtHight && Input.mousePosition.x < mBtWidget * 5.5*mButtonDp + 20 && Input.mousePosition.y < mBtHight * 11/mButtonDp+ 10)
                        {
                            mhSbarValue += (Input.mousePosition.y - mMousePos.y) / (mBtHight * 10);
                            mScrollPosition.y += (Input.mousePosition.y - mMousePos.y);
                            if (mScrollPosition.y < 0)
                            {
                                mScrollPosition.y = 0;
                            }
                            if (mhSbarValue < 0)
                            {
                                mhSbarValue = 0;
                            }
                        }
                    }
                    mMousePos = Input.mousePosition;
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                mIsLock = false;
            }
        }


        void Update()
        {
            //滑动事件
            ScrollViewEvent();
            //显示FPS
            ShowFps();
        }

        void DoMyWindow(int windowID)
        {
            int RealHight = mBtRealHight;
            GUI.contentColor = Color.white;
            GUI.DragWindow(new Rect(0, 0, mBtWidget, RealHight * 2 + 20));

            GUI.skin.box.alignment = TextAnchor.MiddleLeft;
            GUI.skin.button.fontSize = mBtWidget / 7;
            GUI.skin.label.fontSize = mBtWidget / 6;
            GUI.skin.box.fontSize = mBtWidget / 7;
            //GUILayout.Label
            GUI.Box(new Rect(5, 20, mBtWidget, RealHight), mGuiFPSTex);
            //GUI.Box(new Rect(5, 20 + mBtHight, mBtWidget, RealHight), mGuiCPUTex);
            //GUI.Box(new Rect(5, 20 + mBtHight * 2, mBtWidget, RealHight), mGuiMoTex);
         
            for (int i = 0; i < mLogBufs.Count; i++)
            {
                LogUnit lu = mLogBufs[i];
                GUI.contentColor = lu.color;
                GUI.Label(new Rect(5+ mBtWidget / 3 * i, 20 + mBtHight, mBtWidget,RealHight), "(" + lu.mLogInfos.Count.ToString("00") + ")");    
            }
            GUI.contentColor = Color.white;
            if (GUI.Button(new Rect(5, 20 + mBtHight * 2, mBtWidget/2, RealHight), "关闭"))
            {
                CloseLog();
            }
            if (GUI.Button(new Rect(5+ mBtWidget / 2, 20 + mBtHight * 2, mBtWidget/2, RealHight), "展开"))
            {
                mIsScale = false;
            }
        }


        public static void CloseLog()
        {
            if (instance != null)
            {
                Application.logMessageReceived -= instance._OnDebugLogCallbackHandler;
                GameObject.Destroy(instance.gameObject);
            }
        }

        void OnGUI()
        {
            if(mLogType == MyLogType.LT_Hide)
            {
                ShowHide();
            }
            else if(mIsScale)
            {
                GUI.contentColor = Color.white;
                windowRect = GUI.Window(0, windowRect, DoMyWindow, "日志");
            }
            else
            {
                OnGUIEx();
                if (mIsShowLogVersion == null)
                { 
                    GUI.contentColor = Color.white;               
                    windowGMRect = GUI.Window(0, windowGMRect, DoGMWindow, "GM窗口");
                    ShowGM();
                }
            }

            ShowVirsion();
        }

        void ShowVirsion()
        {
            if (mIsShowLogVersion != null)
            {
                GUI.skin.button.fontSize = mBtWidget / 10;
                if (GUI.Button(new Rect(5, Screen.height - mBtRealHight - 10, mBtWidget, mBtRealHight), mIsShowLogVersion.time + "\n错误 Version:" + mIsShowLogVersion.version.ToString()))
                {
                    AllLog.Clear();
                    WarningLog.Clear();
                    ErrorLog.Clear();
                    mIsShowLogVersion = null;
                }
            }
        }

        void ShowHide()
        {
            int minValue = Mathf.Min(mBtRealHight, mBtWidget);
            GUI.skin.box.alignment = TextAnchor.MiddleCenter;
            GUI.skin.box.fontSize = minValue /3;
            GUI.Box(new Rect(5,5, minValue/2.5f, minValue/2.5f),"F");
        }

        void ShowGM()
        {
            if(mGMShowText != "")
            {
                GUI.skin.box.alignment = TextAnchor.UpperLeft;
                int size = (int)(GUI.skin.box.fontSize * 1.3f);
                GUI.Box(new Rect(windowGMRect.x- windowGMRect.width-20-size*5, windowGMRect.y, windowGMRect.width+ size*5, size * (GM.GetShow().Count+2)), mGMShowText);
            }
        }

        void ShowListLog(int index)
        {
            mSelectLogUnity = mLogBufs[index];
            mhSbarValue = 0;
            mhSbarSize = 0;
            mDetailInfo = null;
            mPauseLogInfo = null;
            mScrollPosition = Vector2.zero;
        }

        void DoGMWindow(int windowID)
        {
            int RealHight = mBtRealHight;
            GUI.contentColor = Color.white;
            GUI.DragWindow(new Rect(0, 0, mBtWidget, RealHight*1));

            GUI.skin.box.alignment = TextAnchor.MiddleLeft;
            GUI.skin.button.fontSize = mBtWidget / 7;
            GUI.skin.label.fontSize = mBtWidget / 7;
            GUI.skin.box.fontSize = mBtWidget / 7;
            GUI.skin.textField.fontSize = mBtWidget / 7;
            GUI.skin.textField.alignment = TextAnchor.MiddleLeft;
            //GUI.Box(new Rect(5, 20, mBtWidget, RealHight), mGuiFPSTex);

            GUI.Label(new Rect(5, mBtHight*0.4f, (int)(mBtWidget), RealHight), "输入指令:");
            //输入框
            if (mLastGMText == "")
            {  
                GUI.Label(new Rect(5, mBtHight, (int)(mBtWidget), RealHight), "examples:gm");
            }
            else
            {
                if (GUI.Button(new Rect(5, mBtHight, mBtWidget, RealHight), mLastGMText))
                {
                    GM.ResetGM("gm/" + mInputeText);
                }
            }
            mInputeText = GUI.TextField(new Rect(5, mBtHight*2, (int)(mBtWidget), RealHight), mInputeText);
            if (GUI.Button(new Rect(5 + mBtWidget/2/2.7f, mBtHight*3.0f, mBtWidget / 1.5f, RealHight), "确定"))
            {
                if (mInputeText != "")
                {
                    if (GM.ResetGM("gm/" + mInputeText) == GM.GMResult.GM_Success)
                    {
                        mLastGMText = mInputeText;
                        mInputeText = "";
                    }
                    else
                    {
                        string gm = mInputeText.ToLower();
                        if (gm == "gm")
                        {
                            if (mGMShowText == "")
                            {
                                var gmshwo = GM.GetShow();
                                mGMShowText = "GM指令大全:\nlog/版本号\n";
                                foreach (var k in gmshwo)
                                {
                                    mGMShowText += k.Key + ":" + k.Value + "\n";
                                }                            
                            }
                            else
                            {
                                mGMShowText = "";
                                mInputeText = "";
                            }
                            
                        }
                        else if(gm.Contains("log"))
                        {
                            if(gm.Length <= 3)
                            {
                                AllLog.Clear();
                                WarningLog.Clear();
                                ErrorLog.Clear();
                                LogVersion ver = new LogVersion();
                                ver.ReadFile();
                                for(int i = ver.version; i > 0;i--)
                                {
                                    LogFile log = new LogFile();
                                    if (log.ReadFile(_GetLogPath(i)))
                                    {
                                        if (log.firstError.Count > 0)
                                        {
                                            LogInfo logInfo = new LogInfo();
                                            logInfo.logType = LogType.Error;
                                            logInfo.logName = "version:" + log.version.ToString()+"报错:" + log.time;
                                            logInfo.logStack = log.firstError[0].logStack;
                                            ErrorLog.Add(logInfo);
                                        }
                                    }
                                }
                                LogFile lf = new LogFile();
                                lf.time = "版本日志";
                                lf.version = ver.version;
                                mIsShowLogVersion = lf;
                                ShowListLog(2);
                            }
                            else
                            {
                                string subGm = gm.Substring("log/".Length);
                                int vs = 0;
                                int.TryParse(subGm, out vs);
                                if (vs == 0)
                                {
                                    LogVersion ver = new LogVersion();
                                    ver.ReadFile();
                                    vs = ver.version;
                                }
                                LogFile log = new LogFile();
                                if (log.ReadFile(_GetLogPath(vs)))
                                {
                                    AllLog.Clear();
                                    WarningLog.Clear();
                                    ErrorLog.Clear();
                                    AllLog.AddRange(log.mAllLog);
                                    ErrorLog.AddRange(log.firstError);
                                }
                                mIsShowLogVersion = log;
                            }
                        }
                    }
                }
            }
        }

        void OnGUIEx()
        {
            
            GUI.skin.box.alignment = TextAnchor.MiddleLeft;
            GUI.skin.button.fontSize = mBtWidget / 7;
            GUI.skin.label.fontSize = mBtWidget / 6;
            GUI.skin.box.fontSize = mBtWidget / 7;
            //GUILayout.Label()
            GUI.Box(new Rect(Screen.width - mBtWidget - 10, 10, mBtWidget, mBtRealHight), mGuiFPSTex);
            GUI.Box(new Rect(Screen.width - mBtWidget - 10, 10+mBtHight, mBtWidget, mBtRealHight), mGuiCPUTex);
            GUI.Box(new Rect(Screen.width - mBtWidget - 10, 10 + mBtHight*2, mBtWidget, mBtRealHight), mGuiMoTex);
            if(mIsScale)
            {
                if (GUI.Button(new Rect(10, 10, mBtWidget, mBtRealHight), "展开(↓↓)"))
                {
                    mIsScale = false;
                }
                return;
            }
            if (GUI.Button(new Rect(10, 10 + mBtHight * 3, mBtWidget, mBtRealHight), "收起(↑↑)"))
            {
                mIsScale = true;
            }


            GUI.contentColor = Color.white;
            for (int i = 0; i < mLogBufs.Count; i++)
            {
                LogUnit lu = mLogBufs[i];
                GUI.contentColor = lu.color;
                if (GUI.Button(lu.mRect, lu.keyName + "(" + lu.mLogInfos.Count + ")"))
                {
                    mSelectLogUnity = lu;
                    mhSbarValue = 0;
                    mhSbarSize = 0;
                    mDetailInfo = null;
                    mPauseLogInfo = null;
                    mScrollPosition = Vector2.zero;
                }
            }

            if (mDetailInfo != null)
            {
                GUI.Box(new Rect(mBtWidget + 20, 10 + mBtHight, (int)(mBtWidget * 4.5* mButtonDp),mBtHight*10), "");
                GUILayout.BeginArea(new Rect(mBtWidget + 20, 10 + mBtHight, (int)(mBtWidget * 4.5* mButtonDp), mBtHight * 10));
                mScrollPosition = GUILayout.BeginScrollView(mScrollPosition, GUILayout.Width((int)(mBtWidget * 4.5* mButtonDp)), GUILayout.Height(mBtHight * 10));
                GUI.contentColor = GetColor(mDetailInfo.logType);
                GUILayout.Label("打印信息: \n"+ mDetailInfo.logName+"\n\n堆栈信息:\n"+mDetailInfo.logStack);
                GUILayout.EndScrollView();
                GUILayout.EndArea();
                GUI.contentColor = GetColor(mDetailInfo.logType);
                if (GUI.Button(new Rect(mBtWidget + 20, 10, (int)(mBtWidget * 4.5* mButtonDp), mBtRealHight), mDetailInfo.logName))
                {
                    mDetailInfo = null;
                }
            }
            else if (mSelectLogUnity != null)
            {
                List<LogInfo> infos = mPauseLogInfo != null ? mPauseLogInfo : mSelectLogUnity.mLogInfos;
                int logCout = infos.Count;
            
                int curYe = 0;
                if (logCout > 10)
                {
                    float bb = mhSbarValue / (1 - mhSbarSize); ;
                    mhSbarSize = 10 / (float)logCout;
                    curYe = (int)((logCout - 10) / (1 - mhSbarSize) * mhSbarValue);
                    mhSbarValue = (1 - mhSbarSize) * bb;
                    mhSbarValue = GUI.VerticalScrollbar(new Rect((int)(mBtWidget * 5.5* mButtonDp) +35, 10+mBtHight, mBtWidget/4, mBtHight*10-(mBtHight - mBtRealHight)), mhSbarValue, mhSbarSize, 0.0f, 1.0f);
                    
                }
       
                int index = 0;
                for (int i = curYe; i < infos.Count && i < curYe + 10; i++)
                {
                    LogInfo li = infos[i];
                    GUI.contentColor = GetColor(li.logType);
                    if (GUI.Button(new Rect(mBtWidget + 20, 10 + mBtHight + mBtHight * index++, (int)(mBtWidget * 4.5* mButtonDp), mBtRealHight), li.logName))
                    {
                        mDetailInfo = li;
                    }
                }

                 GUI.contentColor = Color.white;
                 if (mPauseLogInfo != null)
                 {
                      GUI.contentColor = Color.blue;
                      if (GUI.Button(new Rect(mBtWidget + 20, 10, mBtWidget * 2* mButtonDp, mBtRealHight), "开启日志"))
                      {
                          mPauseLogInfo = null;
                      }
                 }
                 else if (GUI.Button(new Rect(mBtWidget + 20, 10, mBtWidget * 2* mButtonDp, mBtRealHight), "暂停日志"))
                 {
                     mPauseLogInfo = new List<LogInfo>(mSelectLogUnity.mLogInfos.ToArray());
                 }
                 GUI.contentColor = Color.red;
                 if (GUI.Button(new Rect(mBtWidget * 2* mButtonDp+mBtWidget + 40, 10, mBtWidget * 2* mButtonDp, mBtRealHight), "清空日志"))
                 {
                     mSelectLogUnity.mLogInfos.Clear();
                     mPauseLogInfo = null;
                 }
                
            }
        }
    }
}
