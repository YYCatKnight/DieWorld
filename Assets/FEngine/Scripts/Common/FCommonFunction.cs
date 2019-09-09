//----------------------------------------------
//  F2DEngine: time: 2015.10  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using UnityEngine.Networking;

namespace F2DEngine
{
    public static class FCommonFunction
    {
        public static WaitForSeconds WaitFor3 = new WaitForSeconds(0.3f);
        private static int mMaxSpeed = 0;

        public static void SwitchSprite(this Image sp, string name)
        {
            var spr = SceneManager.instance.CreateSprite(name);
            if(spr == null)
            {
                if(FEngine.mBackDown != null)
                {
                    if (name.Contains("UI2/BarracksSmall/"))
                    {
                        sp.sprite = SceneManager.instance.CreateSprite("Texture/null_BarracksSmall");
                    }
                    else if(name.Contains("UI2/Technology/"))
                    {
                        sp.sprite = SceneManager.instance.CreateSprite("Texture/Technology_0");
                        if (mMaxSpeed < 10)
                        {
                            mMaxSpeed++;
                            int idex = 0;
                            var timer = Timer_Logic.SetTimer((t) =>
                            {
                                if (sp != null)
                                {
                                    idex++;
                                    if (idex > 2)
                                    {
                                        idex = 0;
                                    }
                                    sp.sprite = SceneManager.instance.CreateSprite("Texture/Technology_" + idex.ToString());
                                    return 0.3f;
                                }
                                else
                                {
                                    mMaxSpeed--;
                                    return -1;
                                }
                            }, 1, null);
                        }
                    }
                    else
                    {
                        sp.sprite = SceneManager.instance.CreateSprite("Texture/null_null");
                    }
                }
            }
            else
            {
                sp.sprite = spr;
            }
        }
        public static void SwitchSprite(this SpriteRenderer sp, string name)
        {
            sp.sprite = SceneManager.instance.CreateSprite(name);
        }
        public static void SwitchSpriteAsync(this Image sp, string name, MonoBehaviour mo, Timer_Mix mix = null)
        {

            SceneManager.instance.LoadPrefabAsync<Sprite>(name, (f) =>
            {
                sp.sprite = f;
            }, mo);
        }

        public static string GetMD5HashFromFile(string fileName)
        {
            FileStream file = new FileStream(fileName, System.IO.FileMode.Open);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(file);
            file.Close();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }

        public static Color HexToColor(this string hex)
        {
            hex = hex.Replace("0x", "");//in case the string is formatted 0xFFFFFF
            hex = hex.Replace("#", "");//in case the string is formatted #FFFFFF
            byte a = 255;//assume fully visible unless specified in hex
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            //Only use alpha if the string has enough characters
            if (hex.Length == 8)
            {
                a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
            }
            return new Color32(r, g, b, a);
        }

        public static FMySprite CreateMySprite(this GameObject go, GameObject parent = null)
        {
            GameObject newGameObject = new GameObject();
            newGameObject.name = "FMySprite_" + go.name;
            newGameObject.layer = go.layer;
            if (parent == null)
            {
                parent = go.transform.parent.gameObject;
            }
            newGameObject.transform.parent = parent.transform;
            newGameObject.transform.position = go.transform.position;
            newGameObject.transform.rotation = go.transform.rotation;
            newGameObject.transform.localScale = go.transform.localScale;
            FMySprite fs = SceneManager.instance.AddComponent<FMySprite>(newGameObject);
            fs.SetHostGameObject(go);
            return fs;
        }

        public static string GetHpFillRes(float hp)
        {
            if (hp >= 0.5f)
            {
                return "UI2/Common/TY_Progressbar05";
            }
            else if (hp >= 0.25f)
            {
                return "UI2/Common/TY_Progressbar01";
            }
            return "UI2/Common/TY_Progressbar02";
        }

        public static FUniversalPanel ChangeUniversal(this GameObject thisGo, string keyName = "F_")
        {
            FUniversalPanel fp = SceneManager.instance.AddComponent<FUniversalPanel>(thisGo);
            fp.mKey = keyName;
            fp.ApplyData();
            return fp;
        }

        public static RectTransform rectTransform(this Component cp)
        {
            return cp.transform as RectTransform;
        }

        public static void FlasePlane(this GameObject go, int delay = 3)
        {
            Vector3 lastPos = go.transform.localPosition;
            go.transform.localPosition = new Vector3(9999, 9999, 0);
            Timer_Frequency.SetTimer((f) =>
            {
                go.transform.localPosition = lastPos;
            }, delay, go.transform);
        }

        public static string GetTimeString(this float value)
        {
            return ((int)(value + 0.999f)).GetTimeString();
        }
        public static string GetTimeString(this int value)
        {
            long lValue = value;
            return lValue.GetTimeString();
        }
        public static string GetTimeString(this long value)
        {
            string mText = "{0:D2}:{1:D2}:{2:D2}";
            long day = value / 86400;
            long hour = value / 3600;
            long minute = (value - hour * 3600) / 60;
            long second = value % 60;

            if (day < 1)
            {
                return string.Format(mText, hour, minute, second);
            }

            value -= day * 86400;
            hour = value / 3600;
            value -= hour * 3600;
            minute = value / 60;
            value -= minute * 60;
            second = value;
            return string.Format("{0:D1}d:{1:D2}:{2:D2}:{3:D2}", day, hour, minute, second);
        }
        public static string GetAboutTimeString(this long value)
        {
            long day = value / 86400; if(day>0)       return day    + "d";
            long hour = value / 3600; if (hour > 0)   return hour   + "h";
            long minute = value / 60; if (minute > 0) return minute + "m";
            long second = value % 60;                 return second + "s";
        }

        public static void HierarchyEffect(this BasePlane plane)
        {
            if(plane != null)
            {
                var canv = SceneManager.instance.AddComponent<Canvas>(plane.gameObject);
                canv.overrideSorting = true;
                canv.sortingOrder = 1;
                SceneManager.instance.AddComponent<GraphicRaycaster>(plane.gameObject);
            }
        }

        //得到时间戳
        public static long GetTimeStamp(this DateTime date)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            long timeStamp = (long)(date - startTime).TotalSeconds; // 相差秒数
            return timeStamp;
        }

        public static long GetTimeStampWM(this DateTime date)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            long timeStamp = (long)(date - startTime).TotalMilliseconds; // 相差毫秒数
            return timeStamp;
        }

        public static long ConvertMsToSecond(this long millisecondsTs)
        {
            return (long)(millisecondsTs / 1000);
        }

        //时间戳转成时间
        public static System.DateTime StampToTimes(this long stamp)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            DateTime dt = startTime.AddMilliseconds(stamp);
            return dt;
        }

        //毫秒时间戳转成标准时间格式
        public static string StampToTimesFormat(this long stamp)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            DateTime dt = startTime.AddMilliseconds(stamp);
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }

        //毫秒时间戳转成时间
        public static System.DateTime StampToUTCTimes(this long stamp)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            DateTime dt = startTime.AddMilliseconds(stamp);
            return dt.ToUniversalTime();
        }

        //秒时间戳转成时间
        public static System.DateTime StampToTimes(this int stamp)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            DateTime dt = startTime.AddSeconds(stamp);
            return dt;
        }

        public static string GetGreaterZeroStr(this long elapse)
        {
            if (elapse >= 0)
            {
                return elapse.GetTimeString();
            }
            return 0.GetTimeString();
        }


        public static Text MirrorText(this Text text)
        {
            text.transform.localEulerAngles = new Vector3(0, 180, 0);
            if ((int)text.alignment % 3 == 0)
            {
                text.alignment += 2;
            }
            else if ((int)text.alignment % 3 == 2)
            {
                text.alignment -= 2;
            }
            return text;
        }

        public static bool BitCheck(int val, int bitNum)
        {
            if (((1 << bitNum) & val) != 0)
            {
                return true;
            }
            return false;
        }

        public static string MergeString(this string str, params object[] keys)
        {
            if (keys!= null&&keys.Length > 0)
            {
                try
                {
                    return string.Format(str, keys);
                }
                catch
                {
                    return str;
                }
            }
            return str;
        }

        public static string TranslationString(this string str, params object[] keys)
        {
            return Lange_ListAsset.instance.GetValue(str).MergeString(keys);
        }

        public static void ChangeGreyR(this GameObject go, bool isChange = true, GreyItem.GreyType type = FCGreyItem.GreyType.GT_ForbidButton)
        {
            GreyItem gi = SceneManager.instance.AddComponent<GreyItem>(go);
            if (isChange)
            {
                if (FUniversalFunction.IsContainSameType((int)type, (int)GreyItem.GreyType.GT_Sprite))
                {
                    gi.PlayGray(SceneManager.LoadPrefab<Material>(ResConfig.MATERIAL_GREY + "S"), type);
                }
                else
                {
                    gi.PlayGray(SceneManager.LoadPrefab<Material>(ResConfig.MATERIAL_GREY), type);
                }
            }
            else
            {
                gi.Clear();
            }
        }

        public static void ChangeDColor(this GameObject go, bool isChange = true, Color color = new Color(), GreyItem.GreyType type = FCGreyItem.GreyType.GT_ForbidButton)
        {
            GreyItem gi = SceneManager.instance.AddComponent<GreyItem>(go);
            if (isChange)
            {
                gi.PlayDrak(color, type);
            }
            else
            {
                gi.Clear();
            }
        }

        public static void ChangeDrak(this GameObject go, bool isChange = true, GreyItem.GreyType type = FCGreyItem.GreyType.GT_ForbidButton)
        {
            GreyItem gi = SceneManager.instance.AddComponent<GreyItem>(go);
            if (isChange)
            {
                gi.PlayDrak(Color.gray, type);
            }
            else
            {
                gi.Clear();
            }
        }

        public static void ChangeGrey(this Image spr, bool isChange = true, string ex = ResConfig.MATERIAL_GREY)
        {
            if (spr == null)
                return;
            if (isChange == false)
            {
                spr.material = null;
            }
            else
            {
                spr.material = SceneManager.LoadPrefab<Material>(ex);
            }
        }

        public static void ChangeGrey(this RawImage spr, bool isChange = true, string ex = ResConfig.MATERIAL_GREY)
        {
            if (isChange == false)
            {
                spr.material = null;
            }
            else
            {
                spr.material = SceneManager.LoadPrefab<Material>(ex);
            }
        }

        public static List<T> RandomSortList<T>(List<T> ListT)
        {
            List<T> newList = new List<T>(ListT);
            int currentIndex;
            T tempValue;
            for (int i = 0; i < newList.Count; i++)
            {
                currentIndex = UnityEngine.Random.Range(0, newList.Count - i);
                tempValue = newList[currentIndex];
                newList[currentIndex] = newList[newList.Count - i - 1];
                newList[newList.Count - i - 1] = tempValue;
            }
            return newList;
        }

        public static void AddClickEvent(this GameObject go, System.Action<string> callEvent)
        {
            Button[] buts = go.GetComponentsInChildren<Button>(true);
            for (int i = 0; i < buts.Length; i++)
            {
                Button bt = buts[i];
                bt.onClick.AddListener(() =>
                {
                    callEvent(bt.name);
                });
            }
        }

        public static float GetLarpF(this float curF, float nextF, float speed)
        {
            float tempF = curF;
            float lent = Mathf.Abs(nextF - curF);
            if (lent > speed * Time.deltaTime)
            {
                float normalF = (nextF - curF) / lent;
                tempF += normalF * speed * Time.deltaTime;
            }
            else
            {
                tempF = nextF;
            }
            return tempF;
        }


        public static Vector3 GetLarpVectorTimes(this Vector3 curPos, Vector3 nextPos, float speed)
        {
            float sqrMag = Vector3.SqrMagnitude(nextPos - curPos);
            Vector3 tempPos = curPos;
            var s = speed;
            if (sqrMag > s * s)
            {
                Vector3 normal = nextPos - curPos;
                normal.Normalize();
                tempPos += normal * s;
            }
            else
            {
                tempPos = nextPos;
            }
            return tempPos;
        }


        public static string ToNoLineSpace(this string str)
        {
            if(!string.IsNullOrEmpty(str))
            {
                return str.Replace(" ", "\u00A0");
            }
            return str;
        }

        public static Vector3 GetLarpVector(this Vector3 curPos, Vector3 nextPos, float speed, float maxTimeDp = 0.03f)
        {
            float sqrMag = Vector3.SqrMagnitude(nextPos - curPos);
            Vector3 tempPos = curPos;
            float deltaTime = Mathf.Min(Time.deltaTime, maxTimeDp);
            var s = speed * deltaTime;
            if (sqrMag > s*s)
            {
                Vector3 normal = nextPos - curPos;
                normal.Normalize();
                tempPos += normal * s;
            }
            else
            {
                tempPos = nextPos;
            }
            return tempPos;
        }

        public static void CopyInfo(this GameObject go, GameObject copyInfo)
        {
            go.transform.rotation = copyInfo.transform.rotation;
            go.transform.position = copyInfo.transform.position;
            go.transform.localScale = copyInfo.transform.lossyScale.RelativeExcept(go.transform.lossyScale);
        }

        public static Vector3 RelativeExcept(this Vector3 pos, Vector3 dc)
        {
            return new Vector3(pos.x / dc.x, pos.y / dc.y, pos.z / dc.z);
        }

        //序列化的类,带[System.Serializable]
        public static T Clone<T>(T RealObject)
        {
            using (Stream objectStream = new MemoryStream())
            {
                //利用 System.Runtime.Serialization序列化与反序列化完成引用对象的复制  
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, RealObject);
                objectStream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(objectStream);
            }
        }

        public static Vector3 RelativeCross(this Vector3 pos, Vector3 dc)
        {
            return new Vector3(pos.x * dc.x, pos.y * dc.y, pos.z * dc.z);
        }


        public static UnityWebRequest CreateHttpMsg(string url, FNetHead head)
        {
            UnityWebRequest www = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
            byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(head));
            www.uploadHandler = new UploadHandlerRaw(postBytes);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            //www.timeout = 30;
            return www;
        }

        public static void SetList<T>(List<T> list, Action<T, int> stateCall)
        {
            for (int i = 0; i < list.Count; i++)
            {
                stateCall(list[i], i);
            }
        }

        public static Rect GetRect(this BoxCollider2D box)
        {
            Transform transform = box.transform;
            //转到世界坐标
            Vector3 ScenePos = transform.TransformPoint(box.offset);
            ScenePos.z = 0;
            Vector3 SceneSize = transform.TransformPoint(box.size);
            SceneSize -= transform.transform.TransformPoint(Vector3.zero);
            SceneSize.z = 100;
            Rect rect = new Rect();
            rect.size = SceneSize;
            rect.center = ScenePos;
            return rect;
        }

        public static Rect GetRect(this Rect box, Transform transform)
        {
            //转到世界坐标
            Vector3 ScenePos = transform.TransformPoint(box.center);
            ScenePos.z = 0;
            Vector3 SceneSize = transform.TransformPoint(box.size);
            SceneSize -= transform.transform.TransformPoint(Vector3.zero);
            SceneSize.z = 100;
            Rect rect = new Rect();
            rect.size = SceneSize;
            rect.center = ScenePos;
            return rect;
        }


        public static List<T> GetList<T>(List<T> list, Func<T, bool> sort)
        {
            List<T> bb = new List<T>();
            for (int i = 0; i < list.Count; i++)
            {
                if (sort(list[i]))
                {
                    bb.Add(list[i]);
                }
            }
            return bb;
        }

        //两个不同的类型,相同赋值
        public static void SetSameTypePro(object des, object source)
        {
            var prop = source.GetType().GetProperties();
            var obj = des.GetType();
            foreach (var node in prop)
            {
                var p = obj.GetProperty(node.Name);
                if (p != null)
                {
                    p.SetValue(des, node.GetValue(source, null), null);
                }
            }
        }

        public static float Get2DAngle(this Vector3 from, Vector3 to)
        {
            Vector3 cross = Vector3.Cross(from, to);
            float angle = Vector2.Angle(from, to);
            return cross.z > 0 ? -angle : angle;
        }

        public static Vector3 Get2DEuler(this Vector3 from)
        {
            return new Vector3(0, 0, -Vector3.up.Get2DAngle(from));
        }

        public static Quaternion Lerp2DQuaternion(this Vector3 from, Vector3 to, float t)
        {
            float angle = Get2DAngle(from, to) * t;
            Vector3 euler = from.Get2DEuler();
            euler.z += angle * -1;
            return Quaternion.Euler(euler);
        }

        public static Quaternion Get2DAngleQuaternion(this Vector3 from)
        {
            Quaternion qt = Quaternion.identity;
            qt.eulerAngles = from.Get2DEuler();
            return qt;
        }

        public static List<Type> GetTypesByAssignable(Type type)
        {
            List<Type> tempTypes = new List<Type>();
            var types = System.Reflection.Assembly.Load("Assembly-CSharp").GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                var t = types[i];
                if (t.IsClass && !t.IsAbstract && type.IsAssignableFrom(t))
                {
                    tempTypes.Add(t);
                }
            }
            return tempTypes;
        }

        public static List<Type> GetAssemblyType(Type type)
        {
            List<Type> tempTypes = new List<Type>();
            var types = System.Reflection.Assembly.Load("Assembly-CSharp").GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                var t = types[i];
                if (t.IsSubclassOf(type) && !t.FullName.Contains("GooglePlayGames"))
                {
                    tempTypes.Add(t);
                }
            }
            return tempTypes;
        }

        /*
		public static void NormalRectTransform(this RectTransform rt)
		{
			rt.offsetMin = Vector2.zero;
			rt.offsetMax = Vector2.zero;
			rt.localPosition = Vector3.zero;
			rt.localScale = Vector3.one;
			rt.localRotation = Quaternion.identity;
		}
        */

        public static T EncryptObject<T>(this string str, bool isArray = false) where T : new()
        {
            string realyValue = str;
            if (realyValue.Contains("|"))
            {
                realyValue = "{" + realyValue.Replace("|", "},{") + "}";
            }
            return StringSerialize.Deserialize<T>("{" + realyValue + "}");
        }


        public static void NormalTransform(this Transform rt)
        {
            rt.localPosition = Vector3.zero;
            rt.localScale = Vector3.one;
            rt.localRotation = Quaternion.identity;
        }

        //删除某个路径下所有文件
        public static void DeletFile(string path)
        {
            if (Directory.Exists(path))
            {
                // 获得文件夹数组
                string[] strDirs = Directory.GetDirectories(path);
                // 获得文件数组  
                string[] strFiles = System.IO.Directory.GetFiles(path);

                // 遍历所有子文件夹 
                foreach (string strFile in strFiles)
                {
                    // 删除文件夹  
                    File.Delete(strFile);
                }
                // 遍历所有文件  
                foreach (string strdir in strDirs)
                {
                    Directory.Delete(strdir, true);
                }
                Directory.Delete(path, true);
            }
        }

        public static void ChangeHigLay(this BasePlane plane)
        {
            plane.transform.SetParent(MainCanvas.instance.GetLayer(LayerType.LT_Hig));
        }

        public static string GetMD5(string msg)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(msg);
            byte[] md5Data = md5.ComputeHash(data, 0, data.Length);
            md5.Clear();

            string destString = "";
            for (int i = 0; i < md5Data.Length; i++)
            {
                destString += System.Convert.ToString(md5Data[i], 16).PadLeft(2, '0');
            }
            destString = destString.PadLeft(32, '0');
            return destString;
        }

        //得到路径下的所以文件
        public static List<string> GetFiles(string path, List<string> FileList, string ex = "")
        {
            if (!Directory.Exists(path))
                return FileList;
            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] fil = dir.GetFiles();
            DirectoryInfo[] dii = dir.GetDirectories();
            foreach (FileInfo f in fil)
            {
                //int size = Convert.ToInt32(f.Length);  
                if (ex == "" || f.FullName.EndsWith(ex))
                    FileList.Add(f.FullName);//添加文件路径到列表中  
            }
            //获取子文件夹内的文件列表，递归遍历  
            foreach (DirectoryInfo d in dii)
            {
                GetFiles(d.FullName, FileList, ex);
            }
            return FileList;
        }

        //世界坐标转到ui主表
        public static Vector3 WorldToUIPoint(Vector3 worldPos)
        {
            Vector3 pos = Camera.main.WorldToViewportPoint(worldPos);
            var uiPos = MainCanvas.instance.GetMianCamera().ViewportToWorldPoint(pos);
            uiPos.z = 0;
            return uiPos;
        }
        //世界坐标转到ui主表
        public static Vector3 CamWorldToUIPoint(Vector3 worldPos, Camera cam)
        {
            Vector3 pos = cam.WorldToViewportPoint(worldPos);
            var uiPos = MainCanvas.instance.GetMianCamera().ViewportToWorldPoint(pos);
            uiPos.z = 0;
            return uiPos;
        }
        public static Vector3 UIToWorld(Vector3 worldPos)
        {
            Vector3 pos = MainCanvas.instance.GetMianCamera().WorldToViewportPoint(worldPos);
            var uiPos = Camera.main.ViewportToWorldPoint(pos);
            uiPos.z = 0;
            return uiPos;
        }


        //大地图专用
        public static Vector3 MapToWorldPosition(Vector3 mapPosition, Vector2 size, int halfOffse = 1)
        {
            Vector3 realyPos = mapPosition;
            realyPos.x = mapPosition.x;
            realyPos.y = mapPosition.y;
            Vector3 pos = Vector3.zero;
            pos.x = size.x / 4.0f + (realyPos.x + realyPos.y) * size.x / 2;
            pos.y = size.y / 4.0f + (realyPos.x - realyPos.y) * size.y / 2;
            pos += new Vector3(halfOffse * size.x / 2, 0, 0);
            return pos;
        }


        //不限制位置返回数据专用
        public static Vector3 WorldPosToMap(this Vector3 worldPos, Vector2 size)
        {
            Vector3 newPos = worldPos;
            newPos.x -= size.x / 4.0f;
            newPos.y -= size.y / 4.0f;
            return new Vector3((int)(newPos.x / size.x + newPos.y / size.y + 0.001f), (int)(newPos.x / size.x - newPos.y / size.y + 0.001f), 0);
        }

        public static string ToColor(this string str, string colorStr)
        {
            return string.Format("<color={0}>{1}</color>", colorStr, str);
        }

        public static string ToBlod(this string str)
        {
            return string.Format("<b>{0}</b>", str);
        }

        public static bool IsNull(this long i)
        {
            if (i == -1)
                return true;
            return false;
        }

        public static bool IsNull(this int i)
        {
            if (i == -1)
                return true;
            return false;
        }
        public static string ToNoticeNum(this int num, bool isBold = false)
        {
            //temp test
            string numStr = num.ToString();
            //            string numStr = num > 0 ? (num < 100 ? num.ToString() : "99+") : "";
            return isBold ? ToBlod(numStr) : numStr;
        }

        public static Vector2 UpdateWrapBound(this RectTransform rect)
        {
            rect.sizeDelta = Vector2.zero;
            var b = RectTransformUtility.CalculateRelativeRectTransformBounds(rect);
            rect.sizeDelta = b.size;

            return rect.sizeDelta;
        }
        public static Vector2 ReCalculateRectHeight(this RectTransform rect)
        {
            float height = 0;
            for (int i = 0; i < rect.childCount; i++)
            {
                if (rect.GetChild(i).gameObject.activeInHierarchy)
                {
                    height += rect.GetChild(i).GetComponent<RectTransform>().sizeDelta.y;
                }
            }
            return new Vector2(rect.sizeDelta.x, height);
        }
        /// <summary>
        /// 将Scroll View的Content重置到初始位置  
        /// </summary>
        /// <param name="rect"></param>
        public static void UpdateContentStartPos(this RectTransform rect)
        {
            rect.localPosition = new Vector3(rect.localPosition.x, 0, rect.localPosition.z);
        }

        public static Vector2 UpdateWrapBound(this Transform trans)
        {
            var rect = trans as RectTransform;
            if (rect)
            {
                rect.sizeDelta = Vector2.zero; ;
                var b = RectTransformUtility.CalculateRelativeRectTransformBounds(rect);
                rect.sizeDelta = b.size;
                return rect.sizeDelta;
            }
            else
            {
                return Vector2.zero;
            }
        }

        public static string ToPosition(this int posId)
        {
            return "X:" + --posId % 1000 + " Y:" + posId / 1000;
        }

        public static bool NameIsCanUse(this string name, int limit = 3, bool isEncoding = false)//默认不按字符编码，也就是说中文视为一个字符
        {
            if(name.Contains("^"))
            {
                return false;
            }
            return isEncoding ? System.Text.Encoding.Default.GetBytes(name).Length >= limit : name.Length >= limit;
        }
        public static int GetStringToCharLength(this string str, bool isEncoding = false)//返回一个字符串的长度，默认中文为一个长度
        {
            return isEncoding ? System.Text.Encoding.Default.GetBytes(str).Length : str.Length;
        }

        public static void SetTextTo3Point(this Text text, string toShow, int maxLen)
        {
            var set = text.GetGenerationSettings(Vector2.zero);
            int targetLen = maxLen - text.fontSize;

            if (text.cachedTextGenerator.GetPreferredWidth(toShow, set) <= targetLen)
            {
                text.text = toShow;
                return;
            }

            Font myFont = text.font;
            myFont.RequestCharactersInTexture(toShow, text.fontSize, text.fontStyle);
            CharacterInfo characterInfo = new CharacterInfo();

            char[] arr = toShow.ToCharArray();

            var totalLength = 0;

            int index = 0;
            foreach (char c in arr)
            {
                myFont.GetCharacterInfo(c, out characterInfo, text.fontSize);

                totalLength += characterInfo.advance;

                if (totalLength > targetLen)
                {
                    text.text = toShow.Substring(0, index) + "...";
                    return;
                }

                index++;
            }
        }

        public static string ToAllyName(this string pName, string allyName)
        {
            return string.IsNullOrEmpty(allyName) ? pName : string.Format("({0}){1}", allyName, pName);
        }

        public static void ToTextPos(this RectTransform moveTf, Text text, float InitPos = 0, string str = "", bool isLeft = true)//默认本身靠文本左移
        {
            if (!string.IsNullOrEmpty(str))
            {
                text.text = str;
            }
            var rectTxt = text.GetComponent<RectTransform>();
            rectTxt.sizeDelta = new Vector2(text.preferredWidth, rectTxt.sizeDelta.y);
            if (isLeft)
            {
                rectTxt.localPosition = new Vector3(InitPos - text.preferredWidth / 2, rectTxt.localPosition.y, rectTxt.localPosition.z);
                moveTf.localPosition = new Vector3(InitPos - text.preferredWidth - moveTf.sizeDelta.x / 2, moveTf.localPosition.y, moveTf.localPosition.z);
            }
            else
            {
                rectTxt.localPosition = new Vector3(InitPos + text.preferredWidth / 2, rectTxt.localPosition.y, rectTxt.localPosition.z);
                moveTf.localPosition = new Vector3(InitPos + text.preferredWidth + moveTf.sizeDelta.x / 2, moveTf.localPosition.y, moveTf.localPosition.z);
            }
        }

        public static string LeveByImageCode(this int level)
        {
            return ((char)((int)'a' + level - 1)).ToString();
        }

        public static void CopyObject(object realObject, object desObj)
        {
            System.Reflection.FieldInfo[] fields = desObj.GetType().GetFields();
            foreach (var field in fields)
            {
                bool isSetKey = false;
                if (field.FieldType == typeof(int))
                {
                    int value = (int)field.GetValue(desObj);
                    if (value == -1)
                    {
                        isSetKey = true;
                    }
                }
                else if (field.FieldType == typeof(long))
                {
                    long value = (long)field.GetValue(desObj);
                    if (value == -1)
                    {
                        isSetKey = true;
                    }
                }
                else
                {
                    if (field.GetValue(desObj) == null)
                    {
                        isSetKey = true;
                    }
                }

                if (isSetKey)
                {
                    field.SetValue(desObj, field.GetValue(realObject));
                }
            }
        }
        public static string ToFormatShort(this int num, string format = "N1")
        {
            long lNum = num;
            return lNum.ToFormatShort(format);
        }
        public static string ToFormatShort(this float num, string format = "N1")
        {
            long lNum = (int)num;
            return lNum.ToFormatShort(format);
        }

        public static string[] unitStr = new string[13] { "", "K", "M", "G", "T", "P", "E", "Z", "Y", "B", "N", "D", "C" };
        //千 Kilo 兆 Mega 吉 Giga 太 Tera 拍 Peta 艾 Exa 泽 Zetta 尧 Yotta  Bronto Nona Dogga Corydon
        //转换为单位显示 
        public static string ToFormatShort(this long num, string format = "N1")
        {
            if (num <= 0)
            {
                return "0";
            }
            else if (num < 1000)
            {
                return num.ToString();
            }
            //string str = num.ToString();
            //int index = (str.Length - 1) / 3;
            int index = (int)Mathf.Log10(num) / 3;
            return (num / Mathf.Pow(10, index * 3) - 0.05f).ToString(format) + unitStr[index];
        }
        public static string ToFormatKilo(this long num, string format = "N2")
        {
            if (num >= 1000)
            {
                return (num * 1.0f / 1000).ToString(format) + "K";
            }
            return num.ToString();
        }
        public static string ToFormatKilo(this int num, string format = "N2")
        {
            long num1 = num;
            return num1.ToFormatKilo();
        }
        public static string ToCommaNumber(this long num,bool isHaveMinus=false)
        {
            if (num <= 0&&!isHaveMinus)
            {
                return "0";
            }
            return num.ToString("N0");
        }
        public static string ToCommaNumber(this int num, bool isHaveMinus = false)
        {
            long num1 = num;
            return ToCommaNumber(num1, isHaveMinus);
        }
        public static string ToCommaNumber(this double num, bool isHaveMinus = false)
        {
            if (num <= 0&&isHaveMinus)
            {
                return "0";
            }
            return num.ToString("N0");
        }
        public static string ToCommaNumber(this float num, bool isHaveMinus = false)
        {
            double num1 = num;
            return ToCommaNumber(num1,isHaveMinus);
        }

        /// <summary>
        /// 数字转字母
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string IntToChar(this int value)
        {
            return ((char)('a' + value - 1)).ToString();
        }

        /// <summary>
        /// Base64编码
        /// </summary>
        public static string Base64Encode(string message)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Base64解码
        /// </summary>
        public static string Base64Decode(string message)
        {
            byte[] bytes = Convert.FromBase64String(message);
            return Encoding.UTF8.GetString(bytes);
        }

        public static List<T> ToMyList<Key, T>(this Dictionary<Key, T> items)
        {
            List<T> results = new List<T>();
            var enumerator = items.GetEnumerator();
            while (enumerator.MoveNext())
            {
                results.Add(enumerator.Current.Value);
            }
            return results;
        }

        public static void ClearAllChildren(this Transform tf)
        {
            for (int i = 0; i < tf.childCount; i++)
            {
                GameObject.Destroy(tf.GetChild(i).gameObject);
            }
        }

        /// <summary>
        /// 把数字根据长度转换
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string GetByToNumble(this int num)
        {
            string str = "";
            if (num < 1000000)
            {
                str = num.ToCommaNumber();
            }
            else
            {
                str = num.ToFormatShort();
            }
            return str;
        }

        /// <summary>
        /// 实现文字过长显示省略号
        /// </summary>
        /// <param name="textComponent"></param>
        /// <param name="value"></param>
        public static void SetTextWithEllipsis(this Text textComponent, string value)
        {
            var generator = new TextGenerator();
            var rectTransform = textComponent.GetComponent<RectTransform>();
            var settings = textComponent.GetGenerationSettings(rectTransform.rect.size);
            generator.Populate(value, settings);
            var characterCountVisible = generator.characterCountVisible;
            var updatedText = value;
            if (value.Length > characterCountVisible)
            {
                updatedText = value.Substring(0, characterCountVisible - 1);
                updatedText += "…";
            }
            textComponent.text = updatedText;
        }
        /// <summary>
        /// 用艺术体时把:换成&
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ChangeTextSymbol(this string str)
        {
            string tt = str;
            string txt = tt.Replace(":", "&");
            return txt;
        }
        /// <summary>
        /// 新的ScrollRect初始入口
        /// </summary>
        /// <param name="scrollRect"></param>
        /// <param name="count"></param>
        /// <param name="group"></param>
        /// <param name="scrollType"></param>
        /// <param name="callback"></param>
        /// <param name="selectgroup"></param>
        public static  void ScrollRectInit(this FCScrollRect scrollRect, int count, int group = 1, FCScrollRect.ScrollType scrollType = FCScrollRect.ScrollType.Repeat, int selectgroup = 0, System.Action<FCScrollRect.ScrollItem> callback = null)
        {
            scrollRect.SetMode(scrollType);
            scrollRect.SetGroup(group);
            scrollRect.UpdateEvent = callback;
            scrollRect.SetCount(count);
            scrollRect.SelectGroup(selectgroup);
        }

        /// <summary>
        /// 加入保存队列
        /// </summary>
        /// <param name="obj"></param>
        public static void InLine(this Save_Object obj)
        {
            SaveDataManager.instance.AddSaveObject(obj);
        }

        /// <summary>
        /// 移出存保存队列
        /// </summary>
        /// <param name="obj"></param>
        public static void OutLine(this Save_Object obj)
        {
            SaveDataManager.instance.RemoveSaveObject(obj);
        }

        /// <summary>
        /// 取位运算
        /// </summary>
        /// <param name="val"></param>
        /// <param name="bit"></param>
        /// <returns></returns>
        public static bool ToCheck(this int val, int bit)
        {
            int endVal = val & (1 << bit);
            if (endVal > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
