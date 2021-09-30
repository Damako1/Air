using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileGuidanceSystem : MonoBehaviour
{
    #region 变量
    [SerializeField] Projectile projectile;
    Vector3 overdriveDistance;

    [SerializeField] float minAngle;
    [SerializeField] float maxAngle;
    #endregion

    float ballisticAngle;
    #region 方法
    public IEnumerator HomingCorotine(GameObject traget)
    {
        ballisticAngle = Random.Range(minAngle, maxAngle);
        while (gameObject.activeSelf)
        {
            if (traget.activeSelf)
            {
                overdriveDistance =traget.transform.position- transform.position;

                float angle = Mathf.Atan2(overdriveDistance.y, overdriveDistance.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                transform.rotation *= Quaternion.Euler(0, 0, ballisticAngle);
                projectile.Move();
            }
            else
            {
                projectile.Move();
            }
            yield return null;
        }
    }
    #endregion
}
