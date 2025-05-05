namespace EasyProcedure.Exceptions;

public class IdDuplicationException(string? message = null) : InvalidBotConfigException(message);