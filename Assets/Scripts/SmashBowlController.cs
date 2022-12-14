using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SmashBowlController : MonoBehaviour
{
    [SerializeField] private BarrelSpawnArea barrelSpawnArea;
    [SerializeField] private Animator grapeSmashGirl;
    [SerializeField] private ParticleSystem smashParticle;
    [SerializeField] private Image FillImage;
    [SerializeField] private GameObject FillImageCanvas;
    [SerializeField] private float workTime = 5f;
    [SerializeField] private int grapeNeeded = 2;
    [SerializeField] private int grapeCounter = 0;
    
    public bool active = false;
    public bool working = false;
    private bool once = false;
    
    void Start()
    {
        if (gameObject.activeInHierarchy)
        {
            active = true;
        }
    }

    public void PlayerGrapeDropping(int value)
    {
        FillImageCanvas.SetActive(true);
        grapeCounter = Mathf.Clamp(grapeCounter + value, 0, grapeNeeded);
        FillImage.fillAmount += 1.0f/  grapeNeeded;
        transform.DOKill();
        transform.DOPunchScale(Vector3.down, 0.3f).SetEase(Ease.InBounce);
    }
    
    void Update()
    {
        if (barrelSpawnArea.barrelAreaMax)
        {
            working = true;
        }
        else
        {
            working = false;
        }
        
        if (grapeCounter == grapeNeeded)
        {
            working = true;
            if (!once)
            {
                StartCoroutine(SmashWorking());
                once = true;
            }
        }
    }

    private IEnumerator SmashWorking()
    {
        FillImageCanvas.SetActive(false);
        FillImage.fillAmount = 0;
        grapeSmashGirl.SetBool("working", true);
        smashParticle.Play();
        yield return new WaitForSeconds(workTime);
        grapeSmashGirl.SetBool("working", false);
        smashParticle.Stop();
        working = false;
        once = false;
        grapeCounter = 0;
        GameEventHandler.current.BarrelGenerator();
        yield return null;
    }
}
