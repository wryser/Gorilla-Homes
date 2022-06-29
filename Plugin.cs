using BepInEx;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using Utilla;

namespace GorillaHomes
{
    /// <summary>
    /// This is your mod's main class.
    /// </summary>

    /* This attribute tells Utilla to look for [ModdedGameJoin] and [ModdedGameLeave] */
    [ModdedGamemode]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        bool inRoom;
        public int homeIndex = 0;
        public int currentHomeIndex = 0;
        string playerpath;
        string rootPath;
        string[] files;
        string[] fileName;
        static public string home_info_stream;
        static public string[] home_info;
        TextMesh home_name;
        TextMesh home_author;
        public static Plugin instance;

        void OnEnable()
        {
            /* Set up your mod here */
            /* Code here runs at the start and whenever your mod is enabled*/

            HarmonyPatches.ApplyHarmonyPatches();
            Utilla.Events.GameInitialized += OnGameInitialized;
        }

        void OnDisable()
        {
            /* Undo mod setup here */
            /* This provides support for toggling mods with ComputerInterface, please implement it :) */
            /* Code here runs whenever your mod is disabled (including if it disabled on startup)*/

            HarmonyPatches.RemoveHarmonyPatches();
            Utilla.Events.GameInitialized -= OnGameInitialized;
        }

        void OnGameInitialized(object sender, EventArgs e)
        {
            /* Code here runs after the game initializes (i.e. GorillaLocomotion.Player.Instance != null) */
            instance = this;
            rootPath = Directory.GetCurrentDirectory();
            playerpath = Path.Combine(rootPath, "BepInEx", "Plugins", "GorillaHomes", "Homes");

            files = Directory.GetFiles(playerpath, "*.home");
            foreach (var file in files)
            {
                Debug.Log("Found" + file);
            }
            fileName = new string[files.Length];
            for (int i = 0; i < fileName.Length; i++)
            {
                fileName[i] = Path.GetFileName(files[i]);
            }
            Stream str = Assembly.GetExecutingAssembly().GetManifestResourceStream("GorillaHomes.Assets.homeswitcher");
            AssetBundle bundle = AssetBundle.LoadFromStream(str);
            GameObject switcher = bundle.LoadAsset<GameObject>("HomeSwitcher");
            var realswitcher = Instantiate(switcher);
            GameObject forward = GameObject.Find("HomeSwitcher(Clone)/Forward");
            GameObject backward = GameObject.Find("HomeSwitcher(Clone)/Backward");
            home_name = GameObject.Find("HomeSwitcher(Clone)/HomeName").GetComponent<TextMesh>();
            home_author = GameObject.Find("HomeSwitcher(Clone)/HomeAuthor").GetComponent<TextMesh>();
            forward.AddComponent<Forward>();
            forward.layer = 18;
            backward.AddComponent<Backward>();
            backward.layer = 18;
            LoadHome(0);
        }

        public void LoadHome(int index)
        {
            AssetBundle homebundle = AssetBundle.LoadFromFile(Path.Combine(playerpath, fileName[index]));
            if (homebundle != null)
            {
                GameObject assethome = homebundle.LoadAsset<GameObject>("home.SandParent");
                if (assethome != null)
                {
                    var parentAsset = Instantiate(assethome);

                    homebundle.Unload(false);
                    parentAsset.transform.position = new Vector3(-67.2225f, 11.57f, -82.611f);
                    if (!inRoom)
                    {
                        foreach (Collider collider in parentAsset.GetComponentsInChildren<Collider>())
                        {
                            Destroy(collider);
                        }
                    }
                    home_info_stream = parentAsset.GetComponent<Text>().text;
                    home_info = home_info_stream.Split('$');
                    home_name.text = home_info[0].ToUpper();
                    home_author.text = home_info[1].ToUpper();
                }
            }
        }

        static public void UnloadHome(int index)
        {
            GameObject home = GameObject.Find("home.SandParent(Clone)");
            if (home != null)
            {
                Debug.Log("Destroying Home " + home);
                Destroy(home);
            }
        }

        public void Forward()
        {
            homeIndex = homeIndex + 1;
            if (homeIndex >= files.Length)
            {
                UnloadHome(currentHomeIndex);
                homeIndex = 0;
                LoadHome(homeIndex);
                currentHomeIndex = homeIndex;
            }
            else
            {
                UnloadHome(currentHomeIndex);
                LoadHome(homeIndex);
                currentHomeIndex = homeIndex;
            }
        }
        public void Backward()
        {
            homeIndex = homeIndex - 1;
            if (homeIndex < 0)
            {
                UnloadHome(currentHomeIndex);
                homeIndex = files.Length - 1;
                LoadHome(homeIndex);
                currentHomeIndex = homeIndex;
            }
            else
            {
                UnloadHome(currentHomeIndex);
                LoadHome(homeIndex);
                currentHomeIndex = homeIndex;
            }
        }

        void Update()
        {
            /* Code here runs every frame when the mod is enabled */
        }

        /* This attribute tells Utilla to call this method when a modded room is joined */
        [ModdedGamemodeJoin]
        public void OnJoin(string gamemode)
        {
            /* Activate your mod here */
            /* This code will run regardless of if the mod is enabled*/

            inRoom = true;
            LoadHome(homeIndex);
        }

        /* This attribute tells Utilla to call this method when a modded room is left */
        [ModdedGamemodeLeave]
        public void OnLeave(string gamemode)
        {
            /* Deactivate your mod here */
            /* This code will run regardless of if the mod is enabled*/

            inRoom = false;
            LoadHome(homeIndex);
        }
    }
}
