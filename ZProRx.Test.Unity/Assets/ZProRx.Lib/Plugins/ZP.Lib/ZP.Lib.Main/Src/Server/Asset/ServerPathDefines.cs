
using ZP.Lib;

public sealed class  ServerPath : PropObjectSingleton<ServerPath>
{
    public static string WorkPath = "../..";  
    public static string AppName = "ZProApp";
    public readonly string ASSETS = $"{WorkPath}/Assets/";
    public readonly string APP_ASSETS = $"{WorkPath}/Assets/{AppName}/";
    public readonly string RESOURCES = $"{WorkPath}/Assets/Resources/";
    public readonly string APP_RESOURCES = $"{WorkPath}/Assets/Resources/{AppName}/";

    public readonly string JSONS_PATH = $"{WorkPath}/Assets/Resources/{AppName}/Jsons/";
    public readonly string SCENE_ROOT = $"{WorkPath}/Assets/Resources/{AppName}/Jsons/Server/";
    public readonly string PREFABS = $"{WorkPath}/Assets/Resources/{AppName}/Jsons/Server/Prefabs.json";
    public readonly string GALAXYAREAS = $"{WorkPath}/Assets/Resources/{AppName}/Jsons/Server/Map/";
}