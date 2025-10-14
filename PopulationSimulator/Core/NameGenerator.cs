namespace PopulationSimulator.Core;

public class NameGenerator
{
    private readonly Random _random;
    
    private static readonly string[] MaleFirstNames = 
    {
        // Biblical names
        "Adam", "Seth", "Enosh", "Cain", "Abel", "Noah", "Shem", "Ham", "Japheth",
        "Abraham", "Isaac", "Jacob", "Joseph", "Moses", "David", "Solomon", "Samuel",
        "Daniel", "Ezra", "Elijah", "Isaiah", "Jeremiah", "Joshua", "Caleb", "Aaron",
        "Benjamin", "Reuben", "Simeon", "Levi", "Judah", "Dan", "Naphtali", "Gad",
        "Asher", "Issachar", "Zebulun", "Ephraim", "Manasseh", "Micah", "Amos",
        "Jonah", "Joel", "Obadiah", "Malachi", "Hosea", "Nahum", "Habakkuk", "Zephaniah",
        "Haggai", "Zechariah", "Saul", "Jonathan", "Absalom", "Adonijah", "Rehoboam",
        "Jeroboam", "Ahab", "Jehu", "Hezekiah", "Josiah", "Ezekiel", "Nehemiah",
        "Ezekiel", "Job", "Enoch", "Methuselah", "Lamech", "Tubal", "Jubal", "Jabal",
        // Roman names
        "Marcus", "Lucius", "Gaius", "Julius", "Augustus", "Titus", "Maximus", "Antonius",
        "Octavius", "Claudius", "Nero", "Constantine", "Hadrian", "Trajan", "Aurelius",
        "Severus", "Vespasian", "Domitian", "Commodus", "Caligula", "Tiberius", "Brutus",
        "Cassius", "Scipio", "Cato", "Cicero", "Pompey", "Crassus", "Decimus", "Fabius",
        // Greek names
        "Alexander", "Philip", "Perseus", "Leonidas", "Achilles", "Hector", "Odysseus",
        "Theseus", "Jason", "Hercules", "Apollo", "Ares", "Atlas", "Orion", "Prometheus",
        "Demetrius", "Nikias", "Pericles", "Solon", "Thales", "Pythagoras", "Socrates",
        "Plato", "Aristotle", "Diogenes", "Epicurus", "Zeno", "Heraclitus", "Anaximander",
        // Germanic/Norse names
        "Erik", "Bjorn", "Ragnar", "Leif", "Olaf", "Gunnar", "Sven", "Thor", "Odin",
        "Sigurd", "Harald", "Magnus", "Ulf", "Halfdan", "Canute", "Rollo", "Ivar",
        // Celtic names
        "Arthur", "Lancelot", "Gawain", "Percival", "Tristan", "Galahad", "Merlin",
        "Conan", "Cormac", "Fergus", "Finn", "Owen", "Lugh", "Bran", "Dylan",
        // Medieval European
        "William", "James", "Thomas", "Robert", "John", "Michael", "Richard", "Charles",
        "Henry", "Edward", "George", "Frederick", "Albert", "Victor", "Edmund", "Harold",
        "Stephen", "Hugh", "Roger", "Walter", "Ralph", "Peter", "Geoffrey", "Bernard",
        // Arabic/Persian
        "Ali", "Hassan", "Hussein", "Omar", "Khalid", "Rashid", "Tariq", "Salim",
        "Karim", "Jamal", "Faisal", "Ibrahim", "Yusuf", "Mustafa", "Ahmad", "Mahmoud",
        "Darius", "Cyrus", "Xerxes", "Artaxerxes", "Rostam", "Bahram", "Kaveh", "Farhad",
        // Modern names
        "Ethan", "Liam", "Mason", "Lucas", "Oliver", "Logan", "Aiden", "Jackson",
        "Sebastian", "Matthew", "Nathan", "Gabriel", "Adrian", "Julian", "Vincent",
        "Dominic", "Marcus", "Felix", "Leo", "Max", "Oscar", "Hugo", "Jasper", "Miles"
    };
    
    private static readonly string[] FemaleFirstNames = 
    {
        // Biblical names
        "Eve", "Sarah", "Rebecca", "Rachel", "Leah", "Miriam", "Deborah", "Ruth",
        "Esther", "Judith", "Hannah", "Naomi", "Abigail", "Bathsheba", "Tamar",
        "Dinah", "Zilpah", "Bilhah", "Keturah", "Hagar", "Mary", "Elizabeth",
        "Martha", "Lydia", "Priscilla", "Phoebe", "Anna", "Joanna", "Susanna",
        "Salome", "Dorcas", "Rhoda", "Claudia", "Eunice", "Lois", "Ada", "Adah",
        "Jezebel", "Athaliah", "Jochebed", "Zipporah", "Rahab", "Delilah", "Orpah",
        "Peninnah", "Michal", "Merab", "Abishag", "Rizpah", "Haggith", "Vashti",
        // Roman names
        "Helena", "Julia", "Livia", "Octavia", "Agrippina", "Valeria", "Cornelia",
        "Fulvia", "Clodia", "Faustina", "Lucretia", "Portia", "Tullia", "Servilia",
        "Aurelia", "Flavia", "Antonia", "Claudia", "Domitia", "Marcella", "Sabina",
        // Greek names
        "Athena", "Artemis", "Aphrodite", "Hera", "Demeter", "Persephone", "Hecate",
        "Cassandra", "Penelope", "Helen", "Andromeda", "Ariadne", "Elektra", "Medea",
        "Phoebe", "Selene", "Iris", "Nike", "Thalia", "Clio", "Calliope", "Daphne",
        "Phyllis", "Xanthe", "Zoe", "Hermione", "Antigone", "Eurydice", "Iphigenia",
        // Germanic/Norse names
        "Freya", "Astrid", "Sigrid", "Ingrid", "Helga", "Brunhild", "Gudrun", "Hilda",
        "Solveig", "Thyra", "Ragna", "Eira", "Saga", "Sif", "Skadi", "Urd", "Verdandi",
        // Celtic names
        "Guinevere", "Morgana", "Isolde", "Brianna", "Maeve", "Deirdre", "Niamh",
        "Brigid", "Rhiannon", "Aine", "Fiona", "Saoirse", "Siobhan", "Ciara", "Moira",
        // Medieval European
        "Eleanor", "Matilda", "Adelaide", "Beatrice", "Catherine", "Margaret", "Anne",
        "Isabella", "Joanna", "Agnes", "Joan", "Blanche", "Constance", "Alice", "Cecilia",
        "Clare", "Edith", "Gertrude", "Maud", "Rosamund", "Philippa", "Eleanor", "Bertha",
        // Arabic/Persian
        "Fatima", "Aisha", "Zainab", "Layla", "Salma", "Amina", "Nadia", "Yasmin",
        "Zahra", "Mariam", "Safiya", "Halima", "Khadija", "Zara", "Soraya", "Roxana",
        "Esther", "Shirin", "Parisa", "Azadeh", "Farah", "Nasrin", "Laleh", "Mitra",
        // Modern names
        "Emma", "Olivia", "Ava", "Sophia", "Charlotte", "Mia", "Amelia", "Isabella",
        "Harper", "Evelyn", "Emily", "Sofia", "Avery", "Ella", "Madison", "Luna",
        "Scarlett", "Grace", "Victoria", "Riley", "Aria", "Lily", "Aurora", "Hazel",
        "Violet", "Stella", "Natalie", "Aurora", "Savannah", "Claire", "Lucy", "Alice"
    };
    
    private static readonly string[] CityNames = 
    {
        // Biblical cities
        "Eden", "Nod", "Enoch", "Babel", "Ur", "Haran", "Damascus", "Jericho",
        "Jerusalem", "Bethel", "Hebron", "Beersheba", "Shechem", "Shiloh", "Gilgal",
        "Ramah", "Mizpah", "Gibeah", "Joppa", "Tyre", "Sidon", "Nineveh", "Babylon",
        "Ashkelon", "Gaza", "Ashdod", "Gath", "Ekron", "Lachish", "Megiddo", "Hazor",
        "Samaria", "Nazareth", "Capernaum", "Bethlehem", "Emmaus", "Caesarea Philippi",
        // Ancient Egyptian
        "Memphis", "Thebes", "Heliopolis", "Akhetaten", "Abydos", "Karnak", "Luxor",
        "Tanis", "Bubastis", "Elephantine", "Aswan", "Edfu", "Kom Ombo", "Dendera",
        // Greek cities
        "Athens", "Sparta", "Corinth", "Thebes", "Argos", "Megara", "Elis", "Delphi",
        "Olympia", "Marathon", "Thermopylae", "Delos", "Rhodes", "Miletus", "Ephesus",
        "Smyrna", "Pergamon", "Sardis", "Philadelphia", "Laodicea", "Colossae", "Thyatira",
        "Troy", "Mycenae", "Knossos", "Phaistos", "Gortyn", "Syracuse", "Tarentum",
        // Roman cities
        "Rome", "Carthage", "Alexandria", "Antioch", "Constantinople", "Ravenna",
        "Milan", "Florence", "Venice", "Naples", "Genoa", "Pisa", "Pompeii", "Herculaneum",
        "Ostia", "Verona", "Bologna", "Padua", "Aquileia", "Mediolanum", "Londinium",
        "Lutetia", "Massilia", "Lugdunum", "Augusta Treverorum", "Colonia Agrippina",
        // Byzantine/Eastern
        "Byzantium", "Adrianople", "Nicaea", "Chalcedon", "Trebizond", "Thessalonica",
        "Edessa", "Antioch", "Tarsus", "Iconium", "Caesarea", "Jerusalem", "Alexandria",
        // Persian/Mesopotamian
        "Persepolis", "Susa", "Ecbatana", "Ctesiphon", "Seleucia", "Pasargadae",
        "Hamadan", "Isfahan", "Shiraz", "Rey", "Nishapur", "Merv", "Bukhara", "Samarkand",
        // Middle Eastern
        "Petra", "Palmyra", "Jerash", "Amman", "Aleppo", "Homs", "Emesa", "Apamea",
        "Dura Europos", "Hatra", "Arbela", "Mosul", "Basra", "Kuwait", "Hormuz",
        // African
        "Cyrene", "Leptis Magna", "Carthago Nova", "Tingis", "Volubilis", "Aksum",
        "Meroe", "Napata", "Adulis", "Zimbabwe", "Kilwa", "Mombasa", "Mogadishu",
        // Ancient Asian
        "Babylon", "Nineveh", "Assur", "Nimrud", "Harran", "Carchemish", "Mari",
        "Ugarit", "Byblos", "Berytus", "Tripoli", "Arwad", "Sidon", "Tyre",
        // Fictional/Mythical
        "Avalon", "Camelot", "Atlantis", "Shambhala", "El Dorado", "Asgard", "Valhalla"
    };
    
    private static readonly string[] CountryNames = 
    {
        // Biblical/Ancient Near East
        "Eden", "Mesopotamia", "Canaan", "Egypt", "Assyria", "Babylon", "Persia",
        "Israel", "Judah", "Phoenicia", "Philistia", "Moab", "Edom", "Ammon",
        "Aram", "Media", "Elam", "Hatti", "Kush", "Punt", "Sheba", "Midian",
        // Classical
        "Greece", "Rome", "Carthage", "Macedonia", "Thrace", "Illyria", "Dacia",
        "Parthia", "Armenia", "Iberia", "Colchis", "Pontus", "Bithynia", "Galatia",
        // European
        "Gaul", "Germania", "Britannia", "Hispania", "Italia", "Dalmatia", "Pannonia",
        "Noricum", "Raetia", "Belgica", "Aquitania", "Lusitania", "Hibernia",
        // Middle Eastern
        "Arabia", "Nabatea", "Palmyra", "Osroene", "Adiabene", "Characene",
        // African
        "Numidia", "Mauretania", "Libya", "Cyrenaica", "Ethiopia", "Axum",
        // Asian
        "Bactria", "Sogdia", "Margiana", "Hyrcania", "Gedrosia", "Carmania"
    };
    
    private static readonly string[] ReligionNames = 
    {
        "Way of Adam", "Sethite Faith", "Ancient Path", "Covenant of Noah",
        "Abrahamic Faith", "Mosaic Law", "Davidic Worship", "Temple Rites",
        "Prophetic Way", "Mystery Cult", "Sun Worship", "Moon Worship",
        "Storm Faith", "Earth Mother", "Sky Father", "River Cult"
    };
    
    private static readonly string[] DynastyPrefixes = 
    {
        "House of", "Dynasty of", "Line of", "Bloodline of", "Clan of"
    };
    
    // Comprehensive invention data (Name, Category, RequiredIntelligence, HealthBonus, LifespanBonus, Description)
    public static readonly (string Name, string Category, int RequiredIntel, int HealthBonus, int LifespanBonus, string Description)[] InventionData = 
    {
        // Prehistoric/Stone Age
        ("Fire", "Basic Technology", 30, 5, 5, "Control of fire for cooking and warmth"),
        ("Stone Tools", "Tools", 25, 0, 0, "Basic stone implements for cutting and scraping"),
        ("Spear", "Weapons", 30, 0, 0, "Hunting weapon with stone point"),
        ("Bow and Arrow", "Weapons", 40, 0, 0, "Ranged hunting weapon"),
        ("Clothing", "Technology", 25, 3, 2, "Protective garments from animal hides"),
        ("Shelter", "Architecture", 30, 5, 3, "Basic permanent dwellings"),
        // Agriculture
        ("Agriculture", "Agriculture", 50, 8, 10, "Systematic cultivation of crops"),
        ("Irrigation", "Agriculture", 55, 5, 5, "Water management for crops"),
        ("Plow", "Agriculture", 45, 0, 0, "Tool for tilling soil"),
        ("Crop Rotation", "Agriculture", 60, 5, 5, "Systematic field management"),
        ("Animal Husbandry", "Agriculture", 50, 5, 5, "Domestication and breeding of animals"),
        // Crafts and Production
        ("Pottery", "Crafts", 40, 0, 0, "Ceramic vessels for storage"),
        ("Weaving", "Crafts", 35, 0, 0, "Textile production from fibers"),
        ("Tanning", "Crafts", 30, 0, 0, "Leather production from hides"),
        ("Dyeing", "Crafts", 40, 0, 0, "Coloring of textiles"),
        ("Glassmaking", "Crafts", 60, 0, 0, "Production of glass objects"),
        // Metallurgy
        ("Copper Working", "Metallurgy", 50, 0, 0, "Working with copper metal"),
        ("Bronze", "Metallurgy", 60, 0, 0, "Alloy of copper and tin"),
        ("Iron Working", "Metallurgy", 70, 0, 0, "Smelting and forging iron"),
        ("Steel", "Metallurgy", 75, 0, 0, "Refined iron alloy"),
        ("Gold Working", "Metallurgy", 55, 0, 0, "Crafting with precious metals"),
        // Construction
        ("Brick Making", "Architecture", 40, 0, 0, "Fired clay construction material"),
        ("Mortar", "Architecture", 45, 0, 0, "Binding material for construction"),
        ("Arch", "Architecture", 65, 0, 0, "Curved structural element"),
        ("Dome", "Architecture", 70, 0, 0, "Hemispherical roof structure"),
        ("Aqueduct", "Engineering", 75, 8, 5, "Water transport system"),
        // Transportation
        ("Wheel", "Technology", 55, 0, 0, "Rotating disc for transport"),
        ("Cart", "Transportation", 45, 0, 0, "Wheeled vehicle"),
        ("Chariot", "Transportation", 50, 0, 0, "Fast wheeled war vehicle"),
        ("Ship", "Transportation", 60, 0, 0, "Water-going vessel"),
        ("Sail", "Transportation", 50, 0, 0, "Wind-powered propulsion"),
        ("Road Paving", "Engineering", 55, 0, 0, "Improved travel infrastructure"),
        // Writing and Knowledge
        ("Writing", "Knowledge", 70, 0, 0, "Recording information in symbols"),
        ("Alphabet", "Knowledge", 75, 0, 0, "Phonetic writing system"),
        ("Papyrus", "Knowledge", 50, 0, 0, "Writing material from reeds"),
        ("Parchment", "Knowledge", 55, 0, 0, "Writing material from animal skin"),
        ("Ink", "Knowledge", 40, 0, 0, "Liquid for writing"),
        ("Library", "Knowledge", 70, 0, 0, "Repository of written works"),
        // Mathematics and Science
        ("Mathematics", "Science", 80, 0, 0, "Systematic study of numbers"),
        ("Geometry", "Science", 80, 0, 0, "Study of shapes and space"),
        ("Astronomy", "Science", 85, 0, 0, "Study of celestial bodies"),
        ("Calendar", "Science", 70, 0, 0, "System for organizing time"),
        ("Sundial", "Science", 60, 0, 0, "Device for telling time"),
        ("Water Clock", "Science", 65, 0, 0, "Time-keeping device"),
        // Medicine
        ("Herbal Medicine", "Medicine", 50, 10, 8, "Plant-based healing"),
        ("Surgery", "Medicine", 75, 15, 12, "Operative medical procedures"),
        ("Antiseptics", "Medicine", 80, 20, 15, "Infection prevention"),
        ("Anesthesia", "Medicine", 85, 15, 10, "Pain management during surgery"),
        ("Penicillin", "Medicine", 95, 30, 25, "Antibiotic for infections"),
        ("Vaccination", "Medicine", 90, 25, 20, "Disease prevention through immunization"),
        ("Anatomy", "Medicine", 80, 10, 8, "Study of body structure"),
        ("Pharmacy", "Medicine", 70, 12, 10, "Preparation of medicinal compounds"),
        // Military
        ("Fortification", "Military", 60, 0, 0, "Defensive structures"),
        ("Siege Weapons", "Military", 70, 0, 0, "Weapons for attacking fortifications"),
        ("Catapult", "Military", 70, 0, 0, "Stone-throwing siege engine"),
        ("Crossbow", "Military", 65, 0, 0, "Mechanical projectile weapon"),
        ("Gunpowder", "Military", 85, 0, 0, "Explosive propellant"),
        ("Cannon", "Military", 80, 0, 0, "Explosive projectile weapon"),
        // Agriculture Advanced
        ("Windmill", "Agriculture", 65, 0, 0, "Wind-powered grain grinding"),
        ("Water Mill", "Agriculture", 60, 0, 0, "Water-powered grain grinding"),
        ("Fertilizer", "Agriculture", 55, 0, 0, "Soil enrichment for better crops"),
        ("Seed Drill", "Agriculture", 60, 0, 0, "Efficient seed planting"),
        // Art and Culture
        ("Music", "Art", 40, 3, 2, "Organized sound for aesthetic purpose"),
        ("Painting", "Art", 45, 0, 0, "Visual art on surfaces"),
        ("Sculpture", "Art", 50, 0, 0, "Three-dimensional art"),
        ("Architecture", "Art", 75, 0, 0, "Design of buildings"),
        ("Drama", "Art", 60, 0, 0, "Theatrical performance"),
        ("Poetry", "Art", 65, 0, 0, "Literary art in verse"),
        // Miscellaneous
        ("Beer Brewing", "Food", 35, 2, 0, "Fermented beverage production"),
        ("Wine Making", "Food", 40, 2, 0, "Grape fermentation"),
        ("Cheese Making", "Food", 30, 3, 2, "Dairy preservation"),
        ("Bread Baking", "Food", 30, 5, 3, "Leavened bread production"),
        ("Salt Preservation", "Food", 35, 5, 3, "Food preservation with salt"),
        ("Smoking", "Food", 25, 3, 2, "Food preservation through smoke"),
        ("Refrigeration", "Technology", 90, 10, 8, "Cold storage for food preservation"),
        ("Soap", "Hygiene", 40, 8, 5, "Cleaning agent"),
        ("Plumbing", "Engineering", 65, 15, 10, "Indoor water systems"),
        ("Sewage System", "Engineering", 70, 20, 15, "Waste disposal infrastructure"),
        ("Printing Press", "Knowledge", 85, 0, 0, "Mechanical text reproduction"),
        ("Telescope", "Science", 85, 0, 0, "Instrument for viewing distant objects"),
        ("Microscope", "Science", 90, 5, 3, "Instrument for viewing small objects"),
        ("Compass", "Navigation", 60, 0, 0, "Magnetic direction finding"),
        ("Map Making", "Navigation", 70, 0, 0, "Cartographic representation")
    };
    
    private static readonly string[] EyeColors = 
    {
        "Brown", "Blue", "Green", "Hazel", "Amber", "Gray"
    };
    
    private static readonly string[] HairColors = 
    {
        "Black", "Dark Brown", "Brown", "Light Brown", "Blonde", "Red", "Auburn"
    };
    
    public NameGenerator(Random random)
    {
        _random = random;
    }
    
    public string GenerateMaleFirstName()
    {
        return MaleFirstNames[_random.Next(MaleFirstNames.Length)];
    }
    
    public string GenerateFemaleFirstName()
    {
        return FemaleFirstNames[_random.Next(FemaleFirstNames.Length)];
    }
    
    public string GenerateLastName(string? fatherName, string? cityName, string? jobName, int generationNumber)
    {
        // Early generations: patronymic names (only if father name is known)
        if (generationNumber < 10 && !string.IsNullOrEmpty(fatherName) && fatherName != "Unknown")
        {
            return $"ben {fatherName}"; // "son of"
        }
        
        // Middle generations: city-based names
        if (generationNumber < 50 && !string.IsNullOrEmpty(cityName))
        {
            return $"of {cityName}";
        }
        
        // Later generations: occupation-based names
        if (!string.IsNullOrEmpty(jobName))
        {
            return jobName;
        }
        
        // Fallback: patronymic (only if father name is known and not "Unknown")
        if (!string.IsNullOrEmpty(fatherName) && fatherName != "Unknown")
        {
            return $"ben {fatherName}";
        }
        
        // Last resort: use mother's last name or "Unknown"
        return "Unknown";
    }
    
    public string GenerateCityName()
    {
        return CityNames[_random.Next(CityNames.Length)];
    }
    
    public string GenerateCountryName()
    {
        return CountryNames[_random.Next(CountryNames.Length)];
    }
    
    public string GenerateReligionName()
    {
        return ReligionNames[_random.Next(ReligionNames.Length)];
    }
    
    public string GenerateDynastyName(string founderName)
    {
        string prefix = DynastyPrefixes[_random.Next(DynastyPrefixes.Length)];
        return $"{prefix} {founderName}";
    }
    
    public string GenerateEyeColor()
    {
        return EyeColors[_random.Next(EyeColors.Length)];
    }
    
    public string GenerateHairColor()
    {
        return HairColors[_random.Next(HairColors.Length)];
    }
    
    public string GenerateWarName(string attackerName, string defenderName)
    {
        string[] patterns = 
        {
            $"War of {attackerName} and {defenderName}",
            $"Conflict of {attackerName}",
            $"Invasion of {defenderName}",
            $"Great War",
            $"Holy War"
        };
        return patterns[_random.Next(patterns.Length)];
    }
}
