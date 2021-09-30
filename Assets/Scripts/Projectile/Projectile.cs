using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    #region 变量

    [Header("子弹命中特效")]
    [SerializeField]GameObject hitVFX;

    [Header("子弹命中音效")]
    [SerializeField] AudioData[] hitSFX;

    [Header("伤害值")]
    [SerializeField]float damage;

    [Header("子弹移动速度")]
    [SerializeField] float moveSpeed = 10f;

    [Header("子弹移动方向")]
    [SerializeField] protected Vector2 moveDirection;

    [Header("子弹瞄准的目标")]
    [SerializeField] protected GameObject target;
    #endregion

    #region 生命周期

    protected virtual void OnEnable()
    {
        StartCoroutine(MoveDirectly());
    }

    #endregion

    #region 方法

    /// <summary>
    /// 子弹移动
    /// </summary>
    /// <returns></returns>
    IEnumerator MoveDirectly()
    {
        while (gameObject.activeSelf)
        {
            Move();

            yield return null;
        }
    }

    /// <summary>
    /// 当子弹产生碰撞
    /// </summary>
    /// <param name="collision"></param>
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Character>(out Character character))
        {
            character.TakeDamage(damage);

            var contactPoint = collision.GetContact(0);
            PoolManager.Release(hitVFX, contactPoint.point,Quaternion.LookRotation(contactPoint.normal));//命中特效

            AudioManager.Instance.PlayRandomSFX(hitSFX);//命中音效
            gameObject.SetActive(false);
        }
    }

    public void Move()=> transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    

    protected void SetTarget(GameObject target)
    {
        this.target = target;
    }




    #endregion
}
