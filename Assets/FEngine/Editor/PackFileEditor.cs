using F2DEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DefaultAsset))]
public class PackFileEditor : Editor
{
    public const string PACKFILEEDITOR = "PackFileEditor";
    public enum PackFileSettingType
    {
        SET_Show,//显示文件
        SET_NONE,//无
        SET_Modify,//可以更改设置
    }
    private static Texture2D[] _defaultFolderIcon = new Texture2D[12];
    private static Dictionary<string, int> mTextureTypes = new Dictionary<string, int>();
    public static Texture2D GetDefaultFolderIcon(int type)
    {
         if (_defaultFolderIcon[type] == null)
            _defaultFolderIcon[type] = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/FEngine/Icons/FileType_" + type.ToString() + ".png");//EditorGUIUtility.FindTexture("Folder Icon");

        return _defaultFolderIcon[type];
    }
    private static Dictionary<int, int> mShowIcon = new Dictionary<int, int>
    {
        {0,0},{(int)FPackageEditor.PackType.part,1},{(int)FPackageEditor.PackType.asy,2},{(int)FPackageEditor.PackType.nb,3},
        {(int)FPackageEditor.PackType.only,4},{(int)(FPackageEditor.PackType.part|FPackageEditor.PackType.only),5},{(int)(FPackageEditor.PackType.part|FPackageEditor.PackType.asy),6},{(int)(FPackageEditor.PackType.asy|FPackageEditor.PackType.only),7}
        ,{(int)FPackageEditor.PackType.micro,10}
    }; 

    [InitializeOnLoadMethod]
    static void ProjectFileIcon()  
    {
        if ((PackFileSettingType)FSettingEdior.GetSettingInt(PACKFILEEDITOR) != PackFileSettingType.SET_NONE)
        {
            //文件夹变名字
            EditorApplication.projectWindowItemOnGUI += (string guid, Rect selectionRect) =>
            {
                if (Application.isPlaying)
                    return;
                if (selectionRect.height < 20)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    if (!AssetDatabase.IsValidFolder(path)) return;

                    if (path.IndexOf(APATH) != -1)
                    {
                        int imageType = 0;
                        if (mTextureTypes.ContainsKey(path))
                        {
                            imageType = mTextureTypes[path];
                        }
                        else
                        {
                            PackFileConfig.FileParam param = PackFileConfig.GetPackType(path);
                            int cType = 0;
                            if (param != null)
                            {
                                cType = param.type;
                            }
                           
                            if (!mShowIcon.TryGetValue(cType, out imageType))
                            {
                                if (IsHaveSameWinType(cType, (int)FPackageEditor.PackType.nb))
                                {
                                    imageType = mShowIcon[(int)FPackageEditor.PackType.nb];
                                }
                                else
                                {
                                    if (IsHaveSameWinType(cType, (int)FPackageEditor.PackType.micro))
                                    {
                                        imageType = mShowIcon[(int)FPackageEditor.PackType.micro];
                                    }
                                    else
                                    {
                                        imageType = 9;
                                    }
                                }
                            }
                            mTextureTypes[path] = imageType;
                        }
                        selectionRect.width = selectionRect.height;
                        GUI.DrawTexture(selectionRect, GetDefaultFolderIcon(imageType));
                    }
                    else
                    {
                        if (PackFileConfig.GetHeadType(path) != -1)
                        {
                            selectionRect.width = selectionRect.height;
                            GUI.DrawTexture(selectionRect, GetDefaultFolderIcon(8));
                        }
                    }
                }
            };
        }
    }

    //打包配置文件
    public class PackFileConfig
    {
        private static PackFileConfig mCofig;
        public class FileParam
        {
            public int type;
            public string microEx;
        }
        public Dictionary<string,FileParam> packTypeDatas = new Dictionary<string,FileParam>();
        public Dictionary<string,int> heads = new Dictionary<string, int>();

        private static PackFileConfig GetPackFileConfig()
        {
            if(mCofig == null)
            {
                mCofig = new PackFileConfig();
                mCofig.ReadFile(); 
            }
            return mCofig;
        }

        public static FileParam GetPackType(string name)
        {
            return GetPackFileConfig()._GetPackType(name);
        }
        public static void SetPackType(string fileName, FileParam type)
        {
            GetPackFileConfig()._SetPackType(fileName, type);
        }
       
        public static void SetHeadFile(Dictionary<string, int> heads)
        {
            GetPackFileConfig()._SetHeadFileData(heads);
          
        }

        public static int GetHeadType(string path)
        {
            return GetPackFileConfig()._GetHeadType(path);
        }

        private  int _GetHeadType(string path)
        {
            int type = 0;
            if (heads.TryGetValue(path, out type))
            {
                return type;
            }
            return -1;
        }

        private FileParam _GetPackType(string fileName)
        {
            FileParam type = null;
            packTypeDatas.TryGetValue(fileName, out type);
            return type;
        }

        private void _SetHeadFileData(Dictionary<string, int> h)
        { 
            heads = h; 
            SaveFile();
        }
        private void _SetPackType(string fileName, FileParam type)
        {
            packTypeDatas[fileName] = type;
            SaveFile();
        }

        private bool ReadFile()
        {
            FSaveHandle sd = FSaveHandle.Create(Application.dataPath + "/" + ResConfig.PACKCOLORPATH, FFilePath.FP_Abs,FOpenType.OT_Binary);
            if (sd.IsLoad) 
            {
                sd.FromObject(this); 
                return true;
            }
            return false;  
        }

        private void SaveFile()
        {
            FSaveHandle sd = FSaveHandle.Create(Application.dataPath + "/" + ResConfig.PACKCOLORPATH, FFilePath.FP_Abs, FOpenType.OT_Write| FOpenType.OT_Binary);
            sd.PushObject(this);
            sd.Save();
        }
    }
   
    public static bool IsHaveSameWinType(int main, int use)
    {
        return (main & use) != 0;
    }

    private const string APATH = "/" + ResConfig.ASSETBUNDLE;


    //文件夹操作

    private string mPathFile;
    private bool mIsValidFolder = false;
    private bool mIsHeadFolder = false;
    private GUIStyle mGui;
    private PackFileConfig.FileParam mPackType = null;
    private PackFileSettingType mSetting = PackFileSettingType.SET_NONE;
    private bool mIsModify = false;
    private void OnEnable()
    {
        mPathFile = AssetDatabase.GetAssetPath(target);
        mIsValidFolder = (!string.IsNullOrEmpty(mPathFile) && AssetDatabase.IsValidFolder(mPathFile) && mPathFile.Contains(APATH));
        mIsModify = false;
        if (mIsValidFolder)
        {
            mGui = new GUIStyle();        
            mIsHeadFolder = mPathFile.Contains(ResConfig.ASSETBUNDLE + "/");
            mPackType = PackFileConfig.GetPackType(mPathFile);
            if(mPackType == null)
            {
                mPackType = new PackFileConfig.FileParam();
                mPackType.type = 0;
            }
            mSetting = (PackFileSettingType)FSettingEdior.GetSettingInt(PACKFILEEDITOR);
            if(mSetting == PackFileSettingType.SET_NONE)
            {
                mIsValidFolder = false;
            }
            else if(mSetting == PackFileSettingType.SET_Modify)
            {
                mIsModify = true;
            }
        }
    }

    //资源特殊文件显示
    public override void OnInspectorGUI()
    {
        if (mIsValidFolder)
        {
            //assetbundle文件夹
            mGui.fontSize = 24;
            mGui.normal.textColor = Color.green;
            GUILayout.Label(ResConfig.ASSETBUNDLE + "文件夹", mGui);

            if (mIsHeadFolder)
            {
                GUI.enabled = mIsModify && (!Application.isPlaying);
                mGui.fontSize = 16;
                mGui.normal.textColor = Color.green;
                var pmode = Enum.GetValues(typeof(FPackageEditor.PackType));
                foreach (var fp in pmode)
                {
                    FPackageEditor.PackType tempType = (FPackageEditor.PackType)fp;
                    if (tempType != FPackageEditor.PackType.none)
                    {
                        bool isSelect = IsHaveSameWinType(mPackType.type, (int)tempType);
                        bool rSelect = EditorGUILayout.Toggle("当前模式状态:[" + tempType.ToString() + "]", isSelect);
                        if (mIsModify)
                        {
                            if (rSelect)
                            {
                                mPackType.type |= (int)tempType;
                            }
                            else
                            {
                                mPackType.type &= (~(int)tempType);
                            }
                        }
                        if(isSelect&&tempType == FPackageEditor.PackType.micro)
                        {
                            //微端额外配置
                            mPackType.microEx = EditorGUILayout.TextField(mPackType.microEx);
                        }
                    }
                }
                
                if (mIsModify)
                {
                    if (GUI.changed)
                    {
                        PackFileConfig.SetPackType(mPathFile, mPackType);
                        if (mTextureTypes.ContainsKey(mPathFile))
                        {
                            mTextureTypes.Remove(mPathFile);
                        }
                    }
                }
                GUI.enabled = false;
            }
            else
            {
                mGui.fontSize = 14;
                mGui.normal.textColor = Color.red;
                GUILayout.Label(ResConfig.ASSETBUNDLE + ":为资源文件夹,放入里面的资源文件将进入打包系统", mGui);
                mGui.normal.textColor = Color.green;
                GUILayout.Label("标签:[" + FPackageEditor.PackType.part + "]:资源分开打包", mGui);
                GUILayout.Label("标签:[" + FPackageEditor.PackType.asy + "]:资源用于异步加载", mGui);
                GUILayout.Label("标签:[" + FPackageEditor.PackType.nb + "]:资源不打包", mGui);
                GUILayout.Label("标签:[" + FPackageEditor.PackType.only + "]:资源只有被引用才打包", mGui);
                GUILayout.Label("标签:[" + FPackageEditor.PackType.micro + "]:微端资源,排除关键字文件,多个以;隔开", mGui);
                GUILayout.Label("标签:[" + FPackageEditor.PackType.cache + "]:缓存标记", mGui);
            }
        }
    }
}
