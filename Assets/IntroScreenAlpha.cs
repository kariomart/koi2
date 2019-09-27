using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroScreenAlpha : MonoBehaviour
{
    // Start is called before the first frame update
    SpriteRenderer sprite;
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();   
        StartCoroutine(fadeOut());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    IEnumerator fadeOut() {

        float a = sprite.color.a;

        for (int i = 0; i < 500; i++)
        {
            a-=.005f;
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, a);
            yield return new WaitForSeconds(.001f);
        }

        Destroy(this.gameObject);
    }
}
