using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using F2DEngine;
using UnityEditor;
using System.Reflection;

public class ArpgStateEditor : EditorWindow
{
    private Rect mScreenRect;
    private FArpgMachine mFArpgMachine;
    private Event mEvent;
    private FArpgMachine.FArpgNode mDragNode;
    private float mDragLen = 0;
    private FArpgMachine.FArpgNode mSelectNode;
    private FArpgMachine.FArpgNode mEditorNode;
    private FArpgMachine.FArpgNode mTempNode;
    private FArpgMachine.FArpgNode mHeadNode;
    private EventType mCurEventType;
    private KeyCode mKeyCode;
    private int mEventButton = 0;
    private Dictionary<string, System.Type> mStateType = new Dictionary<string, System.Type>();
    private List<string> mStateTypeName = new List<string>();
    private System.Type mCurSelectType = null;
    private string mAutoPath = "";
    
    [MenuItem("FEngine/其他/编辑器/状态机(FSM)编辑")]
    public static void Init()
    {
        GetWindow<ArpgStateEditor>().InitData();
    }

    private void RegionsStateType(System.Type type,string decName)
    {
        mStateType[type.FullName] = type;
        mStateTypeName.Add(decName);
    }

    private void Regions()
    {
        //注册类
        RegionsStateType(typeof(FArpgMachine.FArgpBaseData),"基本状态机");

        RegionsStateType(typeof(ArpgActionState.PlayerArpgData),"动画状态机");
        
    }

    private void InitData()
    {
        Regions();
        mFArpgMachine = new FArpgMachine();
    }

    void OnGUI()
    {
        //场景通用
        mScreenRect = new Rect(0, 0, Screen.width, Screen.height);
        mEvent = Event.current;
        mCurEventType = mEvent.type;
        mEventButton = mEvent.button;
        mKeyCode = mEvent.keyCode;



        //拖动文件
        DragFile();

        if (!SelectState() || mFArpgMachine == null)
        {
            return;
        }

        //画背景格子
        DrawBackGround();

        //操作需求
        SelectOperation();

        //画节点
        DrawNode();

        //显示节点数据
        ShowNodeData();
   
    }
    
    private void DragFile()
    {
        var drag = DragAndDrop.objectReferences;
        if (drag.Length == 1)
        {
            DragAndDrop.AcceptDrag();
            var ass = Assembly.Load("Assembly-CSharp");
            System.Type type = ass.GetType(drag[0].name);
            if (type != null)
            {
                if (type.IsSubclassOf(typeof(FArpgMachine.FArgpBaseData)))
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                    if (Event.current.type == EventType.DragExited)
                    {
                        mCurSelectType = type;
                        Clear();
                        mFArpgMachine = new FArpgMachine();
                        Repaint();
                    }
                }
            }
            else if (DragAndDrop.paths[0].IndexOf("." + ResConfig.ARPGEX) != -1)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                if (Event.current.type == EventType.DragExited)
                {
                    LoadFile(DragAndDrop.paths[0]);
                    Repaint();
                }
            }
        }
    }  

    private bool SelectState()
    {
        if (mCurSelectType == null)
        {
            int index = 0;
            Rect rect = new Rect(0, 0, 100, 30);

            GUIStyle guiStyle = new GUIStyle();
            guiStyle.normal.textColor = new Color(0,0.6f,0, 0.8f);
            guiStyle.fontSize = 35;
           // GUI.skin.box.fontSize = 40;
            GUI.Box(mScreenRect, "请选择状态机类型(或者拖拽状态类型脚本到编辑器里)", guiStyle);
            foreach (var k in mStateType)
            {
                Rect tempRect = rect;
                int tempX = index % 3;
                int tempY = index / 3 + 1;
                tempRect.center += new Vector2(100 * tempX, 60 * tempY);

                if (GUI.Button(tempRect, mStateTypeName[index]))
                {
                    mCurSelectType = k.Value;
                }
                index++;
            }
            return false;
        }
        return true;
    }

    private void ShowNodeData()
    {
        Color lastColor = GUI.backgroundColor;
        GUI.backgroundColor = new Color(1,1,1,1);

        GUI.Label(new Rect(200, 0, mScreenRect.width, mScreenRect.height),"1.按住【Shift】右键拖动,设置跳转节点(无视判断,直接跳转)\n2.按【S】设置初始节点\n3【Delete】删除节点");

        if (mAutoPath != "")
        {
            GUIStyle guiStyle = new GUIStyle();
            guiStyle.normal.textColor = new Color(0.4f, 0.4f, 0.6f, 0.8f);
            GUI.Label(new Rect(200,mScreenRect.height - 40,mScreenRect.width,200), "当前加载文件: " + mAutoPath.Replace(Application.dataPath,""), guiStyle);
        }

        GUI.Box(new Rect(0, 0, 200, mScreenRect.height),"");
        if (mTempNode != null)
        {
            Rect rect = mEditorNode.mRect;
            rect.center -= new Vector2(10, 10)/2;
            rect.width += 10;
            rect.height += 10;
            GUI.Box(rect, "");

            ////////////////////////////////////////////////////////
            var argp = mTempNode.mFArgpBaseData;

            int ext = 0;
            if (argp.ConditionCallBack != null)
            {
                ext += argp.ConditionCallBack.Length;
            }
            if (argp.PlayStateCallBacks != null)
            {
                ext += argp.PlayStateCallBacks.Length;
            }


            GUI.backgroundColor = Color.green;
            GUI.Box(new Rect(0, 0, 200,180 + ext * 18), "");

            var data = mTempNode.mFArgpBaseData;

            GUILayout.BeginArea(new Rect(0,0,190,mScreenRect.height));
     

            GUIStyle guiStyle = new GUIStyle();
            guiStyle.normal.textColor = new Color(0,0.8f,0,0.8f);
            EditorGUILayout.LabelField("【节点名称】",guiStyle);
            argp.nodeName = EditorGUILayout.TextField(argp.nodeName);
            EditorGUILayout.LabelField("【状态字符变量】", guiStyle);
            argp.conditionState = EditorGUILayout.TextField(argp.conditionState);
            EditorGUILayout.LabelField("【状态时间变量】", guiStyle);
            argp.conditionTimes = EditorGUILayout.FloatField(argp.conditionTimes);


            EditorGUILayout.LabelField("【条件判断事件】", guiStyle);
            List<System.Func<FArpgMachine.FArpgNode, FArpgMachine.FArpgNode, bool>> tempCallBack = new List<System.Func<FArpgMachine.FArpgNode, FArpgMachine.FArpgNode, bool>>();
            int cout = 0;
            if(argp.ConditionCallBack != null)
            {
                cout = argp.ConditionCallBack.Length;
            }
            for (int i = 0; i < cout+1; i++)
            {
                System.Func<FArpgMachine.FArpgNode, FArpgMachine.FArpgNode, bool> callBack = null;
                if (i < cout)
                { 
                    callBack = argp.ConditionCallBack[i];
                }
                var callEvent = (System.Func<FArpgMachine.FArpgNode, FArpgMachine.FArpgNode, bool>)MyEdior.DrawDelegate(argp, callBack, mCurSelectType, typeof(System.Func<FArpgMachine.FArpgNode, FArpgMachine.FArpgNode, bool>), typeof(bool), typeof(FArpgMachine.FArpgNode), typeof(FArpgMachine.FArpgNode));
                if(callEvent != null)
                {
                    tempCallBack.Add(callEvent);
                }
            }
            argp.ConditionCallBack = tempCallBack.ToArray();



            EditorGUILayout.LabelField("【状态执行事件】", guiStyle);
            List<System.Action<FArpgMachine.FArpgNode, float, int>> plays = new List<System.Action<FArpgMachine.FArpgNode, float, int>>();
            cout = 0;
            if (argp.PlayStateCallBacks != null)
            {
                cout = argp.PlayStateCallBacks.Length;
            }
            for (int i = 0; i < cout + 1; i++)
            {
                System.Action<FArpgMachine.FArpgNode, float, int> callBack = null;
                if (i < cout)
                {
                    callBack = argp.PlayStateCallBacks[i];
                }
                var callEvent = (System.Action<FArpgMachine.FArpgNode, float, int>)MyEdior.DrawDelegate(argp, callBack, mCurSelectType, typeof(System.Action<FArpgMachine.FArpgNode, float, int>), typeof(void), typeof(FArpgMachine.FArpgNode),typeof(float),typeof(int));
                if (callEvent != null)
                {
                    plays.Add(callEvent);
                }
            }
            argp.PlayStateCallBacks = plays.ToArray();


            GUI.backgroundColor = Color.white;

            ////////////////////////////////////////////////////////////
            //绘制扩展类
            EditorGUILayout.LabelField("—————————————————");
            EditorGUILayout.LabelField("——————自定义数据——————");
            EditorGUILayout.LabelField("—————————————————");
            GUILayout.Space(20);

            MyEdior.DrawConverter.DrawObject(data,null);
            GUILayout.EndArea();
        }
        GUI.backgroundColor = lastColor;
    }

    private void KeepNode()
    {
        if (mEditorNode == null)
            return;
        var newData = mEditorNode.mFArgpBaseData;
        var tempData = mTempNode.mFArgpBaseData;

        if (newData.nodeName != tempData.nodeName)
        {
            if (GetIsSameNodeName(tempData.nodeName))
            {
                ShowNotification(new GUIContent("节点名重名保存失败"));
                return;
            }
        }
        var root = mFArpgMachine.GetRoots();
        root.mMainBuffs.Remove(newData.nodeName);
        mEditorNode.mFArgpBaseData = tempData;
        root.mMainBuffs[tempData.nodeName] = mEditorNode;
        mTempNode.mFArgpBaseData = tempData.GetCloneInistane();
    }

    private void SelectOperation()
    {
        //Debug.LogError(mCurEventType+"---------"+mSelectNode);
        //右键菜单
        if (mCurEventType == EventType.MouseDown&&mEventButton == 1)
        {
            var mousePos = mEvent.mousePosition;
            var selectNode = GetNode(mousePos);
            if (selectNode != null)
            {
                mSelectNode = selectNode;
            }
            else
            {
                if (mScreenRect.Contains(mousePos))
                {
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent("添加节点"), false, CreateNode, mousePos);
                    menu.ShowAsContext();
                }
            }
        }
        else if (mCurEventType == EventType.MouseUp&&mEventButton == 1)
        {
            if (mSelectNode != null)
            {
                var tempNode = GetNode(mEvent.mousePosition);
                if(tempNode != mSelectNode)
                {
                    if (mEvent.shift)
                    {
                        if (mSelectNode.mSkipNode == tempNode)
                        {
                            mSelectNode.mSkipNode = null;
                        }
                        else
                        {
                            mSelectNode.mSkipNode = tempNode;
                        }
                    }
                    else
                    {
                        mSelectNode.Regs(tempNode, true);
                    }
                }
                mSelectNode = null;
            }
        }
        else if (mSelectNode != null)
        {
            Color lastColor = Handles.color;
            Handles.color = mEvent.shift?Color.red:Color.green;
            Handles.DrawAAPolyLine(5, mSelectNode.mRect.center, mEvent.mousePosition);
            Handles.color = lastColor;
        }


        //左键键按下
        if (mCurEventType == EventType.MouseDown&& mEventButton == 0)
        {
            if(GUIUtility.keyboardControl != 0)
            {
                KeepNode();
            }
            GUIUtility.keyboardControl = 0;
            mDragNode = GetNode(mEvent.mousePosition);
        }
        else if(mDragNode!= null&&mCurEventType == EventType.MouseDrag)
        {
            mDragNode.mRect.center += mEvent.delta;
            mDragLen += Vector2.Distance(Vector2.zero, mEvent.delta);
        }
        else if(mCurEventType == EventType.MouseUp && mEventButton == 0)
        {
            if(mDragLen>0.3f)
            {
                Event.current.Use();
            }
            mDragLen = 0;
            mDragNode = null;
        }

        //按键操作

        //删除节点操作
        if(mKeyCode == KeyCode.Delete)
        {
            if (mEditorNode != null)
            {
                mFArpgMachine.RemoveNode(mEditorNode);
                if(mHeadNode == mEditorNode)
                {
                    mHeadNode = null;
                }
                Clear();
                Repaint();
            }
        }
        else if(mKeyCode == KeyCode.S)
        {
            if(mEditorNode != null)
            {
                mHeadNode = mEditorNode;
                Repaint();
            }
        }

        //按钮
       
        if (GUI.Button(new Rect(mScreenRect.width -100,0,100,20),"另存文件"))
        {
            string path = MyEdior.SaveFilePanel(ResConfig.ARPGEX);
            if (path == "")
                return;
            mAutoPath = path;
            KeepFile(path);
        }


        if (GUI.Button(new Rect(mScreenRect.width-100, 22, 100, 20),"读取文件"))
        {
           
            string path = MyEdior.OpenFilePanel(ResConfig.ARPGEX);
            if (path == "")
                return;
            LoadFile(path);
        }

        if (mAutoPath != "")
        {
            if (GUI.Button(new Rect(mScreenRect.width - 100,44, 100, 20), "保存文件"))
            {
                KeepFile(mAutoPath);
            }
        }

    }

    private void LoadFile(string path)
    {
        mAutoPath = path;
        Clear();
        string tempPath = "." + ResConfig.ARPGEX;
        if (!path.EndsWith(tempPath))
        {
            path += tempPath;
        }
        var config = mFArpgMachine.LoadFile(path, FFilePath.FP_Abs);
        var ass = Assembly.Load("Assembly-CSharp");
        System.Type type = ass.GetType(config);
        mCurSelectType = type;//mStateType[config.type];
        mHeadNode = mFArpgMachine.GetCurNode();
    }

    private void KeepFile(string path)
    {
        if (mHeadNode == null)
            return;

        if (mHeadNode != null)
        {
            mFArpgMachine.mStartNodeName = mHeadNode.mFArgpBaseData.nodeName;
        }
        else
        {
            mFArpgMachine.mStartNodeName = "";
        }

        string tempPath = "." + ResConfig.ARPGEX;
        if (!path.EndsWith(tempPath))
        {
            path += tempPath;
        }

        mFArpgMachine.SaveFile(path, mCurSelectType);
    }

    private FArpgMachine.FArpgNode GetNode(Vector2 mousePos)
    {
        var startNode = mFArpgMachine.GetRoots();
        foreach (var key in startNode.mMainBuffs)
        {
            if (key.Value.mRect.Contains(mousePos))
            {
                return key.Value;
            }
        }
        return null;
    }

    private void DrawNode()
    {
       
        var root = mFArpgMachine.GetRoots();
        //画线
        List<FArpgMachine.FArpgNode> overNode = new List<FArpgMachine.FArpgNode>();
        Color lastColor = Handles.color;
        Handles.color = Color.white;
        _DrawNodeLine(root, overNode, false);
        Handles.color = lastColor;


        //画节点
        Color oldColor = GUI.backgroundColor;
        
        foreach (var k in root.mMainBuffs)
        {
            var node = k.Value;
            GUI.backgroundColor = new Color(1, 0, 0, 1);
            if(mHeadNode == null)
            {
                mHeadNode = node;
            }

            if (mHeadNode == node)
            {
                GUI.backgroundColor = new Color(0,1,0,1);
            }

            if (GUI.Button(node.mRect, k.Key))
            {
                mEditorNode = node;
                mTempNode = new FArpgMachine.FArpgNode(null,mFArpgMachine);
                mTempNode.mFArgpBaseData = node.mFArgpBaseData.GetCloneInistane();
            }
        }
        GUI.backgroundColor = oldColor;

       
    }


    private void DrawTowNode(FArpgMachine.FArpgNode start, FArpgMachine.FArpgNode end)
    {
        float width = 2;
        var nextNode = end;
        Vector2 offse = Vector2.zero;
        if (nextNode.mNextData.Contains(start))
        {
            Vector2 tempDir = start.mRect.center - nextNode.mRect.center;
            tempDir.Normalize();
            tempDir = Quaternion.Euler(0, 0, 90) * tempDir;
            offse += tempDir * 10;
        }

        Vector2 startPoint = start.mRect.center + offse;
        Vector2 endPoint = nextNode.mRect.center + offse;
        //画线
        Handles.DrawAAPolyLine(width, startPoint, endPoint);
        //画小三角
        Vector3 dir = startPoint - endPoint;
        dir.Normalize();
        Vector2[] buffs = new Vector2[2];
        float des = Vector2.Distance(startPoint, endPoint) / 2;
        Vector3 tempVector3 = startPoint;
        tempVector3 -= dir * des;
        buffs[0] = tempVector3 + Quaternion.Euler(0, 0, 45) * dir * 10;
        buffs[1] = tempVector3 + Quaternion.Euler(0, 0, -45) * dir * 10;
        Handles.DrawAAPolyLine(width, buffs[0], tempVector3, buffs[1]);
    }

    private void _DrawNodeLine(FArpgMachine.FArpgNode node,List<FArpgMachine.FArpgNode> overNode,bool isDrawLine = false)
    {
        if(overNode.Contains(node))
        {
            return;
        }
        overNode.Add(node);
        
        if(node.mSkipNode != null)
        {
            Handles.color = Color.red;
            DrawTowNode(node, node.mSkipNode);
        }

        for (int i = 0; i < node.mNextData.Count;i++)
        {
            if(isDrawLine)
            {
                Handles.color = node.mSkipNode == null ? Color.white : new Color(0.6f, 0.6f, 0.6f, 0.8f);
                DrawTowNode(node, node.mNextData[i]);
            }
            
            _DrawNodeLine(node.mNextData[i],overNode,true);
        }
    }


    private void CreateNode(object obj)
    {
        Vector2 mousePos = (Vector2)obj;
        var startNode = mFArpgMachine.GetRoots();
        object stateData =  System.Activator.CreateInstance(mCurSelectType);
        FArpgMachine.FArgpBaseData fbd = (FArpgMachine.FArgpBaseData)stateData;//new FArpgMachine.FArgpBaseData();
        int index = 0;
        string nodeName = "";
        while(true)
        {
            nodeName = "node_" + (index++).ToString();
            if(!GetIsSameNodeName(nodeName))
            {
                break;
            } 
        }
        fbd.nodeName = nodeName;
        fbd.conditionTimes = 0;
        fbd.conditionState = "";
        var node = startNode.Regs(fbd,true, null);
        node.mRect = new Rect(mousePos.x, mousePos.y, 100, 40);
    }


    private bool GetIsSameNodeName(string key)
    {
        var root = mFArpgMachine.GetRoots();
        if(root.mMainBuffs.ContainsKey(key))
        {
            return true;
        }
        return false;
    }


    private void Clear()
    {
        mDragNode = null;
        mSelectNode = null;
        mEditorNode = null;
        mTempNode = null;
    }

    void DrawBackGround()
    {

        //var lastMatrix = GUI.matrix;
        //Matrix4x4 translation = Matrix4x4.TRS(new Vector3(200,0,0), Quaternion.identity, Vector3.one);
        //Matrix4x4 scale = Matrix4x4.Scale(new Vector3(0.5f,0.5f, 1.0f));
        //GUI.matrix = translation * scale * translation.inverse;


        GUI.color = new Color32(38, 38, 38, 255);
        GUI.DrawTexture(mScreenRect, EditorGUIUtility.whiteTexture);
        GUI.color = Color.white;
        Color lightColor = new Color32(27, 27, 27, 255);
        Color lightDarkColor = new Color32(35, 35, 35, 255);

        Vector2 LineVec1 = new Vector2();
        Vector2 LineVec2 = new Vector2();
        for (int x = 0; ; x++)
        {
            LineVec1.x = LineVec2.x = x * 15;
            LineVec1.y = 0;
            LineVec2.y = mScreenRect.height;
            Handles.color = x % 5 == 0 ? lightColor : lightDarkColor;
            Handles.DrawLine(LineVec1, LineVec2);
            if (LineVec1.x > mScreenRect.width)
                break;
        }
        for (int y = 0; ; y++)
        {
            LineVec1.y = LineVec2.y =  y * 15;
            LineVec1.x = 0;
            LineVec2.x = mScreenRect.width;
            Handles.color = y % 5 == 0 ? lightColor : lightDarkColor;
            Handles.DrawLine(LineVec1, LineVec2);
            if (LineVec1.y > mScreenRect.height)
            {
                //GUI.matrix = lastMatrix;
                return;
            }
        }

        
    }
}
