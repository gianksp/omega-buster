using UnityEngine;
using System.Collections;


namespace Xft
{

    public static class XEase
    {
        public enum Easing
        {
            Linear,

            InQuad,
            OutQuad,
            InOutQuad,
            OutInQuad,

            InCubic,
            OutCubic,
            InOutCubic,
            OutInCubic,

            InQuart,
            OutQuart,
            InOutQuart,
            OutInQuart,

            InQuint,
            OutQuint,
            InOutQuint,
            OutInQuint,

            InSin,
            OutSin,
            InOutSin,
            OutInSin,

            InExp,
            OutExp,
            InOutExp,
            OutInExp,

            InCirc,
            OutCirc,
            InOutCirc,
            OutInCirc,

            InElastic,
            OutElastic,
            InOutElastic,
            OutInElastic,

            InBounce,
            OutBounce,
            InOutBounce,
            OutInBounce,

            InBack,
            OutBack,
            InOutBack,
            OutInBack
        }

        public static float EaseByType(Easing e, float start, float end, float t)
        {
            switch (e)
            {
                case Easing.Linear: return Linear(start, end, t);

                case Easing.InQuad: return InQuad(start, end, t);
                case Easing.OutQuad: return OutQuad(start, end, t);
                case Easing.InOutQuad: return InOutQuad(start, end, t);
                case Easing.OutInQuad: return OutInQuad(start, end, t);

                case Easing.InCubic: return InCubic(start, end, t);
                case Easing.OutCubic: return OutCubic(start, end, t);
                case Easing.InOutCubic: return InOutCubic(start, end, t);
                case Easing.OutInCubic: return OutInCubic(start, end, t);

                case Easing.InQuart: return InQuart(start, end, t);
                case Easing.OutQuart: return OutQuart(start, end, t);
                case Easing.InOutQuart: return InOutQuart(start, end, t);
                case Easing.OutInQuart: return OutInQuart(start, end, t);

                case Easing.InQuint: return InQuint(start, end, t);
                case Easing.OutQuint: return OutQuint(start, end, t);
                case Easing.InOutQuint: return InOutQuint(start, end, t);
                case Easing.OutInQuint: return OutInQuint(start, end, t);

                case Easing.InSin: return InSin(start, end, t);
                case Easing.OutSin: return OutSin(start, end, t);
                case Easing.InOutSin: return InOutSin(start, end, t);
                case Easing.OutInSin: return OutInSin(start, end, t);

                case Easing.InExp: return InExp(start, end, t);
                case Easing.OutExp: return OutExp(start, end, t);
                case Easing.InOutExp: return InOutExp(start, end, t);
                case Easing.OutInExp: return OutInExp(start, end, t);

                case Easing.InCirc: return InCirc(start, end, t);
                case Easing.OutCirc: return OutCirc(start, end, t);
                case Easing.InOutCirc: return InOutCirc(start, end, t);
                case Easing.OutInCirc: return OutInCirc(start, end, t);

                case Easing.InElastic: return InElastic(start, end, t);
                case Easing.OutElastic: return OutElastic(start, end, t);
                case Easing.InOutElastic: return InOutElastic(start, end, t);
                case Easing.OutInElastic: return OutInElastic(start, end, t);

                case Easing.InBounce: return InBounce(start, end, t);
                case Easing.OutBounce: return OutBounce(start, end, t);
                case Easing.InOutBounce: return InOutBounce(start, end, t);
                case Easing.OutInBounce: return OutInBounce(start, end, t);

                case Easing.InBack: return InBack(start, end, t);
                case Easing.OutBack: return OutBack(start, end, t);
                case Easing.InOutBack: return InOutBack(start, end, t);
                case Easing.OutInBack: return OutInBack(start, end, t);

                default: return 0;
            }
        }

        public static float Linear(float start, float end, float t)
        {
            return t * (end - start) + start;
        }

        public static float InQuad(float start, float end, float t)
        {
            return t * t * (end - start) + start;
        }

        public static float OutQuad(float start, float end, float t)
        {
            return -(t * (t - 2)) * (end - start) + start;
        }

        public static float InOutQuad(float start, float end, float t)
        {
            t *= 2;
            if (t < 1)
            {
                return 0.5f * t * t * (end - start) + start;
            }
            else
            {
                --t;
                return -0.5f * (t * (t - 2) - 1) * (end - start) + start;
            }
        }

        public static float OutInQuad(float start, float end, float t)
        {
            t *= 2;
            if (t < 1)
            {
                return (-0.5f * (t * (t - 2) - 1) - 0.5f) * (end - start) + start;
            }
            else
            {
                --t;
                return (0.5f * t * t + 0.5f) * (end - start) + start;
            }
        }

        public static float InCubic(float start, float end, float t)
        {
            return t * t * t * (end - start) + start;
        }

        public static float OutCubic(float start, float end, float t)
        {
            --t;
            return (t * t * t + 1) * (end - start) + start;
        }

        public static float InOutCubic(float start, float end, float t)
        {
            t *= 2;
            if (t < 1)
            {
                return 0.5f * t * t * t * (end - start) + start;
            }
            else
            {
                t -= 2;
                return 0.5f * (t * t * t + 2) * (end - start) + start;
            }
        }

        public static float OutInCubic(float start, float end, float t)
        {
            t *= 2;
            if (t < 1)
            {
                --t;
                return (0.5f * (t * t * t + 2) - 0.5f) * (end - start) + start;
            }
            else
            {
                --t;
                return (0.5f * t * t * t + 0.5f) * (end - start) + start;
            }
        }

        public static float InQuart(float start, float end, float t)
        {
            return t * t * t * t * (end - start) + start;
        }

        public static float OutQuart(float start, float end, float t)
        {
            --t;
            return -(t * t * t * t - 1) * (end - start) + start;
        }

        public static float InOutQuart(float start, float end, float t)
        {
            t *= 2;
            if (t < 1)
            {
                return 0.5f * t * t * t * t * (end - start) + start;
            }
            else
            {
                t -= 2;
                return -0.5f * (t * t * t * t - 2) * (end - start) + start;
            }
        }

        public static float OutInQuart(float start, float end, float t)
        {
            t *= 2;
            if (t < 1)
            {
                --t;
                return (-0.5f * t * t * t * t + 0.5f) * (end - start) + start;
            }
            else
            {
                --t;
                return (0.5f * t * t * t * t + 0.5f) * (end - start) + start;
            }
        }

        public static float InQuint(float start, float end, float t)
        {
            return t * t * t * t * t * (end - start) + start;
        }

        public static float OutQuint(float start, float end, float t)
        {
            --t;
            return (t * t * t * t * t + 1) * (end - start) + start;
        }

        public static float InOutQuint(float start, float end, float t)
        {
            t *= 2;
            if (t < 1)
            {
                return (0.5f * t * t * t * t * t) * (end - start) + start;
            }
            else
            {
                t -= 2;
                return (0.5f * t * t * t * t * t + 1) * (end - start) + start;
            }
        }

        public static float OutInQuint(float start, float end, float t)
        {
            t *= 2;
            if (t < 1)
            {
                --t;
                return (0.5f * (t * t * t * t * t) + 0.5f) * (end - start) + start;
            }
            else
            {
                --t;
                return (0.5f * t * t * t * t * t + 0.5f) * (end - start) + start;
            }
        }

        public static float InSin(float start, float end, float t)
        {
            return -Mathf.Cos(t * Mathf.PI * 0.5f) * (end - start) + end;
        }

        public static float OutSin(float start, float end, float t)
        {
            return Mathf.Sin(t * Mathf.PI * 0.5f) * (end - start) + start;
        }

        public static float InOutSin(float start, float end, float t)
        {
            return (-0.5f * Mathf.Cos(t * Mathf.PI) + 0.5f) * (end - start) + start;
        }

        public static float OutInSin(float start, float end, float t)
        {
            return (t - (-0.5f * Mathf.Cos(t * Mathf.PI) + 0.5f - t)) * (end - start) + start;
        }

        public static float InExp(float start, float end, float t)
        {
            return Mathf.Pow(2, 10 * (t - 1)) * (end - start) + start;
        }

        public static float OutExp(float start, float end, float t)
        {
            return (1 - Mathf.Pow(2, -10 * t)) * (end - start) + start;
        }

        public static float InOutExp(float start, float end, float t)
        {
            t *= 2;
            if (t < 1)
            {
                return 0.5f * Mathf.Pow(2, 10 * (t - 1)) * (end - start) + start;
            }
            else
            {
                --t;
                return 0.5f * (2 - Mathf.Pow(2, -10 * t)) * (end - start) + start;
            }
        }

        public static float OutInExp(float start, float end, float t)
        {
            t *= 2;
            if (t < 1)
            {
                return (0.5f * (2 - Mathf.Pow(2, -10 * t)) - 0.5f) * (end - start) + start;
            }
            else
            {
                --t;
                return (0.5f * Mathf.Pow(2, 10 * (t - 1)) + 0.5f) * (end - start) + start;
            }
        }

        public static float InCirc(float start, float end, float t)
        {
            return -(Mathf.Sqrt(1 - t * t) - 1) * (end - start) + start;
        }

        public static float OutCirc(float start, float end, float t)
        {
            --t;
            return Mathf.Sqrt(1 - t * t) * (end - start) + start;
        }

        public static float InOutCirc(float start, float end, float t)
        {
            t *= 2;
            if (t < 1)
            {
                return (-0.5f * (Mathf.Sqrt(1 - t * t) - 1)) * (end - start) + start;
            }
            else
            {
                t -= 2;
                return (0.5f * (Mathf.Sqrt(1 - t * t) + 1)) * (end - start) + start;
            }
        }

        public static float OutInCirc(float start, float end, float t)
        {
            t *= 2;
            if (t < 1)
            {
                --t;
                return (0.5f * Mathf.Sqrt(1 - t * t)) * (end - start) + start;
            }
            else
            {
                --t;
                return (-0.5f * Mathf.Sqrt(1 - t * t) + 1) * (end - start) + start;
            }
        }

        public static float InElastic(float start, float end, float t)
        {
            float p = 0.3f, s = 0.075f;
            --t;
            return (-Mathf.Pow(2, 10 * t) * Mathf.Sin((t - s) * (2 * Mathf.PI) / p)) * (end - start) + start;
        }

        public static float OutElastic(float start, float end, float t)
        {
            float p = 0.3f, s = 0.075f;
            return (Mathf.Pow(2, -10 * t) * Mathf.Sin((t - s) * (2 * Mathf.PI) / p)) * (end - start) + end;
        }

        public static float InOutElastic(float start, float end, float t)
        {
            float p = 0.3f, s = 0.075f;
            t *= 2;
            if (t < 1)
            {
                --t;
                return (-0.5f * Mathf.Pow(2, 10 * t) * Mathf.Sin((t - s) * (2 * Mathf.PI) / p)) * (end - start) + start;
            }
            else
            {
                --t;
                return (0.5f * Mathf.Pow(2, -10 * t) * Mathf.Sin((t - s) * (2 * Mathf.PI) / p)) * (end - start) + end;
            }
        }

        public static float OutInElastic(float start, float end, float t)
        {
            float p = 0.3f, s = 0.075f;
            t *= 2;
            if (t < 1)
            {
                return (0.5f * Mathf.Pow(2, -10 * t) * Mathf.Sin((t - s) * (2 * Mathf.PI) / p) - 0.5f) * (end - start) + end;
            }
            else
            {
                t -= 2;
                return (-0.5f * Mathf.Pow(2, 10 * t) * Mathf.Sin((t - s) * (2 * Mathf.PI) / p) + 0.5f) * (end - start) + start;
            }
        }

        public static float InBounce(float start, float end, float t)
        {

            t = 1 - t;
            if (t < 1 / 2.75f)
            {
                return (-7.5625f * t * t + 1) * (end - start) + start;
            }
            else if (t < 2.0f / 2.75f)
            {
                t -= 1.5f / 2.75f;
                return (-7.5625f * t * t - 0.75f + 1) * (end - start) + start;
            }
            else if (t < 2.5f / 2.75f)
            {
                t -= 2.25f / 2.75f;
                return (-7.5625f * t * t - 0.9375f + 1) * (end - start) + start;
            }
            else
            {
                t -= 2.625f / 2.75f;
                return (-7.5625f * t * t - 0.984375f + 1) * (end - start) + start;
            }
        }

        public static float OutBounce(float start, float end, float t)
        {

            if (t < 1 / 2.75f)
            {
                return (7.5625f * t * t) * (end - start) + start;
            }
            else if (t < 2.0f / 2.75f)
            {
                t -= 1.5f / 2.75f;
                return (7.5625f * t * t + 0.75f) * (end - start) + start;
            }
            else if (t < 2.5f / 2.75f)
            {
                t -= 2.25f / 2.75f;
                return (7.5625f * t * t + 0.9375f) * (end - start) + start;
            }
            else
            {
                t -= 2.625f / 2.75f;
                return (7.5625f * t * t + 0.984375f) * (end - start) + start;
            }
        }

        public static float InOutBounce(float start, float end, float t)
        {

            t *= 2;
            if (t < 1)
            {
                t = 1 - t;
                if (t < 1 / 2.75f)
                {
                    return (-0.5f * 7.5625f * t * t + 0.5f) * (end - start) + start;
                }
                else if (t < 2.0f / 2.75f)
                {
                    t -= 1.5f / 2.75f;
                    return (-0.5f * (7.5625f * t * t + 0.75f) + 0.5f) * (end - start) + start;
                }
                else if (t < 2.5f / 2.75f)
                {
                    t -= 2.25f / 2.75f;
                    return (-0.5f * (7.5625f * t * t + 0.9375f) + 0.5f) * (end - start) + start;
                }
                else
                {
                    t -= 2.625f / 2.75f;
                    return (-0.5f * (7.5625f * t * t + 0.984375f) + 0.5f) * (end - start) + start;
                }
            }
            else
            {
                --t;
                if (t < 1 / 2.75f)
                {
                    return (0.5f * 7.5625f * t * t + 0.5f) * (end - start) + start;
                }
                else if (t < 2.0f / 2.75f)
                {
                    t -= 1.5f / 2.75f;
                    return (0.5f * (7.5625f * t * t + 0.75f) + 0.5f) * (end - start) + start;
                }
                else if (t < 2.5f / 2.75f)
                {
                    t -= 2.25f / 2.75f;
                    return (0.5f * (7.5625f * t * t + 0.9375f) + 0.5f) * (end - start) + start;
                }
                else
                {
                    t -= 2.625f / 2.75f;
                    return (0.5f * (7.5625f * t * t + 0.984375f) + 0.5f) * (end - start) + start;
                }
            }
        }

        public static float OutInBounce(float start, float end, float t)
        {

            t *= 2;
            if (t < 1)
            {
                if (t < 1 / 2.75f)
                {
                    return (0.5f * 7.5625f * t * t) * (end - start) + start;
                }
                else if (t < 2.0f / 2.75f)
                {
                    t -= 1.5f / 2.75f;
                    return (0.5f * (7.5625f * t * t + 0.75f)) * (end - start) + start;
                }
                else if (t < 2.5f / 2.75f)
                {
                    t -= 2.25f / 2.75f;
                    return (0.5f * (7.5625f * t * t + 0.9375f)) * (end - start) + start;
                }
                else
                {
                    t -= 2.625f / 2.75f;
                    return (0.5f * (7.5625f * t * t + 0.984375f)) * (end - start) + start;
                }
            }
            else
            {
                t = 2 - t;
                if (t < 1 / 2.75f)
                {
                    return (-0.5f * 7.5625f * t * t + 1) * (end - start) + start;
                }
                else if (t < 2.0f / 2.75f)
                {
                    t -= 1.5f / 2.75f;
                    return (-0.5f * (7.5625f * t * t + 0.75f) + 1) * (end - start) + start;
                }
                else if (t < 2.5f / 2.75f)
                {
                    t -= 2.25f / 2.75f;
                    return (-0.5f * (7.5625f * t * t + 0.9375f) + 1) * (end - start) + start;
                }
                else
                {
                    t -= 2.625f / 2.75f;
                    return (-0.5f * (7.5625f * t * t + 0.984375f) + 1) * (end - start) + start;
                }
            }
        }

        public static float InBack(float start, float end, float t)
        {
            return t * t * (2.70158f * t - 1.70158f) * (end - start) + start;
        }

        public static float OutBack(float start, float end, float t)
        {
            --t;
            return (1 - t * t * (-2.70158f * t - 1.70158f)) * (end - start) + start;
        }

        public static float InOutBack(float start, float end, float t)
        {
            t *= 2;
            if (t < 1)
            {
                return (0.5f * t * t * (2.70158f * t - 1.70158f)) * (end - start) + start;
            }
            else
            {
                t -= 2;
                return (1 - 0.5f * t * t * (-2.70158f * t - 1.70158f)) * (end - start) + start;
            }
        }

        public static float OutInBack(float start, float end, float t)
        {
            t *= 2;
            if (t < 1)
            {
                --t;
                return (0.5f - 0.5f * t * t * (-2.70158f * t - 1.70158f)) * (end - start) + start;
            }
            else
            {
                --t;
                return (0.5f * t * t * (2.70158f * t - 1.70158f) + 0.5f) * (end - start) + start;
            }
        }

        public static Vector3 EaseByType(Easing e, Vector3 start, Vector3 end, float t)
        {
            switch (e)
            {
                case Easing.Linear: return Linear(start, end, t);

                case Easing.InQuad: return InQuad(start, end, t);
                case Easing.OutQuad: return OutQuad(start, end, t);
                case Easing.InOutQuad: return InOutQuad(start, end, t);
                case Easing.OutInQuad: return OutInQuad(start, end, t);

                case Easing.InCubic: return InCubic(start, end, t);
                case Easing.OutCubic: return OutCubic(start, end, t);
                case Easing.InOutCubic: return InOutCubic(start, end, t);
                case Easing.OutInCubic: return OutInCubic(start, end, t);

                case Easing.InQuart: return InQuart(start, end, t);
                case Easing.OutQuart: return OutQuart(start, end, t);
                case Easing.InOutQuart: return InOutQuart(start, end, t);
                case Easing.OutInQuart: return OutInQuart(start, end, t);

                case Easing.InQuint: return InQuint(start, end, t);
                case Easing.OutQuint: return OutQuint(start, end, t);
                case Easing.InOutQuint: return InOutQuint(start, end, t);
                case Easing.OutInQuint: return OutInQuint(start, end, t);

                case Easing.InSin: return InSin(start, end, t);
                case Easing.OutSin: return OutSin(start, end, t);
                case Easing.InOutSin: return InOutSin(start, end, t);
                case Easing.OutInSin: return OutInSin(start, end, t);

                case Easing.InExp: return InExp(start, end, t);
                case Easing.OutExp: return OutExp(start, end, t);
                case Easing.InOutExp: return InOutExp(start, end, t);
                case Easing.OutInExp: return OutInExp(start, end, t);

                case Easing.InCirc: return InCirc(start, end, t);
                case Easing.OutCirc: return OutCirc(start, end, t);
                case Easing.InOutCirc: return InOutCirc(start, end, t);
                case Easing.OutInCirc: return OutInCirc(start, end, t);

                case Easing.InElastic: return InElastic(start, end, t);
                case Easing.OutElastic: return OutElastic(start, end, t);
                case Easing.InOutElastic: return InOutElastic(start, end, t);
                case Easing.OutInElastic: return OutInElastic(start, end, t);

                case Easing.InBounce: return InBounce(start, end, t);
                case Easing.OutBounce: return OutBounce(start, end, t);
                case Easing.InOutBounce: return InOutBounce(start, end, t);
                case Easing.OutInBounce: return OutInBounce(start, end, t);

                case Easing.InBack: return InBack(start, end, t);
                case Easing.OutBack: return OutBack(start, end, t);
                case Easing.InOutBack: return InOutBack(start, end, t);
                case Easing.OutInBack: return OutInBack(start, end, t);

                default: return Vector3.zero;
            }
        }

        public static Vector3 Linear(Vector3 start, Vector3 end, float t)
        {
            return t * (end - start) + start;
        }

        public static Vector3 InQuad(Vector3 start, Vector3 end, float t)
        {
            return t * t * (end - start) + start;
        }

        public static Vector3 OutQuad(Vector3 start, Vector3 end, float t)
        {
            return -(t * (t - 2)) * (end - start) + start;
        }

        public static Vector3 InOutQuad(Vector3 start, Vector3 end, float t)
        {
            t *= 2;
            if (t < 1)
            {
                return 0.5f * t * t * (end - start) + start;
            }
            else
            {
                --t;
                return -0.5f * (t * (t - 2) - 1) * (end - start) + start;
            }
        }

        public static Vector3 OutInQuad(Vector3 start, Vector3 end, float t)
        {
            t *= 2;
            if (t < 1)
            {
                return (-0.5f * (t * (t - 2) - 1) - 0.5f) * (end - start) + start;
            }
            else
            {
                --t;
                return (0.5f * t * t + 0.5f) * (end - start) + start;
            }
        }

        public static Vector3 InCubic(Vector3 start, Vector3 end, float t)
        {
            return t * t * t * (end - start) + start;
        }

        public static Vector3 OutCubic(Vector3 start, Vector3 end, float t)
        {
            --t;
            return (t * t * t + 1) * (end - start) + start;
        }

        public static Vector3 InOutCubic(Vector3 start, Vector3 end, float t)
        {
            t *= 2;
            if (t < 1)
            {
                return 0.5f * t * t * t * (end - start) + start;
            }
            else
            {
                t -= 2;
                return 0.5f * (t * t * t + 2) * (end - start) + start;
            }
        }

        public static Vector3 OutInCubic(Vector3 start, Vector3 end, float t)
        {
            t *= 2;
            if (t < 1)
            {
                --t;
                return (0.5f * (t * t * t + 2) - 0.5f) * (end - start) + start;
            }
            else
            {
                --t;
                return (0.5f * t * t * t + 0.5f) * (end - start) + start;
            }
        }

        public static Vector3 InQuart(Vector3 start, Vector3 end, float t)
        {
            return t * t * t * t * (end - start) + start;
        }

        public static Vector3 OutQuart(Vector3 start, Vector3 end, float t)
        {
            --t;
            return -(t * t * t * t - 1) * (end - start) + start;
        }

        public static Vector3 InOutQuart(Vector3 start, Vector3 end, float t)
        {
            t *= 2;
            if (t < 1)
            {
                return 0.5f * t * t * t * t * (end - start) + start;
            }
            else
            {
                t -= 2;
                return -0.5f * (t * t * t * t - 2) * (end - start) + start;
            }
        }

        public static Vector3 OutInQuart(Vector3 start, Vector3 end, float t)
        {
            t *= 2;
            if (t < 1)
            {
                --t;
                return (-0.5f * t * t * t * t + 0.5f) * (end - start) + start;
            }
            else
            {
                --t;
                return (0.5f * t * t * t * t + 0.5f) * (end - start) + start;
            }
        }

        public static Vector3 InQuint(Vector3 start, Vector3 end, float t)
        {
            return t * t * t * t * t * (end - start) + start;
        }

        public static Vector3 OutQuint(Vector3 start, Vector3 end, float t)
        {
            --t;
            return (t * t * t * t * t + 1) * (end - start) + start;
        }

        public static Vector3 InOutQuint(Vector3 start, Vector3 end, float t)
        {
            t *= 2;
            if (t < 1)
            {
                return (0.5f * t * t * t * t * t) * (end - start) + start;
            }
            else
            {
                t -= 2;
                return (0.5f * t * t * t * t * t + 1) * (end - start) + start;
            }
        }

        public static Vector3 OutInQuint(Vector3 start, Vector3 end, float t)
        {
            t *= 2;
            if (t < 1)
            {
                --t;
                return (0.5f * (t * t * t * t * t) + 0.5f) * (end - start) + start;
            }
            else
            {
                --t;
                return (0.5f * t * t * t * t * t + 0.5f) * (end - start) + start;
            }
        }

        public static Vector3 InSin(Vector3 start, Vector3 end, float t)
        {
            return -Mathf.Cos(t * Mathf.PI * 0.5f) * (end - start) + end;
        }

        public static Vector3 OutSin(Vector3 start, Vector3 end, float t)
        {
            return Mathf.Sin(t * Mathf.PI * 0.5f) * (end - start) + start;
        }

        public static Vector3 InOutSin(Vector3 start, Vector3 end, float t)
        {
            return (-0.5f * Mathf.Cos(t * Mathf.PI) + 0.5f) * (end - start) + start;
        }

        public static Vector3 OutInSin(Vector3 start, Vector3 end, float t)
        {
            return (t - (-0.5f * Mathf.Cos(t * Mathf.PI) + 0.5f - t)) * (end - start) + start;
        }

        public static Vector3 InExp(Vector3 start, Vector3 end, float t)
        {
            return Mathf.Pow(2, 10 * (t - 1)) * (end - start) + start;
        }

        public static Vector3 OutExp(Vector3 start, Vector3 end, float t)
        {
            return (1 - Mathf.Pow(2, -10 * t)) * (end - start) + start;
        }

        public static Vector3 InOutExp(Vector3 start, Vector3 end, float t)
        {
            t *= 2;
            if (t < 1)
            {
                return 0.5f * Mathf.Pow(2, 10 * (t - 1)) * (end - start) + start;
            }
            else
            {
                --t;
                return 0.5f * (2 - Mathf.Pow(2, -10 * t)) * (end - start) + start;
            }
        }

        public static Vector3 OutInExp(Vector3 start, Vector3 end, float t)
        {
            t *= 2;
            if (t < 1)
            {
                return (0.5f * (2 - Mathf.Pow(2, -10 * t)) - 0.5f) * (end - start) + start;
            }
            else
            {
                --t;
                return (0.5f * Mathf.Pow(2, 10 * (t - 1)) + 0.5f) * (end - start) + start;
            }
        }

        public static Vector3 InCirc(Vector3 start, Vector3 end, float t)
        {
            return -(Mathf.Sqrt(1 - t * t) - 1) * (end - start) + start;
        }

        public static Vector3 OutCirc(Vector3 start, Vector3 end, float t)
        {
            --t;
            return Mathf.Sqrt(1 - t * t) * (end - start) + start;
        }

        public static Vector3 InOutCirc(Vector3 start, Vector3 end, float t)
        {
            t *= 2;
            if (t < 1)
            {
                return (-0.5f * (Mathf.Sqrt(1 - t * t) - 1)) * (end - start) + start;
            }
            else
            {
                t -= 2;
                return (0.5f * (Mathf.Sqrt(1 - t * t) + 1)) * (end - start) + start;
            }
        }

        public static Vector3 OutInCirc(Vector3 start, Vector3 end, float t)
        {
            t *= 2;
            if (t < 1)
            {
                --t;
                return (0.5f * Mathf.Sqrt(1 - t * t)) * (end - start) + start;
            }
            else
            {
                --t;
                return (-0.5f * Mathf.Sqrt(1 - t * t) + 1) * (end - start) + start;
            }
        }

        public static Vector3 InElastic(Vector3 start, Vector3 end, float t)
        {
            float p = 0.3f, s = 0.075f;
            --t;
            return (-Mathf.Pow(2, 10 * t) * Mathf.Sin((t - s) * (2 * Mathf.PI) / p)) * (end - start) + start;
        }

        public static Vector3 OutElastic(Vector3 start, Vector3 end, float t)
        {
            float p = 0.3f, s = 0.075f;
            return (Mathf.Pow(2, -10 * t) * Mathf.Sin((t - s) * (2 * Mathf.PI) / p)) * (end - start) + end;
        }

        public static Vector3 InOutElastic(Vector3 start, Vector3 end, float t)
        {
            float p = 0.3f, s = 0.075f;
            t *= 2;
            if (t < 1)
            {
                --t;
                return (-0.5f * Mathf.Pow(2, 10 * t) * Mathf.Sin((t - s) * (2 * Mathf.PI) / p)) * (end - start) + start;
            }
            else
            {
                --t;
                return (0.5f * Mathf.Pow(2, -10 * t) * Mathf.Sin((t - s) * (2 * Mathf.PI) / p)) * (end - start) + end;
            }
        }

        public static Vector3 OutInElastic(Vector3 start, Vector3 end, float t)
        {
            float p = 0.3f, s = 0.075f;
            t *= 2;
            if (t < 1)
            {
                return (0.5f * Mathf.Pow(2, -10 * t) * Mathf.Sin((t - s) * (2 * Mathf.PI) / p) - 0.5f) * (end - start) + end;
            }
            else
            {
                t -= 2;
                return (-0.5f * Mathf.Pow(2, 10 * t) * Mathf.Sin((t - s) * (2 * Mathf.PI) / p) + 0.5f) * (end - start) + start;
            }
        }

        public static Vector3 InBounce(Vector3 start, Vector3 end, float t)
        {

            t = 1 - t;
            if (t < 1 / 2.75f)
            {
                return (-7.5625f * t * t + 1) * (end - start) + start;
            }
            else if (t < 2.0f / 2.75f)
            {
                t -= 1.5f / 2.75f;
                return (-7.5625f * t * t - 0.75f + 1) * (end - start) + start;
            }
            else if (t < 2.5f / 2.75f)
            {
                t -= 2.25f / 2.75f;
                return (-7.5625f * t * t - 0.9375f + 1) * (end - start) + start;
            }
            else
            {
                t -= 2.625f / 2.75f;
                return (-7.5625f * t * t - 0.984375f + 1) * (end - start) + start;
            }
        }

        public static Vector3 OutBounce(Vector3 start, Vector3 end, float t)
        {

            if (t < 1 / 2.75f)
            {
                return (7.5625f * t * t) * (end - start) + start;
            }
            else if (t < 2.0f / 2.75f)
            {
                t -= 1.5f / 2.75f;
                return (7.5625f * t * t + 0.75f) * (end - start) + start;
            }
            else if (t < 2.5f / 2.75f)
            {
                t -= 2.25f / 2.75f;
                return (7.5625f * t * t + 0.9375f) * (end - start) + start;
            }
            else
            {
                t -= 2.625f / 2.75f;
                return (7.5625f * t * t + 0.984375f) * (end - start) + start;
            }
        }

        public static Vector3 InOutBounce(Vector3 start, Vector3 end, float t)
        {

            t *= 2;
            if (t < 1)
            {
                t = 1 - t;
                if (t < 1 / 2.75f)
                {
                    return (-0.5f * 7.5625f * t * t + 0.5f) * (end - start) + start;
                }
                else if (t < 2.0f / 2.75f)
                {
                    t -= 1.5f / 2.75f;
                    return (-0.5f * (7.5625f * t * t + 0.75f) + 0.5f) * (end - start) + start;
                }
                else if (t < 2.5f / 2.75f)
                {
                    t -= 2.25f / 2.75f;
                    return (-0.5f * (7.5625f * t * t + 0.9375f) + 0.5f) * (end - start) + start;
                }
                else
                {
                    t -= 2.625f / 2.75f;
                    return (-0.5f * (7.5625f * t * t + 0.984375f) + 0.5f) * (end - start) + start;
                }
            }
            else
            {
                --t;
                if (t < 1 / 2.75f)
                {
                    return (0.5f * 7.5625f * t * t + 0.5f) * (end - start) + start;
                }
                else if (t < 2.0f / 2.75f)
                {
                    t -= 1.5f / 2.75f;
                    return (0.5f * (7.5625f * t * t + 0.75f) + 0.5f) * (end - start) + start;
                }
                else if (t < 2.5f / 2.75f)
                {
                    t -= 2.25f / 2.75f;
                    return (0.5f * (7.5625f * t * t + 0.9375f) + 0.5f) * (end - start) + start;
                }
                else
                {
                    t -= 2.625f / 2.75f;
                    return (0.5f * (7.5625f * t * t + 0.984375f) + 0.5f) * (end - start) + start;
                }
            }
        }

        public static Vector3 OutInBounce(Vector3 start, Vector3 end, float t)
        {

            t *= 2;
            if (t < 1)
            {
                if (t < 1 / 2.75f)
                {
                    return (0.5f * 7.5625f * t * t) * (end - start) + start;
                }
                else if (t < 2.0f / 2.75f)
                {
                    t -= 1.5f / 2.75f;
                    return (0.5f * (7.5625f * t * t + 0.75f)) * (end - start) + start;
                }
                else if (t < 2.5f / 2.75f)
                {
                    t -= 2.25f / 2.75f;
                    return (0.5f * (7.5625f * t * t + 0.9375f)) * (end - start) + start;
                }
                else
                {
                    t -= 2.625f / 2.75f;
                    return (0.5f * (7.5625f * t * t + 0.984375f)) * (end - start) + start;
                }
            }
            else
            {
                t = 2 - t;
                if (t < 1 / 2.75f)
                {
                    return (-0.5f * 7.5625f * t * t + 1) * (end - start) + start;
                }
                else if (t < 2.0f / 2.75f)
                {
                    t -= 1.5f / 2.75f;
                    return (-0.5f * (7.5625f * t * t + 0.75f) + 1) * (end - start) + start;
                }
                else if (t < 2.5f / 2.75f)
                {
                    t -= 2.25f / 2.75f;
                    return (-0.5f * (7.5625f * t * t + 0.9375f) + 1) * (end - start) + start;
                }
                else
                {
                    t -= 2.625f / 2.75f;
                    return (-0.5f * (7.5625f * t * t + 0.984375f) + 1) * (end - start) + start;
                }
            }
        }

        public static Vector3 InBack(Vector3 start, Vector3 end, float t)
        {
            return t * t * (2.70158f * t - 1.70158f) * (end - start) + start;
        }

        public static Vector3 OutBack(Vector3 start, Vector3 end, float t)
        {
            --t;
            return (1 - t * t * (-2.70158f * t - 1.70158f)) * (end - start) + start;
        }

        public static Vector3 InOutBack(Vector3 start, Vector3 end, float t)
        {
            t *= 2;
            if (t < 1)
            {
                return (0.5f * t * t * (2.70158f * t - 1.70158f)) * (end - start) + start;
            }
            else
            {
                t -= 2;
                return (1 - 0.5f * t * t * (-2.70158f * t - 1.70158f)) * (end - start) + start;
            }
        }

        public static Vector3 OutInBack(Vector3 start, Vector3 end, float t)
        {
            t *= 2;
            if (t < 1)
            {
                --t;
                return (0.5f - 0.5f * t * t * (-2.70158f * t - 1.70158f)) * (end - start) + start;
            }
            else
            {
                --t;
                return (0.5f * t * t * (2.70158f * t - 1.70158f) + 0.5f) * (end - start) + start;
            }
        }
    }

    public class XSplineUtil
    {
        static public int ClampIndex(int idx, int len)
        {
            if (idx < 0)
            {
                idx = 0;
            }
            else if (idx > len - 1)
            {
                idx = len - 1;
            }
            return idx;
        }

        static public int WrapIndex(int idx, int len)
        {
            if (idx < 0)
            {
                idx = len + idx % len;
            }
            else if (idx >= len - 1)
            {
                idx = idx % len;
            }
            return idx;
        }

        static public float WrapPosition(XSpline.WrapMode wrapmode, float pos, float len)
        {
            switch (wrapmode)
            {
                case XSpline.WrapMode.Loop:
                    if (pos < 0)
                    {
                        int tms = (int)(-pos / len) + 1;
                        pos += tms * len;
                    }
                    else if (pos >= len)
                    {
                        int tms = (int)(pos / len);
                        pos -= tms * len;
                    }
                    break;

                case XSpline.WrapMode.PingPong:
                    if (pos < 0)
                    {
                        int tms = (int)(-pos / len) + 1;
                        if (tms % 2 == 0)
                        {
                            pos = pos + tms * len;
                        }
                        else
                        {
                            pos = len - (pos + tms * len);
                        }
                    }
                    else if (pos >= len)
                    {
                        int tms = (int)(pos / len);
                        if (tms % 2 == 0)
                        {
                            pos = pos - tms * len;
                        }
                        else
                        {
                            pos = len - (pos - tms * len);
                        }
                    }
                    break;

                case XSpline.WrapMode.Repeat:
                    if (pos < 0)
                    {
                        int tms = (int)(-pos / len) + 1;
                        pos += tms * len;
                    }
                    else if (pos >= len)
                    {
                        int tms = (int)(pos / len);
                        pos -= tms * len;
                    }
                    break;

                case XSpline.WrapMode.Once:
                    if (pos < 0)
                    {
                        pos = 0;
                    }
                    else if (pos > len)
                    {
                        pos = len;
                    }
                    break;
            }
            return pos;
        }
    }
}
