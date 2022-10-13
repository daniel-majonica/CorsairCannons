using System;
using UnityEngine;

namespace EmptySkull.Utilities
{
    [Serializable]
    public class NullableByte : BaseNullable<byte>
    {
        public NullableByte()
        {
        }

        public NullableByte(byte startValue) : base(startValue)
        {
        }
    }

    [Serializable]
    public class NullableSByte : BaseNullable<sbyte>
    {
        public NullableSByte()
        {
        }

        public NullableSByte(sbyte startValue) : base(startValue)
        {
        }
    }

    [Serializable]
    public class NullableShort : BaseNullable<short>
    {
        public NullableShort()
        {
        }

        public NullableShort(short startValue) : base(startValue)
        {
        }
    }

    [Serializable]
    public class NullableUShort : BaseNullable<ushort>
    {
        public NullableUShort()
        {
        }

        public NullableUShort(ushort startValue) : base(startValue)
        {
        }
    }

    [Serializable]
    public class NullableInt : BaseNullable<int>
    {
        public NullableInt()
        {
        }

        public NullableInt(int startValue) : base(startValue)
        {
        }
    }

    [Serializable]
    public class NullableUInt : BaseNullable<uint>
    {
        public NullableUInt()
        {
        }

        public NullableUInt(uint startValue) : base(startValue)
        {
        }
    }

    [Serializable]
    public class NullableLong : BaseNullable<long>
    {
        public NullableLong()
        {
        }

        public NullableLong(long startValue) : base(startValue)
        {
        }
    }

    [Serializable]
    public class NullableULong : BaseNullable<ulong>
    {
        public NullableULong()
        {
        }

        public NullableULong(ulong startValue) : base(startValue)
        {
        }
    }

    [Serializable]
    public class NullableFloat : BaseNullable<float>
    {
        public NullableFloat()
        {
        }

        public NullableFloat(float startValue) : base(startValue)
        {
        }
    }

    [Serializable]
    public class NullableDouble : BaseNullable<double>
    {
        public NullableDouble()
        {
        }

        public NullableDouble(double startValue) : base(startValue)
        {
        }
    }

    [Serializable]
    public class NullableDecimal : BaseNullable<decimal>
    {
        public NullableDecimal()
        {
        }

        public NullableDecimal(decimal startValue) : base(startValue)
        {
        }
    }

    [Serializable]
    public class NullableChar : BaseNullable<char>
    {
        public NullableChar()
        {
        }

        public NullableChar(char startValue) : base(startValue)
        {
        }
    }

    [Serializable]
    public class NullableString : BaseNullable<string>
    {
        public NullableString()
        {
        }

        public NullableString(string startValue) : base(startValue)
        {
        }
    }

    [Serializable]
    public class NullableBool : BaseNullable<bool>
    {
        public NullableBool()
        {
        }

        public NullableBool(bool startValue) : base(startValue)
        {
        }
    }

    [Serializable]
    public class NullableGameObject : BaseNullable<GameObject>
    {
        public NullableGameObject()
        {
        }

        public NullableGameObject(GameObject startValue) : base(startValue)
        {
        }

        protected override bool CheckValueForNull(GameObject v)
        {
            return v == null;
        }
    }

    [Serializable]
    public class NullableVector2 : BaseNullable<Vector2>
    {
        public NullableVector2()
        {
        }

        public NullableVector2(Vector2 startValue) : base(startValue)
        {
        }
    }

    [Serializable]
    public class NullableVector2Int : BaseNullable<Vector2Int>
    {
        public NullableVector2Int()
        {
        }

        public NullableVector2Int(Vector2Int startValue) : base(startValue)
        {
        }
    }

    [Serializable]
    public class NullableVector3 : BaseNullable<Vector3>
    {
        public NullableVector3()
        {
        }

        public NullableVector3(Vector3 startValue) : base(startValue)
        {
        }
    }

    [Serializable]
    public class NullableVector3Int : BaseNullable<Vector3Int>
    {
        public NullableVector3Int()
        {
        }

        public NullableVector3Int(Vector3Int startValue) : base(startValue)
        {
        }
    }

    [Serializable]
    public class NullableVector4 : BaseNullable<Vector4>
    {
        public NullableVector4()
        {
        }

        public NullableVector4(Vector4 startValue) : base(startValue)
        {
        }
    }

    [Serializable]
    public class NullableColor : BaseNullable<Color>
    {
        public NullableColor()
        {
        }

        public NullableColor(Color startValue) : base(startValue)
        {
        }
    }

    [Serializable]
    public class NullableGradient : BaseNullable<Gradient>
    {
        public NullableGradient()
        {
        }

        public NullableGradient(Gradient startValue) : base(startValue)
        {
        }
    }

    [Serializable]
    public class NullableBounds : BaseNullable<Bounds>
    {
        public NullableBounds()
        {
        }

        public NullableBounds(Bounds startValue) : base(startValue)
        {
        }
    }

    [Serializable]
    public class NullableBoundsInt : BaseNullable<BoundsInt>
    {
        public NullableBoundsInt()
        {
        }

        public NullableBoundsInt(BoundsInt startValue) : base(startValue)
        {
        }
    }

    [Serializable]
    public class NullableAnimationCurve : BaseNullable<AnimationCurve>
    {
        public NullableAnimationCurve()
        {
        }

        public NullableAnimationCurve(AnimationCurve startValue) : base(startValue)
        {
        }
    }

    [Serializable]
    public class NullableRect : BaseNullable<Rect>
    {
        public NullableRect()
        {
        }

        public NullableRect(Rect startValue) : base(startValue)
        {
        }
    }
}