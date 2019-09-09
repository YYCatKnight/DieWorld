//----------------------------------------------
//  F2DEngine: time: 2015.10  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;

namespace F2DEngine
{
    
    public abstract class FSceneTemplate<T> : FBaseController<T> where T : FSceneTemplate<T>
    {
        protected MsgMesh mMsgMesh = new MsgMesh();
        sealed protected override void Init()
        {
            if (append_fengine())
            {
                if(!FEngineManager.IsInitFEngine())
                {
                    return;
                }
            }
            var obj = LoadSceneManager.instance.CurMode;
            object[] arg = obj == null?null:obj.GetParams();
            Begin(arg);
        }

        sealed public override void End()
        {
            mMsgMesh.Clear();
            EndScene();
        }

        public virtual void EndScene()
        {

        }

        protected virtual bool append_fengine()
        {
            return true;
        }

        public abstract void Begin(params object[] objs);
    }
}
