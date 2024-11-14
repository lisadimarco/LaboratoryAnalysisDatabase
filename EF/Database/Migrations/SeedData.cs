public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider, LabDbContext context)
    {
        if (!context.LabTests.Any())
        {
            context.LabTests.AddRange(
                new LabTest { TestName = "Emocromo", ReferenceMin = 12.5, ReferenceMax = 16.5 }
                // Aggiungi altri test
            );
            context.SaveChanges();
        }
    }
}
