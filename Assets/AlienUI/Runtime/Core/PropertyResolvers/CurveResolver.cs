using System.Linq;
using UnityEngine;

namespace AlienUI.PropertyResolvers
{
    public class CurveResolver : PropertyResolver<AnimationCurve>
    {
        protected override AnimationCurve OnResolve(string originStr)
        {
            var temp = JsonUtility.FromJson<SerializableCurve>(originStr);
            return temp.ToCurve();
        }

        protected override AnimationCurve OnLerp(AnimationCurve from, AnimationCurve to, float progress)
        {
            if (progress >= 1) return to;
            else return from;
        }

        protected override string Reverse(AnimationCurve value)
        {
            var json = JsonUtility.ToJson(new SerializableCurve(value));
            return json;
        }

        [System.Serializable]
        private class SerializableCurve
        {
            public SerializableKeyframe[] keyframes;

            public SerializableCurve(AnimationCurve curve)
            {
                keyframes = curve.keys.Select(k => new SerializableKeyframe(k)).ToArray();
            }

            public AnimationCurve ToCurve()
            {
                if (keyframes == null)
                    return null;
                return new AnimationCurve(keyframes.Select(k => k.ToKeyframe()).ToArray());
            }
        }

        [System.Serializable]
        private class SerializableKeyframe
        {
            public float Time;

            public float Value;

            public float InTangent;

            public float OutTangent;

            public int TangentMode;

            public int WeightedMode;

            public float InWeight;

            public float OutWeight;

            public SerializableKeyframe(Keyframe key)
            {
                Time = key.time;
                Value = key.value;
                InTangent = key.inTangent;
                OutTangent = key.outTangent;
#pragma warning disable CS0618 // 类型或成员已过时
                TangentMode = key.tangentMode;
#pragma warning restore CS0618 // 类型或成员已过时
                WeightedMode = (int)key.weightedMode;
                InWeight = key.inWeight;
                OutWeight = key.outWeight;
            }

            public Keyframe ToKeyframe()
            {
#pragma warning disable CS0618 // 类型或成员已过时
                return new Keyframe(Time, Value, InTangent, OutTangent, InWeight, OutWeight)
                {
                    tangentMode = TangentMode,
                    weightedMode = (UnityEngine.WeightedMode)WeightedMode
                };
#pragma warning restore CS0618 // 类型或成员已过时
            }
        }
    }
}