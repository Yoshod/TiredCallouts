using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using System.Drawing;
using StopThePed.API;

namespace TiredCallouts.Callouts
{
    [CalloutInfo("ArmedDrunk", CalloutProbability.High)]
    public class ArmedDrunk : Callout
    {
        private Ped Suspect;

        private Blip SuspectBlip;

        private LHandle Pursuit;

        private Vector3 SpawnPoint;

        private bool PursuitCreated;

        private bool OutcomeDecided;

        public override bool OnBeforeCalloutDisplayed()
        {
            SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(1000f));
            ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 30f);
            AddMinimumDistanceCheck(30f, SpawnPoint);
            CalloutMessage = "Armed Drunk";
            CalloutPosition = SpawnPoint;
            LSPD_First_Response.Mod.API.Functions.PlayScannerAudioUsingPosition("WE_HAVE CRIME_RESIST_ARREST_02 IN_OR_ON_POSITION", SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Suspect = new Ped(SpawnPoint);
            Suspect.IsPersistent = true;
            Suspect.BlockPermanentEvents = true;
            Suspect.Inventory.GiveNewWeapon("WEAPON_PISTOL", 12, true);

            AnimationSet animSet = new AnimationSet("move_m@drunk@verydrunk");
            animSet.LoadAndWait();

            Suspect.Metadata.stpAlcoholDetected = true;
            Suspect.Tasks.PlayAnimation(new AnimationDictionary("amb@world_human_bum_standing@drunk@idle_a"), "idle_a", 1f, AnimationFlags.RagdollOnCollision);


            SuspectBlip = Suspect.AttachBlip();
            SuspectBlip.Color = Color.Yellow;

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (!PursuitCreated && Game.LocalPlayer.Character.DistanceTo(Suspect) < 200f)
            {
                Pursuit = LSPD_First_Response.Mod.API.Functions.CreatePursuit();
                LSPD_First_Response.Mod.API.Functions.AddPedToPursuit(Pursuit, Suspect);
                LSPD_First_Response.Mod.API.Functions.SetPursuitInvestigativeMode(Pursuit, true);
                PursuitCreated = true;
                OutcomeDecided = false;

                SuspectBlip.Delete();
            }

            if (PursuitCreated && !LSPD_First_Response.Mod.API.Functions.IsPursuitStillRunning(Pursuit))
            {
                End();
            }

            if (PursuitCreated && !OutcomeDecided && Game.LocalPlayer.Character.DistanceTo(Suspect) < 15f)
            {
                Suspect.Tasks.Clear();

                Random random = new Random();
                int outcome = random.Next(1, 4);
                if (outcome == 1)
                {
                    LSPD_First_Response.Mod.API.Functions.SetPursuitIsActiveForPlayer(Pursuit, true);
                    Suspect.Tasks.FightAgainst(Game.LocalPlayer.Character);
                    Game.DisplayHelp("DEBUG: The suspect is attacking you.");
                }
                else if (outcome == 2)
                {
                    LSPD_First_Response.Mod.API.Functions.SetPursuitIsActiveForPlayer(Pursuit, true);
                    Suspect.Tasks.Clear();
                    Game.DisplayHelp("DEBUG: The suspect is running.");
                }

                else
                {
                    Suspect.Tasks.Clear();
                    Game.DisplayHelp("DEBUG: The suspect is doing nothing.");
                }
                OutcomeDecided = true;
            }
        }

        public override void End()
        {
            base.End();

            if (Suspect.Exists()) Suspect.Dismiss();
            if (SuspectBlip.Exists()) SuspectBlip.Delete();
        }

    }
}