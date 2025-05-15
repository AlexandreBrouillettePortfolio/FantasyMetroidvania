using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChildDestroyed(string name)
    {
        if (name.Equals("IndicatorPillar"))
        {
            transform.Find("LargeObjects").transform.Find("Colliders").transform.Find("FakeFloor").gameObject.SetActive(false);
            transform.Find("Grid").transform.Find("Foreground").gameObject.SetActive(false);
        }
        if (name.Equals("IndicatorPillarTutorial"))
        {
            transform.Find("Colliders").transform.Find("FakeCeiling").gameObject.SetActive(false);
            transform.Find("Grid").transform.Find("FakeCeiling").gameObject.SetActive(false);
        }
    }
}
