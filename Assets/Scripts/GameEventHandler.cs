using System;
using UnityEngine;

public class GameEventHandler : MonoBehaviour
{
    public static GameEventHandler current;

    private void Awake()
    {
        if (current == null)
        {
            current = this;
        }
    }
    public event Action<float> OnUpgradeTriggerEnter;

    public event Action OnUpgradeTriggerExit;

    public event Action BarrelGenerate;

    public event Action<Transform,Vector3> ActiveEmptyDesk;
    

    public void EmptyDesk(Transform desk, Vector3 deskRotation)
    {
        ActiveEmptyDesk?.Invoke(desk,deskRotation);
    }

    public void BarrelGenerator()
    {
        BarrelGenerate?.Invoke();
    }
    

    public void UpgradeTriggerEnter(float value)
    {
        OnUpgradeTriggerEnter?.Invoke(value);
    }

    public void UpgradeTriggerExit()
    {
        OnUpgradeTriggerExit?.Invoke();
    }
}
