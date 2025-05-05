namespace EasyProcedure.Exceptions;

public class InvalidBotConfigException(string? message = null) : Exception(message);

public class InvalidLanguageTitleException(string? message = null) : InvalidBotConfigException(message);

public class DuplicateLanguageException(string? message = null) : InvalidBotConfigException(message);