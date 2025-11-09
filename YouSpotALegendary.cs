using System;
using System.Text;
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
                    
                    String spotted = "You spot " + ParentObject.DisplayName + ".\n";

                    var givesRep = ParentObject.GetPart<GivesRep>();
                    StringBuilder SB = new();
                    givesRep.AppendReputationDescription(SB);
                    
                    var repString = SB.ToString();
                    if (AutoAct.Setting != "")
                    {
                        XRL.Messages.MessageQueue.AddPlayerMessage(spotted + repString);
                        int ichoice = Popup.PickOption(Title: "", Intro: spotted + repString + "\n",
                            Options: new string[] { "Continue", "Interrupt Autoexplore" });
                        if (ichoice == 1) AutoAct.Interrupt();
                    }
                    else
                    {
                        Qud.API.IBaseJournalEntry.DisplayMessage(spotted + repString);
                    }

                    if (Options.GetOptionBool("OptionYouSpotALegendaryAddJournalEntry"))
                    {
                        repString = repString.Replace("\n", " ");
                        repString = repString.Replace("{{C|-----}}", "");
                        
                        JournalAPI.AddMapNote(The.Player.CurrentZone.ZoneID, ParentObject.DisplayName + ": " + repString, "Miscellaneous", null, null,
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