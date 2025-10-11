namespace PopulationSimulator.Core;

public class NameGenerator
{
    private readonly Random _random;
    
    private static readonly string[] MaleFirstNames = 
    {
        "Adam", "Seth", "Enosh", "Cain", "Abel", "Noah", "Shem", "Ham", "Japheth",
        "Abraham", "Isaac", "Jacob", "Joseph", "Moses", "David", "Solomon", "Samuel",
        "Daniel", "Ezra", "Elijah", "Isaiah", "Jeremiah", "Joshua", "Caleb", "Aaron",
        "Benjamin", "Reuben", "Simeon", "Levi", "Judah", "Dan", "Naphtali", "Gad",
        "Asher", "Issachar", "Zebulun", "Ephraim", "Manasseh", "Micah", "Amos",
        "Jonah", "Joel", "Obadiah", "Malachi", "Hosea", "Nahum", "Habakkuk", "Zephaniah",
        "Haggai", "Zechariah", "Saul", "Jonathan", "Absalom", "Adonijah", "Rehoboam",
        "Jeroboam", "Ahab", "Jehu", "Hezekiah", "Josiah", "Jeremiah", "Ezekiel",
        "Marcus", "Lucius", "Gaius", "Julius", "Augustus", "Titus", "Maximus", "Antonius",
        "Octavius", "Claudius", "Nero", "Constantine", "Hadrian", "Trajan", "Aurelius",
        "Alexander", "Philip", "Perseus", "Leonidas", "Achilles", "Hector", "Odysseus",
        "Theseus", "Jason", "Hercules", "Apollo", "Ares", "Atlas", "Orion", "Perseus",
        "William", "James", "Thomas", "Robert", "John", "Michael", "Richard", "Charles",
        "Henry", "Edward", "George", "Arthur", "Frederick", "Albert", "Victor", "Edmund",
        "Ethan", "Noah", "Liam", "Mason", "Lucas", "Oliver", "Elijah", "Logan", "Aiden",
        "Jackson", "Sebastian", "Alexander", "Benjamin", "Matthew", "Samuel", "Andrew"
    };
    
    private static readonly string[] FemaleFirstNames = 
    {
        "Eve", "Sarah", "Rebecca", "Rachel", "Leah", "Miriam", "Deborah", "Ruth",
        "Esther", "Judith", "Hannah", "Naomi", "Abigail", "Bathsheba", "Tamar",
        "Dinah", "Zilpah", "Bilhah", "Keturah", "Hagar", "Mary", "Elizabeth",
        "Martha", "Lydia", "Priscilla", "Phoebe", "Anna", "Joanna", "Susanna",
        "Salome", "Dorcas", "Rhoda", "Chloe", "Julia", "Claudia", "Eunice", "Lois",
        "Jezebel", "Athaliah", "Jochebed", "Zipporah", "Rahab", "Delilah", "Orpah",
        "Peninnah", "Michal", "Merab", "Abishag", "Abigail", "Rizpah", "Haggith",
        "Helena", "Julia", "Livia", "Octavia", "Agrippina", "Valeria", "Cornelia",
        "Fulvia", "Clodia", "Faustina", "Lucretia", "Portia", "Tullia", "Servilia",
        "Athena", "Artemis", "Aphrodite", "Hera", "Demeter", "Persephone", "Hecate",
        "Cassandra", "Penelope", "Helen", "Andromeda", "Ariadne", "Elektra", "Medea",
        "Emma", "Olivia", "Ava", "Isabella", "Sophia", "Charlotte", "Mia", "Amelia",
        "Harper", "Evelyn", "Abigail", "Emily", "Elizabeth", "Sofia", "Avery", "Ella",
        "Scarlett", "Grace", "Chloe", "Victoria", "Riley", "Aria", "Lily", "Aurora"
    };
    
    private static readonly string[] CityNames = 
    {
        "Eden", "Nod", "Enoch", "Babel", "Ur", "Haran", "Damascus", "Jericho",
        "Jerusalem", "Bethel", "Hebron", "Beersheba", "Shechem", "Shiloh", "Gilgal",
        "Ramah", "Mizpah", "Gibeah", "Joppa", "Tyre", "Sidon", "Nineveh", "Babylon",
        "Memphis", "Thebes", "Athens", "Sparta", "Rome", "Carthage", "Alexandria",
        "Corinth", "Ephesus", "Antioch", "Pergamon", "Smyrna", "Sardis", "Philadelphia",
        "Laodicea", "Colossae", "Thyatira", "Troy", "Mycenae", "Knossos", "Delphi",
        "Olympia", "Marathon", "Thermopylae", "Syracuse", "Pompeii", "Herculaneum",
        "Ostia", "Ravenna", "Milan", "Florence", "Venice", "Naples", "Genoa", "Pisa",
        "Byzantium", "Constantinople", "Adrianople", "Nicaea", "Chalcedon", "Tarsus",
        "Caesarea", "Petra", "Palmyra", "Persepolis", "Susa", "Ecbatana", "Ctesiphon"
    };
    
    private static readonly string[] CountryNames = 
    {
        "Eden", "Mesopotamia", "Canaan", "Egypt", "Assyria", "Babylon", "Persia",
        "Israel", "Judah", "Phoenicia", "Philistia", "Moab", "Edom", "Ammon",
        "Aram", "Media", "Elam", "Hatti", "Kush", "Punt"
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
        // Early generations: patronymic names
        if (generationNumber < 10 && !string.IsNullOrEmpty(fatherName))
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
        
        // Fallback: patronymic
        if (!string.IsNullOrEmpty(fatherName))
        {
            return $"ben {fatherName}";
        }
        
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
