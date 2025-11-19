using PopulationSimulator.Models;

namespace PopulationSimulator.Core;

/// <summary>
/// Expanded inventions (120+) and jobs (75+) for richer simulation
/// </summary>
public static class ExpandedContent
{
    /// <summary>
    /// Get all 120+ inventions with categories, requirements, and bonuses
    /// </summary>
    public static List<(string name, string category, int reqIntel, string description, int healthBonus, int lifespanBonus)> GetAllInventions()
    {
        return new List<(string, string, int, string, int, int)>
        {
            // PREHISTORIC (Stone Age)
            ("Fire", "Prehistoric", 20, "Controlled use of fire for warmth and cooking", 5, 5),
            ("Stone Tools", "Prehistoric", 15, "Basic stone implements for hunting and crafting", 2, 1),
            ("Spear", "Prehistoric", 25, "Throwing weapon for hunting large game", 3, 2),
            ("Bow and Arrow", "Prehistoric", 30, "Ranged weapon for hunting and warfare", 3, 2),
            ("Club", "Prehistoric", 10, "Simple weapon for defense", 1, 0),
            ("Flint Knapping", "Prehistoric", 25, "Advanced stone tool creation", 2, 1),
            ("Shelter", "Prehistoric", 20, "Basic protection from elements", 5, 3),
            ("Clothing", "Prehistoric", 20, "Animal hide clothing for warmth", 5, 5),
            ("Bone Tools", "Prehistoric", 25, "Tools made from animal bones", 2, 1),

            // AGRICULTURE (Farming Revolution)
            ("Agriculture", "Agriculture", 35, "Systematic cultivation of crops", 10, 10),
            ("Irrigation", "Agriculture", 40, "Water management for crops", 8, 8),
            ("Plow", "Agriculture", 35, "Tool for tilling soil efficiently", 5, 5),
            ("Crop Rotation", "Agriculture", 45, "Systematic planting to maintain soil", 5, 5),
            ("Fertilizer", "Agriculture", 40, "Organic materials to enrich soil", 5, 5),
            ("Domestication", "Agriculture", 35, "Taming animals for food and labor", 8, 8),
            ("Terracing", "Agriculture", 50, "Farming on hillsides", 3, 3),
            ("Seed Selection", "Agriculture", 40, "Choosing best seeds for planting", 5, 5),
            ("Granary", "Agriculture", 35, "Storage for harvested grain", 8, 5),
            ("Windmill", "Agriculture", 55, "Wind-powered grain processing", 3, 2),

            // CRAFTS (Artisan Skills)
            ("Pottery", "Crafts", 30, "Creating vessels from clay", 5, 3),
            ("Weaving", "Crafts", 30, "Creating fabric from fibers", 5, 5),
            ("Tanning", "Crafts", 25, "Processing animal hides into leather", 3, 2),
            ("Glassmaking", "Crafts", 60, "Creating glass from sand", 5, 3),
            ("Dyeing", "Crafts", 35, "Coloring fabrics and materials", 2, 1),
            ("Basket Weaving", "Crafts", 20, "Creating containers from reeds", 2, 1),
            ("Rope Making", "Crafts", 25, "Creating strong cordage", 3, 1),
            ("Leather Working", "Crafts", 30, "Advanced leather crafts", 3, 2),
            ("Tailoring", "Crafts", 35, "Advanced clothing construction", 5, 5),
            ("Carpet Weaving", "Crafts", 40, "Decorative and functional textiles", 2, 1),

            // METALLURGY (Working Metals)
            ("Copper Working", "Metallurgy", 50, "Smelting and shaping copper", 5, 3),
            ("Bronze", "Metallurgy", 55, "Alloying copper and tin", 8, 5),
            ("Iron Working", "Metallurgy", 60, "Smelting iron ore", 8, 8),
            ("Steel", "Metallurgy", 70, "Advanced iron alloy", 10, 8),
            ("Gold Working", "Metallurgy", 50, "Crafting with precious metals", 3, 2),
            ("Silver Working", "Metallurgy", 50, "Working with silver", 3, 2),
            ("Smelting", "Metallurgy", 55, "Extracting metal from ore", 5, 3),
            ("Casting", "Metallurgy", 55, "Pouring molten metal into molds", 5, 3),
            ("Forging", "Metallurgy", 60, "Shaping metal with hammer and heat", 5, 3),
            ("Alloys", "Metallurgy", 65, "Mixing metals for better properties", 8, 5),

            // CONSTRUCTION (Building)
            ("Brick Making", "Construction", 35, "Manufacturing building bricks", 5, 3),
            ("Mortar", "Construction", 40, "Binding agent for construction", 3, 2),
            ("Arch", "Construction", 60, "Load-bearing curved structure", 5, 3),
            ("Aqueduct", "Construction", 70, "Water transport system", 15, 10),
            ("Concrete", "Construction", 65, "Durable building material", 8, 5),
            ("Architecture", "Construction", 70, "Science of building design", 10, 5),
            ("Dome", "Construction", 70, "Hemispherical roof structure", 5, 3),
            ("Fortification", "Construction", 55, "Defensive structures", 8, 5),
            ("Roads", "Construction", 50, "Paved pathways for transport", 10, 8),
            ("Bridges", "Construction", 60, "Structures spanning obstacles", 8, 5),
            ("Columns", "Construction", 55, "Supportive pillars", 3, 2),
            ("Urban Planning", "Construction", 75, "Systematic city design", 10, 8),

            // TRANSPORTATION (Movement)
            ("Wheel", "Transportation", 45, "Circular device for movement", 10, 5),
            ("Cart", "Transportation", 40, "Wheeled vehicle for goods", 8, 5),
            ("Ship", "Transportation", 55, "Watercraft for travel", 15, 10),
            ("Sail", "Transportation", 50, "Wind-powered boat propulsion", 10, 8),
            ("Chariot", "Transportation", 50, "Two-wheeled war vehicle", 5, 3),
            ("Saddle", "Transportation", 35, "Riding equipment for animals", 5, 3),
            ("Stirrup", "Transportation", 40, "Foot support for riders", 5, 3),
            ("Horseshoe", "Transportation", 45, "Protection for horse hooves", 3, 2),
            ("Canal", "Transportation", 65, "Artificial waterway", 10, 8),
            ("Lighthouse", "Transportation", 60, "Navigation aid for ships", 8, 5),

            // WRITING & KNOWLEDGE (Communication & Learning)
            ("Writing", "Writing", 65, "Recording language in symbols", 10, 10),
            ("Alphabet", "Writing", 70, "Phonetic writing system", 10, 10),
            ("Paper", "Writing", 60, "Material for writing", 5, 5),
            ("Ink", "Writing", 50, "Fluid for writing", 2, 1),
            ("Printing Press", "Writing", 80, "Mechanical text reproduction", 15, 10),
            ("Library", "Writing", 70, "Repository of knowledge", 10, 10),
            ("School", "Writing", 65, "Formal education institution", 15, 15),
            ("University", "Writing", 80, "Advanced education center", 20, 15),
            ("Encyclopedia", "Writing", 75, "Comprehensive knowledge compilation", 10, 8),
            ("Poetry", "Writing", 55, "Artistic use of language", 5, 3),
            ("Literature", "Writing", 60, "Written artistic works", 8, 5),

            // MATHEMATICS & SCIENCE (Understanding)
            ("Mathematics", "Science", 75, "Study of numbers and patterns", 10, 8),
            ("Geometry", "Science", 70, "Study of shapes and space", 8, 5),
            ("Astronomy", "Science", 75, "Study of celestial objects", 10, 8),
            ("Calendar", "Science", 65, "System for tracking time", 8, 5),
            ("Physics", "Science", 85, "Study of matter and energy", 15, 10),
            ("Chemistry", "Science", 85, "Study of substances", 15, 15),
            ("Biology", "Science", 80, "Study of living organisms", 20, 15),
            ("Scientific Method", "Science", 90, "Systematic investigation approach", 25, 20),
            ("Optics", "Science", 75, "Study of light", 8, 5),
            ("Mechanics", "Science", 80, "Study of motion and force", 10, 8),

            // MEDICINE (Health & Healing)
            ("Herbal Medicine", "Medicine", 40, "Plant-based healing", 10, 10),
            ("Surgery", "Medicine", 70, "Medical procedures", 20, 20),
            ("Anesthesia", "Medicine", 75, "Pain prevention during surgery", 15, 15),
            ("Sanitation", "Medicine", 50, "Cleanliness for health", 25, 25),
            ("Quarantine", "Medicine", 60, "Isolation to prevent disease spread", 20, 15),
            ("Vaccination", "Medicine", 85, "Disease prevention through immunity", 35, 30),
            ("Penicillin", "Medicine", 90, "Revolutionary antibiotic", 40, 35),
            ("Anatomy", "Medicine", 70, "Study of body structure", 15, 10),
            ("Midwifery", "Medicine", 45, "Childbirth assistance", 20, 15),
            ("Dentistry", "Medicine", 60, "Oral health care", 10, 8),
            ("Pharmacy", "Medicine", 70, "Preparation of medicines", 20, 15),
            ("Hospital", "Medicine", 75, "Medical care facility", 30, 25),

            // MILITARY (Warfare)
            ("Fortification", "Military", 55, "Defensive structures", 0, 0),
            ("Siege Weapons", "Military", 65, "Weapons for attacking fortifications", 0, 0),
            ("Catapult", "Military", 70, "Stone-throwing siege engine", 0, 0),
            ("Crossbow", "Military", 60, "Mechanical bow weapon", 0, 0),
            ("Gunpowder", "Military", 80, "Explosive propellant", 0, 0),
            ("Cannon", "Military", 85, "Large gunpowder weapon", 0, 0),
            ("Tactics", "Military", 70, "Organized battle strategies", 0, 0),
            ("Armor", "Military", 55, "Protective gear", 0, 0),
            ("Naval Warfare", "Military", 75, "Sea-based combat", 0, 0),
            ("Logistics", "Military", 70, "Supply chain management", 0, 0),

            // ARTS (Culture & Beauty)
            ("Painting", "Arts", 50, "Visual art on surfaces", 5, 3),
            ("Sculpture", "Arts", 55, "Three-dimensional art", 5, 3),
            ("Music", "Arts", 45, "Organized sound for pleasure", 8, 5),
            ("Dance", "Arts", 40, "Rhythmic body movement", 5, 3),
            ("Theater", "Arts", 60, "Dramatic performance art", 8, 5),
            ("Architecture", "Arts", 70, "Design of aesthetic structures", 5, 3),
            ("Mosaic", "Arts", 55, "Art from small pieces", 3, 2),
            ("Tapestry", "Arts", 50, "Decorative woven fabric", 3, 2),
            ("Jewelry", "Arts", 45, "Ornamental personal items", 2, 1),
            ("Perfume", "Arts", 50, "Fragrant substances", 5, 3),

            // FOOD & COOKING (Sustenance)
            ("Bread Baking", "Food", 30, "Creating bread from grain", 10, 8),
            ("Beer Brewing", "Food", 35, "Fermenting grain into alcohol", 5, 3),
            ("Wine Making", "Food", 40, "Fermenting grapes", 5, 5),
            ("Cheese Making", "Food", 35, "Preserving milk as cheese", 8, 8),
            ("Pickling", "Food", 30, "Preserving food in brine", 10, 8),
            ("Smoking", "Food", 25, "Preserving meat with smoke", 8, 5),
            ("Milling", "Food", 35, "Grinding grain into flour", 8, 5),
            ("Spices", "Food", 40, "Flavoring and preserving food", 10, 8),
            ("Sugar Refining", "Food", 55, "Processing sugar cane", 5, 3),
            ("Cooking", "Food", 25, "Preparing food with heat", 10, 10),
        };
    }

    /// <summary>
    /// Get all 75+ jobs with requirements and characteristics
    /// Now includes gender and age restrictions for realism
    /// </summary>
    public static List<(string name, int intel, int str, int minAge, int? maxAge, string gender, decimal salary, int status, double risk, string? requiredInvention)> GetAllJobs()
    {
        return new List<(string, int, int, int, int?, string, decimal, int, double, string?)>
        {
            // BASIC LABOR (No requirements)
            ("Farmer", 10, 20, 12, null, "Any", 10m, 1, 1.0, null),
            ("Hunter", 15, 30, 14, 50, "Male", 15m, 2, 1.5, null), // Physically demanding, age limit
            ("Gatherer", 10, 15, 12, null, "Any", 8m, 1, 0.8, null),
            ("Fisherman", 12, 25, 14, 55, "Male", 12m, 2, 1.2, null), // Physically demanding
            ("Shepherd", 10, 20, 12, null, "Any", 10m, 1, 0.9, null),
            ("Builder", 15, 35, 16, 55, "Male", 20m, 3, 1.3, null), // Heavy physical labor
            ("Servant", 8, 12, 12, null, "Any", 5m, 0, 0.9, null),
            ("Laborer", 8, 30, 14, 60, "Any", 8m, 1, 1.5, null),
            ("Herder", 10, 20, 12, null, "Any", 10m, 1, 1.0, null),
            ("Woodcutter", 10, 35, 16, 55, "Male", 12m, 1, 1.8, null), // Dangerous and physical

            // AGRICULTURE
            ("Miller", 15, 25, 16, 60, "Any", 15m, 2, 1.0, "Milling"),
            ("Vintner", 18, 15, 18, null, "Any", 25m, 3, 0.8, "Wine Making"),
            ("Brewer", 18, 15, 16, null, "Any", 15m, 2, 0.8, "Beer Brewing"),
            ("Baker", 15, 15, 14, null, "Any", 12m, 2, 0.8, "Bread Baking"),
            ("Cheese Maker", 15, 15, 16, null, "Any", 14m, 2, 0.8, "Cheese Making"),

            // CRAFTS
            ("Potter", 20, 15, 16, null, "Any", 18m, 3, 0.9, "Pottery"),
            ("Weaver", 18, 15, 16, null, "Female", 16m, 3, 0.8, "Weaving"), // Traditionally female
            ("Tanner", 15, 20, 16, 60, "Male", 15m, 2, 1.0, "Tanning"), // Physically demanding, unpleasant
            ("Glassmaker", 30, 20, 18, 60, "Male", 35m, 5, 1.2, "Glassmaking"), // High heat, dangerous
            ("Dyer", 18, 15, 16, null, "Any", 16m, 3, 0.9, "Dyeing"),
            ("Basket Weaver", 12, 15, 14, null, "Any", 10m, 2, 0.8, "Basket Weaving"),
            ("Rope Maker", 15, 20, 14, null, "Any", 12m, 2, 0.9, "Rope Making"),
            ("Leather Worker", 18, 18, 16, null, "Any", 18m, 3, 0.9, "Leather Working"),
            ("Tailor", 20, 12, 16, null, "Any", 20m, 3, 0.7, "Tailoring"),
            ("Carpet Weaver", 22, 15, 18, null, "Any", 25m, 4, 0.8, "Carpet Weaving"),

            // METALLURGY (Heavy physical labor, high heat - male dominated)
            ("Copper Smith", 25, 35, 18, 60, "Male", 28m, 4, 1.4, "Copper Working"),
            ("Bronze Smith", 28, 38, 18, 60, "Male", 32m, 5, 1.5, "Bronze"),
            ("Iron Smith", 30, 40, 18, 60, "Male", 38m, 6, 1.6, "Iron Working"),
            ("Blacksmith", 32, 40, 18, 60, "Male", 42m, 6, 1.6, "Steel"),
            ("Goldsmith", 35, 25, 20, null, "Any", 50m, 7, 1.0, "Gold Working"), // Fine work, less physical
            ("Silversmith", 35, 25, 20, null, "Any", 48m, 7, 1.0, "Silver Working"),
            ("Smelter", 28, 40, 18, 55, "Male", 30m, 4, 1.8, "Smelting"), // Very dangerous
            ("Caster", 30, 35, 18, 60, "Male", 32m, 4, 1.5, "Casting"),
            ("Weaponsmith", 35, 38, 20, 60, "Male", 45m, 6, 1.3, "Forging"),

            // CONSTRUCTION (Heavy labor - age limits)
            ("Mason", 20, 38, 16, 60, "Male", 22m, 3, 1.4, "Brick Making"),
            ("Architect", 40, 20, 22, null, "Any", 60m, 8, 0.8, "Architecture"), // Mental work, can work older
            ("Engineer", 42, 25, 22, null, "Any", 65m, 8, 1.0, "Mathematics"),
            ("Stonemason", 22, 40, 16, 60, "Male", 25m, 3, 1.5, "Stone Tools"),
            ("Carpenter", 18, 30, 16, 60, "Male", 20m, 3, 1.3, null),
            ("Roofer", 15, 30, 16, 55, "Male", 18m, 2, 1.8, null), // Dangerous, age limit
            ("Bricklayer", 18, 35, 16, 60, "Male", 20m, 3, 1.4, "Brick Making"),

            // PROFESSIONAL (No age or gender restrictions, knowledge-based)
            ("Merchant", 30, 10, 18, null, "Any", 35m, 5, 0.9, null),
            ("Scribe", 40, 8, 20, null, "Any", 30m, 6, 0.6, "Writing"),
            ("Priest", 35, 8, 20, null, "Male", 30m, 7, 0.7, null), // Historically male dominated
            ("Healer", 38, 10, 20, null, "Female", 40m, 6, 0.8, "Herbal Medicine"), // Traditionally female
            ("Midwife", 35, 10, 25, null, "Female", 25m, 5, 0.9, null), // Female only
            ("Physician", 42, 10, 22, null, "Any", 55m, 8, 0.7, "Surgery"),
            ("Scholar", 42, 8, 22, null, "Any", 35m, 7, 0.6, "Writing"),
            ("Teacher", 38, 8, 20, null, "Any", 28m, 6, 0.6, "School"),
            ("Lawyer", 45, 8, 25, null, "Any", 50m, 8, 0.6, "Writing"),
            ("Judge", 48, 8, 30, null, "Any", 60m, 9, 0.6, "Laws"),
            ("Librarian", 40, 8, 20, null, "Any", 25m, 6, 0.5, "Library"),
            ("Scientist", 50, 8, 25, null, "Any", 40m, 8, 0.6, "Scientific Method"),
            ("Mathematician", 52, 8, 22, null, "Any", 38m, 7, 0.5, "Mathematics"),
            ("Astronomer", 48, 8, 25, null, "Any", 42m, 8, 0.6, "Astronomy"),
            ("Pharmacist", 45, 10, 22, null, "Any", 45m, 7, 0.7, "Pharmacy"),

            // ARTS & CULTURE (Generally no restrictions, talent-based)
            ("Artist", 30, 10, 18, null, "Any", 25m, 4, 0.7, "Painting"),
            ("Sculptor", 32, 20, 18, 65, "Male", 28m, 5, 0.8, "Sculpture"), // Physical work with stone
            ("Musician", 25, 10, 16, null, "Any", 20m, 4, 0.7, "Music"),
            ("Poet", 35, 8, 18, null, "Any", 22m, 5, 0.6, "Poetry"),
            ("Actor", 28, 10, 18, null, "Any", 18m, 4, 0.7, "Theater"),
            ("Dancer", 20, 25, 16, 35, "Any", 15m, 3, 1.0, "Dance"), // Physically demanding, youth career
            ("Singer", 25, 10, 16, null, "Any", 18m, 4, 0.6, "Music"),
            ("Jeweler", 32, 18, 18, null, "Any", 35m, 5, 0.9, "Jewelry"),

            // FOOD & HOSPITALITY
            ("Cook", 15, 15, 14, null, "Any", 12m, 2, 0.8, "Cooking"),
            ("Butcher", 12, 25, 14, 60, "Male", 14m, 2, 1.0, null), // Physical, dangerous
            ("Innkeeper", 25, 15, 20, null, "Any", 22m, 3, 0.8, null),
            ("Tavern Keeper", 22, 18, 18, null, "Any", 20m, 3, 0.9, "Beer Brewing"),

            // TRANSPORTATION (Physical, dangerous - age limits)
            ("Carter", 12, 25, 16, 60, "Male", 16m, 2, 1.1, "Cart"),
            ("Sailor", 18, 30, 16, 50, "Male", 20m, 3, 1.8, "Ship"), // Very dangerous, age limit
            ("Charioteer", 20, 32, 18, 45, "Male", 25m, 4, 1.5, "Chariot"), // Combat-related, prime age only
            ("Ferryman", 15, 25, 16, 65, "Male", 14m, 2, 1.3, "Ship"),
            ("Shipwright", 35, 30, 20, 60, "Male", 40m, 5, 1.2, "Ship"),

            // MINING & EXTRACTION (Very dangerous, physical - male only, age limits)
            ("Miner", 12, 42, 16, 50, "Male", 25m, 3, 2.5, null), // Extremely dangerous
            ("Quarryman", 12, 40, 16, 55, "Male", 22m, 2, 2.3, null),
            ("Prospector", 25, 30, 18, 60, "Male", 28m, 3, 1.8, null),

            // MILITARY (Combat roles - male only, prime age ranges)
            ("Warrior", 20, 40, 16, 45, "Male", 20m, 5, 3.0, null), // Peak fighting age
            ("Guard", 18, 35, 18, 55, "Male", 18m, 4, 1.8, null), // Can guard longer
            ("Archer", 22, 30, 16, 50, "Male", 22m, 5, 2.0, "Bow and Arrow"),
            ("Cavalry", 25, 35, 18, 40, "Male", 28m, 6, 2.5, "Chariot"), // Peak riders
            ("Officer", 35, 30, 25, 60, "Male", 40m, 7, 1.5, "Tactics"), // Leadership, can be older
            ("General", 45, 35, 30, 65, "Male", 80m, 10, 1.2, "Tactics"), // Experience valued

            // GOVERNANCE (Knowledge-based, can work at any age)
            ("Administrator", 40, 10, 25, null, "Any", 45m, 7, 0.7, "Writing"),
            ("Tax Collector", 35, 15, 22, null, "Any", 35m, 5, 1.0, "Mathematics"),
            ("Diplomat", 45, 10, 30, null, "Any", 55m, 9, 0.8, "Writing"),
            ("Spy", 40, 25, 22, 50, "Any", 40m, 5, 2.0, null), // Needs physical fitness

            // LEADERSHIP (Experience-based, older preferred)
            ("Leader", 38, 25, 25, null, "Any", 100m, 10, 1.0, null),
            ("King/Queen", 40, 25, 30, null, "Any", 200m, 10, 0.9, null),
        };
    }
}
