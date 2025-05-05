namespace EasyProcedure.Exceptions;

public class NotSpecifiedException(string? message = null) : InvalidBotConfigException(message);

public class NoSupportedLanguageSpecifiedException(string? message = null) : NotSpecifiedException(message);

public class NoTextSpecifiedForLanguageException(string? message = null) : NotSpecifiedException(message);

public class NoIdSpecifiedException(string? message = null) : NotSpecifiedException(message);

public class NoPreviousStageSpecifiedException(string? message = null) : NotSpecifiedException(message);

public class NoButtonSpecifiedException(string? message = null) : NotSpecifiedException(message);

public class NoRootButtonSpecifiedException(string? message = null) : NoButtonSpecifiedException(message);

public class NoPreviousButtonSpecifiedException(string? message = null) : NoButtonSpecifiedException(message);