using RimWorld;
using Verse;

namespace TSA.SkillAffectsCooldown
{
    public class StatPart_ShootingCooldown : StatPart
    {
        public override void TransformValue(StatRequest req, ref float val)
        {
            var pawn = ResolvePawn(req);
            if (pawn == null || pawn.skills == null) return;

            int lvl = pawn.skills.GetSkill(SkillDefOf.Shooting).Level;
            // Our sliders are TIME multipliers (lower = faster = shorter cooldown)
            float timeMult = SkillAffectsCooldown_Settings.Current?.GetFactorForSkill(lvl) ?? 1f;

            // Cooldown is a time stat ⇒ multiply by time factor (just like warm-up)
            val *= timeMult;
        }

        public override string ExplanationPart(StatRequest req)
        {
            var pawn = ResolvePawn(req);
            if (pawn == null || pawn.skills == null) return null;

            int lvl = pawn.skills.GetSkill(SkillDefOf.Shooting).Level;
            float timeMult = SkillAffectsCooldown_Settings.Current?.GetFactorForSkill(lvl) ?? 1f;

            // Example: "Shooting skill 14: x0.78 (cooldown time)"
            return $"Shooting skill {lvl}: x{timeMult:0.00} (cooldown time)";
        }

        private Pawn ResolvePawn(StatRequest req)
        {
            if (req.Pawn != null) return req.Pawn;
            if (req.HasThing && req.Thing is Pawn p1) return p1;
            if (req.HasThing && req.Thing is Thing t) return FindPawnFromHolder(t.ParentHolder);
            return null;
        }

        private Pawn FindPawnFromHolder(IThingHolder holder)
        {
            if (holder == null) return null;
            if (holder is Pawn p) return p;
            if (holder is Pawn_EquipmentTracker eq) return eq.pawn;
            if (holder is Pawn_ApparelTracker ap) return ap.pawn;
            if (holder is Pawn_InventoryTracker inv) return inv.pawn;
            if (holder is ThingOwner to && to.Owner != null) return FindPawnFromHolder(to.Owner);
            if (holder is Thing th && th.ParentHolder != null) return FindPawnFromHolder(th.ParentHolder);
            return null;
        }
    }
}
