using NUnit.Framework;
using System;
using UnityEngine;

namespace Utils
{
    public static class Tests
    {
        public static bool ColliderIsNegative(Collider collider)
        {
            if (collider is BoxCollider)
            {
                var b = (BoxCollider)collider;
                return (b.size.x < 0) || (b.size.y < 0) || (b.size.z < 0);
            }
            if (collider is SphereCollider)
            {
                return ((SphereCollider)collider).radius < 0;
            }
            return false;
        }

        public static void AssertVectorIsNotNegative(Vector3 vector, Func<string> message)
        {
            Assert.That(vector.x, Is.Not.Negative, () => $"{message()} in x");
            Assert.That(vector.y, Is.Not.Negative, () => $"{message()} in y");
            Assert.That(vector.z, Is.Not.Negative, () => $"{message()} in z");
        }
    }
}
