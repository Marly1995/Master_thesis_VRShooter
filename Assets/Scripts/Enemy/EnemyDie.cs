using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class EnemyDie : MonoBehaviour
{
    GameObject player;
    MeshRenderer render;
    
    [SerializeField]
    GameObject add;

    bool hit = false;

    public int points = 300;

    private void Start()
    {
        render = GetComponent<MeshRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void Die()
    {
        if (!hit)
        {
            hit = true;
            StartCoroutine(DissolveOut());
            WorldState.Score += points;
            GameObject obj = Instantiate(add, transform.position, Quaternion.LookRotation(player.transform.position, Vector3.up));
            obj.GetComponentInChildren<Text>().text = "+" + points.ToString();
            obj.transform.LookAt(player.transform);
            Destroy(obj, 2.0f);
        }
    }

    IEnumerator DissolveOut()
    {
        for (float i = 0f; i <= 1f; i += 0.04f)
        {
            render.material.SetFloat("_SliceAmount", i);
            yield return new WaitForSeconds(0.0002f);
        }
        Destroy(gameObject);
    }
}
