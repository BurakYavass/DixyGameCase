using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BarmenController : ObjectID
{
    public static BarmenController current;
    [SerializeField] private List<GameObject> FullGlass = new List<GameObject>();
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject glassPrefab;
    [SerializeField] private Transform glassSpawnPoint;
    [SerializeField] private Image FillImage;
    public int spawnCounter = 0;
    private bool spawnable = false;
    private int spawnMax = 4;

    private void Awake()
    {
        if (current == null)
        {
            current = this;
        }
    }
    public void PlayerOnBar(int spawn)
    {
        if (FullBarrelArea.current.barIsWorkable && spawnCounter <5 && !spawnable)
        {
            spawnable = true;
            spawnCounter = Mathf.Clamp(spawnCounter + spawn, 0, spawnMax);
            var playerStackPoint = PlayerStackList.current.stackList[PlayerStackList.current.stackList.Count -1];
            FillImage.DOFillAmount(1,0.3f)
                .OnComplete((() =>
                        {
                            var glass = Instantiate(glassPrefab, glassSpawnPoint.position, glassPrefab.transform.rotation) as GameObject;
                            glass.transform.DOJump(playerStackPoint.transform.position, 5, 1, 0.3f)
                                .SetEase(Ease.OutFlash)
                                .OnComplete((() =>
                                {
                                    PlayerStackList.current.stackList.Add(glass);
                                    FillImage.fillAmount = 0;
                                    glass.transform.DOKill();
                                    spawnable = false;
                                }));
                            
                        }
                    ));
        }
    }

    public void WaiterOnBar(int spawn)
    {
        if (FullBarrelArea.current.barIsWorkable && spawnCounter <5 && !spawnable)
        {
            spawnable = true;
            spawnCounter = Mathf.Clamp(spawnCounter+spawn, 0, spawnMax);
            var waiterStackPoint = WaiterStackList.current.stackList[WaiterStackList.current.stackList.Count -1];
            FillImage.DOFillAmount(1,0.3f)
                .OnComplete((() =>
                {
                    var glass = Instantiate(glassPrefab, glassSpawnPoint.position, glassPrefab.transform.rotation) as GameObject;
                    glass.transform.DOJump(waiterStackPoint.transform.position, 5, 1, 0.3f).SetEase(Ease.OutFlash)
                        .OnComplete((() =>
                        {
                            WaiterStackList.current.stackList.Add(glass);
                            FillImage.fillAmount = 0;
                            glass.transform.DOKill();
                            spawnable = false;
                        }));
                    
                }));
        }
    }

    void Update()
    {
        if (spawnCounter == spawnMax)
        {
            FullBarrelArea.current.BarrelControl();
            FullBarrelArea.current.Barrels[0].gameObject.SetActive(false);
            spawnCounter = 0;
        }
        
    }
}
