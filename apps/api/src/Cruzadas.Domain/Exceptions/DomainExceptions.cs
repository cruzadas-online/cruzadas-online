namespace Cruzadas.Domain.Exceptions;

public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }
}

public class QuizNotPublishedException : DomainException
{
    public QuizNotPublishedException(string slug) 
        : base($"O quiz '{slug}' não está publicado.") { }
}

public class AttemptAlreadyCompletedException : DomainException
{
    public AttemptAlreadyCompletedException(Guid attemptId) 
        : base($"A tentativa '{attemptId}' já foi finalizada.") { }
}

public class InvalidAttemptAnswerException : DomainException
{
    public InvalidAttemptAnswerException(string message) : base(message) { }
}
