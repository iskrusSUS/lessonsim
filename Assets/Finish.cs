using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Finish : MonoBehaviour
{
    bool col = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator WaitAndLoadScene(string str)
    {
        GameObject.Find("Text").GetComponent<Text>().text = str;
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!col)
        {
            col = true;
            string text;
            if (collision.gameObject.name == "Car")
            {
                text = "PLAYER WINS!";
            }
            else if (collision.gameObject.name.Contains("CarWaypointBased"))
                text = "BOT WINS!";
            else
                return;

            StartCoroutine(WaitAndLoadScene(text));
        }
    }
}
