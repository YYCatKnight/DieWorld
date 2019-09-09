
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using F2DEngine;

[AddComponentMenu("UI/Tape Image")]
public class FTapeImage : MaskableGraphic
{
    //注意图片格式必须是Repeat格式
    [Serializable]
    public class TapePoint
    {
        public Vector3 pos;
        public float width = 10;
    }

    [HideInInspector]
    [SerializeField]
    public Texture mTexture;
    [HideInInspector]
    [SerializeField]
    public float _Width = 1;
    [HideInInspector]
    [SerializeField]
    private float _TextureDp = 0.01f;
    [HideInInspector]
    [SerializeField]
    public List<TapePoint> mBuffsPoint = new List<TapePoint>();
    [HideInInspector]
    [SerializeField]
    public Mesh mMesh;
    [HideInInspector]
    [SerializeField]
    public int mSize = 20;

    public override Texture mainTexture
    {
        get { return mTexture; }
    }


    public void ComputeMese()
    {
        if (mBuffsPoint.Count > 1)
        {
            mMesh = CreateMesh(CreateBZ());
        }
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        
        if (mBuffsPoint.Count == 0&&mMesh == null)
        { 
            base.OnPopulateMesh(vh);
        }
        else
        {
            Mesh mesh = mMesh==null? CreateMesh(CreateBZ()): mMesh;
            Color32 color32 = color;
            vh.Clear();
            for (int i = 0; i < mesh.vertices.Length; i++)
            {
                vh.AddVert(mesh.vertices[i], color32, mesh.uv[i]);
            }

            for (int i = 0; i < mesh.triangles.Length / 3; i++)
            {
                int index = i * 3;
                vh.AddTriangle(mesh.triangles[index], mesh.triangles[index + 1], mesh.triangles[index + 2]);
            }
        }
    }


    public List<Vector3> CreateBZ()
    {
        if (mBuffsPoint.Count > 1)
        {
            Vector3[] tempVes = new Vector3[mBuffsPoint.Count];
            for(int i = 0; i < mBuffsPoint.Count;i++)
            {
                tempVes[i] = mBuffsPoint[i].pos;
            }
            Vector3[] resultList = PointController.PointList(tempVes,mSize);
            return new List<Vector3>(resultList);
        }
        return null;
    }

    private Mesh CreateMesh(List<Vector3> buffsPoint)
    {

        Mesh mesh = new Mesh();

        if (buffsPoint == null || buffsPoint.Count <= 1)
            return mesh;

        List<Vector3> VesctorBuffs = new List<Vector3>();
        Vector3 normalPos = Vector3.zero;
        Vector3 lastPos = buffsPoint[0];
        List<float> uvLent = new List<float>();
        uvLent.Add(0);
        uvLent.Add(0);
        float len = 0;

        float tempWidth = 1;
        for (int i = 0; i < buffsPoint.Count - 1; i++)
        {
            //宽度可以配置
            int index = i / mSize;

            float widex = mBuffsPoint[index].width;
            float widexNext = mBuffsPoint.Count > (index+1)? mBuffsPoint[index+1].width: widex;

            Vector3 startPos = buffsPoint[i];
            Vector3 endPos = buffsPoint[i + 1];

            len += Vector3.Distance(endPos, startPos) * _TextureDp;
            uvLent.Add(len);
            uvLent.Add(len);

            normalPos = endPos - startPos;

            normalPos.Normalize();
            normalPos = Quaternion.Euler(new Vector3(0, 0, 90)) * normalPos;

            tempWidth = Mathf.Lerp(widex, widexNext, (i % mSize)/(float)mSize);

            VesctorBuffs.Add(startPos + normalPos * tempWidth);
            VesctorBuffs.Add(startPos - normalPos * tempWidth);
        }

        VesctorBuffs.Add(buffsPoint[buffsPoint.Count - 1] + normalPos * tempWidth);
        VesctorBuffs.Add(buffsPoint[buffsPoint.Count - 1] - normalPos * tempWidth);

        mesh.vertices = VesctorBuffs.ToArray();

        Vector2[] uv = new Vector2[VesctorBuffs.Count];

        for (int i = 0; i < VesctorBuffs.Count; i++)
        {
            float uvX = uvLent[i];
            //uvX = uvX - (int)uvX;
            uv[i] = new Vector2(uvX, i % 2 == 0 ? 0 : 1);
        }
        mesh.uv = uv;


        int[] triangles = new int[VesctorBuffs.Count * 3 - 6];

        int zheng = triangles.Length / 6;
        for (int i = 0; i < zheng; i++)
        {
            int index = i * 6;
            int ver = i * 2;
            triangles[index] = ver;
            triangles[index + 1] = ver + 2;
            triangles[index + 2] = ver + 1;

            triangles[index + 3] = ver + 2;
            triangles[index + 4] = ver + 3;
            triangles[index + 5] = ver + 1;
        }
        mesh.triangles = triangles;
        return mesh;
    }
}

public class PointController
{
    /// <summary>
    /// 获取曲线上面的所有点
    /// </summary>
    /// <returns>The list.</returns>
    /// <param name="path">需要穿过的点列表</param>
    /// <param name="pointSize">两个点之间的节点数量</param>
    public static Vector3[] PointList(Vector3[] path, int pointSize)
    {
        Vector3[] controlPointList = PathControlPointGenerator(path);

        int smoothAmount = path.Length*pointSize;
        Vector3[] pointList = new Vector3[smoothAmount+1];

        for (int index = 0; index < pointList.Length; index++)
        {
            Vector3 currPt = Interp(controlPointList, index / (float)smoothAmount);
            pointList[index] = currPt;
        }
        
        return pointList;
    }

    /// <summary>
    /// 获取控制点
    /// </summary>
    /// <returns>The control point generator.</returns>
    /// <param name="path">Path.</param>
    private static Vector3[] PathControlPointGenerator(Vector3[] path)
    {
        int offset = 2;
        Vector3[] suppliedPath = path;
        Vector3[] controlPoint = new Vector3[suppliedPath.Length + offset];
        Array.Copy(suppliedPath, 0, controlPoint, 1, suppliedPath.Length);

        controlPoint[0] = controlPoint[1] + (controlPoint[1] - controlPoint[2]);
        controlPoint[controlPoint.Length - 1] = controlPoint[controlPoint.Length - 2] + (controlPoint[controlPoint.Length - 2] - controlPoint[controlPoint.Length - 3]);

        if (controlPoint[1] == controlPoint[controlPoint.Length - 2])
        {
            Vector3[] tmpLoopSpline = new Vector3[controlPoint.Length];
            Array.Copy(controlPoint, tmpLoopSpline, controlPoint.Length);
            tmpLoopSpline[0] = tmpLoopSpline[tmpLoopSpline.Length - 3];
            tmpLoopSpline[tmpLoopSpline.Length - 1] = tmpLoopSpline[2];
            controlPoint = new Vector3[tmpLoopSpline.Length];
            Array.Copy(tmpLoopSpline, controlPoint, tmpLoopSpline.Length);
        }

        return (controlPoint);
    }

    /// <summary>
    /// 根据 T 获取曲线上面的点位置
    /// </summary>
    /// <param name="pts">Pts.</param>
    /// <param name="t">T.</param>
    private static Vector3 Interp(Vector3[] pts, float t)
    {
        int numSections = pts.Length - 3;
        int currPt = Mathf.Min(Mathf.FloorToInt(t * (float)numSections), numSections - 1);
        float u = t * (float)numSections - (float)currPt;

        Vector3 a = pts[currPt];
        Vector3 b = pts[currPt + 1];
        Vector3 c = pts[currPt + 2];
        Vector3 d = pts[currPt + 3];

        return .5f * (
            (-a + 3f * b - 3f * c + d) * (u * u * u)
            + (2f * a - 5f * b + 4f * c - d) * (u * u)
            + (-a + c) * u
            + 2f * b
            );
    }
}
