using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using F2DEngine;

public class FFontMaker
{

    [MenuItem("Assets/艺术字体生成")]
    static void CarteArtFont()
    {
        UnityEngine.Object obj = Selection.activeObject;
        string setImgPath = AssetDatabase.GetAssetPath(obj);
        CreateFitImag(setImgPath.Replace("Assets/", ""), obj.name);
    }


    [MenuItem("Assets/FEngine/压缩文件")]
    static void CreateZip()
    {
        UnityEngine.Object obj = Selection.activeObject;
        string setImgPath = AssetDatabase.GetAssetPath(obj);
        string filepath = Application.dataPath + "/" + setImgPath.Replace("Assets/", "");
        var files =  FZipTool.ZipDirectory(filepath, FEPath.GetDirectoryName(filepath) + "/", ".meta");

        var t = FSaveHandle.Create(FEPath.GetDirectoryName(filepath) + "/version", FFilePath.FP_Abs, FOpenType.OT_Write | FOpenType.OT_Txt);
        string md5 = FCommonFunction.GetMD5HashFromFile(files);
        int type =  EditorUserBuildSettings.activeBuildTarget != BuildTarget.iOS ? 0 : 1;
        t.SetContext("{\"version\":" + MyEdior.GetTimeVersion().ToString() + ",\"md5\":\"" + md5  + ",\"type\":\"" + type.ToString() + "\"}");
        t.Save();
    }

    public class PngPartInfo
    {
        public Texture2D text2D;
        public int index = 0;
        public Bounds bound;//图片包围盒
    }

    public static void SetTextureImporter(string subPngPath)
    {
        TextureImporter ti = AssetImporter.GetAtPath(subPngPath) as TextureImporter;
        ti.textureType = TextureImporterType.Sprite;
        ti.mipmapEnabled = false;
        ti.isReadable = true;
        ti.filterMode = FilterMode.Bilinear;
        ti.textureCompression = TextureImporterCompression.Uncompressed;
        AssetDatabase.ImportAsset(subPngPath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
    }

    private const float MaxChink = 1.0f;
     //1. R靠紧容器矩形的左上角排放
     //2. R靠紧容器矩形的左下角排放
     //3. A点放置在R1的左上角
     //4. A点放置在R1的右下角
     //5. R的左上角对齐R1的右上角排放
    private static void  _ComputerRectPngPart(PngPartInfo info,List<PngPartInfo> infos,int num)
    {
        if(infos.Count == 0)
        {
            info.bound.center = new Vector3(info.bound.extents.x, info.bound.extents.y, 0);
            return;
        }
        List<PngPartInfo> newInfos = new List<PngPartInfo>(infos);
        newInfos.Add(info);
        List<Bounds> bounds = new List<Bounds>();
        System.Action<PngPartInfo> CallBack = (f) =>
        {
            Vector3 rect = info.bound.center - info.bound.extents;
            if (rect.x >= 0 && rect.y >= 0)
            {
                for (int i = 0; i < infos.Count; i++)
                {
                    if (infos[i].bound.Intersects(info.bound))
                        return;
                }
                bounds.Add(info.bound);
            }
        };

        for(int i = 0; i < infos.Count;i++)
        {
            var temp = infos[i];
            //1. R靠紧容器矩形的左上角排放
            info.bound.center = temp.bound.center + new Vector3(-temp.bound.extents.x+info.bound.extents.x, info.bound.extents.y + temp.bound.extents.y+MaxChink,0);
            CallBack(info);
            //2. R靠紧容器矩形的左下角排放
            info.bound.center = temp.bound.center + new Vector3(-temp.bound.extents.x + info.bound.extents.x, -info.bound.extents.y - temp.bound.extents.y - MaxChink, 0);
            CallBack(info);
            //3. A点放置在R1的左上角
            info.bound.center = temp.bound.center + new Vector3(temp.bound.extents.x + info.bound.extents.x + MaxChink, info.bound.extents.y + temp.bound.extents.y + MaxChink, 0);
            CallBack(info);
            //4. A点放置在R1的右下角
            info.bound.center = temp.bound.center + new Vector3(temp.bound.extents.x + info.bound.extents.x + MaxChink, info.bound.extents.y - temp.bound.extents.y, 0);
            CallBack(info);
            //5. R的左上角对齐R1的右上角排放
            info.bound.center = temp.bound.center + new Vector3(temp.bound.extents.x + info.bound.extents.x + MaxChink, -info.bound.extents.y + temp.bound.extents.y, 0);
            CallBack(info);
        }
        info.bound = bounds[num%bounds.Count];
    }

    //得到最适分数
    private static Vector2 GetFitValue(List<PngPartInfo> infos)
    {
        Vector2 mapSize = Vector2.zero;

        for(int i = 0; i < infos.Count;i++)
        {
            var info = infos[i];
            float tempX = info.bound.center.x + info.bound.extents.x;
            float tempY = info.bound.center.y + info.bound.extents.y;
            if(tempX > mapSize.x)
            {
                mapSize.x = tempX;
            }

            if(tempY > mapSize.y)
            {
                mapSize.y = tempY;
            }
        }
        return mapSize;
    }


    private static void CreateFitImag(string sourceImg, string fileName)
    {
        string filepath = Application.dataPath + "/" + sourceImg+"/";
        string texturePath = filepath + fileName + "_texture.png";
        string materialPath = filepath.Replace(Application.dataPath, "Assets") + fileName + "_mat.mat";
        string fontPath = filepath.Replace(Application.dataPath, "Assets") + fileName + "_font.fontsettings";
        string[] pngs = FEPath.GetFiles(Application.dataPath + "/" + sourceImg, "*.png");

        float maxX = 0;
        float maxY = 0;
        List<PngPartInfo> ppInfos = new List<PngPartInfo>();
        foreach (string path in pngs)
        {
            string subPngPath = path.Replace(Application.dataPath, "Assets");
            if (subPngPath.IndexOf("texture") == -1)
            {
                SetTextureImporter(subPngPath);
                PngPartInfo info = new PngPartInfo();
                Texture2D tex2D = AssetDatabase.LoadAssetAtPath(subPngPath, typeof(Texture2D)) as Texture2D;
                info.text2D = tex2D;
                info.index = Uncode(tex2D.name);
                info.bound = new Bounds();
                info.bound.extents = new Vector3(tex2D.width/2+2, tex2D.height /2+2,0);
                maxX += tex2D.width;
                maxY += tex2D.height;
                ppInfos.Add(info);
            }
        }

        float maxFitValue = maxY + maxX;
       
        var mGeneOnlyData = NeuralAlgorithm.CreateNormalGene(50,ppInfos.Count,20, (t) =>
        {
            List<PngPartInfo> tempInfo = new List<PngPartInfo>();
            for (int i = 0; i < t._Code.Count; i++)
            {
                _ComputerRectPngPart(ppInfos[i], tempInfo, t._Code[i]);
                tempInfo.Add(ppInfos[i]);
            }
            Vector2 tempSize = GetFitValue(tempInfo);
            return (1 - (Mathf.Max(tempSize.x, tempSize.y)) / maxFitValue);
        });

        //100次选出最优的图集方式
        mGeneOnlyData.EvolveGenes(20);
        var gen = mGeneOnlyData.GetBestFit();

        List<PngPartInfo> desInfo = new List<PngPartInfo>();
        for (int i = 0; i < gen._Code.Count; i++)
        {
            _ComputerRectPngPart(ppInfos[i], desInfo,gen._Code[i]);
            desInfo.Add(ppInfos[i]);
        }
        Vector2 mapSize = GetFitValue(desInfo);

        int mapX = (int)(mapSize.x) + 1;
        int mapY = (int)(mapSize.y) + 1;
        Texture2D fullTexture = new Texture2D(mapX, mapY, TextureFormat.RGBA32, false);
        Color[] fullTcolors = fullTexture.GetPixels();
        for (int i = 0; i < fullTcolors.Length; i++)
        {
            fullTcolors[i].a = 0;
            fullTcolors[i].r = 0;
            fullTcolors[i].g = 0;
            fullTcolors[i].b = 0;
        }
        fullTexture.SetPixels(fullTcolors);

        //合并图集
        for (int i = 0; i < desInfo.Count;i++)
        {
            var info = desInfo[i];
            Color[] colors = info.text2D.GetPixels();
            fullTexture.SetPixels((int)(info.bound.center.x-info.bound.extents.x), (int)(info.bound.center.y-info.bound.extents.y), info.text2D.width, info.text2D.height, colors);
        }

        byte[] full = fullTexture.EncodeToPNG();
        File.WriteAllBytes(texturePath, full);
        AssetDatabase.Refresh();
        texturePath = texturePath.Replace(Application.dataPath, "Assets");
        SetTextureImporter(texturePath);

        Texture texture = AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture)) as Texture;
        //生成字体
        Material fontMaterial = AssetDatabase.LoadAssetAtPath(materialPath, typeof(Material)) as Material;
        if (fontMaterial == null)
        {
            fontMaterial = new Material(Shader.Find("UI/Default"));
            fontMaterial.mainTexture = texture;
            AssetDatabase.CreateAsset(fontMaterial, materialPath);
        }
        else
        {
            fontMaterial.mainTexture = texture;
        }

        Font assetFont = AssetDatabase.LoadAssetAtPath(fontPath, typeof(Font)) as Font;
        string ApplicationDataPath = Application.dataPath.Replace("Assets", "") + fontPath;

        if (assetFont == null)
        {
            assetFont = new Font();
            AssetDatabase.CreateAsset(assetFont, fontPath);
        }

        assetFont.material = fontMaterial;

        CharacterInfo[] characters = new CharacterInfo[desInfo.Count];
        for (int i = 0; i < desInfo.Count; i++)
        {
            PngPartInfo ppInfo = desInfo[i];
            CharacterInfo info = new CharacterInfo();
            info.index = ppInfo.index;
            info.uvBottomLeft = new Vector2((ppInfo.bound.center.x - ppInfo.bound.extents.x)/mapX,(ppInfo.bound.center.y - ppInfo.bound.extents.y)/ mapY);
            info.uvTopRight = new Vector2((ppInfo.bound.center.x + ppInfo.bound.extents.x)/mapX, (ppInfo.bound.center.y + ppInfo.bound.extents.y)/ mapY);
            info.minX = 0;
            info.minY = -(int)ppInfo.text2D.height;
            info.maxX = (int)ppInfo.text2D.width;
            info.maxY = 0;
            info.advance = (int)ppInfo.text2D.width;
            characters[i] = info;
        }

        assetFont.characterInfo = characters;
        EditorUtility.SetDirty(assetFont);
        AssetDatabase.SaveAssets();
    }

    //将中文字符转为10进制整数  
    public static int Uncode(string str)
    {
        int outStr = 0;
        var t = FUniversalFunction.GetChunk(str, "[", "]");
        if(t.Count > 1)
        {
            str = t[1];
        }
        if (!string.IsNullOrEmpty(str))
        {
            outStr = ((int)str[0]);
        }
        return outStr;
    }
}