public class ChangeSceneEvent : IEvent
{
    public string SceneName;
    public ChangeSceneEvent(string sceneName) => SceneName = sceneName;
}