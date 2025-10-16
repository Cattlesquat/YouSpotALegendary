using System;
using System.Collections.Generic;
using Qud.API;
using Qud.UI;
using XRL.UI;
using XRL;
using XRL.World.Capabilities;

namespace XRL.World.Parts
{
    // I basically cut this down from the DromadCaravan part -- the very first time the parent object is rendered, display the message.

    [Serializable]
    public class Cattlesquat_YouSpotALegendary : IPart
    {
        public bool Seen = false;
        
        public override bool SameAs(IPart p)
        {
            return false;
        }

        public override bool Render(RenderEvent E)
        {
            if (!Seen)
            {
                Seen = true;
                if (ParentObject.HasPart<GivesRep>())
                {
                    bool result = base.Render(E); // Render before we indicate
                    ParentObject.Indicate(ParentObject.IsHostileTowards(The.Player));
                    
                    String spotted = "You spot " + ParentObject.DisplayName + ".";

                    String factionString = "";
                    List<string> parentFactions = new List<string>(2);
                    var givesRep = ParentObject.GetPart<GivesRep>();
                    foreach (var faction in givesRep.GetLovedFactions(VisibleOnly: true, WithoutReason: true))
                    {
                        parentFactions.Add(faction.Name);
                    }

                    bool any = false;
                    if (parentFactions.Count > 0)
                    {
                        foreach (var member in parentFactions)
                        {
                            if (any)
                            {
                                factionString += ", Loved/Member: ";
                            }
                            factionString += member;
                            any = true;
                        }
                    }
                    
                    foreach (FriendorFoe f in givesRep.relatedFactions)
                    {
                        Faction faction = Factions.GetIfExists(f.faction);
                        if (faction is { Visible: true } && !f.reason.IsNullOrEmpty())
                        {
                            if (any)
                            {
                                factionString += "; ";
                            }

                            if (f.status == "love")
                            {
                                factionString += "Loved: " + faction.Name;
                            }

                            if (f.status == "friend")
                            {
                                factionString += "Admired: " + faction.Name;
                            }
                            else if (f.status == "dislike")
                            {
                                factionString += "Disliked: " + faction.Name;
                            }
                            else if (f.status == "hate")
                            {
                                factionString += "Hated: " + faction.Name;
                            }

                            any = true;
                        }
                    }
                    
                    if (AutoAct.Setting != "")
                    {
                        XRL.Messages.MessageQueue.AddPlayerMessage(spotted + ((factionString != "") ? " (" + factionString + ")" : ""));
                        int ichoice = Popup.PickOption(Title: "", Intro: spotted,
                            Options: new string[] { "Continue", "Interrupt Autoexplore" });
                        if (ichoice == 1) AutoAct.Interrupt();
                    }
                    else
                    {
                        Qud.API.IBaseJournalEntry.DisplayMessage(spotted + ((factionString != "") ? " (" + factionString  + ")" : ""));
                    }

                    if (Options.GetOptionBool("OptionYouSpotALegendaryAddJournalEntry"))
                    {
                        JournalAPI.AddMapNote(The.Player.CurrentZone.ZoneID, factionString, "Miscellaneous", null, null,
                            true, true);
                    }

                    ParentObject.Indicate(ParentObject.IsHostileTowards(The.Player));

                    return result;
                }
            }

            return base.Render(E);
        }
    }
}