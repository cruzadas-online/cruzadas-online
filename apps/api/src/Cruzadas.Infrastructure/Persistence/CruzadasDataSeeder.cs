using Cruzadas.Domain.Entities;
using Cruzadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cruzadas.Infrastructure.Persistence;

public static class CruzadasDataSeeder
{
    public static readonly Guid DefaultQuizId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public static async Task SeedAsync(CruzadasDbContext context, ILogger logger, CancellationToken cancellationToken = default)
    {
        var existingQuiz = await context.Quizzes
            .Include(q => q.Questions)
            .FirstOrDefaultAsync(q => q.Slug == "fundamentos-da-fe", cancellationToken);

        if (existingQuiz != null)
        {
            logger.LogInformation("Seed ignorado: Quiz 'fundamentos-da-fe' já cadastrado com {Count} questões.",
                existingQuiz.Questions.Count);
            return;
        }

        logger.LogInformation("Iniciando seed inicial do Cruzadas.online: Quiz Católico - Fundamentos da Fé.");

        var quiz = new Quiz(
            id: DefaultQuizId,
            title: "Quiz Católico — Fundamentos da Fé",
            slug: "fundamentos-da-fe",
            description: "Teste seus conhecimentos sobre a Sagrada Escritura, os Santos Sacramentos, doutrina, história da Igreja e a Santa Tradição.",
            isPublished: true,
            questionsPerAttempt: 10,
            createdAt: DateTimeOffset.UtcNow);

        // 1. Sacramentos
        var q1 = new Question(
            Guid.NewGuid(), quiz.Id,
            "Quantos são os sacramentos instituídos por Cristo e guardados pela Igreja Católica?",
            "A Igreja ensina que existem sete sacramentos: Batismo, Confirmação (Crisma), Eucaristia, Penitência (Reconciliação), Unção dos Enfermos, Ordem e Matrimônio.",
            "CIC 1113; Concílio de Trento",
            1);
        q1.AddOption(Guid.NewGuid(), "7 sacramentos", true, 1);
        q1.AddOption(Guid.NewGuid(), "5 sacramentos", false, 2);
        q1.AddOption(Guid.NewGuid(), "10 sacramentos", false, 3);
        q1.AddOption(Guid.NewGuid(), "3 sacramentos", false, 4);
        quiz.AddQuestion(q1);

        // 2. Bíblia / Cânon
        var q2 = new Question(
            Guid.NewGuid(), quiz.Id,
            "Quantos livros compõem o cânon bíblico católico completo (Antigo e Novo Testamento)?",
            "A Bíblia católica contém 73 livros: 46 no Antigo Testamento (incluindo os 7 livros deuterocanônicos) e 27 no Novo Testamento.",
            "CIC 120; Concílio de Trento (1546)",
            2);
        q2.AddOption(Guid.NewGuid(), "73 livros (46 no Antigo Testamento e 27 no Novo Testamento)", true, 1);
        q2.AddOption(Guid.NewGuid(), "66 livros (39 no Antigo Testamento e 27 no Novo Testamento)", false, 2);
        q2.AddOption(Guid.NewGuid(), "70 livros (43 no Antigo Testamento e 27 no Novo Testamento)", false, 3);
        q2.AddOption(Guid.NewGuid(), "80 livros (52 no Antigo Testamento e 28 no Novo Testamento)", false, 4);
        quiz.AddQuestion(q2);

        // 3. Eucaristia
        var q3 = new Question(
            Guid.NewGuid(), quiz.Id,
            "Qual termo teológico define a mudança de toda a substância do pão no Corpo de Cristo e do vinho no Seu Sangue na Santa Missa?",
            "Pela consagração opera-se a transubstanciação: toda a substância do pão se converte no Corpo de Cristo e toda a substância do vinho no Seu Sangue, permanecendo apenas as espécies (aparências).",
            "CIC 1376; Concílio de Trento (1551)",
            3);
        q3.AddOption(Guid.NewGuid(), "Transubstanciação", true, 1);
        q3.AddOption(Guid.NewGuid(), "Consubstanciação", false, 2);
        q3.AddOption(Guid.NewGuid(), "Transfinalização", false, 3);
        q3.AddOption(Guid.NewGuid(), "Simbolismo Memorial", false, 4);
        quiz.AddQuestion(q3);

        // 4. Vida de Cristo
        var q4 = new Question(
            Guid.NewGuid(), quiz.Id,
            "Em qual cidade Jesus Cristo nasceu, conforme narrado nos Evangelhos e nas profecias do Antigo Testamento?",
            "Jesus nasceu em Belém da Judeia, a cidade do rei Davi, cumprindo a profecia de Miqueias (Mq 5, 1). Ele cresceu e viveu grande parte de sua vida em Nazaré.",
            "Mt 2, 1; Lc 2, 4-7; Mq 5, 1",
            4);
        q4.AddOption(Guid.NewGuid(), "Belém da Judeia", true, 1);
        q4.AddOption(Guid.NewGuid(), "Nazaré da Galileia", false, 2);
        q4.AddOption(Guid.NewGuid(), "Jerusalém", false, 3);
        q4.AddOption(Guid.NewGuid(), "Cafarnaum", false, 4);
        quiz.AddQuestion(q4);

        // 5. Trindade
        var q5 = new Question(
            Guid.NewGuid(), quiz.Id,
            "O que a Igreja Católica professa solenemente sobre o mistério central da Santíssima Trindade?",
            "O Pai, o Filho e o Espírito Santo não são três deuses, mas um só Deus em três Pessoas divinas realmente distintas e consubstanciais.",
            "CIC 253-255; Credo Niceno-Constantinopolitano",
            5);
        q5.AddOption(Guid.NewGuid(), "Um só Deus em três Pessoas divinas, realmente distintas e consubstanciais", true, 1);
        q5.AddOption(Guid.NewGuid(), "Três deuses distintos unidos por um mesmo propósito moral", false, 2);
        q5.AddOption(Guid.NewGuid(), "Uma só Pessoa que se manifesta sob três modos ou nomes diferentes", false, 3);
        q5.AddOption(Guid.NewGuid(), "Deus Pai criou o Filho, e ambos produziram o Espírito Santo como energia", false, 4);
        quiz.AddQuestion(q5);

        // 6. Papa / Primado
        var q6 = new Question(
            Guid.NewGuid(), quiz.Id,
            "Quem foi constituído por Nosso Senhor Jesus Cristo como o Príncipe dos Apóstolos e primeiro Papa?",
            "Disse Jesus a Simão: 'Tu és Pedro, e sobre esta pedra edificarei a minha Igreja'. São Pedro é o primeiro bispo de Roma e vigário visível de Cristo na Terra.",
            "Mt 16, 18-19; Jo 21, 15-17; CIC 881",
            6);
        q6.AddOption(Guid.NewGuid(), "São Pedro Apóstolo", true, 1);
        q6.AddOption(Guid.NewGuid(), "São Paulo de Tarso", false, 2);
        q6.AddOption(Guid.NewGuid(), "São João Evangelista", false, 3);
        q6.AddOption(Guid.NewGuid(), "São Tiago Maior", false, 4);
        quiz.AddQuestion(q6);

        // 7. Nossa Senhora / Dogma
        var q7 = new Question(
            Guid.NewGuid(), quiz.Id,
            "O que afirma o dogma católico da Imaculada Conceição, proclamado pelo Beato Pio IX em 1854?",
            "Pela graça de Deus e em previsão dos méritos redentores de Cristo, a Bem-Aventurada Virgem Maria foi preservada de qualquer mancha do pecado original desde o primeiro instante de sua concepção.",
            "Bula Ineffabilis Deus (1854); CIC 491",
            7);
        q7.AddOption(Guid.NewGuid(), "Maria foi preservada imune de toda mancha do pecado original desde o primeiro instante de sua concepção", true, 1);
        q7.AddOption(Guid.NewGuid(), "Maria gerou Jesus virginalmente por obra do Espírito Santo", false, 2);
        q7.AddOption(Guid.NewGuid(), "Maria foi assunta de corpo e alma aos Céus ao término de sua vida terrena", false, 3);
        q7.AddOption(Guid.NewGuid(), "Maria nasceu sem pais humanos através de um milagre corporal", false, 4);
        quiz.AddQuestion(q7);

        // 8. História da Igreja / Concílio
        var q8 = new Question(
            Guid.NewGuid(), quiz.Id,
            "Em qual Concílio Ecumênico da Igreja Antiga (325 d.C.) foi formulado o Credo Niceno e condenada a heresia de Ário?",
            "O Primeiro Concílio de Niceia (325) definiu a plena divindade de Jesus Cristo, afirmando que Ele é 'consubstancial' (homoousios) ao Pai.",
            "História Eclesiástica; CIC 242, 465",
            8);
        q8.AddOption(Guid.NewGuid(), "Primeiro Concílio de Niceia", true, 1);
        q8.AddOption(Guid.NewGuid(), "Concílio de Calcedônia", false, 2);
        q8.AddOption(Guid.NewGuid(), "Concílio de Éfeso", false, 3);
        q8.AddOption(Guid.NewGuid(), "Concílio de Trento", false, 4);
        quiz.AddQuestion(q8);

        // 9. Tempo Litúrgico
        var q9 = new Question(
            Guid.NewGuid(), quiz.Id,
            "Qual é o tempo litúrgico penitencial de quarenta dias em que a Igreja se prepara em oração, jejum e caridade para a Páscoa?",
            "A Quaresma é o tempo favorável de quarenta dias que recorda o retiro de Jesus no deserto e prepara o coração dos fiéis para o Tríduo Pascal da Paixão, Morte e Ressurreição.",
            "CIC 540, 1438",
            9);
        q9.AddOption(Guid.NewGuid(), "Quaresma", true, 1);
        q9.AddOption(Guid.NewGuid(), "Advento", false, 2);
        q9.AddOption(Guid.NewGuid(), "Tempo Comum", false, 3);
        q9.AddOption(Guid.NewGuid(), "Tempo Pascal", false, 4);
        quiz.AddQuestion(q9);

        // 10. Primeiro Mandamento
        var q10 = new Question(
            Guid.NewGuid(), quiz.Id,
            "Qual é o Primeiro Mandamento da Lei de Deus, segundo a doutrina e o Catecismo da Igreja Católica?",
            "O primeiro mandamento chama o homem a crer em Deus, a esperar Nele e a amá-lo sobre todas as coisas, repudiando a idolatria e a superstição.",
            "Dt 6, 5; Mt 22, 37; CIC 2083-2084",
            10);
        q10.AddOption(Guid.NewGuid(), "Amar a Deus sobre todas as coisas", true, 1);
        q10.AddOption(Guid.NewGuid(), "Não tomar seu santo nome em vão", false, 2);
        q10.AddOption(Guid.NewGuid(), "Guardar domingos e festas de preceito", false, 3);
        q10.AddOption(Guid.NewGuid(), "Honrar pai e mãe", false, 4);
        quiz.AddQuestion(q10);

        // 11. Virtudes Teologais
        var q11 = new Question(
            Guid.NewGuid(), quiz.Id,
            "Quais são as três virtudes teologais infundidas diretamente por Deus na alma do cristão?",
            "As três virtudes teologais são Fé, Esperança e Caridade. Elas adaptam as faculdades humanas à participação na natureza divina.",
            "1Cor 13, 13; CIC 1812-1813",
            11);
        q11.AddOption(Guid.NewGuid(), "Fé, Esperança e Caridade", true, 1);
        q11.AddOption(Guid.NewGuid(), "Prudência, Justiça e Fortaleza", false, 2);
        q11.AddOption(Guid.NewGuid(), "Humildade, Paciência e Pureza", false, 3);
        q11.AddOption(Guid.NewGuid(), "Sabedoria, Piedade e Temor de Deus", false, 4);
        quiz.AddQuestion(q11);

        // 12. Evangelistas
        var q12 = new Question(
            Guid.NewGuid(), quiz.Id,
            "Quais são os quatro Evangelistas canônicos inspirados pelo Espírito Santo?",
            "Os quatro Evangelhos reconhecidos pela Tradição apostólica como inspirados foram escritos por São Mateus, São Marcos, São Lucas e São João.",
            "Dei Verbum 18; CIC 125-126",
            12);
        q12.AddOption(Guid.NewGuid(), "São Mateus, São Marcos, São Lucas e São João", true, 1);
        q12.AddOption(Guid.NewGuid(), "São Pedro, São Paulo, São João e São Tiago", false, 2);
        q12.AddOption(Guid.NewGuid(), "São Mateus, São Pedro, São Judas e São Tomé", false, 3);
        q12.AddOption(Guid.NewGuid(), "São Lucas, São Paulo, São Barnabé e Santo André", false, 4);
        quiz.AddQuestion(q12);

        // 13. Doutor da Igreja
        var q13 = new Question(
            Guid.NewGuid(), quiz.Id,
            "Qual teólogo dominicano medieval do século XIII é o autor da célebre 'Suma Teológica' e reconhecido como o Doutor Angélico?",
            "Santo Tomás de Aquino (1225-1274), sacerdote e frade da Ordem dos Pregadores, é um dos maiores pilares da teologia e da filosofia católica clássica.",
            "CIC 44; Encíclica Aeterni Patris",
            13);
        q13.AddOption(Guid.NewGuid(), "Santo Tomás de Aquino", true, 1);
        q13.AddOption(Guid.NewGuid(), "Santo Agostinho de Hipona", false, 2);
        q13.AddOption(Guid.NewGuid(), "São Boaventura de Bagnoregio", false, 3);
        q13.AddOption(Guid.NewGuid(), "São Francisco de Assis", false, 4);
        quiz.AddQuestion(q13);

        // 14. Caráter Sacramental
        var q14 = new Question(
            Guid.NewGuid(), quiz.Id,
            "Quais são os três sacramentos que imprimem caráter espiritual indelével e, portanto, nunca podem ser repetidos?",
            "O Batismo, a Confirmação (Crisma) e a Ordem imprimem um caráter sacramental indefectível na alma, selando o cristão para sempre.",
            "CIC 1121, 1272; Concílio de Trento",
            14);
        q14.AddOption(Guid.NewGuid(), "Batismo, Crisma (Confirmação) e Ordem", true, 1);
        q14.AddOption(Guid.NewGuid(), "Batismo, Eucaristia e Matrimônio", false, 2);
        q14.AddOption(Guid.NewGuid(), "Confissão, Unção dos Enfermos e Matrimônio", false, 3);
        q14.AddOption(Guid.NewGuid(), "Eucaristia, Crisma e Penitência", false, 4);
        quiz.AddQuestion(q14);

        // 15. Credo Apostólico
        var q15 = new Question(
            Guid.NewGuid(), quiz.Id,
            "O que é o Símbolo dos Apóstolos (Credo Apostólico)?",
            "É o resumo fiel da fé apostólica utilizado desde os primeiros séculos como o símbolo batismal da Igreja em Roma.",
            "CIC 194; Santo Ambrósio",
            15);
        q15.AddOption(Guid.NewGuid(), "O resumo fiel da fé apostólica e antigo símbolo batismal da Igreja de Roma", true, 1);
        q15.AddOption(Guid.NewGuid(), "Uma constituição pastoral escrita pelos bispos no Concílio de Trento", false, 2);
        q15.AddOption(Guid.NewGuid(), "Uma prece litúrgica redigida exclusivamente para os sacerdotes no confessionário", false, 3);
        q15.AddOption(Guid.NewGuid(), "Um conjunto de regras canônicas para a eleição do Sumo Pontífice", false, 4);
        quiz.AddQuestion(q15);

        context.Quizzes.Add(quiz);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Seed concluído com sucesso: Quiz '{Title}' com {Count} perguntas cadastradas.",
            quiz.Title, quiz.Questions.Count);
    }
}
