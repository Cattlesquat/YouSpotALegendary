using System;
using Qud.API;
using Qud.UI;
using XRL.UI;

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
                    
                    Qud.API.IBaseJournalEntry.DisplayMessage("You spot " + ParentObject.DisplayName + ".");
                    ParentObject.Indicate(ParentObject.IsHostileTowards(The.Player));
                    
                    return result;
                }
            }

            return base.Render(E);
        }
    }
}