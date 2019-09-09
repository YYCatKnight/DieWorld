//----------------------------------------------
//  F2DEngine: time: 2015.10  by fucong QQ:353204643
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


namespace F2DEngine
{
    public class FMySubSprite : FUIObject
    {

        public class MeshMat
        {
            public Mesh mMesh;
            public Material mMat;
        }
        MeshFilter _meshFilter;
        MeshRenderer _renderer;
        private int mSizeX = 0;
        private int mSizeY = 0;
        private GameObject mGo;

        private MeshMat mMeshMat;
        private Vector3[] mVers;
        private Color[] mColors;

        private Vector3[] mCurVers;
        private Color[] mCurColors;

        private int mOffseX;
        private int mOffseY;
        private Vector3 mUnitSize = Vector3.zero;
        private Mesh mMainMesh;
        private string mDefalueMaterialName = null;

        public void SetDefalueMaterial(string mat)
        {
            mDefalueMaterialName = mat;
        }
        private T[] CopyData<T>(T[] buf)
        {
            T[] tmep = new T[buf.Length];
            for (int i = 0; i < buf.Length; i++)
            {
                tmep[i] = buf[i];
            }
            return tmep;
        }

        public void SetMeshMat(MeshMat mesh)
        {
            mMeshMat = mesh;
        }
        public MeshMat GetMesh()
        {
            return mMeshMat;
        }
        public void CreateClip(int xOffse, int yOffse, int xClipSize, int yClipSize, float xSize, float ySize, GameObject go)
        {

            if (mMeshMat == null)
            {
                mMeshMat = getMesh(go);
            }
            mSizeY = yClipSize;
            mSizeX = xClipSize;
            mOffseX = xOffse;
            mOffseY = yOffse;
            _meshFilter = GetComponent<MeshFilter>();

            _meshFilter = _meshFilter == null ? gameObject.AddComponent<MeshFilter>() : _meshFilter;

            mMainMesh = CreateCombineClipMesh(xOffse, yOffse, xClipSize, yClipSize, xSize, ySize, mMeshMat.mMesh);
            _meshFilter.sharedMesh = mMainMesh;

            _renderer = GetComponent<MeshRenderer>();

            _renderer = _renderer == null ? gameObject.AddComponent<MeshRenderer>() : _renderer;

            _renderer.material = mMeshMat.mMat;
        }
        private Mesh CreateCombineClipMesh(int xOffse, int yOffse, int xClipSize, int yClipSize, float xSize, float ySize, Mesh me)
        {
            int numIndex = 0;
            int maxNum = (int)(xClipSize * yClipSize);
            //顶点数据
            Vector3[] vertices = new Vector3[maxNum * 4];
            //顶点索引
            int[] tri = new int[maxNum * 6];
            //颜色
            Color[] colors = new Color[maxNum * 4];
            //向量
            Vector3[] normals = new Vector3[maxNum * 4];

            //纹理uv
            Vector2[] uv = new Vector2[maxNum * 4];
            Vector2[] uv1 = new Vector2[maxNum * 4];
            Vector3[] tempVert = new Vector3[4];

            Mesh _mesh = new Mesh();
            mUnitSize.x = (me.vertices[1].x - me.vertices[0].x) / xSize;
            mUnitSize.y = (me.vertices[2].y - me.vertices[0].y)/ySize;

            Vector3 uvSize = Vector3.zero;
            uvSize.x = (-me.uv[0].x + me.uv[1].x)/xSize;
            uvSize.y = (-me.uv[0].y + me.uv[2].y)/ySize;

            for (int x = xOffse; x < xClipSize + xOffse; x++)
                for (int y = yOffse; y < yClipSize + yOffse; y++)
                {

                    int startIndex = numIndex * 4;
                    //   2----3 
                    //   |    |  ->三角形索引
                    //   0----1

                    vertices[0 + startIndex] = new Vector3(me.vertices[0].x + mUnitSize.x * (float)x, me.vertices[0].y + mUnitSize.y * (float)y, me.vertices[0].z);
                    vertices[1 + startIndex] = new Vector3(me.vertices[0].x + mUnitSize.x * (float)(x + 1), me.vertices[0].y + mUnitSize.y * (float)y, me.vertices[1].z);
                    vertices[2 + startIndex] = new Vector3(me.vertices[0].x + mUnitSize.x * (float)x, me.vertices[0].y + mUnitSize.y * (float)(y + 1), me.vertices[2].z);
                    vertices[3 + startIndex] = new Vector3(me.vertices[0].x + mUnitSize.x * (float)(x + 1), me.vertices[0].y + mUnitSize.y * (float)(y + 1), me.vertices[3].z);

                    startIndex = numIndex * 6;
                    int ex = numIndex * 4;
                    //三角形
                    tri[0 + startIndex] = 0 + ex;
                    tri[1 + startIndex] = 3 + ex;
                    tri[2 + startIndex] = 1 + ex;

                    tri[3 + startIndex] = 0 + ex;
                    tri[4 + startIndex] = 2 + ex;
                    tri[5 + startIndex] = 3 + ex;


                    //normal
                    startIndex = numIndex * 4;
                    colors[0 + startIndex] = me.colors[0];
                    colors[1 + startIndex] = me.colors[0];
                    colors[2 + startIndex] = me.colors[0];
                    colors[3 + startIndex] = me.colors[0];

                    startIndex = numIndex * 4;
                    //_mesh.colors = colors;                 
                    normals[0 + startIndex] = -Vector3.forward;
                    normals[1 + startIndex] = -Vector3.forward;
                    normals[2 + startIndex] = -Vector3.forward;
                    normals[3 + startIndex] = -Vector3.forward;

                    //uv           
                    float tempX = x;
                    float tempY = y;
                    float tempSizeX = xSize;
                    float tempSizeY = ySize;

                    startIndex = numIndex * 4;
                    uv[0 + startIndex] = new Vector2(me.uv[0].x + uvSize.x * (float)x, me.uv[0].y + uvSize.y * (float)y);
                    uv[1 + startIndex] = new Vector2(me.uv[0].x + uvSize.x * (float)(x + 1), me.uv[0].y + uvSize.y * (float)y);
                    uv[2 + startIndex] = new Vector2(me.uv[0].x + uvSize.x * (float)x, me.uv[0].y + uvSize.y * (float)(y + 1));
                    uv[3 + startIndex] = new Vector2(me.uv[0].x + uvSize.x * (float)(x + 1), me.uv[0].y + uvSize.y * (float)(y + 1));

                    uv1[0 + startIndex] = new Vector2(0,0);
                    uv1[1 + startIndex] = new Vector2(1, 0);
                    uv1[2 + startIndex] = new Vector2(0, 1);
                    uv1[3 + startIndex] = new Vector2(1, 1);

                    numIndex++;
                }

            _mesh.vertices = vertices;
            _mesh.normals = normals;
            _mesh.uv = uv;
            _mesh.uv2 = uv1;
            _mesh.colors = colors;
            _mesh.triangles = tri;

            mVers = CopyData<Vector3>(vertices);
            mColors = CopyData<Color>(colors);
            mCurVers = vertices;
            mCurColors = colors;
            return _mesh;
        }

        private MeshMat getMesh(GameObject go)
        {
            //tk2dSprite tk = go.GetComponent<tk2dSprite>();
            //if (tk != null)
            //{
            //    return getMesh(tk);
            //}

            //UISprite us = go.GetComponent<UISprite>();
            //if (us != null)
            //{
            //    return getMesh(us);
            //}

            //UITexture ut = go.GetComponent<UITexture>();
            //if (ut)
            //{
            //    return getMesh(ut);
            //}
            SpriteRenderer ut = go.GetComponent<SpriteRenderer>();
            if (ut)
            {
                return getMesh(ut);
            }

            RawImage ri = go.GetComponent<RawImage>();
            if(ri)
            {
                return getMesh(ri);
            }
            return new MeshMat();
        }

        private MeshMat getMesh(SpriteRenderer sr)
        {
            MeshMat mm = new MeshMat();
            mm.mMat = new Material(SceneManager.LoadPrefab<Material>(mDefalueMaterialName == "" ? ResConfig.MATERIAL_DEFAULT : mDefalueMaterialName));
            Sprite sp = sr.sprite;
            mm.mMat.mainTexture = sp.texture;
            float width = sp.rect.width/sp.pixelsPerUnit;
            float height = sp.rect.height/sp.pixelsPerUnit;
            Mesh mesh = new Mesh();

            Vector3[] vertices = new Vector3[4];
            vertices[0] = new Vector3((-width / 2), (-height / 2), 0);
            vertices[1] = new Vector3((width / 2), (-height / 2), 0);
            vertices[2] = new Vector3((width / 2), (height / 2), 0);
            vertices[3] = new Vector3((-width / 2), (height / 2), 0);
            mesh.vertices = vertices;

            //Vector2[] uv = new Vector2[4];
            //uv[0] = new Vector2(sp.textureRect.x, sp.textureRect.y);
            //uv[1] = new Vector2(sp.textureRect.width, sp.textureRect.y);
            //uv[2] = new Vector2(sp.textureRect.width, sp.textureRect.height);
            //uv[3] = new Vector2(sp.textureRect.x, sp.textureRect.height);

            Vector2[] uv = new Vector2[4];
            uv[0] = new Vector2(0, 0);
            uv[1] = new Vector2(1, 0);
            uv[2] = new Vector2(1, 1);
            uv[3] = new Vector2(0, 1);

            mesh.uv = uv;

            int[] tri = new int[6];
            tri[0] = 0;
            tri[1] = 3;
            tri[2] = 1;

            tri[3] = 1;
            tri[4] = 3;
            tri[5] = 2;
            mesh.triangles = tri;

            Color[] colors = new Color[4];

            colors[0] = sr.color;
            colors[1] = sr.color;
            colors[2] = sr.color;
            colors[3] = sr.color;
            mesh.colors = colors;
            mm.mMesh = mesh;
            return mm;
        }
        private MeshMat getMesh(RawImage ri)
        {
            MeshMat mm = new MeshMat();
            mm.mMat = new Material(SceneManager.LoadPrefab<Material>(mDefalueMaterialName == "" ? ResConfig.MATERIAL_DEFAULT : mDefalueMaterialName));
            mm.mMat.mainTexture = ri.mainTexture;
            float width = ri.rectTransform.sizeDelta.x;
            float height = ri.rectTransform.sizeDelta.y;
            Mesh mesh = new Mesh();

            Vector3[] vertices = new Vector3[4];
            vertices[0] = new Vector3((-width / 2), (-height / 2), 0);
            vertices[1] = new Vector3((width / 2), (-height / 2), 0);
            vertices[2] = new Vector3((width / 2), (height / 2), 0);
            vertices[3] = new Vector3((-width / 2), (height / 2), 0);
            mesh.vertices = vertices;

            Vector2[] uv = new Vector2[4];
            uv[0] = new Vector2(ri.uvRect.x, ri.uvRect.y);
            uv[1] = new Vector2(ri.uvRect.width, ri.uvRect.y);
            uv[2] = new Vector2(ri.uvRect.width, ri.uvRect.height);
            uv[3] = new Vector2(ri.uvRect.x, ri.uvRect.height);

            mesh.uv = uv;

            int[] tri = new int[6];
            tri[0] = 0;
            tri[1] = 3;
            tri[2] = 1;

            tri[3] = 1;
            tri[4] = 3;
            tri[5] = 2;
            mesh.triangles = tri;

            Color[] colors = new Color[4];

            colors[0] = ri.color;
            colors[1] = ri.color;
            colors[2] = ri.color;
            colors[3] = ri.color;
            mesh.colors = colors;
            mm.mMesh = mesh;
            return mm;
        }
        //private MeshMat getMesh(UITexture ut)
        //{
        //    MeshMat mm = new MeshMat();

        //    mm.mMat = new Material(ut.shader);
        //    mm.mMat.mainTexture = ut.mainTexture;


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

        //    Color[] colors = new Color[4];

        //    colors[0] = ut.color;
        //    colors[1] = ut.color;
        //    colors[2] = ut.color;
        //    colors[3] = ut.color;

        //    mesh.colors = colors;
        //    mm.mMesh = mesh;
        //    return mm;
        //}

        //private MeshMat getMesh(tk2dSprite sp)
        //{
        //    MeshMat mm = new MeshMat();
        //    mm.mMat = sp.GetComponent<Renderer>().material;
        //    mm.mMesh =  sp.gameObject.GetComponent<MeshFilter>().sharedMesh;
        //    return mm;
        //}


        //private MeshMat getMesh(UISprite sp)
        //{
        //    MeshMat mm = new MeshMat();
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

        //    mesh.uv = uv;

        //    int[] tri = new int[6];
        //    tri[0] = 0;
        //    tri[1] = 3;
        //    tri[2] = 1;

        //    tri[3] = 1;
        //    tri[4] = 3;
        //    tri[5] = 2;
        //    mesh.triangles = tri;

        //    Color[] colors = new Color[4];

        //    colors[0] = sp.color;
        //    colors[1] = sp.color;
        //    colors[2] = sp.color;
        //    colors[3] = sp.color;

        //    mesh.colors = colors;
        //    mm.mMesh = mesh;
        //    mm.mMat = sp.material;
        //    return mm;
        //}

        public void UpdateMesh()
        {
            _meshFilter.sharedMesh.vertices = mCurVers;
            _meshFilter.sharedMesh.colors = mCurColors;
        }

        public void Reset()
        {
            for (int i = 0; i < mCurColors.Length; i++)
            {
                mCurColors[i] = mColors[i];
                mCurVers[i] = mVers[i];
            }
            UpdateMesh();
        }

        public override void Clear()
        {
            Color c = new Color(0, 0, 0, 0);
            for(int i = 0; i < mCurColors.Length;i++)
            {
                mCurColors[i] = c;
            }
            UpdateMesh();
        }
        public int GetClipNum()
        {
            return mSizeX * mSizeY;
        }
        public void TransColor(int x, int y, Color c)
        {
            int sSize = (x * mSizeY + y) * 4;
            mCurColors[sSize] = c;
            mCurColors[sSize + 1] = c;
            mCurColors[sSize + 2] = c;
            mCurColors[sSize + 3] = c;
        }
        public void Transpos(int x, int y, Vector3 pos)
        {
            Transpos(x, y, pos, Vector3.one);
        }
        public void TransRelativePos(int x, int y, Vector3 pos, Vector3 s)
        {
            Transpos(x-mOffseX, y-mOffseY, pos, s);
        }
        public void TransRelativeColor(int x,int y,Color c)
        {
            TransColor(x - mOffseX, y - mOffseY, c);
        }
        public void Transpos(int x, int y, Vector3 pos, Vector3 s)
        {
            Vector3 cc = (mUnitSize / 2);
            cc.x *= (1-s.x);
            cc.y *= (1-s.y);
            cc.z = 0;
            Vector3 c1 = cc;
            c1.x = cc.x * -1;
            int sSize = (x * mSizeY + y) * 4;
            mCurVers[sSize] = mVers[sSize] + pos + cc;
            mCurVers[sSize + 1] = mVers[sSize + 1] + pos + c1;
            mCurVers[sSize + 2] = mVers[sSize + 2] + pos-c1;
            mCurVers[sSize + 3] = mVers[sSize + 3] + pos - cc;
        }
    }

}
