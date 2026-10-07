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
}
