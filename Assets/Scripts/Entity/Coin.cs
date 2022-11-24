using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class Coin : MonoBehaviour
{
    [Header("Coin")]
    [SerializeField] private int spriteFps = 15;
    [SerializeField] private float speed = 1.3f;
    [SerializeField] private Sprite[] coinSprites = null;
    [SerializeField] private GameObject coinParticle = null;

    [Header("Coin Bounce Stats")]
    [SerializeField] private float jumpHeightMin = 2f;
    [SerializeField] private float jumpHeightMax = 4f;
    [SerializeField] private float landingXmin = -1f;
    [SerializeField] private float landingXmax = 0.5f;
    [SerializeField] private float middleHeight = 0.1f;


    SpriteRenderer rend;
    
    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        StartCoroutine(AnimateCoinSpin());
        StartCoroutine(AnimateCoinJump());
    }

    IEnumerator AnimateCoinSpin()
    {
        int i = 0;
        spriteFps += Random.Range(-6, 6);
        while (true)
        {
            //Loops through the sprite list to create an animation
            rend.sprite = coinSprites[i % coinSprites.Length];
            i++;
            yield return new WaitForSeconds(1f / spriteFps);
        }
    }

    IEnumerator AnimateCoinJump()
    {
        //Initial bounce

        Vector2 spawnLocation = transform.position;
        float landingVector = Random.Range(landingXmin, landingXmax);
        Vector2 endLocation = new Vector2(spawnLocation.x + landingVector, Random.Range(-2.5f, -2f));
        Vector2 middle = new Vector2(Mathf.Lerp(spawnLocation.x, endLocation.x, 0.5f), spawnLocation.y + Random.Range(jumpHeightMin, jumpHeightMax));
        
        float t = 0;
        while (t < 1)       //Bezier curve loop where t, (0-1) is the coin's position on the curve
        {
            //Bezier calculation for 3 points in 2d space
            Vector2 currentPos = middle + Mathf.Pow((1 - t), 2) * (spawnLocation - middle) + Mathf.Pow(t, 2) * (endLocation - middle);
            transform.position = currentPos;

            t += Time.deltaTime * speed;
            yield return new WaitForEndOfFrame();
        }

        //Sound plays when coin hits the ground
        SoundManager.instance.PlaySound("coin", 1f, 1f, 0.4f);


        //Second bounce

        spawnLocation = endLocation;
        endLocation = new Vector2(landingVector * 0.3f + endLocation.x, endLocation.y);
        middle = new Vector2(Mathf.Lerp(spawnLocation.x, endLocation.x, 0.5f), middleHeight);

        t = 0;
        while (t < 0.25f)   //Second bezier curve ends short
        {
            Vector2 currentPos = middle + Mathf.Pow((1 - t), 2) * (spawnLocation - middle) + Mathf.Pow(t, 2) * (endLocation - middle);
            transform.position = currentPos;

            t += Time.deltaTime * speed;
            yield return new WaitForEndOfFrame();
        }

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        //When a coin disappears it shows a particle and adds 1 to player's coin amount
        Instantiate(coinParticle, transform.position, Quaternion.identity);
        PlayerManager.instance.ModifyCoin(1);
    }
}
