using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;

namespace TiredCallouts.Callouts
{
    [CalloutInfo("OfficerWelfareCheck", CalloutProbability.High)]
    public class OfficerWelfareCheck : Callout
    {
        private Ped Officer;
        private Ped Suspect;
        private Vehicle OfficerVehicle;
        private Blip OfficerVehicleBlip;
        private Blip OfficerSearchBlip;
        private Vector3 SpawnPoint;
        private float Heading;
        private bool OutcomeDecided = false;
        private string Outcome;

        private List<Vector3> PossibleSpawnPoints;
        private List<float> PossibleHeadings;
        


        public override bool OnBeforeCalloutDisplayed()
        {

            PossibleSpawnPoints = new List<Vector3>()
            {
                new Vector3(419.7934f, -2093.661f, 20.80133f),
                new Vector3(-738.4366f, 370.884f, 87.86984f),
                new Vector3(659.4977f, -2792.885f, 6.086208f),
                new Vector3(1123.387f, -1612.165f, 34.69255f),
                new Vector3(329.3634f, -1284.784f, 31.78022f),
                new Vector3(51.83815f, -1047.979f, 29.59071f),
                new Vector3(151.1803f, -1080.499f, 29.21224f),
                new Vector3(-766.3402f, -1069.706f, 11.88582f),
                new Vector3(-1195.95f, -1086.676f, 6.215189f),
                new Vector3(-965.5153f, -1481.298f, 5.017407f),
                new Vector3(-209.883f, -1958.572f, 27.62042f),
                new Vector3(145.2174f, -2495.475f, 5.999994f),
                new Vector3(-257.0036f, 71.73859f, 65.52505f),
                new Vector3(-1468.084f, -651.3602f, 29.50239f),
                new Vector3(643.7522f, 615.5172f, 128.9109f),
                new Vector3(869.5339f, -36.04762f, 78.76403f),
                new Vector3(1012.252f, -767.0552f, 57.88043f),
                new Vector3(-168.6624f, -1436.438f, 31.25519f),
                new Vector3(-336.7199f, -1494.349f, 30.61351f),
            };

            PossibleHeadings = new List<float>()
            {
                231.6948f,
                268.5478f,
                291.0345f,
                88.61216f,
                147.3937f,
                245.7431f,
                169.7163f,
                34.74457f,
                198.0033f,
                102.896f,
                103.5972f,
                52.53525f,
                262.406f,
                35.82668f,
                70.94056f,
                233.4939f,
                41.50892f,
                230.0091f,
                186.3043f,
            };

            (SpawnPoint, Heading) = LocationPicker.GetSpawnLocation(PossibleSpawnPoints, PossibleHeadings, Game.LocalPlayer.Character.Position, 0);

            ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 30f);
            AddMinimumDistanceCheck(30f, SpawnPoint);
            AddMaximumDistanceCheck(1000f, SpawnPoint);
            CalloutMessage = "Officer Welfare Check";
            CalloutPosition = SpawnPoint;

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            OfficerVehicle = new Vehicle("POLICE2", SpawnPoint, Heading);
            OfficerVehicle.IsPersistent = true;

            Officer = new Ped("s_m_y_cop_01", OfficerVehicle.GetOffsetPositionFront(5f), Heading);
            Officer.IsPersistent = true;
            Officer.BlockPermanentEvents = true;
            Officer.WarpIntoVehicle(OfficerVehicle, -1);

            OfficerVehicleBlip = OfficerVehicle.AttachBlip();
            OfficerVehicleBlip.Color = System.Drawing.Color.Blue;
            OfficerVehicleBlip.IsRouteEnabled = true;

            Game.DisplayNotification("~b~Dispatch: ~w~Officer has not been responding to radio calls. Please investigate.");

            Random random = new Random();
            int outcome = random.Next(1, 3);

            OutcomeDecided = true;

            return base.OnCalloutAccepted();

        }

        public override void Process()
        {
            base.Process();

            if (Game.LocalPlayer.Character.DistanceTo(OfficerVehicle) < 10f)
            {
                Game.DisplayHelp("Press ~y~Y ~w~to check on the officer.");
                if (Game.IsKeyDown(System.Windows.Forms.Keys.Y))
                {
                    Game.DisplaySubtitle("Work In Progress");
                    End();
                }
            }

        }

        public override void End()
        {
            base.End();

            if (Officer.Exists()) Officer.Dismiss();
            if (OfficerVehicle.Exists()) OfficerVehicle.Dismiss();
            if (OfficerVehicleBlip.Exists()) OfficerVehicleBlip.Delete();
            if (OfficerSearchBlip.Exists()) OfficerSearchBlip.Delete();
        }

    }
}
