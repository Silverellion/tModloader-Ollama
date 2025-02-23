using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OllamaPlayer.Content.Npc.OllamaNpc;

public class OllamaNpcOrbProjectile : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.width = 13;
        Projectile.height = 13;
        Projectile.friendly = false;
        Projectile.hostile = false;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = 9999;
    }

   public override void AI()
    {
        int npcId = (int)Projectile.ai[0];
        if (npcId < 0 || npcId >= Main.npc.Length || !Main.npc[npcId].active)
        {
            Projectile.Kill();
            return;
        }
        NPC owner = Main.npc[npcId];
        OllamaNpcOrbProjectileTracker.RegisterOrb(npcId, Projectile.whoAmI);

        int index = (int)Projectile.ai[1];
        Vector2 offset = Vector2.Zero;

        if (index == 0) offset = new Vector2(-15, -50); 
        if (index == 1) offset = new Vector2(10, -50);  
        if (index == 2) offset = new Vector2(-30, -30); 
        if (index == 3) offset = new Vector2(25, -30);
        
        if (Projectile.localAI[0] == 0)
            Projectile.localAI[0] = Main.rand.NextFloat(-0.1f, 0.1f);
        
        Projectile.alpha = 50;
        Projectile.rotation += Projectile.localAI[0];
        Projectile.Center = owner.Center + offset;

        SpawnDust();
        ShootProjectile();
    }
   
    public override void OnKill(int timeLeft)
    {
        // Unregister the orb from the manager when it is destroyed
        int npcId = (int)Projectile.ai[0];
        OllamaNpcOrbProjectileTracker.UnregisterOrb(npcId, Projectile.whoAmI);
    }
    private void SpawnDust()
    {
        if (Main.rand.NextBool(10)) // (100/10)% chance per frame
        {
            Dust dust = Dust.NewDustDirect(
                Projectile.position, 
                Projectile.width, 
                Projectile.height, 
                DustID.WhiteTorch,
                0f, 0f,
                50,
                default, 
                1.5f 
            );

            dust.noGravity = true;
            dust.fadeIn = 1.2f;
            dust.velocity *= 0.0f;
            dust.noLight = false;
            
            dust.scale *= 0.5f;
        }
    }

    private void ShootProjectile()
    {
        //a random chance to shoot between 3 and 7 secs 
        if (Main.netMode != NetmodeID.Server && Projectile.ai[2]++ >= Main.rand.Next(180, 420)) 
        {
            Projectile.ai[2] = 0; 
            NPC target = null;
            float maxDist = 600f;
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && !npc.friendly && npc.CanBeChasedBy())
                {
                    float dist = Vector2.Distance(npc.Center, Projectile.Center);
                    if (dist < maxDist)
                    {
                        maxDist = dist;
                        target = npc;
                    }
                }
            }

            if (target != null && Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, target.position, target.width, target.height))
            {
                Vector2 direction = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 12f;
                int proj = Projectile.NewProjectile(
                    Projectile.GetSource_FromAI(), 
                    Projectile.Center, 
                    direction, 
                    ProjectileID.DiamondBolt,
                    20, 
                    2f, 
                    Projectile.owner 
                );
                if (proj >= 0 && proj < Main.maxProjectiles)
                {
                    Main.projectile[proj].tileCollide = false; 
                    Main.projectile[proj].netUpdate = true; 
                }
            }
        }
    }
}