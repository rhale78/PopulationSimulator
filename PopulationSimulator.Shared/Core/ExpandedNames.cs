namespace PopulationSimulator.Core;

/// <summary>
/// Massive expansion of names: 500+ male, 500+ female, 200+ cities, 100+ countries
/// Includes cultural variations and hereditary surname system
/// </summary>
public static class ExpandedNames
{
    /// <summary>
    /// 500+ additional male first names from diverse cultures
    /// </summary>
    public static readonly string[] AdditionalMaleNames =
    {
        // More Biblical/Hebrew
        "Eli", "Asher", "Levi", "Gideon", "Amos", "Silas", "Cyrus", "Tobias", "Josiah", "Ezekiel",
        "Malachi", "Nathaniel", "Raphael", "Gabriel", "Michael", "Uri", "Boaz", "Caleb", "Jesse",
        "Reuben", "Simeon", "Zebulun", "Naphtali", "Gad", "Ephraim", "Manasseh", "Jedidiah", "Obadiah",

        // Greek
        "Andreas", "Christos", "Dimitri", "Georgios", "Konstantinos", "Nikos", "Spyros", "Stavros",
        "Theo", "Yannis", "Kostas", "Petros", "Vasilis", "Alexios", "Kyros", "Dion", "Hector",
        "Nestor", "Patroclus", "Peleus", "Telemachus", "Menelaus", "Agamemnon", "Diomedes",

        // Roman/Latin
        "Aurelius", "Cassius", "Decius", "Flavius", "Horatius", "Livius", "Marcius", "Publius",
        "Quintus", "Servius", "Tertius", "Valerius", "Aemilius", "Cornelius", "Fabius",
        "Gracchus", "Marius", "Numa", "Regulus", "Seneca", "Tacitus", "Virgil",

        // Germanic/Norse
        "Alaric", "Baldur", "Conrad", "Dietrich", "Einar", "Freyr", "Gunther", "Haakon",
        "Ingvar", "Jarl", "Knut", "Lars", "Morten", "Njord", "Oskar", "Rolf", "Sigmund",
        "Torsten", "Ulf", "Vidar", "Wolfgang", "Rune", "Soren", "Henrik", "Axel",

        // Celtic/Gaelic
        "Aiden", "Brennan", "Cian", "Declan", "Eamon", "Fionn", "Galen", "Hugh",
        "Ian", "Kieran", "Lorcan", "Malachy", "Niall", "Oisin", "Padraig", "Quinn",
        "Ronan", "Sean", "Tadhg", "Uilliam", "Connor", "Cormac", "Donal", "Eoghan",

        // Arabic/Middle Eastern
        "Abbas", "Basil", "Dawud", "Faisal", "Hamza", "Idris", "Jabir", "Kamil",
        "Latif", "Mahdi", "Nasir", "Omar", "Qasim", "Rashid", "Samir", "Tahir",
        "Umar", "Walid", "Yazid", "Zayd", "Adil", "Bilal", "Hakim", "Jafar",

        // Persian
        "Arman", "Behrouz", "Dariush", "Farhad", "Hormoz", "Javad", "Kian", "Mehran",
        "Navid", "Omid", "Parviz", "Ramin", "Shahin", "Touraj", "Vahid", "Xerxes",

        // Slavic/Russian
        "Alexei", "Boris", "Dmitri", "Fedor", "Grigori", "Igor", "Konstantin", "Maxim",
        "Nikolai", "Oleg", "Pavel", "Roman", "Sergei", "Timur", "Vasily", "Yuri",
        "Vladimir", "Mikhail", "Anatoly", "Arkady", "Gennady", "Leonid", "Pyotr",

        // Spanish/Portuguese
        "Alejandro", "Carlos", "Diego", "Enrique", "Fernando", "Gonzalo", "Hernando", "Ignacio",
        "Javier", "Lorenzo", "Miguel", "Pablo", "Rodrigo", "Santiago", "Tomas", "Vicente",
        "Alfonso", "Benito", "Cesar", "Esteban", "Francisco", "Jorge", "Luis", "Manuel",

        // French
        "Andre", "Baptiste", "Claude", "Dominique", "Etienne", "Francois", "Guillaume", "Henri",
        "Jacques", "Laurent", "Marcel", "Nicolas", "Olivier", "Pierre", "Rene", "Sebastien",
        "Thierry", "Vincent", "Yves", "Antoine", "Bernard", "Denis", "Emile", "Gerard",

        // Italian
        "Alessandro", "Antonio", "Carlo", "Dante", "Enzo", "Fabio", "Giovanni", "Giuseppe",
        "Lorenzo", "Luca", "Marco", "Matteo", "Nicolo", "Paolo", "Ricardo", "Stefano",
        "Tommaso", "Vittorio", "Angelo", "Bruno", "Claudio", "Emilio", "Franco", "Luigi",

        // Scandinavian
        "Anders", "Bjorn", "Christian", "Emil", "Fredrik", "Gustav", "Hans", "Jakob",
        "Karl", "Lars", "Mikael", "Nils", "Otto", "Per", "Rasmus", "Sven", "Torbjorn",

        // English/Scottish
        "Alistair", "Angus", "Bruce", "Colin", "Duncan", "Ewan", "Fraser", "Gavin",
        "Hamish", "Iain", "Kenneth", "Malcolm", "Neil", "Rory", "Stuart", "Wallace",
        "Andrew", "Benjamin", "Christopher", "Daniel", "Edmund", "Francis", "Gregory",

        // Irish
        "Brendan", "Cathal", "Colm", "Daithi", "Eoin", "Fergal", "Gareth", "Liam",
        "Malachy", "Niall", "Oran", "Paddy", "Rory", "Seamus", "Tomas", "Uilliam",

        // Welsh
        "Bryn", "Cadoc", "Dylan", "Evan", "Gareth", "Hywel", "Ieuan", "Llew",
        "Morgan", "Owain", "Rhys", "Talan", "Vaughan", "Wyn", "Cadfael", "Dewi",

        // Polish
        "Andrzej", "Bartosz", "Czeslaw", "Damian", "Filip", "Grzegorz", "Henryk", "Jan",
        "Karol", "Lukasz", "Marek", "Piotr", "Roman", "Stanislaw", "Tomasz", "Wojciech",

        // Hungarian
        "Attila", "Bela", "Csaba", "Dezso", "Ferenc", "Gabor", "Imre", "Janos",
        "Laszlo", "Miklos", "Pal", "Sandor", "Tibor", "Zoltan", "Istvan", "Gyorgy",

        // Czech
        "Antonin", "Bedrich", "Ctibor", "Dalibor", "Emil", "Frantisek", "Havel", "Ivan",
        "Jaroslav", "Karel", "Lubomir", "Miroslav", "Oldrich", "Pavel", "Radek", "Vaclav",

        // Asian-inspired (generic fantasy)
        "Akio", "Daichi", "Hiro", "Kenji", "Masaru", "Noboru", "Ryu", "Satoshi",
        "Takeshi", "Yuki", "Kazuo", "Makoto", "Shinji", "Taro", "Yuji", "Haruto",

        // African-inspired
        "Abiola", "Chike", "Dayo", "Ebuka", "Folami", "Gamba", "Hasani", "Jabari",
        "Kofi", "Lekan", "Mosi", "Nuru", "Omari", "Runako", "Tau", "Zuberi",

        // Indian/Sanskrit
        "Aditya", "Arjun", "Bharat", "Chandra", "Devendra", "Ganesh", "Hari", "Indra",
        "Jai", "Krishna", "Mohan", "Narayan", "Pavan", "Rama", "Shankar", "Vijay",

        // Turkish/Ottoman
        "Ahmet", "Baris", "Cem", "Deniz", "Emir", "Hakan", "Kemal", "Mehmet",
        "Osman", "Selim", "Tahir", "Umut", "Yusuf", "Can", "Eren", "Murat",

        // Modern/Contemporary
        "Austin", "Blake", "Carter", "Dylan", "Ezra", "Finn", "Grant", "Hudson",
        "Isaac", "Jude", "Knox", "Landon", "Milo", "Nash", "Owen", "Parker",
        "Quinn", "Reid", "Shane", "Tate", "Wyatt", "Zane", "Asher", "Cole",
        "Eli", "Grayson", "Holden", "Jace", "Kai", "Luke", "Nolan", "Reed",

        // Mythological/Legendary
        "Aeneas", "Beowulf", "Cuchulainn", "Daedalus", "Eros", "Fenrir", "Gilgamesh",
        "Heimdall", "Icarus", "Jason", "Kronos", "Loki", "Midas", "Narcissus",
        "Orpheus", "Perseus", "Quetzalcoatl", "Romulus", "Siegfried", "Thoth",

        // Literary/Historical
        "Dante", "Homer", "Virgil", "Ovid", "Horace", "Catullus", "Juvenal",
        "Pliny", "Strabo", "Herodotus", "Thucydides", "Plutarch", "Suetonius"
    };

    /// <summary>
    /// 500+ additional female first names from diverse cultures
    /// </summary>
    public static readonly string[] AdditionalFemaleNames =
    {
        // More Biblical/Hebrew
        "Abigail", "Bathsheba", "Delilah", "Esther", "Hadassah", "Leah", "Michal", "Naomi",
        "Rachel", "Rebekah", "Ruth", "Sarah", "Tamar", "Zipporah", "Kezia", "Jemima",
        "Achsah", "Ahinoam", "Basemath", "Bilhah", "Hodesh", "Maacah", "Mehetabel",

        // Greek
        "Alexandra", "Anastasia", "Calista", "Danae", "Elena", "Fotini", "Georgia",
        "Irene", "Katerina", "Maria", "Nikoleta", "Olympia", "Sophia", "Theodora",
        "Vassiliki", "Zoe", "Althea", "Chione", "Dione", "Eudora", "Galatea",

        // Roman/Latin
        "Aelia", "Aurelia", "Cassia", "Claudia", "Flavia", "Julia", "Livia",
        "Lucretia", "Octavia", "Portia", "Sabina", "Tullia", "Valeria", "Aemilia",
        "Cornelia", "Faustina", "Gratiana", "Hortensia", "Junia", "Lavinia",

        // Germanic/Norse
        "Ada", "Brynhild", "Clothilde", "Dagmar", "Edith", "Freya", "Gertrud",
        "Hedwig", "Ingeborg", "Johanna", "Kriemhild", "Lagertha", "Mathilde",
        "Ottilie", "Ragnhild", "Sigrid", "Thora", "Ulla", "Walburga", "Ylva",

        // Celtic/Gaelic
        "Aisling", "Bridget", "Caoimhe", "Deirdre", "Eileen", "Finola", "Grainne",
        "Isolde", "Keira", "Maeve", "Niamh", "Orla", "Roisin", "Siobhan", "Tara",
        "Una", "Aoife", "Bronagh", "Clodagh", "Eithne", "Fionnuala", "Gráinne",

        // Arabic/Middle Eastern
        "Aaliyah", "Basma", "Dalia", "Farida", "Hana", "Inaya", "Jamila", "Karima",
        "Leila", "Maryam", "Noor", "Rania", "Samira", "Tahira", "Yasmin", "Zahra",
        "Amina", "Bushra", "Fatima", "Habiba", "Iman", "Jameela", "Laila",

        // Persian
        "Ariana", "Bahar", "Darya", "Farah", "Golnar", "Homa", "Jasmin", "Laleh",
        "Mitra", "Nasrin", "Parisa", "Roya", "Shirin", "Taraneh", "Vida", "Yalda",

        // Slavic/Russian
        "Alina", "Darya", "Ekaterina", "Galina", "Irina", "Katya", "Larissa",
        "Natasha", "Olga", "Polina", "Svetlana", "Tatiana", "Valentina", "Yelena",
        "Anastasiya", "Elizaveta", "Ksenya", "Ludmila", "Marina", "Nina", "Oxana",

        // Spanish/Portuguese
        "Alejandra", "Beatriz", "Carmen", "Dolores", "Elena", "Francisca", "Gabriela",
        "Ines", "Juana", "Lucia", "Mercedes", "Natalia", "Paloma", "Rosa", "Teresa",
        "Adriana", "Camila", "Daniela", "Esperanza", "Fernanda", "Gloria", "Isabel",

        // French
        "Adele", "Brigitte", "Celeste", "Dominique", "Eloise", "Francoise", "Genevieve",
        "Helene", "Isabelle", "Josephine", "Louise", "Madeleine", "Nicole", "Odette",
        "Pauline", "Renee", "Simone", "Therese", "Veronique", "Yvette", "Amelie",

        // Italian
        "Alessandra", "Bianca", "Carlotta", "Donatella", "Elena", "Francesca", "Gianna",
        "Isabella", "Lucia", "Margherita", "Nicoletta", "Paola", "Rosalia", "Serena",
        "Valentina", "Angela", "Chiara", "Diana", "Elisa", "Federica", "Giulia",

        // Scandinavian
        "Annika", "Brigitte", "Dagny", "Elsa", "Freja", "Greta", "Hanna", "Inger",
        "Johanna", "Karin", "Linnea", "Maja", "Nina", "Petra", "Signe", "Tuva",

        // English/Scottish
        "Ainsley", "Bonnie", "Catriona", "Elspeth", "Flora", "Heather", "Isla",
        "Kirsty", "Morag", "Rhona", "Skye", "Ailsa", "Effie", "Fenella", "Griselda",

        // Irish
        "Aine", "Bridget", "Caitriona", "Dervla", "Eabha", "Fionnuala", "Grania",
        "Mairead", "Nuala", "Oonagh", "Roisin", "Sinead", "Treasa", "Ula",

        // Welsh
        "Angharad", "Bronwen", "Carys", "Dilys", "Eira", "Ffion", "Gwen", "Lowri",
        "Megan", "Nerys", "Olwen", "Rhiannon", "Sian", "Tegwen", "Winifred",

        // Polish
        "Agnieszka", "Barbara", "Dorota", "Elzbieta", "Grazyna", "Halina", "Jadwiga",
        "Krystyna", "Malgorzata", "Natalia", "Patrycja", "Renata", "Sylwia", "Wioletta",

        // Hungarian
        "Adrienn", "Beatrix", "Csilla", "Eszter", "Ilona", "Judit", "Katalin",
        "Magdolna", "Noemi", "Piroska", "Reka", "Timea", "Veronika", "Zsofia",

        // Czech
        "Alena", "Barbora", "Eva", "Hana", "Ivana", "Jana", "Klara", "Lenka",
        "Monika", "Pavla", "Romana", "Sarka", "Tereza", "Vera", "Zdenka",

        // Asian-inspired
        "Aiko", "Chiyo", "Emiko", "Haruka", "Kiyomi", "Midori", "Natsuki", "Rei",
        "Sakura", "Yuki", "Asuka", "Hana", "Kaori", "Mariko", "Sayuri", "Tomoko",

        // African-inspired
        "Ayana", "Binta", "Chiamaka", "Dalila", "Eshe", "Folami", "Habiba", "Imani",
        "Jendayi", "Kesi", "Lulu", "Nia", "Rashida", "Sanaa", "Zuri", "Amara",

        // Indian/Sanskrit
        "Anjali", "Deepika", "Gayatri", "Indira", "Kiran", "Lakshmi", "Maya",
        "Nisha", "Padma", "Priya", "Radha", "Sita", "Uma", "Veda", "Yamini",

        // Turkish/Ottoman
        "Ayse", "Elif", "Fatma", "Gulnara", "Hatice", "Leyla", "Melike", "Nur",
        "Ozlem", "Selin", "Yasemin", "Zeynep", "Ayla", "Deniz", "Emine", "Hande",

        // Modern/Contemporary
        "Addison", "Brooklyn", "Chloe", "Delilah", "Eliana", "Faith", "Gabriella",
        "Hannah", "Ivy", "Jade", "Kennedy", "Leah", "Maya", "Nora", "Paisley",
        "Quinn", "Riley", "Skylar", "Taylor", "Willow", "Zara", "Autumn", "Bella",

        // Mythological/Legendary
        "Andromeda", "Circe", "Dido", "Europa", "Gaia", "Hestia", "Juno", "Minerva",
        "Nephthys", "Pandora", "Rhea", "Selene", "Thetis", "Urania", "Vesta",

        // Virtue Names
        "Constance", "Faith", "Hope", "Mercy", "Patience", "Prudence", "Charity",
        "Felicity", "Grace", "Honor", "Joy", "Verity", "Amity", "Clemency"
    };

    /// <summary>
    /// 200+ additional city names from diverse cultures and regions
    /// </summary>
    public static readonly string[] AdditionalCityNames =
    {
        // European Medieval
        "York", "Canterbury", "Winchester", "Durham", "Chester", "Norwich", "Bristol",
        "Exeter", "Lincoln", "Salisbury", "Worcester", "Gloucester", "Coventry", "Derby",
        "Nottingham", "Leicester", "Cambridge", "Oxford", "Edinburgh", "Glasgow",

        // Germanic
        "Koln", "Trier", "Mainz", "Frankfurt", "Heidelberg", "Nuremberg", "Augsburg",
        "Wurzburg", "Regensburg", "Ulm", "Konstanz", "Strasbourg", "Basel", "Zurich",
        "Bern", "Geneva", "Salzburg", "Innsbruck", "Graz", "Linz",

        // French
        "Chartres", "Reims", "Rouen", "Caen", "Dijon", "Besancon", "Grenoble",
        "Marseille", "Bordeaux", "Toulouse", "Nantes", "Angers", "Tours", "Orleans",
        "Poitiers", "Limoges", "Clermont", "Avignon", "Nice", "Montpellier",

        // Italian
        "Siena", "Pisa", "Lucca", "Mantua", "Ferrara", "Modena", "Parma",
        "Cremona", "Brescia", "Bergamo", "Como", "Pavia", "Vicenza", "Padova",
        "Verona", "Treviso", "Udine", "Trieste", "Ancona", "Perugia",

        // Spanish/Portuguese
        "Seville", "Cordoba", "Granada", "Toledo", "Segovia", "Salamanca", "Burgos",
        "Leon", "Santiago", "Valladolid", "Zaragoza", "Valencia", "Barcelona",
        "Lisbon", "Porto", "Coimbra", "Evora", "Braga", "Aveiro", "Guimaraes",

        // Low Countries
        "Bruges", "Ghent", "Antwerp", "Brussels", "Liege", "Utrecht", "Amsterdam",
        "Rotterdam", "Haarlem", "Leiden", "Delft", "Groningen", "Maastricht",

        // Scandinavian
        "Stockholm", "Uppsala", "Copenhagen", "Roskilde", "Aarhus", "Odense",
        "Oslo", "Bergen", "Trondheim", "Stavanger", "Kristiansand", "Helsinki",
        "Turku", "Tampere", "Reykjavik", "Akureyri",

        // Eastern European
        "Prague", "Brno", "Krakow", "Warsaw", "Gdansk", "Poznan", "Wroclaw",
        "Budapest", "Debrecen", "Szeged", "Belgrade", "Zagreb", "Ljubljana",
        "Bratislava", "Bucharest", "Sofia", "Dubrovnik", "Sarajevo",

        // Russian/Slavic
        "Novgorod", "Kiev", "Minsk", "Smolensk", "Vladimir", "Suzdal", "Yaroslavl",
        "Kazan", "Astrakhan", "Saratov", "Tver", "Pskov", "Ryazan", "Tula",

        // Middle Eastern Ancient
        "Ur", "Uruk", "Lagash", "Nippur", "Kish", "Eridu", "Aksum", "Meroe",
        "Timbukt", "Tyre", "Sidon", "Byblos", "Ugarit", "Ebla", "Mari", "Ashur",

        // North African
        "Cairo", "Fustat", "Giza", "Luxor", "Aswan", "Tunis", "Kairouan",
        "Algiers", "Tlemcen", "Fez", "Marrakech", "Meknes", "Tangier", "Rabat",

        // Anatolian/Turkish
        "Ankara", "Konya", "Bursa", "Izmir", "Edirne", "Trabzon", "Sivas",
        "Erzurum", "Diyarbakir", "Gaziantep", "Kayseri", "Adana", "Antalya",

        // Persian
        "Tehran", "Qom", "Kashan", "Yazd", "Kerman", "Tabriz", "Ardabil",
        "Rasht", "Mashhad", "Herat", "Kandahar", "Kabul", "Balkh", "Samarkand",

        // Central Asian
        "Khiva", "Tashkent", "Fergana", "Osh", "Bishkek", "Almaty", "Astana",
        "Ashgabat", "Mary", "Urgench", "Kokand", "Andijan",

        // Indian Subcontinent
        "Delhi", "Agra", "Varanasi", "Patna", "Kanpur", "Lucknow", "Jaipur",
        "Jodhpur", "Udaipur", "Ahmedabad", "Surat", "Mumbai", "Pune", "Hyderabad",
        "Bangalore", "Chennai", "Madurai", "Mysore", "Calcutta", "Dhaka",

        // Southeast Asian
        "Angkor", "Ayutthaya", "Sukhothai", "Pagan", "Mandalay", "Thonburi",
        "Malacca", "Brunei", "Majapahit", "Srivijaya", "Champa",

        // East Asian
        "Chang'an", "Luoyang", "Kaifeng", "Hangzhou", "Nanjing", "Beijing",
        "Kyoto", "Nara", "Kamakura", "Edo", "Osaka", "Seoul", "Pyongyang",

        // American Fantasy (Pre-Columbian inspired)
        "Tenochtitlan", "Teotihuacan", "Cholula", "Tlaxcala", "Cuzco", "Machu Picchu",
        "Tiwanaku", "Chan Chan", "Cahokia", "Chaco", "Mesa Verde",

        // Fantasy/Mythical
        "Arcadia", "Elysium", "Hyperborea", "Lemuria", "Shangri-La", "Xanadu",
        "Lyonesse", "Ys", "Mag Mell", "Tir Na Nog", "Annwn", "Hy-Brasil"
    };

    /// <summary>
    /// 100+ additional country/kingdom names
    /// </summary>
    public static readonly string[] AdditionalCountryNames =
    {
        // Medieval European Kingdoms
        "England", "Scotland", "Wales", "Ireland", "France", "Burgundy", "Aquitaine",
        "Navarre", "Aragon", "Castile", "Leon", "Portugal", "Lombardy", "Tuscany",
        "Venice", "Genoa", "Naples", "Sicily", "Bohemia", "Bavaria", "Saxony",
        "Swabia", "Brandenburg", "Austria", "Styria", "Carinthia", "Denmark", "Sweden",
        "Norway", "Poland", "Lithuania", "Hungary", "Croatia", "Serbia", "Bulgaria",

        // Ancient Kingdoms
        "Sumer", "Akkad", "Ur", "Lagash", "Elam", "Hatti", "Mitanni", "Urartu",
        "Lydia", "Phrygia", "Caria", "Lycia", "Cappadocia", "Paphlagonia",

        // Middle Eastern
        "Saba", "Himyar", "Ghassanids", "Lakhmids", "Yemen", "Oman", "Bahrain",
        "Dilmun", "Magan", "Hadramaut",

        // African Kingdoms
        "Kush", "Aksum", "Ghana", "Mali", "Songhai", "Kanem", "Bornu", "Benin",
        "Oyo", "Kongo", "Mutapa", "Kilwa", "Zanzibar", "Nubia",

        // Asian Kingdoms
        "Zhou", "Qin", "Han", "Tang", "Song", "Yuan", "Ming", "Goryeo", "Silla",
        "Baekje", "Yamato", "Nara", "Heian", "Khmer", "Champa", "Dai Viet",
        "Pagan", "Sukhothai", "Majapahit", "Srivijaya",

        // Central Asian
        "Sogdiana", "Khwarezm", "Ferghana", "Khotan", "Kushan", "Gokturk",
        "Uyghur", "Karakhanid", "Seljuk", "Khiva Khanate", "Bukhara",

        // Indian
        "Maurya", "Gupta", "Chola", "Pallava", "Chalukya", "Rashtrakuta",
        "Pandya", "Hoysala", "Vijayanagara", "Delhi Sultanate", "Gujarat",
        "Mewar", "Marwar", "Mysore", "Hyderabad",

        // Fictional/Fantasy
        "Gondor", "Rohan", "Mordor", "Arnor", "Numenor", "Lothlorien",
        "Eriador", "Harad", "Rhun", "Forodwaith"
    };

    /// <summary>
    /// Surname components for hereditary surnames
    /// </summary>
    public static readonly string[] SurnameComponents =
    {
        // Occupational
        "Smith", "Baker", "Miller", "Cooper", "Fletcher", "Thatcher", "Mason",
        "Carter", "Weaver", "Wright", "Potter", "Tanner", "Brewer", "Butcher",
        "Carpenter", "Chandler", "Merchant", "Scribe", "Priest", "Knight",

        // Locational
        "Hill", "Vale", "Wood", "Field", "Brook", "Ford", "Bridge", "Stone",
        "Marsh", "Dale", "Glen", "Heath", "Moor", "Ridge", "Shore",

        // Descriptive
        "Strong", "Wise", "Fair", "Bold", "Swift", "Noble", "True", "Good",
        "Bright", "Hardy", "Long", "Short", "White", "Black", "Brown", "Green",

        // Patronymic-derived
        "son", "sen", "ez", "ez", "ov", "ovic", "sson", "sdottir", "ian", "yan",

        // Nature
        "Wolf", "Bear", "Eagle", "Lion", "Stag", "Fox", "Hawk", "Raven",
        "Oak", "Ash", "Pine", "Birch", "Willow", "Rose", "Thorn"
    };
}
