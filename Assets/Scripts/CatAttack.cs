using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CatAttack : MonoBehaviour
{

    [SerializeField]
    private GameObject attackPrefab;

    [SerializeField]
    private int mouseAttackKey;

    [SerializeField]
    private float attackInterval;

    [SerializeField]
    private Slider energyBar;


    [SerializeField]
    private Material projectileMaterial;

    


    private List<Transform> attackPoints;
    private float timeSinceLastAttack;
    private void Start()
    {
        attackPoints = new List<Transform>();
        foreach(Transform child in transform) 
        {
            attackPoints.Add(child);
        }

       
        timeSinceLastAttack = attackInterval;
    }
    // Update is called once per frame
    void Update()
    {
        timeSinceLastAttack += Time.deltaTime;
        CheckAttack();
    }

    private void CheckAttack() 
    {
        if (Input.GetMouseButton(mouseAttackKey) && timeSinceLastAttack >= attackInterval)
        {
            if (energyBar.value - 1 > 0) 
            {
                timeSinceLastAttack = 0;
                int attackPointCount = 0;
                foreach (Transform attackPoint in attackPoints)
                {
                    attackPointCount++;
                    GameObject attackClone = Instantiate(attackPrefab);
                    attackClone.transform.position = attackPoint.position;
                    attackClone.GetComponent<SpriteRenderer>().material = projectileMaterial;
                    RotateObjectToMouse(attackClone);
                }

                energyBar.value -= attackPointCount;
            }
            

        }
    }

    private void RotateObjectToMouse(GameObject rotateObject) 
    {

        Vector3 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - rotateObject.transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        rotateObject.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

}
