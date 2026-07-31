using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calorizer.Business.DTOs
{
    public class CalculationDto
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public int Age { get; set; }
        public int GenderId { get; set; }
        public decimal PhysicalActivityFactor { get; set; }
        public decimal TotalEnergyExpenditure { get; set; }

        // Macronutrient Distribution
        public decimal TotalCalories { get; set; }
        public decimal CarbsPercentageMin { get; set; } = 45m;
        public decimal CarbsPercentageMax { get; set; } = 65m;
        public decimal ProteinPercentageMin { get; set; } = 10m;
        public decimal ProteinPercentageMax { get; set; } = 35m;
        public decimal FatPercentageMin { get; set; } = 20m;
        public decimal FatPercentageMax { get; set; } = 35m;

        // Calculated gram values
        public decimal CarbsGramsMin { get; set; }
        public decimal CarbsGramsMax { get; set; }
        public decimal ProteinGramsMin { get; set; }
        public decimal ProteinGramsMax { get; set; }
        public decimal FatGramsMin { get; set; }
        public decimal FatGramsMax { get; set; }

        // Food Exchange List
        public FoodExchangeListDto FoodExchangeList { get; set; } = new();

        // Notes
        public string Notes { get; set; } = string.Empty;

        // Lookups
        public List<LookupDto> PhysicalActivityFactors { get; set; } = new();
    }

    public class FoodExchangeListDto
    {
        // Milk Group
        public FoodExchangeItemDto SkimmedMilk { get; set; } = new() { FoodGroup = "Skimmed Milk" };
        public FoodExchangeItemDto LowFatMilk { get; set; } = new() { FoodGroup = "Low Fat Milk" };
        public FoodExchangeItemDto FullFatMilk { get; set; } = new() { FoodGroup = "Full Fat Milk" };

        // Other Groups
        public FoodExchangeItemDto Fruit { get; set; } = new() { FoodGroup = "Fruit" };
        public FoodExchangeItemDto Vegetable { get; set; } = new() { FoodGroup = "Vegetable" };
        public FoodExchangeItemDto Sugar { get; set; } = new() { FoodGroup = "Sugar" };
        public FoodExchangeItemDto Starch { get; set; } = new() { FoodGroup = "Starch" };

        // Meat Group
        public FoodExchangeItemDto LeanMeat { get; set; } = new() { FoodGroup = "Lean Meat" };
        public FoodExchangeItemDto MediumFatMeat { get; set; } = new() { FoodGroup = "Medium Fat Meat" };
        public FoodExchangeItemDto HighFatMeat { get; set; } = new() { FoodGroup = "High Fat Meat" };

        // Fat Group
        public FoodExchangeItemDto Fat { get; set; } = new() { FoodGroup = "Fat" };

        // Totals
        public decimal TotalCHO { get; set; }
        public decimal TotalProtein { get; set; }
        public decimal TotalFat { get; set; }
        public decimal TotalCalories { get; set; }
    }

    public class FoodExchangeItemDto
    {
        public string FoodGroup { get; set; } = string.Empty;
        public decimal Serving { get; set; }
        public decimal CHO { get; set; }
        public decimal Protein { get; set; }
        public decimal Fat { get; set; }
        public decimal Calories { get; set; }
    }
}