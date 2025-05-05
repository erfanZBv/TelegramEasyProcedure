namespace EasyProcedure.Exceptions;

public class EntityNotExistException(string? message = null) : InvalidBotConfigException(message);

public class ButtonNotExistException(string? message = null) : EntityNotExistException(message);

public class StageNotExistException(string? message = null) : EntityNotExistException(message);

public class OptionNotExistException(string? message = null) : EntityNotExistException(message);