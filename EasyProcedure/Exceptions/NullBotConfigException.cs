namespace EasyProcedure.Exceptions;

public class NullBotConfigException(string? message = null) : NullReferenceException(message);