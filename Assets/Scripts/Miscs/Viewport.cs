using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viewport : Singleton<Viewport>
{
    float minX;
    float minY;
    float maxX;
    float maxY;
    /// <summary>
    /// 视口中心点的世界坐标X
    /// </summary>
    float middleX;
    private void Start()
    {
        Camera mainCamera = Camera.main;

        Vector2 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector2(0f, 0f));
        Vector2 topRight = mainCamera.ViewportToWorldPoint(new Vector2(1f, 1f));

        minX = bottomLeft.x;
        minY = bottomLeft.y;
        maxX = topRight.x;
        maxY = topRight.y;

        middleX = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0f, 0f)).x;
    }

    public Vector3 PlayerMoveablePosition(Vector3 playerPosition,float paddingX,float paddingY)
    {
        Vector3 position = Vector3.zero;
        position.x= Mathf.Clamp(playerPosition.x, minX+paddingX, maxX-paddingX);
        position.y = Mathf.Clamp(playerPosition.y, minY+paddingY, maxY-paddingY);
        return position;
    }

    /// <summary>
    /// 随机敌人生成位置
    /// </summary>
    /// <param name="paddingX">对象模型的边距值</param>
    /// <param name="paddingY">对象模型的边距值</param>
    /// <returns></returns>
    public Vector3 RandomEnemySpawnPosition(float paddingX,float paddingY)
    {
        Vector3 position = Vector3.zero;

        position.x = maxX+paddingX;
        position.y = Random.Range(minY+paddingY, maxY-paddingY);

        return position;
    }

    /// <summary>
    /// 随机右半部分的位置
    /// </summary>
    /// <returns></returns>
    public Vector3 RandomRightHalfPosition(float paddingX, float paddingY)
    {
        Vector3 position = Vector3.zero;

        position.x = Random.Range(middleX,maxX-paddingX);
        position.y = Random.Range(minY + paddingY, maxY - paddingY);

        return position;
    }

    /// <summary>
    /// 全屏范围内移动
    /// </summary>
    /// <param name="paddingX"></param>
    /// <param name="paddingY"></param>
    /// <returns></returns>
    public Vector3 RandomEnemyMovePosition(float paddingX, float paddingY)
    {
        Vector3 position = Vector3.zero;

        position.x = Random.Range(minX+paddingX, maxX - paddingX);
        position.y = Random.Range(minY + paddingY, maxY - paddingY);

        return position;
    }
}
