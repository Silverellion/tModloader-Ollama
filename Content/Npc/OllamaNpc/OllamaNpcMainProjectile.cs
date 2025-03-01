using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OllamaPlayer.Others;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace OllamaPlayer.Content.Npc.OllamaNpc;

public class OllamaNpcMainProjectile : ModNPC
{
	private readonly OllamaNpcActionsAi _ollamaNpcActionsAi = new OllamaNpcActionsAi();
	private static string _chatText = "";
	private static int _chatTimer;
	private int _detectionTimer = 60 * 10;
	
	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[Type] = 25;

		NPCID.Sets.ExtraFramesCount[Type] = 9; // Generally for Town NPCs, but this is how the NPC does extra things such as sitting in a chair and talking to other NPCs. This is the remaining frames after the walking frames.
		NPCID.Sets.AttackFrameCount[Type] = 4; // The amount of frames in the attacking animation.
		NPCID.Sets.DangerDetectRange[Type] = 0; // The amount of pixels away from the center of the NPC that it tries to attack enemies.
		NPCID.Sets.AttackType[Type] = 3; // The type of attack the Town NPC performs. 0 = throwing, 1 = shooting, 2 = magic, 3 = melee
		NPCID.Sets.AttackTime[Type] = 30; // The amount of time it takes for the NPCs attack animation to be over once it starts.
		NPCID.Sets.AttackAverageChance[Type] = 10; // The denominator for the chance for a Town NPC to attack. Lower numbers make the Town NPC appear more aggressive.
		NPCID.Sets.HatOffsetY[Type] = 4; // For when a party is active, the party hat spawns at a Y offset.
		

		NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
		{
			Velocity = 1f, 
			Direction = 1 
		};

		NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
		NPC.Happiness
			.SetBiomeAffection<ForestBiome>(AffectionLevel.Like) 
			.SetBiomeAffection<SnowBiome>(AffectionLevel.Dislike) 
			.SetNPCAffection(NPCID.Dryad, AffectionLevel.Love) 
			.SetNPCAffection(NPCID.Guide, AffectionLevel.Like)
			.SetNPCAffection(NPCID.Merchant, AffectionLevel.Dislike) 
			.SetNPCAffection(NPCID.Demolitionist, AffectionLevel.Hate);
	}
	
	public override void SetDefaults() {
		NPC.townNPC = true; // Sets NPC to be a Town NPC
		NPC.friendly = true; // NPC Will not attack player
		NPC.width = 18;
		NPC.height = 40;
		NPC.aiStyle = 7;
		NPC.damage = 10;
		NPC.defense = 15;
		NPC.lifeMax = 250;
		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;
		NPC.knockBackResist = 0.5f;
		NPC.netAlways = true; // forces NPC to always sync

		AnimationType = NPCID.Guide;
	}
	
	public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
		bestiaryEntry.Info.AddRange([
			BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
		]);
	}

	public static void SetChat(string text, int duration = 600) // 60 frame = 1 sec
	{
		_chatText = text;
		_chatTimer = duration;
	}
	
	public override void AI()
	{
		if (_chatTimer > 0)
			_chatTimer--;

		HandleOrb();
		DetectEnemy();
		Debug.PrintOllamaAiState();
		if(OllamaNpcGlobalValues.AiState == OllamaAiState.Fight)
			_ollamaNpcActionsAi.FightStrongestEnemy(NPC);
	}

	public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		if (_chatTimer > 0 && !string.IsNullOrEmpty(_chatText))
		{
			Vector2 textPosition = NPC.Center - screenPos + new Vector2(0, -40);
			ChatManager.DrawColorCodedStringWithShadow(
				spriteBatch, 
				FontAssets.MouseText.Value, 
				_chatText, 
				textPosition, 
				Color.White, 
				0f, 
				FontAssets.MouseText.Value.MeasureString(_chatText) / 2, 
				Vector2.One
			);
		}
	}

	private void HandleOrb()
	{
		if (!OrbsExist())
		{
			for (int i = 0; i < 4; i++)
			{
				Vector2 offset = Vector2.Zero; 

				if (i == 0) offset = new Vector2(-15, -50);
				if (i == 1) offset = new Vector2(10, -50); 
				if (i == 2) offset = new Vector2(-30, -30);
				if (i == 3) offset = new Vector2(25, -30); 

				int proj = Projectile.NewProjectile(
					NPC.GetSource_FromAI(),
					NPC.Center + offset,
					Vector2.Zero,
					ModContent.ProjectileType<OllamaNpcOrbProjectile>(),
					0,
					0,
					Main.myPlayer
				);

				Main.projectile[proj].ai[0] = NPC.whoAmI; 
				Main.projectile[proj].ai[1] = i; 
			}
		}
	}
	private bool OrbsExist()
	{
		foreach (Projectile proj in Main.projectile)
			if (proj.active 
			    && proj.type == ModContent.ProjectileType<OllamaNpcOrbProjectile>() 
			    && Math.Abs(proj.ai[0] - NPC.whoAmI) < 0.01f)
				return true;
		return false;
	}

	private void DetectEnemy()
	{
		if (_detectionTimer > 0)
		{
			_detectionTimer--;
			return;
		}
		foreach (NPC enemy in Main.npc)
		{
			if (enemy.active && !enemy.friendly && enemy.CanBeChasedBy())
			{
				float distance = Vector2.Distance(NPC.Center, enemy.Center);
				if (distance <= OllamaNpcGlobalValues.OllamaNpcSight)
				{
					if (Collision.CanHitLine(NPC.position, NPC.width, NPC.height, enemy.position, enemy.width, enemy.height))
					{
						_detectionTimer = 60 * 120; 
							if(Main.netMode == NetmodeID.MultiplayerClient)
							{
								ModPacket enemyDetection = ModContent.GetInstance<OllamaPlayer>().GetPacket();
								enemyDetection.Write((byte)OllamaPacketState.OllamaEnemyDetection);
								enemyDetection.Write(enemy.FullName);
								enemyDetection.Send();
							}
							else if (Main.netMode == NetmodeID.SinglePlayer)
								OllamaNpcActions.DetectEnemy(enemy.FullName);
						break;
					}
				}
			}
		}
	}
}	