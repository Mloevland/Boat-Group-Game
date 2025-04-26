using UnityEngine;

namespace MA.Events
{
    [CreateAssetMenu(fileName = "New Game Object Event", menuName = "Game Events/GameObject Event")]
    public class GameObjectEvent : BaseGameEvent<GameObject> { }
}