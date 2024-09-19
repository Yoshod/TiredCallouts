using Rage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Code inspired by UnitedCallouts by sEbi3
// Source: https://github.com/sEbi3/UnitedCallouts/blob/master/Stuff/LocationChooser.cs


namespace TiredCallouts
{
    public static class LocationPicker
    {

        private static int MaxIndex;
        private static Vector3 SpawnPoint;
        private static float Heading;
        private static Random random = new Random();

        internal static (Vector3, float) GetSpawnLocation(List<Vector3> locations, List<float> headings, Vector3 PlayerLocation, int Attempts)
        {
            MaxIndex = locations.Count - 1;

            int index = random.Next(0, MaxIndex);

            SpawnPoint = locations[index];
            Heading = headings[index];

            if (Vector3.Distance(PlayerLocation, SpawnPoint) > 1000f && Attempts < 5)
            {
                Attempts++;
                return GetSpawnLocation(locations, headings, PlayerLocation, Attempts);
            }

            return (SpawnPoint, Heading);
        }
    }
}
