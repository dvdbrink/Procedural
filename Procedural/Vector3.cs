namespace Procedural
{
    public struct Vector3F
    {
        public float x;
        public float y;
        public float z;

        public Vector3F(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static Vector3F operator *(Vector3F v1, Vector3F v2)
        {
            return new Vector3F(v1.x*v2.x, v1.y*v2.y, v1.z*v2.z);
        }

        public static Vector3F operator *(Vector3F v, float x)
        {
            return new Vector3F(v.x*x, v.y*x, v.z*x);
        }

        public static Vector3F operator /(Vector3F v1, Vector3F v2)
        {
            return new Vector3F(v1.x/v2.x, v1.y/v2.y, v1.z/v2.z);
        }

        public static Vector3F operator /(Vector3F v, float x)
        {
            return new Vector3F(v.x/x, v.y/x, v.z/x);
        }

        public static Vector3F operator +(Vector3F v1, Vector3F v2)
        {
            return new Vector3F(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        }

        public static Vector3F operator +(Vector3F v, float x)
        {
            return new Vector3F(v.x + x, v.y + x, v.z + x);
        }

        public static Vector3F operator -(Vector3F v1, Vector3F v2)
        {
            return new Vector3F(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
        }

        public static Vector3F operator -(Vector3F v, float x)
        {
            return new Vector3F(v.x - x, v.y - x, v.z - x);
        }

        public static Vector3F Lerp(Vector3F v1, Vector3F v2, float x)
        {
            return v1 + (v2 - v1)*x;
        }
    }
}