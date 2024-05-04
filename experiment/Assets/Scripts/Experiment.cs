using System;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;

using Valve.VR;
using UXF;
using TMPro;
using ViveSR.anipal.Eye;


[RequireComponent(typeof(CSVExperimentBuilder))]
[RequireComponent(typeof(CubeMaskScene))]
public class Experiment : MonoBehaviour
{

    public float durationITI = 1f;
    public float durationBlank = 2f;
    public float fadeDuration = 0.5f;
    public Color fadeColor = Color.gray;
    public float previewSceneDuration = 5.0f;
    public bool previewSceneWaitTrigger = true;
    public float startRotationY = 0f;
    public float maxDistanceFromOrigin = 0.3f;
    public float maxViewDirectionError = 40f;

    public GameObject instructionScene; 
    public TextMeshPro instructionText;
    public GameObject cursorCube; // For cube placement
    public GameObject cursorSphere; // To highlight controller origin
    public LaserPointer laserPointer; // Script on right hand controller
    public LaserStick laserStick; // Script on right hand controller
    public AudioSource errorSound; 
    public AudioSource confirmSound;

    private GameObject cursor;
    private GameObject feetIndicator;

    private CubeMaskScene mask;
    
    private SteamVR_Fade fade;
    private int interactablesLayer = 6;

    private bool soundOnViewMisalignment = true;
    private bool soundOnTrialEnd = true;
    private bool previewTargetLabel = false;
    private bool maskSceneEnabled = false;

    private string currentScene = "--none--";
    private List<string> availableScenes;
    private GameObject[] sceneObjects = {null, null, null};
    private bool usesArrangements = false;
    private UXFDataTable arrangementTable = null;
    private List<string> arrangements = new List<string>();

    // Object labels for each scene
    private Dictionary<string, Dictionary<string, string>> objectLabels = new Dictionary<string, Dictionary<string, string>>() {
        {"B01", new Dictionary<string, string>() {{"D", "door"}, 
                                                {"G01", "bathtub"}, 
                                                {"G02", "shower"}, 
                                                {"G03", "sinks"}, 
                                                {"G04", "toilet"}, 
                                                {"G05", "mirrors"}, 
                                                {"G06", "shelf"}, 
                                                {"G07", "cabinet"}, 
                                                {"L01", "toothbrushes"}, 
                                                {"L02", "candle"}, 
                                                {"L03", "toiletpaper"}, 
                                                {"L04", "towelstack"}, 
                                                {"L05", "shampoobottles"}, 
                                                {"L06", "soapdispenser"}, 
                                                {"L07", "rug"},
                                                {"L08", "towelonhook"}, 
                                                {"L09", "toiletbrush"}, 
                                                {"L10", "cups"}, 
                                                {"L11", "soapbar"}, 
                                                {"L12", "decorativestones"}, 
                                                {"L13", "laundrybasket"}, 
                                                {"L14", "slippers"}, 
                                                {"L15", "pictureframes"},
                                                {"L16", "vases"}, 
                                                {"L17", "lamp"}, 
                                                {"L18", "plant"}, 
                                                {"L19", "hairdryer"}, 
                                                {"L20", "comb"}, 
                                                {"L21", "weighingscale"}, 
                                                {"L22", "rubberduck"},
                                                {"L23", "lipstick"}, 
                                                {"L24", "toothpaste"}, 
                                                {"L25", "perfume"}, 
                                                {"L26", "clotheshanger"}, 
                                                {"L27", "incensesticks"}, 
                                                {"L28", "clock"}} },
        {"B02", new Dictionary<string, string>() {{"D", "door"}, 
                                                {"G01", "bench"}, 
                                                {"G02", "toilet"}, 
                                                {"G03", "washstand"}, 
                                                {"G04", "floorlamp"}, 
                                                {"G05", "shower"}, 
                                                {"G06", "bathtub"}, 
                                                {"G07", "shelf"}, 
                                                {"L01", "plant"}, 
                                                {"L02", "shampoo"}, 
                                                {"L03", "soapbar"}, 
                                                {"L04", "toothbrushes"}, 
                                                {"L05", "towels"}, 
                                                {"L06", "laundrybasket"}, 
                                                {"L07", "magazine"},
                                                {"L08", "toiletpaper"}, 
                                                {"L09", "razor"}, 
                                                {"L10", "toweldryer"}, 
                                                {"L11", "plunger"}, 
                                                {"L12", "books"}, 
                                                {"L13", "painting"}, 
                                                {"L14", "champagne"}, 
                                                {"L15", "boxes"},
                                                {"L16", "africanart"}, 
                                                {"L17", "candles"}, 
                                                {"L18", "goldenegg"}, 
                                                {"L19", "tableclock"}, 
                                                {"L20", "pottery"}, 
                                                {"L21", "cage"}, 
                                                {"L22", "pacifier"},
                                                {"L23", "flowers"}, 
                                                {"L24", "walker"}, 
                                                {"L25", "pills"}, 
                                                {"L26", "hairbrush"}, 
                                                {"L27", "footmat"}, 
                                                {"L28", "trashcan"}} },
        {"B03", new Dictionary<string, string>() {{"D", "door"}, 
                                                {"G01", "shelf"}, 
                                                {"G02", "hanginglamps"}, 
                                                {"G03", "cabinets"}, 
                                                {"G04", "washbasins"}, 
                                                {"G05", "toilet"}, 
                                                {"G06", "bathtub"}, 
                                                {"G07", "bidet"}, 
                                                {"L01", "plant"}, 
                                                {"L02", "flowers"}, 
                                                {"L03", "candles"}, 
                                                {"L04", "vases"}, 
                                                {"L05", "cylindricbox"}, 
                                                {"L06", "soapbar"}, 
                                                {"L07", "towels"},
                                                {"L08", "soapdispenser"}, 
                                                {"L09", "toothbrushes"}, 
                                                {"L10", "showerrack"}, 
                                                {"L11", "toiletpaper"}, 
                                                {"L12", "curlingiron"}, 
                                                {"L13", "deodorant"}, 
                                                {"L14", "luckycat"}, 
                                                {"L15", "decorativebull"},
                                                {"L16", "trashcan"}, 
                                                {"L17", "gong"}, 
                                                {"L18", "jewelrypan"}, 
                                                {"L19", "pipe"}, 
                                                {"L20", "manicurekit"}, 
                                                {"L21", "thermometer"}, 
                                                {"L22", "glasses"},
                                                {"L23", "hourglass"}, 
                                                {"L24", "painting"}, 
                                                {"L25", "toiletbrush"}, 
                                                {"L26", "toothpaste"}, 
                                                {"L27", "handsanitizer"}, 
                                                {"L28", "decorativebird"}} },
        {"K01", new Dictionary<string, string>() {{"D", "door"}, 
                                                {"G01", "kitchencounter"}, 
                                                {"G02", "cabinet"}, 
                                                {"G03", "refrigerator"}, 
                                                {"G04", "oven"}, 
                                                {"G05", "exhaust"}, 
                                                {"G06", "sink"}, 
                                                {"G07", "tableandchairs"}, 
                                                {"L01", "eggtimer"}, 
                                                {"L02", "toaster"}, 
                                                {"L03", "spices"}, 
                                                {"L04", "bunsinbasket"}, 
                                                {"L05", "plant"}, 
                                                {"L06", "trashcan"}, 
                                                {"L07", "coffeemaker"},
                                                {"L08", "mortar"}, 
                                                {"L09", "tablesetting"}, 
                                                {"L10", "blender"}, 
                                                {"L11", "kitchenscale"}, 
                                                {"L12", "banana"}, 
                                                {"L13", "cuttingboard"}, 
                                                {"L14", "knifeblock"}, 
                                                {"L15", "waterbottle"},
                                                {"L16", "flowers"}, 
                                                {"L17", "lights"}, 
                                                {"L18", "kitchentowel"}, 
                                                {"L19", "electrickettle"}, 
                                                {"L20", "microwave"}, 
                                                {"L21", "chocolates"}, 
                                                {"L22", "strawberries"},
                                                {"L23", "utensilrack"}, 
                                                {"L24", "breadbox"}, 
                                                {"L25", "picture"}, 
                                                {"L26", "candle"}, 
                                                {"L27", "bowls"}, 
                                                {"L28", "stoveplates"}} },
        {"K02", new Dictionary<string, string>() {{"D", "door"}, 
                                                {"G01", "kitchencounter"}, 
                                                {"G02", "stove"}, 
                                                {"G03", "sink"}, 
                                                {"G04", "exhaust"}, 
                                                {"G05", "wallcupboards"}, 
                                                {"G06", "refrigerator"}, 
                                                {"G07", "diningtable"}, 
                                                {"L01", "mangos"}, 
                                                {"L02", "pastries"}, 
                                                {"L03", "coffeemaker"}, 
                                                {"L04", "halogenspot"}, 
                                                {"L05", "roses"}, 
                                                {"L06", "electrickettle"}, 
                                                {"L07", "deepfryer"},
                                                {"L08", "jugs"}, 
                                                {"L09", "plant"}, 
                                                {"L10", "ovendishes"}, 
                                                {"L11", "pizzaleftovers"}, 
                                                {"L12", "dishes"}, 
                                                {"L13", "candles"}, 
                                                {"L14", "pots"}, 
                                                {"L15", "bread"},
                                                {"L16", "woodentoy"}, 
                                                {"L17", "babybottles"}, 
                                                {"L18", "chopsticks"}, 
                                                {"L19", "vacuum"}, 
                                                {"L20", "sponge"}, 
                                                {"L21", "pan"}, 
                                                {"L22", "childsdrawing"},
                                                {"L23", "spatula"}, 
                                                {"L24", "travelmug"}, 
                                                {"L25", "cheesegrater"}, 
                                                {"L26", "worldmap"}, 
                                                {"L27", "papertowels"}, 
                                                {"L28", "decorativecat"}} },
        {"K03", new Dictionary<string, string>() {{"D", "door"}, 
                                                {"G01", "wallcupboards"}, 
                                                {"G02", "stovecounter"}, 
                                                {"G03", "sinkcounter"}, 
                                                {"G04", "kitchentable"}, 
                                                {"G05", "refrigerator"}, 
                                                {"G06", "spotlights"}, 
                                                {"G07", "treeshelf"}, 
                                                {"L01", "turkey"}, 
                                                {"L02", "pans"}, 
                                                {"L03", "microwave"}, 
                                                {"L04", "toaster"}, 
                                                {"L05", "flowers"}, 
                                                {"L06", "soup"}, 
                                                {"L07", "candles"},
                                                {"L08", "utensils"}, 
                                                {"L09", "muffins"}, 
                                                {"L10", "pots"}, 
                                                {"L11", "pillspeaker"}, 
                                                {"L12", "cutlerycup"}, 
                                                {"L13", "pineapple"}, 
                                                {"L14", "tablet"}, 
                                                {"L15", "kitchenherbs"},
                                                {"L16", "electrickettle"}, 
                                                {"L17", "coffeemaker"}, 
                                                {"L18", "broom"}, 
                                                {"L19", "cookbook"}, 
                                                {"L20", "garlic"}, 
                                                {"L21", "batteries"}, 
                                                {"L22", "servingcart"},
                                                {"L23", "pastajar"}, 
                                                {"L24", "corkscrew"}, 
                                                {"L25", "tea"}, 
                                                {"L26", "peppermill"}, 
                                                {"L27", "carkey"}, 
                                                {"L28", "ginger"}}  },
        {"L01", new Dictionary<string, string>() {{"D", "door"}, 
                                                {"G01", "couch"}, 
                                                {"G02", "armchair"}, 
                                                {"G03", "coffeetable"}, 
                                                {"G04", "shelf"}, 
                                                {"G05", "chair"}, 
                                                {"G06", "carpet"}, 
                                                {"G07", "cabinet"}, 
                                                {"L01", "nachos"}, 
                                                {"L02", "remotecontrol"}, 
                                                {"L03", "candle"}, 
                                                {"L04", "book"}, 
                                                {"L05", "tribalmask"}, 
                                                {"L06", "paperboxes"}, 
                                                {"L07", "tablet"},
                                                {"L08", "television"}, 
                                                {"L09", "apples"}, 
                                                {"L10", "flowers"}, 
                                                {"L11", "tablelamp"}, 
                                                {"L12", "camera"}, 
                                                {"L13", "controllers"}, 
                                                {"L14", "speaker"}, 
                                                {"L15", "fishbowl"},
                                                {"L16", "router"}, 
                                                {"L17", "toytruck"}, 
                                                {"L18", "picture"}, 
                                                {"L19", "fan"}, 
                                                {"L20", "clef"}, 
                                                {"L21", "walnuts"}, 
                                                {"L22", "headphones"},
                                                {"L23", "boxfiles"}, 
                                                {"L24", "vase"}, 
                                                {"L25", "stepper"}, 
                                                {"L26", "clock"}, 
                                                {"L27", "vrhmd"}, 
                                                {"L28", "cactus"}} },
        {"L02", new Dictionary<string, string>() {{"D", "door"}, 
                                                {"G01", "coffeetable"}, 
                                                {"G02", "piano"}, 
                                                {"G03", "hanginglamp"}, 
                                                {"G04", "carpet"}, 
                                                {"G05", "fireplace"}, 
                                                {"G06", "couch"}, 
                                                {"G07", "shelf"}, 
                                                {"L01", "blu-rays"}, 
                                                {"L02", "blanket"}, 
                                                {"L03", "radio"}, 
                                                {"L04", "crucifix"}, 
                                                {"L05", "poster"}, 
                                                {"L06", "lavalamp"}, 
                                                {"L07", "handbag"},
                                                {"L08", "cigarettepack"}, 
                                                {"L09", "metronome"}, 
                                                {"L10", "rockinghorse"}, 
                                                {"L11", "laptop"}, 
                                                {"L12", "ivstand"}, 
                                                {"L13", "sandwiches"}, 
                                                {"L14", "flowers"}, 
                                                {"L15", "recordplayer"},
                                                {"L16", "cocktail"}, 
                                                {"L17", "microphone"}, 
                                                {"L18", "desklamp"}, 
                                                {"L19", "rocherglass"}, 
                                                {"L20", "bookstack"}, 
                                                {"L21", "decorativeheads"}, 
                                                {"L22", "candles"},
                                                {"L23", "roomba"}, 
                                                {"L24", "pieceofcake"}, 
                                                {"L25", "vases"}, 
                                                {"L26", "pictures"}, 
                                                {"L27", "mug"}, 
                                                {"L28", "toyball"}} },
        {"L03", new Dictionary<string, string>() {{"D", "door"}, 
                                                {"G01", "coffeetable"}, 
                                                {"G02", "couch"}, 
                                                {"G03", "xmastree"}, 
                                                {"G04", "peacockchair"}, 
                                                {"G05", "tvstand"}, 
                                                {"G06", "shelf"}, 
                                                {"G07", "carpet"}, 
                                                {"L01", "apples"}, 
                                                {"L02", "television"}, 
                                                {"L03", "presents"}, 
                                                {"L04", "decorativepines"}, 
                                                {"L05", "gameboy"}, 
                                                {"L06", "bells"}, 
                                                {"L07", "treedecorationbox"},
                                                {"L08", "flowers"}, 
                                                {"L09", "mirror"}, 
                                                {"L10", "speaker"}, 
                                                {"L11", "vases"}, 
                                                {"L12", "books"}, 
                                                {"L13", "xmascookies"}, 
                                                {"L14", "baumkuchen"}, 
                                                {"L15", "xmaswreath"},
                                                {"L16", "almonds"}, 
                                                {"L17", "tricycle"}, 
                                                {"L18", "legos"}, 
                                                {"L19", "pinecones"}, 
                                                {"L20", "tangerines"}, 
                                                {"L21", "keyboard"}, 
                                                {"L22", "camera"},
                                                {"L23", "headphones"}, 
                                                {"L24", "pillows"}, 
                                                {"L25", "adventcalendar"}, 
                                                {"L26", "candycane"}, 
                                                {"L27", "chainoflights"}, 
                                                {"L28", "xmasstocking"}} },
        {"O01", new Dictionary<string, string>() {{"D", "door"}, 
                                                {"G01", "shelf"}, 
                                                {"G02", "couch"}, 
                                                {"G03", "cabinet"}, 
                                                {"G04", "drawers"}, 
                                                {"G05", "copier"}, 
                                                {"G06", "floorlamp"}, 
                                                {"G07", "meetingtable"}, 
                                                {"L01", "calendar"}, 
                                                {"L02", "landline"}, 
                                                {"L03", "screen"}, 
                                                {"L04", "airconditioner"}, 
                                                {"L05", "newspaper"}, 
                                                {"L06", "clock"}, 
                                                {"L07", "shredder"},
                                                {"L08", "fruitbowl"}, 
                                                {"L09", "encyclopedia"}, 
                                                {"L10", "cellphone"}, 
                                                {"L11", "holepuncher"}, 
                                                {"L12", "calculator"}, 
                                                {"L13", "whiskeybottle"}, 
                                                {"L14", "magazinestand"}, 
                                                {"L15", "doughnuts"},
                                                {"L16", "flowers"}, 
                                                {"L17", "laptop"}, 
                                                {"L18", "poster"}, 
                                                {"L19", "fishbowl"}, 
                                                {"L20", "metalorbits"}, 
                                                {"L21", "walllamp"}, 
                                                {"L22", "coffeemaker"},
                                                {"L23", "decorativewood"}, 
                                                {"L24", "bonsai"}, 
                                                {"L25", "chewinggum"}, 
                                                {"L26", "papertrash"}, 
                                                {"L27", "scissors"}, 
                                                {"L28", "candybar"}} },
        {"O02", new Dictionary<string, string>() {{"D", "door"}, 
                                                {"G01", "couch"}, 
                                                {"G02", "desk"}, 
                                                {"G03", "shelf"}, 
                                                {"G04", "watercooler"}, 
                                                {"G05", "airconditioner"}, 
                                                {"G06", "cabinet"}, 
                                                {"G07", "hanginglamp"}, 
                                                {"L01", "sushi"}, 
                                                {"L02", "computer"}, 
                                                {"L03", "printer"}, 
                                                {"L04", "keyboard"}, 
                                                {"L05", "landline"}, 
                                                {"L06", "desklamp"}, 
                                                {"L07", "dvd"},
                                                {"L08", "stapler"}, 
                                                {"L09", "flowers"}, 
                                                {"L10", "claviature"}, 
                                                {"L11", "cookies"}, 
                                                {"L12", "glassofmilk"}, 
                                                {"L13", "decorativebirds"}, 
                                                {"L14", "telescope"}, 
                                                {"L15", "tableclock"},
                                                {"L16", "mouse"}, 
                                                {"L17", "books"}, 
                                                {"L18", "helicopter"}, 
                                                {"L19", "harddrive"}, 
                                                {"L20", "rubikscube"}, 
                                                {"L21", "briefcase"}, 
                                                {"L22", "keys"},
                                                {"L23", "globe"}, 
                                                {"L24", "snowglobe"}, 
                                                {"L25", "ashtray"}, 
                                                {"L26", "cushion"}, 
                                                {"L27", "umbrella"}, 
                                                {"L28", "magnifyingglass"}} },
        {"O03", new Dictionary<string, string>() {{"D", "door"}, 
                                                {"G01", "meetingtable"}, 
                                                {"G02", "shelf"}, 
                                                {"G03", "cot"}, 
                                                {"G04", "desk"}, 
                                                {"G05", "chestofdrawers"}, 
                                                {"G06", "floorlamp"}, 
                                                {"G07", "filingcabinet"}, 
                                                {"L01", "vases"}, 
                                                {"L02", "oranges"}, 
                                                {"L03", "landline"}, 
                                                {"L04", "books"}, 
                                                {"L05", "wheelchair"}, 
                                                {"L06", "printer"}, 
                                                {"L07", "candle"},
                                                {"L08", "plants"}, 
                                                {"L09", "flowers"}, 
                                                {"L10", "desklamp"}, 
                                                {"L11", "sandwiches"}, 
                                                {"L12", "projector"}, 
                                                {"L13", "tablet"}, 
                                                {"L14", "diskdrive"}, 
                                                {"L15", "violin"},
                                                {"L16", "fan"}, 
                                                {"L17", "pastry"}, 
                                                {"L18", "mouse"}, 
                                                {"L19", "aquarium"}, 
                                                {"L20", "keyboard"}, 
                                                {"L21", "computerscreen"}, 
                                                {"L22", "towercomputer"},
                                                {"L23", "disk"}, 
                                                {"L24", "paperholder"}, 
                                                {"L25", "ruler"}, 
                                                {"L26", "multisocket"}, 
                                                {"L27", "parcel"}, 
                                                {"L28", "microphone"}} },
        {"S01", new Dictionary<string, string>() {{"D", "door"}, 
                                                {"G01", "bed"}, 
                                                {"G02", "walllamps"}, 
                                                {"G03", "fireplace"}, 
                                                {"G04", "beanbag"}, 
                                                {"G05", "nightstand"}, 
                                                {"G06", "dresser"}, 
                                                {"G07", "shelves"}, 
                                                {"L01", "mirror"}, 
                                                {"L02", "flowers"}, 
                                                {"L03", "guitar"}, 
                                                {"L04", "breakfast"}, 
                                                {"L05", "books"}, 
                                                {"L06", "laptop"}, 
                                                {"L07", "bags"},
                                                {"L08", "television"}, 
                                                {"L09", "speaker"}, 
                                                {"L10", "fan"}, 
                                                {"L11", "candle"}, 
                                                {"L12", "ipod"}, 
                                                {"L13", "poster"}, 
                                                {"L14", "decorativenumbers"}, 
                                                {"L15", "tablelamp"},
                                                {"L16", "earbuds"}, 
                                                {"L17", "owl"}, 
                                                {"L18", "stool"}, 
                                                {"L19", "vacuum"}, 
                                                {"L20", "plant"}, 
                                                {"L21", "tissuebox"}, 
                                                {"L22", "handfan"},
                                                {"L23", "sunglasses"}, 
                                                {"L24", "basketball"}, 
                                                {"L25", "hat"}, 
                                                {"L26", "binoculars"}, 
                                                {"L27", "alarmclock"}, 
                                                {"L28", "bowl"}} },
        {"S02", new Dictionary<string, string>() {{"D", "door"}, 
                                                {"G01", "changingtable"}, 
                                                {"G02", "nightstand"}, 
                                                {"G03", "hammockchair"}, 
                                                {"G04", "dresser"}, 
                                                {"G05", "floorlamp"}, 
                                                {"G06", "babycrib"}, 
                                                {"G07", "bed"}, 
                                                {"L01", "mirror"}, 
                                                {"L02", "stroller"},
                                                {"L03", "boxes"}, 
                                                {"L04", "stool"}, 
                                                {"L05", "candy"}, 
                                                {"L06", "plant"}, 
                                                {"L07", "apple"},
                                                {"L08", "playrug"}, 
                                                {"L09", "toytrain"}, 
                                                {"L10", "tealight"}, 
                                                {"L11", "buildingbricks"}, 
                                                {"L12", "peanuts"}, 
                                                {"L13", "pretzel"}, 
                                                {"L14", "towel"}, 
                                                {"L15", "ironingboard"},
                                                {"L16", "cellphone"}, 
                                                {"L17", "router"}, 
                                                {"L18", "walllamp"}, 
                                                {"L19", "rattle"}, 
                                                {"L20", "jewelcase"}, 
                                                {"L21", "tennisracket"}, 
                                                {"L22", "vitruvianman"},
                                                {"L23", "alarmclock"}, 
                                                {"L24", "book"}, 
                                                {"L25", "eiffeltower"}, 
                                                {"L26", "flowers"}, 
                                                {"L27", "buddha"}, 
                                                {"L28", "flashdrive"}} },
        {"S03", new Dictionary<string, string>() {{"D", "door"}, 
                                                {"G01", "dresser"}, 
                                                {"G02", "hanginglamp"}, 
                                                {"G03", "weightbench"}, 
                                                {"G04", "exercisebike"}, 
                                                {"G05", "treadmill"}, 
                                                {"G06", "bed"}, 
                                                {"G07", "mirror"}, 
                                                {"L01", "dustpan"}, 
                                                {"L02", "drinkingbottle"}, 
                                                {"L03", "fruitbowl"}, 
                                                {"L04", "candles"}, 
                                                {"L05", "emojicushion"}, 
                                                {"L06", "trophy"}, 
                                                {"L07", "electricguitar"},
                                                {"L08", "camera"}, 
                                                {"L09", "readinglights"}, 
                                                {"L10", "television"}, 
                                                {"L11", "smartwatch"}, 
                                                {"L12", "ipod"}, 
                                                {"L13", "creamcheesebun"}, 
                                                {"L14", "books"}, 
                                                {"L15", "cereal"},
                                                {"L16", "speaker"}, 
                                                {"L17", "laptop"}, 
                                                {"L18", "plant"}, 
                                                {"L19", "poster"}, 
                                                {"L20", "headphones"}, 
                                                {"L21", "flowers"}, 
                                                {"L22", "inhaler"},
                                                {"L23", "statue"}, 
                                                {"L24", "americanfootball"}, 
                                                {"L25", "tie"}, 
                                                {"L26", "dartboard"}, 
                                                {"L27", "baseballbat"}, 
                                                {"L28", "horseshoe"}} }
    };

    void Update() {

    }
    
    void Start() {

        cursor = GameObject.Find("CursorCube");
        feetIndicator = GameObject.Find("HomePosition");

        cursorCube.SetActive(false);
        cursorSphere.SetActive(false);

        instructionScene = GameObject.Find("InstructionScene");
        instructionText = GameObject.Find("InstructionText").GetComponent<TextMeshPro>();

        mask = GetComponent<CubeMaskScene>();

        // Make sure all scenes except BaseScene are unloaded on start
        availableScenes = FindAvailableScenes();
        foreach(string scn in availableScenes) {
            try {
                SceneManager.UnloadSceneAsync(scn);
            } catch (Exception) {
                // Ignore scenes that are already unloaded
            }
        }

        if (laserPointer == null) laserPointer = GameObject.Find("Controller (right)").GetComponent<LaserPointer>();
        if (laserStick == null) laserStick = GameObject.Find("Controller (right)").GetComponent<LaserStick>();
    }


    public void SetUpExperiment(Session session) {

        // Generate trials based on input CSV file
        GetComponent<CSVExperimentBuilder>().BuildExperiment(session);
        Debug.Log(string.Format("* Imported trial design from {0} ({1} trials)", 
                                session.settings.GetString("trial_specification_name"), 
                                session.blocks[0].trials.Count));


        // Randomize trials (allowing JSON file to control randomization)
        bool randomize = true;
        if (session.settings.ContainsKey("randomize_trials")) { 
            if (!session.settings.GetBool("randomize_trials")) randomize = false;
        }
        if (randomize) session.blocks[0].trials.Shuffle();

        // Set experiment configuration parameters from JSON
        if (session.settings.ContainsKey("fade_duration")) fadeDuration = session.settings.GetFloat("fade_duration");
        if (session.settings.ContainsKey("iti_duration")) durationITI = session.settings.GetFloat("iti_duration");
        if (session.settings.ContainsKey("preview_duration")) previewSceneDuration = session.settings.GetFloat("preview_duration");
        if (session.settings.ContainsKey("preview_wait_for_trigger")) previewSceneWaitTrigger = session.settings.GetBool("preview_wait_for_trigger");
        if (session.settings.ContainsKey("preview_target_label")) previewTargetLabel = session.settings.GetBool("preview_target_label");
        if (session.settings.ContainsKey("sound_on_view_misalignment")) soundOnViewMisalignment = session.settings.GetBool("sound_on_view_misalignment");
        if (session.settings.ContainsKey("sound_on_trial_end")) soundOnTrialEnd = session.settings.GetBool("sound_on_trial_end");
        if (session.settings.ContainsKey("show_mask_scene")) maskSceneEnabled = session.settings.GetBool("show_mask_scene");
        if (session.settings.ContainsKey("blank_duration")) {
            if (session.settings.GetFloat("blank_duration") >= 1f) {
                durationBlank = session.settings.GetFloat("blank_duration");
            } else {
                durationBlank = fadeDuration * 2.0f; // Ensure at least enough time for fade effect
            }
        }

        // Read arrangement information, if a CSV file was specified
        if (session.settings.ContainsKey("arrangements_csv")) {

            string csvFile = Path.GetFullPath(Path.Combine(Application.streamingAssetsPath, session.settings.GetString("arrangements_csv")));
            if (!File.Exists(csvFile))
            {
                throw new Exception(string.Format("Specified arrangements file {0} does not exist! Must be in StreamingAssets folder!", csvFile));
            } else {

                string[] csvLines = File.ReadAllLines(csvFile);
                arrangementTable = UXFDataTable.FromCSV(csvLines);

                // Collect individual arrangements
                foreach (var row in arrangementTable.GetAsListOfDict()) {
                    foreach(KeyValuePair<string, object> pair in row) {
                        if ((pair.Key == "arrangement") & (!arrangements.Contains((string)pair.Value))) arrangements.Add((string)pair.Value);
                    }
                }
                Debug.Log(string.Format("* Read {0} arrangements from {1}.", arrangements.Count, csvFile));
                usesArrangements = true;
            }

        }

        // Calibrate eye tracker
        if (session.settings.ContainsKey("calibrate_eye_tracker") && session.settings.GetBool("calibrate_eye_tracker")) {
            SRanipal_Eye_API.LaunchEyeCalibration(System.IntPtr.Zero);
        }

        // Start the first trial
        session.BeginNextTrial();

    }


    public void TrialStart(Trial trial) {
        StartCoroutine("RunTrial", trial);
    }


    IEnumerator RunTrial(Trial trial) {

        SteamVR_Action_Boolean trigger = SteamVR_Actions._default.InteractUI;
        SteamVR_Action_Boolean grip = SteamVR_Actions._default.GrabGrip;

        // Ask participant to stand on starting location in correct direction
        if (trial.settings.ContainsKey("view_rotation")) 
        {
            startRotationY = trial.settings.GetFloat("view_rotation");
        } else {
            startRotationY = 0f;
        }
        feetIndicator.transform.rotation = Quaternion.Euler(0f, startRotationY, 0f);
        if ((trial.settings.ContainsKey("view_x")) & (trial.settings.ContainsKey("view_z"))) {
            feetIndicator.transform.position = new Vector3(trial.settings.GetFloat("view_x"),
                                                           0f,
                                                           trial.settings.GetFloat("view_z"));
        } else {
            feetIndicator.transform.position = Vector3.zero;
        }
        feetIndicator.SetActive(true);
        
        // Show instructions
        string instruction = "Please stand on the blue markings<br>and press the trigger to start!";
        if ((previewTargetLabel) && (trial.settings.ContainsKey("preview_label"))) {
            instruction = string.Format("Please stand on the blue markings<br>and press the trigger to start!<br>Next Object: {0}", trial.settings.GetString("preview_label"));
        }
        instructionText.text = instruction;
        instructionScene.SetActive(true);
        
        // Check starting position and wait for trigger press
        yield return StartCoroutine(WaitForStartPosition(trial, trigger, soundOnViewMisalignment));
        trial.result["t_starting_pos"] = Time.time;
        trial.result["standing_y_rotation"] = Camera.main.transform.eulerAngles.y;

        // Fade to grey screen
        instructionScene.SetActive(false);
        SteamVR_Fade.Start(fadeColor, fadeDuration);
        yield return new WaitForSeconds(fadeDuration);

        // Load scene for this trial if necessary
        string trialScene = "";
        if (trial.settings.ContainsKey("scene")) {
            trialScene = trial.settings.GetString("scene");
            if (currentScene != trialScene) {
                // Load the correct scene
                Debug.Log(string.Format("Switching scene: trialScene={0}, currentScene={1}", trialScene, currentScene));
                yield return StartCoroutine("SelectScene", trialScene);
                currentScene = trialScene;
                sceneObjects[0] = GameObject.Find(currentScene);
                sceneObjects[1] = GameObject.Find("LightSourceSun");
                sceneObjects[2] = GameObject.Find("LightSourceRoom");
            } else {
                // Use loaded scene, set elements back to visible
                Debug.Log(string.Format("Re-using current scene: {0}", currentScene));
                foreach (GameObject so in sceneObjects) so.SetActive(true);
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(currentScene));
            }
        }

        // Set up arrangement if enabled and specified in trial file
        if ((usesArrangements) & (trial.settings.ContainsKey("arrangement"))) {
            SetUpArrangement(trial.settings.GetString("arrangement"));
        }

        // Get target object and record its position
        string targetName = "";
        string targetLabel = "-unknown label-";
        GameObject target = null;
        Vector3 tarPos = Vector3.zero;
        Quaternion tarRot = Quaternion.identity;
        Bounds tarBounds = new Bounds();
        if (trial.settings.ContainsKey("target")) {
            targetName = trial.settings.GetString("target");
            target = GameObject.Find(targetName);
            tarPos = target.transform.position;
            tarRot = target.transform.rotation;

            // Get text label for preview cue / log file
            string[] tarL = targetName.Split('_');
            if (objectLabels.ContainsKey(currentScene)) {
                if (objectLabels[currentScene].ContainsKey(tarL[1])) {
                    targetLabel = objectLabels[currentScene][tarL[1]];
                }
            }
            trial.result["target_label"] = targetLabel;
            
            // Target position and orientation, pivot-based
            trial.result["target_pos_x"] = tarPos.x;
            trial.result["target_pos_y"] = tarPos.y;
            trial.result["target_pos_z"] = tarPos.z;
            trial.result["target_euler_x"] = tarRot.eulerAngles.x;
            trial.result["target_euler_y"] = tarRot.eulerAngles.y;
            trial.result["target_euler_z"] = tarRot.eulerAngles.z;

            // Target position and size, bounding box based
            tarBounds = GetBounds(target);
            trial.result["target_bbox_center_x"] = tarBounds.center.x;
            trial.result["target_bbox_center_y"] = tarBounds.center.y;
            trial.result["target_bbox_center_z"] = tarBounds.center.z;
            trial.result["target_bbox_size_x"] = tarBounds.size.x;
            trial.result["target_bbox_size_y"] = tarBounds.size.y;
            trial.result["target_bbox_size_z"] = tarBounds.size.z;
        }

        // Determine response mode for current trial (default: object placement)
        string response_mode = "point";
        if(trial.settings.ContainsKey("response_mode")) {
            response_mode = trial.settings.GetString("response_mode");
        }

        Debug.Log(string.Format("* Starting trial {0}, scene={3}, target={1} ({4}), response_mode={2}.", trial.number, targetName, response_mode, trialScene, targetLabel));

        // Preview scene until trigger press (if enabled) or time limit
        SteamVR_Fade.Start(Color.clear, fadeDuration);
        trial.result["t_scene_preview_start"] = Time.time;
        if (previewSceneWaitTrigger) {
            while(true) {
                if (trigger.GetStateDown(SteamVR_Input_Sources.RightHand)) {
                    trial.result["t_scene_preview_end"] = Time.time;
                    break;
                }
                yield return null;
            }
        } else {
            yield return new WaitForSeconds(previewSceneDuration);
            trial.result["t_scene_preview_end"] = Time.time;
        }

        // Fade scene to black or mask for given period of time, hide target
        if (!maskSceneEnabled) {
            
            // Fade to grey and back after specified time
            SteamVR_Fade.Start(fadeColor, fadeDuration);
            yield return new WaitForSeconds(durationBlank);
            if (target != null) target.SetActive(false);
            SteamVR_Fade.Start(Color.clear, fadeDuration);

        } else {
            
            // Fade to mask via grey
            mask.Randomize();
            SteamVR_Fade.Start(fadeColor, fadeDuration/2.0f);
            yield return new WaitForSeconds(fadeDuration/2.0f);
            foreach (GameObject so in sceneObjects) so.SetActive(false);
            mask.Show();
            SteamVR_Fade.Start(Color.clear, fadeDuration/2.0f);
            yield return new WaitForSeconds(durationBlank);

            // Fade back to scene
            if (target != null) target.SetActive(false);
            SteamVR_Fade.Start(fadeColor, fadeDuration/2.0f);
            yield return new WaitForSeconds(fadeDuration/2.0f);
            mask.Hide();
            foreach (GameObject so in sceneObjects) so.SetActive(true);
            SteamVR_Fade.Start(Color.clear, fadeDuration/2.0f);

        }
        trial.result["t_response_start"] = Time.time;

        // Move objects
        bool hasShiftedObjects = false;
        Dictionary<string, Vector3> previousPositions = new Dictionary<string, Vector3>();
        if(trial.settings.ContainsKey("shift_objects")) {
            string shift_objects = trial.settings.GetString("shift_objects");
            if (shift_objects.Length > 0) {

                hasShiftedObjects = true;
                Vector3 shift_vector = new Vector3(trial.settings.GetFloat("shift_x"), 
                                                   trial.settings.GetFloat("shift_y"), 
                                                   trial.settings.GetFloat("shift_z"));

                string[] objects = shift_objects.Split(' ');
                foreach(string s in objects) {

                    try {

                        Transform o = GameObject.Find(s).transform;
                        previousPositions[s] = o.position;
                        o.Translate(shift_vector, Space.World);
                        Debug.Log(string.Format("* Shifting {0} by vector {1}", s, shift_vector));
                    
                    } catch(NullReferenceException) {
                        
                        Debug.LogWarning(string.Format("* Object {0} specified in trial file but does not exist in scene!", s));

                    }  
                }


            }
        }

        // Set up response mode for this trial
        Vector3 spawnPoint = new Vector3(0f, Camera.main.transform.position.y * 0.75f, 0.6f);
        if ((startRotationY > 315f) || (startRotationY <= 45f)) {
            
            spawnPoint = new Vector3(0f, Camera.main.transform.position.y * 0.75f, 0.6f);

        } else if ((startRotationY > 45f) && (startRotationY <= 135f)) {
            
            spawnPoint = new Vector3(0.6f, Camera.main.transform.position.y * 0.75f, 0f);

        } else if ((startRotationY > 135f) && (startRotationY <= 225f)) {
            
            spawnPoint = new Vector3(0f, Camera.main.transform.position.y * 0.75f, -0.6f);

        } else if ((startRotationY > 225f) && (startRotationY <= 315f)) {
            
            spawnPoint = new Vector3(-0.6f, Camera.main.transform.position.y * 0.75f, 0f);

        }

        switch (response_mode) {

            case "object":

                // Object placement: create a copy of the original object...
                cursor = Instantiate(target, spawnPoint, target.transform.localRotation, target.transform.parent) as GameObject;

                // ...and make it interactable using the controller
                Valve.VR.InteractionSystem.Interactable interactable = cursor.AddComponent<Valve.VR.InteractionSystem.Interactable>();
                interactable.highlightOnHover = true;
                interactable.hideHandOnAttach = false;
                interactable.hideHighlight = new GameObject[0]; // needed to fix highlight on controller contact
                Valve.VR.InteractionSystem.Throwable ts = cursor.AddComponent<Valve.VR.InteractionSystem.Throwable>();
                ts.releaseVelocityStyle = Valve.VR.InteractionSystem.ReleaseStyle.NoChange;
                cursor.GetComponent<Rigidbody>().useGravity = false;
                cursor.GetComponent<Rigidbody>().isKinematic = true;
                
                // Ensure the object and all child objects are on the Interactables layer (#6)
                cursor.layer = interactablesLayer;
                foreach (Transform t in cursor.GetComponentsInChildren<Transform>(true))
                {   
                    t.gameObject.layer = interactablesLayer;
                }
                break;

            case "cube":

                // Cube reference object placement
                cursor = cursorCube;
                cursor.transform.position = spawnPoint;
                cursor.transform.rotation = Quaternion.identity; // reset to upright
                break;

            case "point":

                // Pointing: Use the current controller position as endpoint
                cursor = cursorSphere;
                break;

            case "laser":
                    
                // Laser pointer: turn on the laser pointer object and record position of its target dot
                laserPointer.SetVisible(true);
                cursor = laserPointer.laserDot;
                break;

            case "stick":
                    
                // Laser "stick": like the laser dot, but attached to the controller at a fixed distance
                laserStick.SetVisible(true);
                cursor = laserStick.laserDot;
                break;

        }
        cursor.SetActive(true);

        // Placement tasks: wait until grip was used at least once to avoid "default position" responses
        if ((response_mode == "object") || (response_mode == "cube")) {
            while(true) {
                if (grip.GetStateDown(SteamVR_Input_Sources.RightHand)) {
                    break;
                }
                yield return null;
            }
        }

        // Wait until trigger press to confirm end of trial
        while(true) {
            if (trigger.GetStateDown(SteamVR_Input_Sources.RightHand)) {
                trial.result["t_response_end"] = Time.time;
                break;
            }
            yield return null;
        }
        
        // Save endpoint of current response method
        Vector3 endPos = cursor.transform.position;
        Quaternion endRot = cursor.transform.rotation;
        trial.result["response_pos_x_pivot"] = endPos.x;
        trial.result["response_pos_y_pivot"] = endPos.y;
        trial.result["response_pos_z_pivot"] = endPos.z;
        trial.result["response_euler_x"] = endRot.eulerAngles.x;
        trial.result["response_euler_y"] = endRot.eulerAngles.y;
        trial.result["response_euler_z"] = endRot.eulerAngles.z;

        // Compute and save target error
        if (target != null) {
            
            // Translation
            trial.result["tar_error_pos"] = Vector3.Distance(tarPos, endPos);
            trial.result["tar_error_pos_x"] = endPos.x - tarPos.x;
            trial.result["tar_error_pos_y"] = endPos.y - tarPos.y;
            trial.result["tar_error_pos_z"] = endPos.z - tarPos.z;

            // Translation (bounding box based, only when placement is used)
            if ((response_mode == "object") || (response_mode == "cube")) {
                Bounds endBounds = GetBounds(cursor);
                trial.result["response_bbox_center_x"] = endBounds.center.x;
                trial.result["response_bbox_center_y"] = endBounds.center.y;
                trial.result["response_bbox_center_z"] = endBounds.center.z;
                trial.result["tar_error_bbox"] = Vector3.Distance(tarBounds.center, endBounds.center);
                trial.result["tar_error_bbox_x"] = endBounds.center.x - tarBounds.center.x;
                trial.result["tar_error_bbox_y"] = endBounds.center.y - tarBounds.center.y;
                trial.result["tar_error_bbox_z"] = endBounds.center.z - tarBounds.center.z;
            }

            // Rotation
            trial.result["tar_error_angle"] = Quaternion.Angle(tarRot, endRot);
            trial.result["tar_error_euler_x"] = endRot.eulerAngles.x - tarRot.eulerAngles.x;
            trial.result["tar_error_euler_y"] = endRot.eulerAngles.y - tarRot.eulerAngles.y;
            trial.result["tar_error_euler_z"] = endRot.eulerAngles.z - tarRot.eulerAngles.z;

        }

        // Log participant's standing position
        Vector3 ppPos = Camera.main.transform.position;
        trial.result["standing_pos_x"] = ppPos.x;
        trial.result["standing_pos_y"] = ppPos.y;
        trial.result["standing_pos_z"] = ppPos.z;

        // Fade to black
        SteamVR_Fade.Start(fadeColor, fadeDuration);
        if (soundOnTrialEnd) confirmSound.Play(0);
        yield return new WaitForSeconds(fadeDuration);

        // Reset all object positions and visibility
        if (target != null) target.SetActive(true);
        cursor.SetActive(false);
        if (response_mode == "object") Destroy(cursor);
        if (response_mode == "laser") laserPointer.SetVisible(false);
        if (response_mode == "stick") laserStick.SetVisible(false);
        if (hasShiftedObjects) {
            foreach (KeyValuePair<string, Vector3> o in previousPositions) {
                GameObject.Find(o.Key).transform.position = o.Value;
            }
        }

        // Fade back into instruction scene
        instructionScene.SetActive(true);
        feetIndicator.SetActive(false);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("BaseScene"));
        foreach (GameObject so in sceneObjects) so.SetActive(false);
        SteamVR_Fade.Start(Color.clear, fadeDuration);
        yield return new WaitForSeconds(fadeDuration);

        trial.End();

        yield return null;

    }


    public void TrialEnd(Trial trial) {
        Debug.Log("Trial ended.");
        StartCoroutine("TrialEndCoroutine", trial);
    }


    public IEnumerator TrialEndCoroutine(Trial trial) {
        
        // Continue with next trial unless this was the last trial
        if (trial.number < Session.instance.blocks[0].trials.Count) {
            yield return new WaitForSeconds(durationITI);
            Session.instance.BeginNextTrial();
        } else {
            Session.instance.End();
        }

        yield return null; 

    }


    public IEnumerator SelectScene(string newScene) {

        if (!availableScenes.Contains(newScene)) {

            Debug.LogError("SelectScene: Unknown scene name was specified in trial file!");

        } else {

            var loadStatus = SceneManager.LoadSceneAsync(newScene, LoadSceneMode.Additive);
            if (availableScenes.Contains(currentScene)) SceneManager.UnloadSceneAsync(currentScene);
            while (!loadStatus.isDone){
                yield return null;
            }
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(newScene));
            currentScene = newScene;
            Debug.Log("Done loading scene: " + newScene);

        }
        yield return null;
    }


    public void SessionEnd(Session session) {
        StartCoroutine("SessionEndCoroutine", session);
    }


    public IEnumerator SessionEndCoroutine(Session session) {
        
        instructionText.text = "End of session! Thank you<br>for your participation!";
        instructionScene.SetActive(true);
        yield return new WaitForSeconds(3.0f);
    
    }

    public Bounds GetBounds(GameObject go) {
        // Get Bounds of a GameObject or collection of GameObjects

        Bounds b = new Bounds();

        if (go.transform.childCount > 0) {

            // Grouped game object, collect bounds from subobjects
            MeshRenderer[] renderers = go.GetComponentsInChildren<MeshRenderer>();
            if (renderers.Length > 0)
            {
                b = renderers[0].bounds;
                for (int i = 1, ni = renderers.Length; i < ni; i++)
                {
                    b.Encapsulate(renderers[i].bounds);
                }
            }

        } else {
            // Single game object, collect its bounds directly
            b = go.GetComponent<MeshRenderer>().bounds;
            //Debug.Log(string.Format("X: {0}, Y: {1}, Z: {2}", b.size.x, b.size.y, b.size.z));
        }
        return b;
    }

    public void SetUpArrangement(string arrangement) {

        Debug.Log(string.Format("Setting up arrangement: {0}", arrangement));
        foreach (var row in arrangementTable.GetAsListOfDict()) {

            if ((string)row["arrangement"] == arrangement) {

                try {
                    GameObject go = GameObject.Find((string)row["object"]);
                    if (Convert.ToInt32(row["visible"]) == 0) {
                        
                        // Hack: move the object far outside of the room instead of disabling,
                        // this way it can still be found using GameObject.Find()
                        go.transform.position = new Vector3(100f, 100f, 100f);

                    } else {
                        go.transform.position = new Vector3(Convert.ToSingle(row["pos_x"]),
                                                            Convert.ToSingle(row["pos_y"]),
                                                            Convert.ToSingle(row["pos_z"]));
                        
                        // Orientation is disabled, because it is hard to specify the exact orientation 
                        // from an external file without knowing about Unity's scene graph.

                        //go.transform.eulerAngles = new Vector3(Convert.ToSingle(row["euler_x"]),
                        //                                       Convert.ToSingle(row["euler_y"]),
                        //                                       Convert.ToSingle(row["euler_z"]));

                    }
                    //Debug.Log(string.Format("Arrangement {0}: Updating '{1}'!", arrangement, row["object"]));

                } catch (Exception) {
                    Debug.Log(string.Format("Arrangement {0}: Could not update object '{1}'!", arrangement, row["object"]));
                }
            }

        }
    }

    private IEnumerator WaitForStartPosition(Trial trial, SteamVR_Action_Boolean trigger, bool soundOnViewMisalignment) {

        while(true) {
            if (trigger.GetStateDown(SteamVR_Input_Sources.Any)) {

                // Check if participant is close to origin and facing correctly
                float originDistance = Vector2.Distance(new Vector2(feetIndicator.transform.position.x, 
                                                                    feetIndicator.transform.position.z), 
                                                        new Vector2(Camera.main.transform.position.x, 
                                                                    Camera.main.transform.position.z));
                
                float angleDistance = Mathf.Abs(Mathf.DeltaAngle(startRotationY, Camera.main.transform.eulerAngles.y));

                if ((originDistance >= maxDistanceFromOrigin) || (angleDistance >= maxViewDirectionError)) {
                    if (soundOnViewMisalignment) errorSound.Play(0);
                    yield return new WaitForSeconds(0.6f);
                    continue;
                } else {
                    break;
                }
            }
            yield return null;
        }

    }

    // Returns a list of all scenes found in the build
    private List<string> FindAvailableScenes() {

        int sceneCount = SceneManager.sceneCountInBuildSettings;
        List<string> scenes = new List<string>();

        for (int s = 0; s < sceneCount; s++) {

            string[] scenePath = SceneUtility.GetScenePathByBuildIndex(s).Split('/');
            string sceneFile = scenePath[scenePath.Length -1]; // Split off asset folders and .unity extension
            string scene = sceneFile.Substring(0, sceneFile.Length - 6);
            
            if (scene != "BaseScene") {
                scenes.Add(scene);
            }
        }

        Debug.Log(string.Format("* Found {0} scenes in build: {1}", scenes.Count, String.Join(", ", scenes)));
        return scenes;

    }

}