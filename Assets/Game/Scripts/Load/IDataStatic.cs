public interface IDataStatic
{
    void LoginGame(string eventName);

    void SetPlayerInfo(string userID);

    void OnEnterBaseScene(string eventName);

    void StartGameSuccess(string eventName);

    void DownloadResSuccess(string eventName);
}


public class DataStaticConst
{
    public const string LoginGame = "Login game";
    public const string EnterBase = "EnterBase";
    public const string StartGame = "EnterGame";
    public const string SetPlayerInfo = "SetPlayerInfo";
    public const string DownloadRes = "complete download";
}