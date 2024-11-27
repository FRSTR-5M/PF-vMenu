using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using static CitizenFX.Core.Native.API;

namespace Freecam2
{
    public class Freecam : BaseScript
    {
        static Camera FCamera;
        static bool Attached = false;
        static Entity AttachedEntity;
        static bool HUD = true;

        static Vector3 OffsetCoords = Vector3.Zero;

        static float OffsetRotX = 0.0f;
        static float OffsetRotY = 0.0f;
        static float OffsetRotZ = 0.0f;
        static float Speed = 2.0f;
        static float currspeed = 2.0f;
        static float SpeedMulti = 1;

        static int FilterIndex = 0; // 0 == None

        public Freecam()
        {
            Tick += OnTick;
        }

        private async Task OnTick()
        {
            if (FCamera == null || !FCamera.Equals(World.RenderingCamera) || Game.IsPaused)
                return;

            DisableFirstPersonCamThisFrame();
            BlockWeaponWheelThisFrame();
            foreach (int control in Enum.GetValues(typeof(Control)))
                if ((Control)control != Control.MpTextChatAll) DisableControlAction(0, control, true);



            if (HUD)
            {
                // Initialization/Setup
                Scaleform scaleform = new Scaleform("instructional_buttons");
                scaleform.CallFunction("CLEAR_ALL", new object[0]);
                scaleform.CallFunction("TOGGLE_MOUSE_BUTTONS", 0);

                // Movement/Rotation
                scaleform.CallFunction("SET_DATA_SLOT", 0, Function.Call<string>(Hash.GET_CONTROL_INSTRUCTIONAL_BUTTON, 2, Control.ParachutePitchUpDown, 0), "Forward/Back");
                scaleform.CallFunction("SET_DATA_SLOT", 1, Function.Call<string>(Hash.GET_CONTROL_INSTRUCTIONAL_BUTTON, 2, Control.ParachuteTurnLeftRight, 0), "Left/Right");
                scaleform.CallFunction("SET_DATA_SLOT", 2, Function.Call<string>(Hash.GET_CONTROL_INSTRUCTIONAL_BUTTON, 2, Control.LookLeftRight, 0), "Look");
                scaleform.CallFunction("SET_DATA_SLOT", 3, Function.Call<string>(Hash.GET_CONTROL_INSTRUCTIONAL_BUTTON, 2, Control.Pickup, 0), "");
                scaleform.CallFunction("SET_DATA_SLOT", 4, Function.Call<string>(Hash.GET_CONTROL_INSTRUCTIONAL_BUTTON, 2, Control.Cover, 0), "Roll");
                scaleform.CallFunction("SET_DATA_SLOT", 5, Function.Call<string>(Hash.GET_CONTROL_INSTRUCTIONAL_BUTTON, 2, Control.Duck, 0), "");
                scaleform.CallFunction("SET_DATA_SLOT", 6, Function.Call<string>(Hash.GET_CONTROL_INSTRUCTIONAL_BUTTON, 2, Control.Jump, 0), "Up/Down");

                // Misc
                scaleform.CallFunction("SET_DATA_SLOT", 7, Function.Call<string>(Hash.GET_CONTROL_INSTRUCTIONAL_BUTTON, 2, Control.FrontendDown, 0), "");
                scaleform.CallFunction("SET_DATA_SLOT", 8, Function.Call<string>(Hash.GET_CONTROL_INSTRUCTIONAL_BUTTON, 2, Control.FrontendUp, 0), "Zoom");
                scaleform.CallFunction("SET_DATA_SLOT", 9, Function.Call<string>(Hash.GET_CONTROL_INSTRUCTIONAL_BUTTON, 2, Control.FrontendRight, 0), "");
                scaleform.CallFunction("SET_DATA_SLOT", 10, Function.Call<string>(Hash.GET_CONTROL_INSTRUCTIONAL_BUTTON, 2, Control.FrontendLeft, 0), $"Filter: [{Config.Filters[FilterIndex]}]");
                scaleform.CallFunction("SET_DATA_SLOT", 11, Function.Call<string>(Hash.GET_CONTROL_INSTRUCTIONAL_BUTTON, 2, Control.Reload, 0), $"Reset Filter");
                if (!Attached) scaleform.CallFunction("SET_DATA_SLOT", 12, Function.Call<string>(Hash.GET_CONTROL_INSTRUCTIONAL_BUTTON, 2, Control.CursorAccept, 0), "Attach");
                else scaleform.CallFunction("SET_DATA_SLOT", 13, Function.Call<string>(Hash.GET_CONTROL_INSTRUCTIONAL_BUTTON, 2, Control.CursorCancel, 0), "Detach");
                scaleform.CallFunction("SET_DATA_SLOT", 15, Function.Call<string>(Hash.GET_CONTROL_INSTRUCTIONAL_BUTTON, 2, Control.WeaponWheelNext, 0), $"Speed: {Speed*SpeedMulti}");
                    

                // HUD Toggle
                scaleform.CallFunction("SET_DATA_SLOT", 14, Function.Call<string>(Hash.GET_CONTROL_INSTRUCTIONAL_BUTTON, 2, 236, 0), "Toggle HUD");

                // Drawing
                scaleform.CallFunction("DRAW_INSTRUCTIONAL_BUTTONS", -1);
                Function.Call(Hash.DRAW_SCALEFORM_MOVIE_FULLSCREEN, scaleform.Handle, (int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue, 0);
            }



            Vector3 CamCoord = FCamera.Position;
            Vector3 NewPos = ProcessNewPos(CamCoord);

            if (Attached && IsDisabledControlJustPressed(1, (int)Control.CursorCancel))
            {
                // Attachment cleanup
                FCamera.Detach();
                AttachedEntity = null;
                Attached = false;
            }
            else if (IsDisabledControlJustPressed(1, (int)Control.CursorAccept))
            {
                Entity AttachEnt = GetEntityInFrontOfCam(FCamera);
                if (AttachEnt != null)
                {
                    AttachedEntity = AttachEnt;
                    OffsetCoords = GetOffsetFromEntityGivenWorldCoords(AttachedEntity.Handle, FCamera.Position.X, FCamera.Position.Y, FCamera.Position.Z);
                    AttachCamToEntity(FCamera.Handle, AttachedEntity.Handle, OffsetCoords.X, OffsetCoords.Y, OffsetCoords.Z, true);
                    Attached = true;
                }
            }

            FCamera.Position = NewPos;
            FCamera.Rotation = new Vector3(OffsetRotX, OffsetRotY, OffsetRotZ);
            SetFocusArea(NewPos.X, NewPos.Y, NewPos.Z, 0.0f, 0.0f, 0.0f);

            // Misc controls
            if (IsDisabledControlJustPressed(1, (int)Control.ScriptSelect))
                HUD = !HUD;
            if (IsDisabledControlPressed(1, (int)Control.FrontendUp))
                SetCamFov(FCamera.Handle, FCamera.FieldOfView - 1);
            else if (IsDisabledControlPressed(1, (int)Control.FrontendDown))
                SetCamFov(FCamera.Handle, FCamera.FieldOfView + 1);
            if (IsDisabledControlJustPressed(1, (int)Control.FrontendLeft))
            {
                if (FilterIndex == 0) FilterIndex = Config.Filters.Count - 1;
                else FilterIndex--;
                SetTimecycleModifier(Config.Filters[FilterIndex]);
                SetTimecycleModifierStrength(Config.FilterIntensity);
            }
            else if (IsDisabledControlJustPressed(1, (int)Control.FrontendRight))
            {
                if (FilterIndex == Config.Filters.Count - 1) FilterIndex = 0;
                else FilterIndex++;
                SetTimecycleModifier(Config.Filters[FilterIndex]);
                SetTimecycleModifierStrength(Config.FilterIntensity);
            }
            else if (IsDisabledControlJustPressed(1, (int)Control.Reload))
            {
                FilterIndex = 0;
                SetTimecycleModifier("None");
            }

            await Delay(0);
        }

        public static Vector3 ProcessNewPos(Vector3 CurrentPos)
        {
            Vector3 Return = CurrentPos;

            if (IsDisabledControlPressed(1, 14))
            {
                currspeed = Speed - 0.5f;
                Speed = Speed - 0.5f;
                if (currspeed < 0.5)
                {
                    Speed = 0.5f;
                    currspeed = 0.5f;
                }
            }
            
            if (IsDisabledControlPressed(1, 15))
            {
                currspeed = Speed + 0.5f;
                Speed = Speed + 0.5f;
                if (currspeed > Config.MaxSpeed)
                {
                    Speed = Config.MaxSpeed;
                    currspeed = Config.MaxSpeed;
                }
            }
            
            if (IsDisabledControlPressed(1, 21))
                SpeedMulti = Config.ShiftSpeed;
            else
                SpeedMulti = 1;
           
            Speed = Speed * SpeedMulti;

            if (IsInputDisabled(0))
            {
                // Basic movement--- WASD
                if (IsDisabledControlPressed(1, (int)Control.ParachutePitchUpOnly)) // Forwards
                {
                    float multX = Sin(OffsetRotZ);
                    float multY = Cos(OffsetRotZ);
                    float multZ = Sin(OffsetRotX);

                    Return.X -= (float)(0.1 * Speed * multX);
                    Return.Y += (float)(0.1 * Speed * multY);
                    Return.Z += (float)(0.1 * Speed * multZ);
                }
                if (IsDisabledControlPressed(1, (int)Control.ParachutePitchDownOnly)) // Backwards
                {
                    float multX = Sin(OffsetRotZ);
                    float multY = Cos(OffsetRotZ);
                    float multZ = Sin(OffsetRotX);

                    Return.X += (float)(0.1 * Speed * multX);
                    Return.Y -= (float)(0.1 * Speed * multY);
                    Return.Z -= (float)(0.1 * Speed * multZ);
                }
                if (IsDisabledControlPressed(1, (int)Control.MoveLeftOnly)) // Left
                {
                    float multX = Sin(OffsetRotZ + 90.0f);
                    float multY = Cos(OffsetRotZ + 90.0f);
                    float multZ = Sin(OffsetRotX);

                    Return.X -= (float)(0.1 * Speed * multX);
                    Return.Y += (float)(0.1 * Speed * multY);
                    //Return.Z += (float)(0.1 * Speed * multZ);
                }
                if (IsDisabledControlPressed(1, (int)Control.MoveRightOnly)) // Right
                {
                    float multX = Sin(OffsetRotZ + 90.0f);
                    float multY = Cos(OffsetRotZ + 90.0f);
                    float multZ = Sin(OffsetRotX);

                    Return.X += (float)(0.1 * Speed * multX);
                    Return.Y -= (float)(0.1 * Speed * multY);
                    //Return.Z -= (float)(0.1 * Speed * multZ);
                }

                // Up/Down
                if (IsDisabledControlPressed(1, (int)Control.Jump)) // Up
                    Return.Z += (float)(0.1 * Speed);
                if (IsDisabledControlPressed(1, (int)Control.Duck)) // Down
                    Return.Z -= (float)(0.1 * Speed);


                // Rotation-- Q/E
                OffsetRotX -= (GetDisabledControlNormal(1, (int)Control.LookUpDown) * Config.Precision * 8.0f);
                OffsetRotZ -= (GetDisabledControlNormal(1, (int)Control.LookLeftRight) * Config.Precision * 8.0f);
                if (IsDisabledControlPressed(1, (int)Control.Cover))
                    OffsetRotY -= Config.Precision;
                if (IsDisabledControlPressed(1, (int)Control.Pickup))
                    OffsetRotY += Config.Precision;
            }

            // Not used since the locking is just annoying af

            if (OffsetRotX > 90.0) OffsetRotX = 90.0f;
            else if (OffsetRotX < -90.0f) OffsetRotX = -90.0f;

            if (OffsetRotY > 90.0) OffsetRotY = 90.0f;
            else if (OffsetRotY < -90.0f) OffsetRotY = -90.0f;
            Speed = currspeed;
            return Return;
        }

        public static void Enable()
        {
            FCamera = World.CreateCamera(Game.PlayerPed.Position, Game.PlayerPed.Rotation, 50f);
            DisplayRadar(false);
            HUD = true;
            SetTimecycleModifierStrength(Config.FilterIntensity);
            SetTimecycleModifier(Config.Filters[FilterIndex]);
            World.RenderingCamera = FCamera;
        }

        public static void Disable()
        {
            FCamera.Delete();
            Game.PlayerPed.IsCollisionEnabled = true; // Just a quick and easy workaround for collision issues
            DisplayRadar(true);
            ClearFocus();
            SetTimecycleModifier("None");
            World.RenderingCamera = null;
        }

        public static Entity GetEntityInFrontOfCam(Camera Cam)
        {
            Vector3 CamCoords = Function.Call<Vector3>(Hash.GET_CAM_COORD, Cam);
            Vector3 Offset = new Vector3()
            {
                // Honestly I have no fucking idea what any of this does. I'm just copying it 
                X = CamCoords.X - Sin(OffsetRotZ) * 100.0f,
                Y = CamCoords.Y + Cos(OffsetRotZ) * 100.0f,
                Z = CamCoords.Z + Sin(OffsetRotX) * 100.0f
            };

            int RayHandle = StartShapeTestRay(CamCoords.X, CamCoords.Y, CamCoords.Z, Offset.X, Offset.Y, Offset.Z, 10, 0, 0);


            Vector3 EndCoord = Vector3.Zero;
            Vector3 SurfaceNormal = Vector3.Zero;
            int EntityHit = 0;
            bool Hit = false;
            int ent = GetShapeTestResult(RayHandle, ref Hit, ref EndCoord, ref SurfaceNormal, ref EntityHit);
            return Entity.FromHandle(EntityHit);
        }

        private void DisableMainMenuControls()
        {
            DisableControlAction(1, 85, true); // INPUT_VEH_RADIO_WHEEL = DPAD - LEFT
            DisableControlAction(1, 74, true); // INPUT_VEH_HEADLIGHT = DPAD - RIGHT
            DisableControlAction(1, 48, true); // INPUT_HUD_SPECIAL = DPAD - DOWN
            DisableControlAction(1, 27, true); // INPUT_PHONE = DPAD - UP
            DisableControlAction(1, 80, true); // INPUT_VEH_CIN_CAM = B
            DisableControlAction(1, 73, true); // INPUT_VEH_DUCK = A
        }

        private void DisableAdditionalMainMenuControls()
        {
            DisableControlAction(1, 75, true); // INPUT_VEH_EXIT - Y
            DisableControlAction(1, 37, true); // INPUT_SELECT_WEAPON - X
        }
    }
}
