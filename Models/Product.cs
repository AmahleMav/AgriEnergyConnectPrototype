using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgriEnergyConnectPrototype.Models
{
    public enum ProductKind
    {
        Produce = 0,        // fruits/veg/dairy/meat/honey/grains etc.
        EnergySolution = 1  // solar/wind/biogas/storage/pumps
    }

    public class Product
    {
        [Key] public int Id { get; set; }

        // Common (both kinds)
        [Required] public string ProductName { get; set; }
        [Required] public string Category { get; set; }        // e.g., Vegetables, Fruits, Solar, Wind, Storage, Pump, Biogas…
        [Required] public string Location { get; set; }        // e.g., province or "National"
        [Required] public ProductKind Kind { get; set; } = ProductKind.Produce;

        [Url] public string? ImageUrl { get; set; }

        // Produce-only
        public DateTime? ProductionDate { get; set; }          // nullable (not used by energy)
        public string? Unit { get; set; }                      // kg, tray, punnet, bunch, 50kg bag
        [Range(0, 1_000_000)] public decimal? PricePerUnit { get; set; }
        public bool IsOrganic { get; set; }

        // Energy-only
        public string? VendorName { get; set; }                // supplier
        public string? EnergyType { get; set; }                // Solar, Wind, Biogas, Storage, Pump
        public double? PowerkW { get; set; }                   // 2.2, 5, 10, 20
        public string? SuitableFor { get; set; }               // irrigation, cold storage, dairy, livestock, mixed
        [Url] public string? DatasheetUrl { get; set; }
        public decimal? PriceZar { get; set; }                 // headline price for energy

        // Ownership
        [Required] public int FarmerId { get; set; }
        [ForeignKey(nameof(FarmerId))] public FarmerProfile FarmerProfile { get; set; }
    }
}
