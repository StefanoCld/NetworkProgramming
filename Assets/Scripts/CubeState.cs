using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct CubeState 
{
    short positionX;
    byte positionY;
    short positionZ;

    byte rotationX;
    byte rotationY;
    byte rotationZ;

    public bool isInteracting;
    public ushort index;

    public void CompressData(Vector3 position, Quaternion rotation, bool interacting, ushort cubeIndex)
    {
        // Assuming that the cubes will never go further than 64 on the xz-plane,
        // and 8 on the y-axis, we use 16 bit (for each axis) to describe their poisition on the xz-plane
        // and 8 bit to describe the position on the y-axis

        // We then need to round the values to the next integer since we are not representing them
        // with the floating point notation anymore

        positionX = (short)Mathf.Round(Remap(position.x, -64, 64, short.MinValue, short.MaxValue)); // -32768, 32767
        positionY = (byte)Mathf.Round(Remap(position.y, 0, 8, 0, byte.MaxValue)); // 255
        positionZ = (short)Mathf.Round(Remap(position.z, -64, 64, short.MinValue, short.MaxValue)); //-32768, 32767
        //This thing will save us 7 bytes!

        // Same goes here, but we map the rotation values to 0-255
        // Knowing that (x,y,z,w) is equal to (-x,-y,-z,-w)
        // we send the quaternion with w >= 0

        int adjustedW = (rotation.w >= 0) ? 1 : -1;
        rotationX = (byte)Mathf.Round(Remap(adjustedW * rotation.x, -1, 1, 0, 255));
        rotationY = (byte)Mathf.Round(Remap(adjustedW * rotation.y, -1, 1, 0, 255));
        rotationZ = (byte)Mathf.Round(Remap(adjustedW * rotation.z, -1, 1, 0, 255));
        //This thing will save us 9 bytes!

        isInteracting = interacting;
        index = cubeIndex;
    }

    public void DecompressData(ref Vector3 position, ref Quaternion rotation)
    {
        position.x = Remap(positionX, short.MinValue, short.MaxValue, -64, 64);
        position.y = Remap(positionY, 0, byte.MaxValue, 0, 8);
        position.z = Remap(positionZ, short.MinValue, short.MaxValue, -64, 64);

        rotation.x = Remap(rotationX, 0, byte.MaxValue, -1, 1);
        rotation.y = Remap(rotationY, 0, byte.MaxValue, -1, 1);
        rotation.z = Remap(rotationZ, 0, byte.MaxValue, -1, 1);

        // Recalculating W from the other vector's elements i have received
        rotation.w = Mathf.Sqrt(1 - (rotation.x * rotation.x + rotation.y * rotation.y + rotation.z * rotation.z));
    }

    static float Remap(float value, float originalMin, float originalMax, float newMin, float newMax)
    {
        float t = Mathf.InverseLerp(originalMin, originalMax, value);
        return Mathf.Lerp(newMin, newMax, t);
    }
}
