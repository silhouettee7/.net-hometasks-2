using Microsoft.EntityFrameworkCore;
using Migrator;

Console.WriteLine("Запуск миграций...");
try
{
    await using var context = new AppDbContext();
    Console.WriteLine("-----Применение миграций-----");
    context.Database.Migrate();
    Console.WriteLine("-----Миграции успешно применены!-----");
    await Task.Delay(5000);
    await DatabaseSeeder.SeedAsync(context);
    Console.WriteLine("-----Данные заполнены-----");
}
catch (Exception ex)
{
    Console.WriteLine($"Ошибка при применении миграций: {ex.Message}");
    Console.WriteLine(ex.StackTrace);
    Environment.Exit(1);
}