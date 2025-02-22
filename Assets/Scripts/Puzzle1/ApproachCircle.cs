using System.Collections.Generic;
using UnityEngine;

public class ApproachCircle : MonoBehaviour
{
    public bool approach;

    public List<GameObject> hitCircles;
    public List<GameObject> approachCirclePrefabs;

    public void GenerateApproachCircle(ApproachCircleTypeEnum approachCircleTypeEnum, int hitCircleNumber)
    {
        GameObject prefab = approachCirclePrefabs[0];;

        switch (approachCircleTypeEnum)
        {
            case ApproachCircleTypeEnum.Fast:
                prefab = approachCirclePrefabs[1];
                break;
            case ApproachCircleTypeEnum.Slow:
                prefab = approachCirclePrefabs[2];
                break;
        }

        GameObject approachCircle = Instantiate(prefab);

        switch (hitCircleNumber)
        {
            case 1:
                approachCircle.transform.position = new Vector2(hitCircles[0].transform.position.x, hitCircles[0].transform.position.y);
                approachCircle.transform.SetParent(hitCircles[0].transform);
                break;
            case 2:
                approachCircle.transform.position = new Vector2(hitCircles[1].transform.position.x, hitCircles[1].transform.position.y);
                approachCircle.transform.SetParent(hitCircles[1].transform);
                break;
            case 3:
                approachCircle.transform.position = new Vector2(hitCircles[2].transform.position.x, hitCircles[2].transform.position.y);
                approachCircle.transform.SetParent(hitCircles[2].transform);
                break;
            case 4:
                approachCircle.transform.position = new Vector2(hitCircles[3].transform.position.x, hitCircles[3].transform.position.y);
                approachCircle.transform.SetParent(hitCircles[3].transform);
                break;
            case 5:
                approachCircle.transform.position = new Vector2(hitCircles[4].transform.position.x, hitCircles[4].transform.position.y);
                approachCircle.transform.SetParent(hitCircles[4].transform);
                break;
        }
    }
}
