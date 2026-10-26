using UnityEngine;

namespace Faust.QoLChests.Components;

public class InteractableUsed : MonoBehaviour
{
    public static InteractableUsed Mark(GameObject obj) =>
        obj.GetComponent<InteractableUsed>() ?? obj.AddComponent<InteractableUsed>();
}
