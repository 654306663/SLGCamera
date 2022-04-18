// =======================================================
// 文件：TransformUtil.cs
// 描述：常用的坐标转换
// 作者：王冲
// 创建时间：2021/9/9 9:01:45
// =======================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformUtil
{
    /// <summary>
    /// 获取两点之间指定百分比的一个点
    /// </summary>
    public static Vector3 GetBetweenPointForPercent(Vector3 start, Vector3 end, float percent = 0.5f)
    {
        Vector3 normal = (end - start).normalized;
        float distance = GetDistance(start, end);
        return normal * (distance * percent) + start;
    }

    /// <summary>
    /// 获取两点之间指定距离的点
    /// </summary>
    public static Vector3 GetBetweenPointForDistance(Vector3 start, Vector3 end, float distance)
    {
        Vector3 normal = (end - start).normalized;
        return normal * distance + start;
    }

    /// <summary>
    /// 获取两点之间距离
    /// </summary>
    public static float GetDistance(Vector2 start, Vector2 end)
    {
        return Mathf.Sqrt((end.x - start.x) * (end.x - start.x) + (end.y - start.y) * (end.y - start.y));
    }

    /// <summary>
    /// 获取两点之间距离
    /// </summary>
    public static float GetDistance(Vector3 start, Vector3 end)
    {
        return Mathf.Sqrt((end.x - start.x) * (end.x - start.x) + (end.y - start.y) * (end.y - start.y) + (end.z - start.z) * (end.z - start.z));
    }

    /// <summary>
    /// 获取两点之间距离 Float参数
    /// </summary>
    public static float GetDistance_F(float startX, float startY, float endX, float endY)
    {
        return Mathf.Sqrt((endX - startX) * (endX - startX) + (endY - startY) * (endY - startY));
    }
}
