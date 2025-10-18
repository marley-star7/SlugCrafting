global using System;
global using System.Collections.Generic;
global using System.IO;
global using System.Linq;
global using System.Runtime.CompilerServices;
global using System.Runtime.Serialization.Formatters.Binary;

global using UnityEngine;

global using BepInEx;
global using BepInEx.Logging;

global using Mono.Cecil.Cil;
global using MonoMod.Cil;

global using RWCustom;
global using Noise;
global using Menu;
global using Menu.Remix;
global using Menu.Remix.MixedUI;

global using MoreSlugcats;
global using Watcher;

global using ImprovedInput;
global using SlugBase;

global using MRCustom;
global using MRCustom.UI;
global using MRCustom.Events;
global using MRCustom.Math;
global using MRCustom.Modules;
global using MRCustom.Modules.PhysicalObjects;
global using MRCustom.Modules.PhysicalObjects.Rotations;
global using MRCustom.Modules.PlayerCarryableItems;
global using MRCustom.Modules.Weapons;
global using MRCustom.Modules.Creatures;
global using MRCustom.Animations;
global using MRCustom.Physics;
global using MRCustom.Extensions;
global using MRCustom.Contexts;

global using CompartmentalizedCreatureGraphics;
global using CompartmentalizedCreatureGraphics.Cosmetics;
global using CompartmentalizedCreatureGraphics.Cosmetics.Slugcat;
global using CompartmentalizedCreatureGraphics.Extensions;

global using Fisobs.Core;
global using Fisobs.Items;
global using Fisobs.Properties;
global using Fisobs.Sandbox;


global using SlugCrafting;
global using SlugCrafting.Core;
global using SlugCrafting.Modules.Accessories;
global using SlugCrafting.Crafts;
global using SlugCrafting.Creatures;
global using SlugCrafting.Items;
global using SlugCrafting.Properties;
global using SlugCrafting.Menus;
global using SlugCrafting.Items.Weapons;

global using Color = UnityEngine.Color;
global using Vector2 = UnityEngine.Vector2;
global using Vector3 = UnityEngine.Vector3;
global using Random = UnityEngine.Random;
global using Hooks = SlugCrafting.Hooks;
