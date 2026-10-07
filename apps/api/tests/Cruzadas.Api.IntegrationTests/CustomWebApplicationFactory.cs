using Cruzadas.Domain.Entities;
using Cruzadas.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Cruzadas.Api.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private SqliteConnection? _masterConnection;
    private readonly string _dbName = $"cruzadas_test_{Guid.NewGuid():N}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var connectionString = $"DataSource=file:{_dbName}?mode=memory&cache=shared";

        // Keep master connection open to maintain in-memory SQLite database
        _masterConnection = new SqliteConnection(connectionString);
        _masterConnection.Open();

        builder.UseSetting("Database:Provider", "Sqlite");
        builder.UseSetting("Database:AutoMigrate", "false");
        builder.UseSetting("Database:AutoSeed", "false");
        builder.UseSetting("ConnectionStrings:DefaultConnection", connectionString);
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CruzadasDbContext>();
        db.Database.EnsureCreated();
        SeedIntegrationData(db);

        return host;
    }

    private static void SeedIntegrationData(CruzadasDbContext db)
    {
        if (db.Quizzes.Any()) return;

        // 1. Published Quiz
        var publishedQuiz = new Quiz(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            "Quiz Católico — Fundamentos da Fé",
            "fundamentos-da-fe",
            "Quiz teste de integração",
            true,
            2,
            DateTimeOffset.UtcNow);

        var q1 = new Question(
            Guid.Parse("22222222-2222-2222-2222-222222222221"),
            publishedQuiz.Id,
            "Quantos são os sacramentos?",
            "Sete sacramentos instituídos por Cristo.",
            "CIC 1113",
            1);
        q1.AddOption(Guid.Parse("33333333-3333-3333-3333-333333333331"), "7 sacramentos", true, 1);
        q1.AddOption(Guid.Parse("33333333-3333-3333-3333-333333333332"), "5 sacramentos", false, 2);
        publishedQuiz.AddQuestion(q1);

        var q2 = new Question(
            Guid.Parse("22222222-2222-2222-2222-222222222222"),
            publishedQuiz.Id,
            "Quantos livros compõem a Bíblia católica?",
            "73 livros no total.",
            "CIC 120",
            2);
        q2.AddOption(Guid.Parse("33333333-3333-3333-3333-333333333333"), "73 livros", true, 1);
        q2.AddOption(Guid.Parse("33333333-3333-3333-3333-333333333334"), "66 livros", false, 2);
        publishedQuiz.AddQuestion(q2);

        // 2. Unpublished Quiz
        var draftQuiz = new Quiz(
            Guid.Parse("99999999-9999-9999-9999-999999999999"),
            "Quiz em Rascunho",
            "quiz-rascunho",
            "Ainda não publicado",
            false,
            2,
            DateTimeOffset.UtcNow);

        db.Quizzes.AddRange(publishedQuiz, draftQuiz);
        db.SaveChanges();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _masterConnection?.Close();
        _masterConnection?.Dispose();
    }
}
