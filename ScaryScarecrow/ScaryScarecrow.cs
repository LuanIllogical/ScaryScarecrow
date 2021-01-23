using System;
using System.Collections.Generic;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using Microsoft.Xna.Framework;
using Terraria.Localization;
using Terraria.ID;

namespace ScaryScarecrow
{
    [ApiVersion(2, 1)]
    public class ScaryScarecrow : TerrariaPlugin
    {

        public override string Name => "ScaryScarecrow";
        public override Version Version => new Version(1, 0);
        public override string Author => "ExitiumTheCat";
        public override string Description => "";

        public static List<Player> Players = new List<Player>();

        public bool GameOngoing;
        public int BreakTime = 20;
        public int CurrentBreakTime;
        public int RavenTimer = 45;
        public int CurrentRavenTimer;
        public int StaffTimer = 10;
        public int CurrentStaffTimer;
        public int Scarecrow = 0;
        public int NoTheresNoGotYouBack;

        public ScaryScarecrow(Main game) : base(game) { }

        public override void Initialize()
        {
            Commands.ChatCommands.Add(new Command("tshock.tp.others", SetBreakTime, "setbreaktime"));
            Commands.ChatCommands.Add(new Command("tshock.tp.others", SetRavenTimer, "setraventimer"));
            Commands.ChatCommands.Add(new Command("tshock.tp.others", SetStaffTimer, "setstafftimer"));
            Commands.ChatCommands.Add(new Command("tshock.tp.others", StartOrEnd, "scaryscarecrow"));
            ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.ServerChat.Deregister(this, OnUpdate);
            }
            base.Dispose(disposing);
        }

        private void SetBreakTime(CommandArgs args)
        {
            if (args.Parameters.Count > 0)
            {
                foreach (string input in args.Parameters)
                {
                    int.TryParse(input, out BreakTime);
                    if (BreakTime == 0) BreakTime = 20;
                }
            }
            else
            {
                args.Player.SendErrorMessage("Please input a number in seconds.");
            }
        }
        private void SetRavenTimer(CommandArgs args)
        {
            if (args.Parameters.Count > 0)
            {
                foreach (string input in args.Parameters)
                {
                    int.TryParse(input, out RavenTimer);
                    if (RavenTimer == 0) RavenTimer = 20;
                }
            }
            else
            {
                args.Player.SendErrorMessage("Please input a number in seconds.");
            }
        }
        private void SetStaffTimer(CommandArgs args)
        {
            if (args.Parameters.Count > 0)
            {
                foreach (string input in args.Parameters)
                {
                    int.TryParse(input, out StaffTimer);
                    if (StaffTimer == 0) StaffTimer = 20;
                }
            }
            else
            {
                args.Player.SendErrorMessage("Please input a number in seconds.");
            }
        }

        private void StartOrEnd(CommandArgs args)
        {
            if (GameOngoing)
            {
                GameOngoing = false;
                TSPlayer.All.SendMessage("The Scarecrow Curse's has been contained!", Color.Yellow);
            }
            else
            {
                GameOngoing = true;
                TSPlayer.All.SendMessage("The Scarecrow Curse's has been unleashed!", Color.Yellow);
                Scarecrow = Main.rand.Next(0, TShock.Utils.GetActivePlayerCount());
                CurrentRavenTimer = RavenTimer * 60;
            }
        }
        private void OnUpdate(EventArgs args)
        {
            if (GameOngoing && CurrentBreakTime <= 0)
            {
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Player plr = Main.player[i];
                    if (Main.player[i].active)
                    {
                        if (i == Scarecrow)
                        {
                            plr.armor[0].netDefaults(1788);
                            plr.armor[1].netDefaults(1789);
                            plr.armor[2].netDefaults(1790);
                            plr.armor[3].netDefaults(285);
                            NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, TShock.Players[i].Index, 0 + 59);
                            NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, TShock.Players[i].Index, 1 + 59);
                            NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, TShock.Players[i].Index, 2 + 59);
                            NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, TShock.Players[i].Index, 3 + 59);
                            if (plr.HeldItem.type == ItemID.RavenStaff)
                            {
                                plr.inventory[4].netDefaults(0);
                                NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, TShock.Players[i].Index, 4);
                                CurrentStaffTimer = StaffTimer * 60;
                                TShock.Players[i].SetBuff(3, 240, false);
                                Projectile.NewProjectile(Main.player[i].position, new Vector2(0, 0), 274, 0, 0);
                            }
                            else
                            {
                                if (CurrentStaffTimer <= 0)
                                {
                                    plr.inventory[4].netDefaults(1802);
                                    NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, TShock.Players[i].Index, 4);
                                }
                                else
                                {
                                    CurrentStaffTimer--;
                                }
                            }
                        }
                        else
                        {
                            if (plr.Hitbox.Intersects(Main.player[Scarecrow].Hitbox) && NoTheresNoGotYouBack <= 0)
                            {
                                Main.player[Scarecrow].inventory[4].netDefaults(0);
                                Main.player[Scarecrow].armor[0].netDefaults(0);
                                Main.player[Scarecrow].armor[1].netDefaults(0);
                                Main.player[Scarecrow].armor[2].netDefaults(0);
                                Main.player[Scarecrow].armor[3].netDefaults(0);
                                NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, TShock.Players[Scarecrow].Index, 4);
                                NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, TShock.Players[Scarecrow].Index, 0 + 59);
                                NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, TShock.Players[Scarecrow].Index, 1 + 59);
                                NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, TShock.Players[Scarecrow].Index, 2 + 59);
                                NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, TShock.Players[Scarecrow].Index, 3 + 59);
                                TSPlayer.All.SendMessage(Main.player[Scarecrow].name + " -> " + plr.name, Color.Yellow);
                                Scarecrow = i;
                                CurrentRavenTimer += 60;
                                NoTheresNoGotYouBack = 90;
                            }
                        }
                        if (NoTheresNoGotYouBack != 0)
                            NoTheresNoGotYouBack--;
                        if (CurrentRavenTimer != 0)
                            CurrentRavenTimer--;
                        switch (CurrentRavenTimer)
                        {
                            case (600):
                                TSPlayer.All.SendMessage("10", Color.Yellow);
                                break;
                            case (540):
                                TSPlayer.All.SendMessage("9", Color.Yellow);
                                break;
                            case (480):
                                TSPlayer.All.SendMessage("8", Color.Yellow);
                                break;
                            case (420):
                                TSPlayer.All.SendMessage("7", Color.Yellow);
                                break;
                            case (360):
                                TSPlayer.All.SendMessage("6", Color.Yellow);
                                break;
                            case (300):
                                TSPlayer.All.SendMessage("5", Color.Yellow);
                                break;
                            case (240):
                                TSPlayer.All.SendMessage("4", Color.Yellow);
                                break;
                            case (180):
                                TSPlayer.All.SendMessage("3", Color.Yellow);
                                break;
                            case (120):
                                TSPlayer.All.SendMessage("2", Color.Yellow);
                                break;
                            case (60):
                                TSPlayer.All.SendMessage("1", Color.Yellow);
                                break;
                            case (1):
                                TSPlayer.All.SendMessage("Caw!", Color.Yellow);
                                TShock.Players[Scarecrow].KillPlayer();
                                CurrentBreakTime = BreakTime * 60;
                                break;
                        }
                    }
                }
            }
            else if (GameOngoing)
            {
                if (CurrentBreakTime > 0)
                {
                    CurrentBreakTime--;
                }
                if (CurrentBreakTime == 1)
                {
                    Scarecrow = Main.rand.Next(0, TShock.Utils.GetActivePlayerCount());
                }
            }
        }
    }
}