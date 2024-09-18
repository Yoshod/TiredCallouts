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
    [CalloutInfo("ArmedDrunkBasic", CalloutProbability.High)]
    public class ArmedDrunkBasic : Callout
    {
        private Ped Suspect;

        private Blip SuspectBlip;

        private LHandle Pursuit;

        private Vector3 SpawnPoint;

        private bool OutcomeDecided = false;

        private bool StillBlipped = true;

        private int counter;

        private bool SetDecided;

        private string set;

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
            Suspect.Metadata.hasGunPermit = true;
            Suspect.Metadata.gunLicense = "Handguns";
            Suspect.Metadata.gunPermit = "Concealed";

            counter = 0;

            Random random = new Random();
            int DialogueSet = random.Next(1, 5);

            string set = "Set" + DialogueSet;

            Suspect.Metadata.stpAlcoholDetected = true;
            Suspect.Tasks.PlayAnimation("amb@world_human_bum_standing@drunk@idle_a", "idle_a", 1f, AnimationFlags.Loop);


            SuspectBlip = Suspect.AttachBlip();
            SuspectBlip.Color = Color.Yellow;

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (StillBlipped && Game.LocalPlayer.Character.DistanceTo(Suspect) < 30f)
            {
                OutcomeDecided = false;
                StillBlipped = true;
                if (SuspectBlip.Exists()) SuspectBlip.Delete();
                StillBlipped = false;
            }

            if (!OutcomeDecided && Game.LocalPlayer.Character.DistanceTo(Suspect) < 15f)
            {


                if (Game.IsKeyDown(System.Windows.Forms.Keys.Y))
                {
                    counter++;

                    if (!SetDecided)
                    {
                        Random random = new Random();
                        int DialogueSet = random.Next(1, 5);

                        if (DialogueSet == 1)
                        {
                            set = "Set1";
                        }
                        else if (DialogueSet == 2)
                        {
                            set = "Set2";
                        }
                        else if (DialogueSet == 3)
                        {
                            set = "Set3";
                        }
                        else if (DialogueSet == 4)
                        {
                            set = "Set4";
                        }
                        else
                        {
                            set = "Set1";
                        }

                        SetDecided = true;
                    }

                    switch (counter)
                    {
                        case 1:
                            

                            Game.DisplaySubtitle(DialogueLoader.Instance.GetDialogue("ArmedDrunkBasic", set, counter));
                            break;

                        case 2:
                            Game.DisplaySubtitle(DialogueLoader.Instance.GetDialogue("ArmedDrunkBasic", set, counter));
                            break;

                        case 3:
                            Game.DisplaySubtitle(DialogueLoader.Instance.GetDialogue("ArmedDrunkBasic", set, counter));
                            break;

                        case 4:
                            Game.DisplaySubtitle(DialogueLoader.Instance.GetDialogue("ArmedDrunkBasic", set, counter));
                            break;

                        case 5:
                            Game.DisplaySubtitle(DialogueLoader.Instance.GetDialogue("ArmedDrunkBasic", set, counter));
                            break;

                        case 6:
                            Game.DisplayHelp("DEBUG: Deciding random outcome");
                            Random random = new Random();
                            int outcome = random.Next(1, 4);
                            if (outcome == 1)
                            {
                                Game.DisplaySubtitle(DialogueLoader.Instance.GetDialogue("ArmedDrunkBasic", set, 7));
                                Suspect.Tasks.FightAgainst(Game.LocalPlayer.Character);
                            }
                            else if (outcome == 2)
                            {
                                Game.DisplaySubtitle(DialogueLoader.Instance.GetDialogue("ArmedDrunkBasic", set, 8));
                                Pursuit = LSPD_First_Response.Mod.API.Functions.CreatePursuit();
                                LSPD_First_Response.Mod.API.Functions.AddPedToPursuit(Pursuit, Suspect);
                                LSPD_First_Response.Mod.API.Functions.SetPursuitIsActiveForPlayer(Pursuit, true);
                            }

                            else
                            {
                                Game.DisplaySubtitle(DialogueLoader.Instance.GetDialogue("ArmedDrunkBasic", set, 9));
                            }
                            OutcomeDecided = true;
                            break;

                    }
                }
                
            }

            if (Suspect.IsDead || !Suspect.Exists() || Suspect.IsCuffed || Game.LocalPlayer.IsDead)
            {
                Game.LogTrivial("ArmedDrunk: Suspect is dead, not existing, cuffed or player is dead. Ending callout.");
                End();
            }



        }

        public override void End()
        {
            base.End();

            Game.LogTrivial("ArmedDrunk: End() called");

            if (Suspect.Exists()) Suspect.Dismiss();
            if (SuspectBlip.Exists()) SuspectBlip.Delete();
        }

    }
}