using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using F2DEngine;

public class Client : FBaseController<Client>
{
    private void Start()
    {
        LoadSceneManager.instance.LoadScene(GameProgress.GP_City, LoadMode.SetPName(ResConfig.FFRISTLOAD));
    }
}
