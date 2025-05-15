using UnityEngine;

public class AnimationEventManager : MonoBehaviour
{
    public GameObject playerObject;

    public void ActivateHitbox()
    {
        playerObject.transform.Find("Hitbox").GetComponent<PolygonCollider2D>().enabled = true;
        //playerObject.GetComponent<PolygonCollider2D>().enabled = true;
        if (!playerObject.GetComponent<PlayerMovement>().IsInCombat()) playerObject.GetComponent<PlayerMovement>().SpawnSpellEffect();

    }

    public void DeactivateHitbox()
    {
        playerObject.transform.Find("Hitbox").GetComponent<PolygonCollider2D>().enabled = false;
        playerObject.gameObject.GetComponent<PlayerMovement>().ResetElemOnAttackFinish();
        //playerObject.GetComponent<PolygonCollider2D>().enabled = false;
    }

    public void ActivateIFrames()
    {
        playerObject.GetComponent<BoxCollider2D>().enabled = false;
    }

    public void DeactivateIFrames()
    {
        playerObject.GetComponent<BoxCollider2D>().enabled = true;
    }
}
