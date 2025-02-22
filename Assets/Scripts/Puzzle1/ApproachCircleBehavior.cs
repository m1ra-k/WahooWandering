using UnityEngine;

public class ApproachCircleBehavior : MonoBehaviour
{
    void Update()
    {
        switch (gameObject.name)
        {
            case "ApproachCircleNormal(Clone)":
                ApproachCircleNormalBehavior();
                break;
            case "ApproachCircleFast(Clone)":
                ApproachCircleFastBehavior();
                break;
            case "ApproachCircleSlow(Clone)":
                ApproachCircleSlowBehavior();
                break;
        }

        transform.parent.GetComponent<HitCircleBehavior>().UpdateApproachCircleChildrenSorted(gameObject.transform);
    }

    void ApproachCircleFastBehavior()
    {
        transform.localScale += new Vector3(0.0385f, 0.0385f, 0.0385f); // 26 frames per beat, 0.43 seconds (quarter note)
    }

    void ApproachCircleNormalBehavior()
    {
        transform.localScale += new Vector3(0.0192f, 0.0192f, 0.0192f); // 52 frames per beat, 0.86 seconds (half note)
    }

    void ApproachCircleSlowBehavior()
    {
        transform.localScale += new Vector3(0.0097f, 0.0097f, 0.0097f); // 104 frames per beat, 1.72 seconds (whole note)
    }
}
