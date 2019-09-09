using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using F2DEngine;
using UnityEngine.UI;

public class StartLoad : BaseLoad
{
    private Slider m_slider;
    private Text m_enterText;
    private MsgMesh m_msgMesh;
    private FUniversalPanel m_main;
    private GameType m_gameType;
    private LoadPercent m_loading;
    private GameObject m_enterPanel;
    private GameObject m_loadPanel;
    private bool m_ShowEnter = false;
    private enum GameType
    {
        GT_NONE,
        GT_ResoureEnd,
        GT_LoadScene,
        GT_SceneEnd,
        GT_Over
    }

    public override bool Init()
    {
        m_msgMesh = new MsgMesh();
        m_ShowEnter = true;
        m_main = GetComponent<FUniversalPanel>();
        if (m_main != null)
        {
            m_slider = m_main.GetFObject<Slider>("F_Slider");
            m_enterPanel = m_main.GetFObject("F_EnterPlane");
            m_loadPanel = m_main.GetFObject("F_LoadPlane");
            m_enterText = m_main.GetFObject<Text>("F_Title");
            m_enterPanel.SetActive(false);
            m_loadPanel.SetActive(true);
            m_gameType = GameType.GT_NONE;
            if (mMainPlane == null)
            {
                m_loading = new LoadPercent();
                m_loading.SetTimece(1);
                m_loading.GoOn(1);
            }
            else
            {
                m_loading = mMainPlane.mLoadPercent;
            }
            m_msgMesh.RegEvent(EventStrArray.KeyCodeEvent.KEYCODE_ENTER_DOWN, (f) =>
            {
                FEngine.IsInit = true;
                BeginLoadScene();
          //      EventListenManager.Send(EventStrArray.PlayerCtrlEvent.ENTER_GAME);
            });
        }
        else
        {
            Debug.Log("this == null");
        }
        return base.Init();
    }

    private void Update()
    {
        if (m_gameType == GameType.GT_NONE)
        {
            if (m_loading != null)
            {
                var dp = m_loading.GetPercent();
            }
        }
        else
        {
            if (m_gameType == GameType.GT_LoadScene)
            {
                if (m_loading != null)
                {
                    var dp = m_loading.GetPercent();
                    if (m_slider.value < dp.pre)
                    {
                        m_slider.value += Time.deltaTime;
                    }
                }
                else
                {
                    m_slider.value += Time.deltaTime * 5;
                }
            }
            else if (m_gameType == GameType.GT_SceneEnd)
            {
                float timeDp = m_slider.value + Time.deltaTime * 1;
                if (timeDp > 1)
                {
                    m_gameType = GameType.GT_Over;
                    m_slider.value = 1;
                }
                else
                {
                    m_slider.value = timeDp;
                }
            }
            if (m_slider.value < 0.3f)
            {
                m_slider.value += Time.deltaTime / 100f;
            }
        }
    }

    IEnumerator FirstEnterGame()
    {
        FEngine.IsConnecting = false;
        FEngine.IsInit = false;

        m_enterPanel.SetActive(false);
        if (!m_ShowEnter)
        {
            m_loadPanel.SetActive(true);
            m_slider.value = 0;
            yield return 0;
            yield return 0;
            m_loadPanel.SetActive(true);
        }
        yield return 0;

        m_msgMesh.RegEvent(EventStrArray.PlayerCtrlEvent.ENTER_GAME, (f) =>
        {
            FEngine.IsInit = true;
            BeginLoadScene();
        });
    }

    private void BeginLoadScene()
    {
        m_loadPanel.SetActive(true);
        m_enterPanel.SetActive(false);
        m_slider.value = 0;
        m_gameType = GameType.GT_LoadScene;
    }

    public override IEnumerator PlayResoureOver()
    {
        if (!PlayerPrefs.HasKey(ConstStr.FirstStartGame))
        {
            DataStaticMgr.OnEnterBase();
            PlayerPrefs.SetString(ConstStr.FirstStartGame, "True");
        }
        SceneManager.instance.ComputeLangTable();
        if (!m_ShowEnter)
        {
            StartCoroutine(FirstEnterGame());
        }
        m_enterPanel.SetActive(m_ShowEnter);
        m_loadPanel.SetActive(!m_enterPanel.activeInHierarchy);
        m_gameType = GameType.GT_ResoureEnd;
        while(m_gameType == GameType.GT_ResoureEnd)
        {
            yield return 0;
        }
    }
}
