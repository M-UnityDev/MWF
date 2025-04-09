using UnityEngine;
using DG.Tweening;
using System;
using Unity.Burst;
[BurstCompile] public class MainUIDirector : MonoBehaviour
{
    [SerializeField] private AudioSource HUGEASS;
    [SerializeField] private float Multiplier;
    [SerializeField] private int RoundUntil;
    private float a;
    private float[] spectrum;
    private void Awake() => spectrum = new float[1024];
    private void LateUpdate()
    {
	    HUGEASS.GetOutputData(spectrum, 1);
        foreach (float i in spectrum)
	    {
            if (i.Equals(0))
	        {
                a = 0.95f;
	    	    break;
	        }
	        else a = Mathf.Clamp((float)Math.Round(i*1,RoundUntil)*Multiplier, 0.9f, 0.95f);
	    }
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one*a,50*Time.deltaTime);
	    //transform.DOScale(a, 0.05f).SetEase(Ease.Linear);
    }
}
