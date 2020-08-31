using System.Reflection;
using MelonLoader;

[assembly: AssemblyTitle(zcMod.BuildInfo.Description)]
[assembly: AssemblyDescription(zcMod.BuildInfo.Description)]
[assembly: AssemblyCompany(zcMod.BuildInfo.Company)]
[assembly: AssemblyProduct(zcMod.BuildInfo.Name)]
[assembly: AssemblyCopyright("Created by " + zcMod.BuildInfo.Author)]
[assembly: AssemblyTrademark(zcMod.BuildInfo.Company)]
[assembly: AssemblyVersion(zcMod.BuildInfo.Version)]
[assembly: AssemblyFileVersion(zcMod.BuildInfo.Version)]
[assembly: MelonInfo(typeof(zcMod.zcMod), zcMod.BuildInfo.Name, zcMod.BuildInfo.Version, zcMod.BuildInfo.Author, zcMod.BuildInfo.DownloadLink)]


// Create and Setup a MelonGame to mark a Mod as Universal or Compatible with specific Games.
// If no MelonGameAttribute is found or any of the Values for any MelonGame on the Mod is null or empty it will be assumed the Mod is Universal.
// Values for MelonMame can be found in the Game's app.info file or printed at the top of every log directly beneath the Unity version.
[assembly: MelonGame(null, null)]