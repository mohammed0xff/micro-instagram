namespace NotificationService.DBContext.Seed
{
    public static class DatabaseConfig
    {
        public static async Task ConfigureDatabaseAsync(this IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                await EnsureCreated(serviceScope);
            }
        }

        public static async Task EnsureCreated(IServiceScope serviceScope)
        {
            var context = serviceScope.ServiceProvider.GetService<AppDbContext>();

            if (context == null) { throw new ArgumentNullException(nameof(context)); }

            context.Database.EnsureCreated();
        }

        public static async Task SeedDataAsync(IServiceScope serviceScope) { }
    }
}