//----------------------------------------------
//  F2DEngine: time: 2015.10  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace F2DEngine
{
    public class FMergeSprite : UnitObject
    {

        MeshFilter _meshFilter;
        MeshRenderer _renderer;
        private List<CombineInstance> mMeshList = new List<CombineInstance>();

        public static FMergeSprite PalyMerge(GameObject go)
        {
            FMergeSprite fms = (FMergeSprite)SceneManager.instance.CreateObject(ResConfig.CC_FMERGESPRITE,go.transform.parent.gameObject);
            fms.gameObject.transform.localPosition = go.transform.localPosition;
            fms.CreateClip(go);
            GameObject.Destroy(go);
            return fms;
        }
        private void CreateClip(GameObject go)
        {

            _meshFilter = GetComponent<MeshFilter>();

            _meshFilter = _meshFilter ?? gameObject.AddComponent<MeshFilter>();

            CreateCombineMeshs(go);

            _meshFilter.sharedMesh = new Mesh();

            UpdateMesh();

            _renderer = GetComponent<MeshRenderer>();

            _renderer = _renderer ?? gameObject.AddComponent<MeshRenderer>();

            _renderer.material = null;
        }

        private void CreateCombineMeshs(GameObject go)
        {

            mMeshList.Clear();
            Transform[] subTrans = go.GetComponentsInChildren<Transform>(true);
            for (int index = 0; index < subTrans.Length; index++)
            {
                Transform sub = subTrans[index];
                Mesh me = GetMesh(sub.gameObject);
                if (me == null)
                    continue;

                CombineInstance ci = new CombineInstance();
                ci.subMeshIndex = 0;
                ci.mesh = me;
                ci.transform = this.transform.worldToLocalMatrix*sub.localToWorldMatrix;
                mMeshList.Add(ci);
            }
        }

        private Mesh GetMesh(GameObject go)
        {
            return null;
        }
        //private Mesh getMesh(GameObject go)
        //{
        //    tk2dSprite tk = go.GetComponent<tk2dSprite>();
        //    if (tk != null)
        //    {
        //        return getMesh(tk);
        //    }

        //    //UISprite us = go.GetComponent<UISprite>();
        //    //if (us != null)
        //    //{
        //    //    return getMesh(us);
        //    //}

        //    //UITexture ut = go.GetComponent<UITexture>();
        //    //if (ut)
        //    //{
        //    //    return getMesh(ut);
        //    //}
        //    return null;
        //}

        //private Mesh getMesh(UITexture ut)
        //{
        //    mMat = new Material(ut.shader);
        //    mMat.mainTexture = ut.mainTexture;


        //    int width = ut.width;
        //    int height = ut.height;
        //    Mesh mesh = new Mesh();

        //    Vector3[] vertices = new Vector3[4];
        //    vertices[0] = new Vector3((-width / 2), (-height / 2), 0);
        //    vertices[1] = new Vector3((width / 2), (-height / 2), 0);
        //    vertices[2] = new Vector3((width / 2), (height / 2), 0);
        //    vertices[3] = new Vector3((-width / 2), (height / 2), 0);
        //    mesh.vertices = vertices;

        //    Vector2[] uv = new Vector2[4];
        //    uv[0] = new Vector2(0, 0);
        //    uv[1] = new Vector2(1, 0);
        //    uv[2] = new Vector2(1, 1);
        //    uv[3] = new Vector2(0, 1);

        //    mesh.uv = uv;


        //    int[] tri = new int[6];
        //    tri[0] = 0;
        //    tri[1] = 3;
        //    tri[2] = 1;

        //    tri[3] = 1;
        //    tri[4] = 3;
        //    tri[5] = 2;
        //    mesh.triangles = tri;


        //    return mesh;
        //}

        //private Mesh getMesh(tk2dSprite sp)
        //{
        //    mMat = sp.GetComponent<Renderer>().material;
        //    return sp.gameObject.GetComponent<MeshFilter>().sharedMesh;
        //}


        //private Mesh getMesh(UISprite sp)
        //{
        //    int width = sp.width;
        //    int height = sp.height;
        //    UISpriteData spData = sp.atlas.GetSprite(sp.spriteName);
        //    Rect rect0 = new Rect();
        //    rect0.Set(spData.x + spData.borderLeft, spData.y + spData.borderTop,
        //        spData.width - spData.borderLeft - spData.borderRight,
        //        spData.height - spData.borderBottom - spData.borderTop);
        //    Rect rectNew0 = NGUIMath.ConvertToTexCoords(rect0, sp.mainTexture.width, sp.mainTexture.height);
        //    Rect rect1 = new Rect(
        //            spData.x - spData.paddingLeft,
        //            spData.y - spData.paddingTop,
        //            spData.width + spData.paddingLeft + spData.paddingRight,
        //            spData.height + spData.paddingTop + spData.paddingBottom);


        //    Rect rectNew1 = NGUIMath.ConvertToTexCoords(rect1, sp.mainTexture.width, sp.mainTexture.height);

        //    float scalX = (rectNew0.xMax - rectNew0.xMin) / (rectNew1.xMax - rectNew1.xMin);
        //    float scalY = (rectNew0.yMax - rectNew0.yMin) / (rectNew1.yMax - rectNew1.yMin);

        //    int xx = Mathf.RoundToInt(sp.atlas.pixelSize * (spData.width + spData.paddingLeft + spData.paddingRight)) + 1;
        //    int yy = Mathf.RoundToInt(sp.atlas.pixelSize * (spData.height + spData.paddingTop + spData.paddingBottom)) + 1;

        //    Vector2 centerTemp = new Vector2(0, 0);
        //    centerTemp.x = -((rect1.xMax + rect1.xMin) / 2 - (rect0.xMax + rect0.xMin) / 2) * width / xx;
        //    centerTemp.y = ((rect1.yMax + rect1.yMin) / 2 - (rect0.yMax + rect0.yMin) / 2) * height / yy;

        //    Mesh mesh = new Mesh();

        //    Vector3[] vertices = new Vector3[4];
        //    vertices[0] = new Vector3((-width / 2 * scalX + centerTemp.x), (-height / 2 * scalY + centerTemp.y), 0);
        //    vertices[1] = new Vector3((width / 2 * scalX + centerTemp.x), (-height / 2 * scalY + centerTemp.y), 0);
        //    vertices[2] = new Vector3((width / 2 * scalX + centerTemp.x), (height / 2 * scalY + centerTemp.y), 0);
        //    vertices[3] = new Vector3((-width / 2 * scalX + centerTemp.x), (height / 2 * scalY + centerTemp.y), 0);
        //    mesh.vertices = vertices;

        //    Vector2[] uv = new Vector2[4];
        //    uv[0] = new Vector2(rectNew0.x, rectNew0.y);
        //    uv[1] = new Vector2(rectNew0.xMax, rectNew0.y);
        //    uv[2] = new Vector2(rectNew0.xMax, rectNew0.yMax);
        //    uv[3] = new Vector2(rectNew0.x, rectNew0.yMax);


        //    int[] tri = new int[6];
        //    tri[0] = 0;
        //    tri[1] = 3;
        //    tri[2] = 1;

        //    tri[3] = 1;
        //    tri[4] = 3;
        //    tri[5] = 2;
        //    mesh.triangles = tri;

        //    mesh.uv = uv;
        //    mMat = sp.material;
        //    return mesh;
        //}

        public void UpdateMesh()
        {
            _meshFilter.sharedMesh.Clear();
            _meshFilter.sharedMesh.CombineMeshes(mMeshList.ToArray(), true);
        }

    }
}
