using UnityEngine;

public class GNDCollision : MonoBehaviour {

    private GameObject spawner;
    private GameObject floor;

    // Use this for initialization
    void Start()
    {
        spawner = GameObject.FindGameObjectWithTag("spawner");
        //floor = GameObject.FindGameObjectWithTag("floor");
        //this.GetComponent<Renderer>().enabled = false;
    }

    //when obj collides with something it gets that position and moves spawner to that height
    //ideally this obj collides with the ground mesh.
    void OnCollisionEnter(Collision col)
    {
        floor.SetActive(false);
        Vector3 pos = col.contacts[0].point;
        Vector3 temp = spawner.transform.position;
        temp.y = pos.y - 0.05f;
        spawner.transform.position = temp;
        floor.SetActive(true);
        Destroy(this.gameObject);
    }
}
