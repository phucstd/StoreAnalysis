using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StoreAnalysis.Models;

namespace StoreAnalysis.Data;

public class StoreAnalysisContext : IdentityDbContext<IdentityUser>
{

    public StoreAnalysisContext(DbContextOptions<StoreAnalysisContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
    public DbSet<Slot> Slots { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<ItemStorage> ItemsStorage { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    public List<ItemStorage> GetItemsOnSlot(int slotId)
    {

        var itemIds = Items.Where(_ => _.SlotID.Equals(slotId)).Select(_ => _.ItemId);
        List<ItemStorage> items = new List<ItemStorage>();
        foreach (var id in itemIds)
        {
            items.Add(ItemsStorage.FirstOrDefault(_ => _.Id.Equals(id)));
        }
        return items;
    }
}
