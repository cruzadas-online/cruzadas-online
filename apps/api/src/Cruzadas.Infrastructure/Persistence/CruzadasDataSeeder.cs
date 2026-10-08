using Cruzadas.Domain.Entities;
using Cruzadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cruzadas.Infrastructure.Persistence;

public static class CruzadasDataSeeder
{
    public static readonly Guid DefaultQuizId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public static readonly Guid GroupDoutrinaId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    public static readonly Guid GroupBibliaId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
    public static readonly Guid GroupHistoriaId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
    public static readonly Guid GroupLiturgiaId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");

    public static readonly Guid QuizBibliaId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid QuizHistoriaId = Guid.Parse("33333333-3333-3333-3333-333333333333");
    public static readonly Guid QuizLiturgiaId = Guid.Parse("44444444-4444-4444-4444-444444444444");
    public static readonly Guid QuizCruzadasDefesaId = Guid.Parse("55555555-5555-5555-5555-555555555555");
    public static readonly Guid QuizOrdemDeCristoId = Guid.Parse("66666666-6666-6666-6666-666666666666");
    public static readonly Guid QuizCavalariaBatalhasId = Guid.Parse("77777777-7777-7777-7777-777777777777");

    public static async Task SeedAsync(CruzadasDbContext context, ILogger logger, CancellationToken cancellationToken = default)
    {
        // 1. Seed dos Grupos de Quiz
        var existingGroups = await context.QuizGroups.ToDictionaryAsync(g => g.Slug, cancellationToken);

        if (!existingGroups.ContainsKey("doutrina-sacramentos"))
        {
            var g1 = new QuizGroup(
                GroupDoutrinaId,
                "Doutrina e Sacramentos",
                "doutrina-sacramentos",
                "Fundamentos da fé apostólica, os santos sacramentos e o Catecismo da Igreja Católica.",
                "church",
                1);
            context.QuizGroups.Add(g1);
            existingGroups["doutrina-sacramentos"] = g1;
        }

        if (!existingGroups.ContainsKey("sagradas-escrituras"))
        {
            var g2 = new QuizGroup(
                GroupBibliaId,
                "Sagradas Escrituras",
                "sagradas-escrituras",
                "O cânon bíblico católico, os Santos Evangelhos, os patriarcas e a revelação divina.",
                "book",
                2);
            context.QuizGroups.Add(g2);
            existingGroups["sagradas-escrituras"] = g2;
        }

        if (!existingGroups.ContainsKey("historia-e-tradicao"))
        {
            var g3 = new QuizGroup(
                GroupHistoriaId,
                "História e Tradição",
                "historia-e-tradicao",
                "Concílios ecumênicos, santos doutores, primeiros mártires e a história da Cristandade.",
                "shield",
                3);
            context.QuizGroups.Add(g3);
            existingGroups["historia-e-tradicao"] = g3;
        }

        if (!existingGroups.ContainsKey("liturgia-espiritualidade"))
        {
            var g4 = new QuizGroup(
                GroupLiturgiaId,
                "Liturgia e Oração",
                "liturgia-espiritualidade",
                "O Santo Sacrifício da Missa, o ciclo litúrgico, orações católicas e virtudes cristãs.",
                "flame",
                4);
            context.QuizGroups.Add(g4);
            existingGroups["liturgia-espiritualidade"] = g4;
        }

        await context.SaveChangesAsync(cancellationToken);

        // 2. Quiz 1: Fundamentos da Fé (Doutrina)
        var existingFundamentos = await context.Quizzes
            .Include(q => q.Questions)
            .FirstOrDefaultAsync(q => q.Slug == "fundamentos-da-fe", cancellationToken);

        if (existingFundamentos == null)
        {
            logger.LogInformation("Criando quiz inicial: 'Quiz Católico — Fundamentos da Fé'.");

            var quiz1 = new Quiz(
                id: DefaultQuizId,
                title: "Quiz Católico — Fundamentos da Fé",
                slug: "fundamentos-da-fe",
                description: "Teste seus conhecimentos sobre a Sagrada Escritura, os Santos Sacramentos, doutrina e a Santa Tradição.",
                isPublished: true,
                questionsPerAttempt: 10,
                createdAt: DateTimeOffset.UtcNow,
                groupId: GroupDoutrinaId,
                difficultyLevel: "Iniciante");

            SeedFundamentosQuestions(quiz1);
            context.Quizzes.Add(quiz1);
        }
        else
        {
            if (existingFundamentos.GroupId == null)
            {
                existingFundamentos.AssignToGroup(GroupDoutrinaId);
            }
        }

        // 3. Quiz 2: Sagradas Escrituras
        var existingBiblia = await context.Quizzes
            .Include(q => q.Questions)
            .FirstOrDefaultAsync(q => q.Slug == "evangelhos-e-biblia", cancellationToken);

        if (existingBiblia == null)
        {
            logger.LogInformation("Criando quiz: 'Os Evangelhos e o Cânon Bíblico'.");

            var quiz2 = new Quiz(
                id: QuizBibliaId,
                title: "Os Evangelhos e o Cânon Bíblico",
                slug: "evangelhos-e-biblia",
                description: "Perguntas sobre os quatro Evangelistas, a Revelação Divina e a formação das Escrituras Sagradas.",
                isPublished: true,
                questionsPerAttempt: 10,
                createdAt: DateTimeOffset.UtcNow,
                groupId: GroupBibliaId,
                difficultyLevel: "Intermediário");

            SeedBibliaQuestions(quiz2);
            context.Quizzes.Add(quiz2);
        }

        // 4. Quiz 3: Grandes Concílios e Santos Mártires
        var existingHistoria = await context.Quizzes
            .Include(q => q.Questions)
            .FirstOrDefaultAsync(q => q.Slug == "concilios-e-santos", cancellationToken);

        if (existingHistoria == null)
        {
            logger.LogInformation("Criando quiz: 'Grandes Concílios e Santos Mártires'.");

            var quiz3 = new Quiz(
                id: QuizHistoriaId,
                title: "Grandes Concílios e Santos Mártires",
                slug: "concilios-e-santos",
                description: "Desafie seu conhecimento sobre os dogmas definidos nos concílios e a vida dos heróis da fé cristã.",
                isPublished: true,
                questionsPerAttempt: 10,
                createdAt: DateTimeOffset.UtcNow,
                groupId: GroupHistoriaId,
                difficultyLevel: "Avançado");

            SeedHistoriaQuestions(quiz3);
            context.Quizzes.Add(quiz3);
        }

        // 5. Quiz 4: O Santo Sacrifício da Missa
        var existingLiturgia = await context.Quizzes
            .Include(q => q.Questions)
            .FirstOrDefaultAsync(q => q.Slug == "santa-missa-e-liturgia", cancellationToken);

        if (existingLiturgia == null)
        {
            logger.LogInformation("Criando quiz: 'O Santo Sacrifício da Missa'.");

            var quiz4 = new Quiz(
                id: QuizLiturgiaId,
                title: "O Santo Sacrifício da Missa",
                slug: "santa-missa-e-liturgia",
                description: "Compreensão da celebração eucarística, paramentos, tempos litúrgicos e sua riqueza espiritual.",
                isPublished: true,
                questionsPerAttempt: 10,
                createdAt: DateTimeOffset.UtcNow,
                groupId: GroupLiturgiaId,
                difficultyLevel: "Iniciante");

            SeedLiturgiaQuestions(quiz4);
            context.Quizzes.Add(quiz4);
        }

        // 6. Quiz 5: As Cruzadas e a Defesa da Cristandade (História - Iniciante)
        var existingCruzadas = await context.Quizzes
            .Include(q => q.Questions)
            .FirstOrDefaultAsync(q => q.Slug == "cruzadas-e-cristandade", cancellationToken);

        if (existingCruzadas == null)
        {
            logger.LogInformation("Criando quiz: 'As Cruzadas e a Defesa da Cristandade'.");

            var quiz5 = new Quiz(
                id: QuizCruzadasDefesaId,
                title: "As Cruzadas e a Defesa da Cristandade",
                slug: "cruzadas-e-cristandade",
                description: "Os fundamentos históricos e espirituais das Cruzadas, o apelo de Urbano II, a defesa dos peregrinos e a libertação do Santo Sepulcro.",
                isPublished: true,
                questionsPerAttempt: 10,
                createdAt: DateTimeOffset.UtcNow,
                groupId: GroupHistoriaId,
                difficultyLevel: "Iniciante");

            SeedCruzadasDefesaQuestions(quiz5);
            context.Quizzes.Add(quiz5);
        }

        // 7. Quiz 6: A Ordem de Cristo e as Grandes Navegações (História - Intermediário)
        var existingOrdemCristo = await context.Quizzes
            .Include(q => q.Questions)
            .FirstOrDefaultAsync(q => q.Slug == "ordem-de-cristo-e-navegacoes", cancellationToken);

        if (existingOrdemCristo == null)
        {
            logger.LogInformation("Criando quiz: 'A Ordem de Cristo e as Grandes Navegações'.");

            var quiz6 = new Quiz(
                id: QuizOrdemDeCristoId,
                title: "A Ordem de Cristo e as Grandes Navegações",
                slug: "ordem-de-cristo-e-navegacoes",
                description: "Da transição dos Templários à Ordem de Cristo em Portugal com D. Dinis e o Papa João XXII, até a expansão da Fé nas caravelas do Infante D. Henrique.",
                isPublished: true,
                questionsPerAttempt: 10,
                createdAt: DateTimeOffset.UtcNow,
                groupId: GroupHistoriaId,
                difficultyLevel: "Intermediário");

            SeedOrdemDeCristoQuestions(quiz6);
            context.Quizzes.Add(quiz6);
        }

        // 8. Quiz 7: Tradição de Cavalaria e Batalhas Decisivas da Cristandade (História - Avançado)
        var existingCavalaria = await context.Quizzes
            .Include(q => q.Questions)
            .FirstOrDefaultAsync(q => q.Slug == "cavalaria-e-batalhas-da-cristandade", cancellationToken);

        if (existingCavalaria == null)
        {
            logger.LogInformation("Criando quiz: 'Tradição de Cavalaria e Batalhas Decisivas da Cristandade'.");

            var quiz7 = new Quiz(
                id: QuizCavalariaBatalhasId,
                title: "Tradição de Cavalaria e Batalhas Decisivas da Cristandade",
                slug: "cavalaria-e-batalhas-da-cristandade",
                description: "Análise profunda dos tratados de cavalaria cristã, bulas pontifícias, mestres templários e confrontos milenares que preservaram o Ocidente cristão.",
                isPublished: true,
                questionsPerAttempt: 10,
                createdAt: DateTimeOffset.UtcNow,
                groupId: GroupHistoriaId,
                difficultyLevel: "Avançado");

            SeedCavalariaBatalhasQuestions(quiz7);
            context.Quizzes.Add(quiz7);
        }

        await context.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Seed de grupos e quizzes concluído com sucesso.");
    }

    private static void SeedFundamentosQuestions(Quiz quiz)
    {
        var q1 = new Question(Guid.NewGuid(), quiz.Id,
            "Quantos são os sacramentos instituídos por Cristo e guardados pela Igreja Católica?",
            "A Igreja ensina que existem sete sacramentos: Batismo, Confirmação (Crisma), Eucaristia, Penitência (Reconciliação), Unção dos Enfermos, Ordem e Matrimônio.",
            "CIC 1113; Concílio de Trento", 1);
        q1.AddOption(Guid.NewGuid(), "7 sacramentos", true, 1);
        q1.AddOption(Guid.NewGuid(), "5 sacramentos", false, 2);
        q1.AddOption(Guid.NewGuid(), "10 sacramentos", false, 3);
        q1.AddOption(Guid.NewGuid(), "3 sacramentos", false, 4);
        quiz.AddQuestion(q1);

        var q2 = new Question(Guid.NewGuid(), quiz.Id,
            "Quantos livros compõem o cânon bíblico católico completo (Antigo e Novo Testamento)?",
            "A Bíblia católica contém 73 livros: 46 no Antigo Testamento (incluindo os 7 deuterocanônicos) e 27 no Novo Testamento.",
            "CIC 120; Concílio de Trento (1546)", 2);
        q2.AddOption(Guid.NewGuid(), "73 livros (46 no Antigo Testamento e 27 no Novo Testamento)", true, 1);
        q2.AddOption(Guid.NewGuid(), "66 livros (39 no Antigo Testamento e 27 no Novo Testamento)", false, 2);
        q2.AddOption(Guid.NewGuid(), "70 livros (43 no Antigo Testamento e 27 no Novo Testamento)", false, 3);
        q2.AddOption(Guid.NewGuid(), "80 livros (52 no Antigo Testamento e 28 no Novo Testamento)", false, 4);
        quiz.AddQuestion(q2);

        var q3 = new Question(Guid.NewGuid(), quiz.Id,
            "Qual termo teológico define a mudança de toda a substância do pão no Corpo de Cristo e do vinho no Seu Sangue na Santa Missa?",
            "Pela consagração opera-se a transubstanciação: toda a substância do pão se converte no Corpo de Cristo e a do vinho no Seu Sangue.",
            "CIC 1376; Concílio de Trento", 3);
        q3.AddOption(Guid.NewGuid(), "Transubstanciação", true, 1);
        q3.AddOption(Guid.NewGuid(), "Consubstanciação", false, 2);
        q3.AddOption(Guid.NewGuid(), "Transfinalização", false, 3);
        q3.AddOption(Guid.NewGuid(), "Simbolismo Memorial", false, 4);
        quiz.AddQuestion(q3);

        var q4 = new Question(Guid.NewGuid(), quiz.Id,
            "Em qual acontecimento bíblico o Espírito Santo desceu visivelmente sobre Maria e os Apóstolos reunidos no Cenáculo?",
            "No dia de Pentecostes, cinquenta dias após a Páscoa, a plenitude do Espírito Santo foi manifestada na Igreja nascente.",
            "At 2, 1-4; CIC 731", 4);
        q4.AddOption(Guid.NewGuid(), "Pentecostes", true, 1);
        q4.AddOption(Guid.NewGuid(), "Ascensão do Senhor", false, 2);
        q4.AddOption(Guid.NewGuid(), "Transfiguração", false, 3);
        q4.AddOption(Guid.NewGuid(), "Anunciação do Anjo", false, 4);
        quiz.AddQuestion(q4);

        var q5 = new Question(Guid.NewGuid(), quiz.Id,
            "Quais são as três virtudes teologais infundidas por Deus na alma humana?",
            "As virtudes teologais têm Deus mesmo como origem e fim. São elas: Fé, Esperança e Caridade.",
            "1 Cor 13, 13; CIC 1812-1813", 5);
        q5.AddOption(Guid.NewGuid(), "Fé, Esperança e Caridade", true, 1);
        q5.AddOption(Guid.NewGuid(), "Prudência, Justiça e Fortaleza", false, 2);
        q5.AddOption(Guid.NewGuid(), "Castidade, Humildade e Obediência", false, 3);
        q5.AddOption(Guid.NewGuid(), "Pobreza, Mansidão e Paciência", false, 4);
        quiz.AddQuestion(q5);

        var q6 = new Question(Guid.NewGuid(), quiz.Id,
            "Qual é o dogma mariano proclamado em 1854 pelo Papa Pio IX através da bula Ineffabilis Deus?",
            "A Bem-Aventurada Virgem Maria foi preservada imune de toda mancha do pecado original desde o primeiro instante de sua concepção.",
            "Bula Ineffabilis Deus; CIC 491", 6);
        q6.AddOption(Guid.NewGuid(), "Imaculada Conceição", true, 1);
        q6.AddOption(Guid.NewGuid(), "Assunção aos Céus", false, 2);
        q6.AddOption(Guid.NewGuid(), "Maternidade Divina (Theotokos)", false, 3);
        q6.AddOption(Guid.NewGuid(), "Virgindade Perpétua", false, 4);
        quiz.AddQuestion(q6);

        var q7 = new Question(Guid.NewGuid(), quiz.Id,
            "Quais são as quatro marcas ou notas essenciais da Igreja professadas no Credo Niceno-Constantinopolitano?",
            "No Credo professamos: 'Creio na Igreja, Una, Santa, Católica e Apostólica'.",
            "CIC 811", 7);
        q7.AddOption(Guid.NewGuid(), "Una, Santa, Católica e Apostólica", true, 1);
        q7.AddOption(Guid.NewGuid(), "Universal, Romana, Histórica e Missionária", false, 2);
        q7.AddOption(Guid.NewGuid(), "Bíblica, Tradicional, Espiritual e Escatológica", false, 3);
        q7.AddOption(Guid.NewGuid(), "Hierárquica, Sacramental, Global e Eterna", false, 4);
        quiz.AddQuestion(q7);

        var q8 = new Question(Guid.NewGuid(), quiz.Id,
            "Em qual Concílio Ecumênico da Igreja (325 d.C.) foi proclamado que o Filho é 'consubstancial ao Pai' (Homoousios)?",
            "O Primeiro Concílio de Nicéia (325) refutou a heresia de Ário e redigiu a base do Credo Niceno.",
            "Concílio de Nicéia I; CIC 242, 465", 8);
        q8.AddOption(Guid.NewGuid(), "Primeiro Concílio de Nicéia", true, 1);
        q8.AddOption(Guid.NewGuid(), "Concílio de Éfeso", false, 2);
        q8.AddOption(Guid.NewGuid(), "Concílio de Trento", false, 3);
        q8.AddOption(Guid.NewGuid(), "Concílio Vaticano I", false, 4);
        quiz.AddQuestion(q8);

        var q9 = new Question(Guid.NewGuid(), quiz.Id,
            "Qual é o primeiro e maior de todos os Mandamentos, ensinado por Jesus nos Evangelhos?",
            "Jesus respondeu: 'Amarás o Senhor teu Deus de todo o teu coração, de toda a tua alma e de todo o teu entendimento'.",
            "Mt 22, 37-38; Dt 6, 5; CIC 2055", 9);
        q9.AddOption(Guid.NewGuid(), "Amar a Deus sobre todas as coisas", true, 1);
        q9.AddOption(Guid.NewGuid(), "Não matar", false, 2);
        q9.AddOption(Guid.NewGuid(), "Guardar domingos e festas de guarda", false, 3);
        q9.AddOption(Guid.NewGuid(), "Honrar pai e mãe", false, 4);
        quiz.AddQuestion(q9);

        var q10 = new Question(Guid.NewGuid(), quiz.Id,
            "Quem foi o apóstolo escolhido por Jesus para ser a pedra visível sobre a qual edificaria a Sua Igreja?",
            "Disse Jesus: 'Tu és Pedro, e sobre esta pedra edificarei a minha Igreja'.",
            "Mt 16, 18; CIC 552", 10);
        q10.AddOption(Guid.NewGuid(), "São Pedro (Simão Barjonas)", true, 1);
        q10.AddOption(Guid.NewGuid(), "São Paulo de Tarso", false, 2);
        q10.AddOption(Guid.NewGuid(), "São João Evangelista", false, 3);
        q10.AddOption(Guid.NewGuid(), "São Tiago Maior", false, 4);
        quiz.AddQuestion(q10);
    }

    private static void SeedBibliaQuestions(Quiz quiz)
    {
        var q1 = new Question(Guid.NewGuid(), quiz.Id,
            "Quais são os quatro Evangelhos canônicos do Novo Testamento?",
            "Os quatro Evangelhos são segundo Mateus, Marcos, Lucas e João.",
            "CIC 125", 1);
        q1.AddOption(Guid.NewGuid(), "Mateus, Marcos, Lucas e João", true, 1);
        q1.AddOption(Guid.NewGuid(), "Mateus, Pedro, Tiago e Paulo", false, 2);
        q1.AddOption(Guid.NewGuid(), "Lucas, João, Tomé e Barnabé", false, 3);
        q1.AddOption(Guid.NewGuid(), "Marcos, Lucas, Paulo e Judas", false, 4);
        quiz.AddQuestion(q1);

        var q2 = new Question(Guid.NewGuid(), quiz.Id,
            "Qual evangelista também é o autor inspirado do livro dos Atos dos Apóstolos?",
            "São Lucas, companheiro de São Paulo, escreveu tanto o terceiro Evangelho quanto os Atos dos Apóstolos.",
            "At 1, 1; Lc 1, 1-4", 2);
        q2.AddOption(Guid.NewGuid(), "São Lucas", true, 1);
        q2.AddOption(Guid.NewGuid(), "São Marcos", false, 2);
        q2.AddOption(Guid.NewGuid(), "São João", false, 3);
        q2.AddOption(Guid.NewGuid(), "São Mateus", false, 4);
        quiz.AddQuestion(q2);

        var q3 = new Question(Guid.NewGuid(), quiz.Id,
            "Quais livros compõem o Pentateuco (Torá), os cinco primeiros livros da Bíblia?",
            "O Pentateuco é formado por Gênesis, Êxodo, Levítico, Números e Deuteronômio.",
            "CIC 120", 3);
        q3.AddOption(Guid.NewGuid(), "Gênesis, Êxodo, Levítico, Números e Deuteronômio", true, 1);
        q3.AddOption(Guid.NewGuid(), "Gênesis, Êxodo, Josué, Juízes e Reis", false, 2);
        q3.AddOption(Guid.NewGuid(), "Êxodo, Salmos, Provérbios, Isaías e Daniel", false, 3);
        q3.AddOption(Guid.NewGuid(), "Gênesis, Levítico, Tobias, Judite e Macabeus", false, 4);
        quiz.AddQuestion(q3);

        var q4 = new Question(Guid.NewGuid(), quiz.Id,
            "Quem é considerado o autor humano da maior parte dos Salmos bíblicos?",
            "A tradição bíblica atribui a maior coleção dos Salmos ao Rei Davi.",
            "CIC 2579; 1 Sm 16, 16-23", 4);
        q4.AddOption(Guid.NewGuid(), "Rei Davi", true, 1);
        q4.AddOption(Guid.NewGuid(), "Rei Salomão", false, 2);
        q4.AddOption(Guid.NewGuid(), "Moisés", false, 3);
        q4.AddOption(Guid.NewGuid(), "Profeta Isaías", false, 4);
        quiz.AddQuestion(q4);

        var q5 = new Question(Guid.NewGuid(), quiz.Id,
            "Qual o último livro da Bíblia católica, de teor profético e revelador?",
            "O Apocalipse (ou Revelação), escrito por São João Evangelista na ilha de Patmos.",
            "Ap 1, 1; CIC 120", 5);
        q5.AddOption(Guid.NewGuid(), "Apocalipse de São João", true, 1);
        q5.AddOption(Guid.NewGuid(), "Carta aos Hebreus", false, 2);
        q5.AddOption(Guid.NewGuid(), "Epístola de São Judas", false, 3);
        q5.AddOption(Guid.NewGuid(), "Profecia de Malaquias", false, 4);
        quiz.AddQuestion(q5);

        var q6 = new Question(Guid.NewGuid(), quiz.Id,
            "O que significa o termo 'Vulgata' na história das Sagradas Escrituras?",
            "A tradução da Bíblia para o latim comum realizada por São Jerônimo a pedido do Papa Dâmaso I no século IV.",
            "CIC 120; Concílio de Trento", 6);
        q6.AddOption(Guid.NewGuid(), "A tradução latina da Bíblia feita por São Jerônimo", true, 1);
        q6.AddOption(Guid.NewGuid(), "A versão grega do Antigo Testamento chamada Septuaginta", false, 2);
        q6.AddOption(Guid.NewGuid(), "Um manuscrito aramaico dos Manuscritos do Mar Morto", false, 3);
        q6.AddOption(Guid.NewGuid(), "Uma cópia litúrgica dos Evangelhos em hebraico", false, 4);
        quiz.AddQuestion(q6);

        var q7 = new Question(Guid.NewGuid(), quiz.Id,
            "Qual apóstolo escreveu a famosa passagem sobre a Fé sem obras estar morta?",
            "São Tiago em sua epístola canônica: 'Assim como o corpo sem o espírito é morto, assim também a fé sem obras é morta'.",
            "Tg 2, 26; CIC 1815", 7);
        q7.AddOption(Guid.NewGuid(), "São Tiago Menor", true, 1);
        q7.AddOption(Guid.NewGuid(), "São Pedro", false, 2);
        q7.AddOption(Guid.NewGuid(), "São Paulo", false, 3);
        q7.AddOption(Guid.NewGuid(), "São Tomé", false, 4);
        quiz.AddQuestion(q7);

        var q8 = new Question(Guid.NewGuid(), quiz.Id,
            "Qual profeta do Antigo Testamento profetizou com precisão os sofrimentos do 'Servo Sofredor' messiânico?",
            "O profeta Isaías no capítulo 53 descreve com impressionante fidelidade a Paixão de Cristo.",
            "Is 53; CIC 601", 8);
        q8.AddOption(Guid.NewGuid(), "Isaías", true, 1);
        q8.AddOption(Guid.NewGuid(), "Jeremias", false, 2);
        q8.AddOption(Guid.NewGuid(), "Ezequiel", false, 3);
        q8.AddOption(Guid.NewGuid(), "Daniel", false, 4);
        quiz.AddQuestion(q8);

        var q9 = new Question(Guid.NewGuid(), quiz.Id,
            "Quantas são as cartas atribuídas tradicionalmente a São Paulo no Novo Testamento?",
            "O corpus paulino tradicionalmente reconhecido compreende 14 cartas (incluindo Hebreus).",
            "CIC 120", 9);
        q9.AddOption(Guid.NewGuid(), "14 cartas", true, 1);
        q9.AddOption(Guid.NewGuid(), "7 cartas", false, 2);
        q9.AddOption(Guid.NewGuid(), "10 cartas", false, 3);
        q9.AddOption(Guid.NewGuid(), "12 cartas", false, 4);
        quiz.AddQuestion(q9);

        var q10 = new Question(Guid.NewGuid(), quiz.Id,
            "Quem é considerado o 'Pai da Fé' tanto no Antigo quanto no Novo Testamento?",
            "Abraão, a quem Deus prometeu uma posteridade tão numerosa quanto as estrelas dos céus.",
            "Gn 15, 5-6; Rm 4, 11; CIC 145", 10);
        q10.AddOption(Guid.NewGuid(), "Abraão", true, 1);
        q10.AddOption(Guid.NewGuid(), "Moisés", false, 2);
        q10.AddOption(Guid.NewGuid(), "Jacó (Israel)", false, 3);
        q10.AddOption(Guid.NewGuid(), "Noé", false, 4);
        quiz.AddQuestion(q10);
    }

    private static void SeedHistoriaQuestions(Quiz quiz)
    {
        var q1 = new Question(Guid.NewGuid(), quiz.Id,
            "Qual Concílio Ecumênico da Igreja proclamou Maria Santíssima como 'Theotokos' (Mãe de Deus)?",
            "O Concílio de Éfeso (431 d.C.) condenou o nestorianismo e definiu que Maria é verdadeiramente Mãe de Deus.",
            "Concílio de Éfeso (431); CIC 466", 1);
        q1.AddOption(Guid.NewGuid(), "Concílio de Éfeso (431 d.C.)", true, 1);
        q1.AddOption(Guid.NewGuid(), "Concílio de Trento (1545)", false, 2);
        q1.AddOption(Guid.NewGuid(), "Concílio de Constantinopla I (381)", false, 3);
        q1.AddOption(Guid.NewGuid(), "Concílio Vaticano II (1962)", false, 4);
        quiz.AddQuestion(q1);

        var q2 = new Question(Guid.NewGuid(), quiz.Id,
            "Qual santo bispo de Hipona do século IV e V escreveu 'As Confissões' e 'A Cidade de Deus'?",
            "Santo Agostinho de Hipona (354-430), um dos maiores Doutores e Padres da Igreja latina.",
            "CIC 2560", 2);
        q2.AddOption(Guid.NewGuid(), "Santo Agostinho de Hipona", true, 1);
        q2.AddOption(Guid.NewGuid(), "Santo Tomás de Aquino", false, 2);
        q2.AddOption(Guid.NewGuid(), "São Bento de Núrsia", false, 3);
        q2.AddOption(Guid.NewGuid(), "São Gregório Magno", false, 4);
        quiz.AddQuestion(q2);

        var q3 = new Question(Guid.NewGuid(), quiz.Id,
            "Qual diácono e mártir romano do século III, ao ser ordenado a entregar os tesouros da Igreja, apresentou os pobres e enfermos?",
            "São Lourenço mártir (258 d.C.), queimado vivo sobre uma grelha em Roma.",
            "CIC 2444; Santo Ambrósio", 3);
        q3.AddOption(Guid.NewGuid(), "São Lourenço", true, 1);
        q3.AddOption(Guid.NewGuid(), "Santo Estêvão", false, 2);
        q3.AddOption(Guid.NewGuid(), "São Sebastião", false, 3);
        q3.AddOption(Guid.NewGuid(), "São Pancrácio", false, 4);
        quiz.AddQuestion(q3);

        var q4 = new Question(Guid.NewGuid(), quiz.Id,
            "Qual santo do século XIII fundou a Ordem dos Frades Menores, viveu a radicalidade da pobreza e recebeu os estigmas de Cristo?",
            "São Francisco de Assis (1181-1226), o Pobrezinho de Assis.",
            "História Franciscana; Celano", 4);
        q4.AddOption(Guid.NewGuid(), "São Francisco de Assis", true, 1);
        q4.AddOption(Guid.NewGuid(), "São Domingos de Gusmão", false, 2);
        q4.AddOption(Guid.NewGuid(), "Santo Inácio de Loyola", false, 3);
        q4.AddOption(Guid.NewGuid(), "São Filipe Néri", false, 4);
        quiz.AddQuestion(q4);

        var q5 = new Question(Guid.NewGuid(), quiz.Id,
            "Qual imperador romano promulgou o Edito de Milão em 313 d.C., concedendo liberdade de culto aos cristãos?",
            "O imperador Constantino Magno (junto com Licínio) concedeu liberdade religiosa ao Império.",
            "História Eclesiástica; Eusébio de Cesaréia", 5);
        q5.AddOption(Guid.NewGuid(), "Constantino Magno", true, 1);
        q5.AddOption(Guid.NewGuid(), "Nero", false, 2);
        q5.AddOption(Guid.NewGuid(), "Diocleciano", false, 3);
        q5.AddOption(Guid.NewGuid(), "Marco Aurélio", false, 4);
        quiz.AddQuestion(q5);

        var q6 = new Question(Guid.NewGuid(), quiz.Id,
            "Quem é proclamado o 'Pai dos Monges Ocidentais', autor da célebre Santa Regra (Ora et Labora)?",
            "São Bento de Núrsia (480-547), padroeiro da Europa e fundador da Abadia de Monte Cassino.",
            "Regra de São Bento; Papa São Gregório Magno", 6);
        q6.AddOption(Guid.NewGuid(), "São Bento de Núrsia", true, 1);
        q6.AddOption(Guid.NewGuid(), "Santo Antão do Deserto", false, 2);
        q6.AddOption(Guid.NewGuid(), "São Bernardo de Claraval", false, 3);
        q6.AddOption(Guid.NewGuid(), "São Bruno de Colônia", false, 4);
        quiz.AddQuestion(q6);

        var q7 = new Question(Guid.NewGuid(), quiz.Id,
            "Qual Concílio Ecumênico do século XVI respondeu à crise protestante, reafirmou os sete sacramentos e a Tradição apostólica?",
            "O Concílio de Trento (1545-1563), marco da verdadeira Reforma Católica.",
            "Concílio de Trento; CIC 9", 7);
        q7.AddOption(Guid.NewGuid(), "Concílio de Trento", true, 1);
        q7.AddOption(Guid.NewGuid(), "Concílio de Constança", false, 2);
        q7.AddOption(Guid.NewGuid(), "Concílio de Latrão IV", false, 3);
        q7.AddOption(Guid.NewGuid(), "Concílio Vaticano I", false, 4);
        quiz.AddQuestion(q7);

        var q8 = new Question(Guid.NewGuid(), quiz.Id,
            "Quem foi o primeiro mártir cristão registrado nos Atos dos Apóstolos (o Protomártir)?",
            "Santo Estêvão, diácono apedrejado em Jerusalém, que orou pelo perdão de seus algozes antes de expirar.",
            "At 7, 54-60; CIC 2473", 8);
        q8.AddOption(Guid.NewGuid(), "Santo Estêvão", true, 1);
        q8.AddOption(Guid.NewGuid(), "São Tiago Maior", false, 2);
        q8.AddOption(Guid.NewGuid(), "São Pedro", false, 3);
        q8.AddOption(Guid.NewGuid(), "São Lourenço", false, 4);
        quiz.AddQuestion(q8);

        var q9 = new Question(Guid.NewGuid(), quiz.Id,
            "Qual santa carmelita e doutora da Igreja escreveu 'História de uma Alma' e ensinou a 'Pequena Via' espiritual?",
            "Santa Teresinha do Menino Jesus (Teresa de Lisieux, 1873-1897), Doutora da Igreja.",
            "Manuscritos Autobiográficos; CIC 2011", 9);
        q9.AddOption(Guid.NewGuid(), "Santa Teresinha do Menino Jesus", true, 1);
        q9.AddOption(Guid.NewGuid(), "Santa Teresa de Ávila (de Jesus)", false, 2);
        q9.AddOption(Guid.NewGuid(), "Santa Catarina de Sena", false, 3);
        q9.AddOption(Guid.NewGuid(), "Santa Edith Stein (Teresa Benedita)", false, 4);
        quiz.AddQuestion(q9);

        var q10 = new Question(Guid.NewGuid(), quiz.Id,
            "Qual dogma foi solenemente definido no Concílio Vaticano I (1870) presidido pelo Papa Pio IX?",
            "A infalibilidade papal quando o Sumo Pontífice se pronuncia 'ex cathedra' sobre fé e costumes.",
            "Constituição Pastor Aeternus; CIC 891", 10);
        q10.AddOption(Guid.NewGuid(), "A Infalibilidade Papal ex cathedra", true, 1);
        q10.AddOption(Guid.NewGuid(), "A Maternidade Divina de Maria", false, 2);
        q10.AddOption(Guid.NewGuid(), "O Cânon dos 73 livros", false, 3);
        q10.AddOption(Guid.NewGuid(), "A Criação do Purgatório", false, 4);
        quiz.AddQuestion(q10);
    }

    private static void SeedLiturgiaQuestions(Quiz quiz)
    {
        var q1 = new Question(Guid.NewGuid(), quiz.Id,
            "Quais são as duas partes principais constitutivas da Santa Missa?",
            "A Liturgia da Palavra e a Liturgia Eucarística formam um só ato de culto tão intimamente unidas.",
            "CIC 1346; Sacrosanctum Concilium 56", 1);
        q1.AddOption(Guid.NewGuid(), "Liturgia da Palavra e Liturgia Eucarística", true, 1);
        q1.AddOption(Guid.NewGuid(), "Rito de Entrada e Comunhão dos Fiéis", false, 2);
        q1.AddOption(Guid.NewGuid(), "Leituras Bíblicas e Oração dos Fiéis", false, 3);
        q1.AddOption(Guid.NewGuid(), "Ofertório e Rito de Despedida", false, 4);
        quiz.AddQuestion(q1);

        var q2 = new Question(Guid.NewGuid(), quiz.Id,
            "Qual cor litúrgica é usada nos paramentos sacerdotais durante o Tempo Comum?",
            "O verde, simbolizando a esperança cristã e a vivência diária do Reino de Deus.",
            "Instrução Geral do Missal Romano (IGMR) 346", 2);
        q2.AddOption(Guid.NewGuid(), "Verde", true, 1);
        q2.AddOption(Guid.NewGuid(), "Branco", false, 2);
        q2.AddOption(Guid.NewGuid(), "Roxo", false, 3);
        q2.AddOption(Guid.NewGuid(), "Vermelho", false, 4);
        quiz.AddQuestion(q2);

        var q3 = new Question(Guid.NewGuid(), quiz.Id,
            "Qual cor litúrgica é usada durante os tempos penitenciais do Advento e da Quaresma?",
            "O roxo, simbolizando a penitência, a conversão e a expectativa confiante.",
            "IGMR 346; CIC 1438", 3);
        q3.AddOption(Guid.NewGuid(), "Roxo", true, 1);
        q3.AddOption(Guid.NewGuid(), "Preto", false, 2);
        q3.AddOption(Guid.NewGuid(), "Vermelho", false, 3);
        q3.AddOption(Guid.NewGuid(), "Dourado", false, 4);
        quiz.AddQuestion(q3);

        var q4 = new Question(Guid.NewGuid(), quiz.Id,
            "Qual é o tempo litúrgico que inicia o ano litúrgico católico, preparando a Igreja para a celebração do Natal?",
            "O Tempo do Advento, composto pelos quatro domingos que antecedem o Natal do Senhor.",
            "Normas Universais sobre o Ano Litúrgico; CIC 524", 4);
        q4.AddOption(Guid.NewGuid(), "Advento", true, 1);
        q4.AddOption(Guid.NewGuid(), "Quaresma", false, 2);
        q4.AddOption(Guid.NewGuid(), "Tempo Pascal", false, 3);
        q4.AddOption(Guid.NewGuid(), "Tempo Comum", false, 4);
        quiz.AddQuestion(q4);

        var q5 = new Question(Guid.NewGuid(), quiz.Id,
            "Qual o vaso sagrado litúrgico utilizado na Missa para guardar e consagrar o Preciosíssimo Sangue de Cristo?",
            "O Cálice sagrado, consagrado com óleo do Crisma para o uso exclusivo do sacrifício do altar.",
            "IGMR 327-330", 5);
        q5.AddOption(Guid.NewGuid(), "Cálice", true, 1);
        q5.AddOption(Guid.NewGuid(), "Âmbula (ou Cibório)", false, 2);
        q5.AddOption(Guid.NewGuid(), "Patena", false, 3);
        q5.AddOption(Guid.NewGuid(), "Galhetas", false, 4);
        quiz.AddQuestion(q5);

        var q6 = new Question(Guid.NewGuid(), quiz.Id,
            "O que é o Sacrário (Tabernáculo) presente nas igrejas católicas?",
            "O local sagrado, digno e seguro onde se reserva a Sagrada Eucaristia após a celebração da Missa.",
            "CIC 1379; IGMR 314", 6);
        q6.AddOption(Guid.NewGuid(), "O receptáculo onde o Santíssimo Sacramento fica guardado com honra", true, 1);
        q6.AddOption(Guid.NewGuid(), "O livro oficial das orações da Santa Missa", false, 2);
        q6.AddOption(Guid.NewGuid(), "A mesa de pedra onde são lidas as Sagradas Leituras", false, 3);
        q6.AddOption(Guid.NewGuid(), "O móvel onde o sacerdote atende as confissões", false, 4);
        quiz.AddQuestion(q6);

        var q7 = new Question(Guid.NewGuid(), quiz.Id,
            "Qual cântico de júbilo angelical é suprimido na liturgia durante todo o período da Quaresma?",
            "O 'Aleluia' (e também o 'Glória a Deus nas Alturas') é omitido como sinal de recolhimento penitencial.",
            "IGMR 62", 7);
        q7.AddOption(Guid.NewGuid(), "O Aleluia", true, 1);
        q7.AddOption(Guid.NewGuid(), "O Pai Nosso", false, 2);
        q7.AddOption(Guid.NewGuid(), "O Cordeiro de Deus (Agnus Dei)", false, 3);
        q7.AddOption(Guid.NewGuid(), "O Santo (Sanctus)", false, 4);
        quiz.AddQuestion(q7);

        var q8 = new Question(Guid.NewGuid(), quiz.Id,
            "O que significa a oração oficial da Igreja chamada 'Liturgia das Horas' (Ofício Divino)?",
            "A oração comunitária contínua de louvor e salmos distribuída pelas horas do dia para consagrar todo o tempo a Deus.",
            "CIC 1174-1178; Sacrosanctum Concilium 83", 8);
        q8.AddOption(Guid.NewGuid(), "A oração pública e contínua dos salmos consagrando as horas do dia a Deus", true, 1);
        q8.AddOption(Guid.NewGuid(), "Um conjunto particular de novenas populares para santos padroeiros", false, 2);
        q8.AddOption(Guid.NewGuid(), "A bênção com o Santíssimo Sacramento realizada nos fins de semana", false, 3);
        q8.AddOption(Guid.NewGuid(), "A leitura obrigatória do Catecismo antes de dormir", false, 4);
        quiz.AddQuestion(q8);

        var q9 = new Question(Guid.NewGuid(), quiz.Id,
            "Qual o maior período festivo do ano litúrgico, com 50 dias de júbilo até Pentecostes?",
            "O Tempo Pascal (a Cinquenta Pascal), onde se celebra com alegria a Ressurreição do Senhor.",
            "Normas do Ano Litúrgico 22; CIC 1168", 9);
        q9.AddOption(Guid.NewGuid(), "O Tempo Pascal", true, 1);
        q9.AddOption(Guid.NewGuid(), "O Tempo de Natal", false, 2);
        q9.AddOption(Guid.NewGuid(), "O Tempo Comum", false, 3);
        q9.AddOption(Guid.NewGuid(), "A Quaresma", false, 4);
        quiz.AddQuestion(q9);

        var q10 = new Question(Guid.NewGuid(), quiz.Id,
            "Qual pano de linho sagrado é estendido sobre o altar e sobre o qual se colocam a patena e o cálice na Missa?",
            "O Corporal, assim chamado porque sobre ele repousa o Corpo sacratíssimo de Nosso Senhor.",
            "IGMR 118, 306", 10);
        q10.AddOption(Guid.NewGuid(), "O Corporal", true, 1);
        q10.AddOption(Guid.NewGuid(), "O Sanguíneo", false, 2);
        q10.AddOption(Guid.NewGuid(), "A Pala", false, 3);
        q10.AddOption(Guid.NewGuid(), "O Véu de Cálice", false, 4);
        quiz.AddQuestion(q10);
    }

    private static void SeedCruzadasDefesaQuestions(Quiz quiz)
    {
        var q1 = new Question(Guid.NewGuid(), quiz.Id,
            "Em 1095, no Concílio de Clermont, qual Sumo Pontífice convocou a Cristandade para a Primeira Cruzada em defesa dos peregrinos e dos lugares santos?",
            "No Concílio de Clermont (1095), o Papa Urbano II atendeu aos apelos diante das perseguições e profanações aos lugares sagrados no Oriente, conclamando os nobres cristãos a protegerem seus irmãos na fé.",
            "Fulcher de Chartres, Gesta Francorum; Régine Pernoud, As Cruzadas", 1);
        q1.AddOption(Guid.NewGuid(), "Papa Urbano II", true, 1);
        q1.AddOption(Guid.NewGuid(), "Papa Inocêncio III", false, 2);
        q1.AddOption(Guid.NewGuid(), "Papa Gregório VII", false, 3);
        q1.AddOption(Guid.NewGuid(), "Papa Bonifácio VIII", false, 4);
        quiz.AddQuestion(q1);

        var q2 = new Question(Guid.NewGuid(), quiz.Id,
            "Qual era o brado sagrado proclamado pela assembleia e pelos cavaleiros ao atenderem ao apelo do Papa Urbano II?",
            "O brado unânime que ecoou na assembleia de Clermont foi 'Deus Vult!' ('Deus o quer!'), tornando-se a divisa sagrada dos guerreiros cruzados.",
            "Robert, o Monge, Historia Iherosolimitana; René Grousset", 2);
        q2.AddOption(Guid.NewGuid(), "Deus Vult (Deus o quer)", true, 1);
        q2.AddOption(Guid.NewGuid(), "Pax Romana (Paz Romana)", false, 2);
        q2.AddOption(Guid.NewGuid(), "Soli Deo Gloria (Glória somente a Deus)", false, 3);
        q2.AddOption(Guid.NewGuid(), "Veni, Vidi, Vici (Vim, vi e venci)", false, 4);
        quiz.AddQuestion(q2);

        var q3 = new Question(Guid.NewGuid(), quiz.Id,
            "Após a libertação de Jerusalém em 1099, qual nobre católico recusou usar uma coroa de ouro onde Cristo usara uma coroa de espinhos, adotando o título de 'Defensor do Santo Sepulcro'?",
            "Godofredo de Bouillon aceitou o encargo de governar e guardar Jerusalém, mas recusou o título régio, afirmando que não seria coroado de ouro onde o Redentor fora coroado de espinhos, intitulando-se 'Advocatus Sancti Sepulchri'.",
            "Guilherme de Tiro, Hist. Rer. Transmar.; Régine Pernoud, Os Homens da Cruzada", 3);
        q3.AddOption(Guid.NewGuid(), "Godofredo de Bouillon", true, 1);
        q3.AddOption(Guid.NewGuid(), "Boemundo de Taranto", false, 2);
        q3.AddOption(Guid.NewGuid(), "Raimundo IV de Toulouse", false, 3);
        q3.AddOption(Guid.NewGuid(), "Balduíno de Bolonha", false, 4);
        quiz.AddQuestion(q3);

        var q4 = new Question(Guid.NewGuid(), quiz.Id,
            "Por volta de 1118-1119, em Jerusalém, qual cavaleiro nobre francês fundou com companheiros a Ordem dos Pobres Cavaleiros de Cristo (Templários) para proteger as rotas de peregrinação?",
            "Hugues de Payens e seus oito primeiros companheiros fundaram a ordem militar e religiosa junto ao antigo Templo de Jerusalém, dedicando-se a manter as estradas seguras para os peregrinos desarmados.",
            "Guilherme de Tiro, Livro XII; Régine Pernoud, Os Templários", 4);
        q4.AddOption(Guid.NewGuid(), "Hugues de Payens", true, 1);
        q4.AddOption(Guid.NewGuid(), "Jacques de Molay", false, 2);
        q4.AddOption(Guid.NewGuid(), "Ricardo Coração de Leão", false, 3);
        q4.AddOption(Guid.NewGuid(), "São Luís IX", false, 4);
        quiz.AddQuestion(q4);

        var q5 = new Question(Guid.NewGuid(), quiz.Id,
            "Qual santo abade e Doutor da Igreja redigiu o célebre tratado 'De Laude Novae Militiae' (Elogio da Nova Cavalaria), conferindo sólida fundamentação teológica à Ordem dos Templários?",
            "São Bernardo de Claraval apoiou a Ordem no Concílio de Troyes (1129) e escreveu 'De Laude Novae Militiae', definindo a cavalaria cristã como uma consagração religiosa em defesa da paz e da justiça.",
            "São Bernardo de Claraval, De Laude Novae Militiae; Concílio de Troyes (1129)", 5);
        q5.AddOption(Guid.NewGuid(), "São Bernardo de Claraval", true, 1);
        q5.AddOption(Guid.NewGuid(), "São Tomás de Aquino", false, 2);
        q5.AddOption(Guid.NewGuid(), "Santo Anselmo de Cantuária", false, 3);
        q5.AddOption(Guid.NewGuid(), "São Boaventura", false, 4);
        quiz.AddQuestion(q5);

        var q6 = new Question(Guid.NewGuid(), quiz.Id,
            "Qual manto e emblema sagrado no peito distinguiam oficialmente os Cavaleiros Templários após a aprovação pontifícia?",
            "Os cavaleiros professos vestiam o manto branco, imagem da pureza virginal, ornado com a Cruz Pátea vermelha de oito pontas, testemunho da prontidão de derramar o sangue por Cristo.",
            "Bula Omne Datum Optimum (Papa Inocêncio II, 1139); Regra Primitiva do Templo", 6);
        q6.AddOption(Guid.NewGuid(), "Manto branco com a Cruz Pátea vermelha", true, 1);
        q6.AddOption(Guid.NewGuid(), "Manto preto com cruz amarela dourada", false, 2);
        q6.AddOption(Guid.NewGuid(), "Manto azul escuro com flor-de-lis prateada", false, 3);
        q6.AddOption(Guid.NewGuid(), "Manto carmesim com leão rampante", false, 4);
        quiz.AddQuestion(q6);

        var q7 = new Question(Guid.NewGuid(), quiz.Id,
            "Qual era o tríplice voto religioso professado pelos membros das ordens de cavalaria cristã?",
            "Os cavaleiros eram religiosos sob votos monásticos clássicos de Pobreza, Castidade e Obediência, vivendo em comunidade sob regra aprovada pelo Papa.",
            "Regra Latina dos Templários; Hilaire Belloc, The Crusades", 7);
        q7.AddOption(Guid.NewGuid(), "Pobreza, Castidade e Obediência", true, 1);
        q7.AddOption(Guid.NewGuid(), "Riqueza, Soberania e Conquista", false, 2);
        q7.AddOption(Guid.NewGuid(), "Silêncio Perpétuo, Reclusão e Jejum", false, 3);
        q7.AddOption(Guid.NewGuid(), "Lealdade feudal, Honra heráldica e Vingança", false, 4);
        quiz.AddQuestion(q7);

        var q8 = new Question(Guid.NewGuid(), quiz.Id,
            "Além dos Templários, qual ordem militar fundada em Jerusalém dedicava-se especialmente à hospitalidade e ao cuidado médico dos enfermos na Terra Santa, tornando-se mais tarde conhecida como Ordem de Malta?",
            "A Ordem dos Cavaleiros Hospitalários de São João de Jerusalém (hoje Ordem de Malta) aliou o serviço caritativo aos doentes com a guarda e defesa das fortalezas da Cristandade.",
            "Bula Pie Postulatio Voluntatis (Papa Pascoal II, 1113); Ernle Bradford", 8);
        q8.AddOption(Guid.NewGuid(), "Cavaleiros Hospitalários (Ordem de São João)", true, 1);
        q8.AddOption(Guid.NewGuid(), "Cavaleiros Teutônicos", false, 2);
        q8.AddOption(Guid.NewGuid(), "Ordem de Santiago da Espada", false, 3);
        q8.AddOption(Guid.NewGuid(), "Ordem de Calatrava", false, 4);
        quiz.AddQuestion(q8);

        var q9 = new Question(Guid.NewGuid(), quiz.Id,
            "Como renomados historiadores como Régine Pernoud e René Grousset definem a natureza primordial das Cruzadas medievais?",
            "Documentos históricos provam que as Cruzadas foram atos de legítima defesa e socorro fraterno aos cristãos do Oriente, com o intuito de restaurar o livre acesso aos Lugares Santos violados.",
            "Régine Pernoud, As Cruzadas; René Grousset, Histoire des Croisades", 9);
        q9.AddOption(Guid.NewGuid(), "Uma resposta defensiva e ato de socorro aos cristãos perseguidos, reabrindo o acesso aos Lugares Santos", true, 1);
        q9.AddOption(Guid.NewGuid(), "Uma expedição comercial secular voltada à pilhagem de rotas de especiarias", false, 2);
        q9.AddOption(Guid.NewGuid(), "Uma campanha absolutista para erradicar culturas sem motivação de fé", false, 3);
        q9.AddOption(Guid.NewGuid(), "Um projeto diplomático sem vínculos com a Sé Apostólica", false, 4);
        quiz.AddQuestion(q9);

        var q10 = new Question(Guid.NewGuid(), quiz.Id,
            "Qual decisiva batalha na Península Ibérica em 1212 reuniu os reinos católicos sob a bênção papal de Cruzada, quebrando o poderio almóada e salvaguardando a Cristandade hispânica?",
            "A Batalha de Navas de Tolosa (1212) foi o ápice da cooperação entre Afonso VIII de Castela, Pedro II de Aragão e Sancho VII de Navarra, marcando o ponto de virada definitivo da Reconquista.",
            "Rodrigo Jiménez de Rada, De Rebus Hispaniae; Alexandre Herculano", 10);
        q10.AddOption(Guid.NewGuid(), "Batalha de Navas de Tolosa", true, 1);
        q10.AddOption(Guid.NewGuid(), "Batalha de Guadalete", false, 2);
        q10.AddOption(Guid.NewGuid(), "Batalha de Aljubarrota", false, 3);
        q10.AddOption(Guid.NewGuid(), "Cerco de Granada de 1492", false, 4);
        quiz.AddQuestion(q10);
    }

    private static void SeedOrdemDeCristoQuestions(Quiz quiz)
    {
        var q1 = new Question(Guid.NewGuid(), quiz.Id,
            "No Concílio de Vienne (1312), através de qual bula pontifícia o Papa Clemente V decretou a extinção dos Templários no plano administrativo sem proferir condenação canônica definitiva por heresia?",
            "Na Bula 'Vox in Excelso', o Papa Clemente V expressa claramente que dissolve a Ordem por provisão e ordenação apostólica ('per viam provisionis'), para pacificar a Igreja diante das pressões da Coroa francesa, e não por sentença judicial definitiva condenando a regra ou a fé da instituição.",
            "Bula Vox in Excelso (1312); Pergaminho de Chinon (Vaticano); Régine Pernoud", 1);
        q1.AddOption(Guid.NewGuid(), "Bula Vox in Excelso", true, 1);
        q1.AddOption(Guid.NewGuid(), "Bula Unam Sanctam", false, 2);
        q1.AddOption(Guid.NewGuid(), "Bula Sublimis Deus", false, 3);
        q1.AddOption(Guid.NewGuid(), "Bula Exsurge Domine", false, 4);
        quiz.AddQuestion(q1);

        var q2 = new Question(Guid.NewGuid(), quiz.Id,
            "Qual monarca português defendeu com firmeza a inocência dos cavaleiros no seu reino e negociou com Roma para preservar seu patrimônio militar e espiritual em uma nova ordem lusitana?",
            "O Rei Dom Dinis I ('O Lavrador') recusou entregar os bens templários aos Hospitalários ou absorvê-los para o fisco, negociando pacientemente com a Santa Sé a fundação de uma nova milícia sagrada.",
            "Frei Luís de Sousa, História de S. Domingos; Alexandre Herculano", 2);
        q2.AddOption(Guid.NewGuid(), "Dom Dinis I (O Lavrador)", true, 1);
        q2.AddOption(Guid.NewGuid(), "Dom Afonso Henriques", false, 2);
        q2.AddOption(Guid.NewGuid(), "Dom João I", false, 3);
        q2.AddOption(Guid.NewGuid(), "Dom Manuel I (O Venturoso)", false, 4);
        quiz.AddQuestion(q2);

        var q3 = new Question(Guid.NewGuid(), quiz.Id,
            "Em 15 de março de 1319, qual Papa promulgou a histórica Bula 'Ad Ea Ex Quibus', instituindo a Ordem dos Cavaleiros de Cristo como legítima sucessora e herdeira universal dos bens e espírito dos Templários em Portugal?",
            "O Papa João XXII promulgou a Bula 'Ad Ea Ex Quibus' criando a 'Ordo Militiae Domini Nostri Jesu Christi' (Ordem de Cristo), transferindo-lhe todas as possessões, castelos e tradições templárias portuguesas sob a Regra de Avis e Cister.",
            "Bula Papal Ad Ea Ex Quibus (1319); Arquivo Nacional da Torre do Tombo", 3);
        q3.AddOption(Guid.NewGuid(), "Papa João XXII", true, 1);
        q3.AddOption(Guid.NewGuid(), "Papa Bonifácio VIII", false, 2);
        q3.AddOption(Guid.NewGuid(), "Papa Gregório XI", false, 3);
        q3.AddOption(Guid.NewGuid(), "Papa Urbano V", false, 4);
        quiz.AddQuestion(q3);

        var q4 = new Question(Guid.NewGuid(), quiz.Id,
            "Após ser fundada inicialmente em Castro Marim, qual majestosa fortaleza-convento tornou-se a sede definitiva da Ordem de Cristo em 1357, preservando a célebre Charola templária?",
            "O Convento de Cristo em Tomar, erguido dentro da muralha templária construída por D. Gualdim Pais em 1160, tornou-se o centro espiritual e militar da Ordem de Cristo.",
            "Conde de Sabugosa, O Castelo de Tomar; Alexandre Herculano", 4);
        q4.AddOption(Guid.NewGuid(), "Convento de Cristo em Tomar", true, 1);
        q4.AddOption(Guid.NewGuid(), "Mosteiro da Batalha", false, 2);
        q4.AddOption(Guid.NewGuid(), "Mosteiro de Alcobaça", false, 3);
        q4.AddOption(Guid.NewGuid(), "Torre de Belém", false, 4);
        quiz.AddQuestion(q4);

        var q5 = new Question(Guid.NewGuid(), quiz.Id,
            "Nomeado Governador e Administrador Perpétuo da Ordem de Cristo em 1420, qual príncipe português financiou a epopeia das Grandes Navegações e a Escola de Sagres com as comendas e rendas da Ordem?",
            "O Infante Dom Henrique ('O Navegador') assumiu a liderança da Ordem de Cristo, utilizando sua riqueza monástica e vocação de cavalaria para explorar os mares do Atlântico e expandir a Fé Católica.",
            "Gomes Eanes de Zurara, Crónica dos Feitos da Guiné; João de Barros", 5);
        q5.AddOption(Guid.NewGuid(), "Infante Dom Henrique (O Navegador)", true, 1);
        q5.AddOption(Guid.NewGuid(), "Dom Nuno Álvares Pereira", false, 2);
        q5.AddOption(Guid.NewGuid(), "Infante Dom Pedro (Duque de Coimbra)", false, 3);
        q5.AddOption(Guid.NewGuid(), "Dom Duarte I", false, 4);
        quiz.AddQuestion(q5);

        var q6 = new Question(Guid.NewGuid(), quiz.Id,
            "Por qual razão primordial as velas das caravelas e naus portuguesas ostentavam a insígnia da Cruz Pátea vermelha da Ordem de Cristo?",
            "Porque as viagens de exploração e descobrimento eram concebidas e financiadas juridicamente como uma missão e cruzada marítima da própria Ordem Militar de Cristo.",
            "João de Barros, Décadas da Ásia (Década I); Jaime Cortesão", 6);
        q6.AddOption(Guid.NewGuid(), "Porque as navegações eram oficialmente uma cruzada oceânica promovida e financiada pela Ordem de Cristo", true, 1);
        q6.AddOption(Guid.NewGuid(), "Porque servia como mero desenho geométrico para medir a direção dos ventos alísios", false, 2);
        q6.AddOption(Guid.NewGuid(), "Porque Portugal não possuía nenhum brasão oficial até o século XVII", false, 3);
        q6.AddOption(Guid.NewGuid(), "Por exigência puramente mercantil imposta por navegadores genoveses", false, 4);
        quiz.AddQuestion(q6);

        var q7 = new Question(Guid.NewGuid(), quiz.Id,
            "Qual bula solene promulgada pelo Papa Nicolau V em 1455 confiou à Coroa Portuguesa e à Ordem de Cristo o Padroado Real sobre as terras ultramarinas para erigir templos e propagar o Evangelho?",
            "A Bula 'Romanus Pontifex' de Nicolau V reconheceu os louváveis trabalhos do Infante D. Henrique e conferiu à Ordem de Cristo a jurisdição eclesiástica e o encargo sagrado de evangelizar as novas terras.",
            "Bula Romanus Pontifex (Papa Nicolau V, 1455); Monumenta Henricina", 7);
        q7.AddOption(Guid.NewGuid(), "Bula Romanus Pontifex", true, 1);
        q7.AddOption(Guid.NewGuid(), "Bula Inter Caetera", false, 2);
        q7.AddOption(Guid.NewGuid(), "Bula Aeterni Regis", false, 3);
        q7.AddOption(Guid.NewGuid(), "Bula Laudabiliter", false, 4);
        quiz.AddQuestion(q7);

        var q8 = new Question(Guid.NewGuid(), quiz.Id,
            "Ao desembarcar na costa brasileira em 22 de abril de 1500, que nome eminentemente cristão Pedro Álvares Cabral, fidalgo cavaleiro da Ordem de Cristo, conferiu à terra descoberta?",
            "Em reverência ao Santo Lenho e à Ordem sob cujo estandarte navegava, Cabral nomeou a terra como Ilha de Vera Cruz, logo chamada Terra de Santa Cruz.",
            "Carta de Pero Vaz de Caminha a el-Rei Dom Manuel I (1500)", 8);
        q8.AddOption(Guid.NewGuid(), "Terra de Santa Cruz (inicialmente Ilha de Vera Cruz)", true, 1);
        q8.AddOption(Guid.NewGuid(), "Reino do Pau-Brasil", false, 2);
        q8.AddOption(Guid.NewGuid(), "Nova Província Lusitana", false, 3);
        q8.AddOption(Guid.NewGuid(), "Terra dos Papagaios", false, 4);
        quiz.AddQuestion(q8);

        var q9 = new Question(Guid.NewGuid(), quiz.Id,
            "No Domingo da Pascoela de 1500 (26 de abril), quem celebrou a histórica Primeira Missa no Brasil sob o dossel e a Cruz da Ordem de Cristo na praia de Porto Seguro?",
            "Frei Henrique Soares de Coimbra, frade menor franciscano embarcado na frota cabralina, celebrou a primeira Eucaristia no altar erguido na praia diante da grande Cruz fincada no solo da Terra de Santa Cruz.",
            "Carta de Pero Vaz de Caminha; Frei Manuel da Esperança, História Seráfica", 9);
        q9.AddOption(Guid.NewGuid(), "Frei Henrique de Coimbra, OFM", true, 1);
        q9.AddOption(Guid.NewGuid(), "Padre José de Anchieta, SJ", false, 2);
        q9.AddOption(Guid.NewGuid(), "Padre Manuel da Nóbrega, SJ", false, 3);
        q9.AddOption(Guid.NewGuid(), "Frei Bartolomeu dos Mártires, OP", false, 4);
        quiz.AddQuestion(q9);

        var q10 = new Question(Guid.NewGuid(), quiz.Id,
            "Qual versículo do Salmo 113 constituía a imortal divisa dos Cavaleiros Templários, preservada com profunda honra pela Ordem de Cristo?",
            "A divisa dos cavaleiros de Cristo era 'Non nobis, Domine, non nobis, sed nomini Tuo da gloriam' (Salmo 113, 9), lembrando que toda vitória e todo fruto provêm unicamente da graça divina.",
            "Salmo 113, 9 (Vulgata); Regra da Ordem de Cristo; Hilaire Belloc", 10);
        q10.AddOption(Guid.NewGuid(), "Non nobis, Domine, non nobis, sed nomini Tuo da gloriam", true, 1);
        q10.AddOption(Guid.NewGuid(), "In hoc signo vinces", false, 2);
        q10.AddOption(Guid.NewGuid(), "Fortis cadere, cedere non potest", false, 3);
        q10.AddOption(Guid.NewGuid(), "Memento mori et carpe diem", false, 4);
        quiz.AddQuestion(q10);
    }

    private static void SeedCavalariaBatalhasQuestions(Quiz quiz)
    {
        var q1 = new Question(Guid.NewGuid(), quiz.Id,
            "Em 'De Laude Novae Militiae', qual profunda distinção espiritual São Bernardo de Claraval estabelece entre a 'militia' secular (que chama de malitia) e a Nova Cavalaria de Cristo?",
            "São Bernardo ensina que o guerreiro mundano é cativo da vanglória, do ódio e da rapina passageira; já o cavaleiro de Cristo combate sob disciplina monástica, purificado pelo amor a Deus e pela caridade de defender os irmãos indefesos.",
            "São Bernardo de Claraval, De Laude Novae Militiae, cap. II e III", 1);
        q1.AddOption(Guid.NewGuid(), "A cavalaria mundana é movida por vaidade e cobiça; a Cavalaria de Cristo combate por amor a Deus e legítima defesa sob disciplina de fé", true, 1);
        q1.AddOption(Guid.NewGuid(), "A cavalaria secular é perfeita em santidade, enquanto a monástica é considerada contrária ao Evangelho", false, 2);
        q1.AddOption(Guid.NewGuid(), "O cavaleiro cristão deve recusar a defesa armada mesmo quando inocentes e igrejas estão sob massacre", false, 3);
        q1.AddOption(Guid.NewGuid(), "A Nova Cavalaria busca instituir reinos temporais independentes e desvinculados da autoridade pontifícia", false, 4);
        quiz.AddQuestion(q1);

        var q2 = new Question(Guid.NewGuid(), quiz.Id,
            "Qual Mestre Templário em Portugal liderou com genialidade militar a construção do Castelo de Tomar em 1160 e sua defesa intransigente durante o cerco de 1190 perpetrado pelo califa almóada Abu Yaqub Yusuf?",
            "Dom Gualdim Pais, veterano das Cruzadas na Palestina e irmão de armas de D. Afonso Henriques, construiu Tomar com inovações defensivas cruzadas e derrotou o formidável exército almóada no histórico cerco de 1190.",
            "Inscrição lapidar de Tomar (1160); Alexandre Herculano, História de Portugal", 2);
        q2.AddOption(Guid.NewGuid(), "Dom Gualdim Pais", true, 1);
        q2.AddOption(Guid.NewGuid(), "Dom Gil Martins", false, 2);
        q2.AddOption(Guid.NewGuid(), "Dom Lourenço Martins", false, 3);
        q2.AddOption(Guid.NewGuid(), "Dom Paio Mendes", false, 4);
        quiz.AddQuestion(q2);

        var q3 = new Question(Guid.NewGuid(), quiz.Id,
            "Na Bula 'Vox in Excelso' (1312), qual termo do direito canônico empregado por Clemente V comprova que a dissolução do Templo não foi uma condenação jurídica de heresia corporativa da Ordem?",
            "Clemente V registrou textualmente que agia 'non per modum definitivae sententiae, sed per viam provisionis seu ordinationis apostolicae', explicitando que extinguia a corporação por medida administrativa e prudencial, e não por comprovação jurídica de heresia institucional.",
            "Bula Vox in Excelso, Regesta Vaticana; Régine Pernoud, Os Templários", 3);
        q3.AddOption(Guid.NewGuid(), "Non per modum definitivae sententiae, sed per viam provisionis (por provisão e não por sentença definitiva)", true, 1);
        q3.AddOption(Guid.NewGuid(), "Ex cathedra et jure divino condenando a Regra como apostasia comprovada", false, 2);
        q3.AddOption(Guid.NewGuid(), "Sub poena anathematis transferindo o julgamento final à coroa secular francesa", false, 3);
        q3.AddOption(Guid.NewGuid(), "Per sententiam criminalem atestando a culpabilidade de todos os cavaleiros cristãos", false, 4);
        quiz.AddQuestion(q3);

        var q4 = new Question(Guid.NewGuid(), quiz.Id,
            "Quem foi solenemente investido como o primeiro Grão-Mestre da Ordem Militar de Cristo pela Bula 'Ad Ea Ex Quibus' do Papa João XXII em 1319?",
            "Dom Frei Gil Martins, que era mestre da Ordem de Avis, foi nomeado por D. Dinis e aprovado pelo Papa como primeiro Grão-Mestre da Ordem de Cristo, garantindo a transição e fidelidade monástica cisterciense.",
            "Bula Ad Ea Ex Quibus (1319); Frei Luís de Sousa; Torre do Tombo", 4);
        q4.AddOption(Guid.NewGuid(), "Dom Frei Gil Martins", true, 1);
        q4.AddOption(Guid.NewGuid(), "Dom Nuno Álvares Pereira", false, 2);
        q4.AddOption(Guid.NewGuid(), "Dom Álvaro Gonçalves Camelo", false, 3);
        q4.AddOption(Guid.NewGuid(), "Dom João Fernandes Pacheco", false, 4);
        quiz.AddQuestion(q4);

        var q5 = new Question(Guid.NewGuid(), quiz.Id,
            "No memorável Grande Cerco de Malta em 1565, qual Grão-Mestre dos Cavaleiros Hospitalários liderou cerca de 500 irmãos de armas resistindo ao assédio de mais de 35.000 otomanos enviados por Solimão, o Magnífico?",
            "Jean Parisot de La Valette, septuagenário, comandou com bravura inabalável a defesa dos Fortes de Santo Elmo, Santo Ângelo e São Miguel, infligindo uma derrota decisiva à armada otomana. A cidade de Valletta foi erguida em sua memória.",
            "Giacomo Bosio, Dell'Istoria della Sacra Religione; Ernle Bradford, The Great Siege", 5);
        q5.AddOption(Guid.NewGuid(), "Jean Parisot de La Valette", true, 1);
        q5.AddOption(Guid.NewGuid(), "Philippe Villiers de L'Isle-Adam", false, 2);
        q5.AddOption(Guid.NewGuid(), "Pierre d'Aubusson", false, 3);
        q5.AddOption(Guid.NewGuid(), "Fabrizio del Carretto", false, 4);
        quiz.AddQuestion(q5);

        var q6 = new Question(Guid.NewGuid(), quiz.Id,
            "Na Batalha de Lepanto (7 de outubro de 1571), que Papa santo formou a Santa Liga, pediu jejuns e orações do Santo Rosário a toda a Cristandade e instituiu a celebração litúrgica de Nossa Senhora do Rosário?",
            "O Papa São Pio V foi o grande arquiteto da Santa Liga que congregou Espanha, Veneza e a Santa Sé. Durante a batalha em Lepanto, o Pontífice teve uma visão da vitória naval em Roma e instituiu a festa da Santa Mãe de Deus sob o título do Rosário.",
            "Bula Salvatoris Domini (Papa São Pio V, 1572); Ludwig von Pastor", 6);
        q6.AddOption(Guid.NewGuid(), "Papa São Pio V", true, 1);
        q6.AddOption(Guid.NewGuid(), "Papa Gregório XIII", false, 2);
        q6.AddOption(Guid.NewGuid(), "Papa Paulo IV", false, 3);
        q6.AddOption(Guid.NewGuid(), "Papa Sixto V", false, 4);
        quiz.AddQuestion(q6);

        var q7 = new Question(Guid.NewGuid(), quiz.Id,
            "Quem exerceu o comando supremo (Capitão-General do Mar) da frota católica confederada na Batalha de Lepanto, liderando a nau capitânia 'Real' contra a 'Sultana' de Ali Paxá?",
            "Dom João de Áustria (Don Juan de Austria), então com 24 anos, recebeu do Papa São Pio V o estandarte de Cristo crucificado e comandou com audácia as galés cristãs na vitória mais decisiva da história naval moderna.",
            "René Grousset; Hilaire Belloc, Europe and the Faith", 7);
        q7.AddOption(Guid.NewGuid(), "Dom João de Áustria (Don Juan de Austria)", true, 1);
        q7.AddOption(Guid.NewGuid(), "Sebastiano Venier", false, 2);
        q7.AddOption(Guid.NewGuid(), "Marcantonio Colonna", false, 3);
        q7.AddOption(Guid.NewGuid(), "Álvaro de Bazán", false, 4);
        quiz.AddQuestion(q7);

        var q8 = new Question(Guid.NewGuid(), quiz.Id,
            "Na Batalha de Viena (12 de setembro de 1683), que rei católico polonês comandou a lendária carga dos Hussardos Alados que rompeu o cerco turco de Kara Mustafá, enviando ao Papa a mensagem 'Viemos, vimos, Deus venceu'?",
            "O Rei João III Sobieski, após a Santa Missa celebrada pelo Bem-Aventurado Marco d'Aviano, liderou a maior investida de cavalaria da Europa, libertando Viena e preservando o coração cristão da Europa.",
            "Hilaire Belloc; Salvador de Madariaga; Epistolário do Rei João Sobieski a Inocêncio XI", 8);
        q8.AddOption(Guid.NewGuid(), "Rei João III Sobieski", true, 1);
        q8.AddOption(Guid.NewGuid(), "Imperador Leopoldo I", false, 2);
        q8.AddOption(Guid.NewGuid(), "Príncipe Eugênio de Sabóia", false, 3);
        q8.AddOption(Guid.NewGuid(), "Carlos V de Lorena", false, 4);
        quiz.AddQuestion(q8);

        var q9 = new Question(Guid.NewGuid(), quiz.Id,
            "Durante a conquista de Lisboa em 1147, com o auxílio de cruzados a caminho da Terra Santa, qual fidalgo cavaleiro entregou a própria vida atravessando-se nos portões do Castelo de São Jorge para permitir a entrada dos cristãos?",
            "Martim Moniz sacrificou o próprio corpo contra os batentes de ferro do castelo mouro para evitar que os defensores o fechassem, permitindo o avanço triunfante das tropas de D. Afonso Henriques.",
            "Crónica de 1419; Frei António Brandão, Monarquia Lusitana; De expugnatione Lyxbonensi", 9);
        q9.AddOption(Guid.NewGuid(), "Martim Moniz", true, 1);
        q9.AddOption(Guid.NewGuid(), "Dom Egas Moniz", false, 2);
        q9.AddOption(Guid.NewGuid(), "Mem Ramires", false, 3);
        q9.AddOption(Guid.NewGuid(), "Gonçalo Mendes da Maia", false, 4);
        quiz.AddQuestion(q9);

        var q10 = new Question(Guid.NewGuid(), quiz.Id,
            "Na espiritualidade de cavalaria da Ordem de Cristo, qual era a divisa pessoal em francês antigo cunhada pelo Infante Dom Henrique para expressar sua total fidelidade ao cumprimento do dever cristão?",
            "A divisa 'Talant de bien faire' ('Vontade/desejo inabalável de fazer o bem') figurava nas armas e insígnias do Infante D. Henrique, resumindo sua dedicação incansável à honra de Cristo, à ciência náutica e à salvação das almas no além-mar.",
            "Gomes Eanes de Zurara, Crónica dos Feitos da Guiné, cap. IV; João de Barros", 10);
        q10.AddOption(Guid.NewGuid(), "Talant de bien faire (Vontade de bem fazer)", true, 1);
        q10.AddOption(Guid.NewGuid(), "Dieu et mon droit (Deus e meu direito)", false, 2);
        q10.AddOption(Guid.NewGuid(), "Honni soit qui mal y pense (Envergonhe-se quem nisso vir mal)", false, 3);
        q10.AddOption(Guid.NewGuid(), "Je maintiendrai (Eu sustentarei)", false, 4);
        quiz.AddQuestion(q10);
    }
}
