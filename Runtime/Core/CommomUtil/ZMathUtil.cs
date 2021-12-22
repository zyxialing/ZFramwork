using UnityEngine;
namespace ZFramework
{

    public class ZMathUtil
    {
        public const float const_wh_scale = 0.5625f;//1080f/1920f

        //求最大公约数
        public static int GetGCD(int m, int n)
        {
            int r, t;
            if (m < n)
            {
                t = n;
                n = m;
                m = t;
            }
            while (n != 0)
            {
                r = m % n;
                m = n;
                n = r;

            }
            return (m);
        }

        /// <summary>
        /// 根据曲线获取值
        /// </summary>
        /// <param name="x">输入</param>
        /// <param name="animationCurve">曲线</param>
        /// <returns></returns>
        public static float GetEaseInOutFloat(float x,AnimationCurve animationCurve)
        {
          return  animationCurve.Evaluate(x);
        }


    }
}