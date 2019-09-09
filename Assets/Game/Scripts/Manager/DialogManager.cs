using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using F2DEngine;

public class DialogManager : ManagerTemplate<DialogManager>
{
    public enum DialogType
    {
        DT_Tips_Normal
    }

    public interface IDialogTool
    {
    }

    public class DialogParams : IDialogTool
    {
        public object[] paramss;
    }

    public class DialogContex : IDialogTool
    {
        public string key;
        public string color;
        public object[] paramss;
    }

    public class DialogOK : IDialogTool
    {
        public System.Func<bool> okFunc;
    }

    public class DialogCancel : IDialogTool
    {
        public System.Func<bool> cancelFunc;
    }

    public static DialogParams Params(params object[] objs)
    {
        DialogParams dialogParams = new DialogParams
        {
            paramss = objs
        };
        return dialogParams;
    }

    public static DialogContex Contex(string _key, string _color, params object[] objs)
    {
        DialogContex dialogContex = new DialogContex
        {
            key = _key,
            color = _color,
            paramss = objs
        };
        return dialogContex;
    }
}
