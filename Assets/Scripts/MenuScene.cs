// Script that is attached to the menu scene

using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuScene : BaseUIScene
{ 

    private void Start()
    {
        LoadSocietyFileIfNotExists();

        LoadSettings();

        SetFade();
        SetFadeInSpeed(0.33f);
        UpdateColoursIfColourBlindMode();

        if (currentSettings.GetLanguage() != "English")
            TranslateScene();
    }

    private void LoadSocietyFileIfNotExists()
    {
        if (!System.IO.File.Exists(Application.persistentDataPath + "/data/sockey.json"))
            System.IO.File.WriteAllText(Application.persistentDataPath + "/data/sockey.json", SockeyFile);
    }

    private void Update()
    {
        UpdateFade();
    }

    // Executes when the user clicks on the 'ask a question' button
    public void OnAskQuestionClick()
    {
        // Switches to AR scene if the user has not yet completed the tutorial
        if (!currentSettings.ReturnFieldValue("completeTutorial"))
            SceneManager.LoadScene("AR"); 
        // Switches to AR scene if the user has ticked to automatically use AR in the settings page
        else if (currentSettings.ReturnFieldValue("autoUseAR"))
            SceneManager.LoadScene("AR");
        // Else switches to avatar scene if the user has ticked to automatically use avatar in the settings page
        else if (currentSettings.ReturnFieldValue("autoUseAvatar"))
            SceneManager.LoadScene("Avatar");
        // Otherwise, switch to the chatbot scene 
        else
            SceneManager.LoadScene("Chatbot");
    }

    // Loads info scene
    public void OnInfoClick()
    {
        SceneManager.LoadScene("Info");
    }

    // Loads mapping scene
    public void OnMappingClick()
    {
        SceneManager.LoadScene("Mapping");
    }


    string SockeyFile = @"{
        'keyword': 'A Cappella',
        'name': 'A Cappella',
        'link': 'https://engage.luu.org.uk/groups/8H7/a-cappella'
    },
    {
        'keyword': 'Abuse Awareness',
        'name': 'Abuse Awareness',
        'link': 'https://engage.luu.org.uk/groups/33C/abuse-awareness-society'
    },
    {
        'keyword': 'Adventist',
        'name': 'Adventist Students on Campus',
        'link': 'https://engage.luu.org.uk/groups/VYT/adventist-students-on-campus'
    },
    {
        'keyword': 'African',
        'name': 'African Caribbean',
        'link': 'https://engage.luu.org.uk/groups/RV8/african-caribbean'
    },
    {
        'keyword': 'Ahlul Bayt',
        'name': 'Ahlul Bayt',
        'link': 'https://engage.luu.org.uk/groups/PWD/ahlul-bayt'
    },
    {
        'keyword': 'Aikido',
        'name': 'Aikido',
        'link': 'https://engage.luu.org.uk/groups/243/aikido'
    },
    {
        'keyword': 'AIMES',
        'name': 'AIMES',
        'link': 'https://engage.luu.org.uk/groups/V64/aimes'
    },
    {
        'keyword': 'American Football',
        'name': 'American Football',
        'link': 'https://engage.luu.org.uk/groups/B6C/american-football'
    },
    {
        'keyword': 'Amnesty International',
        'name': 'Amnesty International',
        'link': 'https://engage.luu.org.uk/groups/3BB/amnesty-international'
    },
    {
        'keyword': 'Advertising',
        'name': 'AMP (Advertising, Marketing, PR)',
        'link': 'https://engage.luu.org.uk/groups/G6P/amp-advertising-marketing-pr'
    },
    {
        'keyword': 'Animal Crossing',
        'name': 'Animal Crossing',
        'link': 'https://engage.luu.org.uk/groups/G9M/animal-crossing'
    },
    {
        'keyword': 'Anime',
        'name': 'Anime & Manga',
        'link': 'https://engage.luu.org.uk/groups/MC6/anime-manga'
    },
    {
        'keyword': 'Archery',
        'name': 'Archery',
        'link': 'https://engage.luu.org.uk/groups/P73/archery'
    },
    {
        'keyword': 'Architecture',
        'name': 'Architecture',
        'link': 'https://engage.luu.org.uk/groups/KMG/architecture'
    },
    {
        'keyword': 'Art',
        'name': 'Art',
        'link': 'https://engage.luu.org.uk/groups/MTP/art'
    },
    {
        'keyword': 'Asia Pacific',
        'name': 'Asia Pacific Studies',
        'link': 'https://engage.luu.org.uk/groups/T3Y/asia-pacific-studies'
    },
    {
        'keyword': 'Football',
        'name': 'Association Football (m)',
        'link': 'https://engage.luu.org.uk/groups/Q3C/association-football-m'
    },
    {
        'keyword': 'Astro',
        'name': 'AstroSoc',
        'link': 'https://engage.luu.org.uk/groups/7WT/astrosoc'
    },
    {
        'keyword': 'Athletics',
        'name': 'Athletics',
        'link': 'https://engage.luu.org.uk/groups/WRV/athletics'
    },
    {
        'keyword': 'Aviation',
        'name': 'Aviation',
        'link': 'https://engage.luu.org.uk/groups/FQ8/aviation'
    },
    {
        'keyword': 'Backstage',
        'name': 'Backstage',
        'link': 'https://engage.luu.org.uk/groups/2GC/backstage'
    },
    {
        'keyword': 'Badminton',
        'name': 'Badminton',
        'link': 'https://engage.luu.org.uk/groups/429/badminton'
    },
    {
        'keyword': 'Baking',
        'name': 'Baking',
        'link': 'https://engage.luu.org.uk/groups/6DK/baking-society'
    },
    {
        'keyword': 'Ballet',
        'name': 'Ballet',
        'link': 'https://engage.luu.org.uk/groups/D4F/ballet'
    },
    {
        'keyword': 'Band',
        'name': 'Band Soc',
        'link': 'https://engage.luu.org.uk/groups/VVH/band-soc'
    },
    {
        'keyword': 'Barbell',
        'name': 'Barbell Club',
        'link': 'https://engage.luu.org.uk/groups/J6V/barbell-club'
    },
    {
        'keyword': 'Barrister',
        'name': 'Barrister (BarSoc)',
        'link': 'https://engage.luu.org.uk/groups/4GX/barrister-barsoc'
    },
    {
        'keyword': 'Basketball',
        'name': 'Basketball (m)',
        'link': 'https://engage.luu.org.uk/groups/PCJ/basketball-m'
    },
    {
        'keyword': 'Basketball',
        'name': 'Basketball (w)',
        'link': 'https://engage.luu.org.uk/groups/BT6/basketball-w'
    },
    {
        'keyword': 'Eating',
        'name': 'BEAT',
        'link': 'https://engage.luu.org.uk/groups/RWY/beat'
    },
    {
        'keyword': 'Bedside',
        'name': 'Bedside Buddies',
        'link': 'https://engage.luu.org.uk/groups/QWC/bedside-buddies'
    },
    {
        'keyword': 'Belly Dance',
        'name': 'Belly Dance',
        'link': 'https://engage.luu.org.uk/groups/VKD/belly-dance-society'
    },
    {
        'keyword': 'Bengali',
        'name': 'Bengali',
        'link': 'https://engage.luu.org.uk/groups/7TP/bengali'
    },
    {
        'keyword': 'Bhangra Dance',
        'name': 'Bhangra Dance',
        'link': 'https://engage.luu.org.uk/groups/7FR/bhangra-dance-society'
    },
    {
        'keyword': 'Big Band',
        'name': 'Big Band',
        'link': 'https://engage.luu.org.uk/groups/QFW/big-band'
    },
    {
        'keyword': 'Party',
        'name': 'Big Open Party',
        'link': 'https://engage.luu.org.uk/groups/P42/big-open-party'
    },
    {
        'keyword': 'Biology',
        'name': 'BioSoc',
        'link': 'https://engage.luu.org.uk/groups/447/biosoc'
    },
    {
        'keyword': 'Black Feminist',
        'name': 'Black Feminist',
        'link': 'https://engage.luu.org.uk/groups/6X6/black-feminist-society'
    },
    {
        'keyword': 'Book Club',
        'name': 'Book Club',
        'link': 'https://engage.luu.org.uk/groups/PX8/book-club'
    },
    {
        'keyword': 'Boxing',
        'name': 'Boxing',
        'link': 'https://engage.luu.org.uk/groups/TPG/boxing'
    },
    {
        'keyword': 'Music',
        'name': 'BPM - Electronic Music and DJ',
        'link': 'https://engage.luu.org.uk/groups/BGR/bpm-electronic-music-and-dj-society'
    },
    {
        'keyword': 'Jiu Jitsu',
        'name': 'Brazilian Jiu Jitsu',
        'link': 'https://engage.luu.org.uk/groups/BYP/brazilian-jiu-jitsu'
    },
    {
        'keyword': 'Chinese',
        'name': 'British Chinese',
        'link': 'https://engage.luu.org.uk/groups/PM6/british-chinese'
    },
    {
        'keyword': 'Red Cross',
        'name': 'British Red Cross',
        'link': 'https://engage.luu.org.uk/groups/9G7/british-red-cross'
    },
    {
        'keyword': 'Buddhist',
        'name': 'Buddhist Meditation',
        'link': 'https://engage.luu.org.uk/groups/HY6/buddhist-meditation'
    },
    {
        'keyword': 'Bulgarian',
        'name': 'Bulgarian',
        'link': 'https://engage.luu.org.uk/groups/GXJ/bulgarian'
    },
    {
        'keyword': 'Business',
        'name': 'Business School',
        'link': 'https://engage.luu.org.uk/groups/33H/business-school-society-lubs'
    },
    {
        'keyword': 'Canoe',
        'name': 'Canoe Club',
        'link': 'https://engage.luu.org.uk/groups/CKQ/canoe-club'
    },
    {
        'keyword': 'Card',
        'name': 'Card Games',
        'link': 'https://engage.luu.org.uk/groups/9X3/card-games'
    },
    {
        'keyword': 'Kickabout',
        'name': 'Casual Kickabout',
        'link': 'https://engage.luu.org.uk/groups/HXD/casual-kickabout'
    },
    {
        'keyword': 'Catholic',
        'name': 'Catholic',
        'link': 'https://engage.luu.org.uk/groups/MJ8/catholic'
    },
    {
        'keyword': 'Caving',
        'name': 'Caving',
        'link': 'https://engage.luu.org.uk/groups/FW2/caving'
    },
    {
        'keyword': 'Change Ringers',
        'name': 'Change Ringers',
        'link': 'https://engage.luu.org.uk/groups/KYC/change-ringers'
    },
    {
        'keyword': 'Cheerleading',
        'name': 'Cheerleading',
        'link': 'https://engage.luu.org.uk/groups/FW3/cheerleading'
    },
    {
        'keyword': 'Chemical Engineering',
        'name': 'Chemical Engineering',
        'link': 'https://engage.luu.org.uk/groups/JCR/chemical-engineering'
    },
    {
        'keyword': 'Chemistry',
        'name': 'Chemistry',
        'link': 'https://engage.luu.org.uk/groups/TQB/chemistry'
    },
    {
        'keyword': 'Chess',
        'name': 'Chess',
        'link': 'https://engage.luu.org.uk/groups/KTH/chess'
    },
    {
        'keyword': 'Chinese',
        'name': 'Chinese',
        'link': 'https://engage.luu.org.uk/groups/QTT/chinese'
    },
    {
        'keyword': 'Christian',
        'name': 'Christian Union',
        'link': 'https://engage.luu.org.uk/groups/VDB/christian-union'
    },
    {
        'keyword': 'Civil Engineering',
        'name': 'CivSoc (Civil Engineering)',
        'link': 'https://engage.luu.org.uk/groups/F7Q/civsoc-civil-engineering'
    },
    {
        'keyword': 'Classics',
        'name': 'Classics',
        'link': 'https://engage.luu.org.uk/groups/HXJ/classics'
    },
    {
        'keyword': 'Coffee',
        'name': 'Coffee',
        'link': 'https://engage.luu.org.uk/groups/3GG/coffee'
    },
    {
        'keyword': 'Comedy',
        'name': 'Comedy',
        'link': 'https://engage.luu.org.uk/groups/CXR/comedy-society'
    },
    {
        'keyword': 'Comics',
        'name': 'Comics & Graphic Novels',
        'link': 'https://engage.luu.org.uk/groups/VT6/comics-graphic-novels'
    },
    {
        'keyword': 'Community',
        'name': 'Community First Responders',
        'link': 'https://engage.luu.org.uk/groups/KPC/community-first-responders'
    },
    {
        'keyword': 'Commuters',
        'name': 'Commuters',
        'link': 'https://engage.luu.org.uk/groups/Q93/commuters'
    },
    {
        'keyword': 'Computing',
        'name': 'CompSoc',
        'link': 'https://engage.luu.org.uk/groups/PQ3/compsoc'
    },
    {
        'keyword': 'Conservation',
        'name': 'Conservation Volunteers',
        'link': 'https://engage.luu.org.uk/groups/8GW/conservation-volunteers'
    },
    {
        'keyword': 'Conservatives',
        'name': 'Conservatives',
        'link': 'https://engage.luu.org.uk/groups/MPP/conservatives'
    },
    {
        'keyword': 'Consulting',
        'name': 'Consulting',
        'link': 'https://engage.luu.org.uk/groups/MRG/consulting'
    },
    {
        'keyword': 'Cricket',
        'name': 'Cricket (m)',
        'link': 'https://engage.luu.org.uk/groups/PHF/cricket-m'
    },
    {
        'keyword': 'Cricket',
        'name': 'Cricket (w)',
        'link': 'https://engage.luu.org.uk/groups/TMK/cricket-w'
    },
    {
        'keyword': 'Criminal',
        'name': 'CrimSoc (Criminal Justice)',
        'link': 'https://engage.luu.org.uk/groups/KMY/crimsoc-criminal-justice'
    },
    {
        'keyword': 'Cross Country',
        'name': 'Cross Country',
        'link': 'https://engage.luu.org.uk/groups/3P8/cross-country'
    },
    {
        'keyword': 'Cryptocurrency',
        'name': 'Cryptocurrency & Blockchain',
        'link': 'https://engage.luu.org.uk/groups/MXQ/cryptocurrency-blockchain'
    },
    {
        'keyword': 'Cutting Edge',
        'name': 'Cutting Edge',
        'link': 'https://engage.luu.org.uk/groups/PGM/cutting-edge'
    },
    {
        'keyword': 'Cycling',
        'name': 'Cycling',
        'link': 'https://engage.luu.org.uk/groups/CXF/cycling'
    },
    {
        'keyword': 'Dance',
        'name': 'Dance Expose',
        'link': 'https://engage.luu.org.uk/groups/B89/dance-expose'
    },
    {
        'keyword': 'Ballroom',
        'name': 'Dancesport (Ballroom and Latin)',
        'link': 'https://engage.luu.org.uk/groups/TQT/dancesport-ballroom-and-latin'
    },
    {
        'keyword': 'Darts',
        'name': 'Darts',
        'link': 'https://engage.luu.org.uk/groups/M6T/darts'
    },
    {
        'keyword': 'Debating',
        'name': 'Debating',
        'link': 'https://engage.luu.org.uk/groups/FJ4/debating'
    },
    {
        'keyword': 'Dentists',
        'name': 'DentSoc',
        'link': 'https://engage.luu.org.uk/groups/FR2/dentsoc'
    },
    {
        'keyword': 'Dice',
        'name': 'DiceSoc',
        'link': 'https://engage.luu.org.uk/groups/HC6/dicesoc'
    },
    {
        'keyword': 'Dodgeball',
        'name': 'Dodgeball',
        'link': 'https://engage.luu.org.uk/groups/CC4/dodgeball'
    },
    {
        'keyword': 'Earth',
        'name': 'Earth Sciences (RocSoc)',
        'link': 'https://engage.luu.org.uk/groups/63Q/earth-sciences-rocsoc'
    },
    {
        'keyword': 'African',
        'name': 'East African',
        'link': 'https://engage.luu.org.uk/groups/H8P/east-african'
    },
    {
        'keyword': 'East Asian',
        'name': 'East Asian Research (EARS)',
        'link': 'https://engage.luu.org.uk/groups/Y2R/east-asian-research-ears'
    },
    {
        'keyword': 'Economics',
        'name': 'Economics',
        'link': 'https://engage.luu.org.uk/groups/42Q/economics-society'
    },
    {
        'keyword': 'Enactus Leeds',
        'name': 'Enactus Leeds',
        'link': 'https://engage.luu.org.uk/groups/BV4/enactus-leeds'
    },
    {
        'keyword': 'English',
        'name': 'English',
        'link': 'https://engage.luu.org.uk/groups/H6J/english'
    },
    {
        'keyword': 'Entrepreneurs',
        'name': 'Entrepreneurs',
        'link': 'https://engage.luu.org.uk/groups/7CV/entrepreneurs'
    },
    {
        'keyword': 'Environment',
        'name': 'Environmental (EnviroSoc)',
        'link': 'https://engage.luu.org.uk/groups/HH6/environmental-envirosoc'
    },
    {
        'keyword': 'International',
        'name': 'Erasmus and International Exchange',
        'link': 'https://engage.luu.org.uk/groups/W3X/erasmus-and-international-exchange'
    },
    {
        'keyword': 'video games',
        'name': 'eSports and Video Games',
        'link': 'https://engage.luu.org.uk/groups/H37/esports-and-video-games'
    },
    {
        'keyword': 'Extended Reality',
        'name': 'Extended Reality XR',
        'link': 'https://engage.luu.org.uk/groups/QCD/extended-reality-xr'
    },
    {
        'keyword': 'Biology',
        'name': 'Faculty of Biological Sciences (FoBSoc)',
        'link': 'https://engage.luu.org.uk/groups/7HK/faculty-of-biological-sciences-fobsoc'
    },
    {
        'keyword': 'Fashion',
        'name': 'Fashion',
        'link': 'https://engage.luu.org.uk/groups/R3W/fashion'
    },
    {
        'keyword': 'Feminist',
        'name': 'Feminist',
        'link': 'https://engage.luu.org.uk/groups/T9K/feminist'
    },
    {
        'keyword': 'Fencing',
        'name': 'Fencing',
        'link': 'https://engage.luu.org.uk/groups/PMK/fencing'
    },
    {
        'keyword': 'Filipino',
        'name': 'Filipino',
        'link': 'https://engage.luu.org.uk/groups/G9K/filipino'
    },
    {
        'keyword': 'Film',
        'name': 'Film',
        'link': 'https://engage.luu.org.uk/groups/948/film'
    },
    {
        'keyword': 'Film Making',
        'name': 'Film Making',
        'link': 'https://engage.luu.org.uk/groups/VF4/film-making'
    },
    {
        'keyword': 'Folk',
        'name': 'Folk and Thackray Sage Morris',
        'link': 'https://engage.luu.org.uk/groups/9PX/folk-and-thackray-sage-morris'
    },
    {
        'keyword': 'Food',
        'name': 'Food Science and Nutrition',
        'link': 'https://engage.luu.org.uk/groups/34K/food-science-and-nutrition'
    },
    {
        'keyword': 'Food',
        'name': 'Food Soc',
        'link': 'https://engage.luu.org.uk/groups/FKJ/food-soc'
    },
    {
        'keyword': 'Dance',
        'name': 'Freestyle Dance',
        'link': 'https://engage.luu.org.uk/groups/PG6/freestyle-dance'
    },
    {
        'keyword': 'French',
        'name': 'French',
        'link': 'https://engage.luu.org.uk/groups/DVD/french'
    },
    {
        'keyword': 'Friends',
        'name': 'Friends of MSF',
        'link': 'https://engage.luu.org.uk/groups/6BK/friends-of-msf'
    },
    {
        'keyword': 'Futsal',
        'name': 'Futsal',
        'link': 'https://engage.luu.org.uk/groups/K68/futsal'
    },
    {
        'keyword': 'Geography',
        'name': 'Geography',
        'link': 'https://engage.luu.org.uk/groups/4YF/geography'
    },
    {
        'keyword': 'German',
        'name': 'German',
        'link': 'https://engage.luu.org.uk/groups/B72/german'
    },
    {
        'keyword': 'Golf',
        'name': 'Golf',
        'link': 'https://engage.luu.org.uk/groups/HT4/golf'
    },
    {
        'keyword': 'Greek',
        'name': 'Greek and Cypriot',
        'link': 'https://engage.luu.org.uk/groups/84R/greek-and-cypriot'
    },
    {
        'keyword': 'Green',
        'name': 'Green Action',
        'link': 'https://engage.luu.org.uk/groups/K4T/green-action'
    },
    {
        'keyword': 'Guide Dogs',
        'name': 'Guide Dogs',
        'link': 'https://engage.luu.org.uk/groups/HJ3/guide-dogs'
    },
    {
        'keyword': 'Gymnastics',
        'name': 'Gymnastics',
        'link': 'https://engage.luu.org.uk/groups/6H6/gymnastics'
    },
    {
        'keyword': 'Handball',
        'name': 'Handball',
        'link': 'https://engage.luu.org.uk/groups/M6V/handball'
    },
    {
        'keyword': 'Health',
        'name': 'Healthcare Innovation Team Leeds',
        'link': 'https://engage.luu.org.uk/groups/MVQ/healthcare-innovation-team-leeds'
    },
    {
        'keyword': 'Health',
        'name': 'Healthcare Students for Climate Action - HESCA',
        'link': 'https://engage.luu.org.uk/groups/7XG/healthcare-students-for-climate-action-hesca'
    },
    {
        'keyword': 'Feminist',
        'name': 'Her Campus',
        'link': 'https://engage.luu.org.uk/groups/WC9/her-campus'
    },
    {
        'keyword': 'High',
        'name': 'High on Life',
        'link': 'https://engage.luu.org.uk/groups/YFG/high-on-life'
    },
    {
        'keyword': 'Hiking',
        'name': 'Hiking',
        'link': 'https://engage.luu.org.uk/groups/GYC/hiking'
    },
    {
        'keyword': 'Historia Normannis',
        'name': 'Historia Normannis',
        'link': 'https://engage.luu.org.uk/groups/9MK/historia-normannis'
    },
    {
        'keyword': 'History',
        'name': 'History',
        'link': 'https://engage.luu.org.uk/groups/P9C/history'
    },
    {
        'keyword': 'History',
        'name': 'History and Philosophy of Science',
        'link': 'https://engage.luu.org.uk/groups/HC4/history-and-philosophy-of-science'
    },
    {
        'keyword': 'Hockey',
        'name': 'Hockey (m)',
        'link': 'https://engage.luu.org.uk/groups/X6W/hockey-m'
    },
    {
        'keyword': 'Hockey',
        'name': 'Hockey (w)',
        'link': 'https://engage.luu.org.uk/groups/FF8/hockey-w'
    },
    {
        'keyword': 'Homed',
        'name': 'Homed',
        'link': 'https://engage.luu.org.uk/groups/X2K/homed'
    },
    {
        'keyword': 'Hong Kong',
        'name': 'Hong Kong',
        'link': 'https://engage.luu.org.uk/groups/T7W/hong-kong'
    },
    {
        'keyword': 'Hong Kong',
        'name': 'Hong Kong Public Affairs & Social Services',
        'link': 'https://engage.luu.org.uk/groups/JPT/hong-kong-public-affairs-social-services-society'
    },
    {
        'keyword': 'Horse',
        'name': 'Horse Riding',
        'link': 'https://engage.luu.org.uk/groups/X4J/horse-riding'
    },
    {
        'keyword': 'Hung Kuen',
        'name': 'Hung Kuen',
        'link': 'https://engage.luu.org.uk/groups/68R/hung-kuen'
    },
    {
        'keyword': 'Hungarian',
        'name': 'Hungarian',
        'link': 'https://engage.luu.org.uk/groups/X4R/hungarian'
    },
    {
        'keyword': 'Ice Hockey',
        'name': 'Ice Hockey',
        'link': 'https://engage.luu.org.uk/groups/63J/ice-hockey'
    },
    {
        'keyword': 'Christian',
        'name': 'Inclusive Christian Movement',
        'link': 'https://engage.luu.org.uk/groups/GF7/inclusive-christian-movement'
    },
    {
        'keyword': 'Indian',
        'name': 'Indian Student Association',
        'link': 'https://engage.luu.org.uk/groups/Y32/indian-student-association'
    },
    {
        'keyword': 'Business',
        'name': 'International Business',
        'link': 'https://engage.luu.org.uk/groups/7DG/international-business-society'
    },
    {
        'keyword': 'Iranian',
        'name': 'Iranian',
        'link': 'https://engage.luu.org.uk/groups/6HD/iranian'
    },
    {
        'keyword': 'Irish',
        'name': 'Irish Dancing',
        'link': 'https://engage.luu.org.uk/groups/29V/irish-dancing'
    },
    {
        'keyword': 'Islamic',
        'name': 'Islamic',
        'link': 'https://engage.luu.org.uk/groups/KCT/islamic'
    },
    {
        'keyword': 'Italian',
        'name': 'Italian',
        'link': 'https://engage.luu.org.uk/groups/872/italian-society'
    },
    {
        'keyword': 'Japan',
        'name': 'Japanese',
        'link': 'https://engage.luu.org.uk/groups/WYQ/japanese'
    },
    {
        'keyword': 'Jazz',
        'name': 'Jazz & Blues',
        'link': 'https://engage.luu.org.uk/groups/CRX/jazz-blues'
    },
    {
        'keyword': 'Jewish',
        'name': 'Jewish',
        'link': 'https://engage.luu.org.uk/groups/293/jewish'
    },
    {
        'keyword': 'Jiu Jitsu',
        'name': 'Jiu Jitsu',
        'link': 'https://engage.luu.org.uk/groups/FXT/jiu-jitsu'
    },
    {
        'keyword': 'Judo',
        'name': 'Judo',
        'link': 'https://engage.luu.org.uk/groups/V9Q/judo'
    },
    {
        'keyword': 'Karate',
        'name': 'Karate',
        'link': 'https://engage.luu.org.uk/groups/PTC/karate'
    },
    {
        'keyword': 'Kickboxing',
        'name': 'Kickboxing + Krav Maga',
        'link': 'https://engage.luu.org.uk/groups/6FM/kickboxing-krav-maga'
    },
    {
        'keyword': 'Korean',
        'name': 'Korean',
        'link': 'https://engage.luu.org.uk/groups/HWR/korean'
    },
    {
        'keyword': 'Korfball',
        'name': 'Korfball',
        'link': 'https://engage.luu.org.uk/groups/YDY/korfball'
    },
    {
        'keyword': 'Dance',
        'name': 'KPOP Dance',
        'link': 'https://engage.luu.org.uk/groups/3QW/kpop-dance'
    },
    {
        'keyword': 'Kurdish',
        'name': 'Kurdish',
        'link': 'https://engage.luu.org.uk/groups/DH9/kurdish'
    },
    {
        'keyword': 'Labour',
        'name': 'Labour',
        'link': 'https://engage.luu.org.uk/groups/3GJ/labour'
    },
    {
        'keyword': 'Lacrosse',
        'name': 'Lacrosse (m)',
        'link': 'https://engage.luu.org.uk/groups/G8V/lacrosse-m'
    },
    {
        'keyword': 'Lacrosse',
        'name': 'Lacrosse (w)',
        'link': 'https://engage.luu.org.uk/groups/HG6/lacrosse-w'
    },
    {
        'keyword': 'Music',
        'name': 'LAMMPS',
        'link': 'https://engage.luu.org.uk/groups/W4H/lammps'
    },
    {
        'keyword': 'Law',
        'name': 'Law',
        'link': 'https://engage.luu.org.uk/groups/9VV/law'
    },
    {
        'keyword': 'Marrow',
        'name': 'Leeds Marrow',
        'link': 'https://engage.luu.org.uk/groups/6T7/leeds-marrow'
    },
    {
        'keyword': 'Media',
        'name': 'Leeds Media Students',
        'link': 'https://engage.luu.org.uk/groups/GGJ/leeds-media-students'
    },
    {
        'keyword': 'Surgery',
        'name': 'Leeds Oral and Maxillofacial Surgery (LOMFS)',
        'link': 'https://engage.luu.org.uk/groups/Q7B/leeds-oral-and-maxillofacial-surgery-lomfs'
    },
    {
        'keyword': 'Prison',
        'name': 'Leeds Prison Reform',
        'link': 'https://engage.luu.org.uk/groups/3W9/leeds-prison-reform'
    },
    {
        'keyword': 'Fundraising',
        'name': 'Leeds RAG',
        'link': 'https://engage.luu.org.uk/groups/VBD/leeds-rag'
    },
    {
        'keyword': 'Choir',
        'name': 'Leeds Rev Open Choir',
        'link': 'https://engage.luu.org.uk/groups/V7C/leeds-rev-open-choir'
    },
    {
        'keyword': 'Radio',
        'name': 'Leeds Student Radio',
        'link': 'https://engage.luu.org.uk/groups/VFG/leeds-student-radio'
    },
    {
        'keyword': 'Boob',
        'name': 'Leeds Uni Boob Team',
        'link': 'https://engage.luu.org.uk/groups/2TD/leeds-uni-boob-team'
    },
    {
        'keyword': 'United Nations',
        'name': 'Leeds United Nations',
        'link': 'https://engage.luu.org.uk/groups/6FC/leeds-united-nations'
    },
    {
        'keyword': 'LGBT',
        'name': 'LGBT',
        'link': 'https://engage.luu.org.uk/groups/CBM/lgbt'
    },
    {
        'keyword': 'Liberal Arts',
        'name': 'Liberal Arts',
        'link': 'https://engage.luu.org.uk/groups/YJM/liberal-arts'
    },
    {
        'keyword': 'Linguistics',
        'name': 'Linguistics',
        'link': 'https://engage.luu.org.uk/groups/73G/linguistics'
    },
    {
        'keyword': 'Lippy',
        'name': 'Lippy',
        'link': 'https://engage.luu.org.uk/groups/J9Q/lippy'
    },
    {
        'keyword': 'Tai Chi',
        'name': 'Lishi Tai Chi',
        'link': 'https://engage.luu.org.uk/groups/T4P/lishi-tai-chi'
    },
    {
        'keyword': 'TV',
        'name': 'LSTV',
        'link': 'https://engage.luu.org.uk/groups/FWF/lstv'
    },
    {
        'keyword': 'Community',
        'name': 'LUUMIC (Music Impact in the Community)',
        'link': 'https://engage.luu.org.uk/groups/B3Q/luumic-music-impact-in-the-community'
    },
    {
        'keyword': 'Music',
        'name': 'LUUMS (Music)',
        'link': 'https://engage.luu.org.uk/groups/WYY/luums-music'
    },
    {
        'keyword': 'Smile',
        'name': 'Make A Smile',
        'link': 'https://engage.luu.org.uk/groups/HYD/make-a-smile'
    },
    {
        'keyword': 'Malaysian',
        'name': 'Malaysian',
        'link': 'https://engage.luu.org.uk/groups/86W/malaysian'
    },
    {
        'keyword': 'Singaporean',
        'name': 'Malaysian & Singaporean',
        'link': 'https://engage.luu.org.uk/groups/C6Q/malaysian-singaporean'
    },
    {
        'keyword': 'Mantality',
        'name': 'Mantality',
        'link': 'https://engage.luu.org.uk/groups/BMF/mantality'
    },
    {
        'keyword': 'Marxist',
        'name': 'Marxist',
        'link': 'https://engage.luu.org.uk/groups/VJ4/marxist'
    },
    {
        'keyword': 'Mathematical',
        'name': 'Mathematical',
        'link': 'https://engage.luu.org.uk/groups/PVG/mathematical'
    },
    {
        'keyword': 'Mechanical Engineering',
        'name': 'Mechanical Engineering (Mech Eng Soc)',
        'link': 'https://engage.luu.org.uk/groups/3RP/mechanical-engineering-mech-eng-soc'
    },
    {
        'keyword': 'Medieval',
        'name': 'Medieval',
        'link': 'https://engage.luu.org.uk/groups/J7M/medieval-society'
    },
    {
        'keyword': 'Medicine',
        'name': 'MedSoc (Medicine)',
        'link': 'https://engage.luu.org.uk/groups/QJX/medsoc-medicine'
    },
    {
        'keyword': 'Mexican',
        'name': 'Mexican',
        'link': 'https://engage.luu.org.uk/groups/84C/mexican'
    },
    {
        'keyword': 'Microbiology',
        'name': 'Microbiology',
        'link': 'https://engage.luu.org.uk/groups/8BV/microbiology'
    },
    {
        'keyword': 'Midwife',
        'name': 'Midwifery',
        'link': 'https://engage.luu.org.uk/groups/3WF/midwifery'
    },
    {
        'keyword': 'Mind',
        'name': 'Mind Matters',
        'link': 'https://engage.luu.org.uk/groups/29C/mind-matters'
    },
    {
        'keyword': 'Dance',
        'name': 'Modern Dance',
        'link': 'https://engage.luu.org.uk/groups/JJG/modern-dance'
    },
    {
        'keyword': 'Motorsport',
        'name': 'Motorsport',
        'link': 'https://engage.luu.org.uk/groups/92V/motorsport-society'
    },
    {
        'keyword': 'Climbing',
        'name': 'Mountaineering (Climbing)',
        'link': 'https://engage.luu.org.uk/groups/4FB/mountaineering-climbing'
    },
    {
        'keyword': 'Medics',
        'name': 'MSRC',
        'link': 'https://engage.luu.org.uk/groups/CKB/msrc'
    },
    {
        'keyword': 'Muay Thai',
        'name': 'Muay Thai',
        'link': 'https://engage.luu.org.uk/groups/2HD/muay-thai'
    },
    {
        'keyword': 'Theatre',
        'name': 'Music Theatre',
        'link': 'https://engage.luu.org.uk/groups/WJH/music-theatre'
    },
    {
        'keyword': 'Myanmar',
        'name': 'Myanmar Association',
        'link': 'https://engage.luu.org.uk/groups/K73/myanmar-association'
    },
    {
        'keyword': 'Hindu',
        'name': 'National Hindu Students Forum',
        'link': 'https://engage.luu.org.uk/groups/8KY/national-hindu-students-forum'
    },
    {
        'keyword': 'Natural Sciences',
        'name': 'Natural Sciences',
        'link': 'https://engage.luu.org.uk/groups/XJT/natural-sciences'
    },
    {
        'keyword': 'Nepalese',
        'name': 'Nepalese',
        'link': 'https://engage.luu.org.uk/groups/YQH/nepalese'
    },
    {
        'keyword': 'Netball',
        'name': 'Netball',
        'link': 'https://engage.luu.org.uk/groups/X6D/netball'
    },
    {
        'keyword': 'Neural Networks',
        'name': 'Neural Networks',
        'link': 'https://engage.luu.org.uk/groups/D84/neural-networks'
    },
    {
        'keyword': 'Neurodivergent',
        'name': 'Neurodivergent',
        'link': 'https://engage.luu.org.uk/groups/G3X/neurodivergent'
    },
    {
        'keyword': 'Nigerian',
        'name': 'Nigerian',
        'link': 'https://engage.luu.org.uk/groups/7CH/nigerian'
    },
    {
        'keyword': 'Nightline',
        'name': 'Nightline',
        'link': 'https://engage.luu.org.uk/groups/V62/nightline'
    },
    {
        'keyword': 'Nintendo',
        'name': 'Nintendo and Pokemon',
        'link': 'https://engage.luu.org.uk/groups/T9G/nintendo-and-pokemon'
    },
    {
        'keyword': 'Nursing',
        'name': 'Nursing',
        'link': 'https://engage.luu.org.uk/groups/QRX/nursing'
    },
    {
        'keyword': 'Omani',
        'name': 'Omani',
        'link': 'https://engage.luu.org.uk/groups/WQR/omani'
    },
    {
        'keyword': 'Beat',
        'name': 'On Beat',
        'link': 'https://engage.luu.org.uk/groups/2MP/on-beat'
    },
    {
        'keyword': 'Theatre',
        'name': 'Open Theatre',
        'link': 'https://engage.luu.org.uk/groups/PCD/open-theatre'
    },
    {
        'keyword': 'Medicine',
        'name': 'Oral Medicine & Surgery',
        'link': 'https://engage.luu.org.uk/groups/8KK/oral-medicine-surgery'
    },
    {
        'keyword': 'Orienteering',
        'name': 'Orienteering + Fell Running',
        'link': 'https://engage.luu.org.uk/groups/HQ9/orienteering-fell-running'
    },
    {
        'keyword': 'Christian',
        'name': 'Orthodox Christian',
        'link': 'https://engage.luu.org.uk/groups/HQ2/orthodox-christian'
    },
    {
        'keyword': 'Oxfam',
        'name': 'Oxfam',
        'link': 'https://engage.luu.org.uk/groups/JRV/oxfam'
    },
    {
        'keyword': 'Paintballing',
        'name': 'Paintballing',
        'link': 'https://engage.luu.org.uk/groups/D6J/paintballing'
    },
    {
        'keyword': 'Pakistani',
        'name': 'Pakistani',
        'link': 'https://engage.luu.org.uk/groups/WQF/pakistani'
    },
    {
        'keyword': 'Palestine',
        'name': 'Palestine Solidarity Group',
        'link': 'https://engage.luu.org.uk/groups/72D/palestine-solidarity-group'
    },
    {
        'keyword': 'African',
        'name': 'Pan African',
        'link': 'https://engage.luu.org.uk/groups/Q7R/pan-african'
    },
    {
        'keyword': 'Pantomime',
        'name': 'Pantomime',
        'link': 'https://engage.luu.org.uk/groups/94D/pantomime'
    },
    {
        'keyword': 'Planet',
        'name': 'People & Planet',
        'link': 'https://engage.luu.org.uk/groups/26P/people-planet'
    },
    {
        'keyword': 'Performance',
        'name': 'Performance and Cultural Industries (PCI)',
        'link': 'https://engage.luu.org.uk/groups/H7G/performance-and-cultural-industries-pci'
    },
    {
        'keyword': 'Performing Arts',
        'name': 'Performing Arts',
        'link': 'https://engage.luu.org.uk/groups/GJ4/performing-arts'
    },
    {
        'keyword': 'Philosophy',
        'name': 'Philosophy (Leeds University Philosophical)',
        'link': 'https://engage.luu.org.uk/groups/89M/philosophy-leeds-university-philosophical-society'
    },
    {
        'keyword': 'Physics',
        'name': 'Physics',
        'link': 'https://engage.luu.org.uk/groups/V47/physics'
    },
    {
        'keyword': 'Polish',
        'name': 'Polish',
        'link': 'https://engage.luu.org.uk/groups/PXY/polish'
    },
    {
        'keyword': 'Politics',
        'name': 'Politics and International Studies (POLIS)',
        'link': 'https://engage.luu.org.uk/groups/HR7/politics-and-international-studies-polis'
    },
    {
        'keyword': 'Polo',
        'name': 'Polo',
        'link': 'https://engage.luu.org.uk/groups/2KC/polo'
    },
    {
        'keyword': 'Pool',
        'name': 'Pool and Snooker',
        'link': 'https://engage.luu.org.uk/groups/4KH/pool-and-snooker'
    },
    {
        'keyword': 'Politics',
        'name': 'PPE (Politics, Philosophy, and Economics)',
        'link': 'https://engage.luu.org.uk/groups/F84/ppe-politics-philosophy-and-economics'
    },
    {
        'keyword': 'Psychiatry',
        'name': 'Psyched (Psychiatry)',
        'link': 'https://engage.luu.org.uk/groups/2YD/psyched-psychiatry'
    },
    {
        'keyword': 'Psychology',
        'name': 'Psychology',
        'link': 'https://engage.luu.org.uk/groups/RXB/psychology'
    },
    {
        'keyword': 'LGBT',
        'name': 'QTIPOC',
        'link': 'https://engage.luu.org.uk/groups/XVT/qtipoc'
    },
    {
        'keyword': 'Quidditch',
        'name': 'Quidditch',
        'link': 'https://engage.luu.org.uk/groups/YDH/quidditch'
    },
    {
        'keyword': 'Youth',
        'name': 'Radical Youth Leeds',
        'link': 'https://engage.luu.org.uk/groups/GR6/radical-youth-leeds'
    },
    {
        'keyword': 'Real Ale',
        'name': 'Real Ale',
        'link': 'https://engage.luu.org.uk/groups/TBX/real-ale'
    },
    {
        'keyword': 'Rifle',
        'name': 'Rifle',
        'link': 'https://engage.luu.org.uk/groups/YWF/rifle'
    },
    {
        'keyword': 'Rounders',
        'name': 'Rounders',
        'link': 'https://engage.luu.org.uk/groups/BRK/rounders'
    },
    {
        'keyword': 'Rowing',
        'name': 'Rowing',
        'link': 'https://engage.luu.org.uk/groups/KVM/rowing'
    },
    {
        'keyword': 'Rugby',
        'name': 'Rugby League (m)',
        'link': 'https://engage.luu.org.uk/groups/83P/rugby-league-m'
    },
    {
        'keyword': 'Rugby',
        'name': 'Rugby League (w)',
        'link': 'https://engage.luu.org.uk/groups/QXJ/rugby-league-w'
    },
    {
        'keyword': 'Rugby',
        'name': 'Rugby Union (m)',
        'link': 'https://engage.luu.org.uk/groups/6K4/rugby-union-m'
    },
    {
        'keyword': 'Rugby',
        'name': 'Rugby Union (w)',
        'link': 'https://engage.luu.org.uk/groups/8V6/rugby-union-w'
    },
    {
        'keyword': 'Russian',
        'name': 'Russian and Slavonic',
        'link': 'https://engage.luu.org.uk/groups/4X9/russian-and-slavonic'
    },
    {
        'keyword': 'Sailing',
        'name': 'Sailing Club',
        'link': 'https://engage.luu.org.uk/groups/VYX/sailing-club'
    },
    {
        'keyword': 'Salsa',
        'name': 'Salsa',
        'link': 'https://engage.luu.org.uk/groups/WQY/salsa'
    },
    {
        'keyword': 'Saudi',
        'name': 'Saudi',
        'link': 'https://engage.luu.org.uk/groups/C6X/saudi'
    },
    {
        'keyword': 'Sci Fi',
        'name': 'Sci Fi and Fantasy',
        'link': 'https://engage.luu.org.uk/groups/6M6/sci-fi-and-fantasy'
    },
    {
        'keyword': 'Scout',
        'name': 'Scout & Guide',
        'link': 'https://engage.luu.org.uk/groups/87Q/scout-guide'
    },
    {
        'keyword': 'Sex',
        'name': 'Sexpression',
        'link': 'https://engage.luu.org.uk/groups/JPR/sexpression'
    },
    {
        'keyword': 'Electrical',
        'name': 'ShockSoc (Electrical Engineering)',
        'link': 'https://engage.luu.org.uk/groups/QJ3/shocksoc-electrical-engineering'
    },
    {
        'keyword': 'Sikh',
        'name': 'Sikh',
        'link': 'https://engage.luu.org.uk/groups/68K/sikh'
    },
    {
        'keyword': 'Singaporean',
        'name': 'Singaporean',
        'link': 'https://engage.luu.org.uk/groups/DXB/singaporean'
    },
    {
        'keyword': 'Skate',
        'name': 'Skate',
        'link': 'https://engage.luu.org.uk/groups/P3P/skate'
    },
    {
        'keyword': 'Skydiving',
        'name': 'Skydiving',
        'link': 'https://engage.luu.org.uk/groups/2QJ/skydiving'
    },
    {
        'keyword': 'Snowriders',
        'name': 'Snowriders',
        'link': 'https://engage.luu.org.uk/groups/WG8/snowriders'
    },
    {
        'keyword': 'Geologist',
        'name': 'Economic Geologists',
        'link': 'https://engage.luu.org.uk/groups/VBG/society-of-economic-geologists'
    },
    {
        'keyword': 'Social',
        'name': 'Sociology & Social Policy',
        'link': 'https://engage.luu.org.uk/groups/YGW/sociology-social-policy'
    },
    {
        'keyword': 'Design',
        'name': 'SODSoc (School of Design)',
        'link': 'https://engage.luu.org.uk/groups/3GH/sodsoc-school-of-design'
    },
    {
        'keyword': 'Spanish',
        'name': 'Spanish, Latin American and Portuguese Society (SLAPSOC)',
        'link': 'https://engage.luu.org.uk/groups/BK4/spanish-latin-american-and-portuguese-society-slapsoc'
    },
    {
        'keyword': 'Word',
        'name': 'Spoken Word',
        'link': 'https://engage.luu.org.uk/groups/QXD/spoken-word'
    },
    {
        'keyword': 'Sports Science',
        'name': 'Sports and Exercise Science (SPSC)',
        'link': 'https://engage.luu.org.uk/groups/FMP/sports-and-exercise-science-spsc'
    },
    {
        'keyword': 'Squash',
        'name': 'Squash and Racketball',
        'link': 'https://engage.luu.org.uk/groups/R46/squash-and-racketball'
    },
    {
        'keyword': 'Suicide Awareness',
        'name': 'SSAFE (Suicide Support and Awareness for Everyone)',
        'link': 'https://engage.luu.org.uk/groups/8FJ/ssafe-suicide-support-and-awareness-for-everyone'
    },
    {
        'keyword': 'Ambulance',
        'name': 'St. John Ambulance',
        'link': 'https://engage.luu.org.uk/groups/9H4/st-john-ambulance'
    },
    {
        'keyword': 'Stage Musicals',
        'name': 'Stage Musicals',
        'link': 'https://engage.luu.org.uk/groups/FHK/stage-musicals-society'
    },
    {
        'keyword': 'Crafting',
        'name': 'Stitch & Bitch',
        'link': 'https://engage.luu.org.uk/groups/6TG/stitch-bitch'
    },
    {
        'keyword': 'Dance',
        'name': 'Street Dance',
        'link': 'https://engage.luu.org.uk/groups/Q92/street-dance'
    },
    {
        'keyword': 'Refugees',
        'name': 'Student Action for Refugees',
        'link': 'https://engage.luu.org.uk/groups/KB7/student-action-for-refugees'
    },
    {
        'keyword': 'Health',
        'name': 'Students for Global Health Leeds',
        'link': 'https://engage.luu.org.uk/groups/9QC/students-for-global-health-leeds'
    },
    {
        'keyword': 'Drugs',
        'name': 'Students for Sensible Drug Policy',
        'link': 'https://engage.luu.org.uk/groups/TJP/students-for-sensible-drug-policy'
    },
    {
        'keyword': 'Scuba Diving',
        'name': 'Sub Aqua (Scuba Diving)',
        'link': 'https://engage.luu.org.uk/groups/DHT/sub-aqua-scuba-diving'
    },
    {
        'keyword': 'Surf',
        'name': 'Surf',
        'link': 'https://engage.luu.org.uk/groups/W8M/surf'
    },
    {
        'keyword': 'Swimming',
        'name': 'Swimming & Water Polo',
        'link': 'https://engage.luu.org.uk/groups/H6R/swimming-water-polo'
    },
    {
        'keyword': 'Swing',
        'name': 'Swing',
        'link': 'https://engage.luu.org.uk/groups/BCK/swing'
    },
    {
        'keyword': 'Table Tennis',
        'name': 'Table Tennis',
        'link': 'https://engage.luu.org.uk/groups/RTH/table-tennis'
    },
    {
        'keyword': 'Tae-Kwon-Do',
        'name': 'Tae-Kwon-Do',
        'link': 'https://engage.luu.org.uk/groups/4PX/tae-kwon-do'
    },
    {
        'keyword': 'Taiwanese',
        'name': 'Taiwanese',
        'link': 'https://engage.luu.org.uk/groups/G3R/taiwanese'
    },
    {
        'keyword': 'Hospital',
        'name': 'Teddy Bear Hospital',
        'link': 'https://engage.luu.org.uk/groups/Q2F/teddy-bear-hospital'
    },
    {
        'keyword': 'Tennis',
        'name': 'Tennis (m & w)',
        'link': 'https://engage.luu.org.uk/groups/6KM/tennis-m-w'
    },
    {
        'keyword': 'Arrhythmics',
        'name': 'The Arrhythmics',
        'link': 'https://engage.luu.org.uk/groups/JFG/the-arrhythmics'
    },
    {
        'keyword': 'Gryphon',
        'name': 'The Gryphon',
        'link': 'https://engage.luu.org.uk/groups/3R9/the-gryphon'
    },
    {
        'keyword': 'Scribe',
        'name': 'The Scribe',
        'link': 'https://engage.luu.org.uk/groups/MC8/the-scribe'
    },
    {
        'keyword': 'Social Mobility',
        'name': 'The Social Mobility (SoMoSoc)',
        'link': 'https://engage.luu.org.uk/groups/FKC/the-social-mobility-society-somosoc'
    },
    {
        'keyword': 'Theatre Group',
        'name': 'Theatre Group',
        'link': 'https://engage.luu.org.uk/groups/TFD/theatre-group'
    },
    {
        'keyword': 'Touch Rugby',
        'name': 'Touch Rugby (Mixed)',
        'link': 'https://engage.luu.org.uk/groups/43K/touch-rugby-mixed'
    },
    {
        'keyword': 'Trading',
        'name': 'Trading and Investments',
        'link': 'https://engage.luu.org.uk/groups/Y4Y/trading-and-investments'
    },
    {
        'keyword': 'Trampoline',
        'name': 'Trampoline',
        'link': 'https://engage.luu.org.uk/groups/FH7/trampoline'
    },
    {
        'keyword': 'Triathlon',
        'name': 'Triathlon',
        'link': 'https://engage.luu.org.uk/groups/YFD/triathlon'
    },
    {
        'keyword': 'Religion',
        'name': 'TRS (Theology and Religion)',
        'link': 'https://engage.luu.org.uk/groups/WDH/trs-society-theology-and-religion'
    },
    {
        'keyword': 'UAE',
        'name': 'UAE',
        'link': 'https://engage.luu.org.uk/groups/THQ/uae'
    },
    {
        'keyword': 'Frisbee',
        'name': 'Ultimate Frisbee',
        'link': 'https://engage.luu.org.uk/groups/H4F/ultimate-frisbee'
    },
    {
        'keyword': 'Music',
        'name': 'Union Music Library',
        'link': 'https://engage.luu.org.uk/groups/3MV/union-music-library'
    },
    {
        'keyword': 'Vegetarian',
        'name': 'Vegetarian and Vegan',
        'link': 'https://engage.luu.org.uk/groups/TMP/vegetarian-and-vegan'
    },
    {
        'keyword': 'Vertical Fitness',
        'name': 'Vertical Fitness',
        'link': 'https://engage.luu.org.uk/groups/8GY/vertical-fitness'
    },
    {
        'keyword': 'Bollywood',
        'name': 'Vibes: Bollywood Dance',
        'link': 'https://engage.luu.org.uk/groups/KKP/vibes-bollywood-dance'
    },
    {
        'keyword': 'Volleyball',
        'name': 'Volleyball',
        'link': 'https://engage.luu.org.uk/groups/MJX/volleyball'
    },
    {
        'keyword': 'Welsh',
        'name': 'Welsh',
        'link': 'https://engage.luu.org.uk/groups/3XR/welsh'
    },
    {
        'keyword': 'Medicine',
        'name': 'Wilderness Medicine',
        'link': 'https://engage.luu.org.uk/groups/9DC/wilderness-medicine'
    },
    {
        'keyword': 'Wine',
        'name': 'Wine',
        'link': 'https://engage.luu.org.uk/groups/FQ3/wine'
    },
    {
        'keyword': 'Engineering',
        'name': 'Women in Engineering',
        'link': 'https://engage.luu.org.uk/groups/38Q/women-in-engineering'
    },
    {
        'keyword': 'Leadership',
        'name': 'Women In Leadership',
        'link': 'https://engage.luu.org.uk/groups/TBR/women-in-leadership'
    },
    {
        'keyword': 'Yoga',
        'name': 'Yoga',
        'link': 'https://engage.luu.org.uk/groups/X9V/yoga-society'
    },
    {
        'keyword': 'Activities',
        'name': 'Student Activities',
        'link': 'https://engage.luu.org.uk/groups/JR933/student-activities'
    },
    {
        'keyword': 'Trips',
        'name': 'Trips Programme',
        'link': 'https://engage.luu.org.uk/groups/7JVW8/trips-programme'
    },
    {
        'keyword': 'Hall Exec',
        'name': 'Hall Exec Programme',
        'link': 'https://engage.luu.org.uk/groups/FQT9Q/hall-exec-programme'
    },
    {
        'keyword': 'Liberal',
        'name': 'Young Liberals',
        'link': 'https://engage.luu.org.uk/groups/GKXWD/young-liberals'
    },
    {
        'keyword': 'Justice',
        'name': 'Leeds Global Justice Now',
        'link': 'https://engage.luu.org.uk/groups/DVTXK/leeds-global-justice-now'
    },
    {
        'keyword': 'Blenheim Point',
        'name': 'Blenheim Point',
        'link': 'https://engage.luu.org.uk/groups/9F8WG/blenheim-point'
    },
    {
        'keyword': 'Burley Road',
        'name': 'Burley Road',
        'link': 'https://engage.luu.org.uk/groups/YJMDK/burley-road'
    },
    {
        'keyword': 'Central Village',
        'name': 'Central Village',
        'link': 'https://engage.luu.org.uk/groups/4HWBD/central-village'
    },
    {
        'keyword': 'Charles Morris',
        'name': 'Charles Morris',
        'link': 'https://engage.luu.org.uk/groups/DYCFB/charles-morris'
    },
    {
        'keyword': 'Cityside',
        'name': 'Cityside',
        'link': 'https://engage.luu.org.uk/groups/432QD/cityside'
    },
    {
        'keyword': 'Clarence Dock Village',
        'name': 'Clarence Dock Village',
        'link': 'https://engage.luu.org.uk/groups/D4JG2/clarence-dock-village'
    },
    {
        'keyword': 'Devonshire Hall',
        'name': 'Devonshire Hall',
        'link': 'https://engage.luu.org.uk/groups/YDH42/devonshire-hall'
    },
    {
        'keyword': 'Ellerslie Hall',
        'name': 'Ellerslie Hall',
        'link': 'https://engage.luu.org.uk/groups/TXJTM/ellerslie-hall'
    },
    {
        'keyword': 'Grayson Heights',
        'name': 'Grayson Heights',
        'link': 'https://engage.luu.org.uk/groups/XM44B/grayson-heights'
    },
    {
        'keyword': 'Henry Price Residence',
        'name': 'Henry Price Residence',
        'link': 'https://engage.luu.org.uk/groups/2QVTW/henry-price-residence'
    },
    {
        'keyword': 'Hepworth Lodge',
        'name': 'Hepworth Lodge',
        'link': 'https://engage.luu.org.uk/groups/38PMJ/hepworth-lodge'
    },
    {
        'keyword': 'iQ',
        'name': 'iQ Leeds',
        'link': 'https://engage.luu.org.uk/groups/XJVC8/iq-leeds'
    },
    {
        'keyword': 'James Baillie',
        'name': 'James Baillie Park',
        'link': 'https://engage.luu.org.uk/groups/JDXKJ/james-baillie-park'
    },
    {
        'keyword': 'Leodis Residences',
        'name': 'Leodis Residences',
        'link': 'https://engage.luu.org.uk/groups/YDYQR/leodis-residences'
    },
    {
        'keyword': 'Lupton Residences',
        'name': 'Lupton Residences',
        'link': 'https://engage.luu.org.uk/groups/2RPXX/lupton-residences'
    },
    {
        'keyword': 'Lyddon Hall',
        'name': 'Lyddon Hall',
        'link': 'https://engage.luu.org.uk/groups/9BJ8X/lyddon-hall'
    },
    {
        'keyword': 'Mary Morris House',
        'name': 'Mary Morris House',
        'link': 'https://engage.luu.org.uk/groups/48G3R/mary-morris-house'
    },
    {
        'keyword': 'Montague Burton',
        'name': 'Montague Burton',
        'link': 'https://engage.luu.org.uk/groups/DRGRP/montague-burton'
    },
    {
        'keyword': 'North Hill Court',
        'name': 'North Hill Court',
        'link': 'https://engage.luu.org.uk/groups/F4Q4W/north-hill-court'
    },
    {
        'keyword': 'Oak House',
        'name': 'Oak House',
        'link': 'https://engage.luu.org.uk/groups/8G2Q7/oak-house'
    },
    {
        'keyword': 'Oxley Residences',
        'name': 'Oxley Residences',
        'link': 'https://engage.luu.org.uk/groups/YYVPP/oxley-residences'
    },
    {
        'keyword': 'Royal Park Flats',
        'name': 'Royal Park Flats',
        'link': 'https://engage.luu.org.uk/groups/F43X7/royal-park-flats'
    },
    {
        'keyword': 'Sentinel Towers',
        'name': 'Sentinel Towers',
        'link': 'https://engage.luu.org.uk/groups/PMMDJ/sentinel-towers'
    },
    {
        'keyword': 'St. Marks Residences',
        'name': 'St. Marks Residences',
        'link': 'https://engage.luu.org.uk/groups/Q6WW6/st-marks-residences'
    },
    {
        'keyword': 'Shared Houses',
        'name': 'Shared Houses',
        'link': 'https://engage.luu.org.uk/groups/M3T7R/shared-houses'
    },
    {
        'keyword': 'The Tannery',
        'name': 'The Tannery',
        'link': 'https://engage.luu.org.uk/groups/22H2H/the-tannery'
    },
    {
        'keyword': 'White Rose View',
        'name': 'White Rose View',
        'link': 'https://engage.luu.org.uk/groups/Y4KQR/white-rose-view'
    },
    {
        'keyword': 'Nordic',
        'name': 'Nordic',
        'link': 'https://engage.luu.org.uk/groups/993GV/nordic-society'
    },
    {
        'keyword': 'Musculoskeletal',
        'name': 'Leeds Rheumatology and Musculoskeletal Medicine (LRMMS)',
        'link': 'https://engage.luu.org.uk/groups/4FK8X/leeds-rheumatology-and-musculoskeletal-medicine-society-lrmms'
    },
    {
        'keyword': 'Nutritank',
        'name': 'Leeds Nutritank (MSRC)',
        'link': 'https://engage.luu.org.uk/groups/DMM69/leeds-nutritank-msrc'
    },
    {
        'keyword': 'Surgery',
        'name': 'Plastics Reconstructive and Aesthetics Surgical Society',
        'link': 'https://engage.luu.org.uk/groups/6FFQW/plastics-reconstructive-and-aesthetics-surgical-society'
    },
    {
        'keyword': 'Entertainment',
        'name': 'Leeds ENT',
        'link': 'https://engage.luu.org.uk/groups/QB4B7/leeds-ent-society'
    },
    {
        'keyword': 'Horror',
        'name': 'Leeds Horror',
        'link': 'https://engage.luu.org.uk/groups/TKG97/leeds-horror-society'
    },
    {
        'keyword': 'Girl',
        'name': 'Girls Training Together',
        'link': 'https://engage.luu.org.uk/groups/RCTDB/girls-training-together'
    },
    {
        'keyword': 'United Island',
        'name': 'United Island',
        'link': 'https://engage.luu.org.uk/groups/W7Y8F/united-island'
    },
    {
        'keyword': 'Climate',
        'name': 'Climate Entrepreneurs Club',
        'link': 'https://engage.luu.org.uk/groups/JYBV6/climate-entrepreneurs-club'
    },
    {
        'keyword': 'Sexual Harassment',
        'name': 'Students Against Sexual Harassment & Assault (SASHA)',
        'link': 'https://engage.luu.org.uk/groups/W24FV/students-against-sexual-harassment-assault-sasha'
    },
    {
        'keyword': 'Rock',
        'name': 'Leeds Rock and Alternate',
        'link': 'https://engage.luu.org.uk/groups/9DWR3/leeds-rock-and-alternate-society'
    },
    {
        'keyword': 'Arab',
        'name': 'Arab',
        'link': 'https://engage.luu.org.uk/groups/JF3V3/arab-society'
    },
    {
        'keyword': 'Thai',
        'name': 'Thai',
        'link': 'https://engage.luu.org.uk/groups/6GXT4/thai-society'
    },
    {
        'keyword': 'X-Posure',
        'name': 'X-Posure (MSRC)',
        'link': 'https://engage.luu.org.uk/groups/TKT2K/x-posure-msrc'
    },
    {
        'keyword': 'Haematology',
        'name': 'HaemSoc',
        'link': 'https://engage.luu.org.uk/groups/FDK77/haemsoc'
    },
    {
        'keyword': 'Turkic',
        'name': 'Turkic',
        'link': 'https://engage.luu.org.uk/groups/CWXHV/turkic-society'
    },
    {
        'keyword': 'Plant',
        'name': 'Plant',
        'link': 'https://engage.luu.org.uk/groups/WWYKK/plant-society'
    },
    {
        'keyword': 'Gynaecology',
        'name': 'Leeds Obstetrics & Gynaecology (LOGsoc) ',
        'link': 'https://engage.luu.org.uk/groups/HRBHH/leeds-obstetrics-gynaecology-society-logsoc-msrc'
    },
    {
        'keyword': 'Cardio',
        'name': 'CardioSoc (MSRC)',
        'link': 'https://engage.luu.org.uk/groups/B988F/cardiosoc-msrc'
    },
    {
        'keyword': 'Leeds',
        'name': 'Leeds & You Programme',
        'link': 'https://engage.luu.org.uk/groups/R29X7/leeds-you-programme'
    },
    {
        'keyword': 'Paediatrics',
        'name': 'Leeds University Medics Paediatrics (LUMPS)',
        'link': 'https://engage.luu.org.uk/groups/X3YCJ/leeds-university-medics-paediatrics-society-lumps-msrc'
    },
    {
        'keyword': 'Muslim',
        'name': 'Leeds Muslim Medics (MSRC)',
        'link': 'https://engage.luu.org.uk/groups/FMCJF/leeds-muslim-medics-msrc'
    },
    {
        'keyword': 'Digital',
        'name': 'Digital Media (DigiSoc)',
        'link': 'https://engage.luu.org.uk/groups/PPQRT/digital-media-society-digisoc'
    },
    {
        'keyword': 'Laidlaw',
        'name': 'Laidlaw',
        'link': 'https://engage.luu.org.uk/groups/2DGT2/laidlaw-society'
    },
    {
        'keyword': 'Education',
        'name': 'Education',
        'link': 'https://engage.luu.org.uk/groups/G6TV4/education-society'
    },
    {
        'keyword': 'Design',
        'name': 'Product Design (PDES)',
        'link': 'https://engage.luu.org.uk/groups/6FFVQ/product-design-society-pdes'
    },
    {
        'keyword': 'Edge',
        'name': 'The Edge',
        'link': 'https://engage.luu.org.uk/groups/C3DPM/the-edge'
    },
    {
        'keyword': 'Skate',
        'name': 'Roller Skate',
        'link': 'https://engage.luu.org.uk/groups/YVJ4B/roller-skate'
    },
    {
        'keyword': 'Worsley',
        'name': 'The Worsley Times (MSRC)',
        'link': 'https://engage.luu.org.uk/groups/QFC96/the-worsley-times-msrc'
    },
    {
        'keyword': 'Social',
        'name': 'Social Work',
        'link': 'https://engage.luu.org.uk/groups/4GGVX/social-work'
    },
    {
        'keyword': 'Basketball',
        'name': 'Leeds Medics and Dentists Basketball Club',
        'link': 'https://engage.luu.org.uk/groups/H4WTK/leeds-medics-and-dentists-basketball-club'
    },
    {
        'keyword': 'Muslim',
        'name': 'Leeds Muslim Dentists',
        'link': 'https://engage.luu.org.uk/groups/QGC28/leeds-muslim-dentists'
    },
    {
        'keyword': 'Care',
        'name': 'Leeds University Critical Care and Major Trauma',
        'link': 'https://engage.luu.org.uk/groups/TD399/leeds-university-critical-care-and-major-trauma-society'
    },
    {
        'keyword': 'Walk',
        'name': 'The Right to Walk',
        'link': 'https://engage.luu.org.uk/groups/QXCPR/the-right-to-walk'
    },
    {
        'keyword': 'South Asian Feminist',
        'name': 'South Asian Feminist',
        'link': 'https://engage.luu.org.uk/groups/H94VX/south-asian-feminist-society'
    },
    {
        'keyword': 'Netball',
        'name': 'Development Netball',
        'link': 'https://engage.luu.org.uk/groups/3G8MP/development-netball'
    },
    {
        'keyword': 'Indonesian',
        'name': 'Indonesian',
        'link': 'https://engage.luu.org.uk/groups/WYHTG/indonesian-society'
    },
    {
        'keyword': 'Ophthalmology',
        'name': 'Ophthalmology (MSRC)',
        'link': 'https://engage.luu.org.uk/groups/C7326/ophthalmology-society-msrc'
    },
    {
        'keyword': 'Synchronised Swimming',
        'name': 'Synchronised Swimming',
        'link': 'https://engage.luu.org.uk/groups/YHMX6/synchronised-swimming'
    },
    {
        'keyword': 'Photography',
        'name': 'Photography',
        'link': 'https://engage.luu.org.uk/groups/G8TKD/photography'
    },
    {
        'keyword': 'SolidariTee',
        'name': 'SolidariTee Leeds',
        'link': 'https://engage.luu.org.uk/groups/2KF29/solidaritee-leeds'
    },
    {
        'keyword': 'Warhammer',
        'name': 'Warhammer',
        'link': 'https://engage.luu.org.uk/groups/8DW6K/warhammer-society'
    },
    {
        'keyword': 'Lego',
        'name': 'Lego',
        'link': 'https://engage.luu.org.uk/groups/JTV8T/lego-society'
    },
    {
        'keyword': 'Bahrain',
        'name': 'Bahrain',
        'link': 'https://engage.luu.org.uk/groups/J29QJ/bahrain-society'
    },
    {
        'keyword': 'Commercial',
        'name': 'Commercial Awareness',
        'link': 'https://engage.luu.org.uk/groups/XBC4G/commercial-awareness'
    },
    {
        'keyword': 'Cras Domus',
        'name': 'Cras Domus',
        'link': 'https://engage.luu.org.uk/groups/RTT9Y/cras-domus-society'
    },
    {
        'keyword': 'African',
        'name': 'North African',
        'link': 'https://engage.luu.org.uk/groups/QT3WF/north-african-society'
    },
    {
        'keyword': 'Heelz',
        'name': 'Heelz Dance',
        'link': 'https://engage.luu.org.uk/groups/BXRMV/heelz-dance'
    },
    {
        'keyword': 'Korea',
        'name': 'Korean Drama and Film',
        'link': 'https://engage.luu.org.uk/groups/C6Y7G/korean-drama-and-film-society'
    },
    {
        'keyword': 'Rotaract',
        'name': 'Rotaract Club',
        'link': 'https://engage.luu.org.uk/groups/6RB6P/rotaract-club'
    },
    {
        'keyword': 'Tamil',
        'name': 'Tamil',
        'link': 'https://engage.luu.org.uk/groups/TYF9X/tamil-society'
    },
    {
        'keyword': 'Kashmiri',
        'name': 'Kashmiri',
        'link': 'https://engage.luu.org.uk/groups/BVDYP/kashmiri-society'
    },
    {
        'keyword': 'Ignite',
        'name': 'CICC Ignite',
        'link': 'https://engage.luu.org.uk/groups/6YG3T/cicc-ignite'
    },";
}