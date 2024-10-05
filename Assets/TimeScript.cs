using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TimeScript : MonoBehaviour
{
    private int score = 0;
    IEnumerator DoCheck()
    {
        for (; ; )
        {
            score++;
            this.GetComponent<Text>().text = score.ToString();
            yield return new WaitForSeconds(1f);
        }
    }

    private void Start()
    {
        StartCoroutine("DoCheck");
    }

    private void Update()
    {
        
    }
}